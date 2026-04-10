# RF-REG-025: Incluir medicacion con horario aproximado

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-025 |
| Modulo | REG |
| Actor | Patient (API) |
| Flujo fuente | FL-REG-03 |
| Prioridad | Correctness |

## Precondiciones detalladas
- Aplica al formulario de `DailyCheckin`.
- `medication_taken` forma parte del payload del checkin.
- El horario informado es aproximado y nunca se expone en `safe_projection`.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| medication_taken | bool | Request body | Requerido |
| medication_time | time | Request body | Requerido solo si `medication_taken=true`; se redondea a bloques de 15 minutos |

## Proceso (Happy Path)
1. Si `medication_taken=false`, ignorar `medication_time`.
2. Si `medication_taken=true`, exigir `medication_time`.
3. Normalizar `medication_time` al bloque aproximado de 15 minutos mas cercano.
4. Persistir `medication_taken` y `medication_time` normalizado dentro de `encrypted_payload`.
5. Mantener solo `has_medication` en `safe_projection`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| medication_taken | bool | Indica si hubo medicacion |
| medication_time | time? | Hora aproximada normalizada, solo en payload cifrado |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| MISSING_MEDICATION_TIME | 422 | `medication_taken=true` sin horario | {error: "MISSING_MEDICATION_TIME"} |
| INVALID_TIME_FORMAT | 422 | `medication_time` fuera de formato HH:mm | {error: "INVALID_TIME_FORMAT"} |

## Casos especiales y variantes
- Si `medication_taken=false`, cualquier `medication_time` recibido se ignora.
- La hora registrada es autodeclarada y aproximada; no pretende exactitud clinica al minuto.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| DailyCheckin | INSERT o UPDATE | encrypted_payload.medication_taken, encrypted_payload.medication_time |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente informa medicacion con horario aproximado
  Given medication_taken=true
  When POST /api/v1/daily-checkins con medication_time="08:07"
  Then el payload persiste medication_time aproximado a un bloque de 15 minutos

Scenario: Falta el horario cuando medication_taken=true
  Given medication_taken=true
  When POST /api/v1/daily-checkins sin medication_time
  Then se retorna 422 MISSING_MEDICATION_TIME
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-025-01 | Persiste hora aproximada de medicacion en payload cifrado | Positivo |
| TP-REG-025-02 | Ignora horario si medication_taken=false | Positivo |
| TP-REG-025-03 | 422 si falta horario con medication_taken=true | Negativo |

## Sin ambiguedades pendientes
Ninguna.
