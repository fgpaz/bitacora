# T4 — Owner Passwordless Continuation

**Date:** 2026-04-19  
**Scope:** Continue Wave A Zitadel gap closure after owner enrolled passwordless methods.

## Context Checks

| Check | Result |
|-------|--------|
| Workspace | `bitacora` / `C:\repos\mios\humor` |
| Governance | valid, `governance_blocked=false`, projection in sync |
| Secrets | `mkey pull bitacora prod` completed with `83` variables |
| Admin PAT | `GET /management/v1/orgs/me` returned `nuestrascuentitas` |
| OIDC discovery | `200` |

## G3 Evidence

| Check | Result |
|-------|--------|
| `auth_factors/_search` | `{}` |
| `passwordless/_search` | `200` |
| Ready passwordless credentials | `2` |
| Credential names | `authenticator`, `S23ultra` |
| Credential state | `AUTH_FACTOR_STATE_READY` |

Verdict: G3 is GREEN via owner-managed passwordless/passkeys. No Codex/Playwright-owned OTP factor is active.

## G7 Evidence

| Check | Result |
|-------|--------|
| SMTP test endpoint | `POST /admin/v1/smtp/369306109413949798/_test` |
| Latest result | `200` at `2026-04-19T16:46:34-03:00` |
| DKIM TXT | present for `resend._domainkey.nuestrascuentitas.com` |
| Root SPF TXT | absent via resolver `1.1.1.1` |

Verdict: G7 remains AMBER until Gmail "Show original" authentication headers are provided or SPF DNS is repaired.

## G5 Guardrail

G5 remains AMBER. Legacy Postgres `postgres-bypass-wireless-bus-tupzoj` contains `801` events. No destructive action was taken in this continuation because explicit owner approval for `postgres.remove` was not provided.

## GitHub Sync

| Issue | Result |
|-------|--------|
| `fgpaz/bitacora#18` | body updated: G3 checked; G5/G7 open |
| `fgpaz/bitacora#18` | comment added with G3 passwordless evidence |
| `fgpaz/bitacora#19` | comment added with SMTP re-trigger evidence |

## Closure Verdict

This continuation closes G3. Wave A remains not fully GREEN because G5 and G7 are still open follow-ups.
