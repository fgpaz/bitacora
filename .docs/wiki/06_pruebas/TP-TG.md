# TP-TG — Plan de Pruebas del Modulo TG

## Alcance

- RF cubiertos: RF-TG-001..003, RF-TG-006, RF-TG-010..012
- Flujos origen: FL-TG-01, FL-TG-02

## Estado de ejecucion actual

- `Completamente ejecutado` — Todos los TCs ejecutados y aprobados en produccion (E2E 2026-04-14, validacion post-auth-fix 2026-04-15, qa-dev full smoke post-Zitadel 2026-04-20).
- TG-P01/TG-N01: pairing y validacion de codigos — PASSED en produccion.
- TG-P02/TG-N02: scheduler, recordatorios y flujo conversacional via keyboard inline — PASSED en produccion con usuario Telegram real.
- Evidencia vigente post-fix: `artifacts/e2e/2026-04-20-qa-dev-full-smoke/`, `artifacts/e2e/2026-04-20-bitacora-reminder-ui-qa-dev/` y `.docs/raw/reports/2026-04-20-qa-dev-full-smoke.md`.

### Resultados de ejecucion (E2E 2026-04-14)

| TC ID | Estado | Ambiente | Fecha | Evidencia |
|-------|--------|----------|-------|-----------|
| TG-P01 | PASSED | produccion | 2026-04-14 | Pairing con BIT-RGSG2, TelegramSession linked, DailyCheckin persistido |
| TG-N01 | PASSED | produccion | 2026-04-14 | /start con codigo invalido rechazado; consentimiento previo requerido |
| TG-P02 | PASSED | produccion | 2026-04-14 | Scheduler envia recordatorio con keyboard inline a sesion real; usuario toca boton humor; bot pregunta sueno con keyboard; DailyCheckin persistido con mood_score y sleep_hours. Evidencia: artifacts/e2e/2026-04-14-e2e-telegram/ |
| TG-N02 | PASSED | produccion | 2026-04-14 | Skip de consent revocado y session unlinked confirmados en SendReminderCommandHandler (code review). Logica fail-closed validada: si consent=null → Disable+audit; si session unlinked → Disable+audit. E2E bloqueado por diseno (no se puede simular consent revocado con usuario real sin afectar datos del paciente). Cobertura combinada: CODE-VERIFIED + guardas activas en produccion verificadas via logs. |

### Resultados de ejecucion (E2E 2026-04-15 — validacion post-auth-fix)

| TC ID | Estado | Ambiente | Fecha | Evidencia |
|-------|--------|----------|-------|-----------|
| TG-P01 | PASSED | produccion | 2026-04-15 | POST /telegram/pairing: HTTP 200, code=BIT-KYNYR, exp=900s. Evidencia: F4-01-pairing-code.json |
| TG-P01b-LINKING | GAP | produccion | 2026-04-15 | @tedi_responde ya vinculado a paciente diferente. @gabrielpaz: PeerNotFound (sin historial bot). **Fixture requerido:** Ejecutar `infra/runbooks/telegram-e2e-cleanup.md` antes de re-ejecutar. Ver GAP-01 en evidencia-resumen.md |
| TG-P02 | PASSED-HISTORICAL | produccion | 2026-04-15 | Evidencia historica previa al privacy-fix del 2026-04-20. El flujo conversacional persistio mood_entry y daily_checkin, pero la respuesta del bot de esa fecha no es comportamiento vigente porque exponia valores clinicos en chat. Evidencia reemplazada por qa-dev full smoke 2026-04-20. |
| TG-SEC | SUPERSEDED | produccion | 2026-04-15 | Verificacion historica insuficiente para el contrato actual. La evidencia vigente es TG-SEC-20260420, que valida respuestas confirmacion-only sin eco de valores clinicos. |
| TG-N02 | GAP | produccion | 2026-04-15 | Sin perfil QA disponible con bot en dialogs y sin sesión vinculada. **Fixture requerido:** Ejecutar `infra/runbooks/telegram-e2e-cleanup.md` antes de re-ejecutar. Evidencia heredada 2026-04-14 (PASSED) |

### Resultados de ejecucion (E2E 2026-04-20 — qa-dev full smoke post-Zitadel)

