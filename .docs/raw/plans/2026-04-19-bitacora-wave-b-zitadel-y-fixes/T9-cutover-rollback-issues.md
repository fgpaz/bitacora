# Task T9: Cutover, Rollback, And Issue Evidence

## Shared Context
**Goal:** Deploy/verify Zitadel-only Wave B and update #17/#15 evidence.
**Stack:** Dokploy, smoke scripts, Playwright, GitHub CLI.
**Architecture:** Supabase is rollback-only until acceptance, then retired.

## Locked Decisions
- Do not disable Supabase Auth infrastructure until Wave B acceptance and rollback evidence exist.
- Do not close superseded issues before verified merge/deploy evidence.
- Do not hand-edit board state if `pj-crear-tarjeta` can sync it.

## Task Metadata
```yaml
id: T9
depends_on: [T5, T6, T7, T8]
agent_type: ps-worker
files:
  - create: .docs/raw/reports/2026-04-19-bitacora-wave-b/T9-cutover-green.md
  - create: artifacts/e2e/2026-04-19-bitacora-wave-b-zitadel/README.md
complexity: high
done_when: "#17 evidence comment is posted and T9 cutover report exists"
```

## Reference
Project policy requires issue #17 evidence, smoke artifacts, epic #15 sync, `ps-trazabilidad`, and `ps-auditar-trazabilidad`.

## Prompt
Run the final deployment/smoke only after T5-T8 pass. Post a concise #17 comment with commit SHA, smoke artifact path, OIDC/JWKS status, frontend E2E result, backend smoke result, rollback path, and issue disposition. Update #15 status to reflect Wave B progress. Close #2/#6/#14 and #3 only if the verified implementation fully replaces their behavior and board sync is available.

## Execution Procedure
1. Confirm `git status --short` has only intentional Wave B changes.
2. Run backend build/tests, frontend build/tests, smoke, and Playwright.
3. Write final artifact README and T9 report.
4. Post #17 evidence comment.
5. Update #15 epic status.
6. Sync issue labels/status via approved project tooling.
7. Leave Supabase retirement as post-acceptance step unless explicitly approved.

## Skeleton
```markdown
# T9 Cutover Green

- Commit: <sha>
- Backend tests: pass
- Frontend tests: pass
- OIDC discovery/JWKS: 200/200
- Rollback: redeploy <previous sha/env>
```

## Verify
`Test-Path .docs/raw/reports/2026-04-19-bitacora-wave-b/T9-cutover-green.md` -> `True`

## Commit
`docs(auth): record wave b cutover evidence`
