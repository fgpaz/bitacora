# CT-TELEGRAM-RUNTIME: Runtime de Telegram

> Root: `09_contratos_tecnicos.md` — seccion Superficie canonica diferida.
> **Estado: IMPLEMENTADO (Phase 31 gap-closing).** Todos los endpoints REST de Telegram estan materializados. El ReminderWorker (scheduler) esta implementado. `SendTelegramMessageAsync` hace POST real a Telegram Bot API via `HttpClient` usando `TELEGRAM_BOT_TOKEN` (TECH-TELEGRAM.md line 118).

---

## Objetivo del contrato

Definir la superficie publica del bot de Telegram, incluyendo el flujo de vinculacion con codigo de pairing, el procesamiento de mensajes entrantes, y las restricciones de datos que nunca pueden enviarse al bot.

---

## Autenticacion en Telegram

El bot de Telegram **no** usa JWT. La autenticacion es por TelegramSession:

```
chat_id (Telegram Update)
  -> TelegramSession.chat_id
  -> patient_id
  -> User
```

- La autenticacion del webhook se valida por Telegram signature (secret token).
- No hay header `Authorization: Bearer` en webhooks de Telegram.

---

## Endpoints — Implementados

### POST /api/v1/telegram/pairing

Genera un codigo de vinculacion `BIT-XXXXX` para el paciente autenticado (TTL 15 min).

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient) |
| Consent gate | Requiere `ConsentGrant.status=granted` |
| Estado | **Implementado** |

**Response 200:**

```json
{
  "code": "BIT-7K2Q9",
  "expiresInSeconds": 900,
  "expiresAt": "2026-04-11T12:15:00Z"
}
```

**Errores tipados:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| PATIENT_ID_REQUIRED | 400 | patient_id vacio |
| TG_001_CONSENT_REQUIRED | 403 | Consentimiento no vigente |
| PAIRING_CODE_GENERATION_FAILED | 500 | Colision de codigo tras 5 intentos |

**Logica:**
- Invalida cualquier codigo activo previo para el paciente.
- Formato: `BIT-XXXXX` (5 caracteres alfanumericos sin I, O, 0, 1).
- TTL: 15 minutos.

---

### GET /api/v1/telegram/session

Consulta el estado de vinculacion Telegram del paciente autenticado.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient) |
| Estado | **Implementado** |

**Response 200 (vinculado):**

```json
{
  "isLinked": true,
  "sessionId": "uuid",
  "chatId": null,
  "linkedAtUtc": "2026-04-11T12:00:00Z"
}
```

**Response 200 (no vinculado):**

```json
{
  "isLinked": false,
  "sessionId": null,
  "chatId": null,
  "linkedAtUtc": null
}
```

**Nota:** `chatId` es `null` en la respuesta por razones de privacidad. No se expone el `chat_id` de Telegram al paciente via API.

---

### POST /api/v1/telegram/webhook

Procesa updates entrantes del bot Telegram (comandos `/start CODE`, inline keyboard mood values, mensajes de texto).

| Campo | Detalle |
|-------|---------|
| Autenticacion | Telegram signature (secret token) — validado externamente por Traefik |
| Consent gate | Verifica `ConsentGrant.status=granted` antes de procesar |
| Estado | **Implementado** |

**Request body (DTO interno — traducido por el bot adapter desde el formato nativo de Telegram):**

```json
{
  "update": "/start BIT-ABC12",
  "chatId": "123456789",
  "traceId": "uuid-generado-por-adapter",
  "callbackQueryId": "opcional-para-keyboard-taps"
}
```

**Nota arquitectural:** el webhook de Telegram Bot API apunta al **bot adapter** en `https://tg-adapter.bitacora.nuestrascuentitas.com/webhook`. El adapter transforma el JSON nativo de Telegram al DTO interno y reenvía a `POST /api/v1/telegram/webhook` con header `X-Telegram-Webhook-Secret`.

**Logica implementada:**

1. Llamar `answerCallbackQuery` si `CallbackQueryId` presente (descarta spinner de botones).
2. Parsear el payload para detectar `start_with_code`, `mood_input`, o input de factor secuencial.
3. Validar `chat_id` presente (fail-closed).
4. Buscar `TelegramSession` linked por `chat_id` (fail-closed si no existe).
5. Verificar `ConsentGrant.status=granted` para el patient_id de la sesion.
6. Si `start_with_code`: invocar `ConfirmPairingCommand` para consumir el codigo y crear sesion.
7. Si `session.ConversationState != Idle`: invocar flujo secuencial de factores (RF-REG-013):
   - Sueño (keyboard 4h-9h) → Actividad física (keyboard Sí/No) → Social → Ansiedad → Irritabilidad → Medicación → DailyCheckin
   - Estado conversacional persistido en `telegram_sessions.pending_factors_json` y `conversation_state`
8. Si `mood_input` (estado Idle): invocar RF-REG-012 (crear MoodEntry), avanzar estado a AwaitingFactorSleep, enviar keyboard de sueño.
9. Fallback generico: "Usa /start para vincular tu cuenta o escribe tu estado de animo."
10. Siempre retornar HTTP 200 a Telegram para detener re-delivery.

**Response 200:**

```json
{
  "accepted": true,
  "errorCode": null,
  "botMessage": "Cuenta vinculada. Ya podes registrar tu humor desde aca."
}
```

**Respuesta al usuario (mensaje Telegram):**

