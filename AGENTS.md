# AGENTS.md - Project Operating Policy (Strict Orchestrator Mode)

## Project: Bitacora — Clinical Mood Tracker

**Slug:** bitacora.nuestrascuentitas.com
**Stack:** .NET 10 + Next.js 16 + Telegram Bot + PostgreSQL
**Auth:** Zitadel OIDC + PKCE (`id.nuestrascuentitas.com`)
**Deploy:** Dokploy VPS 54.37.157.93

> **Health Data Sensitivity:** This project processes sensitive health data under Argentine Ley 25.326 (Data Protection), Ley 26.529 (Patient Rights), and Ley 26.657 (Mental Health). Any change to data storage, access control, consent flows, or audit logging must be reviewed for regulatory compliance before push.

## Spec Driven Development Contract (Mandatory)

This project uses Spec Driven Development.
The wiki (`.docs/wiki/00..23`) is the ONLY source of truth.

Before writing any code, the agent MUST:
  1. Identify the `RF-*`, `FL-*`, or `CT-*` that anchors this change.
  2. If no anchor exists, create it first via `$ps-docs` or the matching `crear-*` skill.
  3. Launch `ps-explorer` (uses `mi-lsp`) to confirm anchor and load context. Minimum: `mi-lsp nav pack "<task>"` and `mi-lsp nav ask "<question>"`.
  4. No write subagent (`ps-dotnet10`, `ps-next-vercel`, `ps-python`) starts before step 3.

Non-compliant: implementing without citing an `RF-*`/`FL-*`/`CT-*`, or skipping `ps-explorer`.

## Workflow Stance (XP on `main`, Mandatory)

Bitacora works in **Extreme Programming sobre `main`**: push directo a `main`, sin PRs, sin feature branches de larga duracion.

- Commit atomico a `main` tras validacion local (typecheck + lint + tests aplicables + `dotnet build` cuando toca backend).
- `git push origin main` como cierre estandar. No se abren PRs.
- NO crear feature branches salvo que el humano lo pida explicitamente.
- Cada commit lleva el trailer `- Gabriel Paz -`.
- Closure docs siguen obligatorios para tareas grandes (`.docs/raw/reports/*-closure.md`). XP acelera el merge, no suprime la traceability.
- Safeguards preservados: acciones de alto blast radius (prod SQL, `--force push`, `rm -rf`, modificar Dokploy prod) siguen requiriendo confirmacion explicita incluso bajo XP-on-main.
- Antes de cada push a `main`, ejecutar `$ps-pre-push` como gate final tras `$ps-trazabilidad` (y `$ps-auditar-trazabilidad` cuando corresponda).
- Nunca commitear ni pushear archivos bajo `.docs/raw/` que sean scratch local; solo promover a `.docs/raw/decisiones/`, `.docs/raw/reports/` o equivalentes cuando la evidencia deba persistir.

## 1) Orchestration Mode (MANDATORY - Always Active)

For EVERY task (`feature`, `bugfix`, `refactor`, `docs`, `infra`, `performance`):

1. Start with `$ps-contexto` before proposing changes or implementation.
2. Run `$mi-lsp` as the first exploration tool. Validate the workspace alias/path before assuming one, then use it before raw grep/glob/read loops.
3. After context and exploration, run `$brainstorming` once before planning/execution.
4. Close critical context gaps before execution.
5. ALWAYS work in **Orchestrator Mode**.
6. Close the task with `$ps-trazabilidad` before marking it complete.

### Skill Invocation Semantics

| Skill | When | Mandatory |
|-------|------|-----------|
| `$ps-contexto` | At the start of EVERY task | Yes — before any action |
| `$mi-lsp` | First exploration tool for repo and code investigation | Yes — validate workspace alias/path first; before raw grep/glob/read loops |
| `$brainstorming` | After context, before implementation | Yes — for non-trivial tasks |
| `$writing-plans` | Large/risky tasks, after brainstorming | Yes — for large tasks |
| `$ps-trazabilidad` | Before closing any task | Yes — verifies sync |
| `$ps-auditar-trazabilidad` | Large/risky/multi-module changes | Yes — cross-document audit |
| `$ps-pre-push` | Before any `git push origin main`, after traceability/audit | Yes — final XP-on-main gate |

