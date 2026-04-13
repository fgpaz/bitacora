# Task T0: Create Plan Artifacts

## Shared Context
**Goal:** Persist the wave-prod-mvp plan to disk and commit it so subagents can read subdocuments.
**Stack:** Markdown plan files in `.docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp/`
**Architecture:** Plan directory structure: main plan `.md` + companion folder with per-task subdocs.

## Task Metadata
```yaml
id: T0
depends_on: []
agent_type: ps-worker
complexity: low
done_when: "Plan file + 4 subdocs exist on disk; git commit done"
```

## Prompt

1. Read the main plan file at `.docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp.md` to confirm it exists.
2. Create these 4 subdocument files (use the Write tool for each):
   - `.docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp/T1-frontend-docker-fix.md`
   - `.docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp/T2-e2e-test-prompt.md`
   - `.docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp/T3-telegram-webhook-handlers.md`
   - `.docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp/T4-wiki-sync.md`
3. Each subdoc must follow the writing-plans subdocument template: Shared Context (max 4 lines), YAML metadata block, Reference, Prompt, Skeleton, Verify, Commit.
4. For now, write the T1/T2/T3/T4 subdocs with "TODO: flesh out prompt section" placeholder in the Prompt field — the actual fleshed-out prompts will be written after T0 is verified.
5. After writing all 4 files, run: `ls -la .docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp/` to confirm all files exist.
6. Commit: `git add .docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp.md .docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp/` and `git commit -m "docs(plan): add bitacora-wave-prod-mvp implementation plan"`.

## Verify
`ls .docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp/` shows main plan + 4 subdocs; `git log --oneline -1 .docs/raw/plans/` confirms commit.

## Commit
`docs(plan): add bitacora-wave-prod-mvp implementation plan`
