# RF-TG-002: Vincular chat_id via /start + code

## Execution Sheet
- Modulo: TG
- Trigger: Webhook de Telegram recibe mensaje `/start BIT-XXXXX`
- Actor: Sistema (webhook handler)
- Prioridad PDP: Security > Correctness

## Precondiciones detalladas
- Webhook autenticado con Telegram secret token
- Mensaje tipo `/start` con argumento de codigo
- `TelegramPairingCode` existe y no ha expirado (`expires_at > now()`)
- chat_id no debe estar ya vinculado a otro patient_id (ver RF-TG-003)

## Inputs
| Campo | Origen | Descripcion |
|-------|--------|-------------|
| chat_id | Telegram Update | ID del chat de Telegram |
| code | Texto del mensaje | Argumento del /start, ej: BIT-A3K9Z |

## Proceso (Happy Path)
1. Parsear mensaje: extraer `code` del comando `/start BIT-XXXXX`
2. Buscar `TelegramPairingCode` por `code` WHERE `expires_at > now()`
3. Si no encontrado o expirado → responder al usuario "Codigo invalido o expirado"
4. Verificar unicidad de chat_id (RF-TG-003)
5. Crear `TelegramSession(patient_id, chat_id, status=linked, created_at=now())`
6. DELETE `TelegramPairingCode` usado
7. Responder al usuario en Telegram: "Cuenta vinculada exitosamente"

## Outputs
- Efecto: `TelegramSession` creada con status=linked
- Mensaje Telegram enviado al usuario confirmando vinculacion

## Errores tipados
| Codigo | Descripcion |
|--------|-------------|
| TG_002_CODE_INVALID | Codigo no encontrado |
| TG_002_CODE_EXPIRED | Codigo vencido |
| TG_002_CHAT_DUPLICATE | chat_id ya vinculado (RF-TG-003) |

## Casos especiales y variantes
- `/start` sin argumento: responder con instrucciones, no procesar
- Codigo con formato incorrecto (no BIT-XXXXX): tratar como invalido
- Error al crear TelegramSession: responder "Error interno, intentar de nuevo"

## Impacto en modelo de datos
- INSERT `telegram_sessions`
- DELETE `telegram_pairing_codes`
- Ambas operaciones en una transaccion

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Vinculacion exitosa con codigo valido
  Given TelegramPairingCode BIT-A3K9Z activo para patient_id=42
  When webhook recibe /start BIT-A3K9Z desde chat_id=999
  Then TelegramSession creada con patient_id=42, chat_id=999, status=linked
  And TelegramPairingCode eliminado
  And usuario recibe confirmacion en Telegram

Scenario: Codigo expirado es rechazado
  Given TelegramPairingCode BIT-A3K9Z con expires_at en el pasado
  When webhook recibe /start BIT-A3K9Z
  Then no se crea sesion
  And usuario recibe mensaje de codigo expirado
```

## Trazabilidad de tests
- UT: TG002_ValidCode_CreatesSession
- UT: TG002_ExpiredCode_Rejected
- IT: TG002_Transaction_SessionAndCodeAtomic
- IT: TG002_DuplicateChatId_Rejected

## Sin ambiguedades pendientes
- El codigo se compara case-sensitive
- El DELETE del codigo ocurre en la misma transaccion que el INSERT de la sesion
