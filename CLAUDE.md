# CLAUDE.md - Project Operating Policy (Strict Orchestrator Mode)

## Project: Bitacora — Clinical Mood Tracker

**Slug:** bitacora.nuestrascuentitas.com
**Stack:** .NET 10 + Next.js 16 + Telegram Bot + PostgreSQL
**Auth:** Supabase Auth (shared at auth.tedi.nuestrascuentitas.com)
**Deploy:** Dokploy VPS 54.37.157.93

> **Health Data Sensitivity:** This project processes sensitive health data under Argentine Ley 25.326 (Data Protection), Ley 26.529 (Patient Rights), and Ley 26.657 (Mental Health). Any change to data storage, access control, consent flows, or audit logging must be reviewed for regulatory compliance before merge.

## 0) Skill Invocation Semantics (Critical)

Names like `ps-contexto`, `brainstorming`, `ps-trazabilidad`, `ps-auditar-trazabilidad`, and `ps-crear-agentsclaudemd` are **skills/workflows**, not shell commands. Do not execute skill names directly in terminal.

**Claude Code syntax:**
- Use `Skill(name)` to invoke skills
- Use `AskUserQuestion` to ask the user questions
- Use `Agent` tool with `subagent_type` to delegate to specialized agents

## 1) Orchestration Mode (MANDATORY - Always Active)

For EVERY task (`feature`, `bugfix`, `refactor`, `docs`, `infra`, `performance`):

1. Start with `Skill(ps-contexto)` before proposing changes or implementation.
2. After context load, run `Skill(brainstorming)` once before planning/execution.
3. Close critical context gaps before execution:
   - Use `AskUserQuestion` when available in the current mode.
   - If unavailable, ask directly in chat and continue only after critical gaps are closed.
4. ALWAYS work in **Orchestrator Mode**:
   - Prefer delegating exploration and code-writing with `Skill(dispatching-parallel-agents)` when work can be partitioned safely.
   - Keep direct execution for trivial actions, integration, or non-delegable steps.
5. Close the task with `Skill(ps-trazabilidad)` before marking it complete.

### Brainstorming Question Protocol (Mandatory)

Before every clarifying question, provide:
1) Learning context
2) Why the question matters
3) A small ASCII diagram showing proposed change
4) Per-option pros and cons
5) Explicit recommended option

Use `AskUserQuestion` as the questioning tool in Claude Code.

### Registered Subagent Catalog

| Domain    | Subagent Type       | Purpose                       |
|-----------|---------------------|-------------------------------|
| (to be populated after `/crear-arquitectura` when service boundaries are known) | | |

### Additional Mandatory Rules

- If editing `AGENTS.md` or `CLAUDE.md`, use `Skill(ps-crear-agentsclaudemd)`.
- If the change is large, risky, or multi-module, run `Skill(ps-auditar-trazabilidad)` before final closure.
- No default exceptions. If exceptions are allowed later, document them explicitly here.
- In `Skill(ps-contexto)`, always read these docs before planning/execution:
   - `.docs/wiki/02_arquitectura.md` — service responsibilities and architecture
   - `.docs/wiki/05_modelo_datos.md` — canonical entities, column names, invariants, and states (when it exists)

## 2) Workflow Catalog (How Work Should Flow)

### A) Standard Task Flow
1. `Skill(ps-contexto)`
2. `Skill(brainstorming)`
3. Orchestrator partitioning (`Skill(dispatching-parallel-agents)` when applicable)
4. Execution / integration
5. `Skill(ps-trazabilidad)`

### B) Large / Risky / Multi-Module Change Flow
1. `Skill(ps-contexto)`
2. `Skill(brainstorming)`
3. Orchestrator partitioning + delegated execution
4. Integration
5. `Skill(ps-trazabilidad)`
6. `Skill(ps-auditar-trazabilidad)` (read-only audit before final completion)

### C) AGENTS / CLAUDE Policy Change Flow
1. `Skill(ps-contexto)`
2. `Skill(brainstorming)`
3. `Skill(ps-crear-agentsclaudemd)`
4. Align both `AGENTS.md` and `CLAUDE.md`
5. `Skill(ps-trazabilidad)`

### D) Small / Trivial Task Flow (still mandatory)
1. `Skill(ps-contexto)` (short version allowed)
2. `Skill(brainstorming)` (brief pass)
3. Direct execution only if delegation is unnecessary
4. `Skill(ps-trazabilidad)`

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

- Governance: `.docs/wiki/00_gobierno_documental.md`
- Scope: `.docs/wiki/01_alcance_funcional.md`
- Architecture: `.docs/wiki/02_arquitectura.md`
- Flow index: `.docs/wiki/03_FL.md`
- Flow docs: `.docs/wiki/FL/FL-*.md`
- RF index: `.docs/wiki/04_RF.md`
- RF module docs: `.docs/wiki/RF/RF-*.md`
- Data model: `.docs/wiki/05_modelo_datos.md`
- Test matrix index: `.docs/wiki/06_matriz_pruebas_RF.md`
- Test module docs: `.docs/wiki/pruebas/TP-*.md`
- Research: `.docs/raw/investigacion/`
- Decisions: `.docs/raw/decisiones/`

## 5) Placeholder Mapping for Skills

