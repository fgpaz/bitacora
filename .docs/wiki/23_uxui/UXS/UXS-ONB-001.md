# UXS-ONB-001 — Portada, consentimiento y puente del ONB-first

## Propósito

Este documento fija el contrato UX del slice `ONB-001` como paquete completo.

No describe todavía código ni layout final de implementación. Su función es volver operables la portada `ONB-first`, la variante invitada, el retorno `auth/bootstrap`, el consentimiento y la confirmación con puente al primer registro.

## Relación con el canon

Este documento depende de:

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `16_patrones_ui.md`
- `../UXR/UXR-ONB-001.md`
- `../UXI/UXI-ONB-001.md`
- `../UJ/UJ-ONB-001.md`
- `../VOICE/VOICE-ONB-001.md`
- `../../03_FL/FL-ONB-01.md`
- `../../03_FL/FL-CON-01.md`
- `../../04_RF/RF-ONB-001.md`
- `../../04_RF/RF-ONB-003.md`
- `../../04_RF/RF-CON-001.md`
- `../../04_RF/RF-CON-002.md`
- `../../04_RF/RF-CON-003.md`
- `../../06_pruebas/TP-ONB.md`
- `../../06_pruebas/TP-CON.md`

Y prepara directamente:

- `../PROTOTYPE/PROTOTYPE-ONB-001.md`
- `../UI-RFC/UI-RFC-ONB-001.md`
- `../HANDOFF-SPEC/HANDOFF-SPEC-ONB-001.md`

No debe contradecir:

- la secuencia real `bootstrap -> consent -> needs_first_entry`;
- la regla de una sola acción dominante por contexto;
- el tone set humano y cálido del slice;
- el hard gate de consentimiento.

## Slice dueño

### Nombre

`ONB-001` — entrada `ONB-first` del paciente hasta consentimiento y puente al primer registro.

### Alcance visible

Incluye:

- portada estándar;
- portada con hero adaptado por invitación;
- fallback genérico del hero adaptado;
- interstitial breve de retorno `auth/bootstrap`;
- consentimiento base;
- consentimiento con recordatorio ligero de contexto invitado;
- fricción principal por invitación/contexto;
- conflicto de versión del consentimiento;
- confirmación con CTA al primer registro.

Excluye:

- la implementación del primer `MoodEntry`;
- daily check-in;
- dashboards o pantallas profesionales;
- Telegram.

## Cobertura obligatoria

- desktop;
- mobile;
- estados clave;
- pack ampliado de fricciones con foco principal en confusión por invitación/contexto.

## Sensación del slice

La experiencia debe sentirse como:

- guía personal;
- cálida y seria;
- clara para empezar;
- explícita al hablar de control;
- breve al pasar por auth;
- serena al cerrar.

La anti-sensación principal es:

**“esto parece una herramienta del profesional o una admisión clínica”.**

## Contrato de interacción

### S01 — Portada estándar

#### Objetivo

Presentar Bitácora como espacio personal y abrir el onboarding con una sola acción dominante.

#### Jerarquía obligatoria

1. historia principal de guía personal;
2. CTA principal `Ingresar` (ver nota en la sección "Cambios recientes"; el label canónico desde 2026-04-22 es `Ingresar` y apunta directamente a `/ingresar` OIDC+PKCE);
3. soporte dominante de privacidad y resguardo;
4. cualquier camino de retorno queda fuera del hero principal o con prominencia muy baja.

#### Reglas

- no debe existir un CTA secundario fuerte;
- el soporte de privacidad no debe competir con la acción;
- el hero no debe parecer landing institucional.

### S01 — Hero adaptado por invitación

#### Objetivo

Resolver la fricción de contexto sin crear una home separada.

#### Reglas

- la misma portada adapta su hero;
- la señal dominante es `vínculo + propósito`;
- el propósito visible se expresa como `registro inicial con acompañamiento profesional`;
- el detalle visible es explícito: nombre + rol + frase breve de propósito cuando existan datos;
- si faltan datos, usar hero adaptado genérico y no volver al modo estándar.

#### Soporte

- privacidad y resguardo siguen visibles como capa secundaria;
- la invitación puede llegar hasta consentimiento con recordatorio ligero, pero desaparece en la confirmación final.

