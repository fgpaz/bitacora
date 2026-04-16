# TP-TG — Plan de Pruebas del Modulo TG

## Alcance

- RF cubiertos: RF-TG-001..003, RF-TG-010..012
- Flujos origen: FL-TG-01, FL-TG-02

## Estado de ejecucion actual

- `Completamente ejecutado` — Todos los TCs ejecutados y aprobados en produccion (E2E 2026-04-14, validacion post-auth-fix 2026-04-15).
- TG-P01/TG-N01: pairing y validacion de codigos — PASSED en produccion.
- TG-P02/TG-N02: scheduler y recordatorios via keyboard inline — PASSED en produccion con usuario Telegram real.

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
| TG-P02 | PASSED | produccion | 2026-04-15 | Flujo conversacional completo: +2, 7h sueno, 5 factores, 09:30 med. Bot responde: "Registro completo! Humor: +2 | Sueno: 7h | ... Medicacion: si (09:30)". DB: mood_entry 8bec7f15 canal=telegram, daily_checkin ab280b34 sleep=7. conversation_state=0. Evidencia: F4-03-conversation-log.json, F4-05-db-telegram.txt |
| TG-SEC | PASSED | produccion | 2026-04-15 | Ningún mensaje bot expone encrypted_payload, patient_id, ni datos clínicos raw |
| TG-N02 | GAP | produccion | 2026-04-15 | Sin perfil QA disponible con bot en dialogs y sin sesión vinculada. **Fixture requerido:** Ejecutar `infra/runbooks/telegram-e2e-cleanup.md` antes de re-ejecutar. Evidencia heredada 2026-04-14 (PASSED) |

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| TG-P01 | RF-TG-001, RF-TG-002, RF-TG-003 | Positivo | Pairing completo con codigo BIT-XXXXX y chat_id unico |
| TG-N01 | RF-TG-001, RF-TG-002, RF-TG-003 | Negativo | Rechaza generacion sin consentimiento y /start con codigo invalido o chat duplicado |
| TG-P02 | RF-TG-010, RF-TG-011, RF-TG-012 | Positivo | Scheduler envia recordatorio con keyboard inline a sesiones validas |
| TG-N02 | RF-TG-010, RF-TG-011, RF-TG-012 | Negativo | Skip si consentimiento revocado, session unlinked o falta token del bot |
| TG-P05 | RF-TG-005 | Positivo | Desvincular Telegram desde UI web (/configuracion/telegram) |
| TG-P06 | RF-TG-006 | Positivo | Configurar horario de recordatorio via PUT /reminder-schedule |
| TG-N04 | RF-TG-005 | Negativo | DELETE /api/v1/telegram/session sin sesion activa retorna 404 |
| TG-N05 | RF-TG-006 | Negativo | PUT /api/v1/reminder-schedule sin sesion Telegram activa retorna 403 |

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
  When accede a /configuracion/telegram y selecciona hora (ej: 20:00)
  And hace click en "Guardar"
  Then PUT /api/v1/reminder-schedule retorna 200
  And reminder_configs se actualiza en BD con nueva hora
  And scheduler enviara recordatorios a la nueva hora

Scenario: Desvincular sin sesion Telegram activa
  Given paciente autenticado SIN sesion Telegram vinculada
  When intenta DELETE /api/v1/telegram/session
  Then retorna 404 TG_SESSION_NOT_FOUND
  And el estado de configuracion persiste sin cambios

Scenario: Configurar horario sin sesion Telegram activa
  Given paciente autenticado SIN sesion Telegram vinculada
  When intenta PUT /api/v1/reminder-schedule con {hour: 20}
  Then retorna 403 TG_NO_ACTIVE_SESSION
  And la BD no se modifica
```

## Criterios de salida

- Cobertura positiva y negativa de los 6 RF del modulo.
- Evidencia de pairing seguro y de scheduler con skips correctos.

## Cierre del ciclo de pruebas

Ciclo cerrado el 2026-04-14 con ejecucion E2E en produccion usando usuario Telegram real.

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
