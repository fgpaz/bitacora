# RF-REG-023: Cifrar payload y generar safe_projection (DailyCheckin)

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-023 |
| Modulo | REG |
| Actor | Sistema (servicio de cifrado interno) |
| Flujo fuente | FL-REG-03 |
| Prioridad | Security |

## Precondiciones detalladas
- Clave AES activa disponible en KMS para el key_version vigente (politica: fail-closed).
- Campos de DailyCheckin validados por RF-REG-021.
- patient_id resuelto y verificado.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| payload_raw | json | Campos validados del checkin | Objeto valido, no nulo |
| key_version | string | KMS (version activa) | Clave existente y activa |

## Proceso (Happy Path)
1. Obtener clave AES activa del KMS por key_version.
2. Serializar payload_raw a JSON string.
3. Cifrar con AES (modo GCM). Almacenar como encrypted_payload.
4. Mantener `medication_time` aproximado solo dentro de `encrypted_payload`.
5. Extraer campos operacionales: sleep_hours, has_physical, has_social, has_anxiety, has_irritability, has_medication.
6. Construir safe_projection jsonb con esos seis campos.
7. Retornar encrypted_payload, safe_projection, key_version.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| encrypted_payload | bytea | Payload AES cifrado |
| safe_projection | jsonb | {sleep_hours, has_physical, has_social, has_anxiety, has_irritability, has_medication} |
| key_version | string | Version de clave usada |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| ENCRYPTION_FAILURE | 500 | Clave AES no disponible o error KMS | {error: "ENCRYPTION_FAILURE"} — fail-closed |
| KEY_VERSION_INVALID | 500 | key_version no existe en KMS | {error: "KEY_VERSION_INVALID"} |

## Casos especiales y variantes
- Fail-closed: si la clave no esta disponible, la operacion completa se aborta. No se almacena nada.
- safe_projection no incluye medication_time (dato sensible, solo en encrypted_payload, aun cuando sea aproximado).
- Patron identico a RF-REG-003 pero para entidad DailyCheckin con campos diferentes.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| DailyCheckin | Prepara campos para UPSERT | encrypted_payload, safe_projection, key_version |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Cifrado exitoso genera safe_projection de checkin
  Given la clave AES esta disponible en KMS
  And el payload tiene sleep_hours=7 y medication_taken=true
  When se ejecuta el cifrado
  Then safe_projection.sleep_hours = 7
  And safe_projection.has_medication = true
  And medication_time no aparece en safe_projection

Scenario: KMS no disponible aborta operacion
  Given la clave AES no esta disponible
  When se intenta cifrar el payload del checkin
  Then se lanza ENCRYPTION_FAILURE con HTTP 500
  And no se escribe ningun registro
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-023-01 | Cifrado exitoso con KMS activo | Positivo |
| TP-REG-023-02 | KMS no disponible retorna 500 fail-closed | Negativo |
| TP-REG-023-03 | medication_time ausente de safe_projection | Positivo |

## Sin ambiguedades pendientes
Ninguna.
