# RF-VIN-011: Validar codigo y resolver professional_id

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-011 |
| Modulo | VIN |
| Actor | Sistema (interno, llamado por RF-VIN-012) |
| Flujo fuente | FL-VIN-02 |
| Prioridad | Security |

## Precondiciones detalladas
- Tabla binding_codes existe con columnas code, professional_id, expires_at, used.
- Llamado antes de cualquier INSERT de CareLink por binding.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| code | string | Request body | Formato "BIT-XXXXX" |

## Proceso (Happy Path)
1. Normalizar codigo: uppercase, trim.
2. SELECT professional_id, expires_at, used FROM binding_codes WHERE code=?.
3. Verificar used=false.
4. Verificar expires_at > NOW().
5. Retornar professional_id al llamador.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| professional_id | uuid | ID del profesional dueno del codigo |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CODE_NOT_FOUND | 404 | Codigo no existe en BD | {error: "CODE_NOT_FOUND"} |
| CODE_EXPIRED | 410 | expires_at <= NOW() | {error: "CODE_EXPIRED"} |
| CODE_ALREADY_USED | 409 | used=true | {error: "CODE_ALREADY_USED"} |

## Casos especiales y variantes
- Codigo en minusculas: normalizar antes de lookup para evitar falsos negativos.
- No revelar si el codigo existio pero expiro vs nunca existio (privacy): unificar en CODE_NOT_FOUND es aceptable.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| binding_codes | SELECT | code, professional_id, expires_at, used (solo lectura) |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Codigo valido resuelve professional_id
  Given codigo "BIT-ABC12" existe, no usado y no expirado
  When se valida el codigo
  Then se retorna professional_id del propietario

Scenario: Codigo expirado rechazado
  Given codigo "BIT-XYZ99" tiene expires_at en el pasado
  When se valida el codigo
  Then se retorna CODE_EXPIRED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-011-01 | Codigo valido retorna professional_id | Positivo |
| TP-VIN-011-02 | Codigo expirado retorna 410 | Negativo |
| TP-VIN-011-03 | Codigo ya usado retorna 409 | Negativo |

## Sin ambiguedades pendientes
Ninguna.
