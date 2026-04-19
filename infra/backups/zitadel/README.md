# Zitadel Postgres Backup

Script: [`snapshot.sh`](./snapshot.sh) — docker volume snapshot + offsite upload via rclone.

## Setup on VPS turismo

```bash
# Assumes SSH access to VPS as user ubuntu.
sudo mkdir -p /opt/zitadel-backup /var/backups/zitadel
sudo cp snapshot.sh /opt/zitadel-backup/
sudo chmod +x /opt/zitadel-backup/snapshot.sh

# Create config file from Infisical values
sudo tee /opt/zitadel-backup/.env > /dev/null <<'EOF'
ZITADEL_POSTGRES_VOLUME_NAME=postgres-reboot-wireless-panel-chhbwg-data
ZITADEL_POSTGRES_CONTAINER_NAME=postgres-reboot-wireless-panel-chhbwg.1.fa9g1xeqkh4prqey1kpe0tku2
ZITADEL_BACKUP_LOCAL_DIR=/var/backups/zitadel
ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE=
ZITADEL_BACKUP_RETENTION_LOCAL_DAYS=30
ZITADEL_BACKUP_RETENTION_OFFSITE_DAYS=90
EOF
sudo chmod 600 /opt/zitadel-backup/.env

# rclone install + config (interactive)
curl https://rclone.org/install.sh | sudo bash
rclone config    # Add b2 / r2 / hetzner remote

# Then update ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE in /opt/zitadel-backup/.env

# Install cron (daily at 03:00 UTC)
sudo tee /etc/cron.d/zitadel-backup > /dev/null <<'EOF'
SHELL=/bin/bash
PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin
0 3 * * * root /opt/zitadel-backup/snapshot.sh >> /var/log/zitadel-backup.log 2>&1
EOF
```

## Restore

1. Download backup: `rclone copy <remote>:<bucket>/zitadel-pg-<ts>.tar.gz /tmp/`
2. Stop current Postgres via Dokploy API or `docker service update --replicas 0 <postgresService>`.
3. Replace volume contents:
   ```bash
   docker run --rm -v postgres-reboot-wireless-panel-chhbwg-data:/data -v /tmp:/backup busybox \
     sh -c 'rm -rf /data/* /data/..?* /data/.[!.]* && tar -xzf /backup/zitadel-pg-<ts>.tar.gz -C /'
   ```
4. Scale Postgres back up via Dokploy.
5. Validate: `curl https://id.nuestrascuentitas.com/.well-known/openid-configuration` -> 200.

## Notes

- Backup strategy lockeada 2026-04-18: docker volume snapshot + offsite (NO pg_dump).
- Downtime esperado 30-90s diario a las 03:00 UTC (00:00 ART).
- Retention: 30d local, 90d offsite.
- Offsite provider por definir (ver `ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE` en Infisical).
- Hasta que el offsite remote este configurado, snapshots solo se guardan local.
