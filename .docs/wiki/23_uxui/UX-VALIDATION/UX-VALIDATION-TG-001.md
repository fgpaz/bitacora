# UX-VALIDATION-TG-001 — Validación UX del slice TG-001

## Slice y actor

- **Slice:** `TG-001` — vinculación de cuenta Telegram
- **Actor:** Paciente
- **Fecha de validación intentada:** 2026-04-10
- **Resultado:** validación **bloqueada**

## Evidencia revisada

- `artifacts/e2e/2026-04-10-wave-prod-telegram-validation/summary.md`
- `artifacts/e2e/2026-04-10-wave-prod-telegram-validation/defects.md`
- `artifacts/e2e/2026-04-10-wave-prod-telegram-validation/transcript.md`

## Hallazgos

### TG-VAL-001 — Missing Telegram bot token in validation environment

- **Severidad:** Critical
- **Evidencia:** `telegram_bot_token_missing` — `TELEGRAM_BOT_TOKEN` no está presente en el shell ni en `infra/.env`.
- **Impacto:** no puede ejecutarse mensaje real del bot ni roundtrip de webhook.
- **Owner:** runtime/secret provisioning
- **Estado:** Open

### TG-VAL-002 — No Telegram test account/session harness available

- **Severidad:** Critical
- **Evidencia:** no transcript ni sesión de chat reproducible fue capturada en esta ejecución.
- **Impacto:** los flujos de vinculación (`POST /api/v1/telegram/pairing`) y recordatorio (`ReminderWorker`) no pueden validarse con canal real.
- **Owner:** QA / Telegram E2E setup
- **Estado:** Open

### TG-VAL-003 — Reminder delivery path depends on live bot connectivity

- **Severidad:** High
- **Evidencia:** la implementación de recordatorio llama a Telegram Bot API sin credencial de fallback en esta sesión.
- **Impacto:** el runtime de recordatorio permanece sin verificar end-to-end.
- **Owner:** backend runtime / QA
- **Estado:** Open

## Veredicto de validación

La validación UX del slice `TG-001` queda en estado **`blocked`** en esta sesión.

- El build del backend pasó después de la integración Telegram.
- Los contratos de runtime de Telegram fueron sincronizados a la superficie implementada.
- No fue posible ejecutar conversación real del bot por los bloqueos documentados en `TG-VAL-001` y `TG-VAL-002`.
- La superficie backend de Telegram está materializada (`POST /telegram/pairing`, `GET /telegram/session`, `POST /telegram/webhook`), lo cual se valida en smoke gate ( RF-TG-001, RF-TG-002).
- Este documento es evidencia de un intento de validación bloqueada y no debe interpretarse como cierre exitoso.

## Defectos abiertos

| Defecto | Severidad | slice | Estado |
| --- | --- | --- | --- |
| TG-VAL-001 | Critical | TG-001, TG-002 | Open |
| TG-VAL-002 | Critical | TG-001, TG-002 | Open |
| TG-VAL-003 | High | TG-002 | Open |

## Retorno a la cadena canónica

El slice `TG-001` tiene `UI-RFC + HANDOFF-*` completos bajo waiver de entrada a UI. Esa apertura no modifica el estado de validación: sigue `blocked` hasta que exista evidencia real de cohorte.

## Siguiente paso

Provisionar `TELEGRAM_BOT_TOKEN` y cuenta de test Telegram. Ejecutar cohorte híbrida `G`. Actualizar este documento con evidencia real.

---

**Estado:** validación bloqueada — sesión 2026-04-10.
**Próximo gate:** ejecución de cohorte real con token configurado y cuenta de test.
