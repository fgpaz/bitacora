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
| G3 — Admin MFA | BLOCKED | Requires user QR scan on human admin |
| G4 — Client credentials | GREEN | `.docs/raw/reports/2026-04-19-cerrar-gaps/G4-client-credentials.md` |
| G5 — Legacy Postgres cleanup | AMBER | Non-empty eventstore; see `.docs/raw/reports/2026-04-19-cerrar-gaps/G5-legacy-postgres.md` |
| G6 — Offsite remote | GREEN | `rclone lsl` lists `zitadel-pg-20260419-173731.tar.gz` |
| G7 — DKIM/SPF | AMBER | SMTP test sent; SPF TXT missing and Gmail headers pending |

## Preflight Evidence

| Check | Result |
|-------|--------|
| OIDC discovery | `200` |
| Infisical pull | `82` variables after rebuild and M2M keys |
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

## Remaining Owner-Gated Evidence

| Gap | Needed |
|-----|--------|
| G3 | Enroll TOTP for `paz.fgabriel@gmail.com`; then verify `auth_factors/_search` returns `otp` |
| G5 | Explicit approval before any legacy `postgres.remove` because the DB has `801` events |
| G7 | Paste Gmail authentication headers; DNS currently shows DKIM TXT present and root SPF absent |

## Secret Handling

No secret values are recorded in this artifact. Secret material remains in Infisical and generated local `infra/.env`.
