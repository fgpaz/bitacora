# T4 — Owner Passwordless Continuation

**Date:** 2026-04-19  
**Scope:** Continue Wave A Zitadel gap closure after owner enrolled passwordless methods.

> 2026-04-19 final continuation: G5 and G7 were later closed. See `T5-final-green-closure.md`.

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

Verdict at T4 time: G7 still required Gmail "Show original" authentication headers. Superseded by T5.

## G5 Guardrail

G5 at T4 time still required explicit destructive approval. Superseded by T5 after owner approval, snapshot/offsite, and removal.

## GitHub Sync

| Issue | Result |
|-------|--------|
| `fgpaz/bitacora#18` | body updated at T4 time: G3 checked; G5/G7 still pending |
| `fgpaz/bitacora#18` | comment added with G3 passwordless evidence |
| `fgpaz/bitacora#19` | comment added with SMTP re-trigger evidence |

## Closure Verdict

This continuation closed G3. Final Wave A GREEN closure is superseded by T5.
