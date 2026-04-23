#requires -Version 5.1
<#
.SYNOPSIS
Validates Bitacora XP-on-main pushes before running git push.

.DESCRIPTION
This guard enforces the XP-on-main workflow documented in CLAUDE.md seccion 11.
It checks that HEAD can fast-forward origin/main, that touched surfaces match
the declared scope, that closure evidence is present, and that no dangerous
untracked artifacts leak into the push.

Shape adapted from BuhoSalud `infra/git/Invoke-PrePushGuard.ps1` with Bitacora
surface mapping:
- frontend lives under `frontend/` (not `src/frontend/`).
- backend (.NET solution) lives under `src/`.
- evidence docs convention: `.docs/raw/reports/*-closure.md`,
  `.docs/raw/decisiones/*.md`, `.docs/raw/investigacion/` are durable;
  `.docs/raw/plans/` and `.docs/raw/prompts/` are treated as promotable scratch.

Bitacora has no Project V2 board configured yet; the issue/card check is
optional and defaults to waiver when absent.
#>

[CmdletBinding()]
param(
    [int] $IssueNumber,
    [string] $IssueUrl,
    [string] $WaiverReason,
    [string[]] $ExpectedScope = @(),
    [string[]] $TraceabilityEvidence = @(),
    [string[]] $SharedSkillName = @(),
    [switch] $Json,
    [switch] $DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-GuardGit {
    param(
        [Parameter(Mandatory = $true)]
        [string[]] $GitArgs,
        [switch] $AllowFailure
    )

    $output = & git @GitArgs 2>&1
    $code = $LASTEXITCODE
    $text = ($output | ForEach-Object { "$_" }) -join "`n"

    if ($code -ne 0 -and -not $AllowFailure) {
        throw "git $($GitArgs -join ' ') failed with exit code ${code}: ${text}"
    }

    [pscustomobject]@{
        Code = $code
        Text = $text
    }
}

function Invoke-GuardCommand {
    param(
        [Parameter(Mandatory = $true)]
        [string] $Command,
        [Parameter(Mandatory = $true)]
        [string[]] $Arguments
    )

    $output = & $Command @Arguments 2>&1
    $code = $LASTEXITCODE
    [pscustomobject]@{
        Code = $code
        Text = (($output | ForEach-Object { "$_" }) -join "`n")
    }
}

function Split-Lines {
    param([string] $Text)
    if ([string]::IsNullOrWhiteSpace($Text)) {
        return @()
    }

    @($Text -split "`r?`n" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
}

function Format-GuardPath {
    param([string] $Path)
    ($Path -replace "\\", "/").Trim()
}

function Get-GitPathLines {
    param([string] $Text)
    @(Split-Lines $Text | Where-Object { $_ -notmatch "^warning:" } | ForEach-Object { Format-GuardPath $_ })
}

function Get-ObjectProperty {
    param(
        [object] $Object,
        [string] $Name
    )

    if ($null -eq $Object) { return $null }
    $property = $Object.PSObject.Properties[$Name]
    if ($null -eq $property) { return $null }
    $property.Value
}

function ConvertTo-NormalList {
    param([string[]] $Values)

    $items = New-Object System.Collections.Generic.List[string]
    foreach ($value in @($Values)) {
        if ([string]::IsNullOrWhiteSpace($value)) { continue }
        foreach ($part in ($value -split "[,;]")) {
            $trimmed = $part.Trim().Trim("'`"")
            if (-not [string]::IsNullOrWhiteSpace($trimmed)) {
                $items.Add($trimmed.ToLowerInvariant())
            }
        }
    }
    @($items | Select-Object -Unique)
}

function Read-PjConfig {
    $defaults = @{
        PJ_REPO = "fgpaz/bitacora"
        PJ_PROJECT_OWNER = ""
        PJ_PROJECT_NUMBER = ""
        PJ_PROJECT_STATUS_FIELD = "Status"
    }

    $path = Join-Path (Get-Location) ".pj-crear-tarjeta.conf"
    if (-not (Test-Path -LiteralPath $path)) {
        return $defaults
    }

    foreach ($line in Get-Content -LiteralPath $path) {
        $trimmed = $line.Trim()
        if ($trimmed.Length -eq 0 -or $trimmed.StartsWith("#")) { continue }
        if ($trimmed -match "^(?<key>[A-Za-z_][A-Za-z0-9_]*)=(?<value>.*)$") {
            $key = $Matches["key"]
            $value = $Matches["value"].Trim().Trim("'`"")
            $defaults[$key] = $value
        }
    }

    $defaults
}

function Get-AffectedSurface {
    param([string] $Path)

    switch -Regex ($Path) {
        "^(AGENTS\.md|CLAUDE\.md|SUBAGENTS\.md|PATHS\.md)$" { return "policy" }
        "^\.claude/"                                          { return "shared-skill" }
        "^infra/git/"                                         { return "git-tooling" }
        "^infra/migrations/"                                  { return "migrations-infra" }
        "^infra/(\.env|secrets|.*secret)"                     { return "secrets-infra" }
        "^infra/"                                             { return "infra" }
        "^\.github/"                                          { return "ci" }
        "^\.docs/wiki/23_uxui/"                               { return "canon-docs" }
        "^\.docs/wiki/"                                       { return "canon-docs" }
        "^\.docs/templates/"                                  { return "canon-docs" }
        "^\.docs/raw/reports/"                                { return "evidence-docs" }
        "^\.docs/raw/decisiones/"                             { return "evidence-docs" }
        "^\.docs/raw/investigacion/"                          { return "evidence-docs" }
        "^\.docs/raw/plans/"                                  { return "raw-scratch" }
        "^\.docs/raw/prompts/"                                { return "raw-scratch" }
        "^\.docs/raw/"                                        { return "raw-docs" }
        "^frontend/(e2e|playwright\.config)"                  { return "frontend" }
        "^frontend/lib/auth/"                                 { return "frozen-auth" }
        "^frontend/app/api/"                                  { return "frozen-api" }
        "^frontend/app/auth/"                                 { return "frozen-auth" }
        "^frontend/proxy\.ts$"                                { return "frozen-proxy" }
        "^frontend/src/"                                      { return "frozen-src" }
        "^frontend/"                                          { return "frontend" }
        "^src/Bitacora\.(Api|Application|Domain|DataAccess|Infrastructure|EventBus|Tests)" { return "backend" }
        "^src/(Shared|TelegramBotAdapter)"                    { return "backend" }
        "^src/Bitacora\.sln$"                                 { return "backend" }
        "^src/"                                               { return "backend" }
        "^scripts/"                                           { return "scripts-infra" }
        "^tests/|^test/"                                      { return "tests" }
        default                                               { return "repo-root" }
    }
}

function Test-ScopeMatch {
    param(
        [string] $Surface,
        [string] $Path,
        [string[]] $Expected
    )

    if ($Expected.Count -eq 0) { return $false }

    $aliases = @{
        "shared-skill"      = @("shared-skill", "skills", "skill", "agent-skills")
        "git-tooling"       = @("git-tooling", "infra/git", "prepush", "pre-push", "tooling")
        "policy"            = @("policy", "agents", "claude", "agents.md", "claude.md")
        "canon-docs"        = @("canon-docs", "docs", "wiki")
        "raw-docs"          = @("raw-docs", "raw", "local-raw")
        "raw-scratch"       = @("raw-scratch", "raw", "raw-docs", "plans", "prompts", "scratch")
        "evidence-docs"     = @("evidence-docs", "evidence", "closure", "reports", "decisiones", "investigacion", "decisions")
        "secrets-infra"     = @("secrets-infra", "secrets", "infra")
        "migrations-infra"  = @("migrations-infra", "migrations", "infra", "sql")
        "scripts-infra"     = @("scripts-infra", "scripts", "infra", "tooling")
        "ci"                = @("ci", "github", "actions")
        "frontend"          = @("frontend", "web", "web-next", "next")
        "backend"           = @("backend", "api", "service", "dotnet", "net")
        "infra"             = @("infra", "ops")
        "tests"             = @("tests", "test", "e2e", "qa")
        "frozen-auth"       = @("frozen-auth", "auth", "frozen")
        "frozen-api"        = @("frozen-api", "api-proxy", "frozen")
        "frozen-proxy"      = @("frozen-proxy", "proxy", "frozen")
        "frozen-src"        = @("frozen-src", "src-frontend", "frozen")
    }

    $pathLower = $Path.ToLowerInvariant()
    foreach ($entry in $Expected) {
        if ($entry -eq $Surface) { return $true }
        if ($pathLower.StartsWith($entry.TrimEnd("/") + "/") -or $pathLower -eq $entry.TrimEnd("/")) { return $true }
        if ($aliases.ContainsKey($Surface) -and $aliases[$Surface] -contains $entry) { return $true }
    }

    $false
}

function Test-LabelCompatibility {
    param(
        [string[]] $Surfaces,
        [string[]] $Labels
    )

    $labelsLower = @($Labels | ForEach-Object { $_.ToLowerInvariant() })
    $missing = New-Object System.Collections.Generic.List[string]

    $expectations = @{
        "frontend"    = @("frontend", "web", "web-next", "ux", "ui", "next")
        "backend"     = @("backend", "api", "service", "dotnet", "net")
        "infra"       = @("infra", "ops", "devops")
        "git-tooling" = @("infra", "ops", "tooling", "git")
        "shared-skill" = @("docs", "infra", "tooling", "skills")
        "policy"      = @("docs", "infra", "policy", "governance")
        "canon-docs"  = @("docs", "wiki", "spec", "sdd")
        "evidence-docs" = @("docs", "closure", "evidence")
    }

    foreach ($surface in $Surfaces) {
        if (-not $expectations.ContainsKey($surface)) { continue }
        $ok = $false
        foreach ($expected in $expectations[$surface]) {
            if ($labelsLower -contains $expected) {
                $ok = $true
                break
            }
        }
        if (-not $ok) { $missing.Add($surface) }
    }

    @($missing | Select-Object -Unique)
}

function Find-DangerousUntracked {
    param([string[]] $Paths)

    $patterns = @(
        "^frontend/test-results(/|$)",
        "^frontend/playwright-report(/|$)",
        "^frontend/coverage(/|$)",
        "^frontend/\.next(/|$)",
        "^frontend/node_modules(/|$)",
        "^frontend/dist(/|$)",
        "^frontend/build(/|$)",
        "^src/.*/bin(/|$)",
        "^src/.*/obj(/|$)",
        "^artifacts/(?!e2e/)"
    )

    foreach ($path in $Paths) {
        foreach ($pattern in $patterns) {
            if ($path -match $pattern) { $path }
        }
    }
}

function Resolve-IssueNumber {
    param(
        [int] $Number,
        [string] $Url
    )

    if ($Url) {
        if ($Url -notmatch "/issues/(?<num>[0-9]+)") {
            throw "IssueUrl does not contain /issues/<number>: $Url"
        }
        $fromUrl = [int]$Matches["num"]
        if ($Number -and $Number -ne $fromUrl) {
            throw "IssueNumber ($Number) does not match IssueUrl ($fromUrl)."
        }
        return $fromUrl
    }

    if ($Number) { return $Number }
    return $null
}

function Get-TraceEvidence {
    param(
        [string[]] $Provided,
        [string[]] $ChangedPaths
    )

    $fromEnv = @()
    if ($env:PREPUSH_GUARD_TRACEABILITY_EVIDENCE) {
        $fromEnv = @($env:PREPUSH_GUARD_TRACEABILITY_EVIDENCE -split "[,;]")
    }

    $providedAll = @(ConvertTo-NormalList (@($Provided) + $fromEnv))
    $existingProvided = New-Object System.Collections.Generic.List[string]
    $acceptedProvided = New-Object System.Collections.Generic.List[string]
    foreach ($item in $providedAll) {
        if (Test-Path -LiteralPath $item) {
            $normalized = Format-GuardPath $item
            $existingProvided.Add($normalized)
            $lower = $normalized.ToLowerInvariant()
            $leafName = [System.IO.Path]::GetFileName($lower)
            if (
                $lower -match "(^|/)\.docs/raw/reports/"     -or
                $lower -match "(^|/)\.docs/raw/decisiones/"  -or
                $lower -match "(^|/)\.docs/raw/investigacion/" -or
                $lower -match "(^|/)\.docs/auditoria/"       -or
                $lower -match "(^|/)\.docs/planificacion/"   -or
                $leafName -match "(traceability|trazabilidad|audit|auditoria|verdict|closure)"
            ) {
                $acceptedProvided.Add($normalized)
            }
        }
    }

    $detected = @($ChangedPaths | Where-Object {
        $_ -match "^\.docs/raw/reports/.*-closure\.md$" -or
        $_ -match "^\.docs/raw/decisiones/" -or
        $_ -match "^\.docs/raw/investigacion/" -or
        $_ -match "^\.docs/auditoria/" -or
        $_ -match "^\.docs/planificacion/" -or
        $_ -match "^\.docs/wiki/06_matriz_pruebas_RF\.md$" -or
        $_ -match "^\.docs/wiki/06_pruebas/"
    } | Select-Object -Unique)

    $ok = ($acceptedProvided.Count -gt 0) -or ($detected.Count -gt 0)

    [pscustomobject]@{
        Provided         = @($providedAll)
        ExistingProvided = @($existingProvided | Select-Object -Unique)
        AcceptedProvided = @($acceptedProvided | Select-Object -Unique)
        DetectedInDiff   = @($detected)
        Ok               = $ok
    }
}

function Get-FileHashManifest {
    param([string] $Root)

    if (-not (Test-Path -LiteralPath $Root)) { return $null }

    @(Get-ChildItem -LiteralPath $Root -File -Recurse | ForEach-Object {
        $relative = (Format-GuardPath $_.FullName.Substring($Root.Length).TrimStart("\"))
        [pscustomobject]@{
            Relative = $relative
            Hash = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash
        }
    } | Sort-Object Relative)
}

function Test-SharedSkillMirror {
    param([string] $Name)

    $source = Join-Path "C:\Users\fgpaz\.agents\skills" $Name
    $mirror = Join-Path "C:\repos\buho\assets\skills" $Name
    $sourceManifest = Get-FileHashManifest $source
    $mirrorManifest = Get-FileHashManifest $mirror

    if ($null -eq $sourceManifest) {
        return [pscustomobject]@{ Name = $Name; InSync = $false; Reason = "source missing"; Source = $source; Mirror = $mirror }
    }
    if ($null -eq $mirrorManifest) {
        return [pscustomobject]@{ Name = $Name; InSync = $false; Reason = "mirror missing"; Source = $source; Mirror = $mirror }
    }

    $diff = Compare-Object $sourceManifest $mirrorManifest -Property Relative,Hash
    [pscustomobject]@{
        Name = $Name
        InSync = ($null -eq $diff)
        Reason = if ($null -eq $diff) { "in sync" } else { "hash mismatch" }
        Source = $source
        Mirror = $mirror
    }
}

$blockers = New-Object System.Collections.Generic.List[string]
$waiverableBlockers = New-Object System.Collections.Generic.List[string]
$warnings = New-Object System.Collections.Generic.List[string]

try {
    Invoke-GuardGit @("rev-parse", "--is-inside-work-tree") | Out-Null
} catch {
    Write-Error "Pre-push guard must run inside a git worktree. $($_.Exception.Message)"
    exit 1
}

$branch = (Invoke-GuardGit @("rev-parse", "--abbrev-ref", "HEAD")).Text.Trim()
$head = (Invoke-GuardGit @("rev-parse", "--short", "HEAD")).Text.Trim()
$base = "origin/main"
$pj = Read-PjConfig
$issue = Resolve-IssueNumber -Number $IssueNumber -Url $IssueUrl
$expected = @(ConvertTo-NormalList $ExpectedScope)
$sharedSkillNames = @(ConvertTo-NormalList (@($SharedSkillName) + @($env:PREPUSH_GUARD_SHARED_SKILL)))

$fetch = Invoke-GuardGit @("fetch", "origin", "main", "--prune") -AllowFailure
if ($fetch.Code -ne 0) {
    $blockers.Add("Unable to fetch origin/main; push safety cannot be verified. $($fetch.Text)")
}

$baseExists = (Invoke-GuardGit @("rev-parse", "--verify", $base) -AllowFailure).Code -eq 0
if (-not $baseExists) {
    $blockers.Add("Base ref $base is missing after fetch.")
}

$ffOk = $false
if ($baseExists) {
    $ancestor = Invoke-GuardGit @("merge-base", "--is-ancestor", $base, "HEAD") -AllowFailure
    $ffOk = $ancestor.Code -eq 0
    if (-not $ffOk) {
        $blockers.Add("$base is not an ancestor of HEAD. Pull/rebase/merge main first; do not force-push.")
    }
}

$pendingCommits = @()
$pendingPaths = @()
if ($baseExists) {
    $pendingCommits = @(Split-Lines (Invoke-GuardGit @("log", "--oneline", "$base..HEAD")).Text)
    $pendingPaths = @(Get-GitPathLines (Invoke-GuardGit @("diff", "--name-only", "$base...HEAD") -AllowFailure).Text)
}

$workingPaths = @(Get-GitPathLines (Invoke-GuardGit @("diff", "--name-only")).Text)
$stagedPaths = @(Get-GitPathLines (Invoke-GuardGit @("diff", "--cached", "--name-only")).Text)
$untrackedPaths = @(Get-GitPathLines (Invoke-GuardGit @("ls-files", "--others", "--exclude-standard")).Text)
$allChangedPaths = @($pendingPaths + $workingPaths + $stagedPaths + $untrackedPaths | Where-Object { $_ } | Sort-Object -Unique)
$significantWork = $pendingCommits.Count -gt 0 -or $allChangedPaths.Count -gt 0

$rawDeletionPaths = @()
if ($baseExists) {
    $rawDeletionPaths += @(Get-GitPathLines (Invoke-GuardGit @("diff", "--name-only", "--diff-filter=D", "$base...HEAD", "--", ".docs/raw") -AllowFailure).Text)
}
$rawDeletionPaths += @(Get-GitPathLines (Invoke-GuardGit @("diff", "--name-only", "--diff-filter=D", "--", ".docs/raw")).Text)
$rawDeletionPaths += @(Get-GitPathLines (Invoke-GuardGit @("diff", "--cached", "--name-only", "--diff-filter=D", "--", ".docs/raw")).Text)
$rawDeletionPaths = @($rawDeletionPaths | Select-Object -Unique)

if ($branch -ne "main") {
    $warnings.Add("Current branch is '$branch'. XP push guard is calibrated for pushes to main.")
}

$dangerousUntrackedPaths = @(Find-DangerousUntracked $untrackedPaths | Select-Object -Unique)
foreach ($path in $dangerousUntrackedPaths) {
    $blockers.Add("Dangerous untracked artifact: $path (move to artifacts/e2e/ or add to .gitignore).")
}

# Bitacora convention: .docs/raw/reports/, .docs/raw/decisiones/, .docs/raw/investigacion/
# are durable evidence paths and may be committed. .docs/raw/plans/ and .docs/raw/prompts/
# are promotable scratch (may be committed as reference but flagged as warning).
# Bare .docs/raw/ files with no known subpath are blocked unless waived.
$rawUnknownPaths = @($allChangedPaths | Where-Object {
    $_ -match "^\.docs/raw/[^/]+$"
} | Select-Object -Unique)
foreach ($path in $rawUnknownPaths) {
    if ($rawDeletionPaths -contains $path) {
        $warnings.Add("Raw artifact removed from Git tracking: $path")
        continue
    }
    $blockers.Add("Uncategorized raw artifact under .docs/raw/: $path. Place it under reports/, decisiones/, investigacion/, plans/ or prompts/.")
}

$rawScratchPaths = @($allChangedPaths | Where-Object {
    $_ -match "^\.docs/raw/(plans|prompts)/"
} | Select-Object -Unique)
foreach ($path in $rawScratchPaths) {
    if ($rawDeletionPaths -contains $path) {
        $warnings.Add("Raw scratch removed from Git tracking: $path")
        continue
    }
    $warnings.Add("Raw scratch committed (allowed as reference but treat as local scratch first): $path")
}

$surfaces = @($allChangedPaths | ForEach-Object { Get-AffectedSurface $_ } | Select-Object -Unique)
$criticalSurfaces = @("policy", "shared-skill", "git-tooling", "canon-docs", "migrations-infra", "secrets-infra", "ci", "frozen-auth", "frozen-api", "frozen-proxy", "frozen-src")
$frozenSurfaces = @("frozen-auth", "frozen-api", "frozen-proxy", "frozen-src")

foreach ($surface in $surfaces) {
    if ($frozenSurfaces -contains $surface -and [string]::IsNullOrWhiteSpace($WaiverReason)) {
        $blockers.Add("Frozen surface touched without waiver: '$surface'. See CLAUDE.md seccion 11 Safeguards inviolables.")
    }
}

if ($significantWork -and $expected.Count -eq 0) {
    $blockers.Add("ExpectedScope is required for significant work.")
}

foreach ($path in $allChangedPaths) {
    if ($dangerousUntrackedPaths -contains $path) { continue }
    if ($rawUnknownPaths -contains $path) { continue }
    $surface = Get-AffectedSurface $path
    $matchesScope = Test-ScopeMatch -Surface $surface -Path $path -Expected $expected
    if ($expected.Count -gt 0 -and -not $matchesScope) {
        $blockers.Add("Changed path is outside ExpectedScope: $path ($surface)")
    } elseif ($expected.Count -eq 0 -and $criticalSurfaces -contains $surface) {
        $blockers.Add("Critical surface '$surface' requires ExpectedScope declaration: $path")
    }
}

$trace = Get-TraceEvidence -Provided $TraceabilityEvidence -ChangedPaths $allChangedPaths
if ($significantWork -and -not $trace.Ok) {
    $waiverableBlockers.Add("Traceability evidence missing. Provide -TraceabilityEvidence with a closure/audit file or commit a closure doc under .docs/raw/reports/ in this diff.")
}

$sharedSkillScope = ($expected -contains "shared-skill") -or ($expected -contains "skills") -or ($expected -contains "skill") -or ($surfaces -contains "shared-skill")
$mirrorChecks = @()
if ($significantWork -and $sharedSkillScope) {
    if ($sharedSkillNames.Count -eq 0) {
        $blockers.Add("Shared-skill scope requires -SharedSkillName or PREPUSH_GUARD_SHARED_SKILL so source/mirror sync can be verified.")
    } else {
        $mirrorChecks = @($sharedSkillNames | ForEach-Object { Test-SharedSkillMirror $_ })
        foreach ($check in $mirrorChecks) {
            if (-not $check.InSync) {
                $blockers.Add("Shared skill '$($check.Name)' source/mirror sync failed: $($check.Reason).")
            }
        }
    }
}

$issueLabels = @()
$issueState = $null
$issueTitle = $null
$issueUrlResolved = $IssueUrl
$boardVerified = $false
$boardStatus = $null
$boardReason = $null

# Bitacora does not require a board card today; only warn when significant work
# has no issue and no waiver. If the project registers a Project V2 later, set
# PJ_PROJECT_OWNER + PJ_PROJECT_NUMBER in .pj-crear-tarjeta.conf to harden this.
if ($significantWork -and -not $issue -and [string]::IsNullOrWhiteSpace($WaiverReason)) {
    $warnings.Add("No issue/card declared and no -WaiverReason provided. Bitacora allows this today (no board enforced); document the change in the commit message.")
}

if ($issue) {
    $gh = Get-Command "gh" -ErrorAction SilentlyContinue
    if (-not $gh) {
        $warnings.Add("GitHub CLI 'gh' is not available, so issue state cannot be verified.")
    } else {
        $issueResult = Invoke-GuardCommand "gh" @("issue", "view", "$issue", "--repo", $pj.PJ_REPO, "--json", "number,title,url,state,labels")
        if ($issueResult.Code -ne 0) {
            $waiverableBlockers.Add("Unable to verify GitHub issue #$issue in $($pj.PJ_REPO): $($issueResult.Text)")
        } else {
            $issueJson = $issueResult.Text | ConvertFrom-Json
            $issueLabels = @($issueJson.labels | ForEach-Object { $_.name })
            $issueState = $issueJson.state
            $issueTitle = $issueJson.title
            $issueUrlResolved = $issueJson.url

            if ($significantWork -and $issueState -eq "CLOSED") {
                $blockers.Add("Issue #$issue is already closed before push.")
            }

            if ([string]::IsNullOrWhiteSpace($pj.PJ_PROJECT_NUMBER) -or [string]::IsNullOrWhiteSpace($pj.PJ_PROJECT_OWNER)) {
                $boardReason = "no project configured in .pj-crear-tarjeta.conf"
            } else {
                $projectResult = Invoke-GuardCommand "gh" @("project", "item-list", $pj.PJ_PROJECT_NUMBER, "--owner", $pj.PJ_PROJECT_OWNER, "--format", "json", "--limit", "200")
                if ($projectResult.Code -ne 0) {
                    $waiverableBlockers.Add("Unable to verify Project V2 board $($pj.PJ_PROJECT_OWNER)/$($pj.PJ_PROJECT_NUMBER): $($projectResult.Text)")
                } else {
                    $projectJson = $projectResult.Text | ConvertFrom-Json
                    $items = @(Get-ObjectProperty $projectJson "items")
                    $match = $items | Where-Object {
                        $content = Get-ObjectProperty $_ "content"
                        ((Get-ObjectProperty $content "number") -eq $issue) -or ((Get-ObjectProperty $content "url") -eq $issueUrlResolved)
                    } | Select-Object -First 1

                    if (-not $match) {
                        $waiverableBlockers.Add("Issue #$issue is not present in Project V2 $($pj.PJ_PROJECT_OWNER)/$($pj.PJ_PROJECT_NUMBER).")
                    } else {
                        $boardVerified = $true
                        $statusValue = $null
                        $fieldValues = @(Get-ObjectProperty $match "fieldValues")
                        if ($fieldValues.Count -gt 0) {
                            $statusField = $fieldValues | Where-Object { (Get-ObjectProperty $_ "fieldName") -eq $pj.PJ_PROJECT_STATUS_FIELD } | Select-Object -First 1
                            if ($statusField) { $statusValue = Get-ObjectProperty $statusField "name" }
                        }
                        if (-not $statusValue) { $statusValue = Get-ObjectProperty $match "status" }
                        $boardStatus = $statusValue
                        if ($significantWork -and $boardStatus -eq "Hecho") {
                            $blockers.Add("Project card for issue #$issue is already 'Hecho' before push/checks.")
                        }
                        if ($significantWork -and ($boardStatus -eq "Backlog" -or $boardStatus -eq "Ready")) {
                            $blockers.Add("Project card for issue #$issue is stale ('$boardStatus'); run pj-crear-tarjeta status-sync before the guard.")
                        }
                    }
                }
            }

            $labelMissing = @(Test-LabelCompatibility -Surfaces $surfaces -Labels $issueLabels)
            foreach ($surface in $labelMissing) {
                $warnings.Add("Issue #$issue labels do not appear compatible with touched surface '$surface' (non-blocking).")
            }
        }
    }
} elseif (-not [string]::IsNullOrWhiteSpace($WaiverReason)) {
    $boardReason = "waived"
}

if (-not [string]::IsNullOrWhiteSpace($WaiverReason) -and $waiverableBlockers.Count -gt 0) {
    foreach ($item in $waiverableBlockers) {
        $warnings.Add("Waived: $item")
    }
    $waiverableBlockers.Clear()
}

foreach ($item in $waiverableBlockers) {
    $blockers.Add($item)
}

$verdict = "Approved"
$waivedWarningCount = @($warnings | Where-Object { $_ -like "Waived:*" }).Count
if ($blockers.Count -gt 0) {
    $verdict = "Blocked"
} elseif (-not [string]::IsNullOrWhiteSpace($WaiverReason) -or $waivedWarningCount -gt 0) {
    $verdict = "Approved with waiver"
}

$report = [ordered]@{
    schemaVersion = "1.0-bitacora"
    generatedAt = (Get-Date).ToString("o")
    dryRun = [bool]$DryRun
    verdict = $verdict
    branch = $branch
    base = $base
    head = $head
    fastForward = [bool]$ffOk
    pendingCommits = @($pendingCommits)
    changedPaths = [ordered]@{
        pending = @($pendingPaths)
        working = @($workingPaths)
        staged = @($stagedPaths)
        untracked = @($untrackedPaths)
        all = @($allChangedPaths)
    }
    detectedCard = [ordered]@{
        issueNumber = $issue
        issueUrl = $issueUrlResolved
        title = $issueTitle
        state = $issueState
        labels = @($issueLabels)
        waiverReason = $WaiverReason
    }
    boardState = [ordered]@{
        repo = $pj.PJ_REPO
        projectOwner = $pj.PJ_PROJECT_OWNER
        projectNumber = $pj.PJ_PROJECT_NUMBER
        statusField = $pj.PJ_PROJECT_STATUS_FIELD
        status = $boardStatus
        verified = [bool]$boardVerified
        reason = $boardReason
    }
    traceabilityEvidence = $trace
    sharedSkillMirror = @($mirrorChecks)
    affectedSurfaces = @($surfaces)
    rawScratchPaths = @($rawScratchPaths)
    rawUnknownPaths = @($rawUnknownPaths)
    rawDeletionPaths = @($rawDeletionPaths)
    blockers = @($blockers)
    warnings = @($warnings)
}

if ($Json) {
    $report | ConvertTo-Json -Depth 8
} else {
    Write-Host "PrePushGuard (Bitacora): $verdict"
    Write-Host "Branch: $branch -> $base"
    Write-Host "Head: $head"
    Write-Host "Pending commits: $($pendingCommits.Count)"
    Write-Host "Changed paths: $($allChangedPaths.Count)"
    if ($surfaces.Count -gt 0) {
        Write-Host "Affected surfaces: $($surfaces -join ', ')"
    }
    if ($issue) {
        Write-Host "Issue: #$issue $issueTitle"
        Write-Host "Board status: $boardStatus"
    } elseif ($WaiverReason) {
        Write-Host "Waiver: $WaiverReason"
    }
    if ($blockers.Count -gt 0) {
        Write-Host ""
        Write-Host "Blockers:"
        foreach ($item in $blockers) { Write-Host " - $item" }
    }
    if ($warnings.Count -gt 0) {
        Write-Host ""
        Write-Host "Warnings:"
        foreach ($item in $warnings) { Write-Host " - $item" }
    }
}

if ($verdict -eq "Blocked") {
    exit 2
}

exit 0
