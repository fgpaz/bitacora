param(
    [string]$EnvFile = (Join-Path $PSScriptRoot "..\.env"),
    [string]$BaseUrl,
    [string]$JwtSecret,
    [string]$SmokeSub,
    [string]$SmokeEmail,
    [string]$ResolveIp,
    [switch]$SkipCertificateCheck
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Import-EnvFile {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        return @{}
    }

    $values = @{}
    foreach ($line in Get-Content -Path $Path) {
        if ([string]::IsNullOrWhiteSpace($line)) {
            continue
        }

        $trimmed = $line.Trim()
        if ($trimmed.StartsWith("#")) {
            continue
        }

        $parts = $trimmed -split "=", 2
        if ($parts.Count -ne 2) {
            continue
        }

        $values[$parts[0]] = $parts[1]
    }

    return $values
}

function Get-Setting {
    param(
        [hashtable]$EnvValues,
        [string]$Name,
        [string]$Fallback = ""
    )

    $direct = Get-Variable -Name $Name -ValueOnly -ErrorAction SilentlyContinue
    if (-not [string]::IsNullOrWhiteSpace($direct)) {
        return $direct
    }

    $fromEnv = [Environment]::GetEnvironmentVariable($Name)
    if (-not [string]::IsNullOrWhiteSpace($fromEnv)) {
        return $fromEnv
    }

    if ($EnvValues.ContainsKey($Name) -and -not [string]::IsNullOrWhiteSpace($EnvValues[$Name])) {
        return $EnvValues[$Name]
    }

    return $Fallback
}

function Get-BooleanSetting {
    param(
        [hashtable]$EnvValues,
        [string]$Name,
        [bool]$Fallback = $false
    )

    $rawValue = Get-Setting -EnvValues $EnvValues -Name $Name -Fallback ""
    if ([string]::IsNullOrWhiteSpace($rawValue)) {
        return $Fallback
    }

    switch ($rawValue.Trim().ToLowerInvariant()) {
        { $_ -in @("1", "true", "yes", "y", "on") } { return $true }
        { $_ -in @("0", "false", "no", "n", "off") } { return $false }
        default { throw "Setting '$Name' must be a boolean value." }
    }
}

function ConvertTo-Base64Url {
    param([byte[]]$Bytes)

    return [Convert]::ToBase64String($Bytes).TrimEnd("=").Replace("+", "-").Replace("/", "_")
}

function New-SmokeJwt {
    param(
        [string]$Secret,
        [string]$Subject,
        [string]$EmailAddress
    )

    $now = [DateTimeOffset]::UtcNow
    $headerJson = '{"alg":"HS256","typ":"JWT"}'
    $payloadJson = @{
        sub   = $Subject
        email = $EmailAddress
        iat   = $now.ToUnixTimeSeconds()
        exp   = $now.AddMinutes(15).ToUnixTimeSeconds()
    } | ConvertTo-Json -Compress

    $header = ConvertTo-Base64Url ([Text.Encoding]::UTF8.GetBytes($headerJson))
    $payload = ConvertTo-Base64Url ([Text.Encoding]::UTF8.GetBytes($payloadJson))
    $unsigned = "$header.$payload"

    $hmac = [System.Security.Cryptography.HMACSHA256]::new([Text.Encoding]::UTF8.GetBytes($Secret))
    try {
        $signatureBytes = $hmac.ComputeHash([Text.Encoding]::UTF8.GetBytes($unsigned))
    }
    finally {
        $hmac.Dispose()
    }

    $signature = ConvertTo-Base64Url $signatureBytes
    return "$unsigned.$signature"
}

