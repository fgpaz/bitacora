# AGENTS.md - Project Operating Policy (Strict Orchestrator Mode)

## Project: Bitacora — Clinical Mood Tracker

**Slug:** bitacora.nuestrascuentitas.com
**Stack:** .NET 10 + Next.js 16 + Telegram Bot + PostgreSQL
**Auth:** Supabase Auth (shared at auth.tedi.nuestrascuentitas.com)
**Deploy:** Dokploy VPS 54.37.157.93

> **Health Data Sensitivity:** This project processes sensitive health data under Argentine Ley 25.326 (Data Protection), Ley 26.529 (Patient Rights), and Ley 26.657 (Mental Health). Any change to data storage, access control, consent flows, or audit logging must be reviewed for regulatory compliance before merge.

## 1) Orchestration Mode (MANDATORY - Always Active)

For EVERY task (`feature`, `bugfix`, `refactor`, `docs`, `infra`, `performance`):

1. Start with `$ps-contexto` before proposing changes or implementation.
2. After context load, run `$brainstorming` once before planning/execution.
3. Close critical context gaps before execution.
4. ALWAYS work in **Orchestrator Mode**.
5. Close the task with `$ps-trazabilidad` before marking it complete.

### Skill Invocation Semantics

| Skill | When | Mandatory |
|-------|------|-----------|
| `$ps-contexto` | At the start of EVERY task | Yes — before any action |
| `$mi-lsp` | For all code exploration under src/ | Yes — before raw grep/glob |
| `$brainstorming` | After context, before implementation | Yes — for non-trivial tasks |
| `$writing-plans` | Large/risky tasks, after brainstorming | Yes — for large tasks |
| `$ps-trazabilidad` | Before closing any task | Yes — verifies sync |
| `$ps-auditar-trazabilidad` | Large/risky/multi-module changes | Yes — cross-document audit |

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
- Use `$mi-lsp` as the mandatory semantic navigation tool for code exploration under `src/`. Fallback: rg.
- No default exceptions.
- In `$ps-contexto`, always read these docs before planning/execution:
   - `.docs/wiki/02_arquitectura.md` — service responsibilities and architecture
   - `.docs/wiki/05_modelo_datos.md` — canonical entities (when it exists)
   - `.docs/wiki/07_baseline_tecnica.md` — runtime baseline (when it exists)

## 2) Workflow Catalog (How Work Should Flow)

### A) Standard Task Flow
1. `$ps-contexto` — load project context
2. `$brainstorming` — challenge and lock design decisions
3. Orchestrated execution (subagents in parallel)
4. Documentation synchronization
5. `$ps-trazabilidad` — closure

### B) Large / Risky / Multi-Step Task Flow
1. `$ps-contexto` — load project context
2. `$brainstorming` — design and harden
3. `$writing-plans` — generate wave-dispatchable plan with subdocuments
4. Wave execution with subagents (run `$ps-trazabilidad` per batch)
5. `$ps-trazabilidad` — final closure
6. `$ps-auditar-trazabilidad` — read-only audit before marking done

### C) Policy-Change Flow (AGENTS.md / CLAUDE.md)
1. `$ps-contexto` → `$brainstorming` → `$ps-crear-agentsclaudemd` → `$ps-trazabilidad`

### D) Small / Trivial Task Flow
1. `$ps-contexto` → `$brainstorming` (lock assumptions) → execute → `$ps-trazabilidad`

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

**"Review PR before merge"** — launch 3 in parallel:
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
