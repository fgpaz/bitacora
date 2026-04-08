# RF-VIS-012: Calcular alertas basicas

## Execution Sheet
- Modulo: VIS
- Endpoint: GET /api/v1/professional/patients/{patient_ref}/alerts
- Actor: Professional (autenticado via JWT)
- Prioridad PDP: Security > Correctness > Usability

## Precondiciones detalladas
- JWT valido con rol=professional y User.status=active
- Existe CareLink: professional_id=actor, patient_id=target, status=active, can_view_data=true
- AccessAudit requerido antes de retornar datos (RF-SEC-001, RF-SEC-003)
- Solo se usa `safe_projection.mood_score` para el calculo

## Inputs
| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| patient_ref | string (path) | Si | Referencia opaca del paciente |

## Proceso (Happy Path)
1. Resolver patient_id desde patient_ref
2. Verificar CareLink activo con can_view_data=true
3. Insertar AccessAudit — si falla, abortar (RF-SEC-003)
4. Query ultimas 30 entradas ordenadas por created_at DESC
5. Detectar rachas: grupos de dias consecutivos donde mood_score <= -2
6. Si racha >= 3 dias consecutivos, generar alerta de tipo `LOW_MOOD_STREAK`
7. Retornar lista de alertas activas

## Outputs
```json
{
  "patient_ref": "PAT-0042",
  "alerts": [
    {
      "type": "LOW_MOOD_STREAK",
      "severity": "medium",
      "consecutive_days": 4,
      "start_date": "2026-04-03",
      "last_date": "2026-04-06"
    }
  ],
  "alert_count": 1
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| VIS_012_NOT_LINKED | 403 | No existe CareLink valido |
| VIS_012_AUDIT_FAILED | 500 | Fallo al insertar AccessAudit (fail-closed) |
| VIS_012_PATIENT_NOT_FOUND | 404 | patient_ref invalido |

## Casos especiales y variantes
- Sin rachas detectadas: retorna `alerts=[]`
- Dias no consecutivos aunque con mood_score <= -2: no generan alerta
- Multiple rachas en el periodo: se reportan todas
- Racha activa (llega hasta hoy): `last_date = today`

## Impacto en modelo de datos
- Solo lectura sobre `mood_entries.safe_projection`
- INSERT en `access_audits`
- No persiste alertas: calculo on-demand

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Racha de 3 dias con mood <= -2 genera alerta
  Given un paciente con mood_score=-3,-2,-3 en dias consecutivos
  When GET .../alerts
  Then alerta LOW_MOOD_STREAK con consecutive_days=3

Scenario: Dos dias con mood <= -2 no generan alerta
  Given mood_score=-2,-2 en dias consecutivos
  When GET .../alerts
  Then alerts=[]

Scenario: Dias no consecutivos no generan alerta
  Given mood_score=-3 el lunes y -3 el miercoles (martes=0)
  When GET .../alerts
  Then alerts=[]
```

## Trazabilidad de tests
- UT: VIS012_ConsecutiveDays_Detection
- UT: VIS012_NonConsecutive_NoAlert
- IT: VIS012_AuditFail_Returns500

## Sin ambiguedades pendientes
- Umbral: mood_score <= -2 (incluye -2)
- "Consecutivo" se calcula por `checkin_date`, no por `created_at` datetime
- Ventana de deteccion: 30 dias hacia atras desde today
