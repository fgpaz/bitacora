# RF-ONB-003: Forzar pantalla de consent en onboarding

## Execution Sheet
- Modulo: ONB
- Trigger: Respuesta del bootstrap (RF-ONB-001 / RF-ONB-002) con needs_consent=true
- Actor: API (contrato con frontend) + Sistema de validacion de consent
- Prioridad PDP: Privacy > Usability (el consent es obligatorio)

## Precondiciones detalladas
- User.status = registered (consent aun no otorgado)
- El frontend debe interpretar `needs_consent: true` y mostrar pantalla de consent
- Ningun endpoint de datos del paciente debe responder si User.status = registered

## Inputs
| Campo | Origen | Descripcion |
|-------|--------|-------------|
| JWT | Authorization header | Token del paciente con status=registered |

## Proceso (Happy Path)
1. Bootstrap retorna `{ needs_consent: true }` cuando status=registered (RF-ONB-002)
2. Cualquier endpoint de datos con JWT de usuario status=registered retorna 403 con `ONB_003_CONSENT_REQUIRED`
3. El frontend redirige a pantalla de consent
4. El usuario acepta: `POST /api/v1/consent` con el texto de consent vigente
5. Sistema crea `ConsentGrant(patient_id, status=granted, consent_version, granted_at)`
6. Sistema actualiza `User.status = consent_granted`
7. Retornar `{ status: "consent_granted", needs_first_entry: true }`

## Outputs
```json
// Respuesta al aceptar consent
{
  "status": "consent_granted",
  "needs_first_entry": true
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| ONB_003_CONSENT_REQUIRED | 403 | Usuario en status=registered intenta acceder a datos |
| ONB_003_CONSENT_VERSION_MISSING | 400 | consent_version no provisto en el POST |
| ONB_003_ALREADY_GRANTED | 409 | ConsentGrant ya existe para este usuario |

## Casos especiales y variantes
- Usuario intenta saltar la pantalla de consent accediendo directo a endpoints de datos: bloqueado por middleware global que verifica User.status
- Consent rechazado por el usuario: no se puede crear ConsentGrant, user permanece en status=registered
- Consent revocado en el futuro: status vuelve a requerir re-otorgamiento (fuera de scope del ONB)

## Impacto en modelo de datos
- INSERT en `consent_grants`
- UPDATE `users SET status = 'consent_granted'`
- Ambas operaciones en una transaccion

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Usuario status=registered no puede acceder a datos
  Given User.status=registered
  When GET /api/v1/mood-entries
  Then HTTP 403 con ONB_003_CONSENT_REQUIRED

Scenario: Usuario acepta consent y transiciona a consent_granted
  Given User.status=registered
  When POST /api/v1/consent con consent_version vigente
  Then ConsentGrant creado con status=granted
  And User.status actualizado a consent_granted
  And respuesta con needs_first_entry=true

Scenario: Segundo intento de otorgar consent
  Given ConsentGrant ya existe con status=granted
  When POST /api/v1/consent nuevamente
  Then HTTP 409 con ONB_003_ALREADY_GRANTED
```

## Trazabilidad de tests
- UT: ONB003_RegisteredStatus_BlocksDataEndpoints
- IT: ONB003_ConsentGrant_TransitionsStatus
- IT: ONB003_Transaction_ConsentAndStatusAtomic
- IT: ONB003_DuplicateConsent_Returns409

## Sin ambiguedades pendientes
- El bloqueo de endpoints aplica a TODOS los endpoints de datos, no solo algunos
- El `consent_version` debe coincidir con la version actualmente vigente del texto de consent
