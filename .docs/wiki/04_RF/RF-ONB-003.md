# RF-ONB-003: Forzar pantalla de consent en onboarding

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-ONB-003 |
| Modulo | ONB |
| Trigger | Respuesta del bootstrap con `needs_consent=true` |
| Actor | API + Frontend |
| Prioridad | Privacy |

## Precondiciones detalladas
- `User.status = registered`.
- El frontend interpreta `needs_consent=true` y presenta la pantalla de consentimiento.
- Ningun endpoint de datos del paciente responde mientras el usuario siga `registered`.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| JWT | string | Authorization header | Token valido del paciente |
| version | string | Request body de POST /api/v1/consent | Debe coincidir con la version activa |
| resume_pending_invite | bool | Contexto de bootstrap | Indica si hay invitacion pendiente vigente |

## Proceso (Happy Path)
1. Si `User.status=registered`, cualquier endpoint de datos retorna `403 ONB_003_CONSENT_REQUIRED`.
2. El frontend redirige a la pantalla de consentimiento y el paciente ejecuta `POST /api/v1/consent`.
3. El sistema crea `ConsentGrant(patient_id, status='granted', consent_version, granted_at)`.
4. El sistema actualiza `User.status='consent_granted'`.
5. Si `resume_pending_invite=true` y la invitacion sigue vigente:
   a. Materializar `CareLink` en `status='active'` con `can_view_data=false`.
   b. Marcar `PendingInvite` como `consumed`.
6. Retornar `{status: "consent_granted", needs_first_entry: true, resume_pending_invite: false}`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| status | string | `consent_granted` |
| needs_first_entry | bool | Indica que falta el primer registro clinico |
| resume_pending_invite | bool | Debe retornar `false` tras procesarse el consentimiento |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| ONB_003_CONSENT_REQUIRED | 403 | Usuario `registered` intenta acceder a datos | {error: "ONB_003_CONSENT_REQUIRED"} |
| ONB_003_CONSENT_VERSION_MISSING | 400 | `version` no provista o vacia | {error: "ONB_003_CONSENT_VERSION_MISSING"} |
| ONB_003_ALREADY_GRANTED | 409 | Ya existe `ConsentGrant` activo | {error: "ONB_003_ALREADY_GRANTED"} |

## Casos especiales y variantes
- Si la `PendingInvite` expiro entre bootstrap y consentimiento, el consentimiento se registra igual y no se materializa el `CareLink`.
- El paciente no puede saltear esta pantalla ni registrar datos clinicos antes de completarla.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| ConsentGrant | INSERT | patient_id, consent_version, status, granted_at |
| User | UPDATE | status='consent_granted' |
| CareLink | INSERT (condicional) | professional_id, patient_id, status, can_view_data, invited_at, accepted_at |
| PendingInvite | UPDATE (condicional) | status='consumed', consumed_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Usuario registered no puede acceder a datos
  Given User.status=registered
  When GET /api/v1/mood-entries
  Then HTTP 403 con ONB_003_CONSENT_REQUIRED

Scenario: Consentimiento otorga acceso y consume invitacion pendiente
  Given User.status=registered
  And existe PendingInvite vigente reanudada
  When POST /api/v1/consent con version vigente
  Then ConsentGrant creado con status="granted"
  And User.status actualizado a consent_granted
  And PendingInvite queda consumida
  And se crea CareLink activo con can_view_data=false
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-ONB-003-01 | Bloquea endpoints de datos mientras status=registered | Positivo |
| TP-ONB-003-02 | Consentimiento consume PendingInvite vigente y crea CareLink | Positivo |
| TP-ONB-003-03 | Retorna 409 si ya existe grant activo | Negativo |

## Sin ambiguedades pendientes
Ninguna.
