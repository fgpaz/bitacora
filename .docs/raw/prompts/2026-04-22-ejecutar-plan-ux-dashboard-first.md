<!--
target_platform: claude-code
pressure: aggressive
generated_at: 2026-04-22
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-22-ejecutar-plan-ux-dashboard-first.md' | Set-Clipboard"
-->

# Bootstrap: Ejecutar plan UX "dashboard-first" en Bitácora

Sos un orquestador en Claude Code trabajando sobre `C:\repos\mios\humor` (repo `humor`, workspace `mi-lsp: bitacora`). Tu misión es ejecutar de punta a punta el plan ya escrito en `.docs/raw/plans/2026-04-22-ux-dashboard-first.md` más sus subdocumentos `.docs/raw/plans/2026-04-22-ux-dashboard-first/T0..T5.md`, y cerrar con `ps-trazabilidad` + `ps-auditar-trazabilidad`.

No reescribas el plan. No reabras decisiones. Si algo no coincide con lo escrito, **stop y reportá**; no improvises.

## Contexto congelado (no reabrir)

- **Decisión anchor:** `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`. Lee eso primero, es la fuente de verdad.
- **Alcance UI-only:** nada de tocar backend, contratos, storage, access control, consent storage ni audit logging. Compliance salud Ley 25.326/26.529/26.657 exige que estas superficies permanezcan intactas.
- **Base técnica:** Next.js 16 App Router, React 19, TypeScript, CSS Modules, Playwright 1.59, Zitadel OIDC+PKCE server-side. `/auth/callback/route.ts:57` ya redirige a `/dashboard`; no lo toques.
- **Cambio core:** landing 1-CTA `Ingresar`, eliminar `NextActionBridgeCard` + fase `S04-BRIDGE`, dashboard unificado con `MoodEntryDialog` (modal) + `TelegramReminderBanner` dismissible. Modal vía `<dialog>` nativo, sin librería nueva. No recharts. No tremor. No headlessui.
- **Copy inmutable:** CTA landing = `Ingresar`. Título modal = `Nuevo registro`. Empty state dashboard = `Empezá con tu primer registro`. Banner Telegram = `Recibí recordatorios por Telegram`. Todo en español con tildes y signos de apertura (`¿`, `¡`). Prohibido ASCII-izar o emojis.
- **Clave localStorage banner:** `bitacora.telegram.banner.dismissedAt` (ms), ventana 30 días.
- **`MoodEntryForm` refactor:** soporta prop `embedded?: boolean` + `onSaved?: () => void`. Cuando `embedded=true` no envuelve en `PatientPageShell` y omite links post-success. La ruta `/registro/mood-entry` sigue viva como deep-link desde Telegram reminder.
- **Anti-patrón prohibido:** no envolver `loadData` del dashboard en `useCallback` y llamarlo desde `useEffect` — rompe `react-hooks/set-state-in-effect`. Usá el patrón nonce documentado en `T3-dashboard.md`.

## Paso 1 — Contexto y governance (obligatorio, antes de tocar nada)

1. Correr `Skill(ps-contexto)`. Pedir wiki path y leer `01..05` + `13 voz y tono` + `15 handoff operacional` + `23_uxui/INDEX.md`.
2. Ejecutar:
   ```bash
   mi-lsp workspace status bitacora --format toon
   mi-lsp nav governance --workspace bitacora --format toon
   ```
   Verificar `sync: in_sync` y `blocked: false`. Si governance está blocked, **stop** y reportá; no avances a planificación/ejecución hasta que `crear-gobierno-documental` lo repare.
3. Leer de una sola pasada:
   - `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`
   - `.docs/raw/plans/2026-04-22-ux-dashboard-first.md`
   - todos los `.docs/raw/plans/2026-04-22-ux-dashboard-first/T*.md`
   - `CLAUDE.md` (§0 Skill Invocation Semantics, §1 Orchestration, §9.1 Ortografía, §9.2 Hygiene)

## Paso 2 — Exploración en paralelo (obligatoria, 3 agentes mínimo)

Antes de ejecutar T0, dispará **3 `ps-explorer` en una sola vuelta** (un mensaje, 3 tool calls). Explorer 4 y 5 son opcionales; sumalos si alguno de los tres primeros devuelve contradicciones, paths que no existen, o evidencia de drift entre lo que dice el plan y lo que ves en la rama actual.

- **Explorer A — estado actual del frontend:** verificar que `frontend/app/page.tsx`, `frontend/components/patient/onboarding/OnboardingEntryHero.tsx`, `frontend/components/patient/onboarding/OnboardingFlow.tsx`, `frontend/components/patient/onboarding/NextActionBridgeCard.tsx`, `frontend/components/patient/dashboard/Dashboard.tsx`, `frontend/lib/auth/client.ts` existan y coincidan con lo que asume cada subdoc. Reportar cualquier diff contra el Skeleton.
- **Explorer B — referencias vivas:** `mi-lsp nav refs NextActionBridgeCard --workspace bitacora`, ídem para `signInWithMagicLink`, `S04-BRIDGE`, `Revisá tu correo`. Fallback `rg`. Devolver file:line por hallazgo.
- **Explorer C — canon UX afectado:** abrir `.docs/wiki/23_uxui/UXS/UXS-ONB-001.md`, `UI-RFC/UI-RFC-ONB-001.md`, `HANDOFF-*/ONB-001.md`, `PROTOTYPE/PROTOTYPE-ONB-001.md`, `VOICE/VOICE-ONB-001.md`, `06_pruebas/TP-ONB.md`, `06_pruebas/TP-VIS.md`, `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`, `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md`, `04_RF.md`. Listar las secciones exactas que mencionan Bridge Card / magic link para que T5 las reemplace sin ambigüedad.

