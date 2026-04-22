<!--
target_platform: claude-code
pressure: aggressive
generated_at: 2026-04-22
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-22-impeccable-audit-hardening-frontend.md' | Set-Clipboard"
-->

# Bootstrap: Auditoría UX/UI + hardening completo del frontend de Bitácora con las skills `impeccable-*`

Sos un orquestador en Claude Code trabajando sobre `C:\repos\mios\humor` (repo `humor`, workspace `mi-lsp: bitacora`). Rama base: `main` (HEAD `1c1ac50` — post-deploy del plan "dashboard-first" del 2026-04-22). Compliance salud Ley 25.326 / 26.529 / 26.657 aplica: el cambio es UI-only.

Tu misión: **auditar y hardenizar el frontend entero** (`frontend/`) ejecutando en cadena **TODAS las skills `impeccable-*`** para mejorar absolutamente todo lo que se pueda sin tocar dominio ni seguridad. El objetivo es dejar la UI viva en `bitacora.nuestrascuentitas.com` con la mejor calidad posible en accesibilidad, performance, theming, responsive, microcopy, jerarquía visual, tokens del DS y extracción de componentes. Nada de rescribir dominio ni auth.

No inventes scope. Si algo no coincide con el canon o con el repo real, **parás y reportás**. No reabras decisiones congeladas abajo.

## Decisiones congeladas (NO reabrir)

- **Landing 1-CTA:** label exacto `Ingresar`, destino `/ingresar` (OIDC+PKCE server-side). Ningún campo de email, ningún estado de magic link. Copy: `Tu espacio personal de registro`, privacidad, soporte.
- **Dashboard post-login:** empty state con CTA `Registrar humor` que abre `MoodEntryDialog`; ready state con `+ Nuevo registro` que abre el mismo modal y `Check-in diario` que navega a `/registro/daily-checkin`. Título del modal: `Nuevo registro`. Empty copy: `Empezá con tu primer registro`.
- **Banner Telegram:** copy exacto `Recibí recordatorios por Telegram`, CTA `Conectar` a `/configuracion/telegram`, botón `Ahora no` que persiste 30 días con `localStorage['bitacora.telegram.banner.dismissedAt']`.
- **Onboarding:** `OnboardingFlow` es gate de consent puro. Sin fase `S04-BRIDGE`, sin `NextActionBridgeCard`. Redirección `window.location.assign('/dashboard')` en ambas ramas.
- **Next 16 middleware:** vive en `frontend/proxy.ts` con `export function proxy`. No renombrar, no mover a `middleware.ts`.
- **Auth:** server-side en `frontend/lib/auth/server.ts` + `/app/auth/*`. Cookie `bitacora_session` httpOnly. **No tocar** fuera de cleanup puramente de tipo.
- **API contracts:** `frontend/lib/api/client.ts` tiene shape congelado. Solo cleanup de campos huérfanos confirmados con `grep` en `src/` y frontend.
- **Idioma:** español con tildes y signos de apertura (`¿`, `¡`). Cero emojis en código, copy y docs.
- **Cross-platform:** ningún fix puede introducir paths Linux-only hardcodeados (ver lección `playwright.config.ts`). Condicionar por `process.platform` cuando aplique.
- **Tests:** Playwright 1.59 con 8 specs pasando. Mantener verde durante todo el trabajo.

## Invariantes de proceso

- **NUNCA** correr `taskkill //F //IM node.exe` ni matar PIDs de dev servers del usuario. Si necesitás un entorno limpio, pedí autorización con `AskUserQuestion` o usá puerto alterno con `BASE_URL`. Ver `feedback_never_taskkill_node.md` en memoria.
- **NUNCA** usar `--no-verify`, `--no-gpg-sign`, `--amend` sobre commits empujados, ni `git push --force` a `main`.
- **NUNCA** agregar dependencias npm nuevas. Si alguna skill `impeccable-*` sugiere una librería, priorizá la alternativa nativa o pedí OK explícito antes de instalar.
- **NUNCA** tocar `frontend/app/auth/callback/route.ts`, `frontend/lib/auth/server.ts`, `frontend/proxy.ts` (core) ni endpoints del backend en `src/`.
- **NUNCA** cambiar el copy congelado de arriba.
- Artefactos efímeros (`test-results/`, `tmp/`) van a `tmp/` o `artifacts/` según §9.2 de `CLAUDE.md`, y se limpian antes del cierre.

