# Decisión: Dashboard-first post-login (eliminar Bridge Card)

**Fecha:** 2026-04-22
**Estado:** Aprobada por el product owner
**Autor:** Gabriel Paz (Bitácora)

## Contexto

QA en producción (`bitacora.nuestrascuentitas.com`) detectó tres fricciones de
UX en el flujo de ingreso y primer uso:

1. **Landing con email engañoso.** La home pedía un correo con label
   `"Correo (opcional, pre-completa el login)"` pero lo validaba como requerido
   y mostraba un estado transitorio `"Revisá tu correo"` que no reflejaba la
   realidad: el sistema no envía ningún correo; sólo redirige a Zitadel con
   `login_hint`. En un producto de salud mental, esa promesa falsa erosiona
   confianza.
2. **Onboarding que mentía sobre Telegram.** `NextActionBridgeCard`
   (S04-BRIDGE) siempre mostraba `"Vincular Telegram (opcional)"` aunque el
   paciente ya hubiese vinculado Telegram. Además, la condición
   `needsFirstEntry` se pasaba como `bootstrapData?.needsConsent === false`
   — inversión que dejaba la rama "Ya puedes usar Bitacora" inalcanzable.
3. **Camino post-onboarding incierto.** Tras consent, la Bridge Card ofrecía
   dos links manuales sin CTA "Ir al dashboard". El paciente quedaba varado
   o entraba en loop navegando entre `/onboarding`, `/registro/mood-entry`
   y `/configuracion/telegram`.

Paralelamente, `/dashboard` y `/registro/mood-entry` estaban desacoplados:
cada nuevo registro sacaba al paciente de su historia.

## Decisión

### 1. Landing: un único CTA "Ingresar"
- Eliminar el campo email y todo el estado de magic-link simulado.
- `OnboardingEntryHero` queda con un único `Link` primario `Ingresar` que
  apunta a `/ingresar` (inicio del OIDC + PKCE real).
- Deprecar `signInWithMagicLink`; el único entrypoint de login es
  `signInWithZitadel`.

### 2. Post-login: ir siempre a `/dashboard`
- `/auth/callback/route.ts` ya redirige a `/dashboard`: se mantiene.
- `OnboardingFlow` sólo se ejecuta cuando realmente falta consent. Si
  `!needsConsent` o tras aceptar consent, hace `window.location.assign('/dashboard')`.
- Se elimina `NextActionBridgeCard.tsx` y la fase `S04-BRIDGE`.

### 3. Dashboard como hogar unificado
- Nuevo `MoodEntryDialog` (modal nativo `<dialog>`) que embebe
  `MoodEntryForm` con `embedded onSaved={refresh}`. El paciente no pierde
  contexto del historial al cargar un nuevo registro.
- Nuevo `TelegramReminderBanner`: consulta `getTelegramSession()`. Sólo
  visible si `session.linked === false`. Dismissible por 30 días con
  `localStorage['bitacora.telegram.banner.dismissedAt']`.
- El banner reemplaza el CTA perenne "Vincular Telegram (opcional)" de la
  Bridge Card, y respeta el estado real del backend.

### 4. Copy alineado a voz y tono
- Eliminadas las palabras `"magic"`, `"Revisá tu correo"` y el paréntesis
  `"(opcional)"` cuando no aporta información real.
- Empty state del dashboard: `"Empezá con tu primer registro"` en lugar de
  `"Todavía no hay registros"` (voz activa, centrada en la acción del paciente).

## Alternativas consideradas

- **Mantener la Bridge Card corrigiendo los bugs** (chequear Telegram,
  arreglar `needsFirstEntry`, sumar link al dashboard). Descartado: la
  Bridge Card es una pantalla intermedia sin información nueva, añade un
  paso sin valor.