| Escenario | Mensaje |
|-----------|---------|
| Vinculacion exitosa | "Cuenta vinculada. Ya podes registrar tu humor desde aca." |
| Codigo invalido | "Codigo invalido o expirado." |
| Codigo vencido | "Codigo invalido o expirado." |
| Chat duplicado | "Este Telegram ya esta vinculado a otra cuenta." |
| `/start` sin codigo | "Hola! Usa /start para vincular tu cuenta o escribe tu estado de animo." |
| Mood input | "Registro guardado. Ahora contame un poco mas: ¿Cuantas horas dormiste anoche?" |
| Check-in completo | "Registro completo. Ya podes verlo en tu historial web." |
| Sesion no vinculada | (silent deny — 200 sin mensaje al bot) |
| Consent no vigente | (silent deny — 200 sin mensaje al bot) |

**Nota:** El webhook siempre responde `200` a Telegram para evitar reintentos por errores de negocio ya manejados.

---

## Recordatorios — ReminderWorker

### IHostedService: ReminderWorker

Background service que procesa `ReminderConfig` con `next_fire_at_utc <= now` cada 60 segundos.

| Campo | Detalle |
|-------|---------|
| Intervalo | 60 segundos |
| Estado | **Implementado** |

**Logica del worker:**

1. Query: `ReminderConfig WHERE enabled=true AND next_fire_at_utc <= now_utc`
2. Por cada recordatorio: invoca `SendReminderCommand` (que internamente llama a `SendTelegramMessageAsync`)
3. Errores de un recordatorio no bloquean el procesamiento de los demas

**SendReminderCommand (RF-TG-010..012):**

| Paso | Comportamiento |
|------|----------------|
| Consent revocado | `Disable()` + audit denied + silencia |
| Sesion unlinked | `Disable()` + audit denied + silencia |
| Config disabled | Silencia sin cambios |
| Envio exitoso | `AdvanceNextFire()` + audit ok |
| Envio fallido | Retorna error, no avanza next_fire |

**ESTADO:** `SendTelegramMessageAsync` implementada en `SendReminderCommand.cs:118`. Hace POST real a `https://api.telegram.org/bot{token}/sendMessage` via `HttpClient` con timeout de 10s. Si `TELEGRAM_BOT_TOKEN` no esta configurado, loguea warning y retorna `false` (comportamiento fail-closed para el worker).

**Retry policy:** `SendTelegramMessageAsync` implements exponential backoff on failure: 1s, 2s, 4s (3 retries total) before returning false. Errores de un recordatorio no bloquean el procesamiento de los demas.

**Audit persistence:** `SendReminderCommand` y `HandleWebhookUpdateCommand` invocan `SaveChangesAsync` para persistir `AccessAudit` inmediatamente antes de retornar, garantizando que el audit existe aunque el commandComplete.

---

## Invariantes de Autorizacion

1. **Owner-only:** un `chat_id` solo puede estar vinculado a un paciente a la vez.
2. **Un solo codigo activo por paciente:** al generar uno nuevo se invalida el anterior.
3. **Consentimiento como gate:** toda operacion del webhook requiere consentimiento vigente del paciente.
4. **Autenticacion por signature:** webhooks validados via Telegram secret token, no via JWT.
5. **Fail-closed sobre chat_id ausente o sesion no encontrada:** retorna 200 sin mensaje al bot (silencioso).
6. **chat_id nunca expuesto via API REST paciente:** `GET /telegram/session` retorna `chat_id: null`.

---

## Invariantes de Compliance — NO FUGA A TELEGRAM

> **CRITICO:** Esta es la invariante de compliance mas importante del modulo Telegram.

**Regla absoluta:** ninguna respuesta HTTP dirigida al bot Telegram, ni ningun mensaje enviado por el bot, puede contener:

- `encrypted_payload` o cualquier campo derivable del cifrado
- `safe_projection` con datos clinicos del paciente
- `mood_score`, `sleep_hours`, informacion de medicacion, o cualquier campo derivable de `MoodEntry` o `DailyCheckin`
- `patient_id` real o cualquier identificador que exponga la identidad del paciente fuera de la sesion vinculada

**Implicaciones:**

- Los recordatorios y confirmaciones del bot usan solo texto generico y `patient_id` interno de sesion.
- No existe endpoint Telegram que exponga datos clinicos; toda lectura vuelve por la API REST del paciente.
- El bot nunca hace queries directas a `mood_entries` ni `daily_checkins` para enviar a Telegram.

---

## Superficies deferidas

| Endpoint / Webhook | Estado | Notas |
|--------------------|--------|-------|
| POST /api/v1/telegram/pairing | **Implementado** | |
| GET /api/v1/telegram/session | **Implementado** | |
| POST /api/v1/telegram/webhook | **Implementado** | |
| ReminderWorker | **Implementado** | `SendTelegramMessageAsync` con HTTP POST real a Telegram Bot API |
| Integracion real Telegram Bot API | **Implementado** | `TELEGRAM_BOT_TOKEN` usado en `SendReminderCommand.cs:118` via `SendViaTelegramApiAsync` |
| Registro humor via Telegram | **Implementado** | Flujo completo: mood keyboard (-3..+3) → sleep keyboard (4h-9h) → factores Sí/No → DailyCheckin. E2E verificado en producción 2026-04-14. |

---

## Sincronizacion

Cambios en este contrato fuerzan revision de:
- `09_contratos_tecnicos.md` (seccion Superficie canonica diferida)
- `04_RF/RF-TG-*`
- `07_baseline_tecnica.md` si cambia la invariante de no fuga
