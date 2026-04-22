# HANDOFF-MAPPING-ONB-001 — Correspondencia diseño-código

## Propósito

Este documento traduce `ONB-001` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/page.tsx` | portada pública `ONB-first` |
| `frontend/app/(patient)/onboarding/page.tsx` | orquestación del flujo autenticado |
| `frontend/app/(patient)/consent/page.tsx` | entrada dedicada si el routing final separa consentimiento |
| `frontend/components/patient/onboarding/OnboardingFlow.tsx` | composición del slice y routing interno |
| `frontend/components/patient/consent/ConsentFlow.tsx` | variante separada del panel de consentimiento si T05 la extrae |
| `frontend/lib/patient/onboarding.ts` | llamadas a bootstrap y consentimiento |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-HERO-STANDARD` | `app/page.tsx` + subcomponente `OnboardingEntryHero` | puede vivir inline al inicio, pero conviene separarlo |
| `S01-HERO-INVITE` | `app/page.tsx` + subcomponente `InviteContextHero` | misma página, no ruta paralela |
| `S01-HERO-INVITE-FALLBACK` | misma unidad que `InviteContextHero` | cambiar contenido, no layout completo |
| `S01-CONTEXT-CLARIFICATION` | `OnboardingFlow.tsx` o panel reusable | overlay simple o bloque inline, sin modal compleja |
| `S02-AUTH-INTERSTITIAL` | `OnboardingFlow.tsx` + `AuthBootstrapInterstitial` | visible solo mientras resuelve sesión + bootstrap |
| `S03-CONSENT-*` | `OnboardingFlow.tsx` o `ConsentFlow.tsx` + `ConsentGatePanel` | puede extraerse si el archivo crece |
| `S04-BRIDGE` | > **Deprecado 2026-04-22**: `OnboardingFlow.tsx` + `OnboardingBridgeCard` — estado eliminado. El post-consent hace `window.location.assign('/dashboard')` directo. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`. | — |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `bootstrapPatient(inviteToken?)` | `frontend/lib/patient/onboarding.ts` | `POST /api/v1/auth/bootstrap` |
| `getCurrentConsent()` | `frontend/lib/patient/onboarding.ts` | `GET /api/v1/consent/current` |
| `grantConsent(version)` | `frontend/lib/patient/onboarding.ts` | `POST /api/v1/consent` |

## Rutas y salidas

| Estado final | Navegacion |
| --- | --- |
| consent otorgado (post-`POST /api/v1/consent`) | `window.location.assign('/dashboard')` directo |
| usuario ya consentido al entrar | redirect a `/dashboard` sin bridge intermedio |
| sesion invalida | volver a login o reanudar auth |

> **Deprecado 2026-04-22**: el estado "bridge completado" fue reemplazado por redirect directo a `/dashboard`. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa el flujo `ONB-first` usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

## Cambios recientes

- 2026-04-22: S04-BRIDGE y rutas de bridge deprecated. El post-consent va directo a `/dashboard`. Ver decision doc `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

---

**Estado:** mapping consumido por `T04/T05`. S04-BRIDGE deprecado 2026-04-22.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-ONB-001.md`.