Regla: si dos exploradores se contradicen, lanzá **Explorer D** para resolver. Si `T1/T2/T3` ya están parcialmente aplicados en la rama actual (`main` u otra), lanzá **Explorer E** para mapear lo hecho vs. lo pendiente y reportar al usuario antes de tocar código.

## Paso 3 — Ejecución por waves

### Wave 0 — secuencial

Dispatch `T0-setup.md` con subagente `ps-worker`. Pasá el subdoc verbatim. Done when: rama `feature/ux-dashboard-first` creada, working tree limpio.

**Guardrail:** si T0 reporta working tree sucio, **stop**. Mostrá `git status -s` literal al usuario y preguntá con `AskUserQuestion` si conviene stashear, commitear en otra rama, o abortar.

### Wave 1 — paralela (3 subagentes en una vuelta)

En **un solo mensaje** lanzá tres `Agent` calls con `subagent_type: ps-next-vercel`:

1. `T1-landing.md` — landing 1-CTA.
2. `T2-onboarding.md` — OnboardingFlow sin Bridge Card.
3. `T3-dashboard.md` — dashboard modal + banner.

Pass the full subdocument verbatim como prompt. No resumas.

Tras la vuelta: correr `npm run typecheck && npm run lint` en `frontend/`. Ambos exit 0 antes de pasar a Wave 2. Si alguno falla, **stop** y reportá el error literal.

### Wave 2 — paralela (2 subagentes)

En un solo mensaje:

1. `T4-e2e.md` con `ps-next-vercel` — 3 specs Playwright.
2. `T5-canon-sync.md` con `ps-docs` — 13 docs wiki.

Para T4: arrancá `npm run dev` en background (`run_in_background: true`) antes, esperá ~15s, lanzá `npm run test:e2e`. Al terminar, cortá el dev server.

**Guardrail:** si T5 toca un archivo con estructura que el subagente no reconoce, debe **stop** y reportar. No aceptes edits especulativos en docs hardened.

### Final Wave — closure (vos lo ejecutás, no subagente)

1. `Skill(ps-trazabilidad)` sobre todo el alcance del plan. Clasificar el cambio como `ui-only, no-schema, no-contract`. Verificar sync FL-ONB-01 ↔ RF-ONB ↔ UXS-ONB-001 ↔ UI-RFC-ONB-001 ↔ TP-ONB ↔ TP-VIS ↔ RF-VIS-015 ↔ TECH-FRONTEND-SYSTEM-DESIGN.
2. `Skill(ps-auditar-trazabilidad)` modo `full`. Confirmar `0 critical gaps`.
3. Si hay gaps críticos: reopen Wave 2 T5 con los fixes específicos, re-run ambos. No declares "done" hasta verde.

## Invariantes que NO se negocian

- Ningún commit con `--no-verify` / `--no-gpg-sign` / `--amend` a un commit ya empujado.
- Ninguna dependencia npm nueva.
- Ningún cambio en `frontend/app/auth/callback/route.ts` ni en `frontend/lib/auth/server.ts`.
- Ningún cambio en contratos backend `lib/api/client.ts` fuera del shape ya existente.
- No inventar nombres de agentes. `agent_type` de cada task sólo puede ser uno de los listados en el plan principal.
- Español con tildes y signos de apertura. Sin emojis.
- Artefactos efímeros van a `tmp/` o `artifacts/` y se limpian antes del cierre (política §9.2).

## Criterios de DONE

- Ramas `feature/ux-dashboard-first` con 5 commits (uno por task T1..T5) más, si aplica, un commit de plan ya presente en `main`.
- `npm run typecheck`, `npm run lint`, `npm run test:e2e` verdes en `frontend/`.
- `grep -rn 'NextActionBridgeCard\|S04-BRIDGE\|signInWithMagicLink' frontend/ .docs/wiki/` → 0 matches (o sólo bloques histórico-deprecado explícitos en wiki).
- `mi-lsp nav governance --workspace bitacora --format toon` → `sync: in_sync`.
- `ps-trazabilidad` y `ps-auditar-trazabilidad` cierran sin gaps críticos.
- Rama lista para PR. No la mergees vos; dejá la URL del compare o un resumen breve para que el humano abra el PR.

## Salida esperada al terminar

Un único reporte final con:

1. Commits creados (`git log --oneline feature/ux-dashboard-first ^main`).
2. Output de `typecheck`, `lint`, `test:e2e` (last line cada uno).
3. Resultado de `ps-trazabilidad` y `ps-auditar-trazabilidad` (verdict).
4. Lista de archivos wiki efectivamente modificados.
5. Follow-ups que no pudiste cerrar (con su por qué).

Si cualquier invariante se rompe, parás de una y pedís dirección al humano con `AskUserQuestion`. Mejor parar que improvisar.
