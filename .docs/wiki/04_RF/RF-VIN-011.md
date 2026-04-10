# RF-VIN-011: Validar BindingCode y resolver professional_id

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-011 |
| Modulo | VIN |
| Actor | Sistema (interno, llamado por RF-VIN-012) |
| Flujo fuente | FL-VIN-02 |
| Prioridad | Security |

## Precondiciones detalladas
- Existe la entidad `BindingCode` con columnas `code`, `professional_id`, `ttl_preset`, `expires_at`, `used`.
- El llamado ocurre antes de cualquier `INSERT` de `CareLink` por auto-vinculacion.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| code | string | Request body | Formato `BIT-XXXXX` |

## Proceso (Happy Path)
1. Normalizar `code` a uppercase y trim.
2. SELECT `professional_id`, `expires_at`, `ttl_preset`, `used` FROM `BindingCode` WHERE `code=?`.
3. Verificar `used=false`.
4. Verificar `expires_at > NOW()`.
5. Retornar `professional_id` y metadatos del `BindingCode` al llamador.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| professional_id | uuid | ID del profesional dueno del codigo |
| ttl_preset | string | Preset originalmente elegido |
| expires_at | timestamp | UTC de expiracion validada |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| BINDING_CODE_NOT_FOUND | 404 | Codigo no existe | {error: "BINDING_CODE_NOT_FOUND"} |
| BINDING_CODE_EXPIRED | 410 | `expires_at <= NOW()` | {error: "BINDING_CODE_EXPIRED"} |
| BINDING_CODE_ALREADY_USED | 409 | `used=true` | {error: "BINDING_CODE_ALREADY_USED"} |

## Casos especiales y variantes
- Codigo en minusculas: se normaliza antes del lookup.
- La validacion usa el `expires_at` persistido; el preset no se recalcula ni se hereda de otro contexto.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| BindingCode | SELECT | code, professional_id, ttl_preset, expires_at, used |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Codigo valido resuelve professional_id
  Given existe un BindingCode no usado y no expirado
  When se valida el codigo
  Then se retorna professional_id del propietario

Scenario: Codigo expirado es rechazado
  Given existe un BindingCode con expires_at en el pasado
  When se valida el codigo
  Then se retorna 410 con BINDING_CODE_EXPIRED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-011-01 | Codigo valido retorna professional_id | Positivo |
| TP-VIN-011-02 | Codigo expirado retorna 410 | Negativo |
| TP-VIN-011-03 | Codigo ya usado retorna 409 | Negativo |

## Sin ambiguedades pendientes
Ninguna.
