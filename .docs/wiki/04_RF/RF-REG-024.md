# RF-REG-024: Registrar audit de creacion/actualizacion de DailyCheckin

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-024 |
| Modulo | REG |
| Actor | Sistema |
| Flujo fuente | FL-REG-03 |
| Prioridad | Security |

## Precondiciones detalladas
- RF-REG-020 y RF-REG-022 ya determinaron si la operacion fue `create` o `update`.
- Existe `daily_checkin_id` objetivo.
- El `trace_id` del request esta disponible.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | JWT / contexto interno | Existente |
| daily_checkin_id | uuid | Resultado del UPSERT | Existente |
| operation_kind | string | Contexto interno | `create` o `update` |
| trace_id | uuid | Contexto del request | Requerido |

## Proceso (Happy Path)
1. Resolver `action_type` segun `operation_kind`.
2. INSERT `AccessAudit` append-only con `resource_type='daily_checkin'`, `resource_id=daily_checkin_id`, `patient_id`, `trace_id`.
3. Si la escritura falla, aplicar fail-closed y abortar la respuesta final del endpoint.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| audit_written | bool | `true` si el registro quedo persistido |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| AUDIT_WRITE_FAILED | 500 | Fallo al persistir `AccessAudit` | {error: "AUDIT_WRITE_FAILED"} |
| INVALID_OPERATION_KIND | 500 | `operation_kind` fuera del catalogo | {error: "INVALID_OPERATION_KIND"} |

## Casos especiales y variantes
- `create` y `update` comparten el mismo `resource_type`.
- No existe modo best-effort; si falla la auditoria, la operacion completa falla cerrada.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Create genera audit
  Given se creo un DailyCheckin nuevo
  When se ejecuta RF-REG-024 con operation_kind="create"
  Then se inserta un AccessAudit con action_type="create"

Scenario: Falla de auditoria bloquea la operacion
  Given el DailyCheckin fue preparado para persistirse
  And la escritura de AccessAudit falla
  When se ejecuta RF-REG-024
  Then se retorna 500 AUDIT_WRITE_FAILED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-024-01 | Inserta audit en alta de DailyCheckin | Positivo |
| TP-REG-024-02 | Inserta audit en actualizacion de DailyCheckin | Positivo |
| TP-REG-024-03 | Falla cerrado si no puede auditar | Negativo |

## Sin ambiguedades pendientes
Ninguna.
