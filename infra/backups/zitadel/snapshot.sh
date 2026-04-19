#!/usr/bin/env bash
# snapshot.sh - Zitadel Postgres volume snapshot + offsite upload
# Wave A T3.1, generated 2026-04-19.
#
# Usage (on VPS turismo, as root or via sudo):
#   /opt/zitadel-backup/snapshot.sh
#
# Reads config from /opt/zitadel-backup/.env:
#   ZITADEL_POSTGRES_VOLUME_NAME - Docker volume name
#   ZITADEL_POSTGRES_SERVICE_NAME - Docker Swarm service name (preferred)
#   ZITADEL_POSTGRES_CONTAINER_NAME - Docker container name (fallback)
#   ZITADEL_BACKUP_LOCAL_DIR - local dir for tar snapshots (default /var/backups/zitadel)
#   ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE - rclone remote name (e.g. "b2:bucket-name/path")
#   ZITADEL_BACKUP_MIN_BYTES - minimum acceptable snapshot size (default 1048576)
#   ZITADEL_BACKUP_HEALTHCHECK_URL - optional URL to verify after restart
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
SERVICE="${ZITADEL_POSTGRES_SERVICE_NAME:-}"
CONTAINER="${ZITADEL_POSTGRES_CONTAINER_NAME:-}"
LOCAL_DIR="${ZITADEL_BACKUP_LOCAL_DIR:-/var/backups/zitadel}"
REMOTE="${ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE:-}"
MIN_BYTES="${ZITADEL_BACKUP_MIN_BYTES:-1048576}"
HEALTHCHECK_URL="${ZITADEL_BACKUP_HEALTHCHECK_URL:-}"
RETAIN_LOCAL="${ZITADEL_BACKUP_RETENTION_LOCAL_DAYS:-30}"
RETAIN_OFFSITE="${ZITADEL_BACKUP_RETENTION_OFFSITE_DAYS:-90}"

TS=$(date -u +%Y%m%d-%H%M%S)
FILE="zitadel-pg-${TS}.tar.gz"
PATH_LOCAL="${LOCAL_DIR}/${FILE}"

mkdir -p "$LOCAL_DIR"

assert_service_mount() {
  if [ -z "$SERVICE" ]; then
    return
  fi

  echo "[$(date -u -Iseconds)] Verifying $SERVICE mounts $VOLUME at /var/lib/postgresql/data"
  MOUNTS=$(docker service inspect "$SERVICE" --format '{{json .Spec.TaskTemplate.ContainerSpec.Mounts}}')
  if ! printf '%s\n' "$MOUNTS" | grep -q "\"Source\":\"${VOLUME}\""; then
    echo "[FATAL] Service $SERVICE does not mount volume $VOLUME"
    exit 1
  fi
  if ! printf '%s\n' "$MOUNTS" | grep -q '"Target":"/var/lib/postgresql/data"'; then
    echo "[FATAL] Service $SERVICE must mount Postgres data at /var/lib/postgresql/data"
    exit 1
  fi
}

assert_volume_has_postgres_data() {
  echo "[$(date -u -Iseconds)] Verifying docker volume $VOLUME contains Postgres data"
  docker run --rm -v "${VOLUME}:/data:ro" busybox sh -c '
    test -s /data/PG_VERSION
    test -d /data/base
    kb=$(du -sk /data | awk "{print \$1}")
    test "$kb" -gt 1024
  '
}

assert_snapshot_size() {
  if [ "$SIZE" -lt "$MIN_BYTES" ]; then
    echo "[FATAL] Snapshot too small: ${SIZE} bytes; expected at least ${MIN_BYTES}"
    exit 1
  fi
}

assert_service_mount
assert_volume_has_postgres_data

if [ -n "$SERVICE" ]; then
  echo "[$(date -u -Iseconds)] Scaling $SERVICE to 0 for consistent snapshot"
  docker service scale "${SERVICE}=0"
  for _ in $(seq 1 60); do
    if [ -z "$(docker ps -q --filter "name=${SERVICE}")" ]; then
      break
    fi
    sleep 1
  done
  trap 'echo "[$(date -u -Iseconds)] Scaling service back to 1 (post-error)"; docker service scale "${SERVICE}=1" || true' ERR
else
  if [ -z "$CONTAINER" ]; then
    echo "[FATAL] Set ZITADEL_POSTGRES_SERVICE_NAME or ZITADEL_POSTGRES_CONTAINER_NAME"
    exit 1
  fi
  echo "[$(date -u -Iseconds)] Stopping $CONTAINER for consistent snapshot"
  docker stop "$CONTAINER"
  trap 'echo "[$(date -u -Iseconds)] Restarting container (post-error)"; docker start "$CONTAINER" || true' ERR
fi

assert_volume_has_postgres_data

echo "[$(date -u -Iseconds)] Creating tar of docker volume $VOLUME -> $PATH_LOCAL"
docker run --rm -v "${VOLUME}:/data:ro" -v "${LOCAL_DIR}:/backup" busybox \
  tar -czf "/backup/${FILE}" -C / data

SIZE=$(stat -c '%s' "$PATH_LOCAL" 2>/dev/null || stat -f '%z' "$PATH_LOCAL")
echo "[$(date -u -Iseconds)] Snapshot done: $PATH_LOCAL (${SIZE} bytes)"
assert_snapshot_size

if [ -n "$SERVICE" ]; then
  echo "[$(date -u -Iseconds)] Scaling $SERVICE back to 1"
  docker service scale "${SERVICE}=1"
  for _ in $(seq 1 90); do
    if docker ps --filter "name=${SERVICE}" --format '{{.Status}}' | grep -q 'Up'; then
      break
    fi
    sleep 1
  done
else
  echo "[$(date -u -Iseconds)] Restarting $CONTAINER"
  docker start "$CONTAINER"
fi

trap - ERR

if [ -n "$HEALTHCHECK_URL" ] && command -v curl >/dev/null 2>&1; then
  echo "[$(date -u -Iseconds)] Verifying healthcheck $HEALTHCHECK_URL"
  for _ in $(seq 1 60); do
    if curl -fsS --max-time 10 "$HEALTHCHECK_URL" >/dev/null; then
      break
    fi
    sleep 2
  done
  curl -fsS --max-time 10 "$HEALTHCHECK_URL" >/dev/null
fi

if [ -n "$REMOTE" ] && command -v rclone >/dev/null 2>&1; then
  echo "[$(date -u -Iseconds)] Uploading to offsite $REMOTE"
  rclone copy "$PATH_LOCAL" "$REMOTE"
  echo "[$(date -u -Iseconds)] Pruning offsite backups older than ${RETAIN_OFFSITE}d"
  rclone delete --min-age "${RETAIN_OFFSITE}d" "$REMOTE" || true
else
  echo "[$(date -u -Iseconds)] Skip offsite upload (REMOTE empty or rclone missing)"
fi

echo "[$(date -u -Iseconds)] Pruning local backups older than ${RETAIN_LOCAL}d"
find "$LOCAL_DIR" -name 'zitadel-pg-*.tar.gz' -mtime "+${RETAIN_LOCAL}" -delete

echo "[$(date -u -Iseconds)] Done: ${FILE}"
