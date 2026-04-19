# T2 — ps-auditar-trazabilidad Audit

**Date:** 2026-04-19  
**Scope:** Cross-doc, runtime, and GitHub traceability for Wave A Zitadel gap closure  
**Verdict:** BLOCKED for full Wave A GREEN; approved with follow-ups for shipped subset

> 2026-04-19 continuation: G3 was later closed via owner-managed passwordless/passkeys. See `T4-owner-passwordless-continuation.md`. Remaining blockers are G5 and G7.

## Findings

| Severity | Finding | Evidence | Required action |
|----------|---------|----------|-----------------|
| Resolved | Admin passwordless was not enrolled during the original T2 audit | Superseded by T4: `passwordless/_search` returned 2 `AUTH_FACTOR_STATE_READY` credentials | No further action for G3 |
| High | Legacy Postgres cannot be removed safely yet | `postgres-bypass-wireless-bus-tupzoj` has `events2=801` | Owner approval required before snapshot/stop/soak/remove |
| Medium | SPF is not verified | DNS query for root TXT returned no SPF; DKIM TXT exists | Add/repair SPF or provide Gmail headers proving acceptable auth outcome |
| Low | Active Bitacora runtime docs still say Supabase | `02_arquitectura.md` / `09_contratos_tecnicos.md` remain Supabase-current | Expected until Wave B; do not alter Bitacora runtime docs in this wave |

## Technical Docs Reviewed

- `.docs/wiki/02_arquitectura.md`
- `.docs/wiki/07_baseline_tecnica.md`
- `.docs/wiki/08_modelo_fisico_datos.md`
- `.docs/wiki/09_contratos_tecnicos.md`
- `.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md`
- `infra/runbooks/zitadel-backup.md`
- `infra/runbooks/zitadel-bootstrap.md`
- `infra/runbooks/zitadel-recovery.md`
- `infra/backups/zitadel/README.md`
- `artifacts/e2e/2026-04-19-cerrar-gaps-wave-a/README.md`
- `~/.claude/projects/C--repos-mios-humor/memory/project_idp_zitadel_multi_ecosistema.md`

## Live Runtime Evidence Reviewed

- OIDC discovery: `200`
- Login companion root: `200`
- Real authorize redirect: `200`
- M2M token: `200`, RS256 JWT with `kid`
- Backup cron: enabled
- Offsite backup: `zitadel-pg-20260419-173731.tar.gz` listed remotely
- Active Postgres service: `1/1`
- Login companion service: `1/1`
- Legacy Postgres: `1/1`, non-empty eventstore

## GitHub / Board State

- Repo board config exists: `.pj-crear-tarjeta.conf` points to `fgpaz/bitacora`, project `4`.
- Touched issues: #15, #17, #18. #16 remains closed and was not reopened.
- Board sync was not executed in this pass because the wave is not full GREEN. After T4, remaining owner-gated items are G5/G7.

## Audit Verdict

The shipped subset is technically consistent and observable live. Full closure remains blocked by DNS/destructive-decision gates. Do not comment #15/#17 as fully unblocked until the agreed G5/G7 outcomes are resolved or explicitly accepted as follow-ups.
