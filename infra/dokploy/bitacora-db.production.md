# Bitacora PostgreSQL - Production Dokploy Spec

## Target shape

Current production resource ids after the 2026-04-13 cutover:

- Project: `18WEM8BMIq-z_wgkrNlp8`
- Environment: `ULVQy3BehcO0VOH-J-ZVv`
- PostgreSQL: `BZIF_i_IftviCCVnoS9p7`
- Internal host: `postgres-reboot-solid-state-application-l55mww`

- Service name: `bitacora-db`
- Engine: PostgreSQL
- Database name: `bitacora_db`
- Database user: `bitacora`
- Password source: local `infra/.env`
- Owner repo state: none, database is provisioned via Dokploy

## Creation flow

1. Ensure `infra/.env` exists and `BITACORA_PROJECT_ID` is set or create it first.
2. Create the PostgreSQL service if `BITACORA_DB_SERVICE_ID` is blank:

```powershell
& $DKP POST postgres.create '{"name":"bitacora-db","projectId":"<BITACORA_PROJECT_ID>","databaseName":"bitacora_db","databaseUser":"bitacora","databasePassword":"<BITACORA_DB_PASSWORD>"}'
```

3. Deploy it:

```powershell
& $DKP POST postgres.deploy '{"postgresId":"<BITACORA_DB_SERVICE_ID>"}'
```

4. Read back the service metadata and persist the internal host/port in `infra/.env`.
5. Compose the final API connection string:

```text
ConnectionStrings__BitacoraDb=Host=<BITACORA_DB_HOST>;Port=<BITACORA_DB_PORT>;Database=bitacora_db;Username=bitacora;Password=<BITACORA_DB_PASSWORD>
```

## Backup baseline

- Create a Dokploy backup job after the database is healthy.
- Minimum policy:
  - cadence: daily
  - retention: 30 copies minimum
  - prefix: `bitacora-prod`

Manual trigger shape:

```powershell
& $DKP POST backup.manualBackupPostgres '{"postgresId":"<BITACORA_DB_SERVICE_ID>","databaseName":"bitacora_db"}'
```

## Validation

- `sshr exec --host turismo --cmd "docker ps --format '{{.Names}} {{.Status}}' | grep -i bitacora"`
- explicit migration command succeeds before API readiness is opened
