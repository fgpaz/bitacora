# RF-VIN-023: Activar can_view_data por paciente

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-023 |
| Modulo | VIN |
| Actor | Patient (API) |
| Flujo fuente | FL-VIN-03 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Patient autenticado con JWT valido.
- CareLink con care_link_id existe y tiene status='active'.
- El patient_id del JWT es owner del CareLink (RF-VIN-022).
- can_view_data actual es false.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| care_link_id | uuid | Path param | Existente, status=active |
| can_view_data | bool | Request body | Debe ser true |
| patient_id | uuid | JWT | Owner del CareLink |

## Proceso (Happy Path)
1. Verificar ownership (RF-VIN-022).
2. Verificar status='active'.
3. Verificar que el solicitante es el patient_id, no el professional_id.
4. UPDATE CareLink SET can_view_data=true.
5. INSERT AccessAudit operacion='CARELINK_VIEW_ENABLED'.
6. Retornar 200 con can_view_data=true.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| care_link_id | uuid | ID del vinculo |
| can_view_data | bool | true tras la actualizacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| FORBIDDEN | 403 | Professional intenta cambiar can_view_data | {error: "FORBIDDEN"} |
| FORBIDDEN | 403 | patient_id no es owner | {error: "FORBIDDEN"} |
| INVALID_STATUS | 409 | CareLink no esta active | {error: "INVALID_STATUS"} |
| LINK_NOT_FOUND | 404 | care_link_id inexistente | {error: "LINK_NOT_FOUND"} |

## Casos especiales y variantes
- Desactivar can_view_data (PATCH con can_view_data=false): permitido, misma logica inversa.
- Professional nunca puede invocar este endpoint para su propio beneficio.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | UPDATE | can_view_data |
| AccessAudit | INSERT | trace_id, patient_id, care_link_id, operacion, created_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente activa can_view_data
  Given CareLink status="active" pertenece al patient
  When PATCH /api/v1/care-links/{id} {can_view_data: true}
  Then CareLink.can_view_data=true

Scenario: Professional no puede activar can_view_data
  Given JWT pertenece al professional del CareLink
  When PATCH /api/v1/care-links/{id} {can_view_data: true}
  Then se retorna 403 FORBIDDEN
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-023-01 | Paciente activa can_view_data exitosamente | Positivo |
| TP-VIN-023-02 | 403 si requestor es el professional | Negativo |
| TP-VIN-023-03 | 409 si CareLink no esta active | Negativo |

## Sin ambiguedades pendientes
Ninguna.
