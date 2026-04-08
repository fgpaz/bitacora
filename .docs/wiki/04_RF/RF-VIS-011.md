# RF-VIS-011: Resumen humor ultimos 7 dias por paciente

## Execution Sheet
- Modulo: VIS
- Endpoint: GET /api/v1/professional/patients/{patient_ref}/summary
- Actor: Professional (autenticado via JWT)
- Prioridad PDP: Security > Privacy > Correctness

## Precondiciones detalladas
- JWT valido con rol=professional y User.status=active
- Existe CareLink: professional_id=actor, patient_id=target, status=active, can_view_data=true
- Solo se accede a `safe_projection` de mood_entries (no datos cifrados)
- AccessAudit requerido antes de retornar datos

## Inputs
| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| patient_ref | string (path) | Si | Referencia opaca del paciente |

## Proceso (Happy Path)
1. Resolver patient_id desde patient_ref
2. Verificar CareLink activo con can_view_data=true para el profesional
3. Insertar AccessAudit (RF-SEC-001) — si falla, abortar (RF-SEC-003)
4. Query mood_entries: `WHERE patient_id = @id AND created_at >= now() - interval '7 days'`
5. Proyectar `safe_projection.mood_score`
6. Calcular: avg, min, max, trend (ultimo - primero del periodo)
7. Retornar resumen

## Outputs
```json
{
  "patient_ref": "PAT-0042",
  "period_days": 7,
  "avg_mood": 0.43,
  "min_mood": -2,
  "max_mood": 3,
  "trend": 1,
  "entry_count": 7
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| VIS_011_NOT_LINKED | 403 | No existe CareLink valido |
| VIS_011_AUDIT_FAILED | 500 | Fallo al insertar AccessAudit (fail-closed) |
| VIS_011_PATIENT_NOT_FOUND | 404 | patient_ref invalido |

## Casos especiales y variantes
- Paciente sin entradas en 7 dias: retorna avg/min/max=null, entry_count=0, trend=null
- Un solo entry: trend=0
- `mood_score` siempre de safe_projection, nunca de payload cifrado

## Impacto en modelo de datos
- Solo lectura sobre `mood_entries.safe_projection`
- INSERT en `access_audits` (append-only)

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Profesional obtiene resumen de paciente vinculado
  Given CareLink activo con can_view_data=true
  And el paciente tiene 7 entradas en los ultimos 7 dias
  When GET /api/v1/professional/patients/PAT-0042/summary
  Then HTTP 200 con avg, min, max, trend calculados
  And se inserta AccessAudit para el acceso

Scenario: Profesional sin CareLink valido
  Given CareLink con can_view_data=false
  When GET .../summary
  Then HTTP 403 con codigo VIS_011_NOT_LINKED
```

## Trazabilidad de tests
- UT: VIS011_AggregateCalc_AvgMinMaxTrend
- UT: VIS011_EmptyPeriod_ReturnsNulls
- IT: VIS011_NoCareLink_Returns403
- IT: VIS011_AuditFail_Returns500_NoData

## Sin ambiguedades pendientes
- `trend` = mood_score del ultimo entry - mood_score del primer entry del periodo
- Promedio se redondea a 2 decimales
