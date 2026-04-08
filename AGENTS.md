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

### Additional Mandatory Rules

- If editing `AGENTS.md` or `CLAUDE.md`, use `$ps-crear-agentsclaudemd`.
- If the change is large, risky, or multi-module, run `$ps-auditar-trazabilidad` before final closure.
- No default exceptions.
- In `$ps-contexto`, always read these docs before planning/execution:
   - `.docs/wiki/02_arquitectura.md` — service responsibilities and architecture
   - `.docs/wiki/05_modelo_datos.md` — canonical entities, column names, invariants, and states (when it exists)

## 2) Workflow Catalog (How Work Should Flow)

### A) Standard Task Flow
1. `$ps-contexto`
2. `$brainstorming`
3. Orchestrator partitioning
4. Execution / integration
5. `$ps-trazabilidad`

### B) Large / Risky / Multi-Module Change Flow
1. `$ps-contexto`
2. `$brainstorming`
3. Orchestrator partitioning + delegated execution
4. Integration
5. `$ps-trazabilidad`
6. `$ps-auditar-trazabilidad`

### C) AGENTS / CLAUDE Policy Change Flow
1. `$ps-contexto`
2. `$brainstorming`
3. `$ps-crear-agentsclaudemd`
4. Align both `AGENTS.md` and `CLAUDE.md`
5. `$ps-trazabilidad`

### D) Small / Trivial Task Flow (still mandatory)
1. `$ps-contexto` (short version allowed)
2. `$brainstorming` (brief pass)
3. Direct execution only if delegation is unnecessary
4. `$ps-trazabilidad`

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

## 6) Placeholder Mapping for Skills

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

## 7) Local Search Playbook (Fast + Repeatable)

### 1. Discover module/feature by keywords
```bash
rg -n "<keywords>" .docs/wiki/04_RF.md .docs/wiki/03_FL.md .docs/wiki/RF .docs/wiki/FL
```

### 2. Identify exact IDs
```bash
rg -n "RF-[A-Z]+-[0-9]+|FL-[A-Z]+-[0-9]+" .docs/wiki/04_RF.md .docs/wiki/03_FL.md
```

### 3. Open mandatory technical context
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

## 8) Documentation Synchronization Rule

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

## 9) Project Agents / MCP / Custom Tools

### Agents
- Backend: (TBD after `/crear-arquitectura`)
- Frontend: (TBD after `/crear-arquitectura`)

### MCP
- (TBD — to be configured when backend services are defined)

### Custom Tools
- `git status`: branch status quick check
- `git diff`: branch diff quick check

## 10) Language Rule + Orthography + Artifact Hygiene

- `AGENTS.md`, `CLAUDE.md`, and project-local skills must be written in English.
- Functional wiki docs under `.docs/wiki` may remain in Spanish.
- Avoid emojis in policy docs and governance outputs.
- All user-visible text must use correct Spanish orthography (tildes, n, inverted punctuation).
- Never create ephemeral artifacts in the repository root. Use `tmp/` or `artifacts/`.

## 11) Skill Purpose Map

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
