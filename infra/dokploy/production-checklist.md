# Bitacora Production Checklist

## Scope

This checklist is only for the truthful T01 production surface:

- `bitacora-db`
- `bitacora-api`
- no staging
- no frontend runtime
- no Telegram runtime

## Preconditions

- `infra/.env` exists locally and is not committed.
- `& $DKP doctor` passes.
- `sshr hosts test turismo` passes.
- DNS for `api.bitacora.nuestrascuentitas.com` points to `54.37.157.93`.

## Creation gate

- `BITACORA_PROJECT_ID` exists or is created.
- `BITACORA_DB_SERVICE_ID` exists or is created.
- `BITACORA_API_APP_ID` exists or is created.
- API app uses `src/Bitacora.Api/Dockerfile`.
- `EventBusSettings__HostAddress` stays blank.

## Runtime gate

- PostgreSQL is healthy.
- `ConnectionStrings__BitacoraDb` is final.
- `SUPABASE_JWT_SECRET` is set.
- `BITACORA_ENCRYPTION_KEY` resolves to 32 bytes.
- `BITACORA_PSEUDONYM_SALT` is set.
- Production migrations ran explicitly.
- `GET /health` returns `200`.
- `GET /health/ready` returns `200`.

## Smoke gate

- `pwsh -File .\infra\smoke\backend-smoke.ps1` exits `0`.
- Bootstrap succeeds.
- Consent gate rejects pre-consent writes.
- Consent grant succeeds.
- MoodEntry succeeds after consent.
- DailyCheckin succeeds after consent.

## Operability gate

- Daily backup job exists with 30-copy retention minimum.
- Manual backup path is documented.
- OTLP posture is explicit, even if exporter stays disabled.
- Incident thresholds and owners are documented.

Closure is blocked if any item above remains implicit.
