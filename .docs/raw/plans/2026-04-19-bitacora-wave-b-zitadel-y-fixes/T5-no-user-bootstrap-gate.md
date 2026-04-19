# Task T5: No-User Bootstrap Gate

## Shared Context
**Goal:** Prove the no-real-user assumption and create safe smoke-user bootstrap evidence.
**Stack:** `mi-key-cli`, PostgreSQL read-only checks, Zitadel Management API, docs evidence.
**Architecture:** No migration of real clinical identities is planned.

## Locked Decisions
- If any real production user/clinical row exists, stop and do not proceed with destructive or identity reset work.
- Smoke users may be created only through Zitadel and must not expose passwords/tokens in files.

## Task Metadata
```yaml
id: T5
depends_on: [T4]
agent_type: ps-worker
files:
  - create: .docs/raw/reports/2026-04-19-bitacora-wave-b/B3-no-user-bootstrap.md
  - read: .docs/wiki/05_modelo_datos.md
  - read: .docs/wiki/09_contratos/CT-AUTH-ZITADEL.md
complexity: medium
done_when: "B3 report confirms no-user gate and smoke-user readiness"
```

## Reference
`01_alcance_funcional.md` classifies users and health records; use row counts only, never payload dumps.

## Prompt
Run read-only row count checks for users, patients, professionals, mood entries, daily checkins, care links, consent grants, and Telegram sessions. If counts are zero or known smoke-only, document that. If counts are non-zero and cannot be proven smoke-only, stop. Verify `smoke@bitacora.test` and `smoke-prof@bitacora.test` exist or create them through approved Zitadel API with secrets from Infisical only.

## Execution Procedure
1. Pull secrets through `mkey` without printing values.
2. Run read-only DB row counts; record table names and counts only.
3. Verify or create Zitadel smoke users and project roles.
4. Confirm smoke user role mapping produces `patient` and `professional`.
5. Write report with no PII beyond test email addresses.

## Skeleton
```markdown
# B3 No-User Bootstrap

| Table | Count | Classification |
|---|---:|---|
| users | 0 | empty |
```

## Verify
`Test-Path .docs/raw/reports/2026-04-19-bitacora-wave-b/B3-no-user-bootstrap.md` -> `True`

## Commit
`docs(auth): record no-user zitadel bootstrap gate`
