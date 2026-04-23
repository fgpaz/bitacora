<!--
target_platform: claude-code
pressure: aggressive
generated_at: 2026-04-23
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-23-login-flow-ux-audit.md' | Set-Clipboard"
-->

# MISIÓN — Auditoría UX end-to-end del flujo de acceso y registro de Bitácora

Operás en `C:\repos\mios\humor`, branch `main` (working tree limpio al momento de generar este prompt, commit `0bcb11e`). Hay un dolor explícito del product owner: **la UX de inicio de sesión es mala y no queda claro cómo un usuario ya registrado accede a sus registros ni cómo registra algo nuevo**. Tu trabajo es producir una auditoría read-only, evidenciada, priorizada y accionable — **sin tocar código en esta corrida**. El entregable es un reporte markdown; la implementación vendrá después en otra sesión con otro prompt.

No confíes en la memoria de conversaciones previas. Verificá repo state antes de creer nada que diga este prompt.

## Outcome esperado (no negociable)

Un único archivo nuevo:

```
.docs/raw/reports/2026-04-23-login-flow-audit.md
```

Debe contener:

1. **Resumen ejecutivo** (≤ 200 palabras) con el top-3 problemas y el verdict: ¿ship-ready, needs-refinement, needs-redesign?
2. **Mapa del journey** (ASCII o markdown table) cubriendo las 2 personas con igual peso:
   - Paciente primera vez: `/` → `/ingresar` → Zitadel → `/auth/callback` → `/onboarding` (o `/consent`) → `/dashboard` → CTA Registrar humor → modal → guardado.
   - Paciente recurrente: reentrada con cookie `bitacora_session` viva + sesión expirada + logout explícito.
3. **Inventario de hallazgos** con este shape exacto por cada finding (NO omitir columnas):
   | ID | Surface | Persona | Heurística violada | Severidad | Evidencia file:line | Impacto observado |
4. **Severidad con criterio Bitácora**:
   - `P0-blocker`: bloquea acceso, rompe sesión, o contradice copy congelado 2026-04-22 / canon 10 (refugio clínico sereno). Exemplos: dead-end, CTA invisible, loop de auth, pérdida silenciosa de estado.
   - `P1-major`: fricción alta, ambigüedad que frena a recurrente en < 3s, a11y AA rota en CTA principal, contradicción con identidad visual 11 o patrones 16.
   - `P2-minor`: inconsistencia de tokens, micro-copy mejorable, polish que no bloquea.
   - `P3-delight`: oportunidad no explotada (no es falla).
5. **Recomendaciones priorizadas** agrupadas por severidad, cada una con: qué cambiar, por qué, dónde (file:line), y skill impeccable-* sugerido para el fix.
6. **Follow-ups explícitos** para cosas fuera de scope (backend, analytics, e2e) que surjan durante el audit.
7. **Cierre con verdict + próxima acción recomendada**.

## Invariantes protegidos (violación = abort)

- **NO tocar código productivo** — esto es read-only. Si sentís la tentación de "arreglar algo chiquito", anotalo como recomendación y seguí.
- **NO leer ni opinar sobre**: `frontend/lib/auth/*`, `frontend/app/api/*`, `frontend/app/auth/*`, `frontend/proxy.ts`, `frontend/src/**`. Estos viven fuera del scope del audit. Si un hallazgo apunta a alguna de estas zonas, documentá el síntoma visible pero no te metas en la implementación.
- **Copy congelado post dashboard-first (2026-04-22) es ley**. Textos como "Tu espacio personal de registro", "Solo vos ves lo que registrás. Tus datos son privados.", "Registrar humor", "Empezá con tu primer registro", "Variabilidad diaria" → "Tus últimos días" ya están auditados y aceptados. Si los criticás, tu crítica tiene que justificar por qué vale reabrir una decisión cerrada — de lo contrario, respetalos.
- **Compliance salud mental**: cualquier hallazgo que implique cambios en consent, audit log, o exposición de datos debe señalarse con flag explícito `⚠ legal-review` y citar Ley 25.326 / 26.529 / 26.657 según corresponda.
- **No `taskkill`, no matar el dev server del usuario, no `--no-verify`, no `--force push`, no `--amend`**.
- **No agregar dependencias npm** (ni siquiera mencionarlo como recomendación casual; si creés que hace falta una librería, justificá explícitamente y marcá como P1).

## Paso 0 — Anclaje de contexto (obligatorio, antes de cualquier otra cosa)

Ejecutá en este orden:

