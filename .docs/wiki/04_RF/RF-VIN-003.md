# RF-VIN-003: Aceptar invitacion y activar CareLink

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-003 |
| Modulo | VIN |
| Actor | Patient (API) |
| Flujo fuente | FL-VIN-01 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Patient autenticado con JWT valido.
- `CareLink` con `care_link_id` existe y tiene `status='invited'`.
- El `patient_id` del JWT coincide con el `patient_id` del `CareLink` (RF-VIN-022).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| care_link_id | uuid | Path param | Existente, `status=invited` |
| patient_id | uuid | JWT | Debe ser owner del `CareLink` |

## Proceso (Happy Path)
1. Verificar ownership del `CareLink` mediante RF-VIN-022.
2. Verificar `status='invited'`; rechazar si es otro estado.
3. UPDATE `CareLink SET status='active', accepted_at=NOW()`.
4. Mantener `can_view_data=false` en esta etapa; la apertura de visibilidad se gestiona en RF-VIN-023.
5. INSERT `AccessAudit` con `action_type='grant'`, `resource_type='care_link'`, `resource_id=care_link_id`.
6. Retornar `200` con `status='active'` y `can_view_data=false`.

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
