# RF-EXP-001: Generar CSV con headers estandarizados

## Execution Sheet
- Modulo: EXP
- Endpoint: GET /api/v1/export/csv?from=&to=
- Actor: Patient (autenticado via JWT)
- Prioridad PDP: Privacy > Correctness > Usability

## Precondiciones detalladas
- JWT valido con User.status=active y ConsentGrant.status=granted
- Global Query Filter activo: solo datos del paciente autenticado
- Datos cifrados deben descifrarse en memoria antes de escribirse al CSV (RF-EXP-002)
- Response usa streaming para no buffear dataset en memoria (RF-EXP-003)

## Inputs
| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| from | date (ISO8601) | Si | Inicio del rango de exportacion |
| to | date (ISO8601) | Si | Fin del rango |

## Proceso (Happy Path)
1. Extraer patient_id del JWT
2. Validar `from <= to`
3. Iniciar response HTTP con `Content-Type: text/csv` y `Content-Disposition: attachment; filename="bitacora-export.csv"`
4. Escribir header row: `date,mood_score,sleep_hours,physical_activity,social_activity,anxiety,irritability,medication_taken,medication_time`
5. Para cada dia en el rango (join MoodEntry + DailyCheckin por fecha):
   a. Descifrar campos necesarios de MoodEntry y DailyCheckin (RF-EXP-002)
   b. Escribir row via streaming (RF-EXP-003)
6. Finalizar stream

## Outputs
- `Content-Type: text/csv`
- `Content-Disposition: attachment; filename="bitacora-export.csv"`
- Cuerpo: CSV con headers estandarizados, una fila por dia

Ejemplo:
```
date,mood_score,sleep_hours,physical_activity,social_activity,anxiety,irritability,medication_taken,medication_time
2026-04-01,2,7.5,true,false,1,0,true,08:00
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| EXP_001_RANGE_INVALID | 400 | from > to o parametros ausentes |
| EXP_001_UNAUTHORIZED | 401 | JWT invalido |
| EXP_001_DECRYPT_FAILED | 500 | Fallo al descifrar (fail-closed, RF-EXP-002) |

## Casos especiales y variantes
- Dia con MoodEntry pero sin DailyCheckin: columnas de checkin quedan vacias (no omitir fila)
- Dia con DailyCheckin pero sin MoodEntry: columna mood_score vacia
- Rango sin datos: solo se retorna la fila de headers
- Campos nulos en safe_projection: se escriben como cadena vacia en CSV

## Impacto en modelo de datos
- Solo lectura sobre `mood_entries` y `daily_checkins`
- Descifrado en memoria, no persiste datos descifrados

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Exportacion con datos completos
  Given paciente con MoodEntry y DailyCheckin el 2026-04-01
  When GET /api/v1/export/csv?from=2026-04-01&to=2026-04-01
  Then HTTP 200 con Content-Type=text/csv
  And primera fila contiene los 9 headers estandarizados
  And segunda fila tiene los datos del dia

Scenario: Rango sin datos retorna solo headers
  Given paciente sin entradas en el rango
  When GET /api/v1/export/csv?from=2026-01-01&to=2026-01-01
  Then HTTP 200 con solo la fila de headers
```

## Trazabilidad de tests
- UT: EXP001_Headers_Standardized
- UT: EXP001_MissingCheckin_EmptyColumns
- IT: EXP001_Streaming_NoBulkBuffer
- IT: EXP001_DecryptFail_Returns500

## Sin ambiguedades pendientes
- El orden de las 9 columnas es fijo e inmutable
- `medication_time` se exporta en formato HH:mm UTC
- Caracteres especiales en valores: escapar con comillas dobles segun RFC 4180
