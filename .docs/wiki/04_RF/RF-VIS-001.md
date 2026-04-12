# RF-VIS-001: Query mood_entries por rango de fechas

## Execution Sheet
- Modulo: VIS
- Endpoint: GET /api/v1/visualizacion/timeline?from=&to=
- Actor: Patient (autenticado via JWT)
- Prioridad PDP: Privacy > Correctness > Usability
- Estado: **Implementado** — endpoint combinado que retorna mood entries y daily checkins en una sola estructura `days`.

## Precondiciones detalladas
- JWT valido con supabase_user_id resolvible a User.status=active
- User tiene ConsentGrant.status=granted
- Global Query Filter activo: todas las queries filtran por patient_id del contexto

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
5. Ejecutar query con Global Query Filter: `WHERE patient_id = @patientId AND created_at BETWEEN @from AND @to`
6. Proyectar solo `safe_projection` (mood_score, channel, created_at)
7. Retornar array ordenado por created_at ASC

## Outputs
```json
{
  "days": [
    {
      "date": "2026-04-01",
      "mood_entry": {
        "mood_entry_id": "uuid",
        "score": 2,
        "created_at": "2026-04-01T10:00:00Z"
      },
      "daily_checkin": {
        "daily_checkin_id": "uuid",
        "date": "2026-04-01",
        "sleep_hours": 7.5,
        "physical_activity": true,
        "social_activity": false,
        "anxiety": true,
        "irritability": false,
        "medication_taken": true
      }
    }
  ]
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| VIS_001_RANGE_INVALID | 400 | from > to |
| VIS_001_RANGE_TOO_LARGE | 400 | Rango > 90 dias, usar paginacion |
| VIS_001_UNAUTHORIZED | 401 | JWT invalido o expirado |

## Casos especiales y variantes
- Rango sin datos: retorna `{ "data": [], "count": 0 }` (no 404)
- `from == to`: retorna entradas del dia exacto
- Rango > 90 dias: rechazado con VIS_001_RANGE_TOO_LARGE y hint hacia RF-VIS-003

## Impacto en modelo de datos
- Solo lectura sobre `mood_entries.safe_projection`
- Global Query Filter garantiza aislamiento entre pacientes

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente consulta sus entradas en rango valido
  Given un paciente autenticado con entradas en los ultimos 30 dias
  When GET /api/v1/mood-entries?from=2026-03-08&to=2026-04-07
  Then HTTP 200 con array de safe_projection
  And cada entrada pertenece al paciente autenticado

Scenario: Rango mayor a 90 dias es rechazado
  Given un paciente autenticado
  When GET /api/v1/mood-entries?from=2025-01-01&to=2026-04-07
  Then HTTP 400 con codigo VIS_001_RANGE_TOO_LARGE
```

## Trazabilidad de tests
- UT: VIS001_ValidRange_ReturnsSafeProjection
- UT: VIS001_DefaultRange_Uses90Days
- IT: VIS001_GlobalQueryFilter_IsolatesPatients
- IT: VIS001_RangeExceeds90Days_Returns400

## Sin ambiguedades pendientes
- `created_at` se interpreta en UTC en servidor, nunca en timezone del cliente
- `channel` en safe_projection es string libre, no enum validado en lectura
