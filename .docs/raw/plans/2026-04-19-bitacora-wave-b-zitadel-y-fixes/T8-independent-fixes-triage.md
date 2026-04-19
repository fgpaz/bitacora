# Task T8: Independent Fixes Triage

## Shared Context
**Goal:** Order open fixes without mixing independent backlog into auth cutover.
**Stack:** GitHub issues, docs report.
**Architecture:** Wave B supersedes Supabase/GoTrue blockers; DX/polish stays separate unless tests require it.

## Locked Decisions
- #2, #6, #14 are superseded only after Wave B is verified and merged.
- #3 is partially superseded; `/ingresar` is resolved by Wave B.
- #7, #9, #11, #12, #13 stay independent unless directly fixed.
- #10 must be revalidated after frontend changes.

## Task Metadata
```yaml
id: T8
depends_on: [T4]
agent_type: ps-worker
files:
  - create: .docs/raw/reports/2026-04-19-bitacora-wave-b/F1-fixes-triage.md
complexity: low
done_when: "F1 triage report exists with issue-by-issue disposition"
```

## Reference
`gh issue list --repo fgpaz/bitacora --state open --limit 40` shows #2/#3/#6/#7/#9/#10/#11/#12/#13/#14/#15/#17 open.

## Prompt
Create an issue triage report. Do not close or edit issues. State which issues Wave B can close after verification and which remain backlog. Include exact post-Wave-B closure criteria for each superseded issue.

## Execution Procedure
1. Run read-only `gh issue view` for #2, #3, #6, #7, #9, #10, #11, #12, #13, #14, #15, #17.
2. Record disposition and closure criteria.
3. Write `.docs/raw/reports/2026-04-19-bitacora-wave-b/F1-fixes-triage.md`.

## Skeleton
```markdown
| Issue | Disposition | Close When |
|---|---|---|
| #2 | superseded by Wave B | Zitadel session verified and merged |
```

## Verify
`Test-Path .docs/raw/reports/2026-04-19-bitacora-wave-b/F1-fixes-triage.md` -> `True`

## Commit
`docs(auth): triage wave b related fixes`
