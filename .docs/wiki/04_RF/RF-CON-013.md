# RF-CON-013: Transaccion atomica consent + links

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-CON-013 |
| Modulo | CON |
| Actor | Sistema (orquestador de RF-CON-010/011/012) |
| Flujo fuente | FL-CON-02 |
| Prioridad | Correctness |

## Precondiciones detalladas
- BD soporta transacciones ACID (PostgreSQL).
- RF-CON-010, RF-CON-011 y RF-CON-012 existen como operaciones invocables.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | JWT | Existente con grant activo |
| confirmed | bool | Request body | true |

## Proceso (Happy Path)
1. BEGIN TRANSACTION.
2. Ejecutar RF-CON-010: UPDATE ConsentGrant status='revoked'.
3. Ejecutar RF-CON-011: UPDATE CareLinks status='revoked_by_consent'.
4. Ejecutar RF-CON-012: invalidar caches (operacion no-ACID; fallo es warning).
5. INSERT AccessAudit final con `action_type='revoke'`, `resource_type='consent_grant'`.
6. COMMIT.
7. En cualquier excepcion de pasos 2-3-5: ROLLBACK; retornar 500 REVOCATION_FAILED.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| status | string | "revoked" |
| revoked_at | timestamp | UTC del commit |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| REVOCATION_FAILED | 500 | Cualquier fallo en pasos 2-3-5 | {error: "REVOCATION_FAILED"} con rollback total |

## Casos especiales y variantes
- Fallo de cache (paso 4): no causa rollback; se loguea y continua.
- Deadlock DB: reintentar una vez; si falla de nuevo, 500.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| ConsentGrant | UPDATE | status, revoked_at |
| CareLink | UPDATE | status, revoked_at |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Transaccion completa exitosa
  Given patient con consent y 2 CareLinks activos
  When DELETE /api/v1/consent/current {confirmed: true}
  Then ConsentGrant y ambos CareLinks actualizados en el mismo commit

Scenario: Rollback si falla UPDATE de CareLinks
  Given UPDATE CareLinks lanza excepcion
  When se ejecuta revocacion
  Then ConsentGrant permanece status="granted" y se retorna 500
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-CON-013-01 | Commit atomico exitoso | Positivo |
| TP-CON-013-02 | Rollback total si falla cascade | Negativo |
| TP-CON-013-03 | Fallo de cache no causa rollback | Borde |

## Sin ambiguedades pendientes
Ninguna.
