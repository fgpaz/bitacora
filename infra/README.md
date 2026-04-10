# Bitacora Infra Bootstrap

This directory is the repo-local bootstrap layer for the current production surface:

- `Bitacora.Api`
- dedicated PostgreSQL
- backend-only smoke and operability assets

Out of scope for this bootstrap:

- `frontend/`
- Telegram runtime
- staging

## Files

- `.env.template`: local-only environment contract for Dokploy, runtime, and smoke execution
- `dokploy/`: production specs for the API app and PostgreSQL service
- `runbooks/`: operator steps for bootstrap, migrations, secrets, smoke execution, and restore
- `observability/`: runtime telemetry and incident contract
- `smoke/`: executable backend smoke gate

## Secret Source

Bitacora does not keep control-plane secrets in git.

Use the shared `mi-key-cli` setup from `C:\repos\mios\multi-tedi` to source:

- `DOKPLOY_URL`
- `DOKPLOY_API_KEY`
- `DOKPLOY_ENVIRONMENT_ID`
- `DOKPLOY_GITHUB_PROVIDER_ID`
- shared auth secrets when needed for `SUPABASE_JWT_SECRET`

Then copy only the required values into the local untracked `infra/.env`.

Do not commit `infra/.env`.
