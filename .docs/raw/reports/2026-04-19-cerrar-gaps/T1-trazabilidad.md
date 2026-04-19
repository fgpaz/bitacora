# T1 — ps-trazabilidad Closure Check

**Date:** 2026-04-19  
**Scope:** Wave A Zitadel gap closure, runtime-sensitive infra/auth changes  
**Verdict:** BLOCKED for full GREEN; actionable subset is traceable

## Governance And Canon

| Check | Result |
|-------|--------|
| `.docs/wiki/00_gobierno_documental.md` exists | PASS |
| `.docs/wiki/_mi-lsp/read-model.toml` exists | PASS |
| `07_baseline_tecnica.md` reviewed | PASS |
| `08_modelo_fisico_datos.md` reviewed | PASS |
| `09_contratos_tecnicos.md` reviewed | PASS |
| `09_contratos/CT-AUTH-ZITADEL.md` updated | PASS |

## Runtime Evidence

| Boundary | Evidence | Result |
|----------|----------|--------|
| OIDC discovery | `/.well-known/openid-configuration` | `200` |
| Login companion | `/ui/v2/login` | `200` |
| Real auth request | `/ui/v2/login/loginname?...` | `200`, title `Welcome back!` |
| Bitacora M2M | `POST /oauth/v2/token` | `200`, Bearer JWT |
| JWT signing | decoded header | `alg=RS256`, `kid` present |
| Backup cron | `/etc/cron.d/zitadel-backup` | enabled |
| Offsite backup | `rclone lsl teslita-zitadel:/home/fgpaz/backups/zitadel` | lists `zitadel-pg-20260419-173731.tar.gz` |
| Postgres service | `postgres-reboot-wireless-panel-chhbwg` | `1/1` |
| Login service | `app-connect-haptic-interface-z8m5hi` | `1/1` |

## Technical Drift Summary

- `CT-AUTH-ZITADEL.md` now matches live org/project/client IDs created after the recovery.
- `infra/.env.template` includes all org, Web PKCE, M2M user, backup healthcheck, and backup remote keys.
- `infra/backups/zitadel/snapshot.sh` now matches live Swarm behavior: service scale down/up, named volume mount validation, Postgres data validation, minimum size guard, OIDC healthcheck, rclone upload.
- `zitadel-backup.md`, `zitadel-recovery.md`, and `zitadel-bootstrap.md` were synced to the current endpoints and mount strategy.
- `02_arquitectura.md` and `09_contratos_tecnicos.md` still describe Supabase Auth as the active Bitacora runtime; this is expected because Wave B has not integrated Zitadel into Bitacora app code.

## Remaining Gaps

| Gap | State | Blocking reason |
|-----|-------|-----------------|
| G3 Admin MFA | BLOCKED | Requires owner QR scan and recovery code storage |
| G5 Legacy Postgres cleanup | AMBER | Legacy eventstore has `801` events; destructive remove requires approval |
| G7 DKIM/SPF | AMBER | SMTP test sent, DKIM DNS present, SPF TXT missing and Gmail headers pending |

## Closure Verdict

Do not mark Wave A full GREEN yet. G1/G2/G4/G6 are traceable and live-verified; G3/G5/G7 remain owner-gated.
