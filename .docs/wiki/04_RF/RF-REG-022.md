# RF-REG-022: UPSERT DailyCheckin (uno por dia)

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-022 |
| Modulo | REG |
| Actor | Sistema (logica de persistencia) |
| Flujo fuente | FL-REG-03 |
| Prioridad | Correctness |

## Precondiciones detalladas
- Campos validados por RF-REG-021.
- Payload cifrado y safe_projection generada por RF-REG-023.
- Constraint UNIQUE(patient_id, checkin_date) definido en DB.
- checkin_date = fecha actual en UTC.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | Contexto autenticado | Requerido |
| checkin_date | date | Sistema (UTC) | Fecha actual |
| encrypted_payload | bytea | RF-REG-023 | No nulo |
| safe_projection | jsonb | RF-REG-023 | No nulo |
| key_version | string | KMS | No nulo |

## Proceso (Happy Path)
1. Intentar INSERT DailyCheckin con (patient_id, checkin_date).
2. Si viola UNIQUE constraint: ejecutar UPDATE de encrypted_payload, safe_projection, key_version.
3. Retornar daily_checkin_id y codigo HTTP: 201 si INSERT, 200 si UPDATE.
4. INSERT AccessAudit con operacion='CHECKIN_UPSERT'.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| daily_checkin_id | uuid | ID del registro |
| operation | string | 'created' o 'updated' |
| checkin_date | date | Fecha del checkin |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| UPSERT_FAILED | 500 | Error de DB no esperado | {error: "UPSERT_FAILED"} |

## Casos especiales y variantes
- Solo se permite un DailyCheckin por (patient_id, checkin_date). El UPSERT garantiza esta restriccion.
- Actualizacion multiple el mismo dia: cada update sobreescribe el anterior (historial no conservado en DailyCheckin).

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| DailyCheckin | INSERT o UPDATE | daily_checkin_id, patient_id, checkin_date, encrypted_payload, safe_projection, key_version |
| AccessAudit | INSERT | trace_id, patient_id, operacion, created_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Primer checkin del dia se inserta
  Given no existe DailyCheckin para patient_id en checkin_date
  When se ejecuta UPSERT
  Then se crea nuevo registro y retorna operation='created'

Scenario: Segundo checkin del dia actualiza el existente
  Given ya existe DailyCheckin para patient_id en checkin_date
  When se ejecuta UPSERT con nuevos valores
  Then se actualiza el registro y retorna operation='updated'
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-022-01 | INSERT en primer checkin del dia | Positivo |
| TP-REG-022-02 | UPDATE en checkin repetido del dia | Positivo |
| TP-REG-022-03 | Error de DB retorna 500 | Negativo |

## Sin ambiguedades pendientes
Ninguna.
