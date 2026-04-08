# RF-VIN-021: Invalidar caches post-revocacion de vinculo

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-021 |
| Modulo | VIN |
| Actor | Sistema (interno, disparado por RF-VIN-020) |
| Flujo fuente | FL-VIN-03 |
| Prioridad | Privacy |

## Precondiciones detalladas
- CareLink ya marcado como revocado.
- Cache store disponible con claves indexadas por professional_id:patient_id.
- professional_id y patient_id disponibles como parametros.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| professional_id | uuid | CareLink revocado | Existente |
| patient_id | uuid | CareLink revocado | Existente |

## Proceso (Happy Path)
1. Construir clave: "safe_projection:{professional_id}:{patient_id}".
2. DEL clave en cache store.
3. Si cache no responde: loguear warning, no revertir revocacion.
4. INSERT AccessAudit operacion='CACHE_INVALIDATED_ON_LINK_REVOKE'.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| invalidated | bool | true si clave eliminada, false si no existia |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CACHE_UNAVAILABLE | warning | Cache store sin respuesta | Log warning; no falla la revocacion |

## Casos especiales y variantes
- Clave no existente en cache: DEL es idempotente; no es error.
- Multiples claves relacionadas (listas, indices): si existen, deben incluirse en la invalidacion.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| Cache store | DEL | safe_projection:{professional_id}:{patient_id} |
| AccessAudit | INSERT | trace_id, patient_id, operacion, created_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Cache invalidado tras revocar vinculo
  Given existe cache "safe_projection:prof-1:pat-1"
  When CareLink entre prof-1 y pat-1 es revocado
  Then clave eliminada del cache store

Scenario: Cache no disponible no bloquea revocacion
  Given cache store no responde
  When se ejecuta invalidacion
  Then se loguea warning y revocacion es exitosa
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-021-01 | Clave de cache eliminada tras revocacion | Positivo |
| TP-VIN-021-02 | Clave inexistente no genera error | Borde |
| TP-VIN-021-03 | Warning sin bloqueo si cache no disponible | Negativo |

## Sin ambiguedades pendientes
Ninguna.
