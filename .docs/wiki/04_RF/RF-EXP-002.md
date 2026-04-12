# RF-EXP-002: Descifrar payloads por key_version

## Execution Sheet
- Modulo: EXP
- Trigger: Interno, invocado por RF-EXP-001 y RF-EXP-003 durante exportacion
- Actor: Sistema
- Prioridad PDP: Security > Correctness (fail-closed obligatorio)
- Estado: **Diferido — no invocado en la implementacion actual.** El endpoint `GET /api/v1/export/patient-summary` opera exclusivamente sobre `safe_projection` y no descifra `encrypted_payload`.

## Precondiciones detalladas
- Cada MoodEntry y DailyCheckin tiene un campo `key_version` que identifica la clave de cifrado usada
- El keystore (en memoria o vault) debe tener la clave correspondiente a cada `key_version`
- Si cualquier clave requerida no esta disponible, la operacion completa falla

## Inputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| encrypted_payload | bytes | Payload cifrado del registro |
| key_version | string | Identificador de la clave de cifrado |
| context | string | patient_id + record_id (AAD para AEAD) |

## Proceso (Happy Path)
1. Recibir `(encrypted_payload, key_version, context)`
2. Buscar clave en keystore por `key_version`
3. Si clave no encontrada → lanzar `KeyNotFoundException` → fail-closed (ver Errores)
4. Descifrar payload usando la clave (algoritmo: AES-256-GCM o equivalente AEAD)
5. Verificar integridad con AAD = context
6. Retornar payload descifrado como objeto en memoria
7. No persistir payload descifrado en ningun almacenamiento

## Outputs
- Objeto descifrado en memoria (no serializado a disco ni log)

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| EXP_002_KEY_NOT_FOUND | 500 | key_version no existe en keystore; abortar exportacion |
| EXP_002_DECRYPT_FAILED | 500 | Fallo de integridad o cifrado corrupto |
| EXP_002_INVALID_CONTEXT | 500 | AAD no coincide; posible tampering |

## Casos especiales y variantes
- Registro con key_version desconocida: fallo inmediato, no continuar con registros restantes
- Keystore vacio o no inicializado: error en startup, no en runtime
- Rotacion de claves: multiples key_versions pueden coexistir; cada registro usa la suya

## Impacto en modelo de datos
- Solo lectura sobre columnas cifradas de `mood_entries` y `daily_checkins`
- No genera ninguna escritura

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Descifrado exitoso con key_version conocida
  Given un registro con key_version="v2" y keystore tiene "v2"
  When se invoca el descifrador
  Then retorna payload descifrado en memoria

Scenario: key_version desconocida falla la exportacion completa
  Given un registro con key_version="v99" y keystore no tiene "v99"
  When se invoca el descifrador durante exportacion
  Then la exportacion completa retorna 500 con EXP_002_KEY_NOT_FOUND

Scenario: Tampering detectado por AAD incorrecto
  Given payload con AAD modificado
  When se intenta descifrar
  Then EXP_002_INVALID_CONTEXT y exportacion abortada
```

## Trazabilidad de tests
- UT: EXP002_KnownKey_DecryptsCorrectly
- UT: EXP002_UnknownKey_ThrowsFailClosed
- UT: EXP002_TamperedAAD_ThrowsInvalidContext
- IT: EXP002_MultipleKeyVersions_HandledCorrectly

## Sin ambiguedades pendientes
- "Fail-closed" significa que si una clave falta, NO se retorna ningun dato parcial
- Los payloads descifrados nunca se loguean (ni en debug)
- El algoritmo de cifrado es AES-256-GCM; el IV esta incluido en el payload cifrado
