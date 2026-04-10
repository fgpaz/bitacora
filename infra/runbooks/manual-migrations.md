# Manual Production Migrations

Production keeps `DataAccess:ApplyMigrationsOnStartup=false`.

## Command

Run migrations explicitly with the production connection string loaded in the environment:

```powershell
dotnet ef database update `
  --project src/Bitacora.DataAccess.EntityFramework `
  --startup-project src/Bitacora.Api
```

## Required environment

- `ConnectionStrings__BitacoraDb`
- `SUPABASE_JWT_SECRET`
- `BITACORA_ENCRYPTION_KEY`
- `BITACORA_PSEUDONYM_SALT`

## Order

1. Deploy or start `bitacora-db`.
2. Export the production connection string into the current shell.
3. Run the EF command.
4. Only after success, deploy or restart `bitacora-api`.
5. Verify `GET /health/ready`.

## Rollback

- If the migration fails, stop the API deploy and investigate before any traffic is opened.
- If the migration partially applied, restore from the latest verified backup before retrying.
