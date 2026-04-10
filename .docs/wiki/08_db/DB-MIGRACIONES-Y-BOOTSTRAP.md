# DB-MIGRACIONES-Y-BOOTSTRAP: Estrategia de Migraciones EF Core

> Root: `08_modelo_fisico_datos.md` seccion ownership y safety stance.

## Estrategia

| Campo | Valor |
|-------|-------|
| Herramienta | EF Core Migrations (code-first) |
| Aplicacion | `dotnet ef database update` o `Database.Migrate()` condicionado |
| Dev | Hook de `Database.Migrate()` implementado, pero `DataAccess:ApplyMigrationsOnStartup=false` por defecto |
| Prod | Manual via CLI antes del deploy o del restart productivo |
| Rollback | Script de down-migration generado por EF Core |

## Estado actual

- Migracion inicial generada: `20260409185502_InitialCore`.
- Assembly owner: `Bitacora.DataAccess.EntityFramework`.
- Startup project para tooling: `Bitacora.Api`.
- La ola actual no materializa aun `binding_codes`, `care_links`, `telegram_*` ni `reminder_configs`.
- El gate operativo de produccion queda en `GET /health/ready`; no se abre trafico hasta pasar migracion + readiness + smoke.

## Orden de bootstrap productivo

1. Provisionar `bitacora-db`.
2. Construir `ConnectionStrings__BitacoraDb` final.
3. Materializar secretos de runtime (`SUPABASE_JWT_SECRET`, `BITACORA_ENCRYPTION_KEY`, `BITACORA_PSEUDONYM_SALT`).
4. Ejecutar `dotnet ef database update`.
5. Desplegar `bitacora-api`.
6. Verificar `GET /health/ready`.
7. Ejecutar `infra/smoke/backend-smoke.ps1`.

## Seeding

### Datos iniciales requeridos

| Tabla | Seed | Descripcion |
|-------|------|-------------|
| encryption_key_versions | 1 registro | key_version = 1, is_active = true. Key material en env. |
| (consent text) | Config file | Texto de consentimiento v1.0. No en DB, en appsettings. |
| users.key_version | Default 1 | Version inicial para PII cifrada de nuevos usuarios |

### Datos NO seeded

- Users, MoodEntries, DailyCheckins: nunca se seedean.
- PendingInvites, BindingCodes, CareLinks: nunca se seedean.
- AccessAudits: generados por operacion, nunca seeded.

## Reglas de migracion

1. **Nunca DELETE column en tablas clinicas** sin migracion de datos previa.
2. **Nunca ALTER column type** en encrypted_payload o safe_projection.
3. **Agregar columnas** con default value para no romper registros existentes.
4. **Indices nuevos** se crean con `CREATE INDEX CONCURRENTLY` (via migracion manual en prod).
5. **Rename** de columnas: crear nueva, migrar datos, deprecar vieja en siguiente release.

## Cleanup periodico

| Tabla | Accion | Frecuencia |
|-------|--------|-----------|
| pending_invites | UPDATE status='expired' WHERE expires_at < now() AND status='issued' | Diario (background job) |
| binding_codes | DELETE o archive WHERE expires_at < now() OR used = true | Diario (background job) |
| telegram_pairing_codes | DELETE WHERE expires_at < now() AND used = false | Diario (background job) |
| access_audits | Archivado a cold storage despues de 2 anos | Mensual |

## Backup y restore

- Antes de una migracion productiva se fuerza un backup manual del PostgreSQL dedicado.
- La politica minima es diaria con retencion de 30 copias verificables.
- El restore drill debe probarse sobre un dump o snapshot reciente antes de cerrar la ola de productivizacion.

## Sync gates

Cambios en migraciones fuerzan revision de:
- `05_modelo_datos.md` si cambian entidades
- `08_modelo_fisico_datos.md` si cambian constraints o indices
