# Backend Smoke Gate

Executable gate: `infra/smoke/backend-smoke.ps1`

## What it verifies

1. `GET /health`
2. `GET /health/ready`
3. authenticated `POST /api/v1/auth/bootstrap`
4. `GET /api/v1/consent/current`
5. negative consent gate on `POST /api/v1/mood-entries`
6. `POST /api/v1/consent`
7. positive `POST /api/v1/mood-entries`
8. positive `POST /api/v1/daily-checkins`

## Required environment

- `BITACORA_BASE_URL`
- `SUPABASE_JWT_SECRET`
- optional `BITACORA_SMOKE_SUB`
- optional `BITACORA_SMOKE_EMAIL`
- optional `BITACORA_SMOKE_RESOLVE_IP`
- optional `BITACORA_SMOKE_SKIP_CERT_CHECK`

If `infra/.env` exists, the script can load it directly.
If the smoke identity is omitted, the script generates a fresh user per run so the pre-consent rejection path remains valid on reruns.

## Run

```powershell
pwsh -File .\infra\smoke\backend-smoke.ps1
```

If DNS is still pending but Traefik is already routing the domain, pin the
hostname to the VPS IP for the smoke run:

```powershell
$env:BITACORA_BASE_URL = "https://api.bitacora.nuestrascuentitas.com"
$env:BITACORA_SMOKE_RESOLVE_IP = "54.37.157.93"
pwsh -File .\infra\smoke\backend-smoke.ps1
```

If HTTPS is up but the certificate is still provisioning, add:

```powershell
$env:BITACORA_SMOKE_SKIP_CERT_CHECK = "true"
```

## Success criteria

- every step returns the expected HTTP status
- readiness returns `200`
- the script exits `0`

If any step fails, T01 stays open.
