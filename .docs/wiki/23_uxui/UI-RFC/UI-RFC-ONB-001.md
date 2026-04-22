# UI-RFC-ONB-001 — ONB-first del paciente hasta consentimiento y puente

## Propósito

Este documento traduce `ONB-001` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS` ni autoriza extender el slice hacia registro clínico. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete `ONB-first` que T04/T05 pueden consumir sin reinterpretación fuerte.

## Estado del gate

- `slice`: `ONB-001`
- `estado`: `abierto para UI-RFC + HANDOFF`
- `fundamento`: waiver de entrada a UI + authority pack manual `2026-04-10`
- `límite`: esta apertura aplica solo a `ONB-001`; no relaja el gate de `REG-001` ni `REG-002`
- `deuda explícita`: la validación UX real sigue pendiente en `../UX-VALIDATION/UX-VALIDATION-ONB-001.md`

## Referencias obligatorias

- `../../16_patrones_ui.md`
- `../../21_matriz_validacion_ux.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../07_baseline_tecnica.md`
- `../../09_contratos_tecnicos.md`
- `../../06_matriz_pruebas_RF.md`
- `../../06_pruebas/TP-ONB.md`
- `../../06_pruebas/TP-CON.md`
- `../../03_FL/FL-ONB-01.md`
- `../../03_FL/FL-CON-01.md`
- `../../04_RF/RF-ONB-001.md`
- `../../04_RF/RF-ONB-002.md`
- `../../04_RF/RF-ONB-003.md`
- `../../04_RF/RF-CON-001.md`
- `../../04_RF/RF-CON-002.md`
- `../../04_RF/RF-CON-003.md`
- `../UXR/UXR-ONB-001.md`
- `../UXI/UXI-ONB-001.md`
- `../UJ/UJ-ONB-001.md`
- `../VOICE/VOICE-ONB-001.md`
- `../UXS/UXS-ONB-001.md`
- `../PROTOTYPE/PROTOTYPE-ONB-001.md`
- `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`
- `.docs/raw/decisiones/2026-04-10-onb-001-manual-authority-pack.md`

## Slice cubierto

### In

- portada pública estándar con CTA `Empezar ahora`;
- variante de portada con `hero adaptado` por invitación;
- fallback genérico de invitación cuando faltan datos;
- aclaración de contexto si la invitación genera confusión;
- retorno breve de auth/bootstrap;
- lectura y aceptación de consentimiento vigente;
- recordatorio ligero del contexto invitado hasta consentimiento;
- conflicto de versión y error recuperable de consentimiento;
- confirmación con puente a `Hacer mi primer registro`.

### Out

- formulario del primer `MoodEntry`;
- registro de factores diarios;
- revocación de consentimiento;
- gestión explícita de vínculos ya activos;
- flujos profesionales;
- variantes Telegram.

## Resultado que la UI debe producir

La UI debe sentirse como una guía personal cálida y breve que:

1. abre una entrada simple para el paciente;
2. preserva el contexto invitado sin convertirlo en otra app;
3. aterriza el consentimiento como resguardo claro;
4. deja una salida directa hacia el primer registro.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-HERO-STANDARD` | entrada base de guía personal | visita sin `invite_token` o sin contexto válido | `Empezar ahora` |
| `S01-HERO-INVITE` | explicar vínculo + propósito en la misma portada | `invite_token` presente y contexto resoluble | `Empezar ahora` |
| `S01-HERO-INVITE-FALLBACK` | conservar continuidad aunque falten datos del profesional | `invite_token` presente pero datos parciales | `Empezar ahora` |
| `S01-CONTEXT-CLARIFICATION` | bajar confusión sobre por qué llegó invitado/a | duda contextual antes de auth o antes de consent | `Entiendo, continuar` |
| `S02-AUTH-INTERSTITIAL` | sostener continuidad durante sesión + bootstrap | retorno de Zitadel o resolución de sesión | sin CTA nueva |
| `S03-CONSENT-READY` | aceptar consentimiento con resguardo claro | `needsConsent=true` y consentimiento vigente disponible | `Aceptar y continuar` |
| `S03-CONSENT-REMINDER` | recordar sutilmente el contexto invitado en consent | `resumePendingInvite=true` | `Aceptar y continuar` |
| `S03-VERSION-CONFLICT` | resolver cambio de versión sin jerga | `CONSENT_VERSION_MISMATCH` | `Volver a revisar` |
| `S03-SERVICE-ERROR` | reintentar o esperar si falta consentimiento activo | `NO_CONSENT_CONFIG` o error técnico recuperable | `Reintentar` |
| `S04-BRIDGE` | > **Deprecado 2026-04-22**: cerrar consent y orientar al primer registro via Bridge Card. Reemplazado por redirect directo a `/dashboard`. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`. | — | — |

## Regiones de composición

### `S01-HERO-*`

- `PublicEditorialShell`
- `BrandHeader` breve
- `HeroCopyBlock`
- `SupportTrustBlock`
- `PrimaryActionStack`
- `SecondaryUtilityRow` silenciosa (`Ya tengo cuenta` o equivalente, fuera de jerarquía dominante)

### `S02-AUTH-INTERSTITIAL`

- `PatientPageShell` minimal
- `ContinuityMessage`
- `LoadingIndicator` discreto
- `ContextReminderChip` opcional si el retorno viene desde invitación

### `S03-CONSENT-*`

- `PatientPageShell`
- `SensitiveContainer`
- `ConsentSummaryHeader`
- `ConsentSectionsList`
- `InviteReminderInline` opcional y ligero
- `ConsentDecisionBar`
- `InlineFeedback`

### `S04-BRIDGE`

> **Deprecado 2026-04-22**: esta region fue eliminada. El post-consent deriva directamente a `/dashboard` via `window.location.assign('/dashboard')`. No hay pantalla intermedia. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

Composicion historica (referencia archivada):
- `PatientPageShell`
- `InlineFeedback` de confirmacion factual
- `SecondaryNote` serena sobre el siguiente paso

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `PatientPageShell` | shell general del slice autenticado | loading, ready, error |
| `OnboardingEntryHero` | hero estándar de entrada | default |
| `InviteContextHero` | hero adaptado con vínculo + propósito | explicit, fallback |
| `SupportTrustBlock` | soporte de privacidad/resguardo en portada | default |
| `AuthBootstrapInterstitial` | continuidad breve entre auth y bootstrap | default, invite_context |
| `ConsentGatePanel` | lectura + aceptación del consentimiento | ready, reminder, version_conflict, service_error, submitting |
| `InlineFeedback` | error localizado o confirmación factual | info, error, confirm |
| `ContextClarificationPanel` | microaclaración por invitación/contexto | default |

Los nombres pueden refinarse en código, pero la separación de responsabilidades no debe colapsarse.

## Contratos backend que la UI debe consumir

### 1. Resolución de sesión y bootstrap

- precondición: existe sesión Zitadel válida resuelta por `bitacora_session` en T04;
- request:
  - `POST /api/v1/auth/bootstrap`
  - query opcional: `?invite_token=<token>`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada hoy:
  - `userId`
  - `status` (`registered`, `consent_granted`, `active`)
  - `needsConsent`
  - `resumePendingInvite`

### 2. Lectura de consentimiento

- request:
  - `GET /api/v1/consent/current`
- respuesta esperada hoy:
  - `version`
  - `text`
  - `sections`
  - `patientStatus`

### 3. Otorgamiento de consentimiento

- request:
  - `POST /api/v1/consent`
  - body: `{ "version": string, "accepted": true }`
- respuesta esperada hoy:
  - `consentGrantId`
  - `status` (`consent_granted`)
  - `grantedAtUtc`
  - `needsFirstEntry`
  - `resumePendingInvite`

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| click `Empezar ahora` con sesión inexistente | Zitadel OIDC | no session | ir a login y volver por `/auth/callback` |
| retorno autenticado | `POST /api/v1/auth/bootstrap` | `needsConsent=true` | mostrar `S02` y luego `S03` |
| retorno autenticado | `POST /api/v1/auth/bootstrap` | `status=consent_granted` o `active` | saltar consentimiento e ir a `S04` o a la siguiente ruta |
| apertura de consentimiento | `GET /api/v1/consent/current` | `patientStatus=none|pending|revoked` | `S03-CONSENT-READY` |
| envío de consentimiento | `POST /api/v1/consent` | `201` | > **Deprecado 2026-04-22**: antes derivaba a `S04-BRIDGE`; ahora va directo a `/dashboard`. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`. |
| consentimiento desactualizado | `POST /api/v1/consent` | `CONSENT_VERSION_MISMATCH` | `S03-VERSION-CONFLICT` |
| consentimiento ya otorgado | `POST /api/v1/consent` | `CONSENT_ALREADY_GRANTED` | resolver con redirect a `/dashboard` o refetch de estado |
| consentimiento no disponible | `GET /api/v1/consent/current` | `NO_CONSENT_CONFIG` | `S03-SERVICE-ERROR` |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `ONB_001_JWT_INVALID` / `ONB_001_JWT_EXPIRED` | salida a login con mensaje sobrio | no mostrar lenguaje técnico de JWT |
| `NO_CONSENT_CONFIG` | indisponibilidad operativa clara | no dramatizar ni esconder el bloqueo |
| `CONSENT_VERSION_MISMATCH` | refrescar el contenido y pedir revisión | no dejar al usuario en error genérico |
| `CONSENT_ALREADY_GRANTED` | continuar a `/dashboard` (redirect directo) | no duplicar la pantalla de consentimiento |
| `ACCEPTED_FALSE` | mantener al usuario en consentimiento | no inventar camino alternativo |
| `ONB_003_CONSENT_REQUIRED` / `CONSENT_REQUIRED` | redirección o bloqueo a consentimiento | el primer registro no puede abrir |

