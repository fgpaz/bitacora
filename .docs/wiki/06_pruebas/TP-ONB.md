# TP-ONB — Plan de Pruebas del Modulo ONB

## Alcance

- RF cubiertos: RF-ONB-001..005
- Flujo origen: FL-ONB-01

## Estado de ejecucion actual

- `Wave 1` implementa bootstrap, deteccion de `PendingInvite` reanudable y transicion a `active` con el primer `MoodEntry`.
- El consumo efectivo de `PendingInvite` y la creacion de `CareLink` posterior al consentimiento siguen diferidos hasta que exista el modulo de vinculos.
- T01 agrega un smoke backend minimo que ejecuta `POST /api/v1/auth/bootstrap` contra el runtime real mediante `infra/smoke/backend-smoke.ps1`.

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| ONB-P01 | RF-ONB-001, RF-ONB-002 | Positivo | Bootstrap crea o resuelve usuario local sin duplicar |
| ONB-P02 | RF-ONB-001, RF-ONB-003 | Positivo | Bootstrap reanuda PendingInvite y onboarding obliga consentimiento |
| ONB-N01 | RF-ONB-001 | Negativo | Rechaza JWT invalido o expirado |
| ONB-N02 | RF-ONB-003 | Negativo | Bloquea acceso a datos mientras status=registered |
| ONB-P03 | RF-ONB-004, RF-ONB-005 | Positivo | Primer MoodEntry post-consent lleva al usuario a active |
| ONB-N03 | RF-ONB-005 | Negativo | Evento duplicado o estado inesperado no rompe la transicion |

## Gherkin expandido

```gherkin
Scenario: Bootstrap de usuario nuevo con invitacion pendiente
  Given JWT valido de Supabase con sub no existente
  And existe PendingInvite vigente para el email del JWT
  When POST /api/v1/auth/bootstrap
  Then se crea User local con status="registered"
  And needs_consent=true
  And resume_pending_invite=true

Scenario: Onboarding obliga consentimiento y prepara la continuidad del invite
  Given User.status="registered"
  And existe PendingInvite vigente reanudada
  When POST /api/v1/consent con version vigente
  Then se crea ConsentGrant con status="granted"
  And User.status pasa a consent_granted
  And el runtime queda listo para reanudar el contexto del invite
  And la consumicion del PendingInvite y la creacion del CareLink quedan diferidas a la ola de Vinculos

Scenario: Primer MoodEntry activa definitivamente al usuario
  Given User.status="consent_granted"
  When el paciente crea su primer MoodEntry
  Then se emite FirstMoodEntryCreated
  And User.status pasa a active
  And un evento duplicado posterior no cambia el resultado
```

## Criterios de salida

- Cobertura positiva y negativa de los 5 RF del modulo.
- Evidencia de reanudacion automatica de PendingInvite y de transicion a active tras el primer registro.
- Para la ola de productivizacion backend-only, al menos debe existir evidencia del smoke de bootstrap autenticado.
