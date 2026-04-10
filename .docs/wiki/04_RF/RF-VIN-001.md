# RF-VIN-001: Emitir invitacion de vinculo del profesional

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
- `patient_email` provisto en el body con formato valido.
- No existe `CareLink` activo o invitado entre este professional y ese patient.
- No existe `PendingInvite` vigente para la misma dupla `professional_id + invitee_email_hash`.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_email | string | Request body | Formato email valido |
| professional_id | uuid | JWT | Existente |

## Proceso (Happy Path)
1. Normalizar `patient_email` y calcular `email_hash` mediante RF-VIN-002.
2. Resolver `patient_id` desde `email_hash`.
3. Si `patient_id` existe:
   a. Verificar que no exista `CareLink` activo o invitado para el par.
   b. INSERT `CareLink {professional_id, patient_id, status='invited', can_view_data=false, invited_at=NOW()}`.
   c. INSERT `AccessAudit` con `action_type='create'`, `resource_type='care_link'`.
   d. Retornar `201` con `resource_type='care_link'`, `status='invited'`.
4. Si `patient_id` no existe:
   a. Verificar que no exista `PendingInvite` vigente para el mismo email hash y profesional.
   b. Generar `invite_token` opaco.
   c. INSERT `PendingInvite {professional_id, invitee_email_hash, invite_token, status='issued', expires_at=NOW()+7d}`.
   d. INSERT `AccessAudit` con `action_type='create'`, `resource_type='pending_invite'`.
   e. Retornar `201` con `resource_type='pending_invite'`, `status='issued'`, `expires_at`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| resource_type | string | `care_link` o `pending_invite` |
| resource_id | uuid | ID del artefacto creado |
| status | string | `invited` o `issued` |
| expires_at | timestamp? | Solo para `pending_invite` |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| INVALID_EMAIL_FORMAT | 422 | `patient_email` invalido | {error: "INVALID_EMAIL_FORMAT"} |
| CARELINK_EXISTS | 409 | Ya existe `CareLink` activo o invitado | {error: "CARELINK_EXISTS"} |
| PENDING_INVITE_EXISTS | 409 | Ya existe `PendingInvite` vigente | {error: "PENDING_INVITE_EXISTS"} |

## Casos especiales y variantes
- `can_view_data` siempre nace en `false` cuando se crea un `CareLink`.
- La invitacion a un paciente no registrado no revela si el email ya fue usado en otros contextos.
- `PendingInvite` expira a los 7 dias y debe reemitirse si se vence.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | INSERT (condicional) | professional_id, patient_id, status, can_view_data, invited_at |
| PendingInvite | INSERT (condicional) | professional_id, invitee_email_hash, invite_token, status, expires_at |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Profesional invita a un paciente ya registrado
  Given existe un paciente con email "p@test.com"
  When POST /api/v1/care-links {patient_email: "p@test.com"}
  Then se retorna 201 con resource_type="care_link"
  And status="invited"
  And can_view_data=false

Scenario: Profesional invita a un paciente aun no registrado
  Given no existe un paciente con email "nuevo@test.com"
  When POST /api/v1/care-links {patient_email: "nuevo@test.com"}
  Then se retorna 201 con resource_type="pending_invite"
  And status="issued"
  And expires_at corresponde a 7 dias

Scenario: Invitacion duplicada es rechazada
  Given ya existe un CareLink o PendingInvite vigente para el mismo profesional y email
  When POST /api/v1/care-links {patient_email: "p@test.com"}
  Then se retorna 409 con error CARELINK_EXISTS o PENDING_INVITE_EXISTS
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-001-01 | Crea CareLink invitado para paciente existente | Positivo |
| TP-VIN-001-02 | Crea PendingInvite de 7 dias para paciente no registrado | Positivo |
| TP-VIN-001-03 | Rechaza invitacion duplicada | Negativo |

## Sin ambiguedades pendientes
Ninguna.
