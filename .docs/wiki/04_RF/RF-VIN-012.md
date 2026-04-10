# RF-VIN-012: Crear CareLink por auto-vinculacion

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-012 |
| Modulo | VIN |
| Actor | Patient (API) |
| Flujo fuente | FL-VIN-02 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Patient autenticado con JWT valido.
- Patient tiene `ConsentGrant.status='granted'`.
- Existe un `BindingCode` valido segun RF-VIN-011.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| code | string | Request body | Formato `BIT-XXXXX`, valido segun RF-VIN-011 |
| patient_id | uuid | JWT | Existente con consentimiento activo |

## Proceso (Happy Path)
1. Validar `BindingCode` y resolver `professional_id` mediante RF-VIN-011.
2. Verificar que no existe `CareLink` activo o invitado entre `professional_id` y `patient_id`.
3. INSERT `CareLink {professional_id, patient_id, status='active', can_view_data=false, invited_at=NOW(), accepted_at=NOW()}`.
4. UPDATE `BindingCode SET used=true WHERE code=?`.
5. INSERT `AccessAudit` con `action_type='create'`, `resource_type='care_link'`, `resource_id=care_link_id`.
6. Retornar `201` con `care_link_id`, `status='active'` y `can_view_data=false`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| care_link_id | uuid | ID del vinculo creado |
| status | string | `active` |
| can_view_data | bool | `false` por invariante |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| BINDING_CODE_NOT_FOUND | 404 | Codigo invalido | {error: "BINDING_CODE_NOT_FOUND"} |
| BINDING_CODE_EXPIRED | 410 | Codigo expirado | {error: "BINDING_CODE_EXPIRED"} |
| BINDING_CODE_ALREADY_USED | 409 | Codigo ya usado | {error: "BINDING_CODE_ALREADY_USED"} |
| CARELINK_EXISTS | 409 | CareLink duplicado | {error: "CARELINK_EXISTS"} |
| CONSENT_REQUIRED | 403 | Sin consentimiento activo | {error: "CONSENT_REQUIRED"} |

## Casos especiales y variantes
- La auto-vinculacion crea el link directamente en `status='active'`.
- `can_view_data` permanece `false` hasta que el paciente lo habilite expresamente en RF-VIN-023.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | INSERT | professional_id, patient_id, status, can_view_data, invited_at, accepted_at |
| BindingCode | UPDATE | used=true |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente se vincula con codigo valido
  Given existe un BindingCode valido y no usado
  When POST /api/v1/care-links/bind {code: "BIT-ABCDE"}
  Then CareLink creado con status="active"
  And can_view_data=false

Scenario: Codigo expirado es rechazado
  When POST /api/v1/care-links/bind {code: "BIT-OLD00"}
  Then se retorna 410 BINDING_CODE_EXPIRED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-012-01 | Vinculo creado con BindingCode valido | Positivo |
| TP-VIN-012-02 | 410 con BindingCode expirado | Negativo |
| TP-VIN-012-03 | BindingCode queda marcado como usado | Positivo |

## Sin ambiguedades pendientes
Ninguna.
