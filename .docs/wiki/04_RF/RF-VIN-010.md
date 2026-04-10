# RF-VIN-010: Generar BindingCode para auto-vinculacion

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-010 |
| Modulo | VIN |
| Actor | Professional (API) |
| Flujo fuente | FL-VIN-02 |
| Prioridad | Security |

## Precondiciones detalladas
- Professional autenticado con JWT valido.
- El TTL del codigo se define por emision, no por profesional ni por `CareLink`.
- Solo puede existir un `BindingCode` activo por profesional; el anterior se invalida al generar uno nuevo.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| ttl_preset | string | Request body (opcional) | Uno de `15m`, `3h`, `24h`, `72h`; default `15m` |
| professional_id | uuid | JWT | Existente |

## Proceso (Happy Path)
1. Resolver `ttl_preset`; si no viene, usar `15m`.
2. Generar codigo con formato `BIT-XXXXX` (5 caracteres alfanumericos uppercase, criptograficamente aleatorio).
3. Calcular `expires_at` segun el preset elegido.
4. Invalidar cualquier `BindingCode` activo previo del profesional.
5. INSERT `BindingCode {code, professional_id, ttl_preset, expires_at, used=false}`.
6. INSERT `AccessAudit` con `action_type='create'`, `resource_type='binding_code'`.
7. Retornar `201` con `code`, `ttl_preset` y `expires_at`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| code | string | Codigo generado con formato `BIT-XXXXX` |
| ttl_preset | string | Preset aplicado |
| expires_at | timestamp | UTC de expiracion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| INVALID_BINDING_CODE_TTL | 422 | `ttl_preset` fuera del catalogo permitido | {error: "INVALID_BINDING_CODE_TTL"} |
| CODE_GENERATION_FAILED | 500 | Fallo en generacion aleatoria o colisiones repetidas | {error: "CODE_GENERATION_FAILED"} |

## Casos especiales y variantes
- Si ya existe codigo activo, se invalida antes de insertar el nuevo.
- En colision de codigo, se reintenta hasta 3 veces antes de fallar cerrado.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| BindingCode | UPDATE (condicional) | used=true en codigo activo previo |
| BindingCode | INSERT | code, professional_id, ttl_preset, expires_at, used |
| AccessAudit | INSERT | trace_id, actor_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Profesional genera BindingCode con TTL default
  Given professional autenticado
  When POST /api/v1/professional/binding-codes sin body
  Then se retorna codigo formato "BIT-XXXXX"
  And ttl_preset="15m"

Scenario: Profesional genera BindingCode con preset 72h
  Given professional autenticado
  When POST /api/v1/professional/binding-codes {ttl_preset: "72h"}
  Then se retorna expires_at a 72 horas

Scenario: Preset invalido es rechazado
  When POST /api/v1/professional/binding-codes {ttl_preset: "2d"}
  Then se retorna 422 INVALID_BINDING_CODE_TTL
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-010-01 | Codigo generado con TTL default 15m | Positivo |
| TP-VIN-010-02 | Codigo generado con preset 72h | Positivo |
| TP-VIN-010-03 | 422 si el preset no pertenece al catalogo | Negativo |

## Sin ambiguedades pendientes
Ninguna.
