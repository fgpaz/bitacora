# RF-REG-001: Crear MoodEntry con score validado

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-001 |
| Modulo | REG |
| Actor | Patient (API / Telegram) |
| Flujo fuente | FL-REG-01 |
| Prioridad | Correctness |

## Precondiciones detalladas
- ConsentGrant.status = 'granted' para el patient_id (RF-REG-004)
- TelegramSession activa si canal es telegram (RF-REG-011)
- Clave AES activa disponible en KMS (RF-REG-003)

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| score | int | Request body / callback_data | Rango -3..+3 (RF-REG-002) |
| patient_id | uuid | JWT / TelegramSession | Existente y con consent |
| channel | string | Contexto de llamada | 'api' o 'telegram' |

## Proceso (Happy Path)
1. Verificar consent activo (RF-REG-004).
2. Validar score en rango -3..+3 (RF-REG-002).
3. Verificar ventana de idempotencia 1 min (RF-REG-005).
4. Cifrar payload completo y generar safe_projection (RF-REG-003).
5. INSERT MoodEntry con estado IMMUTABLE.
6. INSERT AccessAudit con `action_type='create'`, `resource_type='mood_entry'`, `resource_id=mood_entry_id`.
7. Retornar 201 con safe_projection.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| mood_entry_id | uuid | ID del registro creado |
| safe_projection | jsonb | {mood_score, channel, created_at} |
| created_at_utc | timestamp | Timestamp UTC de creacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CONSENT_REQUIRED | 403 | No hay consent activo | {error: "CONSENT_REQUIRED"} |
| INVALID_SCORE | 422 | score fuera de -3..+3 | {error: "INVALID_SCORE", field: "score"} |
| ENCRYPTION_FAILURE | 500 | Clave AES no disponible | {error: "ENCRYPTION_FAILURE"} |
| DUPLICATE_ENTRY | 200 | Idempotencia activada | safe_projection del registro existente |

## Casos especiales y variantes
- Canal telegram: channel='telegram', disparado desde RF-REG-012.
- Idempotencia: si existe entrada dentro de 1 min con mismo score, retorna existente (200).

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| MoodEntry | INSERT | mood_entry_id, patient_id, encrypted_payload, safe_projection, key_version, encrypted_at, created_at_utc |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Crear MoodEntry con score valido
  Given el patient tiene consent activo
  And el score es 2
  When POST /api/v1/mood-entries con {score: 2}
  Then se retorna 201 con safe_projection.mood_score = 2

Scenario: Rechazar score invalido
  Given el patient tiene consent activo
  When POST /api/v1/mood-entries con {score: 5}
  Then se retorna 422 con error INVALID_SCORE
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-001-01 | Score valido crea MoodEntry y AccessAudit | Positivo |
| TP-REG-001-02 | Score invalido retorna 422 | Negativo |
| TP-REG-001-03 | Sin consent retorna 403 | Negativo |

## Sin ambiguedades pendientes
Ninguna.
