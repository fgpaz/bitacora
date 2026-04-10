# RF-VIS-011: Resumen de humor ultimos 7 dias por paciente

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIS-011 |
| Modulo | VIS |
| Actor | Professional (API) |
| Flujo fuente | FL-VIS-02 |
| Prioridad | Security |

## Precondiciones detalladas
- JWT valido con `role=professional` y `User.status=active`.
- Existe `CareLink` activo con `can_view_data=true` entre el profesional y el paciente objetivo.
- Solo se accede a `safe_projection` de `MoodEntry`.
- `AccessAudit` debe escribirse antes de retornar datos; si falla, aplica fail-closed.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_ref | string | Path param | Referencia opaca valida del paciente |

## Proceso (Happy Path)
1. Resolver `patient_id` desde `patient_ref`.
2. Verificar `CareLink` activo con `can_view_data=true`.
3. INSERT `AccessAudit` con `action_type='read'`, `resource_type='mood_entry'`; si falla, abortar.
4. Query `MoodEntry.safe_projection` de los ultimos 7 dias.
5. Calcular `avg_mood`, `min_mood`, `max_mood`, `trend` y `entry_count`.
6. Retornar resumen agregado.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| patient_ref | string | Referencia opaca del paciente |
| period_days | int | Cantidad de dias consultados |
| avg_mood | decimal? | Promedio del periodo |
| min_mood | int? | Minimo del periodo |
| max_mood | int? | Maximo del periodo |
| trend | int? | Ultimo score - primer score del periodo |
| entry_count | int | Cantidad de entradas consideradas |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| VIS_011_PATIENT_NOT_FOUND | 404 | `patient_ref` invalido | {error: "VIS_011_PATIENT_NOT_FOUND"} |
| VIS_011_NOT_LINKED | 403 | No existe `CareLink` valido | {error: "VIS_011_NOT_LINKED"} |
| VIS_011_AUDIT_FAILED | 500 | Fallo al insertar `AccessAudit` | {error: "VIS_011_AUDIT_FAILED"} |

## Casos especiales y variantes
- Paciente sin entradas en 7 dias: retorna `avg_mood=null`, `min_mood=null`, `max_mood=null`, `trend=null`, `entry_count=0`.
- Si existe un solo registro en la ventana, `trend=0`.
- Nunca se descifra `encrypted_payload` para este endpoint.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| MoodEntry | SELECT | patient_id, safe_projection, created_at_utc |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Profesional obtiene resumen de paciente vinculado
  Given CareLink activo con can_view_data=true
  And el paciente tiene entradas en los ultimos 7 dias
  When GET /api/v1/professional/patients/PAT-0042/summary
  Then HTTP 200 con avg, min, max y trend calculados

Scenario: Profesional sin CareLink valido es rechazado
  Given CareLink con can_view_data=false
  When GET /api/v1/professional/patients/PAT-0042/summary
  Then HTTP 403 con VIS_011_NOT_LINKED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIS-011-01 | Calcula resumen de 7 dias con safe_projection | Positivo |
| TP-VIS-011-02 | Retorna nulls si no hay entradas en la ventana | Positivo |
| TP-VIS-011-03 | Falla cerrado si la auditoria no puede persistirse | Negativo |

## Sin ambiguedades pendientes
Ninguna.
