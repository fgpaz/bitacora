<!--
target_platform: claude-code
pressure: aggressive
generated_at: 2026-04-23
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-23-login-flow-redesign.md' | Set-Clipboard"
-->

# MISIÓN — Implementar el rediseño del flujo de acceso de Bitácora (P0 + P1) sobre el audit 2026-04-23

Operás en `C:\repos\mios\humor`. El audit read-only del 2026-04-23 terminó y cerró con commit `28b02aa` en la rama `audit/login-flow-2026-04-23`. El veredicto es `needs-redesign` para el dashboard como hub de retorno y `needs-refinement` para el resto del flujo. El product owner fue literal: **la UX de login es malísima para un recurrente y no queda claro cómo registra algo nuevo**. Tu trabajo es ejecutar los fixes P0 + P1 del audit en una rama de feature separada, en waves con checkpoints, sin reabrir decisiones cerradas ni tocar zonas congeladas.

No confíes en memoria de conversaciones previas. Verificá repo state antes de creer nada de este prompt.

## Punto de partida

- Base del fix: `main` (HEAD `0bcb11e`). **No** partir de `audit/login-flow-2026-04-23` — esa rama queda read-only para review asíncrono del humano.
- Rama de trabajo sugerida: `feature/login-flow-redesign-2026-04-23` (si surge conflicto con convenciones existentes, adaptá sin pelear con el humano).
- Artefactos canónicos para esta corrida (leer primero, no opinar sin ellos):
  - `.docs/raw/reports/2026-04-23-login-flow-audit.md` — reporte final, 36 hallazgos priorizados, tabla de recomendaciones mapeadas a skills impeccable-*, follow-ups explícitos. Lo trabajás con este documento en la mano.
  - `.docs/raw/reports/2026-04-23-login-flow-baseline.md` — baseline consolidado (output de `impeccable-audit`), evidencia file:line de cada finding.
  - `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md` — closure del hardening previo; 12 ítems vigentes que debés preservar (modal a11y completo, empty state sin vigilancia, focus return, aria-modal, etc.).
  - `.docs/raw/prompts/2026-04-23-login-flow-ux-audit.md` — prompt que generó el audit; define zonas congeladas + invariantes.

## Scope obligatorio (waves)

El audit propuso ejecutar en 4 waves. Respetá el orden. Checkpoint humano al final de cada wave (pausa + resumen + espera de OK antes de avanzar).

### Wave 1 — Blockers pre-merge (P0)
1. **R-P0-1** Subir `"+ Nuevo registro"` al top del dashboard en `ready` → skill **`impeccable-distill`**. Target: `Dashboard.tsx:292-307` + `Dashboard.module.css`. Recomendación del audit: rail de acción personal arriba de DashboardSummary con heading tipo `"Hoy"` + CTA grande `"Registrar humor"` (alinea con copy congelado y resuelve inconsistencia E3-F8 en la misma pasada). No tocar empty state — ya respeta el patrón.
2. **R-P0-2** Proteger logout de taps accidentales → skill **`impeccable-harden`**. Target: `PatientPageShell.tsx:41-49` + `.module.css:62`. Dos opciones cerradas por el audit: (a) overflow menu con ítem `"Cerrar sesión"`, o (b) confirmación inline `"¿Cerrar tu sesión?" → "Sí, cerrar" / "Cancelar"`. (a) está más alineada con canon 12 §"pocas decisiones visibles". Elegir (a) por default salvo que el brainstorming cierre con (b).
3. **R-P0-3** Crear `frontend/app/global-error.tsx` → skill **`impeccable-harden`**. Contrato Next.js: `'use client'` + wrapper `<html><body>...</body></html>` + wordmark "Bitácora" + título canon 13 (`"No pudimos cargar el sitio"`) + sub concreto + CTA `"Recargar"` con `reset()`.
4. **R-P0-4** Cerrar modal post-success + puente de siguiente acción → skill **`impeccable-clarify`** + **`impeccable-harden`**. Target: `Dashboard.tsx:111` (`handleEntrySaved` debe esperar ~800ms y llamar `closeDialog()`) + `MoodEntryForm.tsx:82-87` (puente embedded) + toast `aria-live="polite"` en dashboard con `"Registro sumado a tu historial."`.

