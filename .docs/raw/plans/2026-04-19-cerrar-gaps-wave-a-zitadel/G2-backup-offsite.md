# Task G2/G6: Install Backup Cron and Teslita Offsite

## Shared Context
**Goal:** Make Zitadel volume snapshots run daily and upload offsite at zero provider cost.
**Stack:** `sshr`, Docker volume snapshot, rclone SFTP, Tailscale host `teslita`.
**Architecture:** `turismo` pushes snapshots to `teslita:/home/fgpaz/backups/zitadel/`.

## Locked Decisions
- Use docker volume snapshot, not `pg_dump`.
- Use offsite `teslita` via Tailscale/SFTP, not paid B2/R2/Hetzner.
- Retain 30 days local and 90 days offsite.

## Task Metadata
```yaml
id: G2_G6
depends_on: [G0]
agent_type: ps-worker
files:
  - modify: infra/runbooks/zitadel-backup.md
  - modify: infra/.env.template
  - create: .docs/raw/reports/2026-04-19-cerrar-gaps/G2-G6-backup-offsite.md
complexity: high
done_when: "Manual backup file exists locally and rclone lists it on teslita offsite"
```

## Reference
`infra/backups/zitadel/snapshot.sh` — canonical backup script.

## Prompt
Install the existing script at `/opt/zitadel-backup/snapshot.sh` on `turismo`, configure `/opt/zitadel-backup/.env`, configure an SSH key from `turismo` to `teslita`, configure an rclone SFTP remote named `teslita-zitadel`, install `/etc/cron.d/zitadel-backup`, run one manual backup, and verify the artifact exists locally and remotely. Store remote name and non-secret config in Infisical/template; store private key only in Infisical or root-owned server file, never docs.

## Execution Procedure
1. On `teslita`, create `/home/fgpaz/backups/zitadel` with owner `fgpaz`.
2. On `turismo`, install `rclone` if missing.
3. Generate or install dedicated SSH key for backup push; append public key to `teslita` authorized_keys with a restrictive comment.
4. Copy `infra/backups/zitadel/snapshot.sh` to `/opt/zitadel-backup/snapshot.sh`.
5. Create `/opt/zitadel-backup/.env` with active volume/container and `ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE=teslita-zitadel:/home/fgpaz/backups/zitadel`.
6. Configure rclone SFTP remote to host `teslita` or `100.115.71.27`.
7. Add `/etc/cron.d/zitadel-backup` for 03:00 UTC daily.
8. Run manual backup and verify OIDC discovery after restart.
9. Verify `rclone ls teslita-zitadel:/home/fgpaz/backups/zitadel` shows the new tar.
10. Write evidence and commit with `ops(zitadel): install backup offsite to teslita`.

## Skeleton
```dotenv
ZITADEL_POSTGRES_VOLUME_NAME=postgres-reboot-wireless-panel-chhbwg-data
ZITADEL_POSTGRES_CONTAINER_NAME=<current-active-container>
ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE=teslita-zitadel:/home/fgpaz/backups/zitadel
```

## Verify
`rclone ls teslita-zitadel:/home/fgpaz/backups/zitadel | tail -1` -> includes `zitadel-pg-*.tar.gz`

## Commit
`ops(zitadel): install backup offsite to teslita`
