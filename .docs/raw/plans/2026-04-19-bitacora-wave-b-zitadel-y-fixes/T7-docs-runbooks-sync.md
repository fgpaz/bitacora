# Task T7: Docs And Runbooks Sync

## Shared Context
**Goal:** Align canonical docs and runbooks with Zitadel-only runtime.
**Stack:** Markdown wiki, runbooks, GitHub issue evidence.
**Architecture:** `CT-AUTH-ZITADEL.md` becomes active auth contract; Supabase auth is rollback-only legacy.

## Locked Decisions
- Do not mark Supabase as active runtime after Wave B.
- Do not close superseded issues until implementation is verified.
- Keep health-data security language explicit.

## Task Metadata
```yaml
id: T7
depends_on: [T4]
agent_type: ps-docs
files:
  - modify: .docs/wiki/02_arquitectura.md
  - modify: .docs/wiki/07_baseline_tecnica.md
  - modify: .docs/wiki/09_contratos_tecnicos.md
  - modify: .docs/wiki/09_contratos/CT-AUTH-ZITADEL.md
  - modify: .docs/wiki/09_contratos/CT-AUTH.md
  - modify: infra/runbooks/**
  - create: .docs/raw/reports/2026-04-19-bitacora-wave-b/docs-sync.md
complexity: medium
done_when: "docs-sync report states no active Supabase auth contradictions remain"
```

## Reference
`CT-AUTH-ZITADEL.md:146-154` lists docs forced by auth contract changes.

## Prompt
Update architecture, baseline, technical contracts, and runbooks so active runtime is Zitadel-only. Mark `CT-AUTH.md` as legacy rollback-only. Fix stale references that say login companion is missing or Wave A is partial. Update issue/status prose but do not mutate GitHub issues in this task unless explicitly assigned through T9.

## Execution Procedure
1. Search docs for `Supabase`, `GoTrue`, `SUPABASE_JWT_SECRET`, `sb-access-token`, and `Wave A PARTIAL`.
2. Update only active-runtime contradictions.
3. Preserve historical records by marking them historical, not rewriting past evidence.
4. Write docs sync report with changed paths and remaining follow-ups.

## Skeleton
```markdown
# Docs Sync

- Active contract: CT-AUTH-ZITADEL
- Legacy rollback-only: CT-AUTH
- Remaining Supabase references: historical only
```

## Verify
`rg -n "runtime actual|SUPABASE_JWT_SECRET|GoTrue|Wave A PARTIAL" .docs/wiki infra/runbooks` -> no active-runtime contradiction

## Commit
`docs(auth): sync bitacora zitadel runtime contract`