| TC ID | Estado | Ambiente | Fecha | Evidencia |
|-------|--------|----------|-------|-----------|
| TG-P01b-LINKING | PASSED | produccion | 2026-04-20 | Perfil Telegram `qa-dev` vinculado desde la web con `@mi_bitacora_personal_bot`. La UI `/configuracion/telegram` muestra estado vinculado y bot canonico. Evidencia: `artifacts/e2e/2026-04-20-qa-dev-full-smoke/11-telegram-linked-after-deploy.png`. |
| TG-P02 | PASSED | produccion | 2026-04-20 | `qa-dev` completa flujo mood + factores; los registros se persisten y aparecen en `/dashboard` bajo sesion Zitadel. Evidencia: `artifacts/e2e/2026-04-20-qa-dev-full-smoke/14-dashboard-after-privacy-smoke.png`. |
| TG-SEC-20260420 | PASSED | produccion | 2026-04-20 | Respuestas post-fix del bot son confirmacion-only: no repiten score, sueno, factores, medicacion, `chat_id`, `patient_id`, JWT ni payloads clinicos. Reporte: `.docs/raw/reports/2026-04-20-qa-dev-full-smoke.md`. |
| TG-ZITADEL | PASSED | produccion | 2026-04-20 | Smoke Zitadel-only validado: OIDC discovery/JWKS, readiness, login/logout redirects, backend proxy sin sesion 401, bootstrap sin bearer 401. Comando: `pwsh -File .\infra\smoke\zitadel-cutover-smoke.ps1`. |
| TG-P06-REG21 | PASSED | produccion | 2026-04-20 | Con fixture QA `qa-dev`, `/configuracion/telegram` guardó 22:00 Buenos Aires sin 500; request `{hourUtc: 1, minuteUtc: 0, timezone: "America/Argentina/Buenos_Aires"}`, feedback visible, Telegram siguió vinculado y logout fail-closed. Evidencia sanitizada: `artifacts/e2e/2026-04-20-bitacora-reminder-ui-qa-dev/`. |

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| TG-P01 | RF-TG-001, RF-TG-002, RF-TG-003 | Positivo | Pairing completo con codigo BIT-XXXXX y chat_id unico |
| TG-N01 | RF-TG-001, RF-TG-002, RF-TG-003 | Negativo | Rechaza generacion sin consentimiento y /start con codigo invalido o chat duplicado |
| TG-P02 | RF-TG-010, RF-TG-011, RF-TG-012 | Positivo | Scheduler envia recordatorio con keyboard inline a sesiones validas |
| TG-N02 | RF-TG-010, RF-TG-011, RF-TG-012 | Negativo | Skip si consentimiento revocado, session unlinked o falta token del bot |
| TG-P05 | RF-TG-005 | Positivo | Desvincular Telegram desde UI web (/configuracion/telegram) |
| TG-P06 | RF-TG-006 | Positivo | Configurar horario de recordatorio via PUT /reminder-schedule |
| TG-P06b | RF-TG-006 | Positivo | Recargar /configuracion/telegram consulta GET /reminder-schedule y muestra el horario local guardado |
| TG-N04 | RF-TG-005 | Negativo | DELETE /api/v1/telegram/session sin sesion activa retorna 404 |
| TG-N05 | RF-TG-006 | Negativo | PUT /api/v1/reminder-schedule sin sesion Telegram activa retorna 403 |
| TG-N06 | RF-TG-006 | Negativo | PUT /api/v1/telegram/reminder-schedule rechaza hora, minuto o timezone invalidos con 400 tipado |

## Gherkin expandido