1. `Skill("ps-contexto")` — carga scope, arquitectura, flujos, modelo de datos, riesgos y canon UX/UI. No negocies este paso.
2. Leé explícitamente estos documentos (usá Read directo, no te confíes de que ps-contexto ya los haya cargado todos):
   - `.docs/wiki/01_alcance_funcional.md`
   - `.docs/wiki/02_arquitectura.md`
   - `.docs/wiki/03_FL.md` + el FL de login/acceso/registro si existe (buscalo en `.docs/wiki/03_FL/`).
   - `.docs/wiki/04_RF.md` + RFs de acceso y registro.
   - `.docs/wiki/10_manifesto.md` / `.docs/wiki/11_identidad_visual.md` / `.docs/wiki/12_lineamientos_ux_ui.md` / `.docs/wiki/13_voz_tono.md` / `.docs/wiki/16_patrones_ux.md` (si existen con esos slugs; si la numeración difiere, usá `ps-asistente-wiki` para mapear).
   - `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md` — closure de la sesión previa, define qué ya está resuelto.
3. Validá workspace con `mi-lsp workspace list` y elegí el alias que cubra `frontend/`. Si `mi-lsp` devuelve `hint` o `next_hint`, seguí esa guía antes de avanzar.

Si algún doc no existe o está vacío, **anotalo como gap de documentación en el reporte final** pero seguí el audit — no bloquees la corrida por eso.

## Paso 1 — Exploración paralela obligatoria (5 `ps-explorer` en UN SOLO mensaje)

Despachá los 5 en paralelo. No secuencial. El scope es cross-module (landing + auth redirect + callback side effects + onboarding gate + consent gate + dashboard + modal + sesión expirada), por eso 5 es el piso, no 3.

Usá `mi-lsp` para navegación semántica en cada subagent cuando explore bajo `frontend/`. Raw grep solo como fallback.

**Explorer 1 — Entrada pública y redirect a IdP**
Objetivo: mapear landing (`frontend/app/page.tsx`), `OnboardingEntryHero`, el handler de `/ingresar` (sin leer `lib/auth/*` ni `app/api/*` — solo comportamiento observable: qué renderiza, qué CTAs, dónde apuntan, qué response esperan). Reportar: estructura de jerarquía visual, CTAs visibles, copy literal, y qué pasa cuando un usuario con cookie viva aterriza en `/` (¿lo redirige? ¿lo deja ver landing pública?). Evidencia: file:line por cada afirmación.

**Explorer 2 — Post-callback onboarding y consent**
Objetivo: mapear `frontend/app/(patient)/onboarding/page.tsx` y `frontend/app/(patient)/consent/page.tsx`, los componentes bajo `frontend/components/patient/onboarding/*` y `frontend/components/patient/consent/*`, y cómo deciden si mostrar onboarding vs consent vs dashboard. Reportar: lógica de decisión observable, copy, CTAs, qué ve un recurrente vs primera vez, y cómo se recupera si recarga a mitad del flow.

**Explorer 3 — Dashboard como hub de retorno**
Objetivo: mapear `frontend/app/(patient)/dashboard/page.tsx` + `frontend/components/patient/dashboard/*`. Reportar con file:line: dónde está el CTA "Registrar humor" (visibilidad, jerarquía, touch target, contraste), cómo se ve el empty state vs ready, cómo se ve el `DashboardSummary`, cómo descubre un recurrente sus registros previos, si hay breadcrumb/back/logout accesible, y dónde está el acceso a configuración/Telegram pairing. Cross-checkear contra `.docs/raw/reports/2026-04-22-impeccable-audit-baseline.md` para no redescubrir findings ya cerrados.

**Explorer 4 — Modal de registro y flujo de guardado**
Objetivo: mapear el modal que abre "Registrar humor" y `frontend/app/(patient)/registro/mood-entry/page.tsx` / `registro/daily-checkin/page.tsx`. Reportar: estructura del modal (a11y: `aria-modal`, focus trap, ESC, focus return), campos, validación visible, estados loading/error/success, mensaje post-guardado, y cómo el usuario entiende que volvió al dashboard con su registro sumado. Flaky a11y en modal = P0 automático.

**Explorer 5 — Edge states (sesión expirada, logout, error, not-found)**
Objetivo: mapear `frontend/app/error.tsx`, `frontend/app/not-found.tsx`, y el comportamiento observable cuando la sesión expira (sin tocar `lib/auth/*`: mirá qué renderiza el usuario, qué copy ve, dónde lo manda). Reportar: ¿hay CTA claro "Volver a ingresar"? ¿hay pérdida silenciosa de estado? ¿el copy es sereno según canon 13 o es genérico? ¿Dónde está el logout en la UI? ¿qué ve el usuario después de logout?

