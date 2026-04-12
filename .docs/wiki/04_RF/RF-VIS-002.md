# RF-VIS-002: Query daily_checkins por rango de fechas

## Execution Sheet
- Modulo: VIS
- Endpoint: GET /api/v1/daily-checkins?from=&to=
- Actor: Patient (autenticado via JWT)
- Prioridad PDP: Privacy > Correctness > Usability
- Estado: **Diferido — endpoint dedicado no implementado.** Los datos de daily checkin se incluyen en `GET /api/v1/visualizacion/timeline` (RF-VIS-001).

## Precondiciones detalladas
- JWT valido con supabase_user_id resolvible a User.status=active
- User tiene ConsentGrant.status=granted
- Global Query Filter activo: todas las queries filtran por patient_id del contexto
- Solo existe un DailyCheckin por paciente por dia

## Inputs
| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| from | date (ISO8601) | No | Inicio del rango. Default: today - 90 days |
| to | date (ISO8601) | No | Fin del rango. Default: today |

## Proceso (Happy Path)
1. Extraer patient_id del JWT via contexto autenticado
2. Si `from` o `to` ausentes, aplicar defaults (90 dias hacia atras)
3. Validar que `from <= to` y que el rango no exceda 90 dias
4. Si rango > 90 dias, retornar 400 (ver RF-VIS-003 para paginacion)
5. Ejecutar query con Global Query Filter: `WHERE patient_id = @patientId AND checkin_date BETWEEN @from AND @to`
6. Proyectar solo `safe_projection` (sleep_hours, activity flags)
7. Retornar array ordenado por checkin_date ASC

## Outputs
```json
{
  "data": [
    {
      "checkin_date": "2026-04-01",
      "sleep_hours": 7.5,
      "physical_activity": true,
      "social_activity": false
    }
  ],
  "count": 1
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| VIS_002_RANGE_INVALID | 400 | from > to |
| VIS_002_RANGE_TOO_LARGE | 400 | Rango > 90 dias, usar paginacion |
| VIS_002_UNAUTHORIZED | 401 | JWT invalido o expirado |

## Casos especiales y variantes
- Rango sin datos: retorna `{ "data": [], "count": 0 }` (no 404)
- `from == to`: retorna checkin del dia exacto si existe
- Dias sin checkin no generan entradas en el array (no null-fill)

## Impacto en modelo de datos
- Solo lectura sobre `daily_checkins.safe_projection`
- Global Query Filter garantiza aislamiento entre pacientes

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente consulta sus checkins en rango valido
  Given un paciente autenticado con checkins en los ultimos 30 dias
  When GET /api/v1/daily-checkins?from=2026-03-08&to=2026-04-07
  Then HTTP 200 con array de safe_projection
  And cada checkin pertenece al paciente autenticado

Scenario: Dia sin checkin no aparece en resultados
  Given un paciente sin checkin en 2026-04-01
  When GET /api/v1/daily-checkins?from=2026-04-01&to=2026-04-01
  Then HTTP 200 con data=[]
```

## Trazabilidad de tests
- UT: VIS002_ValidRange_ReturnsSafeProjection
- UT: VIS002_MissingDay_NotIncluded
- IT: VIS002_GlobalQueryFilter_IsolatesPatients
- IT: VIS002_RangeExceeds90Days_Returns400

## Sin ambiguedades pendientes
- `checkin_date` es date (sin hora), no datetime
- `safe_projection` no incluye campos cifrados del checkin
