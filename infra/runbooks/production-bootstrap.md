# Production Bootstrap Runbook

## Scope

This runbook bootstraps the truthful Bitacora production surface for T01:

- dedicated PostgreSQL
- `Bitacora.Api`
- backend-only smoke gate

## Preconditions

1. `infra/.env` exists locally and is not committed.
2. `dkp.ps1 doctor` passes from the Bitacora repo.
3. `sshr hosts test turismo` succeeds.
4. DNS for `api.bitacora.nuestrascuentitas.com` points to `54.37.157.93`.
5. If the root host will be parked on the backend, DNS for `bitacora.nuestrascuentitas.com` also points to `54.37.157.93`.

## Bootstrap order

1. Source approved secrets into local `infra/.env`.
2. Create or discover `BITACORA_PROJECT_ID`.
3. Create and deploy `bitacora-db`.
4. Persist returned DB host metadata into local `infra/.env`.
5. Build `ConnectionStrings__BitacoraDb`.
6. Create and configure `bitacora-api`.
7. Attach `api.bitacora.nuestrascuentitas.com`.
8. Attach `bitacora.nuestrascuentitas.com` as a temporary parking host for the backend root route.
9. Run explicit EF migrations.
10. Deploy `bitacora-api`.
11. Wait for `GET /health/ready` to return `200`.
12. Run `infra/smoke/backend-smoke.ps1` against the public host.
13. Verify `https://bitacora.nuestrascuentitas.com/` returns the backend redirect to `/scalar/v1`.
14. Create or verify the daily backup job.
15. Confirm observability defaults and incident hooks from `infra/observability/otlp-contract.md`.

## Control-plane bridge

Use the shared `mi-key-cli` setup from `C:\repos\mios\multi-tedi` to source control-plane values and the shared auth JWT secret.

Minimum operator sequence:

```powershell
$MKEY = "$HOME\.agents\skills\mi-key-cli\scripts\mkey.ps1"
& $MKEY doctor
& $MKEY status
```

Populate Bitacora `infra/.env` only with the values needed for this repo.

## Failure policy

- If Dokploy auth cannot be materialized locally, stop and record the blocker.
- If DB deploy succeeds but readiness fails, do not open traffic.
- If smoke fails, do not mark T01 complete.
