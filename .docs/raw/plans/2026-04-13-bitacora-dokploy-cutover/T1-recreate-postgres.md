# Task T1: Recreate PostgreSQL

## Shared Context
**Goal:** Provision a fresh `bitacora-db` inside the new `bitacora` Dokploy project.
**Stack:** Dokploy PostgreSQL 18 service, internal-only Docker network.
**Architecture:** New API must point to this DB, not to the old `nuestrascuentitas` project DB.

## Task Metadata
```yaml
id: T1
depends_on: [T0]
agent_type: ps-worker
files:
  - read: infra/dokploy/bitacora-db.production.md:14-31
  - read: /home/fgpaz/.agents/skills/dokploy-cli/references/api-endpoints.md:124-140
complexity: medium
done_when: "A new postgresId exists for bitacora-db and postgres.one returns its internal appName/host"
```

## Reference
`infra/dokploy/bitacora-db.production.md:17-31` gives the canonical create/deploy shape.

## Prompt
1. Use `postgres.create` with:
   - `name: bitacora-db`
   - `projectId: <newProjectId>`
   - `databaseName: bitacora_db`
   - `databaseUser: bitacora`
   - `databasePassword: c3fd62bcf1bd6dba57682a06fbcabf93`
2. Deploy it with `postgres.deploy`.
3. Read it back with `postgres.one`.
4. Extract and report:
   - `postgresId`
   - `appName` (internal host)
   - `databaseName`
   - `databaseUser`
5. Build the connection string for the API task:
   `Host=<appName>;Port=5432;Database=bitacora_db;Username=bitacora;Password=<same-password>`
6. Do not expose the password in the final user-facing summary; keep it only in the app env update call.

## Skeleton
```bash
curl -s -X POST -H "x-api-key: $DOKPLOY_API_KEY" -H "Content-Type: application/json" \
  -d '{"name":"bitacora-db","projectId":"...","databaseName":"bitacora_db","databaseUser":"bitacora","databasePassword":"..."}' \
  "$DOKPLOY_URL/api/postgres.create"
```

## Verify
`curl -s -H "x-api-key: $DOKPLOY_API_KEY" "$DOKPLOY_URL/api/postgres.one?postgresId=<id>"` returns `applicationStatus` not equal to `error`.

## Commit
`chore(infra): recreate bitacora postgres in dedicated Dokploy project`
