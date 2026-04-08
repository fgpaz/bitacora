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
- Existe version vigente publicada en consent_versions.
- No existe ConsentGrant activo para ese patient_id y esa version.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| version | string | Request body | Debe coincidir con version vigente |
| accepted | bool | Request body | Debe ser true |
| patient_id | uuid | JWT | Existente |

## Proceso (Happy Path)
1. Verificar que version coincide con la vigente; rechazar si no.
2. Verificar que accepted=true; rechazar si false.
3. Verificar que no existe grant previo con status='granted' para esta version.
4. INSERT ConsentGrant {patient_id, consent_version, status='granted', granted_at=NOW()}.
5. INSERT AccessAudit con trace_id, operacion='CONSENT_GRANT'.
6. Retornar 201 con consent_grant_id y granted_at.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| consent_grant_id | uuid | ID del grant creado |
| status | string | "granted" |
| granted_at | timestamp | UTC del momento de aceptacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| VERSION_MISMATCH | 409 | version != vigente | {error: "VERSION_MISMATCH"} |
| CONSENT_ALREADY_GRANTED | 409 | Grant activo existente | {error: "CONSENT_ALREADY_GRANTED"} |
| ACCEPTED_FALSE | 422 | accepted != true | {error: "ACCEPTED_FALSE"} |

## Casos especiales y variantes
- Paciente re-otorga tras revocar: se crea nuevo ConsentGrant; el revocado permanece en historial.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| ConsentGrant | INSERT | consent_grant_id, patient_id, consent_version, status, granted_at |
| AccessAudit | INSERT | trace_id, patient_id, operacion, created_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Aceptar version vigente
  Given version vigente es "v1.2"
  When POST /api/v1/consent {version: "v1.2", accepted: true}
  Then se retorna 201 con status="granted"

Scenario: Rechazar version antigua
  Given version vigente es "v1.2"
  When POST /api/v1/consent {version: "v1.0", accepted: true}
  Then se retorna 409 con error VERSION_MISMATCH
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-CON-002-01 | Grant creado con version correcta | Positivo |
| TP-CON-002-02 | 409 por version desactualizada | Negativo |
| TP-CON-002-03 | 409 si ya existe grant activo | Negativo |

## Sin ambiguedades pendientes
Ninguna.
