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

`telegram_sessions`, `telegram_pairing_codes` y `reminder_configs` siguen en el canon de alcance, pero todavia no existen en la base fisica de esta ola.

T01 congela para produccion una topologia backend-only: una DB dedicada `bitacora-db` y una app `bitacora-api`. La creacion live en Dokploy depende de materializar localmente `infra/.env` con credenciales de control-plane.

## Ownership y safety stance

- Owner de migraciones: `Bitacora.DataAccess.EntityFramework` con `Bitacora.Api` como startup project.
- Safety stance: sin DELETE fisico en tablas clinicas. `mood_entries` y `access_audits` son append-only.
- Backup: snapshot o dump diario + retencion minima de 30 copias verificables.
- Aislamiento (T3-4): DB dedicada; credenciales no compartidas.

## Tablas y ownership

| Tabla | Owner | Patron | Notas | Estado |
|-------|-------|--------|-------|--------|
| users | Auth | CRUD | PII cifrado + `key_version`; `email_hash` para lookup. | Materializada |
| mood_entries | Registro | Append-only | `encrypted_payload + safe_projection`. | Materializada |
| daily_checkins | Registro | Upsert | `UNIQUE(patient_id, checkin_date)`. | Materializada |
| consent_grants | Consent | State machine | `pending → granted → revoked`. | Materializada |
| pending_invites | Vinculos | Temporal/state machine | TTL 7 dias, sin acceso clinico. | Materializada |
| binding_codes | Vinculos | Temporal | `ttl_preset` por codigo; un activo por profesional a nivel app. | Materializada (Wave 30) |
| care_links | Vinculos | State machine | `invited → active → revoked_*`; `can_view_data` default false. | Materializada (Wave 30) |
| telegram_sessions | Telegram | CRUD | `UNIQUE(chat_id)`. | Materializada |
| telegram_pairing_codes | Telegram | Temporal | TTL 15 min. | Materializada |
| reminder_configs | Telegram | CRUD | Horarios por paciente. | Materializada |
| access_audits | Seguridad | Append-only | `trace_id + pseudonym_id`, sin UPDATE/DELETE. | Materializada |
| encryption_key_versions | Seguridad | Append-only | Key material en vault/env, no en DB. | Materializada |

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
| users | UNIQUE(supabase_user_id) | Lookup desde JWT | Materializado |
| users | INDEX(email_hash) | Busqueda por email sin descifrar | Materializado |
| mood_entries | INDEX(patient_id, created_at_utc) | Timeline queries | Materializado |
| daily_checkins | UNIQUE(patient_id, checkin_date) | Un checkin por dia | Materializado |
| pending_invites | INDEX(professional_id, invitee_email_hash, status) | Reutilizacion/duplicados de invitacion | Materializado |
| binding_codes | UNIQUE(code) | Lookup del codigo | Materializado (Wave 30) |
| binding_codes | INDEX(professional_id, used, expires_at) | Invalida codigo activo del profesional | Materializado (Wave 30) |
| care_links | INDEX(professional_id, status) | Dashboard profesional | Materializado (Wave 30) |
| care_links | INDEX(patient_id, status) | Lista de vinculos del paciente | Materializado (Wave 30) |
| access_audits | INDEX(patient_id, created_at_utc) | Consulta de auditoria | Materializado |
| telegram_sessions | UNIQUE(chat_id) | Lookup desde webhook | Materializado |

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
