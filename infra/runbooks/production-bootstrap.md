# Production Bootstrap Runbook

## Scope

This runbook bootstraps the historical T01 backend production surface. For the current post-Wave B surface, use `infra/dokploy/production-checklist.md`.

- dedicated PostgreSQL
- `Bitacora.Api`
- backend smoke gate (historical T01)

This runbook is the closed backend baseline reused by `wave-prod`.
After T04 hardening, the legacy Supabase smoke gate (`infra/smoke/backend-smoke.ps1`) covered these additional surfaces:
- Vinculos (patient + professional): `GET/PATCH vinculos`, `POST vinculos/accept`, `DELETE vinculos/{id}`
- Visualizacion (patient + professional): `timeline`, `summary`, `alerts`
- Export: `patient-summary` (JSON + CSV)
- Telegram: `pairing`, `session`, `webhook`

The frontend (Next.js), Telegram bot hosting, and final UX validation are out of scope for this runbook.

## Preconditions

1. `infra/.env` exists locally and is not committed.
2. `dkp.ps1 doctor` passes from the Bitacora repo.
3. `sshr hosts test turismo` succeeds.
4. DNS for `api.bitacora.nuestrascuentitas.com` points to `54.37.157.93`.
5. If the root host will be parked on the backend, DNS for `bitacora.nuestrascuentitas.com` also points to `54.37.157.93`.

## Rollout order (fail-closed gates)

See `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md` for the canonical gate catalog.
The sequence below is the authoritative baseline for T01.

### Fase 1 — Infra y DB

1. Source approved secrets into local `infra/.env`.
2. Create or discover `BITACORA_PROJECT_ID`.
3. Create and deploy `bitacora-db`.
4. Persist returned DB host metadata into local `infra/.env`.
5. Build `ConnectionStrings__BitacoraDb`.
6. Run manual migrations via `infra/runbooks/manual-migrations.md`.
7. Verify `GET /health/ready` returns 200 before proceeding.

### Fase 2 — API

8. Create and configure `bitacora-api`.
9. Attach `api.bitacora.nuestrascuentitas.com`.
10. Attach `bitacora.nuestrascuentitas.com` as a temporary parking host for the backend root route.
11. Deploy `bitacora-api`.
12. Wait for `GET /health/ready` to return `200`.
13. Historical T01 only: run `infra/smoke/backend-smoke.ps1` against the public host. Current post-Zitadel deploys must use `infra/smoke/zitadel-cutover-smoke.ps1`.
14. Verify `https://bitacora.nuestrascuentitas.com/` returns the backend redirect to `/scalar/v1`.

### Fase 3 — Post-deploy

15. Create or verify the daily backup job per `infra/runbooks/backup-and-restore.md`.
16. Confirm observability defaults and incident hooks from `infra/observability/otlp-contract.md`.
17. **Validacion de UI es actividad terminal.** No se marca T01 como completo hasta tener evidencia de prototype validation.

## Control-plane bridge

Use `mi-key-cli` to source control-plane values, Zitadel configuration, and rollback secrets from Infisical. Do not create plaintext secrets in the repository.

Minimum operator sequence:

```powershell
$MKEY = "$HOME\.agents\skills\mi-key-cli\scripts\mkey.ps1"
& $MKEY doctor
& $MKEY status
& $MKEY pull bitacora prod
```

Populate Bitacora `infra/.env` only with the values needed for this repo.

## Failure policy

- If Dokploy auth cannot be materialized locally, stop and record the blocker.
- If DB deploy succeeds but readiness fails, do not open traffic.
- If smoke fails, do not mark T01 complete.
