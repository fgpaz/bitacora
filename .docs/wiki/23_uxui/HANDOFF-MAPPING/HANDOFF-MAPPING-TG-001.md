# HANDOFF-MAPPING-TG-001 — Correspondencia diseño-código

## Proposito

Este documento traduce `TG-001` a ownership tecnico concreto para `backend/telegram/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `src/Bitacora.Api/Endpoints/Telegram/Webhook.cs` | recepcion de mensajes entrantes del bot |
| `src/Bitacora.Api/Endpoints/Telegram/Pairing.cs` | generacion de codigo de vinculacion |
| `src/Bitacora.Api/Domain/Entities/TelegramSession.cs` | entidad de sesion vinculada |
| `src/Bitacora.Api/Domain/Entities/PairingCode.cs` | entidad de codigo |
| `src/Bitacora.Api/Services/Telegram/TelegramBotService.cs` | logica de parseo, validacion y respuesta |
| `src/Bitacora.Api/BackgroundServices/TelegramReminderService.cs` | background service de recordatorios (delegado a TG-002) |
| `src/Bitacora.Api/Program.cs` | registro de webhook y servicios |

## Bloques conversacionales y destino sugerido

| Bloque UX/UI | Implementacion sugerida | Nota |
| --- | --- | --- |
| evaluacion de `/start <codigo>` | `TelegramBotService.EvaluateStartCommand(string code)` | primer punto de entrada |
| validacion de codigo | `PairingCodeService.Validate(code)` | retorna resultado con estado |
| creacion de sesion | `TelegramSessionService.Link(patientId, chatId)` | solo si codigo valido y no vinculado |
| respuesta al usuario | `TelegramBotService.SendMessage(chatId, copy)` | un solo mensaje por transicion |
| logging de audit | `AccessAuditService.Log(telegram_session_linked, ...)` | momento auditable obligatorio |

## Contratos de transicion

### Webhook recibido por el bot

```
POST /api/v1/telegram/webhook
body: { "chat_id": number, "message": "/start BIT-7K2Q9" }
```

### Confirmacion de vinculacion (endpoint consumido por el bot)

```
POST /api/v1/telegram/pairing/confirm
body: { "code": string, "chat_id": number }
header: X-Telegram-Bot-Token: <token>
```

Respuestas esperadas:

- `200`: `{ "status": "linked", "patient_id": string }`
- `410`: `{ "error": "code_expired" }`
- `404`: `{ "error": "code_invalid" }`
- `409`: `{ "error": "chat_already_linked" }`

### Lectura de sesion (para saber si ya esta vinculado)

```
GET /api/v1/telegram/session?chat_id=<chat_id>
```

Respuestas esperadas:

- `200`: `{ "linked": true, "patient_id": string }`
- `200`: `{ "linked": false }`

## Estados y su correspondencia

| Estado conversacional | Logica | Response copy |
| --- | --- | --- |
| `idle` | recibe cualquier mensaje | — |
| `code_evaluating` | valida codigo contra PairingCode | — |
| `linked` | crea TelegramSession y confirma | `Cuenta vinculada. Ya podes registrar tu humor desde aca.` |
| `expired` | orienta a web | `Ese codigo ya vencio. Genera uno nuevo desde la web.` |
| `invalid` | orienta al codigo correcto | `No reconocimos ese codigo. Mira el que aparece en la web e intentalo de nuevo.` |
| `already_linked` | salida clara | `Esta cuenta de Telegram ya esta vinculada a un registro.` |
| `no_code` | orienta a flujo web | `Envia el codigo que aparece en la seccion de Telegram de la web.` |
| `unrecognized` | recordatorio de formato | `No entendimos ese mensaje. Usa el comando /start junto con el codigo.` |

## Rutas y filenames todavia no existentes en runtime

Estos paths se crean cuando el modulo Telegram se materialice en `src/`:

- `src/Bitacora.Api/Endpoints/Telegram/Webhook.cs`
- `src/Bitacora.Api/Endpoints/Telegram/Pairing.cs`
- `src/Bitacora.Api/Domain/Entities/TelegramSession.cs`
- `src/Bitacora.Api/Domain/Entities/PairingCode.cs`
- `src/Bitacora.Api/Services/Telegram/TelegramBotService.cs`
- `src/Bitacora.Api/BackgroundServices/TelegramReminderService.cs` (TG-002)

## Momentos auditables

| Momento | Datos logueados |
| --- | --- |
| generacion del codigo | patient_id, code, expires_at, created_at |
| validacion del codigo | code, result, timestamp |
| creacion de TelegramSession | patient_id, chat_id, linked_at, result |
| respuesta de vinculacion | copy enviado, timestamp |

## Restricciones cerradas para implementacion

- no usar jerga tecnica en copy del bot (`pairing`, `binding`, `sync`, `session`, `workflow`)
- no almacenar `chat_id` en logs de aplicacion
- no permitir mas de 1 validacion por codigo (rate limit interna)
- no confirmar vinculo si `ConsentGrant` esta revocada
- el codigo de vinculacion es de un solo uso
- TTL del codigo: 15 minutos
- no ofrecer reintento de validacion con el mismo codigo si ya fue usado

## Regla de ownership

- `TG-001` implementa solo el flujo de vinculacion;
- recordatorios y registro conversacional quedan fuera de este mapping (delegados a `TG-002`);
- si el codigo necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

## Runtime ausencia

`TelegramSession` y `PairingCode` no existen hoy en el runtime de `src/`. El equipo consume este contrato como especificacion objetivo; la implementacion real espera la materializacion del modulo Telegram.

---

**Estado:** mapping listo para `backend/telegram/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-TG-001.md`.
**Runtime Telegram:** diferido.
