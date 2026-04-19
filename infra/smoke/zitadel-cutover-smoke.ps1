param(
    [string]$WebBaseUrl = "",
    [string]$ApiBaseUrl = "",
    [string]$ZitadelAuthority = "",
    [string]$ZitadelClientId = "",
    [string]$RedirectUri = ""
)

$ErrorActionPreference = "Stop"

function Resolve-Value {
    param(
        [string]$Value,
        [string]$EnvName,
        [string]$Default
    )

    if (-not [string]::IsNullOrWhiteSpace($Value)) {
        return $Value.TrimEnd("/")
    }

    $envValue = [Environment]::GetEnvironmentVariable($EnvName)
    if (-not [string]::IsNullOrWhiteSpace($envValue)) {
        return $envValue.TrimEnd("/")
    }

    return $Default.TrimEnd("/")
}

function New-SmokeClient {
    param([bool]$AllowRedirect = $false)

    $handler = [System.Net.Http.HttpClientHandler]::new()
    $handler.AllowAutoRedirect = $AllowRedirect
    return [System.Net.Http.HttpClient]::new($handler)
}

function Invoke-SmokeRequest {
    param(
        [string]$Method,
        [string]$Uri,
        [bool]$AllowRedirect = $false
    )

    $client = New-SmokeClient -AllowRedirect $AllowRedirect
    try {
        $request = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::new($Method), $Uri)
        $response = $client.Send($request)
        $body = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()

        return [pscustomobject]@{
            StatusCode = [int]$response.StatusCode
            Location = if ($response.Headers.Location) { $response.Headers.Location.ToString() } else { "" }
            Body = $body
        }
    }
    finally {
        $client.Dispose()
    }
}

function Assert-Status {
    param(
        [string]$Name,
        [object]$Response,
        [int[]]$Expected
    )

    if ($Expected -notcontains $Response.StatusCode) {
        throw "$Name expected HTTP $($Expected -join "/") but got $($Response.StatusCode)"
    }

    Write-Host "PASS $Name -> HTTP $($Response.StatusCode)"
}

function Assert-BodyContains {
    param(
        [string]$Name,
        [object]$Response,
        [string]$Needle
    )

    if ($Response.Body -notlike "*$Needle*") {
        throw "$Name body did not contain expected marker '$Needle'"
    }

    Write-Host "PASS $Name contains $Needle"
}

function Assert-BodyNotContains {
    param(
        [string]$Name,
        [object]$Response,
        [string]$Needle
    )

    if ($Response.Body -like "*$Needle*") {
        throw "$Name body contained forbidden marker '$Needle'"
    }

    Write-Host "PASS $Name omits $Needle"
}

$web = Resolve-Value -Value $WebBaseUrl -EnvName "BITACORA_WEB_BASE_URL" -Default "https://bitacora.nuestrascuentitas.com"
$api = Resolve-Value -Value $ApiBaseUrl -EnvName "BITACORA_API_BASE_URL" -Default "https://api.bitacora.nuestrascuentitas.com"
$issuer = Resolve-Value -Value $ZitadelAuthority -EnvName "ZITADEL_AUTHORITY" -Default "https://id.nuestrascuentitas.com"

if ([string]::IsNullOrWhiteSpace($ZitadelClientId)) {
    $ZitadelClientId = [Environment]::GetEnvironmentVariable("ZITADEL_WEB_CLIENT_ID")
}
if ([string]::IsNullOrWhiteSpace($ZitadelClientId)) {
    $ZitadelClientId = "369306336963330406"
}

if ([string]::IsNullOrWhiteSpace($RedirectUri)) {
    $RedirectUri = [Environment]::GetEnvironmentVariable("ZITADEL_WEB_REDIRECT_URI")
}
if ([string]::IsNullOrWhiteSpace($RedirectUri)) {
    $RedirectUri = "$web/auth/callback"
}

$discovery = Invoke-SmokeRequest -Method "GET" -Uri "$issuer/.well-known/openid-configuration"
Assert-Status -Name "oidc discovery" -Response $discovery -Expected @(200)

$jwks = Invoke-SmokeRequest -Method "GET" -Uri "$issuer/oauth/v2/keys"
Assert-Status -Name "oidc jwks" -Response $jwks -Expected @(200)

$health = Invoke-SmokeRequest -Method "GET" -Uri "$api/health"
Assert-Status -Name "api health" -Response $health -Expected @(200)

$ready = Invoke-SmokeRequest -Method "GET" -Uri "$api/health/ready"
Assert-Status -Name "api readiness" -Response $ready -Expected @(200)
Assert-BodyContains -Name "api readiness" -Response $ready -Needle "zitadel_metadata"
Assert-BodyNotContains -Name "api readiness" -Response $ready -Needle "supabase_jwt_secret"

$homeResponse = Invoke-SmokeRequest -Method "GET" -Uri $web -AllowRedirect $true
Assert-Status -Name "web root" -Response $homeResponse -Expected @(200)

$login = Invoke-SmokeRequest -Method "HEAD" -Uri "$web/ingresar"
Assert-Status -Name "login redirect" -Response $login -Expected @(302, 307, 308)

if ($login.Location -notlike "$issuer/oauth/v2/authorize*") {
    throw "login redirect did not target Zitadel authorize endpoint"
}

$encodedRedirectUri = [System.Uri]::EscapeDataString($RedirectUri)
if ($login.Location -notlike "*client_id=$ZitadelClientId*" -or
    $login.Location -notlike "*redirect_uri=$encodedRedirectUri*" -or
    $login.Location -notlike "*code_challenge_method=S256*") {
    throw "login redirect is missing required OIDC PKCE parameters"
}
Write-Host "PASS login redirect contains sanitized OIDC PKCE markers"

$session = Invoke-SmokeRequest -Method "GET" -Uri "$web/api/auth/session"
Assert-Status -Name "public session endpoint" -Response $session -Expected @(200)
Assert-BodyContains -Name "public session endpoint" -Response $session -Needle '"user":null'

$proxy = Invoke-SmokeRequest -Method "GET" -Uri "$web/api/backend/consent/current"
Assert-Status -Name "backend proxy without session" -Response $proxy -Expected @(401)

$bootstrap = Invoke-SmokeRequest -Method "POST" -Uri "$api/api/v1/auth/bootstrap"
Assert-Status -Name "api bootstrap without bearer" -Response $bootstrap -Expected @(401)

$logout = Invoke-SmokeRequest -Method "HEAD" -Uri "$web/auth/logout"
Assert-Status -Name "logout redirect" -Response $logout -Expected @(302, 307, 308)
if ($logout.Location -notlike "$issuer/oidc/v1/end_session*") {
    throw "logout redirect did not target Zitadel end_session endpoint"
}
Write-Host "PASS logout redirect targets Zitadel end_session"

Write-Host "Zitadel cutover smoke passed."
