# RF-ONB-005: Transicionar User a estado active

## Execution Sheet
- Modulo: ONB
- Trigger: Evento `FirstMoodEntryCreated` emitido por RF-ONB-004
- Actor: Sistema
- Prioridad PDP: Correctness

## Precondiciones detalladas
- Evento `FirstMoodEntryCreated` recibido con `patient_id`
- User.status debe ser `consent_granted` para que la transicion aplique
- Si User.status ya es `active`, la operacion es no-op (idempotente)
- La transicion es unidireccional: consent_granted → active

## Inputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| patient_id | uuid | Del evento FirstMoodEntryCreated |

## Proceso (Happy Path)
1. Recibir evento con `patient_id`
2. Query: `SELECT status FROM users WHERE user_id = @patientId`
3. Si `status = active`: no-op, retornar (idempotente)
4. Si `status = consent_granted`:
   a. UPDATE `users SET status = 'active' WHERE user_id = @patientId AND status = 'consent_granted'`
   b. Verificar que el UPDATE afecto exactamente 1 fila
5. Si `status = registered`: loguear warning (consent deberia haber sido otorgado), no transicionar

## Outputs
- Efecto: `User.status = active`
- No retorna datos al cliente (es un handler interno de evento)

## Errores tipados
| Codigo | Descripcion |
|--------|-------------|
| ONB_005_UNEXPECTED_STATUS | User en status=registered recibe evento; loguear, no transicionar |
| ONB_005_UPDATE_FAILED | UPDATE no afecto ninguna fila; loguear para investigacion |

## Casos especiales y variantes
- Evento duplicado (retry): UPDATE con `AND status = 'consent_granted'` garantiza idempotencia
- User no encontrado: loguear error, no lanzar excepcion al caller del evento
- La transicion falla: el MoodEntry ya fue creado y persiste; no se revierte (eventual consistency aceptable)

## Impacto en modelo de datos
- UPDATE `users SET status = 'active'`
- Solo aplica si `status = consent_granted` (condicion en WHERE)

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Transicion exitosa de consent_granted a active
  Given User.status=consent_granted
  When evento FirstMoodEntryCreated es recibido
  Then User.status actualizado a active

Scenario: Evento duplicado no causa error
  Given User.status=active (ya transicionado)
  When evento FirstMoodEntryCreated llega de nuevo
  Then no-op, User.status permanece active, sin error

Scenario: User en status=registered recibe evento por error
  Given User.status=registered
  When evento FirstMoodEntryCreated es recibido
  Then warning logueado, status NO cambia
  And ningun error propagado al emisor del evento
```

## Trazabilidad de tests
- UT: ONB005_ConsentGranted_TransitionsToActive
- UT: ONB005_AlreadyActive_NoOp
- UT: ONB005_Registered_LogsWarning_NoTransition
- IT: ONB005_ConcurrentEvents_Idempotent

## Sin ambiguedades pendientes
- La transicion no se puede revertir: una vez active, el usuario no puede volver a consent_granted via este flujo
- El evento es el unico mecanismo para esta transicion; no hay endpoint directo para setear status=active
