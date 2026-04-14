# RF-TG-011: Enviar mensaje con keyboard inline

## Execution Sheet
- Modulo: TG
- Trigger: Invocado por ReminderWorker para cada recordatorio due
- Actor: Sistema (Telegram Bot API)
- Prioridad PDP: Usability (dentro de lo que permite Security/Privacy)
- Estado: **Implementado (produccion)** — `SendReminderCommand` + `HandleWebhookUpdateCommandHandler` producen la integracion completa con Telegram Bot API (E2E verificado 2026-04-14)

## Precondiciones detalladas
- `chat_id` resuelto y valido (TelegramSession.status=linked)
- ConsentGrant.status=granted verificado antes de llamar (RF-TG-012)
- Bot token disponible como variable de entorno

## Inputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| chat_id | string | ID del chat de Telegram destino |
| patient_id | uuid | Para contexto de respuesta (callback data) |

## Proceso (Happy Path)
1. Construir payload para Telegram Bot API `sendMessage`:
   - `chat_id`: provisto
   - `text`: "Como te sentis hoy?"
   - `reply_markup.inline_keyboard`: una fila con 7 botones [-3, -2, -1, 0, +1, +2, +3]
   - Cada boton: `{ text: "-3", callback_data: "-3" }` (valor numerico directo, sin prefijo)
2. POST a `https://api.telegram.org/bot{token}/sendMessage`
3. Si HTTP 200: exito
4. Si HTTP 4xx/5xx: loguear error con chat_id (sin patient_id en log), lanzar excepcion

## Outputs
- Mensaje enviado en el chat de Telegram del paciente
- El paciente ve 7 botones con valores de -3 a +3

## Errores tipados
| Codigo | Descripcion |
|--------|-------------|
| TG_011_SEND_FAILED | Telegram API retorno error; loguear y propagar |
| TG_011_BOT_TOKEN_MISSING | Variable de entorno ausente; fallo en startup |
| TG_011_CHAT_NOT_FOUND | Telegram 400: chat_id invalido; marcar sesion para revision |

## Keyboards implementados

| Keyboard | Tipo | Botones | callback_data | Usado en |
|---------|------|---------|--------------|---------|
| Humor | InlineKeyboard | -3, -2, -1, 0, +1, +2, +3 | valor numerico directo | Recordatorio scheduler |
| Sueño | InlineKeyboard | 4h, 5h, 6h, 7h, 8h, 9h | "4".."9" | Tras registrar mood score |
| Sí/No | InlineKeyboard | Sí, No | "si", "no" | Cada factor binario (física, social, ansiedad, irritabilidad, medicacion) |

**Nota de contrato:** el campo `reply_markup` debe omitirse del JSON cuando no corresponde un keyboard (no enviar `null`). Telegram rechaza `{"reply_markup":null}` con 400.

## Casos especiales y variantes
- Telegram retorna 403 (bot bloqueado por usuario): marcar TelegramSession como `status=unlinked`
- Timeout de la llamada HTTP: reintentar con backoff 1s/2s/4s (T3-TG-01)
- Los botones no incluyen etiquetas textuales adicionales, solo el valor

## Impacto en modelo de datos
- Solo lectura durante la operacion
- Si chat_id invalido (403): UPDATE `telegram_sessions SET status='unlinked'`

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Mensaje enviado exitosamente
  Given chat_id valido y bot token configurado
  When RF-TG-011 es invocado
  Then Telegram API recibe sendMessage con inline keyboard de 7 botones
  And retorna HTTP 200

Scenario: Usuario bloqueo el bot (403)
  Given Telegram retorna 403 para el chat_id
  When RF-TG-011 intenta enviar
  Then TelegramSession.status se actualiza a unlinked
  And el error se loguea sin exponer patient_id

Scenario: Bot token ausente impide iniciar el servicio
  Given BOT_TOKEN no esta en variables de entorno
  When la aplicacion inicia
  Then error en startup, no en runtime de envio
```

## Trazabilidad de tests
- UT: TG011_Payload_InlineKeyboard7Buttons
- UT: TG011_403_MarksSessionUnlinked
- IT: TG011_BotTokenMissing_FailsOnStartup

## Sin ambiguedades pendientes
- Texto del mensaje: exactamente "Como te sentis hoy?" (sin tilde, para compatibilidad)
- `callback_data` formato: `mood:{valor}` donde valor es entero de -3 a 3