## Reglas de jerarquía y copy

- una sola acción dominante visible por estado;
- `Empezar ahora` es la CTA primaria arriba del fold;
- privacidad/resguardo vive como soporte, no como hero institucional;
- la invitación se expresa como `registro inicial con acompañamiento profesional`;
- el contexto invitado desaparece al redirigir a `/dashboard` (antes desaparecía en `S04-BRIDGE`, que fue deprecado 2026-04-22);
- la confirmación final es factual y no celebratoria.

## Responsive y accesibilidad

- `mobile-first` real para todo el slice;
- hero sin columnas paralelas en mobile;
- recordatorio invitado siempre debajo de la propuesta principal, nunca compitiendo con el CTA;
- consentimiento con headings, listas y controles navegables por teclado;
- `prefers-reduced-motion` respetado en interstitial y transiciones;
- el bridge debe conservar legibilidad y foco visible en mobile y desktop.

## Criterio de implementación

La implementación de T04/T05 cumple este contrato si:

1. `app/page.tsx` puede actuar como entrada pública `ONB-first`;
2. el flujo autenticado resuelve `bootstrap -> consent -> bridge` sin inventar estados nuevos;
3. la variante invitada reutiliza la misma portada con hero adaptado;
4. el primer registro queda fuera del slice y solo se abre por el bridge;
5. ningún texto o bloque revive el drift previo de Stitch (`bilingüe`, `poético`, `demasiado ceremonial`).

