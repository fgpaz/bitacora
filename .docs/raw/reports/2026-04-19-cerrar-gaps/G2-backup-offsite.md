# G2/G6 — Zitadel Backup Cron + Offsite Remote

**Date:** 2026-04-19  
**Status:** GREEN after incident recovery  
**Tracking:** fgpaz/bitacora#18

## Result

Backup is installed on VPS `turismo`, runs from `/opt/zitadel-backup/snapshot.sh`, and writes local plus offsite snapshots.

| Check | Result |
|-------|--------|
| Cron path | `/etc/cron.d/zitadel-backup` |
| Schedule | `03:00 UTC` daily |
| Service controlled | `postgres-reboot-wireless-panel-chhbwg` |
| Volume snapped | `postgres-reboot-wireless-panel-chhbwg-data` |
| Required mount target | `/var/lib/postgresql/data` |
| Offsite remote | `teslita-zitadel:/home/fgpaz/backups/zitadel` |
| Local retention | `30d` |
| Offsite retention | `90d` |

## Manual Smoke

| Evidence | Result |
|----------|--------|
| Service mount check | source `postgres-reboot-wireless-panel-chhbwg-data`, target `/var/lib/postgresql/data` |
| Volume data check | `PG_VERSION` + `base/` present and >1 MiB |
| Snapshot file | `zitadel-pg-20260419-173731.tar.gz` |
| Snapshot size | `10,716,399` bytes |
| Healthcheck after restart | OIDC discovery `200` |
| Offsite listing | `rclone lsl` lists `zitadel-pg-20260419-173731.tar.gz` |

## Incident Note

During the first backup attempt, the Dokploy Postgres service was found to mount the named volume at `/var/lib/postgresql/18/docker` while the live database data was in an anonymous volume at `/var/lib/postgresql/data`. The first tar was therefore empty-sized and was rejected as evidence.

Recovery actions completed:

- Disabled cron during incident.
- Created emergency physical and logical backup under `/var/backups/zitadel/incidents/20260419-postgres-mount-recovery/`.
- Copied the live anonymous volume contents into the intended named volume.
- Updated the service mount to `postgres-reboot-wireless-panel-chhbwg-data:/var/lib/postgresql/data`.
- Rebuilt Wave A runtime objects that were lost during the bad mount cycle.
- Added script guardrails for mount target, Postgres data structure, minimum snapshot size, and OIDC healthcheck.
- Re-enabled cron after a successful manual backup.

## Secret Handling

No secret values are recorded here. Rclone credentials and Infisical material stay on the host and in Infisical.
