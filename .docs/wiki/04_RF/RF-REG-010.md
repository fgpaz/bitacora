# RF-REG-010: Recibir y validar webhook de Telegram

## Estado actual

`Diferido`.

Este RF describe la conducta objetivo del canal Telegram. El runtime actual no registra `POST /api/v1/telegram/webhook` y no materializa `TelegramSession`.

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-010 |
| Modulo | REG |
| Actor | Telegram Bot API |
| Flujo fuente | FL-REG-02 |
| Prioridad | Security |

## Precondiciones detalladas
- Endpoint POST /api/v1/telegram/webhook registrado en Telegram Bot API.
- Secret token configurado en variable de entorno (no hardcoded).
- Firma X-Telegram-Bot-Api-Secret-Token presente en header.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| X-Telegram-Bot-Api-Secret-Token | header | Telegram | Debe coincidir con secret configurado |
| update | json | Request body | Objeto Update de Telegram valido |
| chat_id | int | update.message / update.callback_query | Requerido para resolucion de sesion |

## Proceso (Happy Path)
1. Verificar header X-Telegram-Bot-Api-Secret-Token contra secret configurado.
2. Parsear body como objeto Update de Telegram.
3. Extraer chat_id de message.chat.id o callback_query.message.chat.id.
4. Determinar tipo: message de texto o callback_data de inline keyboard.
5. Enrutar a RF-REG-011 para resolucion de sesion.
6. Retornar 200 OK inmediatamente (Telegram requiere respuesta rapida).

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| status | int | 200 OK siempre que firma sea valida |
| parsed_update | object | Update parseado para procesamiento interno |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| INVALID_SIGNATURE | 403 | Secret token no coincide | {error: "INVALID_SIGNATURE"} |
| MALFORMED_UPDATE | 400 | Body no es Update de Telegram valido | {error: "MALFORMED_UPDATE"} |

## Casos especiales y variantes
- Updates sin chat_id reconocible: descartar silenciosamente con 200 (evitar reintentos de Telegram).
- Tipo edited_message: ignorar, retornar 200.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| Ninguna | — | Solo validacion y enrutamiento |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Webhook valido es procesado
  Given el header secret es correcto
  And el body es un Update de Telegram valido
  When POST /api/v1/telegram/webhook
  Then se retorna 200 y se enruta a resolucion de sesion

Scenario: Firma invalida es rechazada
  Given el header secret es incorrecto
  When POST /api/v1/telegram/webhook
  Then se retorna 403 INVALID_SIGNATURE
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-010-01 | Webhook con firma valida retorna 200 | Positivo |
| TP-REG-010-02 | Firma incorrecta retorna 403 | Negativo |
| TP-REG-010-03 | Body malformado retorna 400 | Negativo |

## Sin ambiguedades pendientes
Ninguna.
