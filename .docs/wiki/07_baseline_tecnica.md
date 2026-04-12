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

- `Wave 1` tiene runtime backend completo con superficies de Vinculos, Visualizacion, Export y Telegram. Estas superficies estan materializadas y operativas.
- Alcance materializado hoy: `Auth`, `Consent`, `Registro`, `Vinculos`, `Visualizacion`, `Export`, `Telegram` y `Seguridad`.
- `frontend/` existe en el repo con `package.json`, `middleware.ts`, `lib/api/professional.ts` y `.next/`. El runtime web completo queda diferido a Phase 40.
- Telegram webhook y scheduler estan implementados en el backend; el bot de Telegram opera como consumidor externo.
- T01 ya materializo la primera capa repo-local de productivizacion bajo `infra/`:
  - contrato local `infra/.env.template`
  - runbooks de bootstrap, migraciones, secretos, humo y backup
  - especificacion Dokploy para `bitacora-api` y `bitacora-db`
- T04 extiende el smoke gate y los runbooks para cubrir las nuevas superficies:
  - `infra/smoke/backend-smoke.ps1` cubre vinculos, visualizacion, export y telegram (GATE-SMOKE-007..015)
  - `infra/runbooks/production-bootstrap.md` referencia el scope completo de superficies
  - gates `GATE-SMOKE-007..015` documentan la cobertura operacional
- ReminderWorker (IHostedService) registrado en Program.cs — scheduler de recordatorios activo en background

## Regla de exploración y navegación

- toda exploración de código bajo `src/` debe comenzar con `mi-lsp`;
- antes de asumir un alias, validar workspace con `mi-lsp workspace list` y `mi-lsp workspace status <alias-o-path> --format toon`;
- si `mi-lsp` devuelve `hint` o `next_hint`, seguir esa guía antes de hacer fallback;
- el fallback a `rg` o lectura manual solo aplica después de intentar `mi-lsp`.

## Middleware pipeline (orden de ejecucion, fail-closed)

El pipeline de middleware de Bitacora.Api sigue este orden obligatorio (Program.cs lineas 349-354):

```
UseRateLimiter()            → fail-closed: 429 si se excede el limite por IP (politica "auth": 10 req/IP/min)
TraceIdMiddleware           → genera trace_id al ingreso si no existe
ApiExceptionMiddleware       → envuelve errores en envelope con trace_id
Correlate                    → propaga X-Correlation-ID
UseAuthentication            → valida JWT Supabase, extrae sub claim
UseAuthorization             → avalia claims de autorizacion
ConsentRequiredMiddleware    → hard gate: bloquea POST /mood-entries y /daily-checkins sin consentimiento activo
```

### Fail-closed runtime rules

| Regla | Descripcion |
|-------|-------------|
| T3-10 | Toda falla de seguridad bloquea la operacion. Authentication y Authorization devuelven 401/403 respectivamente. |
| T3-11 | ConsentRequiredMiddleware es el unico gate de escritura clinica: todo POST a `/api/v1/mood-entries` o `/api/v1/daily-checkins` sin ConsentGrant activo retorna 403 y genera AccessAudit con outcome=Denied. |
| T3-12 | PseudonymizationService fail-closed: si BITACORA_PSEUDONYM_SALT no resuelve, el servicio lanza excepcion y toda operacion que dependa de ella falla con 500. |
| T3-14 | Encryption key fail-closed: si BITACORA_ENCRYPTION_KEY no esta disponible o no resuelve a 32 bytes, GET /health/ready queda en `not_ready`. Ningun dato clinico se escribe sin cifrar. |

## Modulos internos

| Modulo | Responsabilidad | Entidades principales | Estado |
|--------|----------------|----------------------|--------|
| Auth | Validar JWT Supabase, resolver identidad, bootstrap de paciente | User | Implementado |
| Registro | Crear MoodEntry y DailyCheckin, cifrar, `safe_projection` | MoodEntry, DailyCheckin | Implementado |
| Consent | Hard gate, otorgamiento y revocacion de consentimiento | ConsentGrant | Implementado |
| Vinculos | CareLink lifecycle, invitaciones profesionales, aceptacion de binding codes | CareLink, PendingInvite, BindingCode | Implementado |
| Visualizacion | Queries sobre `safe_projection` y lectura profesional via `patient_ref` opaco | (queries) | Implementado |
| Telegram | Webhook entrypoint, pairing code, session state y ReminderWorker (scheduler diferido) | TelegramSession, TelegramPairingCode | Implementado (ReminderWorker activo; `SendTelegramMessageAsync` hace POST real a Telegram Bot API via `HttpClient`; recordatorios scheduler diferidos a Phase 31+) |
| Export | Generar CSV y JSON para paciente | (generacion) | Implementado |
| Seguridad | Cifrado AES-GCM, audit, pseudonimizacion, fail-closed | AccessAudit, EncryptionKeyVersion | Implementado |

## Infraestructura

