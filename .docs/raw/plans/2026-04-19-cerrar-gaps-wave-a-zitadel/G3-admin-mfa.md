# Task G3: Enroll Admin MFA

## Shared Context
**Goal:** Human IAM Owner admin must have TOTP MFA enrolled.
**Stack:** Zitadel console/login UI, user phone authenticator, Infisical.
**Architecture:** Requires G1 login companion to be GREEN first.

## Locked Decisions
- Use TOTP now.
- Store recovery codes only in Infisical key `ZITADEL_ADMIN_TOTP_RECOVERY`.
- Do not paste QR, TOTP secret, or recovery codes into repo/docs/chat.

## Task Metadata
```yaml
id: G3
depends_on: [G1]
agent_type: ps-worker
files:
  - create: .docs/raw/reports/2026-04-19-cerrar-gaps/G3-admin-mfa.md
complexity: medium
done_when: "management API reports otp auth factor for paz.fgabriel@gmail.com"
```

## Reference
`.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md:76` — admin users.

## Prompt
Coordinate with the user in real time. Open console/login after G1. User logs in with `paz.fgabriel@gmail.com`, scans TOTP QR on their phone, confirms code, and provides recovery codes only through a secure secret-setting flow. Verify factor through read-only management API.

## Execution Procedure
1. Confirm `/ui/v2/login` and `/ui/console/` work.
2. Guide user to account security/authenticators.
3. User scans QR and confirms TOTP.
4. Persist recovery codes with `mkey set bitacora ZITADEL_ADMIN_TOTP_RECOVERY "<codes>" --env prod`.
5. Run `mkey pull bitacora prod` and verify key exists without printing value.
6. Query user auth factors and require `otp`.
7. Write masked evidence and commit with `security(zitadel): enroll admin totp`.

## Skeleton
```markdown
| Check | Result |
|-------|--------|
| TOTP factor | GREEN: otp present |
| Recovery codes | GREEN: Infisical key exists |
```

## Verify
`GET /management/v1/users/{adminId}/auth-factors/_search` -> includes `otp`

## Commit
`security(zitadel): enroll admin totp`