function Invoke-Step {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers = @{},
        [object]$Body = $null,
        [int[]]$ExpectedStatusCodes = @(200),
        [string]$ResolvedIp = "",
        [bool]$AllowInvalidCertificate = $false
    )

    $content = $null

    if ([string]::IsNullOrWhiteSpace($ResolvedIp) -and -not $AllowInvalidCertificate) {
        $invokeArgs = @{
            Method             = $Method
            Uri                = $Url
            Headers            = $Headers
            SkipHttpErrorCheck = $true
        }

        if ($null -ne $Body) {
            $invokeArgs["Body"] = ($Body | ConvertTo-Json -Depth 10 -Compress)
            $invokeArgs["ContentType"] = "application/json"
        }

        $response = Invoke-WebRequest @invokeArgs
        $statusCode = [int]$response.StatusCode

        if (-not [string]::IsNullOrWhiteSpace($response.Content)) {
            try {
                $content = $response.Content | ConvertFrom-Json
            }
            catch {
                $content = $response.Content
            }
        }
    }
    else {
        $uri = [Uri]$Url
        $port = if ($uri.IsDefaultPort) {
            if ($uri.Scheme -eq "https") { 443 } else { 80 }
        }
        else {
            $uri.Port
        }

        $curlArgs = New-Object System.Collections.Generic.List[string]
        $curlArgs.Add("-sS")
        $curlArgs.Add("-X")
        $curlArgs.Add($Method)
        $curlArgs.Add("-H")
        $curlArgs.Add("Accept: application/json")

        if ($AllowInvalidCertificate) {
            $curlArgs.Add("-k")
        }

        foreach ($header in $Headers.GetEnumerator()) {
            $curlArgs.Add("-H")
            $curlArgs.Add("$($header.Key): $($header.Value)")
        }

        $tempBodyFile = $null
        try {
            if ($null -ne $Body) {
                $tempBodyFile = New-TemporaryFile
                $bodyJson = $Body | ConvertTo-Json -Depth 10 -Compress
                Set-Content -Path $tempBodyFile -Value $bodyJson -NoNewline
                $curlArgs.Add("-H")
                $curlArgs.Add("Content-Type: application/json")
                $curlArgs.Add("--data-binary")
                $curlArgs.Add("@$tempBodyFile")
            }

            if (-not [string]::IsNullOrWhiteSpace($ResolvedIp)) {
                $curlArgs.Add("--resolve")
                $curlArgs.Add(("{0}:{1}:{2}" -f $uri.Host, $port, $ResolvedIp))
            }

            $curlArgs.Add("-w")
            $curlArgs.Add("`n__STATUS__:%{http_code}")
            $curlArgs.Add($Url)

            $rawOutput = (& curl.exe @curlArgs 2>&1) -join [Environment]::NewLine
            if ($LASTEXITCODE -ne 0) {
                throw "curl.exe failed for step '$Name' with exit code $LASTEXITCODE.`n$rawOutput"
            }

            $marker = "__STATUS__:"
            $markerIndex = $rawOutput.LastIndexOf($marker)
            if ($markerIndex -lt 0) {
                throw "curl.exe output for step '$Name' did not include an HTTP status marker."
            }

            $bodyText = $rawOutput.Substring(0, $markerIndex).Trim()
            $statusText = $rawOutput.Substring($markerIndex + $marker.Length).Trim()
            $statusCode = [int]$statusText

            if (-not [string]::IsNullOrWhiteSpace($bodyText)) {
                try {
                    $content = $bodyText | ConvertFrom-Json
                }
                catch {
                    $content = $bodyText
                }
            }
        }
        finally {
            if ($null -ne $tempBodyFile -and (Test-Path $tempBodyFile)) {
                Remove-Item -Path $tempBodyFile -Force
            }
        }
    }

    if ($ExpectedStatusCodes -notcontains $statusCode) {
        throw "Step '$Name' failed. Expected [$($ExpectedStatusCodes -join ', ')], got $statusCode."
    }

    [pscustomobject]@{
        Name       = $Name
        StatusCode = $statusCode
        Body       = $content
    }
}

$envValues = Import-EnvFile -Path $EnvFile
$resolvedBaseUrl = (Get-Setting -EnvValues $envValues -Name "BaseUrl" -Fallback "")
if ([string]::IsNullOrWhiteSpace($resolvedBaseUrl)) {
    $resolvedBaseUrl = Get-Setting -EnvValues $envValues -Name "BITACORA_BASE_URL" -Fallback "http://localhost:5254"
}

$resolvedJwtSecret = Get-Setting -EnvValues $envValues -Name "JwtSecret" -Fallback ""
if ([string]::IsNullOrWhiteSpace($resolvedJwtSecret)) {
    $resolvedJwtSecret = Get-Setting -EnvValues $envValues -Name "SUPABASE_JWT_SECRET"
}