Cada explorer debe devolver: findings con file:line, copy literal citado, y no más de 10 puntos por reporte. Si dos explorers se contradicen, despachá un sexto en una segunda ronda para resolver. No integres sin verificación cruzada.

## Paso 2 — Cadena impeccable-* (todas las skills, en este orden)

Cada skill se ejecuta sobre el output consolidado del Paso 1. No saltes pasos ni los mezcles — cada skill produce una capa que la siguiente lee.

1. **`Skill("impeccable-audit")`** — baseline consolidado. Un único reporte que unifique los 5 explorers, dedupe, y clasifique por surface. Guardar en `.docs/raw/reports/2026-04-23-login-flow-baseline.md`.

2. **`Skill("impeccable-clarify")`** — resolvé las zonas ambiguas que detectaste (copy vago, comportamiento no obvio, gaps de spec). Por cada ambigüedad, proponé la decisión concreta + evidencia canon (manifesto 10, identidad 11, lineamientos 12, voz 13, patrones 16). Si una ambigüedad requiere input del usuario humano, marcála `🔒 needs-human-decision` — no la resuelvas inventando.

3. **`Skill("impeccable-critique")`** — corazón del audit. Aplicá el formato 1 rediseñar / 3 refinar / 1 mantener por surface (landing, onboarding/consent, dashboard, modal, edge states). Severidad P0–P3 según criterios de arriba. Cross-check cada crítica contra el copy congelado 2026-04-22 — si contradice algo ya aceptado, justificá por qué reabrir.

4. **`Skill("impeccable-harden")`** — pasada específica de a11y + edge cases + resiliencia:
   - Contraste ≥ 4.5:1 sobre todos los CTAs y copy crítico (usá tokens de `frontend/styles/tokens.css`).
   - Touch targets ≥ 44×44.
   - `aria-modal`, focus trap, focus return en el modal.
   - `aria-live` / `role="status"` en mensajes de éxito/error.
   - `focus-visible` + `:focus` + keyboard navigation full.
   - Reduced motion.
   - Comportamiento con JS deshabilitado para `/` y `/ingresar`.
   - Copy que no asume pantalla grande ni mouse.

5. **`Skill("impeccable-normalize")`** — consistencia de patrones:
   - Tokens de `tokens.css` (radios 4/8/12, spacing, colores, tipografías) aplicados uniformemente.
   - CTAs primarios vs secundarios vs link-btn — jerarquía coherente entre landing / dashboard / modal / pairing.
   - Iconografía (si existe) uniforme.
   - Gradientes, sombras, bordes — flagueá divergencias con canon 11.
   - Voz/tono (canon 13) coherente entre pantallas.

6. **`Skill("impeccable-onboard")`** — foco en paciente primera vez:
   - ¿La landing comunica la promesa en < 3 segundos?
   - ¿El primer aterrizaje post-login orienta sin abrumar?
   - ¿El primer CTA de registro es inequívoco?
   - ¿El empty state del dashboard acompaña o intimida?
   - ¿Hay microcopy que reduzca ansiedad (canon 10: refugio sereno)?

7. **`Skill("impeccable-delight")`** — foco en paciente recurrente:
   - ¿El retorno se siente familiar (reconocimiento > recall)?
   - ¿Hay continuidad emocional entre sesiones?
   - ¿El sistema acusa recibo cuando el usuario vuelve (sin ser invasivo)?
   - ¿Los micro-momentos (guardado, refresh del historial) tienen texture?
   - Listar máximo 5 oportunidades de delight que NO violen canon 10 (sobriedad > virtuosismo).

8. **`Skill("impeccable-polish")`** — última pasada de refinamiento micro:
   - Espaciados irregulares, alineaciones off, grids inconsistentes.
   - Letter-spacing / line-height que rompen ritmo.
   - Transiciones que se sienten cheap o falta de ellas donde deberían estar.
   - Copy con tildes faltantes, signos invertidos ausentes (¿/¡), puntuación inconsistente.

No ejecutes `impeccable-extract`, `impeccable-optimize`, ni `impeccable-adapt` en esta corrida — quedan para una sesión de implementación posterior si el audit los justifica.

## Paso 3 — Escritura del reporte final

Consolidá todo en `.docs/raw/reports/2026-04-23-login-flow-audit.md` con el shape obligatorio definido en "Outcome esperado". Requisitos adicionales:

