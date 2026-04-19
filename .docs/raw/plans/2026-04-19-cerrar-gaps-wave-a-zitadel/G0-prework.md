# Task G0: Pre-work, Tracking, and Preflight

## Shared Context
**Goal:** Close Wave A Zitadel gaps without reopening #16.
**Stack:** Dokploy, Zitadel v4.9.0, Infisical, GitHub issues.
**Architecture:** Bitacora still uses Supabase Auth at runtime; Zitadel is shared Teslita IdP for Wave B.

## Locked Decisions
- Use one new umbrella GitHub issue with checklist G1-G7.
- Do not reopen #16.
- Do not touch `src/Bitacora.*`, `frontend/`, Supabase Auth, or `ZITADEL_MASTERKEY`.

## Task Metadata
```yaml
id: G0
depends_on: []
agent_type: ps-worker
files:
  - create: artifacts/e2e/2026-04-19-cerrar-gaps-wave-a/README.md
  - create: .docs/raw/reports/2026-04-19-cerrar-gaps/G0-preflight.md
  - read: .docs/wiki/09_contratos/CT-AUTH-ZITADEL.md
complexity: medium
done_when: "Umbrella GitHub issue exists and G0 preflight report records GREEN/blocked status"
```

## Reference
`artifacts/e2e/2026-04-19-zitadel-wave-a-smoke/README.md` — copy evidence style and masking.

## Prompt
Run the mandatory preflight exactly. Create a new GitHub issue in `fgpaz/bitacora` titled `[MIGR-IDP] Wave A gap closure — Zitadel GREEN completion` with checklist G1-G7 and links to #15, #16, #17. Record only non-secret values in `G0-preflight.md`. If any preflight check fails, stop before infra mutation.

## Execution Procedure
1. Run `git log -5 --oneline` and verify top history includes `61f17b6`.
2. Run `git status --short`; proceed only if dirty state is limited to known untracked prompts/reports and current task outputs.
3. Run `gh issue view 16 --repo fgpaz/bitacora --json state` and require `CLOSED`.
4. Run OIDC discovery HTTP code check and require `200`.
5. Run `mkey pull bitacora prod`; verify at least 60 keys and no plaintext secrets committed.
6. Verify `ZITADEL_ADMIN_PAT` via `/management/v1/orgs/me` and require org `nuestrascuentitas`.
7. Create the umbrella issue if it does not exist already.
8. Create or update initial smoke README with the task matrix and G0 evidence.
9. Commit only G0 artifacts with `docs(zitadel): add wave a gap closure preflight`.

## Skeleton
```markdown
# G0 Preflight

| Check | Result | Evidence |
|-------|--------|----------|
| Base commit | GREEN | 61f17b6 present |
```

## Verify
`gh issue list --repo fgpaz/bitacora --search "Wave A gap closure" --state open --json number,title` -> returns one umbrella issue

## Commit
`docs(zitadel): add wave a gap closure preflight`
