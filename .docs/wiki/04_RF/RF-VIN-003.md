# RF-VIN-003: Aceptar vinculo mediante BindingCode

## Estado actual

`Implementado — POST /api/v1/vinculos/accept`.

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-003 |
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
| bindingCode | string | Request body | Formato `BIT-XXXXX`, existe y no expirado |
| patient_id | uuid | JWT | Existente con consentimiento activo |

## Proceso (Happy Path)
1. Validar `BindingCode` y resolver `professional_id` mediante RF-VIN-011.
2. Verificar que no existe `CareLink` activo o invitado para el par `professional_id + patient_id`.
3. INSERT `CareLink {professional_id, patient_id, status='active', can_view_data=false, invited_at=NOW(), accepted_at=NOW()}`.
4. UPDATE `BindingCode SET used=true WHERE code=?`.
5. INSERT `AccessAudit` con `action_type='create'`, `resource_type='care_link'`, `resource_id=care_link_id`.
6. Retornar `201` con `care_link_id`, `status='active'` y `can_view_data=false`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| careLinkId | uuid | ID del vinculo creado |
| status | string | `active` |
| canViewData | bool | `false` por invariante T3-11 |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| BINDING_CODE_NOT_FOUND | 404 | Codigo invalido | {error: "BINDING_CODE_NOT_FOUND"} |
| BINDING_CODE_EXPIRED | 410 | Codigo expirado | {error: "BINDING_CODE_EXPIRED"} |
| BINDING_CODE_ALREADY_USED | 409 | Codigo ya usado | {error: "BINDING_CODE_ALREADY_USED"} |
| CARELINK_EXISTS | 409 | CareLink duplicado | {error: "CARELINK_EXISTS"} |
| CONSENT_REQUIRED | 403 | Sin consentimiento activo | {error: "CONSENT_REQUIRED"} |

## Delta respecto al contrato original
- El contrato congelado preveia `POST /api/v1/care-links/{id}/accept` con path param `care_link_id` para aceptar una invitacion del profesional.
- La implementacion actual usa `POST /api/v1/vinculos/accept` con body `bindingCode` para auto-vinculacion directa.
- La invitacion formal del profesional (RF-VIN-001) queda diferida.
- No existe transicion de `invited` a `active` porque el vinculo se crea directamente en `active`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| care_link_id | uuid | ID del vinculo |
| status | string | `active` |
| can_view_data | bool | `false` por invariante |
| accepted_at | timestamp | UTC de aceptacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| LINK_NOT_FOUND | 404 | `care_link_id` inexistente | {error: "LINK_NOT_FOUND"} |
| FORBIDDEN | 403 | `patient_id` no es owner | {error: "FORBIDDEN"} |
| INVALID_STATUS | 409 | `status != invited` | {error: "INVALID_STATUS"} |

## Casos especiales y variantes
- La materializacion de un `PendingInvite` durante onboarding reanudado ocurre dentro de `FL-ONB-01`; este RF aplica a invitaciones ya materializadas como `CareLink`.
- El endpoint es idempotente solo si el cliente no reintenta luego de activado; una segunda aceptacion retorna `409 INVALID_STATUS`.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | UPDATE | status, accepted_at |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente acepta invitacion valida
  Given CareLink status="invited" pertenece al patient
  When POST /api/v1/care-links/{id}/accept
  Then status="active"
  And can_view_data=false
  And accepted_at presente

Scenario: Otro paciente no puede aceptar
  Given CareLink pertenece a otro patient
  When POST /api/v1/care-links/{id}/accept
  Then se retorna 403 FORBIDDEN
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-003-01 | Invitacion aceptada correctamente | Positivo |
| TP-VIN-003-02 | 403 si no es owner | Negativo |
| TP-VIN-003-03 | 409 si el link ya no esta invited | Negativo |

## Sin ambiguedades pendientes
Ninguna.
