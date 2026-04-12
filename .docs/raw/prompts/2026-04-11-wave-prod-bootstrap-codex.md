<!--
target: codex
pressure: aggressive
generated: 2026-04-11
topic: wave-prod-bootstrap
-->

# Wave-Prod Bootstrap Prompt

```text
Start a new Codex session to begin executing the active `wave-prod` portfolio for Bitácora.

Mission:
- Start `wave-prod` from the first active phase: `.docs/plans/wave-prod/10-docs-normalizacion-canon.md`.
- This kickoff is documentation-only. Do not jump to hardening, code generation, runtime hardening, or UX validation yet.

Use these skills:
- `$ps-contexto` — mandatory first action
- `$mi-lsp` — mandatory semantic navigation under `src/`
- `$brainstorming` — mandatory after context and exploration
- `$ps-asistente-wiki` — mandatory because this kickoff is wiki/spec work
- `$ps-trazabilidad` — mandatory closure
- `$ps-auditar-trazabilidad` — mandatory because this is a large, risky, multi-module phase

Treat this as repo-first work. Verify the actual repo state before trusting any sentence in this prompt.

Mandatory exploration before planning or execution:
- Use `$mi-lsp` first for semantic navigation under `src/`.
- Do not use raw grep/glob on `src/` until `$mi-lsp` has been tried.
- Dispatch at least 5 `ps-explorer` probes in parallel if your harness/tooling allows delegation. If your harness does not allow subagent delegation in this run, say so explicitly and perform the equivalent exploration yourself with `$mi-lsp` plus targeted file inspection.
- Use these minimum exploration objectives:
  1. Verify current backend runtime shape from `src/Bitacora.Api/Program.cs` and the existing endpoint modules under `src/Bitacora.Api/Endpoints/`.
  2. Verify current domain and persistence truth under `src/Bitacora.Domain`, `src/Bitacora.DataAccess.Interface`, and `src/Bitacora.DataAccess.EntityFramework`, including whether `CareLink`, `BindingCode`, and `TelegramSession` exist.
  3. Verify current functional/technical canon under `.docs/wiki/03_FL`, `.docs/wiki/04_RF`, `.docs/wiki/05_modelo_datos.md`, `.docs/wiki/07_baseline_tecnica.md`, `.docs/wiki/08_modelo_fisico_datos.md`, and `.docs/wiki/09_contratos_tecnicos.md`.
  4. Verify current UX/UI canon and gating under `.docs/wiki/21_matriz_validacion_ux.md` and `.docs/wiki/23_uxui/`.
  5. Verify the active portfolio under `.docs/plans/wave-prod/` and confirm it supersedes `.docs/raw/plans/wave-1/` for pending work.
  6. Verify current TP/runbook/ops surfaces under `.docs/wiki/06_matriz_pruebas_RF.md`, `.docs/wiki/06_pruebas/`, and `infra/`.
- Cross-check exploration results before acting. If two probes contradict each other, resolve the contradiction before editing docs.

Workflow:
1. `$ps-contexto`
   Read at minimum:
   - `AGENTS.md`
   - `CLAUDE.md`
   - `.docs/wiki/02_arquitectura.md`
   - `.docs/wiki/05_modelo_datos.md`
   - `.docs/wiki/07_baseline_tecnica.md`
   - `.docs/plans/wave-prod/INDEX.md`
   - `.docs/plans/wave-prod/10-docs-normalizacion-canon.md`
2. Mandatory exploration block
   Use `$mi-lsp` for `src/` verification, plus targeted doc reads.
3. `$brainstorming`
   Lock whether the repo still matches the committed `wave-prod` assumptions or whether plan-vs-repo drift must be reconciled before execution.
4. `$ps-asistente-wiki`
   Confirm the current documentation phase is still the correct next step for a `drift-reconciliation / docs normalization` kickoff.
5. Execute `.docs/plans/wave-prod/10-docs-normalizacion-canon.md`
   - Read and execute the subdocs in order:
     - `.docs/plans/wave-prod/10-docs-normalizacion-canon/T1-canon-gap-map.md`
     - `.docs/plans/wave-prod/10-docs-normalizacion-canon/T2-functional-canon-sync.md`
     - `.docs/plans/wave-prod/10-docs-normalizacion-canon/T3-technical-canon-sync.md`
     - `.docs/plans/wave-prod/10-docs-normalizacion-canon/T4-tp-runbook-crosslinks.md`
   - Work wave by wave.
   - If repo truth still matches the plan, do not regenerate plans.
   - Only rerun `$writing-plans` if you find material repo drift that invalidates `064d277`.
6. Run `$ps-trazabilidad`
7. Run `$ps-auditar-trazabilidad`

Verified repo state as of 2026-04-11:
- Active planning portfolio: `.docs/plans/wave-prod/`
- Portfolio commit: `064d277 docs(plan): add wave-prod portfolio`
- `wave-prod` is active for pending work; `.docs/raw/plans/wave-1/` is historical only
- Current backend runtime still exposes:
  - `app.MapAuthEndpoints();`
  - `app.MapConsentEndpoints();`
  - `app.MapRegistroEndpoints();`
- `CareLink` is not implemented in `src/`
- `TelegramSession` is not implemented in `src/`
- `frontend/` does not exist yet
- `ONB-001` is the only patient slice already open at `UI-RFC + HANDOFF` level
- Final UX/UI validation is deferred to `.docs/plans/wave-prod/60-validacion-final-trazabilidad.md`

Locked decisions you must not reopen:
- Spec-driven sequence is fixed:
  - documentation first
  - then pre-code hardening
  - then code generation
  - then runtime hardening
  - then final validation and closure
- Do not jump to Phase 11, 20, 30, 31, 40, 41, 50, or 60 until Phase 10 is actually closed or explicitly blocked.
- Do not treat `wave-1` as active authority for pending work.
- Do not mark any UX/UI slice as validated during this kickoff.
- Do not create `frontend/` in this kickoff.
- Do not implement `CareLink`, `BindingCode`, or `TelegramSession` in this kickoff.

Primary sources to read first:
- `AGENTS.md`
- `CLAUDE.md`
- `.docs/plans/wave-prod/INDEX.md`
- `.docs/plans/wave-prod/10-docs-normalizacion-canon.md`
- `.docs/wiki/02_arquitectura.md`
- `.docs/wiki/05_modelo_datos.md`
- `.docs/wiki/07_baseline_tecnica.md`
- `.docs/wiki/09_contratos_tecnicos.md`
- `.docs/wiki/06_matriz_pruebas_RF.md`
- `.docs/raw/plans/2026-04-10-wave-1-productivizacion.md` only as historical context, not active authority

Boundaries:
- Scope is Phase 10 only.
- This is docs/spec work only.
- Code edits are not allowed unless you uncover a tiny blocking documentation-readability issue that cannot be expressed another way; if that happens, stop and explain instead of freelancing.
- Preserve these existing untracked prompt artifacts:
  - `.docs/raw/prompts/2026-04-10-wave-1-productivizacion-bootstrap.md`
  - `.docs/raw/prompts/2026-04-10-wave-1-wave-0-prod-bootstrap-followups-codex.md`

Severity rules:
- If the repo contradicts this prompt, trust the repo and name the contradiction explicitly.
- If the canon claims implementation that the repo does not support, fix the canon first.
- If you find stale or double authority between `wave-prod` and `wave-1`, treat it as a real defect.
- If a technical contract or runtime statement is ambiguous, treat that as a blocker for downstream implementation.
- If you detect plan-vs-repo drift large enough to invalidate Phase 10 assumptions, stop, explain the exact drift, and only then decide whether replanning is needed.

Expected deliverable for this session:
- Complete Phase 10 from `.docs/plans/wave-prod/10-docs-normalizacion-canon.md`
- Leave the normalized docs persisted to disk
- End with a concise summary of:
  - what Phase 10 changed
  - any blockers found
  - `$ps-trazabilidad` result
  - `$ps-auditar-trazabilidad` result
```
