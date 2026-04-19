# Manual Production Migrations

Production keeps `DataAccess:ApplyMigrationsOnStartup=false`.

## Command

Run migrations explicitly with the production connection string loaded in the environment:

```powershell
dotnet ef database update `
  --project src/Bitacora.DataAccess.EntityFramework `
  --startup-project src/Bitacora.Api
```

If PostgreSQL is reachable only from Dokploy internal networking, execute the migration from `turismo`
using the checked-out application code and an ephemeral SDK container that shares the live app network namespace:

```bash
container=$(docker ps --filter name=app-input-neural-matrix-psstrb --format "{{.Names}}" | sed -n 1p)
workdir=/etc/dokploy/applications/app-input-neural-matrix-psstrb/code
envfile=$(mktemp)

docker service inspect app-input-neural-matrix-psstrb \
  --format '{{range .Spec.TaskTemplate.ContainerSpec.Env}}{{println .}}{{end}}' > "$envfile"

docker run --rm \
  --network container:$container \
  --env-file "$envfile" \
  -v "$workdir:/workspace" \
  -w /workspace/src \
  --entrypoint /bin/bash \
  mcr.microsoft.com/dotnet/sdk:10.0 \
  -lc 'dotnet restore Bitacora.sln >/dev/null \
    && dotnet tool install --tool-path /tmp/dotnet-tools dotnet-ef --version 10.0.3 >/dev/null \
    && /tmp/dotnet-tools/dotnet-ef database update --project Bitacora.DataAccess.EntityFramework --startup-project Bitacora.Api --verbose'

rm -f "$envfile"
```

## Required environment

- `ConnectionStrings__BitacoraDb`
- `ZITADEL_AUTHORITY`
- `ZITADEL_AUDIENCE`
- `BITACORA_ENCRYPTION_KEY`
- `BITACORA_PSEUDONYM_SALT`
- `SUPABASE_JWT_SECRET` only if executing a rollback build

## Wave B cutover note

Migration `20260419000001_AddLegacyAuthSubject` was applied in production on 2026-04-19 before deploying commit `b0d876c`. It adds `users.legacy_auth_subject`, copies existing subjects for rollback, and preserves the physical `supabase_user_id` column as the active `auth_subject` storage during the rollback window.

## Order

1. Deploy or start `bitacora-db`.
2. Materialize the runtime secrets and connection string in Dokploy.
3. If the DB is reachable from the operator shell, run `dotnet ef database update` locally.
4. If the DB is internal-only, run the remote SDK-container flow on `turismo`.
5. Only after success, deploy or restart `bitacora-api`.
6. Verify `GET /health/ready`.

## Rollback

- If the migration fails, stop the API deploy and investigate before any traffic is opened.
- If the migration partially applied, restore from the latest verified backup before retrying.
