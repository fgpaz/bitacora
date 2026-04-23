# CLAUDE.md - Project Operating Policy (Strict Orchestrator Mode)

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
  2. If no anchor exists, create it first via `Skill(ps-docs)` or the matching `crear-*` skill.
  3. Launch `ps-explorer` (uses `mi-lsp`) to confirm anchor and load context. Minimum: `mi-lsp nav pack "<task>"` and `mi-lsp nav ask "<question>"`.
  4. No write subagent (`ps-dotnet10`, `ps-next-vercel`, `ps-python`) starts before step 3.

Non-compliant: implementing without citing an `RF-*`/`FL-*`/`CT-*`, or skipping `ps-explorer`.

## Workflow Stance (XP on `main`, Mandatory)

Bitacora works in **Extreme Programming sobre `main`**: push directo a `main`, sin PRs, sin feature branches de larga duracion.

- Commit atomico a `main` tras validacion local (typecheck + lint + tests aplicables + `dotnet build` cuando toca backend).
- `git push origin main` como cierre estandar. No se abren PRs.
- NO crear feature branches salvo que el humano lo pida explicitamente para experimentacion.
- Cada commit lleva el trailer `- Gabriel Paz -`.
- Closure docs siguen obligatorios para tareas grandes (`.docs/raw/reports/*-closure.md`). XP acelera el merge, no suprime la traceability.
- Safeguards preservados: acciones de alto blast radius (prod SQL, `--force push`, `rm -rf`, modificar Dokploy prod) siguen requiriendo confirmacion explicita incluso bajo XP-on-main.
- Antes de cada push a `main`, ejecutar `Skill(ps-pre-push)` como gate final tras `Skill(ps-trazabilidad)` (y `Skill(ps-auditar-trazabilidad)` cuando corresponda).
- Nunca commitear ni pushear archivos bajo `.docs/raw/` que sean scratch local; solo promover a `.docs/raw/decisiones/`, `.docs/raw/reports/` o equivalentes cuando la evidencia deba persistir.

## 0) Skill Invocation Semantics (Critical)

Names like `ps-contexto`, `brainstorming`, `ps-trazabilidad`, `ps-auditar-trazabilidad`, and `ps-crear-agentsclaudemd` are **skills/workflows**, not shell commands. Do not execute skill names directly in terminal.

**Claude Code syntax:**
- Use `Skill(name)` to invoke skills
- Use `AskUserQuestion` to ask the user questions
- Use `Agent` tool with `subagent_type` to delegate to specialized agents

| Skill | When | Mandatory |
|-------|------|-----------|
| `ps-contexto` | At the start of EVERY task | Yes — before any action |
| `mi-lsp` | First exploration tool for repo and code investigation | Yes — validate workspace alias/path first; before raw grep/glob/read loops |
| `brainstorming` | After context, before implementation | Yes — for non-trivial tasks |
| `writing-plans` | Large/risky tasks, after brainstorming | Yes — for large tasks |
| `ps-trazabilidad` | Before closing any task | Yes — verifies sync |
| `ps-auditar-trazabilidad` | Large/risky/multi-module changes | Yes — cross-document audit |
| `ps-pre-push` | Before any `git push origin main`, after traceability/audit | Yes — final XP-on-main gate |

## 1) Orchestration Mode (MANDATORY - Always Active)

For EVERY task (`feature`, `bugfix`, `refactor`, `docs`, `infra`, `performance`):

1. Start with `Skill(ps-contexto)` before proposing changes or implementation.
2. Run `mi-lsp` as the first exploration tool. Validate the workspace alias/path before assuming one, then use it before raw grep/glob/read loops.
3. After context and exploration, run `Skill(brainstorming)` once before planning/execution.
4. Close critical context gaps before execution:
   - Use `AskUserQuestion` when available in the current mode.
   - If unavailable, ask directly in chat and continue only after critical gaps are closed.
5. ALWAYS work in **Orchestrator Mode**:
   - Prefer delegating exploration and code-writing with `Skill(dispatching-parallel-agents)` when work can be partitioned safely.
   - Keep direct execution for trivial actions, integration, or non-delegable steps.
6. Close the task with `Skill(ps-trazabilidad)` before marking it complete.

### Brainstorming Question Protocol (Mandatory)

