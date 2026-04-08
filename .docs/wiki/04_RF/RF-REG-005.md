# RF-REG-005: Idempotencia por patient_id + timestamp

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-005 |
| Modulo | REG |
| Actor | Sistema (guardia de idempotencia) |
| Flujo fuente | FL-REG-01 |
| Prioridad | Correctness |

## Precondiciones detalladas
- patient_id verificado y con consent activo.
- Score validado disponible.
- Ventana de idempotencia definida: 1 minuto desde created_at_utc del registro existente.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | Contexto autenticado | Requerido |
| score | int | Validado por RF-REG-002 | Rango -3..+3 |
| now_utc | timestamp | Sistema | Timestamp actual |

## Proceso (Happy Path)
1. Consultar MoodEntry WHERE patient_id = :patient_id AND safe_projection->>'mood_score' = :score AND created_at_utc >= now_utc - interval '1 minute'.
2. Si existe registro, retornar safe_projection del existente con HTTP 200.
3. Si no existe, continuar flujo de creacion.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| is_duplicate | bool | true si se encontro entrada en ventana |
| existing_entry | jsonb | safe_projection del registro existente (si aplica) |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| DUPLICATE_ENTRY | 200 | Registro identico dentro de 1 min | safe_projection del existente |

## Casos especiales y variantes
- Mismo score pero fuera de ventana de 1 min: se crea nuevo registro normalmente.
- Score diferente dentro del minuto: se crea nuevo registro (no hay idempotencia entre scores distintos).

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| MoodEntry | SELECT | patient_id, safe_projection, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Registro duplicado dentro de ventana retorna existente
  Given existe un MoodEntry con score=1 creado hace 30 segundos
  When se solicita crear MoodEntry con score=1 para el mismo patient
  Then se retorna 200 con safe_projection del registro existente

Scenario: Mismo score fuera de ventana crea nuevo registro
  Given existe un MoodEntry con score=1 creado hace 2 minutos
  When se solicita crear MoodEntry con score=1
  Then se retorna 201 con nuevo mood_entry_id
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-005-01 | Duplicado en ventana retorna 200 con existente | Positivo |
| TP-REG-005-02 | Fuera de ventana crea nuevo registro | Positivo |
| TP-REG-005-03 | Score diferente en ventana crea nuevo registro | Positivo |

## Sin ambiguedades pendientes
Ninguna.