## Dependencias abiertas

- `REG-001` y `REG-002` siguen bloqueados por el gate visual previo y no deben contaminar este contrato;
- `T04` debe proveer sesión Zitadel, boundary y cliente API antes de `T05`;
- cuando exista implementación funcional, `UX-VALIDATION-ONB-001.md` podrá reabrir este contrato si la evidencia contradice el slice.

## Componentes fuera de alcance de ONB-001 (movidos al dashboard)

Los siguientes componentes eran parte del slice `ONB-001` en su version original y fueron removidos en 2026-04-22 como resultado de la decision "dashboard-first":

| Componente | Razon | Destino actual |
|-----------|-------|----------------|
| `NextActionBridgeCard` | La fase S04-BRIDGE fue eliminada | Eliminado; no tiene sucesor directo en ONB. La funcionalidad de puente se reemplaza por redirect a `/dashboard`. |
| CTA "Hacer mi primer registro" | Pertenecia a S04-BRIDGE | Reemplazado por CTA "Registrar humor" en el empty state del dashboard |
| CTA "Vincular Telegram (opcional)" | Pertenecia a S04-BRIDGE; mostraba estado incorrecto | Reemplazado por `TelegramReminderBanner` en el dashboard (respeta `linked` real del backend) |

El contrato de este documento cubre unicamente los estados S01, S02 y S03.

## Cambios recientes

- 2026-04-22: S04-BRIDGE y `NextActionBridgeCard` deprecados. El post-consent va directo a `/dashboard`. Ver decision doc `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

---

**Estado:** `UI-RFC` activo para `ONB-001` bajo authority pack manual. Cubre estados S01, S02 y S03. S04-BRIDGE deprecado 2026-04-22.
**Siguiente capa gobernada:** `HANDOFF-SPEC-ONB-001.md`, `HANDOFF-ASSETS-ONB-001.md`, `HANDOFF-MAPPING-ONB-001.md`, `HANDOFF-VISUAL-QA-ONB-001.md`.
