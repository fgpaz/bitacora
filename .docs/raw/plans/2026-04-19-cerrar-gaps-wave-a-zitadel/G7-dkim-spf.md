# Task G7: Verify DKIM and SPF

## Shared Context
**Goal:** Prove Resend/Zitadel SMTP sends authenticated mail for `nuestrascuentitas.com`.
**Stack:** Zitadel admin API, Resend SMTP config, Gmail headers.
**Architecture:** SMTP config id is `369203306586702095`.

## Locked Decisions
- Re-trigger a fresh test mail.
- User supplies Gmail headers.
- If DKIM/SPF fails, open DNS follow-up and mark only G7 AMBER.

## Task Metadata
```yaml
id: G7
depends_on: [G0]
agent_type: ps-worker
files:
  - create: .docs/raw/reports/2026-04-19-cerrar-gaps/G7-dkim-spf.md
complexity: medium
done_when: "Fresh test mail headers show DKIM pass and SPF pass, or DNS follow-up issue exists"
```

## Reference
`artifacts/e2e/2026-04-19-zitadel-wave-a-smoke/README.md:57` — prior SMTP test pending headers.

## Prompt
Trigger a fresh SMTP test to `paz.fgabriel@gmail.com`. Ask user to paste the authentication-results headers. Record only masked headers and pass/fail status. If fail, create a GitHub issue for DNS remediation and do not claim Wave A full GREEN.

## Execution Procedure
1. POST `/admin/v1/smtp/369203306586702095/_test` with receiver `paz.fgabriel@gmail.com`.
2. Wait for user to provide headers from Gmail original message.
3. Extract `dkim=pass` and `spf=pass`.
4. If both pass, write evidence.
5. If either fails or mail missing, create DNS follow-up issue and mark G7 AMBER.
6. Commit with `test(zitadel): verify smtp dkim spf`.

## Skeleton
```markdown
Authentication-Results: mx.google.com;
  dkim=pass header.d=nuestrascuentitas.com;
  spf=pass smtp.mailfrom=...
```

## Verify
`G7-dkim-spf.md` contains masked `dkim=pass` and `spf=pass`, or linked DNS issue

## Commit
`test(zitadel): verify smtp dkim spf`