### S02 — Retorno `auth/bootstrap`

#### Objetivo

Mantener continuidad y evitar un salto técnico.

#### Reglas

- resolver como `interstitial breve`;
- una sola idea principal: se está preparando el espacio para seguir;
- no mostrar progreso técnico, claims ni estados internos;
- no agregar CTA paralelos.

### S03 — Consentimiento

#### Objetivo

Hacer explícito el control con `resguardo claro`.

#### Estructura mínima

1. encabezado breve;
2. resumen de control;
3. texto vigente del consentimiento;
4. confirmación explícita;
5. CTA principal `Aceptar y seguir`.

#### Reglas

- el resumen de control aparece antes del texto completo;
- debe nombrar que aceptar no activa acceso automático del profesional;
- si hubo invitación, mostrar un recordatorio contextual ligero, no dominante;
- el consentimiento sigue siendo una sola columna, claro en desktop y mobile;
- la acción primaria se habilita sólo con lectura + confirmación explícita.

### S03 — Fricción principal por contexto

#### Qué debe resolver

Cuando la persona duda qué significa la invitación o qué implica “acompañamiento profesional”, la UI debe aclarar:

- por qué llegó;
- qué parte del vínculo es contextual;
- y qué sigue bajo control del paciente.

#### Regla

Esta aclaración vive dentro del mismo slice, sin modal paralelo ni desvío largo.

### S03 — Conflicto de versión

- debe mostrarse como estado explícito;
- recentra foco sobre la versión vigente;
- evita reintentos ambiguos;
- mantiene el tono sereno.

### S04 — Confirmación + puente

> **Deprecado 2026-04-22**: El estado S04-BRIDGE y el componente `NextActionBridgeCard` fueron eliminados. El post-consent deriva directamente a `/dashboard` via `window.location.assign('/dashboard')`. No existe pantalla de puente. El primer registro se hace desde el empty state del dashboard. Ver decision doc `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`. El estado de reemplazo es `S05-DASHBOARD-EMPTY` (empty state del dashboard).

Historia del contrato (referencia archivada):

#### Objetivo

Cerrar el consentimiento y empujar al siguiente valor.

#### Reglas (historicas)

