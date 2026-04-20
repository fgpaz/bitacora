# 07 — Baseline Tecnica

> **Nota de sensibilidad:** Bitacora procesa datos de salud mental bajo regimen de tres leyes argentinas: Ley 25.326 (Proteccion de Datos Personales), Ley 26.529 (Derechos del Paciente) y Ley 26.657 (Salud Mental). Toda decision de implementacion, configuracion de infraestructura o cambios en flujo de datos debe respetar estos tres invariantes como requisito no negociable.

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
- `Wave B Zitadel` quedo deployada en produccion el 2026-04-19: frontend OIDC Authorization Code + PKCE, backend JWT Bearer RS256 via JWKS, y link-on-first-login por `email_hash`.
- Alcance materializado hoy: `Auth`, `Consent`, `Registro`, `Vinculos`, `Visualizacion`, `Export`, `Telegram` y `Seguridad`.
- `frontend/` implementado y deployado en `bitacora.nuestrascuentitas.com` (Phase 40 EJECUTADA 2026-04-15). Incluye dashboard paciente, vinculos, logout, Telegram wizard completo, health check y error boundaries.
- Telegram webhook y scheduler estan implementados en el backend; el bot de Telegram opera como consumidor externo.
- T01 ya materializo la primera capa repo-local de productivizacion bajo `infra/`:
  - contrato local `infra/.env.template`
  - runbooks de bootstrap, migraciones, secretos, humo y backup
  - especificacion Dokploy para `bitacora-api` y `bitacora-db`
