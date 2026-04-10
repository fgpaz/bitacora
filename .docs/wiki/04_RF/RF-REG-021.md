# RF-REG-021: Validar rangos de factores diarios

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-021 |
| Modulo | REG |
| Actor | Sistema (regla de validacion interna) |
| Flujo fuente | FL-REG-03 |
| Prioridad | Correctness |

## Precondiciones detalladas
- Invocado antes de cualquier operacion sobre DailyCheckin.
- Todos los campos del checkin presentes en el input.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| sleep_hours | float | Request | 0.0 <= sleep_hours <= 24.0 |
| physical_activity | bool | Request | true o false, no nulo |
| social_activity | bool | Request | true o false, no nulo |
| anxiety | bool | Request | true o false, no nulo |
| irritability | bool | Request | true o false, no nulo |
| medication_taken | bool | Request | true o false, no nulo |
| medication_time | time | Request | HH:MM formato 24h; requerido si medication_taken=true y luego normalizado a 15 minutos |

## Proceso (Happy Path)
1. Validar sleep_hours: float, rango [0, 24].
2. Validar campos booleanos: exactamente true o false.
3. Si medication_taken=true, validar medication_time presente y formato HH:MM valido.
4. Normalizar medication_time al bloque aproximado de 15 minutos mas cercano.
5. Si medication_taken=false, ignorar medication_time.
6. Retornar campos validados al flujo llamador.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| campos_validados | object | Todos los campos validados y normalizados |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| INVALID_SLEEP_HOURS | 422 | sleep_hours fuera de [0, 24] | {error: "INVALID_SLEEP_HOURS", range: "0-24"} |
| INVALID_BOOLEAN | 422 | Campo booleano nulo o tipo incorrecto | {error: "INVALID_BOOLEAN", field: "..."} |
| MISSING_MEDICATION_TIME | 422 | medication_taken=true pero sin medication_time | {error: "MISSING_MEDICATION_TIME"} |
| INVALID_TIME_FORMAT | 422 | medication_time no es HH:MM valido | {error: "INVALID_TIME_FORMAT"} |

## Casos especiales y variantes
- sleep_hours = 0 y sleep_hours = 24 son valores de borde validos.
- medication_time con medication_taken=false: ignorar silenciosamente.
- La validacion no busca exactitud clinica al minuto; solo consistencia de formato para almacenar un horario aproximado.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| Ninguna | — | Regla de validacion pura, sin escritura |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Todos los campos validos pasan validacion
  Given sleep_hours=7.5, todos los booleanos presentes, medication_taken=false
  When se ejecuta la validacion
  Then todos los campos son aceptados

Scenario: sleep_hours fuera de rango es rechazado
  Given sleep_hours = 25
  When se ejecuta la validacion
  Then se lanza INVALID_SLEEP_HOURS con HTTP 422
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-021-01 | Campos validos pasan todos | Positivo |
| TP-REG-021-02 | sleep_hours=25 retorna 422 | Negativo |
| TP-REG-021-03 | medication_taken=true sin time retorna 422 | Negativo |

## Sin ambiguedades pendientes
Ninguna.
