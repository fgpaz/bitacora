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
| G2 — Backup cron | PENDING | Awaiting VPS install |
| G3 — Admin MFA | PENDING | Requires G1 + user QR scan |
| G4 — Client credentials | PENDING | Current smoke still returns `invalid_client` |
| G5 — Legacy Postgres cleanup | PENDING | Requires snapshot/stop/soak/remove approval |
| G6 — Offsite remote | PENDING | Target locked: `teslita` via Tailscale/SFTP |
| G7 — DKIM/SPF | PENDING | Fresh SMTP test + Gmail headers required |

## Preflight Evidence

| Check | Result |
|-------|--------|
| OIDC discovery | `200` |
| Infisical pull | `70` variables |
| `ZITADEL_*` keys | `53` |
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

## Secret Handling

No secret values are recorded in this artifact. Secret material remains in Infisical and generated local `infra/.env`.