- `<ALCANCE_DOC>` -> `.docs/wiki/01_alcance_funcional.md`
- `<ARQUITECTURA_DOC>` -> `.docs/wiki/02_arquitectura.md`
- `<FL_INDEX_DOC>` -> `.docs/wiki/03_FL.md`
- `<FL_DOCS_DIR>` -> `.docs/wiki/FL`
- `<RF_INDEX_DOC>` -> `.docs/wiki/04_RF.md`
- `<RF_DOCS_DIR>` -> `.docs/wiki/RF`
- `<MODELO_DATOS_DOC>` -> `.docs/wiki/05_modelo_datos.md`
- `<TP_INDEX_DOC>` -> `.docs/wiki/06_matriz_pruebas_RF.md`
- `<TP_DOCS_DIR>` -> `.docs/wiki/pruebas`
- `<GOBIERNO_DOC>` -> `.docs/wiki/00_gobierno_documental.md`

## 6) Local Search Playbook (Fast + Repeatable)

### 1. Discover module/feature by keywords
```bash
rg -n "<keywords>" .docs/wiki/04_RF.md .docs/wiki/03_FL.md .docs/wiki/RF .docs/wiki/FL
```

### 2. Identify exact IDs (`RF-*` / `FL-*`)
```bash
rg -n "RF-[A-Z]+-[0-9]+|FL-[A-Z]+-[0-9]+" .docs/wiki/04_RF.md .docs/wiki/03_FL.md
```

### 3. Open mandatory technical context (RF / FL / data / architecture)
```bash
rg -n "<RF-ID>|<keyword>" .docs/wiki/RF/RF-*.md
rg -n "<FL-ID>|<keyword>" .docs/wiki/FL/FL-*.md
rg -n "<entity|state|field>" .docs/wiki/05_modelo_datos.md .docs/wiki/RF
rg -n "<endpoint|queue|component>" .docs/wiki/02_arquitectura.md .docs/wiki/FL .docs/wiki/RF
```

### 4. Verify test traceability before closure
```bash
rg -n "<RF-ID>|TP-<MOD>" .docs/wiki/06_matriz_pruebas_RF.md .docs/wiki/pruebas
```

## 7) Documentation Synchronization Rule

If any of these changes:
- I/O contracts
- Typed errors
- Entities/states
- Flow steps

Then review and update (if applicable):
- `.docs/wiki/RF/RF-<MOD>.md`
- `.docs/wiki/FL/FL-<MOD>-<NN>.md`
- `.docs/wiki/05_modelo_datos.md`
- `.docs/wiki/04_RF.md`
- `.docs/wiki/06_matriz_pruebas_RF.md`
- `.docs/wiki/pruebas/TP-<MOD>.md`

## 8) Project Agents / MCP / Custom Tools

### Agents
- Backend: (TBD after `/crear-arquitectura`)
- Frontend: (TBD after `/crear-arquitectura`)

### MCP (Project-Preferred)
- (TBD — to be configured when backend services are defined)

### Custom Tools / Shortcuts (Project Convention)
- `git status`: branch status quick check
- `git diff`: branch diff quick check

## 9) Language Rule (Project Policy)

- `AGENTS.md`, `CLAUDE.md`, and project-local skills must be written in English.
- Functional wiki docs under `.docs/wiki` may remain in Spanish.
- Avoid emojis in policy docs and governance outputs.

## 9.1) Spanish Orthography Rule (Mandatory)

All user-visible text — UI labels, copy, messages, `aria-label`, tooltips, placeholder, empty states, loading messages — must use correct Spanish orthography:

- Tildes are mandatory: a, e, i, o, u, A, E, I, O, U.
- n/N must never be replaced with n/N.
- u must be preserved in loanwords (e.g., "pinguino").
- Inverted punctuation is required at sentence start: ?, !.
- These characters must survive linting, formatting, and code generation without being stripped or escaped.

Enforcement:
- Before committing user-visible string changes, verify accented characters are intact.
- Auto-formatters (Prettier, ESLint, etc.) must not be configured to escape non-ASCII in JSX strings.
- If a tool strips these characters, fix the tool config — do not sacrifice correct Spanish.

## 9.2) Artifact Hygiene Rule (Mandatory)

- Never create ephemeral artifacts in the repository root or in arbitrary source folders.
- Default destinations are:
  - `tmp/` for short-lived scratch outputs that must be deleted before task closure.
  - `artifacts/` for outputs that need to survive the task for inspection or handoff.
- Before marking a task complete, delete ephemeral artifacts or move retained evidence to its canonical artifact folder.

## 10) Skill Purpose Map

- `ps-contexto`: Project context bootstrap + precedence method
- `brainstorming`: Post-context challenge and decision lock
- `dispatching-parallel-agents`: Orchestrator delegation for independent work
- `ps-trazabilidad`: Task closure traceability and documentation sync
- `ps-auditar-trazabilidad`: Read-only audit for large/risky changes
- `ps-crear-agentsclaudemd`: Policy wiring for `AGENTS.md` / `CLAUDE.md`
- `crear-alcance`: Generate scope document (01_alcance_funcional.md)
- `crear-arquitectura`: Generate architecture document (02_arquitectura.md)
- `crear-flujo`: Generate flow documents (FL-*.md)
- `crear-requerimiento`: Generate requirement documents (RF-*.md)
- `crear-modelo-de-datos`: Generate data model document (05_modelo_datos.md)
- `ps-asistente-wiki`: SDD workflow navigation and phase gate validation
- `ps-investigar`: Research and evidence gathering

## 11) Alignment Rule

Keep this file aligned with `AGENTS.md`. If the skill workflow policy changes, update both files in the same task.
