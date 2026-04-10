# RF-VIN-023: Gestionar can_view_data por paciente

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-023 |
| Modulo | VIN |
| Actor | Patient (API) |
| Flujo fuente | FL-VIN-04 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Patient autenticado con JWT valido.
- `CareLink` con `care_link_id` existe y tiene `status='active'`.
- El `patient_id` del JWT es owner del `CareLink` (RF-VIN-022).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| care_link_id | uuid | Path param | Existente, `status=active` |
| can_view_data | bool | Request body | Valor objetivo requerido |
| patient_id | uuid | JWT | Owner del `CareLink` |

## Proceso (Happy Path)
1. Verificar ownership mediante RF-VIN-022.
2. Verificar `status='active'`.
3. UPDATE `CareLink SET can_view_data = <valor solicitado>`.
4. INSERT `AccessAudit` con `action_type='grant'` si se habilita o `action_type='revoke'` si se deshabilita; `resource_type='care_link'`.
5. Retornar `200` con el estado actualizado.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| care_link_id | uuid | ID del vinculo |
| can_view_data | bool | Estado final de visibilidad |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| FORBIDDEN | 403 | El solicitante no es el paciente owner | {error: "FORBIDDEN"} |
| INVALID_STATUS | 409 | `CareLink` no esta `active` | {error: "INVALID_STATUS"} |
| LINK_NOT_FOUND | 404 | `care_link_id` inexistente | {error: "LINK_NOT_FOUND"} |

## Casos especiales y variantes
- Habilitar y deshabilitar usan el mismo endpoint y la misma validacion.
- Si el valor solicitado coincide con el actual, la operacion es idempotente y retorna `200` sin side effects adicionales fuera del audit.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | UPDATE | can_view_data |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente habilita can_view_data
  Given CareLink status="active" pertenece al patient
  When PATCH /api/v1/care-links/{id} {can_view_data: true}
  Then CareLink.can_view_data=true

Scenario: Paciente deshabilita can_view_data
  Given CareLink status="active" y can_view_data=true
  When PATCH /api/v1/care-links/{id} {can_view_data: false}
  Then CareLink.can_view_data=false

Scenario: Professional no puede cambiar visibilidad
  Given JWT pertenece al professional del CareLink
  When PATCH /api/v1/care-links/{id} {can_view_data: true}
  Then se retorna 403 FORBIDDEN
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-023-01 | Paciente habilita visibilidad exitosamente | Positivo |
| TP-VIN-023-02 | Paciente deshabilita visibilidad exitosamente | Positivo |
| TP-VIN-023-03 | 403 si el requestor no es el owner | Negativo |

## Sin ambiguedades pendientes
Ninguna.