### Registered Subagent Catalog

| Domain | Subagent | Purpose | Sandbox |
|--------|----------|---------|---------|
| Exploration | ps-explorer | Search symbols, trace paths, read code | read-only |
| Backend | ps-dotnet10 | Generate/modify .NET 10 code (Bitacora.Api) | workspace-write |
| Frontend | ps-next-vercel | Generate/modify Next.js 16 code | workspace-write |
| Python | ps-python | Scripts, Telegram utilities, tooling | workspace-write |
| Code review | ps-code-reviewer | Review diffs, audit quality P>D>S | read-only |
| QA orchestrator | ps-qa-orchestrator | Quality + security + testing audit | read-only |
| QA security | ps-qa-backend-security | Backend security compliance | read-only |
| Gap audit | ps-gap-auditor | Spec-vs-code gap detection | read-only |
| SDD sync | ps-sdd-sync-gen | Auto-generate specs from code | workspace-write |
| Docs | ps-docs | Wiki, specs, READMEs, changelogs | workspace-write |
| Worker | ps-worker | Git, config, shell, plans, ops | workspace-write |

### Additional Mandatory Rules

- If editing `AGENTS.md` or `CLAUDE.md`, use `$ps-crear-agentsclaudemd`.
- If the change is large, risky, or multi-module, run `$ps-auditar-trazabilidad` before final closure.
- Use `$mi-lsp` as the mandatory first exploration tool for repo and code navigation. Validate the workspace alias/path with `mi-lsp workspace list` and `mi-lsp workspace status <alias-or-path> --format toon` before assuming a workspace name. If `mi-lsp` returns `hint` or `next_hint`, follow that rerun guidance before retrying. Fallback only after `mi-lsp` has been tried.
- No default exceptions.
- In `$ps-contexto`, always read these docs before planning/execution:
   - `.docs/wiki/02_arquitectura.md` — service responsibilities and architecture
   - `.docs/wiki/05_modelo_datos.md` — canonical entities (when it exists)
   - `.docs/wiki/07_baseline_tecnica.md` — runtime baseline (when it exists)

## 2) Workflow Catalog (How Work Should Flow)

### A) Standard Task Flow
1. `$ps-contexto` — load project context
2. `$mi-lsp` — validate workspace alias/path and explore first
3. `$brainstorming` — challenge and lock design decisions
4. Orchestrated execution (subagents in parallel)
5. Documentation synchronization
6. `$ps-trazabilidad` — closure

### B) Large / Risky / Multi-Step Task Flow
1. `$ps-contexto` — load project context
2. `$mi-lsp` — validate workspace alias/path and explore first
3. `$brainstorming` — design and harden
4. `$writing-plans` — generate wave-dispatchable plan with subdocuments
5. Wave execution with subagents (run `$ps-trazabilidad` per batch)
6. `$ps-trazabilidad` — final closure
7. `$ps-auditar-trazabilidad` — read-only audit before marking done

### C) Policy-Change Flow (AGENTS.md / CLAUDE.md)
1. `$ps-contexto` → `$mi-lsp` → `$brainstorming` → `$ps-crear-agentsclaudemd` → `$ps-trazabilidad`

### D) Small / Trivial Task Flow
1. `$ps-contexto` → `$mi-lsp` → `$brainstorming` (lock assumptions) → execute → `$ps-trazabilidad` → `$ps-pre-push` before pushing to `main`

