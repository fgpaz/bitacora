# RF-VIN-002: Buscar paciente por email hash

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-002 |
| Modulo | VIN |
| Actor | Sistema (interno) |
| Flujo fuente | FL-VIN-01 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Funcion de hash deterministico configurada (SHA-256 con salt de aplicacion).
- Tabla User tiene columna email_hash indexada.
- Email en texto plano nunca se persiste en esta operacion.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| input_email | string | Llamador interno | Formato email valido antes del hash |

## Proceso (Happy Path)
1. Normalizar email: lowercase, trim.
2. Calcular email_hash = SHA256(salt + normalized_email).
3. SELECT patient_id FROM users WHERE email_hash = ?.
4. Si existe: retornar patient_id.
5. Si no existe: retornar null (el llamador decide que hacer).

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| patient_id | uuid o null | ID resuelto o null si no encontrado |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| HASH_FAILURE | 500 | Fallo en funcion de hash | {error: "HASH_FAILURE"} |

## Casos especiales y variantes
- Email en mayusculas: normalizado antes del hash para garantizar match.
- Email no encontrado: retorna null, nunca lanza excepcion; el llamador interpreta.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| User | SELECT | email_hash, patient_id (solo lectura) |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Email existente resuelve patient_id
  Given user con email_hash = HASH("p@test.com") existe
  When se llama lookup con "p@test.com"
  Then se retorna el patient_id correspondiente

Scenario: Email inexistente retorna null
  Given no existe user con ese email_hash
  When se llama lookup con "noexiste@test.com"
  Then se retorna null sin excepcion
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-002-01 | Lookup exitoso retorna patient_id | Positivo |
| TP-VIN-002-02 | Email inexistente retorna null | Negativo |
| TP-VIN-002-03 | Email en mayusculas normalizado antes del hash | Borde |

## Sin ambiguedades pendientes
Ninguna.
