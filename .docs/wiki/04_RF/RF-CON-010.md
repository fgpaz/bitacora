# RF-CON-010: Revocar ConsentGrant con confirmacion

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-CON-010 |
| Modulo | CON |
| Actor | Patient (API) |
| Flujo fuente | FL-CON-02 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Patient autenticado con JWT valido.
- Existe ConsentGrant con status='granted' para el patient_id.
- UI ha enviado confirmed=true (segunda confirmacion explicita del usuario).

## Inputs
| Campo | Valor | Origen | Validacion |
|-------|-------|--------|-----------|
| patient_id | uuid | JWT | Existente con grant activo |
| confirmed | bool | Request body | Debe ser true |

## Proceso (Happy Path)
1. Verificar confirmed=true; si false retornar 422.
2. Iniciar transaccion atomica (ver RF-CON-013).
3. UPDATE ConsentGrant SET status='revoked', revoked_at=NOW().
4. Disparar cascade sobre CareLinks (RF-CON-011).
5. Invalidar caches (RF-CON-012).
6. INSERT AccessAudit con trace_id, operacion='CONSENT_REVOKE'.
7. Commit. Retornar 200 con revoked_at.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| status | string | "revoked" |
| revoked_at | timestamp | UTC del momento de revocacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CONFIRMED_FALSE | 422 | confirmed != true | {error: "CONFIRMED_FALSE"} |
| NO_ACTIVE_CONSENT | 404 | No hay grant activo | {error: "NO_ACTIVE_CONSENT"} |
| REVOCATION_FAILED | 500 | Fallo en transaccion | {error: "REVOCATION_FAILED"} |

## Casos especiales y variantes
- Patient sin CareLinks activos: cascade no hace nada; revocacion igual es exitosa.
- Fallo parcial: rollback total garantizado por RF-CON-013.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| ConsentGrant | UPDATE | status, revoked_at |
| AccessAudit | INSERT | trace_id, patient_id, operacion, created_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Revocar con confirmacion
  Given ConsentGrant.status="granted"
  When DELETE /api/v1/consent/current {confirmed: true}
  Then ConsentGrant.status="revoked" y se retorna 200

Scenario: Revocar sin confirmacion rechazado
  When DELETE /api/v1/consent/current {confirmed: false}
  Then se retorna 422 con error CONFIRMED_FALSE
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-CON-010-01 | Revocacion exitosa con confirmed=true | Positivo |
| TP-CON-010-02 | 422 con confirmed=false | Negativo |
| TP-CON-010-03 | 404 sin grant activo | Negativo |

## Sin ambiguedades pendientes
Ninguna.
