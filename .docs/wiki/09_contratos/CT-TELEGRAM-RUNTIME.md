# CT-TELEGRAM-RUNTIME: Runtime de Telegram

> Root: `09_contratos_tecnicos.md` — seccion Superficie canonica diferida.
> **Estado: IMPLEMENTADO (Phase 31 gap-closing).** Todos los endpoints REST de Telegram estan materializados. El ReminderWorker (scheduler) esta implementado, pero `SendTelegramMessageAsync` es un stub (no llama a la API de Telegram real).

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
  "expires_in_seconds": 900,
  "expires_at": "2026-04-11T12:15:00Z"
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
  "is_linked": true,
  "session_id": "uuid",
  "chat_id": null,
  "linked_at_utc": "2026-04-11T12:00:00Z"
}
```

**Response 200 (no vinculado):**

```json
{
  "is_linked": false,
  "session_id": null,
  "chat_id": null,
  "linked_at_utc": null
}
```

**Nota:** `chat_id` es `null` en la respuesta por razones de privacidad. No se expone el `chat_id` de Telegram al paciente via API.

---

### POST /api/v1/telegram/webhook

Procesa updates entrantes del bot Telegram (comandos `/start CODE`, inline keyboard mood values, mensajes de texto).

| Campo | Detalle |
|-------|---------|
| Autenticacion | Telegram signature (secret token) — validado externamente por Traefik |
| Consent gate | Verifica `ConsentGrant.status=granted` antes de procesar |
| Estado | **Implementado** |

**Request body:**

```json
{
  "update": "/start BIT-ABC12",
  "chat_id": "123456789",
  "trace_id": "uuid"
}
```

**Logica implementada:**

1. Parsear el payload para detectar `start_with_code`, `mood_input`, o `text_input`.
2. Validar `chat_id` presente (fail-closed).
3. Buscar `TelegramSession` linked por `chat_id` (fail-closed si no existe).
4. Verificar `ConsentGrant.status=granted` para el patient_id de la sesion.
5. Si `start_with_code`: invocar `ConfirmPairingCommand` para consumir el codigo y crear sesion.
6. Si `mood_input`: procesar inline keyboard (-3..+3), confirmando recepcion (registro diferido a RF-REG-012).
7. Fallback generico: "Hola! Usa /start para vincular tu cuenta o escribe tu estado de animo."
8. Siempre retornar HTTP 200 a Telegram para detener re-delivery.

**Response 200:**

```json
{
  "accepted": true,
  "error_code": null,
  "bot_message": "Cuenta vinculada. Ya podes registrar tu humor desde aca."
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
| Mood input | "Recibido. Lo registramos en tu bitacora." |
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
| Estado | **Implementado (stub de Telegram API)** |

**Logica del worker:**

1. Query: `ReminderConfig WHERE enabled=true AND next_fire_at_utc <= now_utc`
2. Por cada recordatorio: invoca `SendReminderCommand`
3. Errores de un recordatorio no bloquean el procesamiento de los demas

**SendReminderCommand (RF-TG-010..012):**

| Paso | Comportamiento |
|------|----------------|
| Consent revocado | `Disable()` + audit denied + silencia |
| Sesion unlinked | `Disable()` + audit denied + silencia |
| Config disabled | Silencia sin cambios |
| Envio exitoso | `AdvanceNextFire()` + audit ok |
| Envio fallido | Retorna error, no avanza next_fire |

**PENDIENTE:** `SendTelegramMessageAsync` es un stub que solo loguea. La integracion real con la API de Telegram (`TELEGRAM_BOT_TOKEN`) no esta wireada.

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
| ReminderWorker | **Implementado** | Pero `SendTelegramMessageAsync` es stub |
| Integracion real Telegram Bot API | **PENDIENTE** | `TELEGRAM_BOT_TOKEN` no conectado a HTTP client |
| Registro humor via Telegram | **PENDIENTE** | `ProcessMoodInputAsync` confirmando recepcion nomas |

---

## Sincronizacion

Cambios en este contrato fuerzan revision de:
- `09_contratos_tecnicos.md` (seccion Superficie canonica diferida)
- `04_RF/RF-TG-*`
- `07_baseline_tecnica.md` si cambia la invariante de no fuga
