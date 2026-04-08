# RF-REG-003: Cifrar payload y generar safe_projection (MoodEntry)

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-003 |
| Modulo | REG |
| Actor | Sistema (servicio de cifrado interno) |
| Flujo fuente | FL-REG-01 |
| Prioridad | Security |

## Precondiciones detalladas
- Clave AES activa disponible en KMS para el key_version vigente (politica: fail-closed).
- Score validado disponible (RF-REG-002).
- patient_id resuelto y verificado.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| payload_raw | json | Score + metadatos del registro | Objeto valido, no nulo |
| key_version | string | KMS (version activa) | Clave existente y activa |

## Proceso (Happy Path)
1. Obtener clave AES activa del KMS por key_version.
2. Serializar payload_raw a JSON string.
3. Cifrar con AES (modo GCM recomendado). Almacenar como encrypted_payload.
4. Extraer campos operacionales: mood_score, channel, created_at.
5. Construir safe_projection jsonb: {mood_score, channel, created_at}.
6. Retornar encrypted_payload, safe_projection, key_version, encrypted_at.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| encrypted_payload | bytea | Payload AES cifrado |
| safe_projection | jsonb | {mood_score, channel, created_at} |
| key_version | string | Version de clave usada |
| encrypted_at | timestamp | Momento del cifrado UTC |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| ENCRYPTION_FAILURE | 500 | Clave AES no disponible o error KMS | {error: "ENCRYPTION_FAILURE"} — fail-closed |
| KEY_VERSION_INVALID | 500 | key_version no existe en KMS | {error: "KEY_VERSION_INVALID"} |

## Casos especiales y variantes
- Fail-closed: si la clave no esta disponible, la operacion completa se aborta. No se almacena nada.
- safe_projection nunca contiene datos sensibles clinicos mas alla de mood_score.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| MoodEntry | Prepara campos para INSERT | encrypted_payload, safe_projection, key_version, encrypted_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Cifrado exitoso genera safe_projection
  Given la clave AES esta disponible en KMS
  And el payload tiene score = -1
  When se ejecuta el cifrado
  Then safe_projection.mood_score = -1
  And encrypted_payload no es nulo

Scenario: KMS no disponible aborta operacion
  Given la clave AES no esta disponible
  When se intenta cifrar el payload
  Then se lanza ENCRYPTION_FAILURE con HTTP 500
  And no se escribe ningun registro
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-003-01 | Cifrado exitoso con KMS activo | Positivo |
| TP-REG-003-02 | KMS no disponible retorna 500 fail-closed | Negativo |

## Sin ambiguedades pendientes
Ninguna.
