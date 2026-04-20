---
id: RF-TG-003
title: Validar unicidad de chat_id
implements:
  - src/Bitacora.Application/Commands/Telegram/ConfirmPairingCommand.cs
  - src/Bitacora.DataAccess.EntityFramework/Repositories/TelegramSessionRepository.cs
  - src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs
tests:
  - src/Bitacora.Tests/ReminderScheduleTests.cs
---

# RF-TG-003: Validar unicidad de chat_id

## Execution Sheet
- Modulo: TG
- Trigger: Invocado por RF-TG-002 antes de crear TelegramSession
- Actor: Sistema
- Prioridad PDP: Security > Correctness

## Precondiciones detalladas
- `chat_id` obtenido del webhook de Telegram
- Tabla `telegram_sessions` tiene partial unique index `UNIQUE(chat_id) WHERE status='Linked'`
- Un chat_id solo puede estar vinculado a un patient_id a la vez

## Inputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| chat_id | long/string | ID del chat de Telegram a vincular |
| patient_id | uuid | Paciente que intenta vincular |

## Proceso (Happy Path)
1. Query: `SELECT patient_id FROM telegram_sessions WHERE chat_id = @chatId AND status = 'linked'`
2. Si no existe fila: chat_id disponible, continuar con RF-TG-002
3. Si existe fila con `patient_id = @patientId` (mismo paciente): sesion existente, devolver confirmacion idempotente
4. Si existe fila con `patient_id != @patientId`: rechazar con TG_003_CHAT_DUPLICATE

## Outputs
- No retorna datos al cliente directamente
- Lanza excepcion tipada si duplicado detectado

## Errores tipados
| Codigo | HTTP/Accion | Descripcion |
|--------|-------------|-------------|
| TG_003_CHAT_DUPLICATE | Abortar vinculacion | chat_id ya vinculado a otro patient_id |
| TG_003_ALREADY_LINKED | Informativo | Mismo paciente ya tiene sesion con este chat_id |

## Casos especiales y variantes
- Mismo paciente con sesion status=unlinked en este chat_id: se puede re-vincular (no es duplicado)
- Constraint UNIQUE en DB como backstop: si la validacion en aplicacion falla, el INSERT lanza UniqueViolationException que debe mapearse a TG_003_CHAT_DUPLICATE
- chat_id con status=unlinked de otro paciente: tratar como disponible (no duplicado activo)

## Impacto en modelo de datos
- Solo lectura en validacion previa
- UNIQUE constraint en `telegram_sessions(chat_id)` donde `status='linked'` (partial unique index)

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: chat_id libre es aceptado
  Given ningun TelegramSession activo con chat_id=999
  When se valida unicidad de chat_id=999
  Then validacion pasa, continuar con creacion de sesion

Scenario: chat_id de otro paciente es rechazado
  Given TelegramSession linked con chat_id=999 para patient_id=1
  When patient_id=2 intenta vincular chat_id=999
  Then TG_003_CHAT_DUPLICATE y vinculacion abortada
  And mensaje al usuario en Telegram explicando conflicto

Scenario: Mismo paciente re-vincula su propio chat_id
  Given TelegramSession unlinked con chat_id=999 para patient_id=1
  When patient_id=1 intenta vincular chat_id=999
  Then validacion pasa (sesion anterior unlinked no cuenta)
```

## Trazabilidad de tests
- UT: TG003_FreeChatId_Passes
- UT: TG003_OtherPatientLinked_Rejected
- IT: TG003_UniqueConstraint_Backstop
- IT: TG003_UnlinkedSamePatient_CanRelink

## Sin ambiguedades pendientes
- El partial unique index aplica solo sobre registros con `status='linked'`
- El mensaje al usuario en Telegram no debe revelar que otro paciente usa ese chat_id
