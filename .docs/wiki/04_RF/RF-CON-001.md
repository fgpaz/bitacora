# RF-CON-001: Presentar texto de consentimiento vigente autenticado

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-CON-001 |
| Modulo | CON |
| Actor | Patient (API) |
| Flujo fuente | FL-CON-01 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Patient autenticado con JWT valido.
- Existe configuracion activa de consentimiento en runtime (`Consent__ActiveVersion`, `Consent__ActiveText`, `Consent__Sections`).
- El `patient_id` resuelto desde JWT corresponde a un `User` existente.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | JWT | Existente |

## Proceso (Happy Path)
1. Leer `version`, `text` y `sections` del consentimiento activo desde configuracion de servicio.
2. Resolver el `ConsentGrant` mas reciente del paciente para la `consent_version` activa.
3. Retornar `version`, `text`, `sections` y `patient_status`.
4. INSERT `AccessAudit` append-only con `action_type='read'`, `resource_type='consent_grant'`, `patient_id` y `created_at_utc`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| version | string | Identificador de version vigente |
| text | string | Texto legal completo |
| sections | jsonb | Secciones numeradas del texto |
| patient_status | string | `pending`, `granted`, `revoked` o `none` |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| NO_CONSENT_CONFIG | 503 | No hay consentimiento activo configurado | {error: "NO_CONSENT_CONFIG"} |
| PATIENT_NOT_FOUND | 404 | patient_id invalido | {error: "PATIENT_NOT_FOUND"} |

## Casos especiales y variantes
- Si no existe `ConsentGrant` para la version activa, retornar `patient_status="none"`.
- Versiones historicas no se exponen desde este endpoint; siempre se retorna la activa.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| ConsentGrant | SELECT | patient_id, consent_version, status |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Leer consentimiento activo con grant vigente
  Given existe consentimiento activo "v1.2" en configuracion
  And paciente autenticado tiene ConsentGrant.status="granted"
  When GET /api/v1/consent/current con JWT valido
  Then se retorna 200 con version="v1.2"
  And patient_status="granted"

Scenario: Paciente sin grant activo recibe status none
  Given existe consentimiento activo "v1.2" en configuracion
  And el paciente no tiene ConsentGrant para "v1.2"
  When GET /api/v1/consent/current con JWT valido
  Then se retorna 200 con patient_status="none"

Scenario: Falla si no hay consentimiento configurado
  Given no existe consentimiento activo en configuracion
  When GET /api/v1/consent/current con JWT valido
  Then se retorna 503 con error NO_CONSENT_CONFIG
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-CON-001-01 | Retorna consentimiento activo autenticado | Positivo |
| TP-CON-001-02 | Retorna patient_status=none si no existe grant activo | Positivo |
| TP-CON-001-03 | 503 si falta configuracion activa de consentimiento | Negativo |

## Sin ambiguedades pendientes
Ninguna.