Before every clarifying question, provide:
1) Learning context
2) Why the question matters
3) A small ASCII diagram showing proposed change
4) Per-option pros and cons
5) Explicit recommended option

Use `AskUserQuestion` as the questioning tool in Claude Code.

### Registered Subagent Catalog

| Domain | Subagent Type | Purpose | Mode |
|--------|--------------|---------|------|
| Exploration | ps-explorer | Search symbols, trace paths, read code | read-only |
| Backend | ps-dotnet10 | Generate/modify .NET 10 code (Bitacora.Api) | write |
| Frontend | ps-next-vercel | Generate/modify Next.js 16 code | write |
| Python | ps-python | Scripts, Telegram utilities, tooling | write |
| Code review | ps-code-reviewer | Review diffs, audit quality P>D>S | read-only |
| QA orchestrator | ps-qa-orchestrator | Quality + security + testing audit | read-only |
| QA security | ps-qa-backend-security | Backend security compliance | read-only |
| Gap audit | ps-gap-auditor | Spec-vs-code gap detection | read-only |
| SDD sync | ps-sdd-sync-gen | Auto-generate specs from code | write |
| Docs | ps-docs | Wiki, specs, READMEs, changelogs | write |
| Worker | ps-worker | Git, config, shell, plans, ops | write |

### Additional Mandatory Rules

- If editing `AGENTS.md` or `CLAUDE.md`, use `Skill(ps-crear-agentsclaudemd)`.
- If the change is large, risky, or multi-module, run `Skill(ps-auditar-trazabilidad)` before final closure.
- Use `mi-lsp` as the mandatory first exploration tool for repo and code navigation. Validate the workspace alias/path with `mi-lsp workspace list` and `mi-lsp workspace status <alias-or-path> --format toon` before assuming a workspace name. If `mi-lsp` returns `hint` or `next_hint`, follow that rerun guidance before retrying. Fallback order: `mi-lsp` → `rg`, and only after `mi-lsp` has been tried.
- No default exceptions. If exceptions are allowed later, document them explicitly here.
- In `Skill(ps-contexto)`, always read these docs before planning/execution:
   - `.docs/wiki/02_arquitectura.md` — service responsibilities and architecture
   - `.docs/wiki/05_modelo_datos.md` — canonical entities, column names, invariants, and states (when it exists)
   - `.docs/wiki/07_baseline_tecnica.md` — runtime baseline and invariants (when it exists)

## 2) Workflow Catalog (How Work Should Flow)

### A) Standard Task Flow
1. `Skill(ps-contexto)` — load project context
2. `mi-lsp` — validate workspace alias/path and explore first
3. `Skill(brainstorming)` — challenge and lock design decisions
4. Orchestrated execution (subagents in parallel)
5. Documentation synchronization
6. `Skill(ps-trazabilidad)` — closure

### B) Large / Risky / Multi-Step Task Flow
1. `Skill(ps-contexto)` — load project context
2. `mi-lsp` — validate workspace alias/path and explore first
3. `Skill(brainstorming)` — design and harden
4. `Skill(writing-plans)` — generate wave-dispatchable plan with subdocuments
5. Wave execution with subagents (run `Skill(ps-trazabilidad)` per batch)
6. `Skill(ps-trazabilidad)` — final closure
7. `Skill(ps-auditar-trazabilidad)` — read-only audit before marking done

### C) AGENTS / CLAUDE Policy Change Flow
1. `Skill(ps-contexto)` → `mi-lsp` → `Skill(brainstorming)` → `Skill(ps-crear-agentsclaudemd)` → `Skill(ps-trazabilidad)`

### D) Small / Trivial Task Flow (still mandatory)
1. `Skill(ps-contexto)` → `mi-lsp` → `Skill(brainstorming)` (lock assumptions) → execute → `Skill(ps-trazabilidad)` → `Skill(ps-pre-push)` before pushing to `main`

