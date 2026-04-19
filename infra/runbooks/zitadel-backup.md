# Zitadel Teslita — Backup Runbook

Docker volume snapshot strategy (NO pg_dump). Script git-tracked at `infra/backups/zitadel/snapshot.sh`.

## Cómo funciona

1. Cron at 03:00 UTC daily on VPS turismo
2. Stops the Zitadel Postgres container
3. Tar-gzip snapshot of the Docker volume `postgres-reboot-wireless-panel-chhbwg-data` to `/var/backups/zitadel/zitadel-pg-<ts>.tar.gz`
4. Restarts the container
5. Uploads to offsite remote (rclone, provider TBD)
6. Prunes local backups >30d, offsite >90d

Downtime per snapshot: ~30-90 seconds (acceptable at 00:00 ART low-traffic window).

## Setup on VPS (run once)

See `infra/backups/zitadel/README.md` for detailed commands. Key steps:

```bash
ssh turismo
sudo mkdir -p /opt/zitadel-backup /var/backups/zitadel
# Copy snapshot.sh and create .env file with volume/container/remote configured
# Install rclone + configure remote
# Install cron at /etc/cron.d/zitadel-backup
```

## Run manually

```bash
ssh turismo
sudo /opt/zitadel-backup/snapshot.sh
# Verify
ls -la /var/backups/zitadel/ | tail -5
rclone ls <remote>:<bucket>/ | tail -5
```

## Monitoring

```bash
tail -200 /var/log/zitadel-backup.log
```

Alert thresholds:
- No successful backup 24h+ → warn
- No successful backup 72h+ → page
- Local `/var/backups/zitadel/` full → run manual prune

## Known gaps (as of 2026-04-19)

- **Offsite remote NOT configured yet.** Snapshots only local until user picks a provider (B2/R2/Hetzner/rsync-desktop) and configures rclone. Tech debt T3.1 closure.
- **No cron installed yet.** First manual run pending VPS setup.

## Rotation and upgrade

Change offsite provider: update `/opt/zitadel-backup/.env` with new `ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE`, rerun `rclone config` for new remote, test with a manual run.

Upgrade Postgres major version: snapshot first, restore into a fresh Postgres volume on the new major version container, validate Zitadel still reads it correctly (Zitadel may require specific PG versions — currently pinned to 17).
