# RF-SEC-003: Fail-closed: no retornar datos si audit falla

## Execution Sheet
- Modulo: SEC
- Mecanismo: Regla de negocio transversal; implementada en RF-SEC-001 y RF-VIS-014
- Actor: Sistema
- Prioridad PDP: Security (no negociable)

## Precondiciones detalladas
- Aplica a CUALQUIER endpoint donde un profesional accede a datos de pacientes
- El INSERT en `access_audits` debe ocurrir ANTES de que los datos sean retornados al cliente
- Si el INSERT falla por cualquier razon (DB down, constraint, timeout), el request DEBE fallar

## Inputs
- No tiene inputs propios; es una politica que envuelve el flujo de RF-SEC-001

## Proceso (Happy Path)
1. Ejecutar INSERT AccessAudit (RF-SEC-001)
2. Si INSERT exitoso: continuar con el handler del endpoint, retornar datos
3. Si INSERT lanza cualquier excepcion:
   a. Loguear la excepcion con trace_id y actor_id (nunca patient_id en logs)
   b. Retornar HTTP 500 con cuerpo `{ "error": "SEC_003_AUDIT_REQUIRED" }`
   c. NO ejecutar el handler del endpoint
   d. NO retornar ningun dato de paciente

## Outputs
- Si falla: HTTP 500 con `SEC_003_AUDIT_REQUIRED`
- Ningun dato de paciente en la respuesta de error

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| SEC_003_AUDIT_REQUIRED | 500 | El sistema no puede garantizar audit; datos no retornados |

## Casos especiales y variantes
- DB temporalmente no disponible: HTTP 500, no datos
- Timeout en INSERT: HTTP 500, no datos
- Constraint violation en audit: HTTP 500, no datos (no intentar workaround)
- Dos inserts de audit para el mismo request (bug): primer insert prevalece; segundo podria fallar por duplicate key pero los datos del primero ya fueron retornados (no es el escenario de fail-closed)

## Impacto en modelo de datos
- Si se ejecuta correctamente: INSERT en `access_audits`
- Si falla: no hay escritura en ninguna tabla y no hay lectura de datos retornada

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Audit exitoso, datos retornados normalmente
  Given INSERT en access_audits es exitoso
  When profesional accede a datos de paciente
  Then HTTP 200 con datos del paciente

Scenario: Fallo en audit, datos NO retornados
  Given el INSERT en access_audits lanza DbException
  When profesional accede a datos de paciente
  Then HTTP 500 con { "error": "SEC_003_AUDIT_REQUIRED" }
  And la respuesta no contiene ningun dato del paciente

Scenario: Timeout en audit, datos NO retornados
  Given el INSERT en access_audits excede el timeout configurado
  When profesional accede a datos de paciente
  Then HTTP 500, ningun dato expuesto
```

## Trazabilidad de tests
- UT: SEC003_AuditFail_ResponseContainsNoPatientData
- IT: SEC003_DBDown_Returns500
- IT: SEC003_AuditTimeout_Returns500_NoData

## Sin ambiguedades pendientes
- "Fail-closed" es absoluto: no hay modo de fallback ni flag de feature para deshabilitarlo
- El log del error de audit NO debe incluir patient_id (solo trace_id y tipo de error)
- El cliente recibe HTTP 500, nunca 200 con datos parciales
