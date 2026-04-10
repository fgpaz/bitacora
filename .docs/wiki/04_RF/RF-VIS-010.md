# RF-VIS-010: Listar pacientes visibles para dashboard profesional

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIS-010 |
| Modulo | VIS |
| Actor | Professional (API) |
| Flujo fuente | FL-VIS-02 |
| Prioridad | Security |

## Precondiciones detalladas
- JWT valido con `role=professional` y `User.status=active`.
- Solo se exponen pacientes donde existe `CareLink` con `professional_id=actor_id`, `status=active` y `can_view_data=true`.
- La auditoria por paciente expuesto se delega a RF-VIS-014.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| cursor | string | Query string (opcional) | Cursor de paginacion valido |
| page_size | int | Query string (opcional) | Maximo 20, default 20 |

## Proceso (Happy Path)
1. Extraer `professional_id` del JWT.
2. Query `CareLink` filtrando `status='active'` y `can_view_data=true`.
3. Aplicar paginacion cursor-based segun RF-VIS-013.
4. Retornar una lista de pacientes con `patient_ref`, `pseudonym_id` y `care_link_id`.
5. Disparar RF-VIS-014 para registrar `AccessAudit` por cada paciente incluido en la respuesta.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| patients | array | Lista paginada de pacientes visibles |
| count | int | Cantidad de elementos en la pagina actual |
| next_cursor | string? | Cursor para la pagina siguiente |
| has_more | bool | Indica si existen mas resultados |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| VIS_010_UNAUTHORIZED | 401 | JWT invalido | {error: "VIS_010_UNAUTHORIZED"} |
| VIS_010_FORBIDDEN | 403 | JWT no corresponde a `role=professional` | {error: "VIS_010_FORBIDDEN"} |

## Casos especiales y variantes
- Profesional sin pacientes visibles: retorna `patients=[]`, `count=0`.
- `CareLink` con `can_view_data=false` o `status!=active`: se excluye silenciosamente.
- El dashboard solo lista pacientes; resumen y alertas se consultan por endpoints separados (RF-VIS-011 y RF-VIS-012).

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | SELECT | professional_id, patient_id, status, can_view_data |
| AccessAudit | INSERT (delegado a RF-VIS-014) | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Profesional lista sus pacientes visibles
  Given un profesional con 3 CareLinks activos y can_view_data=true
  When GET /api/v1/professional/dashboard
  Then HTTP 200 con una lista de 3 pacientes

Scenario: Pacientes con can_view_data=false no aparecen
  Given un profesional con 1 CareLink activo y can_view_data=false
  When GET /api/v1/professional/dashboard
  Then HTTP 200 con patients=[]

Scenario: Usuario sin rol professional es rechazado
  Given JWT con role=patient
  When GET /api/v1/professional/dashboard
  Then HTTP 403 con VIS_010_FORBIDDEN
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIS-010-01 | Lista pacientes visibles del profesional | Positivo |
| TP-VIS-010-02 | Oculta pacientes con can_view_data=false | Negativo |
| TP-VIS-010-03 | Rechaza JWT sin role professional | Negativo |

## Sin ambiguedades pendientes
Ninguna.