### E) Pre-Push Flow for XP on `main`
1. `Skill(ps-trazabilidad)` debe estar completo.
2. `Skill(ps-auditar-trazabilidad)` debe estar completo para cambios grandes/riesgosos, multi-modulo, policy-changing, o closure de waves.
3. Tests aplicables verdes (frontend: typecheck + lint + e2e; backend: `dotnet build` + tests unitarios cuando existan).
4. Grep de zonas congeladas (`lib/auth/*`, `proxy.ts`, `app/api/*`, `app/auth/*`, `src/` frontend) debe dar 0 cruces cuando el scope lo requiera.
5. Sin archivos `.docs/raw/` de scratch agregados al commit.
6. `Skill(ps-pre-push)` debe retornar `Approved` o `Approved with waiver` antes de `git push origin main`.

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
Subagents run on degraded models (Sonnet in Claude Code). They are fast and cheap but:
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

| Task | Subagent | Type |
|------|----------|------|
| Explore code, find symbols | ps-explorer | read-only |
| Generate/modify .NET 10 code | ps-dotnet10 | write |
| Generate/modify Next.js code | ps-next-vercel | write |
| Generate/modify Python code | ps-python | write |
| Code review, audit quality | ps-code-reviewer | read-only |
| QA: quality + security + testing | ps-qa-orchestrator | read-only |
| Backend security compliance | ps-qa-backend-security | read-only |
| Gap detection (docs vs code) | ps-gap-auditor | read-only |
| Spec-code sync | ps-sdd-sync-gen | write |
| Git, config, shell, plans, ops | ps-worker | write |
| Create/update docs, wiki, specs | ps-docs | write |

### Dispatch Examples

**"Implement CRUD endpoint"** — launch 5 in parallel:
1. ps-explorer: "Find if MoodEntry entity exists in Domain/Entities"
2. ps-explorer: "List existing endpoints in Api/Endpoints/ and their pattern"
3. ps-explorer: "Find test patterns in Tests/"
4. ps-dotnet10: "Create POST /api/v1/mood-entries endpoint with Command and Handler"
5. ps-dotnet10: "Create GET /api/v1/mood-entries endpoint with Query and pagination"

**"Review diff before push to main"** (XP-on-main) — launch 3 in parallel:
1. ps-code-reviewer: "Review the diff, focus on P0 performance and security"
2. ps-explorer: "Check if changed files have tests"
3. ps-qa-orchestrator: "Audit quality and security on diff files"

**"Close documentation gaps"** — launch 3 in parallel:
1. ps-gap-auditor: "Scan FL → RF → TP traceability for missing links"
2. ps-explorer: "Find all RF that reference entities not in 05_modelo_datos.md"
3. ps-docs: "Update 09_contratos_tecnicos.md with any new endpoints"

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

## 9) Language Rule (Project Policy)

- `AGENTS.md`, `CLAUDE.md`, and project-local skills must be written in English.
- Functional wiki docs under `.docs/wiki` may remain in Spanish.
- Avoid emojis in policy docs and governance outputs.

## 9.1) Spanish Orthography Rule (Mandatory)

All user-visible text must use correct Spanish orthography:
- Tildes mandatory: a, e, i, o, u.
- n must never be replaced with n.
- Inverted punctuation required: ?, !.
- Characters must survive linting and formatting.

## 9.2) Artifact Hygiene Rule (Mandatory)

- Never create ephemeral artifacts in the repository root or arbitrary source folders.
- Default destinations:
  - `tmp/` for short-lived scratch (delete before task closure).
  - `artifacts/` for outputs that survive for inspection.
  - `artifacts/e2e/<YYYY-MM-DD>-<task-slug>/` for E2E/browser evidence.
- Before marking a task complete, delete ephemeral artifacts or move to canonical folder.

## 10) Skill Purpose Map

- `ps-contexto`: Project context bootstrap + precedence method
- `brainstorming`: Post-context challenge and decision lock
- `dispatching-parallel-agents`: Orchestrator delegation for independent work
- `writing-plans`: Wave-dispatchable implementation plans
- `ps-trazabilidad`: Task closure traceability and documentation sync
- `ps-auditar-trazabilidad`: Read-only audit for large/risky changes
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

- Todo trabajo cierra con `git push origin main` tras `Skill(ps-pre-push)`.
- Sin PRs, sin `gh pr create`, sin feature branches.
- Commits atomicos con trailer `- Gabriel Paz -`.
- Si un cambio grande requiere waves, cada wave cierra con su propio `Skill(ps-trazabilidad)` + commit directo a `main` (no acumular en feature branch).

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
- Ausencia de closure doc en una tarea que lo amerita es **gap de traceability** y bloqueante para `Skill(ps-pre-push)`.

