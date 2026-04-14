# RF-TG-002: Vincular chat_id via /start + code

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-TG-002 |
| Modulo | TG |
| Trigger | Webhook de Telegram recibe mensaje `/start BIT-XXXXX` |
| Actor | Sistema (webhook handler) |
| Prioridad | Security |
| Estado | **Implementado** via `POST /api/v1/telegram/webhook` + `ConfirmPairingCommand` |

## Precondiciones detalladas
- Webhook autenticado con Telegram secret token.
- Mensaje tipo `/start` con argumento de codigo.
- Existe un `TelegramPairingCode` no expirado.
- `chat_id` no esta ya vinculado a otro paciente (RF-TG-003).

## Inputs (DTO interno — no es formato nativo de Telegram)

El endpoint `POST /api/v1/telegram/webhook` recibe un DTO interno con header `X-Telegram-Webhook-Secret`.
El bot adapter extrae los campos del update nativo de Telegram y los mapea a este contrato.

| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| Update | string | `message.text` o `callback_query.data` del update de Telegram | Requerido para interpretar el comando |
| ChatId | string | `message.chat.id` o `callback_query.message.chat.id` (como string) | Requerido |
| TraceId | Guid | Generado por el adapter o cliente | Requerido, no debe ser Guid.Empty |
| CallbackQueryId | string? | `callback_query.id` (solo en taps de inline keyboard) | Opcional; si presente, el handler llama `answerCallbackQuery` |
| code | string | Extraido de Update cuando es `/start BIT-XXXXX` | Formato `BIT-XXXXX` |

## Proceso (Happy Path)
1. Parsear el mensaje y extraer `code` del comando `/start BIT-XXXXX`.
2. Buscar `TelegramPairingCode` por `code` con `expires_at > now()` y `used=false`.
3. Verificar unicidad de `chat_id` mediante RF-TG-003.
4. INSERT `TelegramSession(patient_id, chat_id, status='linked', linked_at=now())`.
5. Marcar o eliminar el `TelegramPairingCode` usado dentro de la misma transaccion.
6. Responder al usuario: `Cuenta vinculada. Ya podes registrar tu humor desde aca.`

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| telegram_session_id | uuid | ID de la sesion creada |
| status | string | `linked` |
| bot_message | string | Confirmacion enviada al usuario |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| TG_002_CODE_INVALID | 200 | Codigo no existe o formato invalido | Mensaje Telegram: "Codigo invalido o expirado." |
| TG_002_CODE_EXPIRED | 200 | Codigo vencido | Mensaje Telegram: "Codigo invalido o expirado." |
| TG_002_CHAT_DUPLICATE | 200 | `chat_id` ya vinculado a otra cuenta | Mensaje Telegram: "Este Telegram ya esta vinculado a otra cuenta." |

## Casos especiales y variantes
- `/start` sin argumento: responder con instrucciones, sin crear sesion.
- El webhook siempre responde `200` a Telegram para evitar reintentos por errores de negocio ya manejados.
- El `TelegramPairingCode` se consume en la misma transaccion que crea la sesion.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| TelegramSession | INSERT | patient_id, chat_id, status, linked_at |
| TelegramPairingCode | UPDATE o DELETE | used=true o eliminacion del codigo consumido |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Vinculacion exitosa con codigo valido
  Given existe un TelegramPairingCode activo
  When webhook recibe /start BIT-ABCDE desde un chat nuevo
  Then se crea TelegramSession con status="linked"
  And el codigo queda consumido

Scenario: Codigo expirado es rechazado con mensaje de ayuda
  Given existe un TelegramPairingCode vencido
  When webhook recibe /start BIT-ABCDE
  Then no se crea sesion
  And el usuario recibe "Codigo invalido o expirado."
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-TG-002-01 | Crea TelegramSession con codigo valido | Positivo |
| TP-TG-002-02 | Rechaza codigo expirado sin crear sesion | Negativo |
| TP-TG-002-03 | Rechaza chat_id ya vinculado | Negativo |

## Sin ambiguedades pendientes
Ninguna.

## Notas de implementacion
- El DTO `{Update, ChatId, TraceId, CallbackQueryId}` es un contrato interno del bot adapter; no es el formato nativo de Telegram.
- `CallbackQueryId` permite al handler llamar `answerCallbackQuery` para descartar el spinner del boton inline inmediatamente.
- Los botones del inline keyboard de humor usan `callback_data = "+2"`, `-1"` etc., que `ParsePayload` reconoce como `mood_input` sin cambios adicionales.
- Los botones de horas de sueño usan `callback_data = "4"` .. `"9"`, reconocidos por `TryParseSleepHours`.
