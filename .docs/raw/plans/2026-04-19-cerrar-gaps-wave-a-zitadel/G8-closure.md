# Task G8: Closure, Docs, GitHub, and Traceability

## Shared Context
**Goal:** Close Wave A gap cycle with signed evidence and synced documentation.
**Stack:** Markdown docs, memory file, GitHub issues, traceability skills.
**Architecture:** Contract and runtime evidence must agree before #17 can start without caveats.

## Locked Decisions
- Do not reopen #16.
- Comment on #15 and #17 after docs/evidence are committed.
- Mark Wave A full GREEN only if G1-G7 are all green; if DKIM/SPF fails, state AMBER explicitly.

## Task Metadata
```yaml
id: G8
depends_on: [G1, G2_G6, G3, G4, G5, G7]
agent_type: ps-docs
files:
  - modify: .docs/wiki/09_contratos/CT-AUTH-ZITADEL.md
  - modify: infra/runbooks/zitadel-backup.md
  - modify: infra/runbooks/zitadel-recovery.md
  - modify: infra/dokploy/zitadel/README.md
  - create: artifacts/e2e/2026-04-19-cerrar-gaps-wave-a/README.md
  - create: .docs/raw/reports/2026-04-19-cerrar-gaps/closure.md
  - modify: C:/Users/fgpaz/.claude/projects/C--repos-mios-humor/memory/project_idp_zitadel_multi_ecosistema.md
complexity: high
done_when: "Traceability and audit pass, #15/#17 comments are posted, and final commit exists"
```

## Reference
`.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md:130` — gaps section to close.

## Prompt
Update docs and evidence from actual command outputs only. Do not invent green status. Keep secrets masked. The final smoke README must include OIDC, login UI, client credentials, MFA, backup/offsite, DKIM/SPF, and legacy PG removal evidence.

## Execution Procedure
1. Update CT section 11 with each gap status and evidence path.
2. Update runbooks and infra README known gaps.
3. Update `infra/.env.template` with login companion/offsite keys if missing.
4. Update memory from PARTIAL to completed or completed-with-G7-AMBER.
5. Write final smoke README and closure report.
6. Run `ps-trazabilidad`.
7. Run `ps-auditar-trazabilidad`.
8. Comment on #15 and #17 with final status and evidence links.
9. Commit with `docs(zitadel): close wave a gaps`.

## Skeleton
```markdown
# Wave A Gap Closure Smoke

| Gap | Status | Evidence |
|-----|--------|----------|
| G1 Login companion | GREEN | ... |
```

## Verify
`git log -1 --oneline` -> `docs(zitadel): close wave a gaps`

## Commit
`docs(zitadel): close wave a gaps`
