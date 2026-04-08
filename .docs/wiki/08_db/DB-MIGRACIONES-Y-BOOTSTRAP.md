# DB-MIGRACIONES-Y-BOOTSTRAP: Estrategia de Migraciones EF Core

> Root: `08_modelo_fisico_datos.md` seccion ownership y safety stance.

## Estrategia

| Campo | Valor |
|-------|-------|
| Herramienta | EF Core Migrations (code-first) |
| Aplicacion | `dotnet ef database update` |
| Dev | Auto-migrate al arrancar (Database.Migrate() en Program.cs) |
| Prod | Manual via CLI o pipeline antes del deploy |
| Rollback | Script de down-migration generado por EF Core |

## Seeding

### Datos iniciales requeridos

| Tabla | Seed | Descripcion |
|-------|------|-------------|
| encryption_key_versions | 1 registro | key_version = 1, is_active = true. Key material en env. |
| (consent text) | Config file | Texto de consentimiento v1.0. No en DB, en appsettings. |

### Datos NO seeded

- Users, MoodEntries, DailyCheckins: nunca se seedean.
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
| telegram_pairing_codes | DELETE WHERE expires_at < now() AND used = false | Diario (background job) |
| access_audits | Archivado a cold storage despues de 2 anos | Mensual |

## Sync gates

Cambios en migraciones fuerzan revision de:
- `05_modelo_datos.md` si cambian entidades
- `08_modelo_fisico_datos.md` si cambian constraints o indices