$resolvedSmokeSub = Get-Setting -EnvValues $envValues -Name "SmokeSub" -Fallback ""
if ([string]::IsNullOrWhiteSpace($resolvedSmokeSub)) {
    $resolvedSmokeSub = Get-Setting -EnvValues $envValues -Name "BITACORA_SMOKE_SUB"
}

$resolvedSmokeEmail = Get-Setting -EnvValues $envValues -Name "SmokeEmail" -Fallback ""
if ([string]::IsNullOrWhiteSpace($resolvedSmokeEmail)) {
    $resolvedSmokeEmail = Get-Setting -EnvValues $envValues -Name "BITACORA_SMOKE_EMAIL"
}

$resolvedIp = Get-Setting -EnvValues $envValues -Name "ResolveIp" -Fallback ""
if ([string]::IsNullOrWhiteSpace($resolvedIp)) {
    $resolvedIp = Get-Setting -EnvValues $envValues -Name "BITACORA_SMOKE_RESOLVE_IP"
}

$allowInvalidCertificate = $SkipCertificateCheck.IsPresent
if (-not $allowInvalidCertificate) {
    $allowInvalidCertificate = Get-BooleanSetting -EnvValues $envValues -Name "BITACORA_SMOKE_SKIP_CERT_CHECK" -Fallback $false
}

if ([string]::IsNullOrWhiteSpace($resolvedSmokeSub)) {
    $resolvedSmokeSub = [guid]::NewGuid().ToString()
}

if ([string]::IsNullOrWhiteSpace($resolvedSmokeEmail)) {
    $suffix = $resolvedSmokeSub.Replace("-", "")
    $resolvedSmokeEmail = "smoke+$suffix@bitacora.nuestrascuentitas.com"
}

if ([string]::IsNullOrWhiteSpace($resolvedJwtSecret)) {
    throw "SUPABASE_JWT_SECRET is required for the smoke run."
}

$jwt = New-SmokeJwt -Secret $resolvedJwtSecret -Subject $resolvedSmokeSub -EmailAddress $resolvedSmokeEmail
$authHeaders = @{ Authorization = "Bearer $jwt" }
$results = New-Object System.Collections.Generic.List[object]

$results.Add((Invoke-Step -Name "health" -Method "GET" -Url "$resolvedBaseUrl/health" -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))
$results.Add((Invoke-Step -Name "ready" -Method "GET" -Url "$resolvedBaseUrl/health/ready" -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))
$results.Add((Invoke-Step -Name "bootstrap" -Method "POST" -Url "$resolvedBaseUrl/api/v1/auth/bootstrap" -Headers $authHeaders -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))

$currentConsent = Invoke-Step -Name "consent-current" -Method "GET" -Url "$resolvedBaseUrl/api/v1/consent/current" -Headers $authHeaders -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate
$results.Add($currentConsent)

$negativeMood = Invoke-Step -Name "mood-without-consent" -Method "POST" -Url "$resolvedBaseUrl/api/v1/mood-entries" -Headers $authHeaders -Body @{ score = 1 } -ExpectedStatusCodes @(403) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate
$results.Add($negativeMood)

if ($negativeMood.Body.error.code -ne "CONSENT_REQUIRED") {
    throw "Expected CONSENT_REQUIRED before granting consent."
}

$consentVersion = [string]$currentConsent.Body.version
if ([string]::IsNullOrWhiteSpace($consentVersion)) {
    throw "Consent version missing from current consent response."
}

$results.Add((Invoke-Step -Name "grant-consent" -Method "POST" -Url "$resolvedBaseUrl/api/v1/consent" -Headers $authHeaders -Body @{ version = $consentVersion; accepted = $true } -ExpectedStatusCodes @(201, 409) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))
$results.Add((Invoke-Step -Name "mood-with-consent" -Method "POST" -Url "$resolvedBaseUrl/api/v1/mood-entries" -Headers $authHeaders -Body @{ score = 1 } -ExpectedStatusCodes @(200, 201) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))
$results.Add((Invoke-Step -Name "daily-checkin" -Method "POST" -Url "$resolvedBaseUrl/api/v1/daily-checkins" -Headers $authHeaders -Body @{
        sleepHours      = 8
        physicalActivity = $false
        socialActivity   = $true
        anxiety          = $false
        irritability     = $false
        medicationTaken  = $false
        medicationTime   = $null
    } -ExpectedStatusCodes @(200, 201) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))

