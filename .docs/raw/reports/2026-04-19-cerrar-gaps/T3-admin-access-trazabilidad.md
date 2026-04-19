# T3 — Admin Access Traceability

**Date:** 2026-04-19  
**Scope:** G3 admin login/MFA safety after Playwright-assisted validation.

## Checks

| Check | Result | Evidence |
|-------|--------|----------|
| Secret plaintext excluded from docs | PASS | Docs reference only key names, not values |
| Human admin password rotated | PASS | Infisical key `ZITADEL_ADMIN_INITIAL_PASSWORD`; encrypted backup refreshed in `infra/secrets.enc.env` |
| No automation-owned OTP factor active | PASS | `auth_factors/_search` returned `{}` |
| Owner-managed passwordless active | PASS | `passwordless/_search` returned 2 `AUTH_FACTOR_STATE_READY` credentials |
| Login future path documented | PASS | `CT-AUTH-ZITADEL.md` section 6/7/11 and smoke README G3 note |
| Passwordless risk documented | PASS | Passkey enrollment must happen on owner device; Codex enrollment only acceptable as break-glass shared TOTP |

## Closure Verdict

G3 is GREEN via owner-managed passwordless/passkeys. The account is not locked behind an automation-owned factor, and the current password recovery source of truth remains Infisical plus encrypted SOPS backup.