### E) Pre-Push Flow for XP on `main`
1. `$ps-trazabilidad` debe estar completo.
2. `$ps-auditar-trazabilidad` debe estar completo para cambios grandes/riesgosos, multi-modulo, policy-changing, o closure de waves.
3. Tests aplicables verdes (frontend: typecheck + lint + e2e; backend: `dotnet build` + tests unitarios cuando existan).
4. Grep de zonas congeladas (`lib/auth/*`, `proxy.ts`, `app/api/*`, `app/auth/*`, `src/` frontend) debe dar 0 cruces cuando el scope lo requiera.
5. Sin archivos `.docs/raw/` de scratch agregados al commit.
6. `$ps-pre-push` debe retornar `Approved` o `Approved with waiver` antes de `git push origin main`.

## 3) Project Decision Priority

1. Security
2. Privacy
3. Correctness
4. Usability
5. Maintainability
6. Performance
7. Cost
8. Time-to-market

**Source of truth:** `.docs/wiki/02_arquitectura.md`.

## 4) Canonical Source of Truth (Project Paths)

- Scope: `.docs/wiki/01_alcance_funcional.md`
- Architecture: `.docs/wiki/02_arquitectura.md`
- Flow index: `.docs/wiki/03_FL.md`
- Flow docs: `.docs/wiki/03_FL/FL-*.md`
- RF index: `.docs/wiki/04_RF.md`
- RF docs: `.docs/wiki/04_RF/RF-*.md`
- Data model: `.docs/wiki/05_modelo_datos.md`
- Test matrix: `.docs/wiki/06_matriz_pruebas_RF.md`
- Test plans: `.docs/wiki/06_pruebas/TP-*.md`
- Tech baseline: `.docs/wiki/07_baseline_tecnica.md`
- Tech detail: `.docs/wiki/07_tech/TECH-*.md`
- Physical data: `.docs/wiki/08_modelo_fisico_datos.md`
- DB detail: `.docs/wiki/08_db/DB-*.md`
- Contracts: `.docs/wiki/09_contratos_tecnicos.md`
- Contract detail: `.docs/wiki/09_contratos/CT-*.md`
- Research: `.docs/raw/investigacion/`
- Decisions: `.docs/raw/decisiones/`
- Plans: `.docs/raw/plans/`

## 5) Placeholder Mapping for Skills

- `<ALCANCE_DOC>` → `.docs/wiki/01_alcance_funcional.md`
- `<ARQUITECTURA_DOC>` → `.docs/wiki/02_arquitectura.md`
- `<FL_INDEX_DOC>` → `.docs/wiki/03_FL.md`
- `<FL_DOCS_DIR>` → `.docs/wiki/03_FL`
- `<RF_INDEX_DOC>` → `.docs/wiki/04_RF.md`
- `<RF_DOCS_DIR>` → `.docs/wiki/04_RF`
- `<MODELO_DATOS_DOC>` → `.docs/wiki/05_modelo_datos.md`
- `<TP_INDEX_DOC>` → `.docs/wiki/06_matriz_pruebas_RF.md`
- `<TP_DOCS_DIR>` → `.docs/wiki/06_pruebas`

## 6) Subagent Orchestration Protocol

### Why subagents
- Save tokens in the main orchestrator context window
- Gain speed with parallel execution
- Do not pollute the main agent context with intermediate data
- Subagents return SUMMARIES, not full dumps

### Subagent mental model
Subagents run on degraded models (Codex-Spark in Codex). They are fast and cheap but:
- DO NOT reason well about ambiguous tasks
- INVENT when the task is not well-scoped
- DO NOT have project context unless explicitly provided

### Launch rules
- Trivial task (read 1 file, find 1 symbol): 1 subagent
- Medium task (investigate a module, implement 1 feature): >= 3 in parallel
- Complex task (cross-cutting refactor, multi-step plan): >= 5 in parallel
- ALWAYS launch in a single message with multiple tool calls (not sequential)
- NEVER delegate understanding — the principal must understand the result, not the subagent

### Task formulation for degraded models
Each delegated task MUST:
- Be atomic (1 clear objective, not "investigate then fix")
- Include minimal context (relevant paths, what to search, what to ignore)
- Specify response format (file:line, diff, list, JSON)
- Have limited scope (max 3-5 files or 1 directory)

