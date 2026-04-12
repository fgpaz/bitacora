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

### Extended surfaces (Phase 30/31 backend, Phase 40/41 frontend)

9. `GET /api/v1/vinculos`
10. `GET /api/v1/vinculos/active`
11. `GET /api/v1/visualizacion/timeline?from=&to=`
12. `GET /api/v1/visualizacion/summary?from=&to=`
13. `GET /api/v1/export/patient-summary?from=&to=`
14. `GET /api/v1/export/patient-summary/csv?from=&to=`
15. `POST /api/v1/telegram/pairing`
16. `GET /api/v1/telegram/session`
17. `POST /api/v1/telegram/webhook` (requires `BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN`)

## Required environment

- `BITACORA_BASE_URL`
- `SUPABASE_JWT_SECRET`
- optional `BITACORA_SMOKE_SUB`
- optional `BITACORA_SMOKE_EMAIL`
- optional `BITACORA_SMOKE_RESOLVE_IP`
- optional `BITACORA_SMOKE_SKIP_CERT_CHECK`
- optional `BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN` (enables webhook smoke step)

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

## Terminal validation note

**Smoke es precondition, no actividad terminal.** El smoke gate habilita la apertura de trafico, pero ninguna fase se marca completa hasta que la validacion UX tenga evidencia. Ver `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md` seccion timing de validacion terminal.
