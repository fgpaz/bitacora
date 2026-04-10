# RF-CON-002: Registrar aceptacion de consentimiento con version y timestamp

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-CON-002 |
| Modulo | CON |
| Actor | Patient (API) |
| Flujo fuente | FL-CON-01 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Patient autenticado con JWT valido.
- Existe consentimiento activo en configuracion de runtime.
- No existe `ConsentGrant` activo para ese `patient_id` y esa version.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| version | string | Request body | Debe coincidir con la version activa |
| accepted | bool | Request body | Debe ser `true` |
| patient_id | uuid | JWT | Existente |

## Proceso (Happy Path)
1. Leer `consent_version` activa desde configuracion del servicio.
2. Verificar que `version` coincide con la activa; rechazar si no.
3. Verificar que `accepted=true`; rechazar si `false`.
4. Verificar que no existe grant previo con `status='granted'` para esta version y paciente.
5. INSERT `ConsentGrant {patient_id, consent_version, status='granted', granted_at=NOW()}`.
6. INSERT `AccessAudit` append-only con `action_type='grant'`, `resource_type='consent_grant'`, `resource_id=consent_grant_id` y `created_at_utc`.
7. Retornar `201` con `consent_grant_id`, `status='granted'` y `granted_at`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| consent_grant_id | uuid | ID del grant creado |
| status | string | `granted` |
| granted_at | timestamp | UTC del momento de aceptacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CONSENT_VERSION_MISMATCH | 409 | `version != vigente` | {error: "CONSENT_VERSION_MISMATCH"} |
| CONSENT_ALREADY_GRANTED | 409 | Grant activo existente | {error: "CONSENT_ALREADY_GRANTED"} |
| ACCEPTED_FALSE | 422 | `accepted != true` | {error: "ACCEPTED_FALSE"} |

## Casos especiales y variantes
- Paciente re-otorga tras revocar: se crea un nuevo `ConsentGrant`; el revocado permanece en historial.
- El texto fuente del consentimiento no se persiste en DB; solo la `consent_version` aceptada.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| ConsentGrant | INSERT | consent_grant_id, patient_id, consent_version, status, granted_at |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Aceptar version vigente
  Given version vigente es "v1.2"
  When POST /api/v1/consent {version: "v1.2", accepted: true}
  Then se retorna 201 con status="granted"

Scenario: Rechazar version antigua
  Given version vigente es "v1.2"
  When POST /api/v1/consent {version: "v1.0", accepted: true}
  Then se retorna 409 con error CONSENT_VERSION_MISMATCH
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-CON-002-01 | Grant creado con version correcta | Positivo |
| TP-CON-002-02 | 409 por version desactualizada | Negativo |
| TP-CON-002-03 | 409 si ya existe grant activo | Negativo |

## Sin ambiguedades pendientes
Ninguna.