| Componente | Tecnologia | Ubicacion |
|-----------|-----------|-----------|
| Backend | .NET 10 (Bitacora.Api) | Runnable local hoy; target prod-first en Dokploy sobre `turismo` |
| Base de datos | PostgreSQL (`bitacora_db` dedicada) | Local/dev y target Dokploy dedicado en el mismo VPS |
| Auth | Supabase Auth (GoTrue) | auth.tedi.nuestrascuentitas.com |
| Reverse proxy | Traefik (via Dokploy) | Target de produccion |
| Dominio API | `api.bitacora.nuestrascuentitas.com` | Target backend-only de T01 |
| Dominio web | `bitacora.nuestrascuentitas.com` | `frontend/` existe con implementacion inicial bajo `frontend/`; deployment completo a `bitacora.nuestrascuentitas.com` planeado para Phase 40. Mientras tanto, `GET /` en `Bitacora.Api` redirige a `/scalar/v1`. |
| Frontend | Next.js 16 | Implementacion inicial existente en `frontend/`; deployment a production diferido a Phase 40 |

## Observabilidad minima

| Aspecto | Implementacion |
|---------|---------------|
| Logs | `Console` + `Debug` providers del host .NET |
| Tracing | OpenTelemetry con `trace_id` end-to-end; OTLP deshabilitado por default hasta configurar endpoint |
| Pseudonimizacion | `pseudonym_id` en logs operacionales; `actor_id` solo en `AccessAudit` |
| Liveness | `GET /health` |
| Readiness | `GET /health/ready` valida connection string, `SUPABASE_JWT_SECRET`, clave de cifrado, salt y conectividad PostgreSQL |
| Smoke operativo | `infra/smoke/backend-smoke.ps1` cubre la superficie backend completa (auth, consent, registro, vinculos, visualizacion, export, telegram) sin staging |

## Invariantes operacionales

1. Fail-closed (T3-10): toda falla de seguridad bloquea la operacion.
2. `trace_id` obligatorio (T3-13): se genera al ingreso si no existe.
3. Cifrado app-layer (T3-5): `encrypted_payload + safe_projection`. Ver `07_tech/TECH-CIFRADO.md`.
4. `EventBusSettings:HostAddress` vacio mantiene el runtime en `NoOp`; RabbitMQ sigue fuera del bootstrap de T01.
5. Global Query Filters (T3-6): EF Core filtra por `patient_id` automaticamente en `MoodEntry` y `DailyCheckin`.
6. Consentimiento activo en config: el texto vigente no se persiste en DB; en DB solo queda la evidencia aceptada (`ConsentGrant`).
7. `patient_ref` es una proyeccion opaca de API, no un campo persistido del modelo.
8. `DataAccess:ApplyMigrationsOnStartup=false` se preserva para produccion; las migraciones son explicitas.
9. **ConsentRequiredMiddleware es el hard gate de registro:** toda peticion POST a `/api/v1/mood-entries` y `/api/v1/daily-checkins` sin consentimiento activo retorna 403 y genera AccessAudit con outcome=Denied.
10. **Sin fuga de datos clinicos a Telegram:** ninguna respuesta del bot Telegram puede incluir contenido de encrypted_payload, safe_projection con datos clinicos, o cualquier informacion derivada de registros del paciente. El bot solo confirma receipt y solicita proximo input.
11. **Supresion irreversible:** cuando un paciente ejercita su derecho de supresion, User.status='anonymized' + destruccion de la EncryptionKey activa para ese usuario. AccessAudit se retiene por su propio periodo regulatorio (2 anos minimo).
12. **PseudonymizationService fail-closed:** si BITACORA_PSEUDONYM_SALT no resuelve, el servicio lanza excepcion y toda operacion que dependa de ella falla con 500.

## Detail docs

| Doc | Tema |
|-----|------|
| `07_tech/TECH-CIFRADO.md` | Patron encrypted_payload + safe_projection + key rotation |
| `07_tech/TECH-TELEGRAM.md` | Modos webhook/polling, flujo conversacional, pairing y recordatorios del canal diferido |
| `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` | Gramatica tecnica de la futura UI Next.js 16, estados, accesibilidad y trazabilidad Stitch |
| `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md` | Orden de rollout, fail-closed runtime gates, limites de Telegram, smoke terminal, secretos y config runtime |

## Decision register

| Fecha | Decision | Doc |
|-------|----------|-----|
| 2026-04-10 | **Hardening exceptions register + go/no-go** | `.docs/raw/decisiones/2026-04-10-wave-prod-hardening-exceptions-register.md` |

## Sync gates

Cambios en 07 pueden forzar revision de:

- `04_RF/RF-SEC-*` si cambia fail-closed o pseudonimizacion
- `04_RF/RF-REG-*` si cambia cifrado o Global Query Filters
- `09_contratos_tecnicos.md` si cambia auth, readiness o endpoints
- `08_modelo_fisico_datos.md` si cambia estrategia de migraciones o backup
- `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` si cambian error envelopes, auth, estados sensibles o reglas frontend de acceso

---

*Fuente: `.docs/raw/decisiones/02_decisiones_arquitectura.md`, `infra/runbooks/production-bootstrap.md`, `src/Bitacora.Api/Program.cs`*