- **Redirect condicional server-side** (callback consulta bootstrap +
  timeline + telegram antes de decidir destino). Descartado por ahora:
  requiere 2-3 calls adicionales en el callback y complica el retry/fallback.
  El dashboard ya maneja empty state y consent-required correctamente.
- **Tabs "Historial / Nuevo" en el dashboard**. Descartado: overkill para el
  primer rediseño; el modal mantiene contexto sin cambio estructural.

## Consecuencias

### Positivas
- Menos fricción: el paciente logueado siempre ve su historial.
- Copy honesto: ningún flujo promete correos que no envía.
- El banner de Telegram respeta el estado real del backend.
- Menos superficies UI para mantener (se elimina `NextActionBridgeCard.*`,
  se simplifica `OnboardingEntryHero`).

### Negativas / a observar
- Pérdida del pre-fill de email en Zitadel. Observable: ¿aumenta la tasa de
  usuarios que escriben mal su correo en Zitadel? Métrica a trackear.
- La Bridge Card era el recordatorio principal para vincular Telegram post-
  consent. Ahora depende del banner en el dashboard. Si la tasa de vinculación
  baja, reforzar el banner (e.g. persistirlo durante los primeros 7 días sin
  dismiss).
- Canon wiki (`23_uxui/UXS/UXS-ONB-001.md`, `UI-RFC/UI-RFC-ONB-001.md`,
  `HANDOFF-*/ONB-001`, `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`, `TP-ONB.md`,
  `TP-VIS.md`) necesita sync completo. Ver prompt de seguimiento
  `.docs/raw/prompts/2026-04-22-sync-canon-ux-dashboard-first.md`.

## Archivos impactados

### Código frontend
- Modificados: `frontend/app/page.tsx`,
  `frontend/components/patient/onboarding/OnboardingEntryHero.tsx` (+css),
  `frontend/components/patient/onboarding/OnboardingFlow.tsx`,
  `frontend/components/patient/mood/MoodEntryForm.tsx`,
  `frontend/components/patient/dashboard/Dashboard.tsx`,
  `frontend/lib/auth/client.ts`.
- Nuevos: `frontend/components/patient/dashboard/MoodEntryDialog.tsx` (+css),
  `frontend/components/patient/dashboard/TelegramReminderBanner.tsx` (+css).
- Eliminados:
  `frontend/components/patient/onboarding/NextActionBridgeCard.tsx` (+css).

### Tests
- Eliminado `frontend/e2e/login.spec.ts` (obsoleto).
- Nuevos: `frontend/e2e/landing.spec.ts`,
  `frontend/e2e/dashboard-modal.spec.ts`,
  `frontend/e2e/telegram-banner.spec.ts`.

### Canon (actualizado en esta entrega)
- `.docs/wiki/03_FL/FL-ONB-01.md`.

### Canon (pendiente, ver prompt de seguimiento)
- `.docs/wiki/23_uxui/UXS/UXS-ONB-001.md`.
- `.docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md`.
- `.docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-ONB-001.md`,
  `HANDOFF-VISUAL-QA-ONB-001.md`, `HANDOFF-MAPPING-ONB-001.md`,
  `HANDOFF-ASSETS-ONB-001.md`.
- `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-ONB-001.md`,
  `.docs/wiki/23_uxui/VOICE/VOICE-ONB-001.md`.
- `.docs/wiki/06_pruebas/TP-ONB.md`, `.docs/wiki/06_pruebas/TP-VIS.md`.
- `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`,
  `.docs/wiki/07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md`.
- `.docs/wiki/04_RF.md` (índice) y nuevo
  `.docs/wiki/04_RF/RF-VIS-015-dashboard-paciente-inline-entry.md`.

## Compliance salud (Leyes 25.326 / 26.529 / 26.657)
No se modifican storage, access control, consent flows ni audit logging. El
cambio es puramente de UI del paciente. El consent sigue siendo un hard gate
antes de cualquier lectura/escritura clínica (ver `FL-ONB-01` y `FL-CON-01`).
