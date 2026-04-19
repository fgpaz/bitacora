#!/usr/bin/env bash
# snapshot.sh - Zitadel Postgres volume snapshot + offsite upload
# Wave A T3.1, generated 2026-04-19.
#
# Usage (on VPS turismo, as root or via sudo):
#   /opt/zitadel-backup/snapshot.sh
#
# Reads config from /opt/zitadel-backup/.env:
#   ZITADEL_POSTGRES_VOLUME_NAME - Docker volume name
#   ZITADEL_POSTGRES_CONTAINER_NAME - Docker container name (swarm task name)
#   ZITADEL_BACKUP_LOCAL_DIR - local dir for tar snapshots (default /var/backups/zitadel)
#   ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE - rclone remote name (e.g. "b2:bucket-name/path")
#   ZITADEL_BACKUP_RETENTION_LOCAL_DAYS (default 30)
#   ZITADEL_BACKUP_RETENTION_OFFSITE_DAYS (default 90)
#
# Downtime: ~30-90s while Postgres container is stopped for consistent snapshot.
set -euo pipefail

CONFIG_FILE=${ZITADEL_BACKUP_CONFIG:-/opt/zitadel-backup/.env}
if [ ! -f "$CONFIG_FILE" ]; then
  echo "[FATAL] Config file not found: $CONFIG_FILE"
  exit 1
fi
# shellcheck disable=SC1090
source "$CONFIG_FILE"

VOLUME="${ZITADEL_POSTGRES_VOLUME_NAME:?missing}"
CONTAINER="${ZITADEL_POSTGRES_CONTAINER_NAME:?missing}"
LOCAL_DIR="${ZITADEL_BACKUP_LOCAL_DIR:-/var/backups/zitadel}"
REMOTE="${ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE:-}"
RETAIN_LOCAL="${ZITADEL_BACKUP_RETENTION_LOCAL_DAYS:-30}"
RETAIN_OFFSITE="${ZITADEL_BACKUP_RETENTION_OFFSITE_DAYS:-90}"

TS=$(date -u +%Y%m%d-%H%M%S)
FILE="zitadel-pg-${TS}.tar.gz"
PATH_LOCAL="${LOCAL_DIR}/${FILE}"

mkdir -p "$LOCAL_DIR"

echo "[$(date -u -Iseconds)] Stopping $CONTAINER (swarm task) for consistent snapshot"
docker stop "$CONTAINER"

trap 'echo "[$(date -u -Iseconds)] Restarting $CONTAINER (post-error)"; docker start "$CONTAINER" || true' ERR

echo "[$(date -u -Iseconds)] Creating tar of docker volume $VOLUME -> $PATH_LOCAL"
docker run --rm -v "${VOLUME}:/data:ro" -v "${LOCAL_DIR}:/backup" busybox \
  tar -czf "/backup/${FILE}" -C / data

SIZE=$(stat -c '%s' "$PATH_LOCAL" 2>/dev/null || stat -f '%z' "$PATH_LOCAL")
echo "[$(date -u -Iseconds)] Snapshot done: $PATH_LOCAL (${SIZE} bytes)"

echo "[$(date -u -Iseconds)] Restarting $CONTAINER"
docker start "$CONTAINER"

trap - ERR

if [ -n "$REMOTE" ] && command -v rclone >/dev/null 2>&1; then
  echo "[$(date -u -Iseconds)] Uploading to offsite $REMOTE"
  rclone copy "$PATH_LOCAL" "$REMOTE" --progress
  echo "[$(date -u -Iseconds)] Pruning offsite backups older than ${RETAIN_OFFSITE}d"
  rclone delete --min-age "${RETAIN_OFFSITE}d" "$REMOTE" || true
else
  echo "[$(date -u -Iseconds)] Skip offsite upload (REMOTE empty or rclone missing)"
fi

echo "[$(date -u -Iseconds)] Pruning local backups older than ${RETAIN_LOCAL}d"
find "$LOCAL_DIR" -name 'zitadel-pg-*.tar.gz' -mtime "+${RETAIN_LOCAL}" -delete

echo "[$(date -u -Iseconds)] Done: ${FILE}"
