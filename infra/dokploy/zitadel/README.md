# Zitadel Teslita IdP — Infra Reference

This directory contains retro-generated git-tracked artifacts reflecting the live Zitadel v4.9.0 stack deployed in Dokploy on VPS turismo, accessible at `https://id.nuestrascuentitas.com`.

**IMPORTANT:** These files are NOT auto-deployed by Dokploy. Dokploy's managed compose is stored in Dokploy itself. This directory serves as:

- Reference for disaster recovery / redeploy from scratch
- Onboarding doc for new ops
- Alignment canon for future modifications (change here → mirror in Dokploy UI or API)

## Files

| File | Purpose |
|------|---------|
| `compose.yml` | Reference docker-compose reflecting live Zitadel core + Postgres |
| `env.example` | Shape of required env vars (values live in Infisical) |
| `traefik.labels.md` | Active Traefik routing labels in Dokploy |

## Live state snapshot (2026-04-20)

- **Dokploy project:** `teslita-shared-idp` (`AKHsAJScexTwhJBzfFRlk`)
- **Zitadel app:** `teslita-shared-zitadel` (`zFdEECmPr1hhxwL0DKu4B`), appName `app-program-optical-matrix-xmjswe`
- **Postgres (active):** `teslita-shared-idp-db-pg17` (`BjjSOBWAwe6XpGttK4XoY`), appName `postgres-reboot-wireless-panel-chhbwg`, image `postgres:17`, DB `shared_zitadel_v4`
- **Postgres (legacy, unused):** `teslita-shared-idp-db` (`5e5bKMG5jBS30S7d8nn6c`), image `postgres:18`, DB `shared_zitadel` — candidate removal
- **Zitadel image:** `ghcr.io/zitadel/zitadel:v4.9.0` (pinned)
- **Domain:** `id.nuestrascuentitas.com` → Traefik → port 8080
- **TLS:** Let's Encrypt via Dokploy/Traefik
- **Admin:** `paz.fgabriel@gmail.com` (human) + `zitadel-admin-sa` + `login-client` (machines with PATs in Infisical)

## Known gaps (as of 2026-04-20)

- **Login companion app deployed.** Zitadel v4 login UI v2 is live for `/ui/v2/login/*` and Bitacora OIDC authorize flow completes through it.
- **Backup/offsite posture closed in Wave A.** See `infra/backups/zitadel/` and the Wave A closure evidence for operational details.
- **Legacy Postgres (postgres:18)** still running — can be removed safely.

## Related docs

- Runbooks: `../../runbooks/zitadel-*.md`
- Backup: `../../backups/zitadel/`
- Contract: `../../../.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md`
- Memoria: `~/.claude/projects/C--repos-mios-humor/memory/project_idp_zitadel_multi_ecosistema.md`