### Tests obligatorios pre-push

- Frontend: `npm run typecheck && npm run lint` exit 0.
- Frontend e2e: `npx playwright test --project=chromium` verde (workers=1 por flakiness mitigation 2026-04-23).
- Backend: `dotnet build src/Bitacora.sln` 0 errors.
- Backend tests: `dotnet test` verde cuando existan specs afectadas.
- Grep final de zonas congeladas: 0 cruces sobre el diff.

### Evidence docs convention (Bitacora)

Bitacora mantiene evidence docs bajo `.docs/raw/` (distinto de BuhoSalud que usa `.docs/auditoria/`). Taxonomia:

| Carpeta | Proposito | Durabilidad | Reconocido por guard como evidence |
|---------|-----------|-------------|------------------------------------|
| `.docs/raw/reports/*-closure.md` | Closure docs de tareas grandes | Durable (committable) | Si |
| `.docs/raw/decisiones/` | Decision docs (legal, retention, architecture) | Durable | Si |
| `.docs/raw/investigacion/` | Research / evidencia externa | Durable | Si |
| `.docs/raw/plans/` | Plans wave-dispatchable | Promotable scratch (warning al commitear) | No (directo) |
| `.docs/raw/prompts/` | Prompts canonicos de input | Promotable scratch (warning al commitear) | No (directo) |
| `.docs/raw/<otro>` | Sin categorizar | Blocked por guard | No |

El guard `infra/git/Invoke-PrePushGuard.ps1` reconoce `.docs/raw/reports/`, `.docs/raw/decisiones/` y `.docs/raw/investigacion/` como evidence paths validos. Tambien acepta `.docs/auditoria/` y `.docs/planificacion/` si se adoptan mas adelante.

### Pre-push guard script

Ubicacion: `infra/git/Invoke-PrePushGuard.ps1`.

Invocacion normal:

```powershell
pwsh -NoProfile -File .\infra\git\Invoke-PrePushGuard.ps1 -ExpectedScope policy,git-tooling -TraceabilityEvidence .docs/raw/reports/2026-04-23-login-flow-followups-v2-closure.md
```

Dry-run (no push):

```powershell
pwsh -NoProfile -File .\infra\git\Invoke-PrePushGuard.ps1 -ExpectedScope policy -TraceabilityEvidence <path> -DryRun
```

Con waiver (sin issue/card en board, sin evidence doc dedicado, cambios de policy de baja superficie):

```powershell
pwsh -NoProfile -File .\infra\git\Invoke-PrePushGuard.ps1 -WaiverReason "Policy patch chat-approved; self-contained" -ExpectedScope policy
```

Exit codes: `0` Approved o Approved with waiver; `2` Blocked.

Ver `infra/runbooks/pre-push-guard.md` para runbook completo.

## 12) Alignment Rule

Keep this file aligned with `AGENTS.md`. If the skill workflow policy changes, update both files in the same task.

## 13) Runtime tooling

### dokploy-cli
- Script: `~/.claude/skills/dokploy-cli/scripts/dkp.ps1` (Windows) or `scripts/dkp.sh` (POSIX)
- Auth: `DOKPLOY_API_KEY` + `DOKPLOY_URL` in `infra/.env` (searched upward from CWD)
- Override: set `DOKPLOY_API_KEY` and `DOKPLOY_URL` directly in environment
- Header: `x-api-key` (NOT `Authorization: Bearer`)
- Verify: `bash ~/.claude/skills/dokploy-cli/scripts/dkp.sh doctor`

### mi-key-cli (mkey)
- Script: `~/.claude/skills/mi-key-cli/scripts/mkey.ps1` (Windows) or `scripts/mkey.sh` (POSIX)
- Project config: `infra/mkey.yaml` (vault `teslita`, project `bitacora`, default env `prod`)
- Verify: `bash ~/.claude/skills/mi-key-cli/scripts/mkey.sh doctor`
- Pull secrets: `bash ~/.claude/skills/mi-key-cli/scripts/mkey.sh pull bitacora prod`
- Set secret: `bash ~/.claude/skills/mi-key-cli/scripts/mkey.sh set bitacora <KEY> <VALUE> --env prod`
- PowerShell note: initialize `$LASTEXITCODE = 0` before first `mkey set` in a fresh session (strict mode workaround).