### Wave 2 — P1 onboarding + continuidad + consent
5. **R-P1-1** Continuidad del recurrente en landing (E1-F2 + E1-F5) → skill **`impeccable-onboard`**. Opción A: crear `frontend/middleware.ts` que redirija `/` → `/dashboard` con cookie viva. Opción B (UI-only si el equipo auth bloquea A): variante `variant="returning"` del hero con `<h1>"Volviste"</h1>` + CTA "Seguir registrando". **Requiere handoff explícito al owner de `lib/auth/*` antes de tocar middleware**.
6. **R-P1-2** Reubicar inviteHint en consent + inviteLabel en hero → skill **`impeccable-onboard`**. `ConsentGatePanel.tsx:53-55` mover después de `.sections`. `OnboardingEntryHero.tsx:17-27` mover inviteLabel a subtítulo bajo h1.
7. **R-P1-3** CTA de rechazo en consent (E2-F2) ⚠ **legal-review obligatorio antes de arrancar** → skill **`impeccable-harden`**. `ConsentGatePanel.tsx:82-93`. Mensaje sereno al rechazar + redirect sin borrar sesión.
8. **R-P1-4** Revocabilidad visible en consent (E2-F3) → skill **`impeccable-clarify`**. Texto breve en `ConsentGatePanel.tsx` cerca del decisionBar.
9. **R-P1-8** Heading recurrente del dashboard (E3-F2 + E3-F7) 🔒 **decisión humana pendiente antes de arrancar** → skill **`impeccable-onboard`**. `app/(patient)/dashboard/page.tsx:22`. Las 3 opciones están cerradas en el audit §4.2; recomendado (c) saludo contextual con subtítulo de privacidad.

### Wave 3 — P1 dashboard distill + harden
10. **R-P1-5** Rediseñar DashboardSummary en ready (E3-F3 + E3-F4) → skill **`impeccable-distill`**. Tres caminos ordenados en el audit; elegir (b) texto corrido o (c) absorber en rail de acción.
11. **R-P1-6** Acceso visible a configuración (E3-F5 + E3-F12) → skill **`impeccable-onboard`** + **`impeccable-clarify`**. Nav mínima persistente en shell o sección `.settings` en dashboard.
12. **R-P1-7** Focus ring canonizado en CTAs de entry points (E1-F3 + revisión sistemática) → skill **`impeccable-normalize`**.
13. **R-P1-9** trendChart colapsable en 360px (E3-F9) → skill **`impeccable-adapt`**. Breakpoint ≤400px: limitar a 5 entradas o resumen textual.
14. **R-P1-10** Mensajes de error concretos + filtro de `err.message` (E5-F1 + E5-F7 + E5-F10) → skill **`impeccable-harden`** + **`impeccable-clarify`**. Crear `frontend/lib/errors/user-facing.ts` con `formatUserFacingError(err)`. Tipar prop `error` de `PatientPageShell` como `UserFacingError`. Reemplazar sub de `app/error.tsx:16`.

### Wave 4 — Tests + cierre
15. Ajustar E2E (`dashboard-modal.spec.ts`, posibles nuevos specs para global-error + logout confirmation). 8/8 verdes obligatorio.
16. typecheck + lint exit 0.
17. `Skill("ps-trazabilidad")` final.
18. `Skill("ps-auditar-trazabilidad")` full — cross-check del plan, waves, commits y sync canon.
19. Opcional (solo si surgieron cambios en UX/UI del canon 23_uxui): generar `UJ-ONB-*` + `UXS-ONB-*` para el flujo login+dashboard+modal (el audit §6 lo marca como gap de traceability). NO bloquear cierre por esto — documentar como follow-up si no hay tiempo.

## Invariantes protegidos (violación = abort)

