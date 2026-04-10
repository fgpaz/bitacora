# PostgreSQL Backup and Restore

## Baseline

- Target service: `bitacora-db`
- Database: `bitacora_db`
- Minimum cadence: daily
- Minimum retention: 30 verified copies
- Additional rule: trigger a manual backup before every production migration

## Backup ownership

- Primary operator: the T01 production owner running Dokploy bootstrap
- Control plane: Dokploy backup endpoint for PostgreSQL
- Remote evidence: verify the service exists on `turismo`

## Manual backup

```powershell
& $DKP POST backup.manualBackupPostgres '{"postgresId":"<BITACORA_DB_SERVICE_ID>","databaseName":"bitacora_db"}'
```

Record the timestamp and keep it with the release evidence.

## Restore expectation

1. Stop or isolate `bitacora-api` traffic.
2. Restore the most recent verified backup or snapshot.
3. Re-run explicit migrations only if the restored snapshot predates the intended schema.
4. Verify `GET /health/ready`.
5. Re-run `infra/smoke/backend-smoke.ps1` before reopening traffic.

## Blocking conditions

- No verified recent backup exists.
- Restore ownership is ambiguous.
- Readiness or smoke stays red after restore.