- confirmacion factual breve;
- > **Deprecado 2026-04-22**: CTA principal exacto `Hacer mi primer registro` — reemplazado por redirect directo al dashboard sin pantalla de puente. El CTA equivalente en el empty state del dashboard es `Registrar humor`. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.
- sin celebraciones;
- sin recordar otra vez la invitacion;
- sin presentar el primer formulario dentro de este slice.

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| `hero_standard` | portada `ONB-first` estándar | CTA principal lista para iniciar |
| `hero_invite_explicit` | hero adaptado con vínculo + propósito visibles | baja confusión contextual |
| `hero_invite_fallback` | hero adaptado genérico | conserva contexto aunque falten datos |
| `invite_expired` | contexto no válido o vencido | salida clara y digna |
| `auth_interstitial` | transición breve post-auth | continuidad sin explicación técnica |
| `consent_default` | consentimiento base con resumen de control | prepara aceptación |
| `consent_invite_reminder` | consentimiento con recordatorio ligero del contexto invitado | continuidad contextual sin dominancia |
| `consent_context_clarification` | aclaración por confusión de invitación/contexto | resuelve la fricción principal |
| `consent_version_conflict` | cambio de versión vigente | exige revisar la versión actual |
| `consent_error_retryable` | error recuperable cerca del rail de acción | reintento digno |
| `bridge_ready` | > **Deprecado 2026-04-22**: confirmacion + CTA al primer registro (Bridge Card). Reemplazado por redirect a `/dashboard`. El estado de reemplazo es el empty state del dashboard (`S05-DASHBOARD-EMPTY`). Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`. | — |

## Responsive

- mobile-first de una columna;
- desktop conserva una columna dominante con soporte discreto;
- el hero adaptado no debe fragmentarse en varias columnas que rompan la lectura;
- el rail de acción de consentimiento debe seguir siendo inequívoco en ambos tamaños.

## Accesibilidad

- foco visible y estable en todos los estados;
- CTA y checkbox accesibles por teclado;
- estados no comunicados sólo por color;
- el recordatorio contextual de invitación debe ser legible sin depender de iconografía;
- el interstitial debe seguir siendo entendible con movimiento reducido.

## Acceptance criteria

1. La portada estándar deja una sola acción dominante: `Ingresar` (label canónico desde 2026-04-22; va directo a `/ingresar` OIDC+PKCE).
2. No existe un camino secundario fuerte dentro del hero.
3. La variante invitada adapta el mismo hero y muestra vínculo + propósito explícitos.
4. Si faltan datos del vínculo, existe hero adaptado genérico.
5. El retorno `auth/bootstrap` se resuelve con interstitial breve y no técnico.
6. El consentimiento se siente como `resguardo claro` y no como pared legal.
7. La fricción principal por invitación/contexto tiene una resolución visible dentro del slice.
8. La invitación persiste hasta consentimiento con recordatorio ligero y desaparece en confirmación.
9. > **Deprecado 2026-04-22**: La confirmación final usa `Hacer mi primer registro` como CTA inequívoco — este AC corresponde a S04-BRIDGE eliminado. El reemplazo es: tras `grantConsent` el paciente hace `window.location.assign('/dashboard')` y el CTA "Registrar humor" del empty state del dashboard abre el `MoodEntryDialog`. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.
10. Desktop y mobile cubren el mismo state pack sin reinterpretación.

## Defaults transferibles

Este `UXS` fija como defaults para implementación posterior:

- hero único con variante contextual;
- secondary paths silenciosos;
- interstitials breves de continuidad;
- consentimiento con resumen de control;
- bridges de siguiente acción sin celebración.

## Criterio de validación rápida

El slice está bien calibrado si:

- guía, contexto y resguardo se leen en ese orden;
- la invitación aclara sin colonizar toda la experiencia;
- el consentimiento no rompe el ritmo;
- el cierre deja una acción siguiente clara.

El slice está mal calibrado si:

- el hero parece institucional;
- la invitación se vuelve una mini-home separada;
- auth o consentimiento se vuelven técnicos;
- o la confirmación pretende cerrar más de lo que el slice muestra.

## Cambios recientes

- 2026-04-22: S04-BRIDGE y el estado `bridge_ready` deprecados. El post-consent va directo a `/dashboard`. Ver decision doc `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

## Deltas 2026-04-22 — impeccable-hardening

> 2026-04-22 — sync impeccable-hardening: deltas aplicados sobre implementación en rama `feature/impeccable-hardening-2026-04-22` (W2–W3–W4–W7–W10). Fuente de verdad: `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md`.

### ConsentGatePanel — correcciones semánticas y accesibilidad

- Elemento raíz cambiado de `<section role="listitem">` a `<div role="listitem">`: la combinación landmark + listitem era inválida según ARIA 1.2 (W2).
- Copy contextual corregido con tildes obligatorias (regla 9.1): `"Recordá que viniste a través de una invitación de tu profesional."` (W2).
- `ConsentGatePanel.acceptBtn` y `ConsentGatePanel.retryBtn`: `min-height: 44px` aplicado para cumplir WCAG 2.5.5 — CTAs de consentimiento son acción legal crítica (W3).

### OnboardingEntryHero — copy y microinteracción CTA

- Wordmark en heading: `"Bitácora"` con tilde (regla 9.1) (W2).
- Sub-copy: `"Un lugar sobrio..."` corregido para eliminar la duplicación de "tranquilidad" detectada en critique T1 (W2).
- CTA `"Ingresar"` (copy congelado — no modificar): hover con transición `background-color 150ms` + `color-mix` sobre `--brand-primary` + guard `@media (prefers-reduced-motion: reduce)` local en el módulo CSS (W10).

### OnboardingFlow — error fallback

- Mensaje de error recuperable especificado como `"No pudimos guardar el registro..."` reemplazando genérico anterior (canon 13 §Errores) (W2).

### Notas de implementación

- CTA `"Ingresar"` y copy `"Tu espacio personal de registro"` son zonas congeladas; no fueron modificados.
- Todos los cambios son `ui-only, no-schema, no-contract, no-auth`.