### Challenge protocol
When receiving subagent results:
1. Cross-check results between subagents — if 2 of 3 agree, high confidence
2. Demand evidence: file:line, grep output, or concrete diff
3. If contradiction, launch 1 additional subagent to resolve
4. NEVER integrate a result without verifying that cited paths exist
5. If a subagent reports "found nothing", verify with another before accepting

## 7) Subagent Routing

| Task | Subagent | Sandbox |
|------|----------|---------|
| Explore code, find symbols | ps-explorer | read-only |
| Generate/modify .NET 10 code | ps-dotnet10 | workspace-write |
| Generate/modify Next.js code | ps-next-vercel | workspace-write |
| Generate/modify Python code | ps-python | workspace-write |
| Code review, audit quality | ps-code-reviewer | read-only |
| QA: quality + security + testing | ps-qa-orchestrator | read-only |
| Backend security compliance | ps-qa-backend-security | read-only |
| Gap detection (docs vs code) | ps-gap-auditor | read-only |
| Spec-code sync | ps-sdd-sync-gen | workspace-write |
| Git, config, shell, plans, ops | ps-worker | workspace-write |
| Create/update docs, wiki, specs | ps-docs | workspace-write |

### Dispatch Examples

**"Implement CRUD endpoint"** — launch 5 in parallel:
1. ps-explorer: "Find if MoodEntry entity exists in Domain/Entities"
2. ps-explorer: "List existing endpoints in Api/Endpoints/"
3. ps-explorer: "Find test patterns in Tests/"
4. ps-dotnet10: "Create POST /api/v1/mood-entries with Command and Handler"
5. ps-dotnet10: "Create GET /api/v1/mood-entries with Query and pagination"

**"Review diff before push to main"** (XP-on-main) — launch 3 in parallel:
1. ps-code-reviewer: "Review the diff, focus on P0 performance and security"
2. ps-explorer: "Check if changed files have tests"
3. ps-qa-orchestrator: "Audit quality and security on diff files"

**"Close documentation gaps"** — launch 3 in parallel:
1. ps-gap-auditor: "Scan FL → RF → TP traceability for missing links"
2. ps-explorer: "Find RF that reference entities not in 05_modelo_datos.md"
3. ps-docs: "Update 09_contratos_tecnicos.md with new endpoints"

## 8) Documentation Synchronization Rule

If any of these changes:
- I/O contracts, typed errors, endpoints
- Entities, states, lifecycle
- Flow steps or ownership
- Runtime budgets, auth, headers

Then review and update (if applicable):
- `.docs/wiki/04_RF/RF-<MOD>-<NNN>.md`
- `.docs/wiki/03_FL/FL-<DOM>-<NN>.md`
- `.docs/wiki/05_modelo_datos.md`
- `.docs/wiki/04_RF.md`
- `.docs/wiki/06_matriz_pruebas_RF.md`
- `.docs/wiki/06_pruebas/TP-<MOD>.md`
- `.docs/wiki/07_baseline_tecnica.md` / `07_tech/TECH-*.md`
- `.docs/wiki/08_modelo_fisico_datos.md` / `08_db/DB-*.md`
- `.docs/wiki/09_contratos_tecnicos.md` / `09_contratos/CT-*.md`

## 9) Language Rule + Orthography + Artifact Hygiene

- `AGENTS.md`, `CLAUDE.md`, and project-local skills must be written in English.
- Functional wiki docs under `.docs/wiki` may remain in Spanish.
- Avoid emojis in policy docs and governance outputs.
- All user-visible text must use correct Spanish orthography (tildes, n, inverted punctuation).
- Never create ephemeral artifacts in repository root. Use `tmp/` or `artifacts/`.
- E2E/browser evidence goes to `artifacts/e2e/<YYYY-MM-DD>-<task-slug>/`.
- Delete or relocate ephemeral outputs before task closure.

## 10) Skill Purpose Map

