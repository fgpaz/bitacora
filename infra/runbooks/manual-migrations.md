# Manual Production Migrations

Production keeps `DataAccess:ApplyMigrationsOnStartup=false`.

## Command

Run migrations explicitly with the production connection string loaded in the environment.

For the final Zitadel-only auth cleanup, apply the checked-in SQL script:

```powershell
psql "$env:ConnectionStrings__BitacoraDb" -f .\infra\migrations\zitadel\20260420_retire_supabase_auth.sql
```

If PostgreSQL is reachable only from Dokploy internal networking, execute the migration from `turismo` using a temporary file and `psql` from a PostgreSQL client container on the live app network.

## Required environment

- `ConnectionStrings__BitacoraDb`
- `ZITADEL_AUTHORITY`
- `ZITADEL_AUDIENCE`
- `BITACORA_ENCRYPTION_KEY`
- `BITACORA_PSEUDONYM_SALT`

## Zitadel-Only Auth Subject Cleanup

Script `infra/migrations/zitadel/20260420_retire_supabase_auth.sql`:

- drops rollback-only identity indexes if present;
- renames the physical user identity column to `auth_subject`;
- drops the rollback-only legacy subject column;
- creates `IX_users_auth_subject` as the unique lookup index.

## Analytics events (2026-04-23)

Script `infra/migrations/bitacora/20260423_create_analytics_events.sql`:

- creates `analytics_events` table (append-only via application).
- adds indexes `IX_analytics_events_event_name_created_at_utc` and `IX_analytics_events_patient_id_created_at_utc`.
- `props_json` is `jsonb` with no-PII enforcement at caller responsibility.

### Apply (dev or prod)

Helper convenience:

```powershell
# Dev (connection string loaded via mkey pull or export):
bash scripts/apply-analytics-migration.sh

# Prod:
$env:BITACORA_ENV = "prod"
bash scripts/apply-analytics-migration.sh
```

Manual equivalent:

```powershell
psql "$env:ConnectionStrings__BitacoraDb" -v ON_ERROR_STOP=1 -f .\infra\migrations\bitacora\20260423_create_analytics_events.sql
```

Verify:

```powershell
psql "$env:ConnectionStrings__BitacoraDb" -c "SELECT COUNT(*) FROM analytics_events;"
```

Rollback (if needed before any inserts):

```powershell
psql "$env:ConnectionStrings__BitacoraDb" -c "DROP TABLE IF EXISTS analytics_events;"
```

### Retention policy

180d rolling window via cron task. Decision doc: `.docs/raw/decisiones/2026-04-23-analytics-retention-policy.md`. Cron task operacional queda pendiente de agendamiento en Dokploy tras ratificación clínico-legal.

## Order

1. Verify a recent DB backup exists.
2. Materialize runtime secrets and the production connection string.
3. Apply the SQL script.
4. Deploy or restart `bitacora-api`.
5. Verify `GET /health/ready`.
6. Run `infra/smoke/zitadel-cutover-smoke.ps1`.

## Rollback

- If the migration fails before commit, stop the API deploy and investigate.
- If the migration partially applied, restore from the latest verified backup before retrying.
- Application rollback uses the previous Git commit/image plus DB backup, not Supabase Auth.
