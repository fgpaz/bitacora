# Bitacora Production Checklist

## Scope

This checklist covers the current production surface after Wave B Zitadel cutover:

- `bitacora-db`
- `bitacora-api`
- `bitacora-frontend`
- shared Zitadel IdP at `id.nuestrascuentitas.com`
- Telegram runtime remains backend-owned; no separate Telegram app is deployed here

## Preconditions

- `infra/.env` exists locally and is not committed.
- `& $DKP doctor` passes.
- `sshr hosts test turismo` passes.
- DNS for `api.bitacora.nuestrascuentitas.com` points to `54.37.157.93`.
- DNS for `bitacora.nuestrascuentitas.com` points to `54.37.157.93`.
- Zitadel discovery and JWKS return `200`.

## Creation gate

- `BITACORA_PROJECT_ID` exists or is created.
- `BITACORA_DB_SERVICE_ID` exists or is created.
- `BITACORA_API_APP_ID` exists or is created.
- `BITACORA_FRONTEND_APP_ID` exists.
- API app uses the root `Dockerfile`/service Dockerfile mirror.
- Frontend app uses `frontend/Dockerfile`.
- `EventBusSettings__HostAddress` stays blank.

## Runtime gate

- PostgreSQL is healthy.
- `ConnectionStrings__BitacoraDb` is final.
- `ZITADEL_AUTHORITY=https://id.nuestrascuentitas.com`.
- `ZITADEL_AUDIENCE=369306332534145382`.
- `ZITADEL_WEB_CLIENT_ID=369306336963330406`.
- Supabase/GoTrue secrets must be absent from the active API and frontend runtime.
- `BITACORA_ENCRYPTION_KEY` resolves to 32 bytes.
- `BITACORA_PSEUDONYM_SALT` is set.
- Production migrations ran explicitly.
- `GET /health` returns `200`.
- `GET /health/ready` returns `200`.

## Smoke gate

- `pwsh -File .\infra\smoke\zitadel-cutover-smoke.ps1` exits `0`.
- Backend readiness body includes `zitadel_metadata`.
- `/ingresar` redirects to Zitadel authorize with PKCE markers.
- `/api/auth/session` returns a null public session without leaking tokens.
- Protected API/proxy endpoints return `401` without a session.
- Authenticated clinical writes require a real Zitadel user session; do not forge HS256 JWTs post-cutover.

## Operability gate

- Daily backup job exists with 30-copy retention minimum.
- Manual backup path is documented.
- OTLP posture is explicit, even if exporter stays disabled.
- Incident thresholds and owners are documented.

Closure is blocked if any item above remains implicit.