- **Zonas congeladas — NO TOCAR bajo ninguna circunstancia**: `frontend/lib/auth/*`, `frontend/app/api/*`, `frontend/app/auth/*`, `frontend/proxy.ts`, `frontend/src/**`. Si R-P1-1 Opción A exige middleware, ese es EL ÚNICO caso donde podés proponer tocar una zona auth-adjacente, y solo previo OK humano explícito.
- **Copy congelado 2026-04-22 es ley** salvo decisión explícita del humano tomada en el brainstorming: "Ingresar", "Tu espacio personal de registro", "Solo vos ves lo que registrás. Tus datos son privados.", "Registrar humor", "Empezá con tu primer registro", "+ Nuevo registro", "Check-in diario", "Registro guardado.", "Check-in guardado.", "Tus últimos días", "Recibí recordatorios por Telegram", "Conectar", "Ahora no", "Nuevo registro".
- **Compliance salud mental**: Ley 25.326 / 26.529 / 26.657. R-P1-3 requiere legal-review PREVIO. Cualquier cambio que toque consent, audit log, retención o exposición de datos debe marcarse explícito y, si el brainstorming lo considera sensible, pausarse para review humano.
- **Dependencias**: NO agregar dependencias npm nuevas. El stack actual tiene 5 prod deps y eso es una ventaja del producto. Si creés que hace falta una lib, justificá explícitamente y marcá como P1 para decisión humana — no instales nada por tu cuenta.
- **Git hygiene**: NO `taskkill`, NO `--no-verify`, NO `--force push`, NO `--amend` sobre commits empujados, NO mergear a `main` sin OK humano. Cada wave = 1 o más commits atómicos. Mensajes con trailer `- Gabriel Paz -`.
- **12 positivos vigentes del closure 2026-04-22 deben preservarse** (listados en audit §6): `aria-modal`, `role=dialog` nativo, focus return via `triggerRef + rAF`, `role=radiogroup/radio/aria-checked` en MoodScale, `aria-busy` en submits, `role=status aria-live=polite` en success blocks, `aria-pressed` en DailyCheckin boolean buttons, `aria-required` en sleep_hours, `InlineFeedback` sin conflicto role+aria-live, `prefers-reduced-motion` local, DashboardSummary oculto en empty, tokens `--foreground-muted #4A4440` + radios 4/8/12.

## Paso 0 — Anclaje + verificación (obligatorio antes de cualquier acción)

1. `Skill("ps-contexto")` — scope, arquitectura, flujos, modelo de datos, canon UX/UI 10-23. No negocies.
2. Leé con Read directo, no te confíes del ps-contexto:
   - `.docs/raw/reports/2026-04-23-login-flow-audit.md` — documento maestro de esta corrida.
   - `.docs/raw/reports/2026-04-23-login-flow-baseline.md` — evidencia file:line.
   - `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md` — 12 ítems vigentes.
   - `.docs/wiki/10_manifiesto_marca_experiencia.md` §10.1, §12.2 (dashboards de vigilancia).
   - `.docs/wiki/12_lineamientos_interfaz_visual.md` §Jerarquía de CTA, §Densidad.
   - `.docs/wiki/13_voz_tono.md` §Errores, §Reglas para estados sensibles.
   - `.docs/wiki/16_patrones_ui.md` #3 Gesto rápido, #7 Error recuperable, #12 Puente de siguiente acción.
   - `.docs/wiki/04_RF/RF-ONB-003.md`, `RF-ONB-004.md`, `RF-ONB-005.md` (hard gate + primer registro + transición a active).
   - `frontend/styles/tokens.css` — paleta y tokens reales.
   - `AGENTS.md`, `CLAUDE.md` — política operativa.
3. Validá workspace: `mi-lsp workspace list` → alias `bitacora`. Si `mi-lsp` devuelve `hint` o `next_hint`, seguilos antes de avanzar. Es tu herramienta principal de navegación para todo lo que cae bajo `frontend/`.
4. Creá la rama: `git checkout main && git pull && git checkout -b feature/login-flow-redesign-2026-04-23`.

## Paso 1 — Exploración obligatoria (3 `ps-explorer` mínimos, 5 si es necesario)

Despachá en UN SOLO mensaje, no secuencial. El scope es multi-surface (dashboard + shell + modal + consent + landing + global-error), por eso 5 es el piso probable.

**Explorer 1 — Dashboard + shell actual (reality check post-audit)**
Verificá que el código actual matchee lo reportado en el baseline. Diff-proofing: para cada finding P0 del audit, buscar con file:line y confirmar vigencia. Si algo ya fue arreglado en commits entre 2026-04-22 y hoy, marcarlo. Archivos: `Dashboard.tsx`, `Dashboard.module.css`, `DashboardSummary.tsx/.module.css`, `PatientPageShell.tsx/.module.css`.

**Explorer 2 — Modal + forms actual (reality check)**
Verificar los findings P0/P1 del modal: trigger, aria-modal, focus return, handleEntrySaved, closeDialog, closeBtn size, puente success embedded. Confirmar los 12 positivos vigentes del closure. Archivos: `MoodEntryDialog.tsx/.module.css`, `MoodEntryForm.tsx/.module.css`, `MoodScale.tsx/.module.css`, `DailyCheckinForm.tsx`.

