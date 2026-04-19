# Task T0: B0 Preflight, Secrets, Redirects, No-User Gate

## Shared Context
**Goal:** Unlock Wave B safely by proving secrets access, Zitadel app config, and empty-user assumptions.
**Stack:** PowerShell, Bash, `mi-key-cli`, GitHub CLI, curl, PostgreSQL/Dokploy evidence.
**Architecture:** Active auth will be Zitadel-only; Supabase remains rollback-only until acceptance.

## Locked Decisions
- Do not create plaintext secrets.
- Do not regenerate `ZITADEL_MASTERKEY`.
- Do not start code changes until this task passes.
- Treat any real user or clinical data as a stop condition.

## Task Metadata
```yaml
id: T0
depends_on: []
agent_type: ps-worker
files:
  - create: .docs/raw/reports/2026-04-19-bitacora-wave-b/B0-preflight.md
  - read: .docs/wiki/09_contratos/CT-AUTH-ZITADEL.md
  - read: .docs/raw/reports/2026-04-19-zitadel-shared-idp-integration-handoff.md
complexity: medium
done_when: "B0 report exists and confirms mkey pull, discovery, JWKS, redirect/logout URI, and no-user gate"
```

## Reference
`.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md:22-73` defines issuer, JWKS, org, project, and client IDs.

## Prompt
Run only non-mutating or secret-safe checks. Verify `mkey pull bitacora prod` works through the approved `mi-key-cli` path and never print secret values. Verify OIDC discovery and JWKS return 200. Verify the Bitacora Zitadel web client has exact redirect URI `https://bitacora.nuestrascuentitas.com/auth/callback`, local redirect URI for smoke, and post logout redirect `https://bitacora.nuestrascuentitas.com/`. Produce a report with masked evidence only. If secrets access is missing, stop and report the exact auth setup needed.

## Execution Procedure
1. Run `mi-lsp workspace status bitacora --format toon` and `mi-lsp nav governance --workspace bitacora --format toon`.
2. Run `git status --short`; stop if tracked code WIP exists.
3. Run `bash -lc '/c/Users/fgpaz/.claude/skills/mi-key-cli/scripts/mkey.sh pull bitacora prod >/tmp/bitacora-mkey.out'`; if path fails, retry `/mnt/c/Users/fgpaz/.claude/skills/mi-key-cli/scripts/mkey.sh`.
4. If the command fails with `AuthTokenMissing`, stop before all code waves and write that blocker in the report.
5. Run OIDC discovery and JWKS `curl` status checks; expected `200`.
6. Query redirect/logout configuration only through approved API or trusted runbook evidence; mask all tokens.
7. Verify no real users/clinical data with read-only row counts if DB access is available. If unavailable, report `not verified` and stop before destructive/data assumptions.
8. Write `.docs/raw/reports/2026-04-19-bitacora-wave-b/B0-preflight.md`.

## Skeleton
```markdown
# B0 Preflight

- Governance: pass|fail
- Git status: clean|known-untracked|blocked
- Infisical: pass|blocked
- OIDC discovery: 200
- JWKS: 200
- Redirect URIs: verified|blocked
- No-user gate: verified|blocked
- Stop conditions: none|...
```

## Verify
`Test-Path .docs/raw/reports/2026-04-19-bitacora-wave-b/B0-preflight.md` -> `True`

## Commit
`docs(zitadel): record wave b preflight evidence`
