# RF-VIS-013: Paginacion de lista de pacientes

## Execution Sheet
- Modulo: VIS
- Endpoint: GET /api/v1/professional/dashboard (parametros de paginacion)
- Actor: Professional (autenticado via JWT)
- Prioridad PDP: Correctness > Usability

## Precondiciones detalladas
- JWT valido con rol=professional
- Paginacion aplica sobre la lista de CareLinks activos del profesional
- Cursor es opaco (base64 de care_link_id del ultimo elemento)

## Inputs
| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| cursor | string | No | Cursor opaco de pagina anterior |
| page_size | int | No | Max 20, default 20 |

## Proceso (Happy Path)
1. Extraer professional_id del JWT
2. Si `cursor` ausente, iniciar desde el primer registro
3. Si `cursor` presente, decodificar para obtener `last_care_link_id`
4. Query: `WHERE professional_id = @id AND status='active' AND can_view_data=true AND care_link_id > @last_id ORDER BY care_link_id LIMIT @page_size + 1`
5. Si hay `page_size + 1` resultados, generar next_cursor con el id del ultimo incluido
6. Retornar pagina actual

## Outputs
```json
{
  "patients": [...],
  "count": 20,
  "next_cursor": "eyJpZCI6InV1aWQifQ==",
  "has_more": true
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| VIS_013_CURSOR_INVALID | 400 | Cursor malformado |
| VIS_013_PAGE_SIZE_EXCEEDED | 400 | page_size > 20 |

## Casos especiales y variantes
- Profesional con 0 pacientes: `patients=[], has_more=false`
- Cursor de otro profesional: devuelve pagina vacia por filtro de professional_id
- Ultima pagina: `has_more=false`, `next_cursor=null`

## Impacto en modelo de datos
- Solo lectura sobre `care_links`
- Index recomendado: `(professional_id, status, can_view_data, care_link_id)`

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Profesional con 25 pacientes pagina correctamente
  Given 25 CareLinks activos con can_view_data=true
  When GET /api/v1/professional/dashboard
  Then HTTP 200 con 20 pacientes y has_more=true

Scenario: Segunda pagina trae los 5 restantes
  Given cursor de primera pagina
  When GET con cursor=<cursor>
  Then HTTP 200 con 5 pacientes y has_more=false

Scenario: page_size=21 es rechazado
  When GET con page_size=21
  Then HTTP 400 con codigo VIS_013_PAGE_SIZE_EXCEEDED
```

## Trazabilidad de tests
- UT: VIS013_CursorGeneration_Roundtrip
- IT: VIS013_Pagination_25Patients_TwoPages
- IT: VIS013_CrossProfessionalCursor_Empty

## Sin ambiguedades pendientes
- Limite de 20 pacientes por pagina es fijo, no configurable por cliente
- Ordenamiento por care_link_id garantiza estabilidad de cursor