## Paso 0 — Verificar estado del repo

Antes de cualquier skill o subagente, confirmá:

```bash
git rev-parse HEAD          # debería arrancar en 1c1ac50 o más nuevo sobre main
git status --short          # debe estar limpio
git log --oneline -5 main   # verificar que los commits del plan dashboard-first están en main
```

Si algo no matchea, **stop y reportá** antes de avanzar.

## Paso 1 — Contexto + governance (obligatorio)

1. `Skill("ps-contexto")`. Cuando pida wiki path, respondé `.docs/wiki`. Leé scope, arquitectura, flujos, requerimientos, modelo de datos, y el canon UX/UI hardened completo:
   - `.docs/wiki/10_manifiesto_marca_experiencia.md`
   - `.docs/wiki/11_identidad_visual.md`
   - `.docs/wiki/12_lineamientos_interfaz_visual.md`
   - `.docs/wiki/13_voz_tono.md`
   - `.docs/wiki/14_metodo_prototipado_validacion_ux.md`
   - `.docs/wiki/15_handoff_operacional_uxui.md`
   - `.docs/wiki/16_patrones_ui.md`
   - `.docs/wiki/17_UXR.md` … `.docs/wiki/22_aprendizaje_ux_ui_spec_driven.md`
   - `.docs/wiki/23_uxui/INDEX.md` y los slices activos (`ONB-001`, `VIS-001/015`, `REG-*`).
2. Ejecutar:
   ```bash
   mi-lsp workspace status bitacora --format toon
   mi-lsp nav governance --workspace bitacora --format toon
   ```
   Confirmá `sync: in_sync`, `blocked: false`. Si está bloqueado, **stop** y derivá a `crear-gobierno-documental`.
