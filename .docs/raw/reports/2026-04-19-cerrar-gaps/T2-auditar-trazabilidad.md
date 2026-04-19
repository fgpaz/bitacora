# T2 — ps-auditar-trazabilidad Audit

**Date:** 2026-04-19  
**Scope:** Cross-doc, runtime, and GitHub traceability for Wave A Zitadel gap closure  
**Verdict:** Superseded by final GREEN closure

> 2026-04-19 continuation: G3 was later closed via owner-managed passwordless/passkeys. See `T4-owner-passwordless-continuation.md`. Remaining blockers are G5 and G7.

> 2026-04-19 final continuation: G5 and G7 were later closed. See `T5-final-green-closure.md`.

## Findings

| Severity | Finding | Evidence | Required action |
|----------|---------|----------|-----------------|
| Resolved | Admin passwordless was not enrolled during the original T2 audit | Superseded by T4: `passwordless/_search` returned 2 `AUTH_FACTOR_STATE_READY` credentials | No further action for G3 |
| Resolved | Legacy Postgres could not be removed during the original T2 audit | Superseded by T5: snapshot/offsite done, Dokploy resource removed, service/container/volume counts `0` | No further action for G5 |
| Resolved | SPF was not verified during the original T2 audit | Superseded by T5: Gmail headers show SPF pass and DKIM pass | No further action for G7 |
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
- Board sync was not executed in this pass because the wave was not full GREEN at T2 time. Final status is superseded by T5.

## Audit Verdict

The shipped subset was technically consistent and observable live at T2 time. Final closure is superseded by T5, where G1..G7 are GREEN.