- **Cada hallazgo debe citar file:line**. Si no podés citarlo, no es un hallazgo, es una intuición — descartalo o movelo a "preguntas abiertas".
- **Copy literal entre comillas**. Nada de parafrasear.
- **Diferenciá claramente** síntomas observables (dentro de scope) de hipótesis de causa raíz en zonas bloqueadas (`lib/auth/*`, `app/api/*`, etc.).
- **No inventes estado de prod ni de Dokploy**. Si una recomendación requiere validar en prod, marcála `needs-prod-validation`.
- **Tabla de métricas proxy** al final: cantidad de P0 / P1 / P2 / P3, cantidad de surfaces tocadas, cantidad de heurísticas únicas violadas, cantidad de findings por persona.
- **Enlazá** al baseline (`2026-04-23-login-flow-baseline.md`) y al closure previo (`2026-04-22-impeccable-hardening-closure.md`) en el header.

## Paso 4 — Cierre

1. `Skill("ps-trazabilidad")` — verificá que el reporte esté bien enlazado, que no haya huérfanos, y que los RF/FL referenciados existan. Si detectás que el audit destapa gaps de traceability en wiki, listalos como follow-up (no los corrijas acá).
2. Hacé un commit atómico **solo de los dos archivos nuevos** (baseline + audit) en una branch nueva `audit/login-flow-2026-04-23`. No mergeés a main — esta es una corrida read-only sobre código. El merge del reporte lo decide el humano.
3. Mensaje del commit: `docs(audit): read-only audit del flujo login+dashboard+modal 2026-04-23` con trailer `- Gabriel Paz -`.
4. En tu mensaje final de cierre, devolvé:
   - Path absoluto de los 2 archivos nuevos.
   - Cantidad de P0/P1/P2/P3 encontrados.
   - Top-3 findings con una frase cada uno.
   - Verdict: `ship-ready` / `needs-refinement` / `needs-redesign`.
   - Próxima acción recomendada (qué prompt generar después, con qué skill impeccable-* empezar el fix).

## Reglas de operación

- **Orchestrator mode siempre**. Los explorers y el audit-lint van en subagents. El hilo principal sintetiza y decide, no scrapea.
- **Cross-check ≥ 2 fuentes** para cualquier finding con severidad P0 o P1 antes de estampar severidad.
- **Evidencia o no existe**. Si un subagent reporta "encontré X" sin file:line, pedílo de vuelta o descartálo.
- **Si dos impeccable-* dicen lo contrario sobre un mismo elemento**, el orden que gana es: `harden > clarify > critique > normalize > onboard > delight > polish`. Documentá la decisión.
- **Timeboxing flexible**. Si un explorer se estira más de lo razonable, cortá y recalibrá con una segunda ronda focalizada — no dejes que un solo surface consuma toda la sesión.
- **Si algo queda inconcluso al final**, es mejor entregar un reporte honesto con "no validado: X, Y" que inventar verdicts.

## Referencias útiles (leé según corresponda)

- `CLAUDE.md` (raíz) — política operativa completa.
- `AGENTS.md` (raíz) — catálogo de subagents.
- `.docs/raw/reports/2026-04-22-impeccable-audit-baseline.md` — baseline previo del frontend entero (evitá redescubrir).
- `.docs/raw/reports/2026-04-22-impeccable-critique.md` — critique previo (decisiones ya cerradas).
- `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md` — qué quedó resuelto el 22.
- `.docs/raw/plans/2026-04-22-impeccable-hardening.md` — plan ejecutado y cerrado.
- `frontend/styles/tokens.css` — fuente de verdad de tokens.
- `frontend/e2e/landing.spec.ts`, `frontend/e2e/dashboard-modal.spec.ts`, `frontend/e2e/telegram-banner.spec.ts` — especificaciones e2e vigentes, útiles como contrato de comportamiento esperado.

## Última palabra

No caigas en el patrón de "audit aguado". El product owner dijo literalmente que la UX es **malísima** para un recurrente. Si al final no encontrás nada P0 o P1, pensá de nuevo: o el dolor no está en el código (está en el flujo mental del usuario, en expectativas no cumplidas, en ausencias más que en defectos), o no miraste lo suficiente. Ausencia de CTA es un hallazgo. Ausencia de pista de retorno es un hallazgo. Un dashboard que exige que el usuario deduzca dónde hace clic es un hallazgo. Miralo con ojos de persona cansada en el celular a las 23:47 — no con ojos de developer que sabe dónde está todo.

Empezá ya. Paso 0 primero. Nada de preámbulos.