- `ps-contexto`: Project context bootstrap + precedence method
- `brainstorming`: Post-context challenge and decision lock
- `dispatching-parallel-agents`: Orchestrator delegation for independent work
- `writing-plans`: Wave-dispatchable implementation plans
- `ps-trazabilidad`: Task closure traceability and documentation sync
- `ps-auditar-trazabilidad`: Read-only audit for large/risky changes
- `ps-pre-push`: Final XP-on-main gate before `git push origin main`
- `ps-crear-agentsclaudemd`: Policy wiring for `AGENTS.md` / `CLAUDE.md`
- `crear-alcance`: Generate scope document (01_alcance_funcional.md)
- `crear-arquitectura`: Generate architecture document (02_arquitectura.md)
- `crear-flujo`: Generate flow documents (03_FL/FL-*.md)
- `crear-requerimiento`: Generate requirement documents (04_RF/RF-*.md)
- `crear-modelo-de-datos`: Generate data model document (05_modelo_datos.md)
- `crear-capa-tecnica-wiki`: Generate technical layer (07/08/09 + detail docs)
- `ps-asistente-wiki`: SDD workflow navigation and phase gate validation
- `ps-gap-terminator`: Gap detection across spec-driven stack
- `ps-investigar`: Research and evidence gathering

## 11) XP-on-main Workflow (Mandatory)

### Direct push to `main`

- Todo trabajo cierra con `git push origin main` tras `$ps-pre-push`.
- Sin PRs, sin `gh pr create`, sin feature branches.
- Commits atomicos con trailer `- Gabriel Paz -`.
- Si un cambio grande requiere waves, cada wave cierra con su propio `$ps-trazabilidad` + commit directo a `main` (no acumular en feature branch).

### Cuando SI usar rama separada

Solo cuando el humano pide explicitamente:

- Experimentacion de alto riesgo que puede descartarse sin costo.
- Spike tecnico sin intencion de merge inmediato.
- Colaboracion con agentes externos que requieren branch dedicada.

En estos casos, nombrar la rama `spike/<slug>-<YYYY-MM-DD>` y documentar la razon en el commit inicial.

### Safeguards inviolables

Incluso bajo XP-on-main, las siguientes acciones SIEMPRE requieren confirmacion explicita del humano en el mismo turno:

- Aplicar SQL plano en produccion (VPS 54.37.157.93) — seguir `infra/runbooks/manual-migrations.md`.
- Force push a `main` (`git push --force origin main`).
- `git reset --hard` sobre commits pusheados.
- `rm -rf` sobre directorios del proyecto.
- Modificar Dokploy produccion (restart, env vars, scheduled tasks).
- Rotar secrets produccion.
- Tocar zonas congeladas sin decision explicita: `frontend/lib/auth/**`, `frontend/app/api/**`, `frontend/app/auth/**`, `frontend/proxy.ts`, `frontend/src/**`.
- Comprometer PII o contenido clinico via logs, analytics props, o audit sin pseudonimizacion.

### Closure docs siguen obligatorios

- Tareas grandes (>= 5 commits o cross-module) producen `.docs/raw/reports/<YYYY-MM-DD>-<slug>-closure.md`.
- Closure doc lista verdict por scope item + verificaciones + follow-ups + referencias a commits.
- Ausencia de closure doc en una tarea que lo amerita es **gap de traceability** y bloqueante para `$ps-pre-push`.

### Tests obligatorios pre-push

- Frontend: `npm run typecheck && npm run lint` exit 0.
- Frontend e2e: `npx playwright test --project=chromium` verde (workers=1 por flakiness mitigation 2026-04-23).
- Backend: `dotnet build src/Bitacora.sln` 0 errors.
- Backend tests: `dotnet test` verde cuando existan specs afectadas.
- Grep final de zonas congeladas: 0 cruces sobre el diff.

## 12) Alignment Rule

Keep this file aligned with `CLAUDE.md`. If the skill workflow policy changes, update both files in the same task.