**Explorer 3 — Consent + onboarding + edge actual**
Verificar posición del inviteHint, estructura del decisionBar, ausencia de `global-error.tsx`, regresión del sub en `app/error.tsx`, formato de errores en `OnboardingFlow.tsx:85`, `PatientPageShell.tsx:27-33` con prop error sin tipar. Archivos: `OnboardingFlow.tsx`, `OnboardingEntryHero.tsx/.module.css`, `ConsentGatePanel.tsx/.module.css`, `AuthBootstrapInterstitial.tsx`, `app/error.tsx`, `app/not-found.tsx`, `VinculosManager.tsx`.

**Explorer 4 — Tokens + tests + routes (condicional: lanzar si Wave 1-3 parecen cruzar 10+ archivos)**
Mapear CSS modules que usan `:focus-visible` vs `:focus`, inventario de `var(--focus-ring)` en frontend, lista de Playwright specs afectadas por los cambios previstos, rutas del App Router relevantes. Archivos: `frontend/styles/tokens.css`, `frontend/e2e/*.spec.ts`, tree de `frontend/app/`.

**Explorer 5 — Middleware feasibility (condicional: lanzar solo si humano autoriza Opción A para R-P1-1)**
Sin TOCAR `lib/auth/*`: mapear qué cookies/headers están observables desde un hipotético `frontend/middleware.ts` + qué patrón usan otros proyectos del ecosistema (buho, turismo, salud) si sus repos tienen middleware similar. Reportar viabilidad y riesgos.

Cada explorer devuelve: findings con file:line, copy literal entre comillas, máximo 10 puntos. Si dos explorers se contradicen, despachá un sexto focalizado. No integres sin cross-check.

## Paso 2 — Brainstorming (obligatorio antes de writing-plans)

`Skill("brainstorming")`. Usá `AskUserQuestion` para cerrar al menos estas 4 decisiones críticas — con el protocolo completo (contexto de aprendizaje + impacto aplicado + diagrama ASCII + opciones pros/cons + recomendación):

1. **🔒 E2-F2 CTA rechazo del consent**: ¿agregamos `"Ahora no"` con redirect sin borrar sesión, mantenemos hard gate puro, o pausamos hasta review legal formal? (Recomendado: agregar con salida respetuosa + flaggear para review legal en paralelo.)
2. **🔒 E3-F7 h1 del dashboard**: ¿actualizar canon al h1 actual `"Mi historial"`, migrar h1 al copy congelado `"Tu espacio personal de registro"`, o introducir saludo contextual tipo `"Hola. Acá está lo que registraste."` con subtítulo `"Solo vos ves lo que registrás. Tus datos son privados."`? (Recomendado: saludo contextual + subtítulo que absorbe el congelado.)
3. **R-P0-2 logout**: ¿overflow menu `"⋯"` con ítem `"Cerrar sesión"` (recomendado) o confirmación inline?
4. **R-P1-1 continuidad recurrente**: ¿middleware server-side (requiere handoff auth) o variante UI-only del hero? (Decisión depende de si el equipo auth está disponible.)

Cualquier decisión sensible adicional debe marcarse `🔒 needs-human-decision` y pausarse.

## Paso 3 — Plan wave-dispatchable

`Skill("writing-plans")`. Persistir a `.docs/raw/plans/2026-04-23-login-flow-redesign.md` + subdocumentos por wave en `.docs/raw/plans/2026-04-23-login-flow-redesign/W1-blockers.md`, `W2-onboard-consent.md`, `W3-distill-harden.md`, `W4-tests-closure.md`.

Cada wave en el plan debe tener:
- Objetivo único claro.
- Lista de archivos a tocar (max 10 por wave salvo normalize que puede ser más).
- Decisiones ya cerradas en el brainstorming.
- Checkpoint de verificación (typecheck/lint/e2e/grep de zonas congeladas).
- Mensaje de commit propuesto con trailer.

Patrón de referencia: las 11 waves del plan `.docs/raw/plans/2026-04-22-impeccable-hardening.md` funcionaron. Imitá el shape.

## Paso 4 — Ejecución por wave

