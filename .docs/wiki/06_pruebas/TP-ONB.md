# TP-ONB — Plan de Pruebas del Modulo ONB

## Alcance

- RF cubiertos: RF-ONB-001..005
- Flujo origen: FL-ONB-01
- Rutas implementadas: `/onboarding` (patient shell), `/consent` (patient shell)

## Estado de ejecucion actual

- `Wave 1` implementa bootstrap via `OnboardingFlow` con maquina de estados en cliente: `auth -> consent -> redirect /dashboard`. (El estado `bridge` fue eliminado el 2026-04-22; ver decision doc `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.)
- `OnboardingFlow` consume `POST /api/v1/auth/bootstrap` y maneja errores tipados con `trace_id`.
- `PendingInvite` reanudable detectable via `resumePendingInvite: true` en respuesta bootstrap.
- Transicion a `active` con el primer `MoodEntry` sigue diferida al modulo de vinculos.
- `ConsentGatePanel` renderiza el flujo de consentimiento con version vigente y manejo de conflictos.
- El smoke vigente usa `infra/smoke/zitadel-cutover-smoke.ps1` y E2E browser con sesion real de Zitadel.

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| ONB-P01 | RF-ONB-001, RF-ONB-002 | Positivo | Bootstrap crea o resuelve usuario local sin duplicar |
| ONB-P02 | RF-ONB-001, RF-ONB-003 | Positivo | Bootstrap reanuda PendingInvite y onboarding obliga consentimiento |
| ONB-N01 | RF-ONB-001 | Negativo | Rechaza JWT invalido o expirado con mensaje humanizado |
| ONB-N02 | RF-ONB-003 | Negativo | Bloquea acceso a datos mientras status=registered |
| ONB-P03 | RF-ONB-004, RF-ONB-005 | Positivo | Primer MoodEntry post-consent lleva al usuario a active |
| ONB-N03 | RF-ONB-005 | Negativo | Evento duplicado o estado inesperado no rompe la transicion |

## Gherkin expandido

```gherkin
Scenario: Bootstrap de usuario nuevo con invitacion pendiente
  Given JWT valido de Zitadel con sub no existente
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

## Estados de interfaz segun implementacion

| Estado UI | Significado | Comportamiento |
|-----------|-------------|----------------|
| `loading` | skeleton `PatientPageShell loading` | se muestra mientras resuelve bootstrap |
| `error` (auth) | JWT invalido/expirado | mensaje humanizado + `PatientPageShell error` |
| `auth` | bootstrap resuelto, necesita consentimiento | `AuthBootstrapInterstitial` con variant |
| `consent` | necesita consentimiento | `ConsentGatePanel` + carga de version vigente |
| `bridge` | consentimiento ya otorgado | > **Deprecado 2026-04-22**: `NextActionBridgeCard` con `needsFirstEntry` — reemplazado por redirect directo a `/dashboard`. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`. |
| `locked` | nunca alcanzado en ONB por diseño | diferido a RF-REG-002 |

## Dependencias de validacion final

- Validacion UX de `AuthBootstrapInterstitial` en variantes `default` e `invite_context`.
- Validacion UX de `ConsentGatePanel` en escenarios `ready`, `conflict`, `error`, `submitting`.
- > **Deprecado 2026-04-22**: Validacion UX de `NextActionBridgeCard` — componente eliminado. El post-consent va directo a `/dashboard`. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.
- Cierre visual de `HANDOFF-VISUAL-QA-ONB-001` pendiente.

## Resultados de ejecucion (E2E 2026-04-15)

| TC ID | Estado | Ambiente | Fecha | Evidencia |
|-------|--------|----------|-------|-----------|
| ONB-P01 | HISTORICAL PASSED | produccion (GoTrue legacy) | 2026-04-15 | Evidencia previa al cutover Zitadel: artifacts/e2e/2026-04-15-e2e-full/F1-01-gotrue-login.json |
| ONB-P02 | HISTORICAL PASSED | produccion (GoTrue legacy) | 2026-04-15 | Bootstrap legacy validado antes de Wave B. Evidencia: F1-02-bootstrap.json |
| ONB-DB | HISTORICAL PASSED | produccion | 2026-04-15 | DB verification legacy previa a auth_subject. Evidencia: F1-03-db-user.txt |
| ONB-N01 | PASSED | produccion | 2026-04-15 | JWT fake retorna 401 UNAUTHORIZED. Evidencia: F7 SEC-JWT |

## Criterios de salida

- Cobertura positiva y negativa de los 5 RF del modulo.
- Evidencia de reanudacion automatica de PendingInvite y de transicion a active tras el primer registro.
- Para la ola de productivizacion backend-only, al menos debe existir evidencia del smoke de bootstrap autenticado.
- **Sin marcar validacion UX como completa** — el checklist visual sigue abierto segun `HANDOFF-VISUAL-QA-ONB-001`.
