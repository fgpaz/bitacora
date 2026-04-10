# RF-VIS-012: Calcular alertas basicas por paciente

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIS-012 |
| Modulo | VIS |
| Actor | Sistema (endpoint profesional) |
| Flujo fuente | FL-VIS-02 |
| Prioridad | Security |

## Precondiciones detalladas
- JWT valido con `role=professional` y `User.status=active`.
- Existe `CareLink` activo con `can_view_data=true`.
- `AccessAudit` debe persistirse antes de retornar datos; si falla, aplica fail-closed.
- El calculo usa solo `safe_projection.mood_score`.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_ref | string | Path param | Referencia opaca valida del paciente |

## Proceso (Happy Path)
1. Resolver `patient_id` desde `patient_ref`.
2. Verificar `CareLink` activo con `can_view_data=true`.
3. INSERT `AccessAudit` con `action_type='read'`, `resource_type='mood_entry'`; si falla, abortar.
4. Query ultimas 30 entradas de `MoodEntry.safe_projection`.
5. Detectar rachas de dias consecutivos con `mood_score <= -2`.
6. Generar alerta `LOW_MOOD_STREAK` si la racha es de 3 o mas dias.
7. Retornar lista de alertas activas.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| patient_ref | string | Referencia opaca del paciente |
| alerts | array | Alertas activas calculadas on-demand |
| alert_count | int | Cantidad de alertas retornadas |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| VIS_012_PATIENT_NOT_FOUND | 404 | `patient_ref` invalido | {error: "VIS_012_PATIENT_NOT_FOUND"} |
| VIS_012_NOT_LINKED | 403 | No existe `CareLink` valido | {error: "VIS_012_NOT_LINKED"} |
| VIS_012_AUDIT_FAILED | 500 | Fallo al insertar `AccessAudit` | {error: "VIS_012_AUDIT_FAILED"} |

## Casos especiales y variantes
- Sin rachas detectadas: retorna `alerts=[]`.
- Dias no consecutivos con score critico no generan alerta.
- Las alertas no se persisten; se calculan por demanda.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| MoodEntry | SELECT | patient_id, safe_projection, created_at_utc |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Racha de 3 dias con mood <= -2 genera alerta
  Given un paciente con mood_score=-3,-2,-3 en dias consecutivos
  When GET /api/v1/professional/patients/PAT-0042/alerts
  Then se retorna una alerta LOW_MOOD_STREAK

Scenario: Dias no consecutivos no generan alerta
  Given mood_score=-3 el lunes y -3 el miercoles
  When GET /api/v1/professional/patients/PAT-0042/alerts
  Then se retorna alerts=[]
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIS-012-01 | Detecta racha de bajo humor consecutiva | Positivo |
| TP-VIS-012-02 | No crea alertas con dias no consecutivos | Positivo |
| TP-VIS-012-03 | Falla cerrado si la auditoria no puede persistirse | Negativo |

## Sin ambiguedades pendientes
Ninguna.
