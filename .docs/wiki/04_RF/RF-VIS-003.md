# RF-VIS-003: Paginacion para periodos largos

## Execution Sheet
- Modulo: VIS
- Endpoints: GET /api/v1/mood-entries, GET /api/v1/daily-checkins (con cursor)
- Actor: Patient (autenticado via JWT)
- Prioridad PDP: Correctness > Usability
- Estado: **Diferido — paginacion con cursor no implementada.**

## Precondiciones detalladas
- JWT valido con User.status=active
- El rango solicitado supera los 90 dias O el cliente solicita paginacion explicitamente
- Cursor provisto en requests subsecuentes es opaco (base64 de created_at + id)

## Inputs
| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| from | date (ISO8601) | Si | Inicio del rango |
| to | date (ISO8601) | Si | Fin del rango |
| cursor | string | No | Cursor opaco de pagina anterior |
| page_size | int | No | Max 100, default 100 |

## Proceso (Happy Path)
1. Extraer patient_id del JWT
2. Validar `from <= to`
3. Si `cursor` ausente, iniciar desde `from`
4. Si `cursor` presente, decodificar (base64) para obtener `(last_created_at, last_id)`
5. Ejecutar query: `WHERE patient_id = @id AND created_at >= @from AND created_at <= @to AND (created_at, id) > (@last_created_at, @last_id) ORDER BY created_at, id LIMIT @page_size + 1`
6. Si se obtienen `page_size + 1` registros, hay pagina siguiente; generar next_cursor
7. Retornar pagina actual sin el registro extra

## Outputs
```json
{
  "data": [...],
  "count": 100,
  "next_cursor": "eyJjcmVhdGVkX2F0IjoiMjAyNi0wMS0wMSIsImlkIjoiYWJjIn0=",
  "has_more": true
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| VIS_003_CURSOR_INVALID | 400 | Cursor malformado o expirado |
| VIS_003_PAGE_SIZE_EXCEEDED | 400 | page_size > 100 |
| VIS_003_RANGE_REQUIRED | 400 | from o to ausentes en modo paginado |

## Casos especiales y variantes
- Ultima pagina: `has_more=false`, `next_cursor=null`
- Cursor de otro paciente: rechazado por Global Query Filter (devuelve pagina vacia, no error)
- `page_size=0`: equivale a page_size=100

## Impacto en modelo de datos
- Solo lectura. Index recomendado: `(patient_id, created_at, id)`

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paginacion sobre rango largo
  Given un paciente con 250 mood_entries en el ultimo año
  When GET /api/v1/mood-entries?from=2025-04-07&to=2026-04-07
  Then HTTP 200 con 100 entradas y next_cursor != null

Scenario: Segunda pagina con cursor
  Given un cursor valido de la primera pagina
  When GET /api/v1/mood-entries?from=...&to=...&cursor=<cursor>
  Then HTTP 200 con las siguientes 100 entradas

Scenario: Cursor invalido
  When GET con cursor="invalido"
  Then HTTP 400 con codigo VIS_003_CURSOR_INVALID
```

## Trazabilidad de tests
- UT: VIS003_CursorEncoding_Roundtrip
- IT: VIS003_FullDataset_PaginatesCorrectly
- IT: VIS003_CrossPatientCursor_ReturnsEmpty

## Sin ambiguedades pendientes
- El cursor no expira por tiempo; expira si los datos subyacentes cambian (acceptable)
- `page_size` maximo es 100 sin excepciones