# =====================================================================
# NEW SURFACES — Vinculos (patient-facing)
# =====================================================================

$results.Add((Invoke-Step -Name "vinculos-list" -Method "GET" -Url "$resolvedBaseUrl/api/v1/vinculos" -Headers $authHeaders -ExpectedStatusCodes @(200) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))
$results.Add((Invoke-Step -Name "vinculos-active" -Method "GET" -Url "$resolvedBaseUrl/api/v1/vinculos/active" -Headers $authHeaders -ExpectedStatusCodes @(200) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))

# =====================================================================
# NEW SURFACES — Visualizacion (patient-facing)
# =====================================================================

$fromDate = [DateOnly]::new((Get-Date).Year, (Get-Date).Month, 1).ToString("yyyy-MM-dd")
$toDate = [DateOnly]::new((Get-Date).Year, (Get-Date).Month, [Math]::Min((Get-Date).Day, 28)).ToString("yyyy-MM-dd")
$results.Add((Invoke-Step -Name "visualizacion-timeline" -Method "GET" -Url "$resolvedBaseUrl/api/v1/visualizacion/timeline?from=$fromDate&to=$toDate" -Headers $authHeaders -ExpectedStatusCodes @(200) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))
$results.Add((Invoke-Step -Name "visualizacion-summary" -Method "GET" -Url "$resolvedBaseUrl/api/v1/visualizacion/summary?from=$fromDate&to=$toDate" -Headers $authHeaders -ExpectedStatusCodes @(200) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))

# =====================================================================
# NEW SURFACES — Export (patient-facing)
# =====================================================================

$results.Add((Invoke-Step -Name "export-patient-summary" -Method "GET" -Url "$resolvedBaseUrl/api/v1/export/patient-summary?from=$fromDate&to=$toDate" -Headers $authHeaders -ExpectedStatusCodes @(200) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))
$results.Add((Invoke-Step -Name "export-patient-summary-csv" -Method "GET" -Url "$resolvedBaseUrl/api/v1/export/patient-summary/csv?from=$fromDate&to=$toDate" -Headers $authHeaders -ExpectedStatusCodes @(200) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))

# =====================================================================
# NEW SURFACES — Telegram pairing (requires consent + patient auth)
# =====================================================================

$results.Add((Invoke-Step -Name "telegram-pairing" -Method "POST" -Url "$resolvedBaseUrl/api/v1/telegram/pairing" -Headers $authHeaders -ExpectedStatusCodes @(200) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))
$results.Add((Invoke-Step -Name "telegram-session" -Method "GET" -Url "$resolvedBaseUrl/api/v1/telegram/session" -Headers $authHeaders -ExpectedStatusCodes @(200) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))

# =====================================================================
# NEW SURFACES — Telegram webhook (X-Telegram-Webhook-Secret auth)
# NOTE: Webhook smoke uses the secret token; if BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN
# is not configured the endpoint still returns HTTP 200. Secret validation is fail-closed
# (business-logic): missing or mismatched secret returns HTTP 200 with Accepted=false and
# ErrorCode=FORBIDDEN — the update is never dispatched. Only when expectedToken is null/empty
# (not configured) is validation skipped entirely. This smoke validates the endpoint shape.
# =====================================================================

$telegramWebhookSecret = Get-Setting -EnvValues $envValues -Name "BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN" -Fallback ""
if (-not [string]::IsNullOrWhiteSpace($telegramWebhookSecret)) {
    $webhookHeaders = @{ "X-Telegram-Webhook-Secret" = $telegramWebhookSecret }
    $results.Add((Invoke-Step -Name "telegram-webhook" -Method "POST" -Url "$resolvedBaseUrl/api/v1/telegram/webhook" -Headers $webhookHeaders -Body @{ Update = "smoke-test"; ChatId = $null; TraceId = [guid]::NewGuid().ToString() } -ExpectedStatusCodes @(200) -ResolvedIp $resolvedIp -AllowInvalidCertificate $allowInvalidCertificate))
}

$results | Format-Table -AutoSize
