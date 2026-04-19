# T3 — Admin Access Traceability

**Date:** 2026-04-19  
**Scope:** G3 admin login/MFA safety after Playwright-assisted validation.

## Checks

| Check | Result | Evidence |
|-------|--------|----------|
| Secret plaintext excluded from docs | PASS | Docs reference only key names, not values |
| Human admin password rotated | PASS | Infisical key `ZITADEL_ADMIN_INITIAL_PASSWORD`; encrypted backup refreshed in `infra/secrets.enc.env` |
| No automation-owned OTP factor active | PASS | `auth_factors/_search` returned `{}` |
| Login future path documented | PASS | `CT-AUTH-ZITADEL.md` section 6/7/11 and smoke README G3 note |
| Passwordless risk documented | PASS | Passkey enrollment must happen on owner device; Codex enrollment only acceptable as break-glass shared TOTP |

## Closure Verdict

G3 remains BLOCKED for Wave A GREEN because the owner-managed MFA/passkey enrollment is not complete. The account is not locked behind an automation-owned factor, and the current password recovery source of truth is Infisical plus encrypted SOPS backup.
