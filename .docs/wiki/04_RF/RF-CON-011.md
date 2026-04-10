# RF-CON-011: Cascade — revocar CareLinks al revocar consent

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-CON-011 |
| Modulo | CON |
| Actor | Sistema (interno, disparado por RF-CON-010) |
| Flujo fuente | FL-CON-02 |
| Prioridad | Security |

## Precondiciones detalladas
- Llamado dentro de la transaccion atomica de RF-CON-013.
- ConsentGrant ya marcado como revocado en la misma transaccion.
- patient_id disponible como parametro.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | Contexto de transaccion | Existente |

## Proceso (Happy Path)
1. SELECT todos los CareLinks WHERE patient_id=? AND status='active'.
2. UPDATE CareLinks SET status='revoked_by_consent', revoked_at=NOW() para cada uno.
3. Para cada CareLink afectado, INSERT AccessAudit con `action_type='revoke'`, `resource_type='care_link'`, `resource_id=care_link_id`.
4. Si no hay CareLinks activos: operacion es no-op, transaccion continua.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| affected_count | int | Cantidad de CareLinks revocados |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CASCADE_FAILURE | 500 | Error en UPDATE | Rollback total via RF-CON-013 |

## Casos especiales y variantes
- CareLink en status='invited': tambien se revoca (status='revoked_by_consent').
- CareLink ya revocado por paciente: no se modifica; se ignora.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | UPDATE | status, revoked_at |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Cascade revoca links activos
  Given patient tiene 2 CareLinks con status="active"
  When se ejecuta cascade por revocacion de consent
  Then ambos CareLinks tienen status="revoked_by_consent"

Scenario: Sin links activos cascade es no-op
  Given patient no tiene CareLinks activos
  When se ejecuta cascade
  Then operacion exitosa, affected_count=0
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-CON-011-01 | Links activos pasan a revoked_by_consent | Positivo |
| TP-CON-011-02 | Links invited tambien revocados | Positivo |
| TP-CON-011-03 | No-op sin links activos | Borde |

## Sin ambiguedades pendientes
Ninguna.
