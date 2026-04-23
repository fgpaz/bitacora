# 08 — Modelo Fisico de Datos

## Store principal

| Campo | Valor |
|-------|-------|
| Motor | PostgreSQL 16+ |
| Base de datos | `bitacora_db` (dedicada, credenciales propias) |
| Server | VPS 54.37.157.93 |
| Schema | `public` (unico) |
| ORM | EF Core 10 |
| Migraciones | EF Core Migrations (code-first) |
| Politica prod | Migracion manual explicita antes de abrir readiness |

## Estado de materializacion actual

La migracion `InitialCore` y la migracion `AddBindingCodesAndCareLinks` ya fueron generadas y materializan estas tablas:

- `users`
- `mood_entries`
- `daily_checkins`
- `consent_grants`
- `pending_invites`
- `access_audits`
- `encryption_key_versions`
- `binding_codes` (Wave 30)
- `care_links` (Wave 30)

`telegram_sessions`, `telegram_pairing_codes` y `reminder_configs` fueron materializadas en Wave anterior y permanecen activas en Phase 40 (nueva columna `reminder_timezone` agregada).

`analytics_events` fue agregada 2026-04-23 como follow-up del rediseno login flow (medicion UX impact). SQL plano en `infra/migrations/bitacora/20260423_create_analytics_events.sql`; aplicar en prod siguiendo `infra/runbooks/manual-migrations.md`.

Wave B Zitadel retiro el almacenamiento Supabase mediante `20260420020000_RetireSupabaseAuthSubject` y la migracion SQL manual `infra/migrations/zitadel/20260420_retire_supabase_auth.sql`. La columna fisica activa es `auth_subject`; `supabase_user_id` y `legacy_auth_subject` no forman parte del schema vigente.

T01 congela para produccion una topologia backend-only: una DB dedicada `bitacora-db` y una app `bitacora-api`. La creacion live en Dokploy depende de materializar localmente `infra/.env` con credenciales de control-plane.

## Ownership y safety stance

- Owner de migraciones: `Bitacora.DataAccess.EntityFramework` con `Bitacora.Api` como startup project.
- Safety stance: sin DELETE fisico en tablas clinicas. `mood_entries` y `access_audits` son append-only.
- Backup: snapshot o dump diario + retencion minima de 30 copias verificables.
- Aislamiento (T3-4): DB dedicada; credenciales no compartidas.

## Tablas y ownership

| Tabla | Owner | Patron | Notas | Estado |
|-------|-------|--------|-------|--------|
| users | Auth | CRUD | PII cifrado + `key_version`; `email_hash` para lookup; `auth_subject` Zitadel activo. | Materializada |
| mood_entries | Registro | Append-only | `encrypted_payload + safe_projection`. | Materializada |
| daily_checkins | Registro | Upsert | `UNIQUE(patient_id, checkin_date)`. | Materializada |
| consent_grants | Consent | State machine | `pending → granted → revoked`. | Materializada |
| pending_invites | Vinculos | Temporal/state machine | TTL 7 dias, sin acceso clinico. | Materializada |
| binding_codes | Vinculos | Temporal | `ttl_preset` por codigo; un activo por profesional a nivel app. | Materializada (Wave 30) |
| care_links | Vinculos | State machine | `invited → active → revoked_*`; `can_view_data` default false. | Materializada (Wave 30) |
| telegram_sessions | Telegram | CRUD | `UNIQUE(chat_id) WHERE status='Linked'`. | Materializada |
| telegram_pairing_codes | Telegram | Temporal | TTL 15 min. | Materializada |
| reminder_configs | Telegram | CRUD | Horarios por paciente + zona horaria IANA. | Materializada |
| access_audits | Seguridad | Append-only | `trace_id + pseudonym_id`, sin UPDATE/DELETE. | Materializada |
| encryption_key_versions | Seguridad | Append-only | Key material en vault/env, no en DB. | Materializada |
| analytics_events | Analytics | Append-only (app) + cleanup operacional | `event_name + props_json no-PII`; whitelist en handler; **retention 180d** via cron `DELETE WHERE created_at_utc < NOW() - INTERVAL '180 days'`; decision en `.docs/raw/decisiones/2026-04-23-analytics-retention-policy.md`. | Materializada (2026-04-23 via SQL plano); cron task operacional pendiente de agendamiento en Dokploy |

## Schema Telegram

### reminder_configs (Telegram)

Horarios de recordatorio del bot Telegram por paciente. Materializadas en Wave anterior.