3. Leé la decisión anchor: `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.
4. Leé `CLAUDE.md` §0 (skill invocation), §1 (orchestrator), §7 (routing de subagentes), §9 (idioma y hygiene), §12 (runtime tooling).

## Paso 2 — Exploración paralela obligatoria (mínimo 3 `ps-explorer`)

En **un solo mensaje con múltiples tool calls** dispará 3 `ps-explorer` a la vez. Sumá explorer 4 y 5 si:
- aparecen contradicciones entre lo que dice el canon y el código real,
- hallazgos iniciales sugieren drift infra (tokens, responsive breakpoints, a11y),
- o un slice del canon referencia componentes que no existen.

**Explorer A — Estado visible del frontend vs. canon UX/UI**
Alcance: `frontend/app/**/*.tsx`, `frontend/components/**/*.tsx`, `frontend/app/globals.css`, CSS Modules de componentes.
Objetivo: devolver un mapa `componente → archivo → copy visible → tokens usados → estados manejados (loading/error/empty/ready)`. Resaltar divergencias respecto al canon (CTA, jerarquía, estados faltantes, microcopy fuera de la voz).

**Explorer B — Accesibilidad, ARIA y semántica**
Alcance: mismo que A + `frontend/e2e/**/*.spec.ts`.
Objetivo: listar por archivo:line violaciones o gaps de WCAG AA: roles ARIA faltantes/incorrectos, `aria-label` vacíos, contraste sospechoso sobre tokens definidos, foco visible (`:focus-visible`), trampas de foco en `<dialog>`, alternativas a `aria-live` para banners, headings fuera de orden, `alt` en imágenes, controles de teclado en modal y banner, `prefers-reduced-motion`.

**Explorer C — Performance, bundle y tokens del design system**
Alcance: `frontend/app/globals.css`, `frontend/app/layout.tsx`, CSS Modules (buscar magic numbers, valores fuera de tokens), imports dinámicos ausentes, uso de `next/image` vs `<img>`, `next/font`, `use client` innecesarios en árboles grandes, re-renders sin memoización que impacten interacción del modal/banner, LCP candidates (hero), `next.config.mjs` (headers, output `standalone`).
Objetivo: devolver una lista priorizada por impacto y esfuerzo (high/med/low), con evidencia `file:line`.

**Explorer D (opcional) — Responsive / mobile-first**
Alcance: CSS Modules + `frontend/app/globals.css`.
Objetivo: verificar que todos los componentes visibles sigan la regla "mobile-first de una columna" del canon, detectar breakpoints inconsistentes, desbordes en pantallas ≤ 360px, tipos fluidos con `clamp()`, mínimos de `min-width` del modal.

**Explorer E (opcional) — Estados faltantes y error paths**
Alcance: `OnboardingFlow`, `Dashboard`, `MoodEntryForm`, `MoodEntryDialog`, `TelegramReminderBanner`, `ConsentGatePanel`.
Objetivo: listar estados no cubiertos por la UI: error de red del banner, `CONSENT_REQUIRED` durante save, save exitoso con toast/inline, `ENCRYPTION_FAILURE`, offline, focus return al cerrar modal. Cruzar con `UXS-ONB-001` y `UI-RFC-ONB-001`.

Regla de conflicto: si dos exploradores contradicen, lanzá una vuelta corta adicional para resolver antes de avanzar.

## Paso 3 — Baseline con `impeccable-audit`

Ejecutá `Skill("impeccable-audit")` sobre el frontend entero usando los hallazgos consolidados de Paso 2 como input. El audit debe cubrir accesibilidad, performance, theming, responsive y a11y. Persistí el reporte en `.docs/raw/reports/2026-04-22-impeccable-audit-baseline.md`.

## Paso 4 — Brainstorming sobre priorización

`Skill("brainstorming")`. Presentá al humano **una** pregunta en formato multiple-choice sobre cómo priorizar el hardening. Opciones sugeridas (orden importa en cada una):

- A. a11y primero → harden → normalize → extract → clarify → optimize → polish → delight (Recomendado por compliance salud y usuarios en vulnerabilidad).
- B. performance primero → optimize → harden → extract → normalize → clarify → polish → delight.
- C. visual/delight primero → colorize → delight → bolder → polish → harden → normalize → clarify → optimize.

Aplicá el protocolo mandatorio: learning ×2, diagrama ASCII, pros/cons, recomendado. Bloqueá el plan con la respuesta del humano antes de `writing-plans`.

## Paso 5 — Plan formal (`writing-plans`)

`Skill("writing-plans")`. Generá un plan con subdocumentos wave-dispatchables en `.docs/raw/plans/2026-04-22-impeccable-hardening/`. Cada sub-wave debe usar **una** skill `impeccable-*` con alcance atómico:

1. `impeccable-audit` (ya corrido como baseline en Paso 3; el plan sólo lo referencia).
2. `impeccable-critique` — evaluación UX y story del flujo completo.
3. `impeccable-harden` — error handling, i18n-ready copy, overflow handling, edge cases.
4. `impeccable-normalize` — alinear al DS (tokens, espaciados, radios, tipografías).
5. `impeccable-extract` — consolidar componentes reutilizables (Banner, Dialog, InlineFeedback, EmptyState genéricos).
6. `impeccable-clarify` — microcopy, errores, labels, instrucciones, aria-labels.
7. `impeccable-optimize` — performance (LCP hero, server components donde sea seguro, memo, image optim, font display).
8. `impeccable-adapt` — responsive pass cross-device, breakpoints consistentes, modal mobile.
9. `impeccable-onboard` — empty states y first-time experience (ya existe empty state; refinar).
10. `impeccable-polish` — alineación, spacing, consistencia fina.
11. `impeccable-delight` — micro-interacciones sutiles respetando `prefers-reduced-motion`.
12. `impeccable-colorize`, `impeccable-bolder`, `impeccable-quieter`, `impeccable-distill` — aplicarlas solo si el brief del humano o el audit las justifican. Caso contrario, marcar como N/A con razón explícita.

Cada sub-wave declara: archivos tocados, invariantes respetados, test impact, doc impact (canon 23_uxui), y un check local (`npm run typecheck && npm run lint && npm run test:e2e`).

Persistí el plan con `Write` (no lo dejes solo en memoria).

## Paso 6 — Ejecución por waves

Para cada sub-wave del plan:

1. Dispatch de la skill `impeccable-*` correspondiente.
2. Si la skill genera diff >5 archivos o toca tokens globales, delegá en `ps-next-vercel` con el subdoc verbatim como prompt. Si el diff es local y puntual, ejecutá vos.
3. Al terminar la skill: `npm run typecheck && npm run lint` (exit 0). Si toca rutas protegidas, corré los 8 specs con `npm run test:e2e` (cookie `bitacora_session` ya inyectada por el helper `frontend/e2e/helpers/session.ts`; dev server debe existir, si falta pedilo al humano con `AskUserQuestion` — no lo arranques vos por la regla de no-taskkill).
4. Si una skill propone cambios al canon UX/UI (23_uxui, 10-22), pasalos por `Skill("ps-asistente-wiki")` para elegir la skill de documentación correcta antes de editar.
5. Commit por sub-wave: `style(impeccable-<skill>): <alcance>`. Cada commit referencia el reporte baseline del Paso 3.
6. Al finalizar cada 3 sub-waves, correr `Skill("ps-trazabilidad")` como checkpoint intermedio y re-verificar `mi-lsp nav governance`.

## Paso 7 — Closure

1. `Skill("ps-trazabilidad")` sobre todo el alcance. Clasificar como `ui-only, no-schema, no-contract`.
2. `Skill("ps-auditar-trazabilidad")` modo `full`. Verdict esperado: `0 critical gaps`.
3. Si quedan warnings, triage entre follow-up commit dentro de esta rama o ticket separado (preguntar al humano con `AskUserQuestion`).
4. Limpiar efímeros: `rm -rf frontend/test-results test-results tmp`.
5. Último check: `git status` limpio, `grep -rn 'Empezar ahora\|Hacer mi primer registro\|NextActionBridgeCard\|S04-BRIDGE\|signInWithMagicLink' frontend/ .docs/wiki/` devuelve 0 matches activos (solo bloques `> Deprecado 2026-04-22`).

## Paso 8 — Merge & deploy

Si el humano lo autoriza:

```bash
git switch main
git pull --ff-only origin main
git merge --no-ff <feature-branch> -m "Merge: impeccable hardening pass 2026-04-22"
git push origin main
```

Si el humano prefiere PR para review:

```bash
gh pr create --base main --head <feature-branch> \
  --title "style(impeccable): audit + hardening completo frontend" \
  --body "Plan: .docs/raw/plans/2026-04-22-impeccable-hardening/"
```

No mergees sin autorización explícita.

## Reporte final esperado

Un único mensaje de cierre con:

1. Lista de commits creados (`git log --oneline <base>..HEAD`).
2. Diff resumen por dimensión impeccable:
   - Accesibilidad: qué violaciones WCAG cerraron, qué ARIA se agregó, contraste corregido.
   - Performance: LCP antes/después (si se midió), bundle delta, server components migrados.
   - Visual quality: tokens normalizados, componentes extraídos, before/after snapshots.
   - Microcopy: cambios en copy visible (respetando los congelados) y en error messages.
   - Responsive: breakpoints ajustados, casos mobile arreglados.
   - Consistencia tokens: matchea al DS (23_uxui/UI-RFC/TECH-FRONTEND-SYSTEM-DESIGN).
3. Outputs (`last line`) de `typecheck`, `lint`, `test:e2e`.
4. Verdict de `ps-trazabilidad` y `ps-auditar-trazabilidad`.
5. Archivos canon wiki tocados.
6. Follow-ups abiertos que no cerraste, con por qué y tamaño estimado.

Si cualquier invariante se rompe en cualquier paso, **parás de una** y pedís dirección con `AskUserQuestion`. Mejor parar que improvisar.
