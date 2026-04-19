# Task G4: Fix Client Credentials Grant

## Shared Context
**Goal:** Existing API clients must issue RS256 JWTs with `client_credentials`.
**Stack:** Zitadel management API, four API clients, Infisical secrets.
**Architecture:** Preserve all client IDs/secrets; do not recreate clients unless explicitly approved.

## Locked Decisions
- Fix existing API clients.
- Do not rotate `ZITADEL_CLIENT_<ORG>_API_SECRET` unless PATCH path is impossible and user approves.
- Verify all four org API clients, not only Bitacora.

## Task Metadata
```yaml
id: G4
depends_on: [G0]
agent_type: ps-worker
files:
  - create: .docs/raw/reports/2026-04-19-cerrar-gaps/G4-client-credentials.md
complexity: high
done_when: "All four API clients return JWT access_token with alg RS256 and kid"
```

## Reference
`.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md:61` — canonical project/client table.

## Prompt
Read current app configs before mutation. Apply the minimum Zitadel API changes required so each API app supports `client_credentials`. Use `ZITADEL_ADMIN_PAT` from env and mask all secrets. Test `nuestrascuentitas-api`, `bitacora-api`, `multi-tedi-api`, and `gastos-api`.

## Execution Procedure
1. Reproduce current `invalid_client` for `bitacora-api` and record masked error.
2. Read each project/app config via management API.
3. Patch project/app settings required for API `client_credentials`.
4. Re-test each API client using `grant_type=client_credentials&scope=openid`.
5. Decode JWT header only and record `alg`, `kid_present`, `iss`, and token lifetime, not token body with secrets.
6. If PATCH cannot work without recreating apps, stop and ask user before any rotation.
7. Write evidence and commit with `fix(zitadel): enable api client credentials`.

## Skeleton
```powershell
$body = "grant_type=client_credentials&client_id=$clientId&client_secret=$clientSecret&scope=openid"
curl.exe -sS -X POST -H "Content-Type: application/x-www-form-urlencoded" --data $body https://id.nuestrascuentitas.com/oauth/v2/token
```

## Verify
`client_credentials` for all four API clients -> JWT header `{"alg":"RS256","kid":"..."}`

## Commit
`fix(zitadel): enable api client credentials`
