# RF-VIN-010: Generar codigo de vinculacion para profesional

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
- No existe binding_code activo (no expirado) para este professional (o se permite uno por vez).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| professional_id | uuid | JWT | Existente |

## Proceso (Happy Path)
1. Generar codigo con formato "BIT-XXXXX" (5 chars alfanumericos uppercase, criptograficamente aleatorio).
2. Calcular expires_at = NOW() + 15 minutos.
3. INSERT binding_codes {code, professional_id, expires_at, used=false}.
4. INSERT AccessAudit operacion='BINDING_CODE_GENERATED'.
5. Retornar 201 con code y expires_at.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| code | string | "BIT-XXXXX" codigo generado |
| expires_at | timestamp | UTC de expiracion (15 min) |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CODE_GENERATION_FAILED | 500 | Fallo en generacion aleatoria | {error: "CODE_GENERATION_FAILED"} |

## Casos especiales y variantes
- Si ya existe codigo activo: invalidar el anterior e insertar nuevo (un codigo activo por professional).
- Colision de codigo: reintentar generacion hasta que sea unico (max 3 intentos).

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| binding_codes | INSERT | code, professional_id, expires_at, used |
| binding_codes | UPDATE (condicional) | used=true en codigo anterior |
| AccessAudit | INSERT | trace_id, professional_id, operacion, created_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Profesional genera codigo de vinculacion
  Given professional autenticado
  When POST /api/v1/professional/binding-codes
  Then se retorna codigo formato "BIT-XXXXX" con TTL 15 min

Scenario: Codigo expira tras 15 minutos
  Given codigo "BIT-ABC12" generado hace 16 minutos
  When se intenta usar el codigo
  Then se rechaza por expirado (RF-VIN-011)
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-010-01 | Codigo generado con formato correcto y TTL 15 min | Positivo |
| TP-VIN-010-02 | Codigo anterior invalidado al generar nuevo | Borde |
| TP-VIN-010-03 | Colision resuelta con reintento | Borde |

## Sin ambiguedades pendientes
Ninguna.