Cada wave:
1. Mostrar al humano el diff propuesto antes de aplicar cambios grandes (>5 archivos).
2. Dispatch de subagentes cuando la wave se pueda paralelizar:
   - `ps-next-vercel` para código Next.js / React (principal).
   - `ps-worker` para ajustes de config, scripts, y operaciones de git.
   - `ps-code-reviewer` al final de cada wave para audit P>D>S.
3. Correr tests relevantes: typecheck + lint + e2e subset afectado.
4. Grep final de zonas congeladas (0 cruces obligatorio): `lib/auth/|app/api/|app/auth/|proxy\.ts|^frontend/src/`.
5. Commit atómico con trailer `- Gabriel Paz -`.
6. `Skill("ps-trazabilidad")` parcial (diff vs canon; aceptable tener drift documentado entre waves).
7. **CHECKPOINT humano**: pausa + resumen de wave + espera OK antes de continuar.

## Paso 5 — Cierre

1. `Skill("ps-trazabilidad")` final — full sync RF/FL/canon/tests.
2. `Skill("ps-auditar-trazabilidad")` full — cross-check baseline + plan + commits + canon.
3. typecheck + lint + 8/8 e2e verdes OBLIGATORIO.
4. Escribir reporte de closure: `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md` con el mismo shape que el closure 2026-04-22 (cadena de commits, checkpoints, verdict, follow-ups, deuda aceptada).
5. **NO mergear a main**. La rama queda para PR humano, igual que el hardening 2026-04-22.
6. Devolver al humano: path del closure, conteo de commits, verdict, próxima acción.

## Reglas de operación

- **Orchestrator mode siempre**. Exploración + implementación por wave van en subagentes (`ps-explorer`, `ps-next-vercel`, `ps-worker`, `ps-code-reviewer`). El hilo principal sintetiza, decide y commitea.
- **Cross-check ≥ 2 fuentes** para cualquier finding antes de tocar código.
- **Evidencia o no existe**. Si un subagent reporta sin file:line, pedilo de vuelta o descartá el hallazgo.
- **Si dos skills impeccable-* dicen lo contrario** sobre un mismo elemento, el orden que gana: `harden > clarify > critique > normalize > onboard > delight > polish`. Documentá la decisión.
- **Timeboxing flexible**. Si una wave se estira, cortá y pedí OK humano para splittear.
- **Si algo queda inconcluso**, closure honesto con "no validado: X, Y" > inventar verdicts.
- **Si un fix P0 destapa algo mayor** no documentado en el audit, NO lo absorbas: reportá, abrí issue/card, seguí con el scope original. Lo que no está en el audit no está en esta corrida.

## Referencias

- `CLAUDE.md` (raíz) — política operativa completa.
- `AGENTS.md` (raíz) — catálogo de subagents.
- `.docs/raw/reports/2026-04-23-login-flow-audit.md` — documento maestro (LEER PRIMERO).
- `.docs/raw/reports/2026-04-23-login-flow-baseline.md` — evidencia file:line.
- `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md` — patrón de referencia de 11 waves.
- `.docs/raw/plans/2026-04-22-impeccable-hardening.md` — shape de plan wave-dispatchable.
- `.docs/raw/reports/2026-04-22-impeccable-audit-baseline.md`, `2026-04-22-impeccable-critique.md` — decisiones ya cerradas que no se reabren.
- `frontend/styles/tokens.css` — fuente de verdad de tokens.
- `frontend/e2e/*.spec.ts` — contratos e2e vigentes.
- `.docs/wiki/10-16` — canon UX/UI endurecido.
- `.docs/wiki/04_RF/RF-ONB-003/004/005.md` — RF del onboarding.

## Última palabra

Este no es un refactor cosmético. Es la respuesta a una queja concreta del product owner sobre una UX de salud mental que está fallando para el usuario nuclear (recurrente en mobile, cansado, a las 23:47). Cada wave que empujás a producción debe mejorar medible ese caso. Si al final del rediseño el recurrente sigue sin ver el CTA de registro cuando abre el dashboard, fallaste — por más que los tests e2e estén verdes.

Miralo con ojos de paciente en vulnerabilidad. No con ojos de developer que sabe dónde está todo. El manifesto 10 no es decoración: es un contrato con el usuario al que le cambiaron la vida con una ley de salud mental.

Empezá ya. Paso 0 primero. Nada de preámbulos.
