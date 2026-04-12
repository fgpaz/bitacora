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

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| chat_id | bigint | Telegram Update | Requerido |
| code | string | Texto del mensaje | Formato esperado `BIT-XXXXX` |

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
