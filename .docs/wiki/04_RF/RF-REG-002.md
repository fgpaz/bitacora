# RF-REG-002: Validar score en rango -3..+3

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-002 |
| Modulo | REG |
| Actor | Sistema (regla de validacion interna) |
| Flujo fuente | FL-REG-01 |
| Prioridad | Correctness |

## Precondiciones detalladas
- Invocado antes de cualquier operacion de creacion de MoodEntry.
- El valor de score debe estar presente en el input (no null, no string).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| score | int | Request body / callback_data | Requerido, no nulo, tipo entero |

## Proceso (Happy Path)
1. Recibir valor score.
2. Verificar que sea de tipo entero.
3. Verificar que score >= -3 y score <= +3.
4. Retornar score validado al flujo llamador.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| score | int | Score validado listo para procesamiento |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| INVALID_SCORE | 422 | score fuera de [-3, +3] | {error: "INVALID_SCORE", field: "score", range: "-3..+3"} |
| MISSING_SCORE | 422 | score ausente o nulo | {error: "MISSING_SCORE", field: "score"} |
| TYPE_ERROR | 422 | score no es entero | {error: "TYPE_ERROR", field: "score"} |

## Casos especiales y variantes
- score = 0 es valido (estado neutro).
- score = -3 y score = +3 son valores de borde validos (inclusivos).
- Strings numericos ("2") deben rechazarse con TYPE_ERROR.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| Ninguna | — | Regla de validacion pura, sin escritura |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Score en limite superior valido
  Given el score es 3
  When se ejecuta la validacion de rango
  Then el score es aceptado

Scenario: Score fuera de rango rechazado
  Given el score es 4
  When se ejecuta la validacion de rango
  Then se lanza INVALID_SCORE con HTTP 422
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-002-01 | Scores -3, 0, +3 son aceptados | Positivo |
| TP-REG-002-02 | Score 4 retorna 422 INVALID_SCORE | Negativo |
| TP-REG-002-03 | Score nulo retorna 422 MISSING_SCORE | Negativo |

## Sin ambiguedades pendientes
Ninguna.
