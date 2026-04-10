param(
    [string]$EnvFile = (Join-Path $PSScriptRoot "..\.env"),
    [string]$BaseUrl,
    [string]$JwtSecret,
    [string]$SmokeSub,
    [string]$SmokeEmail
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
        [int[]]$ExpectedStatusCodes = @(200)
    )

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
    $content = if ([string]::IsNullOrWhiteSpace($response.Content)) { $null } else { $response.Content | ConvertFrom-Json }

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

$results.Add((Invoke-Step -Name "health" -Method "GET" -Url "$resolvedBaseUrl/health"))
$results.Add((Invoke-Step -Name "ready" -Method "GET" -Url "$resolvedBaseUrl/health/ready"))
$results.Add((Invoke-Step -Name "bootstrap" -Method "POST" -Url "$resolvedBaseUrl/api/v1/auth/bootstrap" -Headers $authHeaders))

$currentConsent = Invoke-Step -Name "consent-current" -Method "GET" -Url "$resolvedBaseUrl/api/v1/consent/current" -Headers $authHeaders
$results.Add($currentConsent)

$negativeMood = Invoke-Step -Name "mood-without-consent" -Method "POST" -Url "$resolvedBaseUrl/api/v1/mood-entries" -Headers $authHeaders -Body @{ score = 1 } -ExpectedStatusCodes @(403)
$results.Add($negativeMood)

if ($negativeMood.Body.error.code -ne "CONSENT_REQUIRED") {
    throw "Expected CONSENT_REQUIRED before granting consent."
}

$consentVersion = [string]$currentConsent.Body.version
if ([string]::IsNullOrWhiteSpace($consentVersion)) {
    throw "Consent version missing from current consent response."
}

$results.Add((Invoke-Step -Name "grant-consent" -Method "POST" -Url "$resolvedBaseUrl/api/v1/consent" -Headers $authHeaders -Body @{ version = $consentVersion; accepted = $true } -ExpectedStatusCodes @(201, 409)))
$results.Add((Invoke-Step -Name "mood-with-consent" -Method "POST" -Url "$resolvedBaseUrl/api/v1/mood-entries" -Headers $authHeaders -Body @{ score = 1 } -ExpectedStatusCodes @(200, 201)))
$results.Add((Invoke-Step -Name "daily-checkin" -Method "POST" -Url "$resolvedBaseUrl/api/v1/daily-checkins" -Headers $authHeaders -Body @{
        sleepHours      = 8
        physicalActivity = $false
        socialActivity   = $true
        anxiety          = $false
        irritability     = $false
        medicationTaken  = $false
        medicationTime   = $null
    } -ExpectedStatusCodes @(200, 201)))

$results | Format-Table -AutoSize
