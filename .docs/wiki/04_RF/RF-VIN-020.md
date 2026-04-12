# RF-VIN-020: Revocar CareLink por paciente

## Estado actual

`Implementado — DELETE /api/v1/vinculos/{id}`.

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-020 |
| Modulo | VIN |
| Actor | Patient (API) |
| Flujo fuente | FL-VIN-03 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Patient autenticado con JWT valido.
- CareLink con `care_link_id` existe y tiene `status='active'` o `status='invited'`.
- El `patient_id` del JWT es owner del CareLink (RF-VIN-022).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| care_link_id | uuid | Path param `{id}` | Existente |
| confirmed | bool | Request body | Obligatorio; debe ser `true` |
| patient_id | uuid | JWT | Owner del CareLink |

## Proceso (Happy Path)
1. Verificar ownership (RF-VIN-022).
2. Verificar status en `('active', 'invited')`.
3. UPDATE `CareLink SET status='revoked_by_patient', revoked_at=NOW()`.
4. INSERT `AccessAudit` con `action_type='revoke'`, `resource_type='care_link'`, `resource_id=care_link_id`.
5. Retornar `200` con `care_link_id` y `status='revoked_by_patient'`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| careLinkId | uuid | ID del vinculo |
| status | string | `revoked_by_patient` |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| FORBIDDEN | 403 | Actor no es owner del vinculo | {error: "FORBIDDEN"} |
| NOT_FOUND | 404 | CareLink inexistente | {error: "NOT_FOUND"} |
| UNPROCESSABLE_ENTITY | 422 | CareLink no esta en `active` ni `invited` | {error: "UNPROCESSABLE_ENTITY"} |

## Delta respecto al contrato original
- Requiere cuerpo `{"Confirmed": true}` (no era parte del contrato original).
- La invalidacion de caches del profesional (RF-VIN-021) queda diferida.
- El endpoint usa ruta `/api/v1/vinculos/{id}` (no `/care-links/{id}`).

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| care_link_id | uuid | ID del vinculo |
| status | string | "revoked_by_patient" |
| revoked_at | timestamp | UTC de revocacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| LINK_NOT_FOUND | 404 | care_link_id inexistente | {error: "LINK_NOT_FOUND"} |
| FORBIDDEN | 403 | patient_id no es owner | {error: "FORBIDDEN"} |
| INVALID_STATUS | 409 | status ya revocado | {error: "INVALID_STATUS"} |

## Casos especiales y variantes
- Revocacion inmediata: sin periodo de gracia.
- Professional pierde acceso a datos del patient en el instante del UPDATE.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | UPDATE | status, revoked_at |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente revoca vinculo activo
  Given CareLink status="active" pertenece al patient
  When DELETE /api/v1/care-links/{id}
  Then status="revoked_by_patient" y revoked_at presente

Scenario: Paciente no puede revocar vinculo ajeno
  Given CareLink pertenece a otro patient
  When DELETE /api/v1/care-links/{id}
  Then se retorna 403 FORBIDDEN
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-020-01 | Revocacion exitosa por owner | Positivo |
| TP-VIN-020-02 | 403 si no es owner | Negativo |
| TP-VIN-020-03 | 409 si ya revocado | Negativo |

## Sin ambiguedades pendientes
Ninguna.
