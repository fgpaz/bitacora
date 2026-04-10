# RF-REG-020: Crear DailyCheckin con campos validados

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-020 |
| Modulo | REG |
| Actor | Patient (API) |
| Flujo fuente | FL-REG-03 |
| Prioridad | Correctness |

## Precondiciones detalladas
- ConsentGrant.status = 'granted' para el patient_id (RF-REG-004).
- Clave AES activa disponible en KMS (RF-REG-023).
- Fecha del dia disponible en UTC.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| sleep_hours | float | Request body | 0-24, RF-REG-021 |
| physical_activity | bool | Request body | Requerido |
| social_activity | bool | Request body | Requerido |
| anxiety | bool | Request body | Requerido |
| irritability | bool | Request body | Requerido |
| medication_taken | bool | Request body | Requerido |
| medication_time | time | Request body | Requerido si medication_taken=true; horario aproximado normalizado por RF-REG-025 |

## Proceso (Happy Path)
1. Verificar consent activo (RF-REG-004).
2. Validar todos los campos (RF-REG-021).
3. Validar y normalizar `medication_time` aproximado via RF-REG-025.
4. Ejecutar UPSERT via RF-REG-022.
5. Cifrar payload y generar safe_projection (RF-REG-023).
6. Registrar AccessAudit via RF-REG-024.
7. Retornar 201 (creacion) o 200 (actualizacion) con safe_projection.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| daily_checkin_id | uuid | ID del registro |
| safe_projection | jsonb | {sleep_hours, has_physical, has_social, has_anxiety, has_irritability, has_medication} |
| checkin_date | date | Fecha del checkin |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CONSENT_REQUIRED | 403 | Sin consent activo | {error: "CONSENT_REQUIRED"} |
| VALIDATION_ERROR | 422 | Campos fuera de rango | {error: "VALIDATION_ERROR", fields: [...]} |
| ENCRYPTION_FAILURE | 500 | Fallo de KMS | {error: "ENCRYPTION_FAILURE"} |

## Casos especiales y variantes
- Si ya existe DailyCheckin del dia: actualizar via RF-REG-022 (no error).
- `medication_time` representa una hora aproximada y se normaliza a bloques de 15 minutos.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| DailyCheckin | INSERT o UPDATE | Todos los campos |
| AccessAudit | INSERT (delegado a RF-REG-024) | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Crear DailyCheckin con datos validos
  Given patient con consent activo
  And no existe DailyCheckin para hoy
  When POST /api/v1/daily-checkins con campos validos
  Then se retorna 201 con safe_projection

Scenario: Actualizar DailyCheckin existente del dia
  Given ya existe DailyCheckin para hoy
  When POST /api/v1/daily-checkins con nuevos valores
  Then se retorna 200 con safe_projection actualizado
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-020-01 | Creacion exitosa retorna 201 | Positivo |
| TP-REG-020-02 | Actualizacion del dia retorna 200 | Positivo |
| TP-REG-020-03 | Sin consent retorna 403 | Negativo |

## Sin ambiguedades pendientes
Ninguna.
