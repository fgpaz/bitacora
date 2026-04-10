# 07 — Baseline Tecnica

## Servicio

| Campo | Valor |
|-------|-------|
| Nombre | Bitacora.Api |
| Tipo | Monolito modular |
| Runtime | .NET 10 |
| Template | `sandinas-ms10` (local full skeleton) |
| Subagente | ps-dotnet10 |
| Puerto local | `5254` HTTP / `7281` HTTPS via `launchSettings.json` |
| Artefacto de deploy | `Dockerfile` (Dokploy) + `src/Bitacora.Api/Dockerfile` (copia local del servicio) |
| Health checks | `GET /health` (liveness), `GET /health/ready` (readiness) |

## Estado actual de implementacion

- `Wave 1` hoy tiene runtime backend-only.
- Alcance materializado hoy: `Auth`, `Consent`, `Registro` y `Seguridad`.
- `frontend/` no existe en el repo y no forma parte del runtime actual.
- Telegram sigue documentado en el canon, pero no existe runtime real en esta sesion.
- T01 ya materializo la primera capa repo-local de productivizacion bajo `infra/`:
  - contrato local `infra/.env.template`
  - runbooks de bootstrap, migraciones, secretos, humo y backup
  - especificacion Dokploy para `bitacora-api` y `bitacora-db`

## Modulos internos

| Modulo | Responsabilidad | Entidades principales | Estado |
|--------|----------------|----------------------|--------|
| Auth | Validar JWT Supabase, resolver identidad, bootstrap de paciente | User | Implementado |
| Registro | Crear MoodEntry y DailyCheckin, cifrar, `safe_projection` | MoodEntry, DailyCheckin | Implementado |
| Consent | Hard gate, otorgamiento y revocacion de consentimiento | ConsentGrant | Implementado |
| Vinculos | Detectar `PendingInvite` reanudable desde onboarding | PendingInvite | Parcial |
| Visualizacion | Queries sobre `safe_projection` y lectura profesional via `patient_ref` opaco | (queries) | Diferido |
| Telegram | Webhook/polling, pairing, sesiones y recordatorios | TelegramSession, TelegramPairingCode | Diferido |
| Export | Generar CSV, descifrar payload bajo demanda | (generacion) | Diferido |
| Seguridad | Cifrado AES-GCM, audit, pseudonimizacion, fail-closed | AccessAudit, EncryptionKeyVersion | Implementado |

## Infraestructura

| Componente | Tecnologia | Ubicacion |
|-----------|-----------|-----------|
| Backend | .NET 10 (Bitacora.Api) | Runnable local hoy; target prod-first en Dokploy sobre `turismo` |
| Base de datos | PostgreSQL (`bitacora_db` dedicada) | Local/dev y target Dokploy dedicado en el mismo VPS |
| Auth | Supabase Auth (GoTrue) | auth.tedi.nuestrascuentitas.com |
| Reverse proxy | Traefik (via Dokploy) | Target de produccion |
| Dominio API | `api.bitacora.nuestrascuentitas.com` | Target backend-only de T01 |
| Dominio web | `bitacora.nuestrascuentitas.com` | Reservado para futuro runtime web |
| Frontend | Next.js 16 | Planeado para T04/T05, sin runtime hoy |

## Observabilidad

| Aspecto | Implementacion |
|---------|---------------|
| Logs | `Console` + `Debug` providers del host .NET |
| Tracing | OpenTelemetry con `trace_id` end-to-end; OTLP deshabilitado por default hasta configurar endpoint |
| Pseudonimizacion | `pseudonym_id` en logs operacionales; `actor_id` solo en `AccessAudit` |
| Liveness | `GET /health` |
| Readiness | `GET /health/ready` valida connection string, `SUPABASE_JWT_SECRET`, clave de cifrado, salt y conectividad PostgreSQL |
| Smoke operativo | `infra/smoke/backend-smoke.ps1` cubre la superficie backend actual sin staging |

## Invariantes operacionales

1. Fail-closed (T3-10): toda falla de seguridad bloquea la operacion.
2. `trace_id` obligatorio (T3-13): se genera al ingreso si no existe.
3. Cifrado app-layer (T3-5): `encrypted_payload + safe_projection`. Ver `07_tech/TECH-CIFRADO.md`.
4. `EventBusSettings:HostAddress` vacio mantiene el runtime en `NoOp`; RabbitMQ sigue fuera del bootstrap de T01.
5. Global Query Filters (T3-6): EF Core filtra por `patient_id` automaticamente en `MoodEntry` y `DailyCheckin`.
6. Consentimiento activo en config: el texto vigente no se persiste en DB; en DB solo queda la evidencia aceptada (`ConsentGrant`).
7. `patient_ref` es una proyeccion opaca de API, no un campo persistido del modelo.
8. `DataAccess:ApplyMigrationsOnStartup=false` se preserva para produccion; las migraciones son explicitas.

## Detail docs

| Doc | Tema |
|-----|------|
| `07_tech/TECH-CIFRADO.md` | Patron encrypted_payload + safe_projection + key rotation |
| `07_tech/TECH-TELEGRAM.md` | Modos webhook/polling, flujo conversacional, pairing y recordatorios |
| `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` | Gramatica tecnica de la futura UI Next.js 16, estados, accesibilidad y trazabilidad Stitch |

## Sync gates

Cambios en 07 pueden forzar revision de:

- `04_RF/RF-SEC-*` si cambia fail-closed o pseudonimizacion
- `04_RF/RF-REG-*` si cambia cifrado o Global Query Filters
- `09_contratos_tecnicos.md` si cambia auth, readiness o endpoints
- `08_modelo_fisico_datos.md` si cambia estrategia de migraciones o backup
- `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` si cambian error envelopes, auth, estados sensibles o reglas frontend de acceso

---

*Fuente: `.docs/raw/decisiones/02_decisiones_arquitectura.md`, `infra/runbooks/production-bootstrap.md`*
