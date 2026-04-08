# RF-VIN-012: Crear CareLink por auto-vinculacion

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-012 |
| Modulo | VIN |
| Actor | Patient (API) |
| Flujo fuente | FL-VIN-02 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Patient autenticado con JWT valido.
- Patient tiene ConsentGrant.status='granted'.
- Codigo "BIT-XXXXX" valido (RF-VIN-011).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| code | string | Request body | Formato "BIT-XXXXX", valido segun RF-VIN-011 |
| patient_id | uuid | JWT | Existente con consent activo |

## Proceso (Happy Path)
1. Validar codigo y resolver professional_id (RF-VIN-011).
2. Verificar que no existe CareLink activo entre professional_id y patient_id.
3. INSERT CareLink {professional_id, patient_id, status='active', can_view_data=false, invited_at=NOW(), accepted_at=NOW()}.
4. UPDATE binding_codes SET used=true WHERE code=?.
5. INSERT AccessAudit operacion='CARELINK_SELF_BOUND'.
6. Retornar 201 con care_link_id y status='active'.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| care_link_id | uuid | ID del vinculo creado |
| status | string | "active" |
| can_view_data | bool | false (invariante RF-VIN-004) |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CODE_NOT_FOUND | 404 | Codigo invalido | {error: "CODE_NOT_FOUND"} |
| CODE_EXPIRED | 410 | Codigo expirado | {error: "CODE_EXPIRED"} |
| LINK_ALREADY_EXISTS | 409 | CareLink duplicado | {error: "LINK_ALREADY_EXISTS"} |
| CONSENT_REQUIRED | 403 | Sin consent activo | {error: "CONSENT_REQUIRED"} |

## Casos especiales y variantes
- Auto-vinculacion crea link con status='active' directamente (sin pasar por 'invited').
- Codigo marcado como used=true inmediatamente despues del INSERT.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | INSERT | care_link_id, professional_id, patient_id, status, can_view_data, invited_at, accepted_at |
| binding_codes | UPDATE | used=true |
| AccessAudit | INSERT | trace_id, patient_id, operacion, created_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente se vincula con codigo valido
  Given codigo "BIT-ABC12" valido y no usado
  When POST /api/v1/care-links/bind {code: "BIT-ABC12"}
  Then CareLink creado con status="active" y can_view_data=false

Scenario: Codigo expirado rechazado
  When POST /api/v1/care-links/bind {code: "BIT-OLD00"}
  Then se retorna 410 CODE_EXPIRED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-012-01 | Vinculo creado con codigo valido | Positivo |
| TP-VIN-012-02 | 410 con codigo expirado | Negativo |
| TP-VIN-012-03 | Codigo marcado used=true tras vinculacion | Positivo |

## Sin ambiguedades pendientes
Ninguna.