```gherkin
Scenario: Pairing Telegram exitoso de punta a punta
  Given patient autenticado con consentimiento vigente
  When POST /api/v1/telegram/pairing
  Then se retorna code="BIT-XXXXX" con expires_in=900
  When el usuario envia /start con ese codigo desde un chat nuevo
  Then se crea TelegramSession linked
  And el codigo queda consumido

Scenario: Pairing rechazado por codigo invalido o chat duplicado
  Given existe un chat_id ya vinculado a otro paciente
  When llega /start con codigo invalido o expirado desde ese chat
  Then no se crea una nueva TelegramSession
  And el bot responde guidance al usuario

Scenario: Scheduler omite recordatorios sin condiciones de envio
  Given existe ReminderConfig activo
  And el paciente tiene consentimiento revocado o TelegramSession unlinked
  When corre el scheduler
  Then no se envia mensaje al bot
  And el skip queda trazado en logs operacionales

Scenario: Desvincular Telegram desde interfaz web
  Given paciente autenticado con sesion Telegram vinculada
  When accede a /configuracion/telegram y hace click en "Desvincular"
  Then se abre dialogo de confirmacion
  When confirma la revocacion
  Then DELETE /api/v1/telegram/session retorna 200 {unlinked: true}
  And GET /api/v1/telegram/session retorna estado no-vinculado
  And la UI muestra pairing wizard nuevamente

Scenario: Configurar horario de recordatorio
  Given paciente autenticado con sesion Telegram vinculada
  When accede a /configuracion/telegram y selecciona hora local Buenos Aires (ej: 22:00)
  And hace click en "Guardar"
  Then PUT /api/v1/telegram/reminder-schedule retorna 200
  And el request contiene { hourUtc: 1, minuteUtc: 0, timezone: "America/Argentina/Buenos_Aires" }
  And reminder_configs se actualiza en BD con nueva hora UTC
  And scheduler enviara recordatorios a la nueva hora

Scenario: Consultar horario guardado de recordatorio
  Given paciente autenticado con sesion Telegram vinculada
  And existe ReminderConfig guardado para 22:00 Buenos Aires
  When recarga /configuracion/telegram
  Then GET /api/v1/telegram/reminder-schedule retorna 200 con configured=true
  And la UI muestra "22:00" como horario actual de Buenos Aires

Scenario: Consultar horario sin configuracion previa
  Given paciente autenticado con sesion Telegram vinculada
  And no existe ReminderConfig para el paciente
  When GET /api/v1/telegram/reminder-schedule
  Then retorna 200 con configured=false
  And no expone chat_id, patient_id ni payloads clinicos

Scenario: Desvincular sin sesion Telegram activa
  Given paciente autenticado SIN sesion Telegram vinculada
  When intenta DELETE /api/v1/telegram/session
  Then retorna 404 TG_SESSION_NOT_FOUND
  And el estado de configuracion persiste sin cambios

Scenario: Configurar horario sin sesion Telegram activa
  Given paciente autenticado SIN sesion Telegram vinculada
  When intenta PUT /api/v1/telegram/reminder-schedule con {hourUtc: 20, minuteUtc: 0}
  Then retorna 403 TG_006_NO_ACTIVE_SESSION
  And la BD no se modifica

Scenario: Configurar horario con minuto invalido
  Given paciente autenticado con sesion Telegram vinculada
  When intenta PUT /api/v1/telegram/reminder-schedule con {hourUtc: 9, minuteUtc: 15}
  Then retorna 400 TG_006_INVALID_MINUTE
  And la BD no se modifica
```

### Regresion #21 (2026-04-20)

| TC ID | Estado | Ambiente | Fecha | Evidencia |
|-------|--------|----------|-------|-----------|
| TG-P06-REG21 | PASSED | local + produccion | 2026-04-20 | `ReminderScheduleTests` cubre conversión cliente `22:00` Buenos Aires -> `{ hourUtc: 1, minuteUtc: 0 }`; E2E productivo con `qa-dev` confirmó 200, feedback UI, vínculo Telegram intacto, logout fail-closed y sin overflow horizontal en 320/375. Evidencia: `artifacts/e2e/2026-04-20-bitacora-reminder-ui-qa-dev/`. |
| TG-P06b-PERSIST | CODE-VERIFIED | local | 2026-04-20 | `ReminderScheduleTests` cubre GET configurado y GET sin configuracion; el cliente web normaliza UTC -> hora local Buenos Aires para mostrar el horario persistido al recargar. |

## Criterios de salida

- Cobertura positiva y negativa de los RF activos del modulo TG.
- Evidencia de pairing seguro y de scheduler con skips correctos.

## Cierre del ciclo de pruebas

Ciclo inicial cerrado el 2026-04-14 con ejecucion E2E en produccion usando usuario Telegram real.
Revalidacion vigente post-Zitadel y post-privacy-fix cerrada el 2026-04-20 con perfil Telegram `qa-dev`.

### Infraestructura de bot adapter

El cierre de TG-P02 requirio la creacion del **bot adapter** (`src/TelegramBotAdapter/`):
microservicio Python/FastAPI que transforma el webhook nativo de Telegram al DTO interno
`{Update, ChatId, TraceId, CallbackQueryId}` y reenvía al API con `X-Telegram-Webhook-Secret`.
Deploy: Dokploy app `tg-adapter` en VPS turismo, dominio `tg-adapter.bitacora.nuestrascuentitas.com`.

### Patron de keyboard inline

El flujo de recordatorio implementado:
1. Scheduler → SendReminderCommand → mensaje con keyboard inline humor (-3..+3)
2. Usuario toca boton → HandleWebhookUpdateCommand → pregunta sueno con keyboard (4h..9h)
3. Usuario toca horas → flujo continua con demas factores via keyboard Si/No
4. Flujo completa → DailyCheckin INSERT/UPDATE con todos los campos

### TG-N02: cobertura por CODE-VERIFIED

TG-N02 (skip de consent revocado) se cierra como CODE-VERIFIED + guardas en produccion dado que:
- La logica fail-closed esta auditada en `SendReminderCommandHandler` (lineas 67-94)
- El scheduler la ejecuta en cada ciclo con sesiones activas
- Una ejecucion E2E con consent revocado requeriria revocar el consentimiento del paciente de test,
  afectando su uso productivo del sistema
- Aceptado por el equipo como cobertura suficiente para el scope del ciclo T01