## Deltas 2026-04-23 — login flow redesign

> 2026-04-23 — sync login flow redesign: deltas aplicados sobre implementación en rama `feature/login-flow-redesign-2026-04-23` (W1–W4), merged a `main` en commit `5d91158`. Fuente de verdad: `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md`.

### OnboardingEntryHero — variant `"returning"` para paciente con cookie viva

- Nueva variant `returning` en `OnboardingEntryHero.tsx`: cuando `app/page.tsx` (Server Component) detecta cookie `bitacora_session` viva, el hero muestra `h1 "Volviste."` + sub `"Seguí donde dejaste."` + único CTA `"Seguir registrando"` → `/dashboard`. Rompe el anti-patrón de tratar al recurrente como primera vez (E1-F2 del audit 2026-04-23).
- La detección es server-side sin tocar `lib/auth/*` (zona congelada): `app/page.tsx` usa `cookies()` de `next/headers` con nombre literal `'bitacora_session'` hardcoded (espejo de `lib/auth/constants.ts`).
- El `privacyNote` (`"Solo vos ves lo que registrás. Tus datos son privados."`) se conserva en ambas variants como soporte constante.

### ConsentGatePanel — CTA secundario `"Ahora no"` y revocabilidad visible

- CTA secundario `"Ahora no"` agregado al `decisionBar` (`ConsentGatePanel.tsx:101-108`). Ofrece salida respetuosa sin expresión de causa. Ley 26.529 Art. 2 inc. e) autonomía.
- `handleDecline` redirige a `/?declined=1` SIN borrar cookie. La sesión sigue activa; el paciente puede volver a aceptar sin re-autenticar OIDC.
- `revocationNote` `"Podés revocarlo cuando quieras desde Mi cuenta."` agregado cerca del `decisionBar` (`ConsentGatePanel.tsx:97-99`). Ley 26.529 Art. 10 revocabilidad.
- `inviteHint` reubicado DESPUÉS del `.sections` block (`ConsentGatePanel.tsx:69-73`): resuelve la anti-señal "frame de supervisión profesional precede al contenido del consent" (E2-F1 del audit 2026-04-23).

### Landing post-decline — feedback sereno canon 13

- `OnboardingEntryHero` gana prop `message?: string` renderizada como `<p className={styles.heroMessage} role="status" aria-live="polite">`. `app/page.tsx` emite el mensaje `"Podés aceptar cuando quieras. Tu sesión sigue activa."` cuando el query param `declined=1` está presente.
- El mensaje es factual, no dramatiza, no regaña. Canon 13 §voz sereno en momentos sensibles.

### Legal-review R-P1-3

- Review interno pragmatic documentado en `.docs/raw/decisiones/2026-04-23-legal-review-r-p1-3.md` contra Ley 26.529 (Art. 2, 5, 10), Ley 25.326 y Ley 26.657. Verdict: `resuelto-sin-cambios-pending-formal-legal-opinion` (sin red flags hard; red flags soft como follow-up).

### Notas de implementación

- Todos los cambios `ui-only, no-schema, no-contract, no-auth`.
- Copy congelado preservado: `"Ingresar"`, `"Tu espacio personal de registro"`, `"Solo vos ves lo que registrás. Tus datos son privados."`, `"Aceptar y continuar"`, `"Ahora no"` (nuevo canon 2026-04-23).
- Zonas congeladas: `lib/auth/*`, `app/api/*`, `app/auth/*`, `proxy.ts`, `src/*` sin cambios.

---

**Estado:** `UXS` activo para `ONB-001`. Cubre S01 (incluyendo variant returning), S02 y S03 (con CTA secundario y revocabilidad visible). S04-BRIDGE deprecado 2026-04-22; reemplazado por `S05-DASHBOARD-EMPTY`.
**Precedencia:** depende de `UXR`, `UXI`, `UJ`, `VOICE` y `FL/RF` del onboarding real.
**Siguiente capa gobernada:** `../PROTOTYPE/PROTOTYPE-ONB-001.md`, `../UI-RFC/UI-RFC-ONB-001.md` y la cadena `HANDOFF-*`.