| Columna | Tipo | Constraints | Default | Descripcion |
|---------|------|-------------|---------|-------------|
| id | UUID | PK | gen_random_uuid() | Identificador unico |
| patient_id | UUID | FK(users.user_id), NOT NULL | | Paciente dueno del recordatorio |
| hour | INT | NOT NULL, [0-23] | | Hora del dia (0-23) |
| minute | INT | NOT NULL, {0,30} | | Minuto (0 o 30) |
| reminder_timezone | VARCHAR(64) | NOT NULL | `'America/Argentina/Buenos_Aires'` | Zona horaria IANA del paciente para el recordatorio |
| created_at_utc | TIMESTAMP WITH TZ | NOT NULL | now() | Creacion en UTC |
| updated_at_utc | TIMESTAMP WITH TZ | NOT NULL | now() | Ultima actualizacion en UTC |

Invariante: la UI paciente muestra hora local Buenos Aires, convierte en el boundary frontend a `hour_utc`/`minute_utc` y conserva `reminder_timezone` para display/scheduler. El backend valida UTC y timezone antes de persistir.

## Cifrado en reposo

| Nivel | Que protege | Implementacion |
|-------|-------------|---------------|
| Disk | Todo el volumen | LUKS (VPS level) |
| App-layer (PII) | nombre, email, DNI, telefono en `users` | AES-256 con `key_version` |
| App-layer (clinico) | payload completo en `mood_entries` y `daily_checkins` | `encrypted_payload + safe_projection` |

> Detalle completo: `07_tech/TECH-CIFRADO.md`

## Indices criticos

| Tabla | Indice | Justificacion | Estado |
|-------|--------|---------------|--------|
| users | UNIQUE(auth_subject) | Lookup desde JWT Zitadel | Materializado |
| users | INDEX(email_hash) | Busqueda por email sin descifrar | Materializado |
| mood_entries | INDEX(patient_id, created_at_utc) | Timeline queries | Materializado |
| daily_checkins | UNIQUE(patient_id, checkin_date) | Un checkin por dia | Materializado |
| pending_invites | INDEX(professional_id, invitee_email_hash, status) | Reutilizacion/duplicados de invitacion | Materializado |
| binding_codes | UNIQUE(code) | Lookup del codigo | Materializado (Wave 30) |
| binding_codes | INDEX(professional_id, used, expires_at) | Invalida codigo activo del profesional | Materializado (Wave 30) |
| care_links | INDEX(professional_id, status) | Dashboard profesional | Materializado (Wave 30) |
| care_links | INDEX(patient_id, status) | Lista de vinculos del paciente | Materializado (Wave 30) |
| access_audits | INDEX(patient_id, created_at_utc) | Consulta de auditoria | Materializado |
| telegram_sessions | UNIQUE(chat_id) WHERE status='Linked' | Lookup desde webhook; permite re-vincular sesiones con soft-delete | Materializado |

## Invariantes de timezone

- Toda columna `*_at_utc` y `created_at_utc` en PostgreSQL usa tipo `timestamp with time zone`. Npgsql (driver PostgreSQL para .NET) requiere que los valores `DateTime` tengan `DateTimeKind.Utc`. `DateTimeKind.Unspecified` es rechazado en tiempo de query con: `Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone', only UTC is supported`.
- La conversión de `DateOnly` a `DateTime` para comparaciones debe usar `DateTime.SpecifyKind(..., DateTimeKind.Utc)` explicitamente. `DateOnly.ToDateTime()` produce `Kind=Unspecified`.

## Retencion

| Tabla | Retencion | Justificacion |
|-------|-----------|---------------|
| access_audits | 2 anos minimo | Compliance auditoria |
| mood_entries (score = -3) | 5 anos minimo | Ley 26.657 |
| consent_grants | Permanente | Evidencia legal |
| pending_invites | Hasta expiracion o consumo | Artefacto temporal |
| binding_codes | Hasta expiracion o uso | Artefacto temporal |
| telegram_pairing_codes | Limpieza diaria | Artefacto temporal |

## EF Core Global Query Filters

```csharp
modelBuilder.Entity<MoodEntry>().HasQueryFilter(e => e.PatientId == _currentPatientId);
modelBuilder.Entity<DailyCheckin>().HasQueryFilter(e => e.PatientId == _currentPatientId);
```

> La lectura profesional no usa `patient_ref` persistido; resuelve el target por contrato de aplicacion.

## Detail docs

| Doc | Tema |
|-----|------|
| `08_db/DB-MIGRACIONES-Y-BOOTSTRAP.md` | Estrategia de migraciones EF Core, seeding, cleanup y rollback |

## Sync gates

Cambios en 08 pueden forzar revision de:
- `05_modelo_datos.md` si cambian entidades o campos
- `04_RF/RF-REG-*` si cambian constraints
- `07_baseline_tecnica.md` si cambia estrategia de migraciones

---

*Fuente: `.docs/wiki/05_modelo_datos.md`, `.docs/raw/decisiones/02_decisiones_arquitectura.md`*