- T04 extiende el smoke gate y los runbooks para cubrir las nuevas superficies:
  - `infra/smoke/zitadel-cutover-smoke.ps1` cubre la salud productiva y el inicio OIDC como smoke no interactivo.
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
UseAuthentication            → valida JWT Zitadel RS256 via JWKS, extrae sub claim
UseAuthorization             → avalia claims de autorizacion
ConsentRequiredMiddleware    → hard gate: bloquea POST /mood-entries y /daily-checkins sin consentimiento activo
```

### Fail-closed runtime rules

| Regla | Descripcion |
|-------|-------------|
| T3-10 | Toda falla de seguridad bloquea la operacion. Authentication y Authorization devuelven 401/403 respectivamente. |
| T3-11 | ConsentRequiredMiddleware es el unico gate de escritura clinica: todo POST a `/api/v1/mood-entries` o `/api/v1/daily-checkins` sin ConsentGrant activo retorna 403 y genera AccessAudit con outcome=Denied. |
| T3-11b | ConsentRequiredMiddleware usa deny-list explícita: todo POST a `/api/v1/mood-entries` o `/api/v1/daily-checkins` esta protegido; las demas rutas POST bajo `/api/v1/` que NO empiecen con `/api/v1/auth`, `/api/v1/consent`, `/api/v1/telegram` o `/api/v1/export` se asumen clinicas y se bloquean si no hay ConsentGrant activo. |
| T3-12 | PseudonymizationService fail-closed: si BITACORA_PSEUDONYM_SALT no resuelve, el servicio lanza excepcion y toda operacion que dependa de ella falla con 500. |
| T3-14 | Encryption key fail-closed: si BITACORA_ENCRYPTION_KEY no esta disponible o no resuelve a 32 bytes, GET /health/ready queda en `not_ready`. Ningun dato clinico se escribe sin cifrar. |
| T3-RL-01 | Rate limiting fail-closed: politica `auth` 10 req/IP/min; cualquier exceso devuelve 429 + `Retry-After: 60` (segundos fijo, fijo en OnRejected de Program.cs). |
| T3-RL-02 | Telegram reminder throttle: max 1 recordatorio por paciente por dia (sin importar la configuracion en ReminderConfig). |
| T3-RL-03 | Consent revocado corta inmediatamente el recordatorio: ReminderWorker checkea ConsentGrant activo antes de cada envio. |
| T3-RL-04 | Auth bootstrap usa policy rate limiting `auth` (no `write`). |
| T3-RL-05 | Health/ready endpoint respeta el rate limiter (politica `auth`). |
| T3-SEC-10 | ProfessionalDataAccessAuthorizer fail-closed: lanza 403 en vez de revelar datos cuando el profesional no tiene CareLink autorizado con el paciente. No hay fuga de existencia. |
| T3-SEC-11 | Frontend Proxy valida la sesion OIDC `bitacora_session` y deriva rol desde `urn:zitadel:iam:org:project:roles`; enforce rol `professional` para rutas profesionales; falla cerrado si el rol no corresponde. |
| T3-TG-01 | Telegram API client retry con exponential backoff: 1s, 2s, 4s antes de fallar. |
| T3-TG-02 | SendReminderCommand y HandleWebhookUpdateCommand invocan SaveChangesAsync para persistir AccessAudit antes de retornar. |
| T3-TG-03 | `DELETE /api/v1/telegram/session` requiere auth valido + rate-limit `"write"` (5 req/IP/min). Marca la TelegramSession activa como `Unlinked` para el usuario. |
| T3-TG-04 | `PUT /api/v1/telegram/reminder-schedule` requiere auth valido + rate-limit `"write"` + `TelegramSession` linked. `GET /api/v1/telegram/reminder-schedule` devuelve la configuracion owner-only o `configured=false` sin exponer `chat_id`. La UI convierte hora local Buenos Aires a UTC al guardar y reconstruye hora local al cargar; backend valida `hourUtc`, `minuteUtc` y `timezone`, y persiste `reminder_timezone` (default: `America/Argentina/Buenos_Aires`). |

## Runtime hardening (Phase 40 y posteriores)

Las siguientes reglas se hardened durante Phase 40, Phase 50 (T2/T3/T4) y reflejan el comportamiento runtime actual:

| Rule | Descripcion |
|------|-------------|
| T3-TG-01 | Telegram API retry con exponential backoff: 1s, 2s, 4s antes de fallar. Implementado en `SendReminderCommand.cs`. |
| T3-TG-02 | `SendReminderCommand` y `HandleWebhookUpdateCommand` invocan `SaveChangesAsync` para persistir `AccessAudit` antes de retornar. |
| T3-TG-03 | `DELETE /api/v1/telegram/session` requiere auth valido + rate-limit `"write"` (5 req/IP/min). Marca la TelegramSession activa como `Unlinked`; el índice parcial `UNIQUE(chat_id) WHERE status='Linked'` permite re-vinculación posterior sin DELETE físico. Implementado en Phase 40. |
| T3-TG-04 | `PUT /api/v1/telegram/reminder-schedule` requiere auth valido + rate-limit `"write"` + `TelegramSession` linked. `GET /api/v1/telegram/reminder-schedule` requiere auth valido y permite que la UI recargada muestre el horario guardado. La UI convierte hora local Buenos Aires a UTC; backend valida hora/minuto/timezone con errores tipados y persiste `reminder_timezone` en `reminder_configs` (default: `America/Argentina/Buenos_Aires`). Regresion #21 cubierta localmente y por E2E productivo `qa-dev` el 2026-04-20. |
| T3-11b | `ConsentRequiredMiddleware` usa deny-list explícita para rutas no clinicas: `/api/v1/auth`, `/api/v1/consent`, `/api/v1/telegram`, `/api/v1/export` se excluyen; cualquier otra ruta POST bajo `/api/v1/` se trata como clinica hasta que se acredite lo contrario. |
| T3-SEC-10 | `ExportEndpoints` — `/constraints` autoriza profesional antes de verificar `CareLink`. `ProfessionalDataAccessAuthorizer` fail-closed: 403 si no tiene vinculo activo. |
| T3-RL-01 | Rate limiter fail-closed: politica `auth` 10 req/IP/min; cualquier exceso devuelve 429 + `Retry-After: 60` (segundos fijo). |
| T3-RL-04 | Auth bootstrap usa policy rate limiting `auth` (no `write`). |
| T3-RL-05 | Health/ready endpoint respeta el rate limiter (politica `auth`). |
| T3-SEC-11 | Frontend `proxy.ts` valida `bitacora_session` y enforce rol `professional` para rutas profesionales. |

---

## Modulos internos

| Modulo | Responsabilidad | Entidades principales | Estado |
|--------|----------------|----------------------|--------|
| Auth | Validar JWT Zitadel, resolver identidad, bootstrap/link-on-first-login de paciente | User | Implementado |
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
| Auth | Zitadel self-hosted v4.9.0 | `id.nuestrascuentitas.com` (IdP compartido Teslita). |
| Reverse proxy | Traefik (via Dokploy) | Target de produccion |
| Dominio API | `api.bitacora.nuestrascuentitas.com` | Target backend-only de T01 |
| Dominio web | `bitacora.nuestrascuentitas.com` | Frontend Next.js 16 deployado en produccion (Phase 40 EJECUTADA 2026-04-15). Ruta raiz operativa. |
| Frontend | Next.js 16 | Deployado en `bitacora.nuestrascuentitas.com` (Phase 40 completada). Dashboard paciente, vinculos, logout, Telegram wizard, health check y error boundaries operativos. |

## Orden de rollout (Phase 30 → 31 → 40 ✓ → 41 → 50 → 60)

| Phase | Superficie | Estado | Fecha |
|-------|-----------|--------|-------|
| 30 | Backend basico: Auth, Consent, Registro, Vinculos, Visualizacion, Export | EJECUTADA | 2026-04-10 |
| 31 | Telegram webhook + recordatorios (ReminderWorker activo) | EJECUTADA | 2026-04-12 |
| 40 | Frontend web Next.js 16 (bitacora.nuestrascuentitas.com) | **EJECUTADA** | **2026-04-15 / 2026-04-16** |
| 41 | Profesional dashboard (profesionales.nuestrascuentitas.com) | Pendiente | — |
| 50 | Alertas y notificaciones push | Pendiente | — |
| 60 | UX validation terminal de todas las superficies | Pendiente | — |

**Regla:** ninguna phase se abre si la anterior no tiene smoke passing + evidencia de UX.

## Observabilidad minima

| Aspecto | Implementacion |
|---------|---------------|
| Logs | `Console` + `Debug` providers del host .NET |
| Tracing | OpenTelemetry con `trace_id` end-to-end; OTLP deshabilitado por default hasta configurar endpoint |
| Pseudonimizacion | `pseudonym_id` en logs operacionales; `actor_id` solo en `AccessAudit` |
| Liveness | `GET /health` |
| Readiness | `GET /health/ready` valida connection string, autoridad/audiencia/metadata Zitadel, clave de cifrado, salt y conectividad PostgreSQL |
| Smoke operativo | `infra/smoke/zitadel-cutover-smoke.ps1` cubre la salud productiva y el inicio OIDC; E2E autenticado usa sesión Zitadel real |
| Evidencia Telegram #21 | E2E productivo 2026-04-20 con `qa-dev`: `artifacts/e2e/2026-04-20-bitacora-reminder-ui-qa-dev/`; schema live validado con `reminder_timezone` e índice parcial de `telegram_sessions` |
| trace_id propagation | Requerido en todo request/response; se inyecta en logs y AccessAudit |
| Datos de salud | Prohibido en logs, trazas y telemetry: `encrypted_payload`, `safe_projection` con datos clinicos, identificadores directos del paciente. Solo `pseudonym_id` y `trace_id`. |

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

## Hardening exceptions register

Ver `.docs/raw/decisiones/2026-04-10-wave-prod-hardening-exceptions-register.md` para el registro unico de excepciones aceptadas, atajos prohibidos y gates obligatorios antes de cada phase de codigo.

Este documento es el unico checkpoint que las fases `30`, `31`, `40`, `41`, `50` y `60` deben satisfacer antes de avanzar.

---

*Fuente: `.docs/raw/decisiones/02_decisiones_arquitectura.md`, `infra/runbooks/production-bootstrap.md`, `src/Bitacora.Api/Program.cs`*
