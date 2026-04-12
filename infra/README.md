# Bitacora Infra Bootstrap

This directory is the repo-local bootstrap layer for the current production surface:

- `Bitacora.Api`
- dedicated PostgreSQL
- backend-only smoke and operability assets

Out of scope for this bootstrap:

- `frontend/`
- Telegram runtime
- staging

Pending work after this backend-only bootstrap is governed by `.docs/plans/wave-prod/`.
`.docs/raw/plans/wave-1/` remains historical context only.

## Files

- `.env.template`: local-only environment contract for Dokploy, runtime, and smoke execution
- `dokploy/`: production specs for the API app and PostgreSQL service
- `runbooks/`: operator steps for bootstrap, migraciones, secretos, humo, backup y restauracion
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

## Runtime Secrets Contract

Every production deployment must have these variables set via Dokploy:

| Variable | Validation | Fail-closed |
|----------|------------|-------------|
| `SUPABASE_JWT_SECRET` | Non-empty string | Startup throws if missing |
| `BITACORA_ENCRYPTION_KEY` | 32 bytes Base64-decoded | `/health/ready` returns 503 if invalid; writes blocked |
| `BITACORA_PSEUDONYM_SALT` | Non-empty string | Any operation needing pseudonym throws 500 |
| `ConnectionStrings__BitacoraDb` | Valid PostgreSQL connection string | `/health/ready` returns 503 if unreachable |
| `DataAccess:ApplyMigrationsOnStartup` | `false` in production | Migrations run via `infra/runbooks/manual-migrations.md` only |

See `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md` for the complete fail-closed gate catalog.

## Rollout Order (terminal validation)

1. Secrets materialized in Dokploy.
2. `bitacora-db` deployed and reachable.
3. Manual migrations via runbook.
4. `GET /health/ready` returns 200.
5. `bitacora-api` deployed.
6. Smoke gate `infra/smoke/backend-smoke.ps1` passes (exit 0).
7. Only then: open traffic or start next surface rollout phase.

**Validation de UI es actividad terminal.** No se marca ninguna fase como completa hasta que la validacion UX tenga evidencia.

See `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md` for the full rollout gate catalog.
