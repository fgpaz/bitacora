# HANDOFF-MAPPING-TG-002 — Correspondencia diseño-código

## Proposito

Este documento traduce `TG-002` a ownership tecnico concreto para el runtime actual de Telegram.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `src/Bitacora.Api/Workers/ReminderWorker.cs` | timer cada 1 min, consulta `ReminderConfig`, gate y envio |
| `src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs` | endpoints REST + webhook Telegram |
| `src/Bitacora.Application/Commands/Telegram/SendReminderCommand.cs` | envio de recordatorio con keyboard inline |
| `src/Bitacora.Application/Commands/Telegram/HandleWebhookUpdateCommand.cs` | parseo de respuesta y flujo conversacional |
| `src/Bitacora.Application/Commands/Telegram/ConfigureReminderScheduleCommandHandler.cs` | configuracion de horario con validacion RF-TG-006 |
| `src/Bitacora.Domain/Entities/TelegramSession.cs` | entidad de sesion vinculada y estado conversacional |
| `src/Bitacora.Domain/Entities/ReminderConfig.cs` | entidad de configuracion de recordatorios |
| `src/Bitacora.Domain/Entities/DailyCheckin.cs` | registro diario generado desde Telegram |
| `frontend/components/patient/telegram/TelegramPairingCard.tsx` | UI web de vinculo y horario local |

## Bloques conversacionales y destino sugerido

| Bloque UX/UI | Implementacion sugerida | Nota |
| --- | --- | --- |
| `daily_reminder` trigger | `ReminderWorker.ExecuteAsync()` | timer 1 min, query `ReminderConfig` |
| gate de consentimiento | `SendReminderCommandHandler` | revisa `ConsentGrant` activo |
| gate de sesion | `SendReminderCommandHandler` | revisa `TelegramSession` vinculada |
| envio de pregunta de humor | `SendReminderCommandHandler.SendViaTelegramApiAsync()` | keyboard con escala y `Ahora no` |
| recepcion de respuesta | `HandleWebhookUpdateCommandHandler` | parsea update/callback y dispatcha |
| registro diario | `HandleWebhookUpdateCommandHandler` | crea/actualiza `DailyCheckin` sin ecoar valores clinicos |
| confirmacion | `HandleWebhookUpdateCommandHandler` | copy factual sin celebracion ni emoji |
| factores prompt | `HandleWebhookUpdateCommandHandler` | continuacion opcional |
| cierre final | `HandleWebhookUpdateCommandHandler` | cierre breve sin insistencia |
| horario local | `frontend/lib/api/client.ts` | convierte Buenos Aires local a UTC antes del PUT |

## Contratos de transicion

### Envio de recordatorio (background service — no expuesto al bot)

```
ReminderConfig WHERE next_fire_at <= now()
Gate: ConsentGrant.active == true AND TelegramSession.linked == true
Si gate pasa -> Telegram API SendMessage
```

### Registro conversacional interno

```
POST /api/v1/telegram/webhook
header: X-Telegram-Webhook-Secret: <secret>
body: TelegramWebhookRequest
```

El handler resuelve la sesion vinculada internamente, crea/actualiza `DailyCheckin` y devuelve a Telegram solo mensajes de control. El bot no ecoa valores clinicos, factores, medicacion, identificadores internos ni payloads.

### Rate limit

- Telegram API: max 30 mensajes/segundo global
- Retry con backoff exponencial: max 3 intentos
- Si persiste, registrar fallo y no insistir

## Estados y su correspondencia

| Estado conversacional | Logica | Response copy |
| --- | --- | --- |
| `reminder_sent` | pregunta enviada, esperando respuesta | `Como te sentis ahora?` + keyboard |
| `reply_submitting` | registro en curso | proteccion double-tap |
| `reply_success` | confirmacion + continuacion opcional | `Registrado: +1.` + prompt factores |
| `factors_prompt` | continuacion opcional | pregunta de sueano con keyboard |
| `reply_error` | error recuperable | `No pudimos registrar esa respuesta. Proba de nuevo si queres.` |
| `reminder_skipped` | usuario eligio `Ahora no` | silencio util — no hay mensaje |
| `unrecognized` | mensaje no esperado | `No entendimos ese mensaje. Usa /registrar o esperi tu proximo recordatorio.` |

## Runtime actual

Los paths de primera pasada fueron reemplazados por el runtime materializado listado arriba. Para QA y mantenimiento, usar `ReminderWorker`, `TelegramEndpoints`, `SendReminderCommand`, `HandleWebhookUpdateCommand`, `TelegramSession`, `ReminderConfig` y `TelegramPairingCard` como ownership vigente.

## Momentos auditables

| Momento | Datos logueados |
| --- | --- |
| recordatorio enviado | patient_id, reminder_config_id, timestamp, resultado_telegram |
| respuesta recibida | patient_id, mood_value, timestamp |
| registro creado | mood_entry_id, patient_id, mood_value, source=telegram |
| factores registrados | daily_checkin_id, patient_id, factors, source=telegram |
| omision (`Ahora no`) | patient_id, timestamp, sin detalle del valor |
| error de API | patient_id, endpoint, error_code, timestamp |

## Restricciones cerradas para implementacion

- no usar phrasing prohibido: `No te olvides de registrarte`, `Es importante que respondas ahora`, `Seguimos esperando`, `No cumples con tu registro`, `pendiente`
- no enviar recordatorio si `ConsentGrant` esta revocada
- no enviar recordatorio si `TelegramSession` no esta vinculada
- no registrar respuesta si el usuario eligio `Ahora no`
- Telegram API rate limit: max 30 mensajes/segundo; retry con backoff exponencial (max 3 intentos)
- no confirmar con emoji o tono enfatico
- keyboard inline: maximo 8 opciones por fila
- el estado `reply_submitting` debe proteger contra double-tap

## Regla de ownership

- `TG-002` implementa el flujo de recordatorio y registro conversacional;
- vinculacion de cuenta queda fuera de este mapping (delegada a `TG-001`);
- si el codigo necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

## Deltas 2026-04-22 — impeccable-hardening

> 2026-04-22 — sync impeccable-hardening: referencia cruzada al split presentacional de `TelegramPairingCard` (W5, rama `feature/impeccable-hardening-2026-04-22`). Fuente de verdad: `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md`.

### Referencia cruzada — subcomponentes presentacionales

Los 3 subcomponentes extraídos de `TelegramPairingCard` en W5 son relevantes para este slice en lo que respecta al schedule picker y la sección de recordatorio conversacional:

| Subcomponente | Responsabilidad en TG-002 |
|---|---|
| `PairingReminderSection.tsx` | Configuración del horario de recordatorio (schedule picker) visible desde la web |
| `PairingCodeDisplay.tsx` | Sin uso directo en TG-002 (exclusivo de TG-001 pairing) |
| `PairingInstructions.tsx` | Sin uso directo en TG-002 (exclusivo de TG-001 pairing) |

El mapping completo de los 3 subcomponentes (paths, ownership, estado en padre) está en `HANDOFF-MAPPING-TG-001.md §Deltas 2026-04-22`.

---

**Estado:** mapping listo para `backend/telegram/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-TG-002.md`.
**Runtime Telegram:** implementado; regresion RF-TG-006 validada con evidencia `qa-dev` y seguimiento local de persistencia UI el 2026-04-20; referencia subcomponentes actualizada 2026-04-22.
