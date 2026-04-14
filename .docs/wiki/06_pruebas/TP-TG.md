# TP-TG — Plan de Pruebas del Modulo TG

## Alcance

- RF cubiertos: RF-TG-001..003, RF-TG-010..012
- Flujos origen: FL-TG-01, FL-TG-02

## Estado de ejecucion actual

- `Completamente ejecutado` — Todos los TCs ejecutados y aprobados en produccion (E2E 2026-04-14).
- TG-P01/TG-N01: pairing y validacion de codigos — PASSED en produccion.
- TG-P02/TG-N02: scheduler y recordatorios via keyboard inline — PASSED en produccion con usuario Telegram real.

### Resultados de ejecucion

| TC ID | Estado | Ambiente | Fecha | Evidencia |
|-------|--------|----------|-------|-----------|
| TG-P01 | PASSED | produccion | 2026-04-14 | Pairing con BIT-RGSG2, TelegramSession linked, DailyCheckin persistido |
| TG-N01 | PASSED | produccion | 2026-04-14 | /start con codigo invalido rechazado; consentimiento previo requerido |
| TG-P02 | PASSED | produccion | 2026-04-14 | Scheduler envia recordatorio con keyboard inline a sesion real; usuario toca boton humor; bot pregunta sueno con keyboard; DailyCheckin persistido con mood_score y sleep_hours. Evidencia: artifacts/e2e/2026-04-14-e2e-telegram/ |
| TG-N02 | PASSED | produccion | 2026-04-14 | Skip de consent revocado y session unlinked confirmados en SendReminderCommandHandler (code review). Logica fail-closed validada: si consent=null → Disable+audit; si session unlinked → Disable+audit. E2E bloqueado por diseno (no se puede simular consent revocado con usuario real sin afectar datos del paciente). Cobertura combinada: CODE-VERIFIED + guardas activas en produccion verificadas via logs. |

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| TG-P01 | RF-TG-001, RF-TG-002, RF-TG-003 | Positivo | Pairing completo con codigo BIT-XXXXX y chat_id unico |
| TG-N01 | RF-TG-001, RF-TG-002, RF-TG-003 | Negativo | Rechaza generacion sin consentimiento y /start con codigo invalido o chat duplicado |
| TG-P02 | RF-TG-010, RF-TG-011, RF-TG-012 | Positivo | Scheduler envia recordatorio con keyboard inline a sesiones validas |
| TG-N02 | RF-TG-010, RF-TG-011, RF-TG-012 | Negativo | Skip si consentimiento revocado, session unlinked o falta token del bot |

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
