# HANDOFF-MAPPING-TG-002 — Correspondencia diseño-código

## Proposito

Este documento traduce `TG-002` a ownership tecnico concreto para `backend/telegram/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `src/Bitacora.Api/BackgroundServices/TelegramReminderService.cs` | timer cada 1 min, consulta ReminderConfig, gate y envio |
| `src/Bitacora.Api/Domain/Entities/TelegramSession.cs` | entidad de sesion vinculada |
| `src/Bitacora.Api/Domain/Entities/ReminderConfig.cs` | entidad de configuracion de recordatorios |
| `src/Bitacora.Api/Services/Telegram/TelegramBotService.cs` | parseo de respuesta inline keyboard |
| `src/Bitacora.Api/Domain/Entities/MoodEntry.cs` | registro de humor con `source: "telegram"` |
| `src/Bitacora.Api/Domain/Entities/DailyCheckin.cs` | registro parcial de factores |
| `src/Bitacora.Api/Endpoints/Telegram/Webhook.cs` | recepcion de callback queries del inline keyboard |
| `src/Bitacora.Api/Program.cs` | registro de IHostedService y webhook |

## Bloques conversacionales y destino sugerido

| Bloque UX/UI | Implementacion sugerida | Nota |
| --- | --- | --- |
| `daily_reminder` trigger | `TelegramReminderService.OnTimerTick()` | timer 1 min, query ReminderConfig |
| gate de consentimiento | `TelegramReminderService.ShouldSend(patientId)` | revisa ConsentGrant activo |
| gate de sesion | `TelegramReminderService.ShouldSend(patientId)` | revisa TelegramSession vinculada |
| envio de pregunta de humor | `TelegramBotService.SendInlineKeyboard(chatId, scaleKeyboard)` | keyboard con `+3 .. -3` + `Ahora no` |
| recepcion de respuesta | `TelegramBotService.HandleCallbackQuery(callback)` | parsea valor y dispatcha |
| registro de humor | `MoodEntryService.Create(moodValue, source: telegram)` | llama `POST /api/v1/mood-entries` internamente |
| confirmacion | `TelegramBotService.SendMessage(chatId, confirmCopy)` | sin celebracion ni emoji |
| factores prompt | `TelegramBotService.SendInlineKeyboard(chatId, sleepKeyboard)` | continuacion opcional |
| cierre final | `TelegramBotService.SendMessage(chatId, "Buen dia.")` | sin insistencia |
| logging de audit | `AccessAuditService.Log(...)` | cada recordatorio y cada respuesta |

## Contratos de transicion

### Envio de recordatorio (background service — no expuesto al bot)

```
ReminderConfig WHERE next_fire_at <= now()
Gate: ConsentGrant.active == true AND TelegramSession.linked == true
Si gate pasa -> Telegram API SendMessage
```

### Registro de humor via API

```
POST /api/v1/mood-entries
body: { "mood_value": number, "source": "telegram", "chat_id": number }
header: Authorization: Bearer <access_token>

Respuestas:
- 201: { "mood_entry_id": string, "mood_value": number, "recorded_at": string }
- 401: mensaje generico (sin sesion)
- 422: valor fuera de rango
```

### Registro de factores via API

```
POST /api/v1/daily-checkins
body: { "sleep_hours": string, "physical_activity": boolean, "medication_taken": string, "chat_id": number }
header: Authorization: Bearer <access_token>

Body es parcial si el usuario no completa todos los factores.
```

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

## Rutas y filenames todavia no existentes en runtime

Estos paths se crean cuando el modulo Telegram se materialice en `src/`:

- `src/Bitacora.Api/BackgroundServices/TelegramReminderService.cs`
- `src/Bitacora.Api/Domain/Entities/TelegramSession.cs`
- `src/Bitacora.Api/Domain/Entities/ReminderConfig.cs`
- `src/Bitacora.Api/Services/Telegram/TelegramBotService.cs`

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

## Runtime ausencia

`TelegramSession`, `ReminderConfig`, el background service y el webhook no existen hoy en el runtime de `src/`. El equipo consume este contrato como especificacion objetivo; la implementacion real espera la materializacion del modulo Telegram.

---

**Estado:** mapping listo para `backend/telegram/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-TG-002.md`.
**Runtime Telegram:** diferido.
