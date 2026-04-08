# RF-VIN-001: Crear CareLink con invitacion del profesional

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-001 |
| Modulo | VIN |
| Actor | Professional (API) |
| Flujo fuente | FL-VIN-01 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Professional autenticado con JWT valido.
- patient_email provisto en el body.
- No existe CareLink activo o invitado entre este professional y ese patient.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_email | string | Request body | Formato email valido |
| professional_id | uuid | JWT | Existente |

## Proceso (Happy Path)
1. Calcular email_hash = HASH(patient_email) (ver RF-VIN-002).
2. Resolver patient_id desde email_hash.
3. Verificar que no existe CareLink activo/invitado para el par professional_id:patient_id.
4. INSERT CareLink {care_link_id, professional_id, patient_id, status='invited', can_view_data=false, invited_at=NOW()}.
5. INSERT AccessAudit operacion='CARELINK_INVITED'.
6. Retornar 201 con care_link_id y status.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| care_link_id | uuid | ID del vinculo creado |
| status | string | "invited" |
| invited_at | timestamp | UTC de creacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| PATIENT_NOT_FOUND | 404 | email_hash sin match | {error: "PATIENT_NOT_FOUND"} |
| LINK_ALREADY_EXISTS | 409 | CareLink duplicado | {error: "LINK_ALREADY_EXISTS"} |

## Casos especiales y variantes
- can_view_data siempre false en creacion (invariante RF-VIN-004).
- Si patient no existe en sistema: retornar 404 sin revelar si el email existe.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | INSERT | care_link_id, professional_id, patient_id, status, can_view_data, invited_at |
| AccessAudit | INSERT | trace_id, professional_id, operacion, created_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Profesional invita paciente existente
  Given paciente con email "p@test.com" existe
  When POST /api/v1/care-links {patient_email: "p@test.com"}
  Then se retorna 201 con status="invited" y can_view_data=false

Scenario: Email sin match retorna 404
  When POST /api/v1/care-links {patient_email: "noexiste@test.com"}
  Then se retorna 404 con error PATIENT_NOT_FOUND
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-001-01 | CareLink creado con status=invited | Positivo |
| TP-VIN-001-02 | 404 si email no tiene match | Negativo |
| TP-VIN-001-03 | 409 si link duplicado | Negativo |

## Sin ambiguedades pendientes
Ninguna.
