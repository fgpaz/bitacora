# 08 — Modelo Fisico de Datos

## Store principal

| Campo | Valor |
|-------|-------|
| Motor | PostgreSQL 16+ |
| Base de datos | `bitacora_db` (dedicada, credenciales propias) |
| Server | VPS 54.37.157.93 (mismo que multi-tedi, DB separada) |
| Schema | `public` (unico) |
| ORM | EF Core 10 |
| Migraciones | EF Core Migrations (code-first) |

## Ownership y safety stance

- **Owner de migraciones:** Bitacora.Api (EF Core aplica migraciones al arrancar en dev, manual en prod).
- **Safety stance:** Sin DELETE fisico en tablas clinicas. MoodEntry es append-only. AccessAudit es append-only.
- **Backup:** Independiente de multi-tedi. pg_dump diario + retencion 30 dias.
- **Aislamiento (T3-4):** DB dedicada. Credenciales no compartidas con otros servicios.

## Tablas y ownership

| Tabla | Owner | Patron | Notas |
|-------|-------|--------|-------|
| users | Auth | CRUD | PII cifrado. email_hash para lookup. |
| mood_entries | Registro | Append-only | encrypted_payload + safe_projection. Inmutable. |
| daily_checkins | Registro | Upsert | UNIQUE(patient_id, checkin_date). encrypted_payload + safe_projection. |
| consent_grants | Consent | State machine | pending → granted → revoked. |
| care_links | Vinculos | State machine | invited → active → revoked_*. can_view_data default false. |
| telegram_sessions | Telegram | CRUD | UNIQUE(chat_id). |
| telegram_pairing_codes | Telegram | Temporal | TTL 15min. Se elimina al usar. |
| reminder_configs | Telegram | CRUD | Horarios de recordatorio por paciente. |
| access_audits | Seguridad | Append-only | Sin UPDATE/DELETE. trace_id + pseudonym_id. |
| encryption_key_versions | Seguridad | Append-only | Key material en vault/env, no en DB. |

## Cifrado en reposo

| Nivel | Que protege | Implementacion |
|-------|-------------|---------------|
| Disk | Todo el volumen | LUKS (VPS level) |
| App-layer (PII) | nombre, email, DNI, telefono en `users` | AES-256 con key_version |
| App-layer (clinico) | payload completo en mood_entries y daily_checkins | encrypted_payload + safe_projection |

> Detalle completo: `07_tech/TECH-CIFRADO.md`

## Indices criticos

| Tabla | Indice | Justificacion |
|-------|--------|---------------|
| users | UNIQUE(supabase_user_id) | Lookup desde JWT |
| users | INDEX(email_hash) | Busqueda por email sin descifrar |
| mood_entries | INDEX(patient_id, created_at_utc) | Timeline queries |
| daily_checkins | UNIQUE(patient_id, checkin_date) | Constraint uno por dia |
| care_links | INDEX(professional_id, status) | Dashboard profesional |
| care_links | INDEX(patient_id, status) | Lista de vinculos del paciente |
| access_audits | INDEX(patient_id, created_at_utc) | Consulta de auditoria |
| telegram_sessions | UNIQUE(chat_id) | Lookup desde webhook |

## Retencion

| Tabla | Retencion | Justificacion |
|-------|-----------|---------------|
| access_audits | 2 anos minimo | Compliance auditoria |
| mood_entries (score = -3) | 5 anos minimo | Ley 26.657 (salud mental) |
| consent_grants | Permanente | Evidencia legal |
| telegram_pairing_codes | Limpieza diaria (expired) | Temporal |

## EF Core Global Query Filters

```csharp
// Aplicado en DbContext.OnModelCreating
modelBuilder.Entity<MoodEntry>().HasQueryFilter(e => e.PatientId == _currentPatientId);
modelBuilder.Entity<DailyCheckin>().HasQueryFilter(e => e.PatientId == _currentPatientId);
```

> Migracion a RLS de PostgreSQL en Roadmap.

## Detail docs

| Doc | Tema |
|-----|------|
| `08_db/DB-MIGRACIONES-Y-BOOTSTRAP.md` | Estrategia de migraciones EF Core, seeding, rollback |

## Sync gates

Cambios en 08 pueden forzar revision de:
- `05_modelo_datos.md` si cambian entidades o campos
- `04_RF/RF-REG-*` si cambian constraints
- `07_baseline_tecnica.md` si cambia estrategia de migraciones

---

*Fuente: `.docs/wiki/05_modelo_datos.md`, `.docs/raw/decisiones/02_decisiones_arquitectura.md`*
