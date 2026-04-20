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
