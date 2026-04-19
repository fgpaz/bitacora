# G3 — Admin Access, MFA, and Passwordless Gate

**Date:** 2026-04-19  
**Status:** BLOCKED by owner-managed factor enrollment  
**User:** `paz.fgabriel@gmail.com` (`369304228571316582`)

## What Is Safe Now

- The human admin password was rotated on 2026-04-19 after browser automation exposed sensitive input in a Playwright snapshot.
- The current password is stored only in Infisical as `ZITADEL_ADMIN_INITIAL_PASSWORD` and in the encrypted SOPS backup `infra/secrets.enc.env`.
- Local plaintext appears only after an explicit `mkey pull bitacora prod` into ignored `infra/.env`.
- Clipboard was cleared after the browser login operation.
- The partially created OTP factor was removed to avoid locking the owner out.

## Live Verification

Command:

```powershell
POST /management/v1/users/369304228571316582/auth_factors/_search
```

Result:

```json
{}
```

Interpretation: no active TOTP factor is enrolled on the human admin account.

## Passwordless Decision

Passkeys/WebAuthn credentials are device/browser-bound. A passkey enrolled by Codex through Playwright would belong to the automation browser profile, not to the owner phone, laptop, security key, or password manager. That would not be a usable personal recovery path.

Accepted paths:

1. Owner-managed factor (recommended): owner logs in at `https://id.nuestrascuentitas.com/ui/console` from their own browser and enrolls a passkey or TOTP on their own device. Codex can verify the factor through the Management API after enrollment.
2. Break-glass shared TOTP: Codex enrolls a TOTP factor and stores the seed in Infisical, for operational emergency access only. This is not personal MFA and must be documented as a shared secret control.

## Current Closure State

G3 remains BLOCKED until the owner enrolls a factor on their own device or explicitly accepts a break-glass shared TOTP model.
