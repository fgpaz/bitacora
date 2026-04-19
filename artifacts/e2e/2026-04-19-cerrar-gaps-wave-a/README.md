# Wave A Gap Closure Smoke — Zitadel Teslita

**Date:** 2026-04-19  
**Tracking:** fgpaz/bitacora#18  
**Epic:** fgpaz/bitacora#15  
**Wave A:** fgpaz/bitacora#16 (closed, not reopened)  
**Wave B:** fgpaz/bitacora#17

## Status Matrix

| Gap | Status | Evidence |
|-----|--------|----------|
| G0 — Preflight + tracking | GREEN | `.docs/raw/reports/2026-04-19-cerrar-gaps/G0-preflight.md` |
| G1 — Login companion | GREEN | `.docs/raw/reports/2026-04-19-cerrar-gaps/G1-login-companion.md` |
| G2 — Backup cron | GREEN | `.docs/raw/reports/2026-04-19-cerrar-gaps/G2-backup-offsite.md` |
| G3 — Admin MFA/passwordless | GREEN | `passwordless/_search` returns 2 owner-managed `AUTH_FACTOR_STATE_READY` credentials |
| G4 — Client credentials | GREEN | `.docs/raw/reports/2026-04-19-cerrar-gaps/G4-client-credentials.md` |
| G5 — Legacy Postgres cleanup | GREEN | Snapshot/offsite done; service/container/volume counts are `0` |
| G6 — Offsite remote | GREEN | `rclone lsl` lists `zitadel-pg-20260419-173731.tar.gz` |
| G7 — DKIM/SPF | GREEN | Gmail headers show DKIM pass and SPF pass |

## Preflight Evidence

| Check | Result |
|-------|--------|
| OIDC discovery | `200` |
| Infisical pull | `83` variables after rebuild, M2M keys, and admin access hardening |
| `ZITADEL_*` keys | refreshed from Infisical |
| Admin PAT org | `nuestrascuentitas` |
| Tracking issue | `https://github.com/fgpaz/bitacora/issues/18` |

## G1 Login Companion Smoke

| Check | Result |
|-------|--------|
| Dokploy application | `0qRNmuYflmhQZz9vFeC8f` |
| Swarm service | `app-connect-haptic-interface-z8m5hi` |
| Image | `ghcr.io/zitadel/zitadel-login:v4.9.0` |
| OIDC discovery | `200` |
| JWKS | `200` |
| Console | `200` |
| `/ui/v2/login` | `200` |
| Real authorize redirect | `200`, `/ui/v2/login/loginname?...`, title `Welcome back!` |

## G2/G6 Backup Smoke

| Check | Result |
|-------|--------|
| Cron | `/etc/cron.d/zitadel-backup` enabled |
| Guarded service mount | `postgres-reboot-wireless-panel-chhbwg-data:/var/lib/postgresql/data` |
| Manual snapshot | `zitadel-pg-20260419-173731.tar.gz` |
| Snapshot size | `10,716,399` bytes |
| OIDC healthcheck after restart | `200` |
| Offsite remote | `teslita-zitadel:/home/fgpaz/backups/zitadel` |
| Offsite listing | file visible via `rclone lsl` |

## G4 Client Credentials Smoke

| Org | HTTP | token_type | alg | `kid` |
|-----|------|------------|-----|-------|
| nuestrascuentitas | `200` | `Bearer` | `RS256` | present |
| bitacora | `200` | `Bearer` | `RS256` | present |
| multi-tedi | `200` | `Bearer` | `RS256` | present |
| gastos | `200` | `Bearer` | `RS256` | present |

## G3 Admin Passwordless Smoke

| Check | Result |
|-------|--------|
| Human admin | `paz.fgabriel@gmail.com` |
| Automation-owned OTP factor | none; `auth_factors/_search` returned `{}` |
| Owner-managed passwordless | 2 ready credentials |
| Credential states | `AUTH_FACTOR_STATE_READY` |
| Credential names | `authenticator`, `S23ultra` |

## G5 Legacy Postgres Cleanup Smoke

| Check | Result |
|-------|--------|
| Legacy service | `postgres-bypass-wireless-bus-tupzoj` |
| Legacy postgresId | `5e5bKMG5jBS30S7d8nn6c` |
| Pre-cleanup event count | `801` |
| Owner approval | explicit approval before destructive action |
| Snapshot | `legacy-zitadel-pg18-20260419-200003.tar.gz` |
| Snapshot size | `8,090,041` bytes |
| Snapshot sha256 | `447bca8e914512cb7f167c31b177998c3ccfa4d7a35b3d832ae5823e3e5f4be0` |
| Offsite path | `teslita-zitadel:/home/fgpaz/backups/zitadel/legacy` |
| Final service/container/volume count | `0 / 0 / 0` |
| Final OIDC/JWKS/admin PAT | `200 / 200 / 200` |

## G7 DKIM/SPF Smoke

| Check | Result |
|-------|--------|
| SMTP test endpoint | `POST /admin/v1/smtp/369306109413949798/_test` |
| Gmail DKIM | `pass` for `@nuestrascuentitas.com`, selector `resend` |
| Gmail SPF | `pass` for envelope sender `send.nuestrascuentitas.com` |
| Transport TLS | TLS 1.3 observed by Gmail |

## Remaining Owner-Gated Evidence

None. Wave A gap closure is GREEN.

### G3 Access Safety Note

During Playwright-assisted validation, the human admin password was rotated and persisted only in Infisical key `ZITADEL_ADMIN_INITIAL_PASSWORD` plus encrypted backup `infra/secrets.enc.env`. A transient OTP factor was removed after the user asked how future login would work. Live verification returned `{}` from `auth_factors/_search`, so the owner is not locked behind an automation-owned factor. The owner later enrolled passwordless/passkeys on personal devices; `passwordless/_search` returned 2 ready credentials. Codex/Playwright must not create personal passkeys for the owner.

## Secret Handling

No secret values are recorded in this artifact. Secret material remains in Infisical and generated local `infra/.env`.
