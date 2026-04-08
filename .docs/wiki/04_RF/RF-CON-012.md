# RF-CON-012: Invalidar caches de safe_projection post-revocacion

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-CON-012 |
| Modulo | CON |
| Actor | Sistema (interno, disparado por RF-CON-010) |
| Flujo fuente | FL-CON-02 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Consent revocado y CareLinks en cascade ya procesados (RF-CON-011).
- Existe capa de cache (Redis o equivalente) con claves indexadas por professional_id:patient_id.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | Contexto de revocacion | Existente |
| affected_professional_ids | uuid[] | Lista de professionals de CareLinks revocados | Puede ser vacia |

## Proceso (Happy Path)
1. Para cada professional_id en affected_professional_ids:
   a. Construir clave de cache: "safe_projection:{professional_id}:{patient_id}".
   b. DEL clave en cache store.
2. Si cache store no responde: loguear warning, no bloquear transaccion.
3. INSERT AccessAudit operacion='CACHE_INVALIDATED' por cada professional afectado.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| invalidated_count | int | Claves eliminadas del cache |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CACHE_UNAVAILABLE | warning | Cache store sin respuesta | Log warning; no falla la transaccion |

## Casos especiales y variantes
- Lista vacia: no-op, ninguna clave eliminada.
- Cache ya expirado: DEL es idempotente; no hay error si la clave no existe.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| Cache store | DEL | safe_projection:{professional_id}:{patient_id} |
| AccessAudit | INSERT | trace_id, patient_id, operacion, created_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Invalidar cache de profesional con acceso previo
  Given existe cache "safe_projection:prof-1:pat-1"
  When se ejecuta invalidacion post-revocacion
  Then la clave es eliminada del cache store

Scenario: Cache no disponible no bloquea flujo
  Given cache store no responde
  When se ejecuta invalidacion
  Then se loguea warning y la revocacion completa igualmente
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-CON-012-01 | Clave de cache eliminada correctamente | Positivo |
| TP-CON-012-02 | No-op con lista vacia | Borde |
| TP-CON-012-03 | Warning sin bloqueo si cache no disponible | Negativo |

## Sin ambiguedades pendientes
Ninguna.
