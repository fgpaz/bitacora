# Task T6: Smoke And E2E Hygiene

## Shared Context
**Goal:** Replace Supabase-era smoke/E2E patterns with Zitadel-safe evidence.
**Stack:** PowerShell smoke, Playwright, artifacts.
**Architecture:** Smokes must exercise real Zitadel auth without forging JWTs.

## Locked Decisions
- Do not forge HS256 JWTs.
- Do not persist token bodies, refresh tokens, PATs, or client secrets.
- Client credentials are allowed only for M2M contract checks, not patient/professional user flows.

## Task Metadata
```yaml
id: T6
depends_on: [T4]
agent_type: ps-worker
files:
  - modify: infra/smoke/backend-smoke.ps1
  - modify: infra/runbooks/backend-smoke.md
  - create: artifacts/e2e/2026-04-19-bitacora-wave-b-zitadel/README.md
complexity: medium
done_when: "smoke evidence exists and no smoke script forges or stores JWT bodies"
```

## Reference
`infra/smoke/backend-smoke.ps1:92-100` currently forges HS256 JWTs and must be removed.

## Prompt
Update smoke flow so patient/professional authenticated checks obtain tokens through a real browser/PKCE or approved test harness and only record masked token metadata. Remove `SUPABASE_JWT_SECRET` as a required smoke env. Add stale Supabase cookie regression. Ensure artifacts record URLs, HTTP status, screenshots, and masked claims only.

## Execution Procedure
1. Remove `New-SmokeJwt` and HS256 generation from `infra/smoke/backend-smoke.ps1`.
2. Replace Supabase secret requirement with Zitadel env requirements.
3. Add a token metadata masker that emits only `iss`, `aud`, `alg`, and presence of `kid`.
4. Update runbook expected env and examples.
5. Run smoke in local/staging if B0 credentials are available.
6. Write artifact README.

## Skeleton
```powershell
$metadata = Get-MaskedJwtMetadata -Token $accessToken
if ($metadata.alg -ne "RS256") { throw "Expected RS256 token." }
```

## Verify
`rg -n "New-SmokeJwt|SUPABASE_JWT_SECRET|sb-access-token" infra/smoke infra/runbooks/backend-smoke.md` -> no active Supabase auth requirement remains

## Commit
`test(auth): migrate smoke gates to zitadel`
