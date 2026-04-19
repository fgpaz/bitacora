# Zitadel Teslita — Backup Runbook

Docker volume snapshot strategy (NO pg_dump). Script git-tracked at `infra/backups/zitadel/snapshot.sh`.

## Cómo funciona

1. Cron at 03:00 UTC daily on VPS turismo
2. Verifies the Docker service mounts `postgres-reboot-wireless-panel-chhbwg-data` at `/var/lib/postgresql/data`
3. Verifies the volume contains a real Postgres data directory before stopping anything
4. Scales the Zitadel Postgres Swarm service to 0
5. Tar-gzip snapshot of the Docker volume to `/var/backups/zitadel/zitadel-pg-<ts>.tar.gz`
6. Rejects snapshots smaller than `ZITADEL_BACKUP_MIN_BYTES` (default `1048576`)
7. Scales the service back to 1 and verifies OIDC discovery
8. Uploads to offsite remote through rclone
9. Prunes local backups >30d, offsite >90d

Downtime per snapshot: ~30-90 seconds (acceptable at 00:00 ART low-traffic window).

## Setup on VPS (run once)

See `infra/backups/zitadel/README.md` for detailed commands. Key steps:

```bash
ssh turismo
sudo mkdir -p /opt/zitadel-backup /var/backups/zitadel
# Copy snapshot.sh and create .env file with volume/service/remote configured
# Install rclone + configure remote teslita-zitadel
# Install cron at /etc/cron.d/zitadel-backup
```

## Run manually

```bash
ssh turismo
sudo /opt/zitadel-backup/snapshot.sh
# Verify
ls -la /var/backups/zitadel/ | tail -5
rclone lsl teslita-zitadel:/home/fgpaz/backups/zitadel | tail -5
```

## Monitoring

```bash
tail -200 /var/log/zitadel-backup.log
```

Alert thresholds:
- No successful backup 24h+ → warn
- No successful backup 72h+ → page
- Local `/var/backups/zitadel/` full → run manual prune

## Current state (2026-04-19)

- Offsite remote configured: `teslita-zitadel:/home/fgpaz/backups/zitadel`.
- Cron installed: `/etc/cron.d/zitadel-backup`.
- Last verified manual backup: `zitadel-pg-20260419-173731.tar.gz`, `10,716,399` bytes, listed remotely via rclone.
- Incident guardrails are mandatory: do not remove the mount/size/OIDC checks from `snapshot.sh`.

## Incident footnote

On 2026-04-19 the first install attempt exposed a bad Dokploy mount: the service mounted the named volume at `/var/lib/postgresql/18/docker`, while live data lived at `/var/lib/postgresql/data` in an anonymous volume. The service is now corrected to mount `postgres-reboot-wireless-panel-chhbwg-data:/var/lib/postgresql/data`.

## Rotation and upgrade

Change offsite provider: update `/opt/zitadel-backup/.env` with new `ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE`, rerun `rclone config` for new remote, test with a manual run.

Upgrade Postgres major version: snapshot first, restore into a fresh Postgres volume on the new major version container, validate Zitadel still reads it correctly (Zitadel may require specific PG versions — currently pinned to 17).
