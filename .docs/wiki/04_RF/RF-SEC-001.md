# RF-SEC-001: Interceptor de audit para acceso profesional

## Execution Sheet
- Modulo: SEC
- Mecanismo: Middleware / ActionFilter en endpoints profesionales
- Actor: Sistema (automatico, no invocado explicitamente)
- Prioridad PDP: Security (obligatorio, no bypasseable)

## Precondiciones detalladas
- El interceptor se aplica a todos los endpoints donde `rol=professional` accede a datos de pacientes
- JWT del professional ya fue validado por el pipeline de autenticacion
- El interceptor corre ANTES de retornar datos al cliente
- Si el audit falla, el request falla (RF-SEC-003)

## Inputs
| Campo | Origen | Descripcion |
|-------|--------|-------------|
| actor_id | JWT claims | supabase_user_id del profesional |
| patient_id | Route/Query param | ID del paciente accedido |
| trace_id | Header X-Trace-Id | Correlacion del request |
| action_type | Atributo del endpoint | Constante definida por decorator/attribute |
| resource_type | Atributo del endpoint | Tipo de recurso accedido |
| resource_id | Route param o result | ID del recurso especifico |

## Proceso (Happy Path)
1. Extraer `actor_id` del JWT
2. Calcular `pseudonym_id` via RF-SEC-002
3. Obtener `trace_id` del header (generar UUID si ausente)
4. Construir registro `AccessAudit`:
   - audit_id: nuevo UUID v4
   - actor_id, pseudonym_id, trace_id: calculados
   - action_type, resource_type, resource_id: del contexto del endpoint
   - patient_id: del contexto
   - outcome: SUCCESS (pre-insert; si la accion falla, actualizar a FAILURE o insertar nuevo)
   - created_at_utc: now() UTC
5. INSERT en `access_audits`
6. Si INSERT falla → HTTP 500, no retornar datos (RF-SEC-003)
7. Continuar con la ejecucion del endpoint

## Outputs
- Efecto: registro insertado en `access_audits`
- El cliente no ve nada del audit en la respuesta

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| SEC_001_AUDIT_INSERT_FAILED | 500 | Fallo en DB; fail-closed |
| SEC_001_SALT_MISSING | 500 | env_salt no disponible (RF-SEC-002) |
| SEC_001_ACTOR_MISSING | 500 | actor_id no resolvible del JWT |

## Casos especiales y variantes
- Endpoints de paciente (no profesional): interceptor no aplica
- Batch de pacientes (dashboard): ver RF-VIS-014 para batch audit
- Endpoints publicos / health: interceptor no aplica
- `resource_id` no disponible pre-ejecucion: puede ser null; se actualiza post-ejecucion si es necesario

## Impacto en modelo de datos
- INSERT en `access_audits` (append-only)

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Acceso profesional genera audit automaticamente
  Given profesional autenticado accede a datos de paciente
  When el request es procesado
  Then un AccessAudit es insertado con actor_id, pseudonym_id, trace_id y outcome=SUCCESS

Scenario: Fallo en INSERT audit aborta el request
  Given el INSERT en access_audits lanza excepcion
  When el profesional accede al endpoint
  Then HTTP 500 y ningun dato de paciente es retornado

Scenario: Endpoint de paciente no genera audit
  Given paciente autenticado accede a sus propios datos
  When el request es procesado
  Then no se inserta AccessAudit
```

## Trazabilidad de tests
- UT: SEC001_AuditRecord_CorrectFields
- IT: SEC001_ProfessionalEndpoint_AuditInserted
- IT: SEC001_AuditFail_Returns500_NoData
- IT: SEC001_PatientEndpoint_NoAudit

## Sin ambiguedades pendientes
- El interceptor es declarativo (attribute/decorator), no se debe agregar manualmente en cada endpoint
- `outcome` inicial es SUCCESS; si el handler lanza excepcion, el audit ya fue insertado con SUCCESS (aceptable, se registra el intento)
