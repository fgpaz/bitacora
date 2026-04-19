# Zitadel Teslita — Recovery Runbook

Common failure scenarios and recovery steps.

## Scenario 1: Zitadel container down, DB intact

Symptom: `curl https://id.nuestrascuentitas.com/.well-known/openid-configuration` → connection refused / 502.

```bash
# Check status
bash ~/.claude/skills/dokploy-cli/scripts/dkp.sh status zFdEECmPr1hhxwL0DKu4B
# Redeploy
curl -sS -X POST -H "x-api-key: $DOKPLOY_API_KEY" -H "Content-Type: application/json" \
  -d '{"applicationId":"zFdEECmPr1hhxwL0DKu4B"}' "$DOKPLOY_URL/api/application.redeploy"
# Verify
sleep 30 && curl -sS https://id.nuestrascuentitas.com/.well-known/openid-configuration
```

## Scenario 2: Postgres container down, volume intact

Symptom: Zitadel reports 503 / DB connection errors.

```bash
sshr exec --host turismo --cmd "docker ps -a --filter name=postgres-reboot-wireless-panel"
# Restart service via Dokploy UI or:
curl -sS -X POST -H "x-api-key: $DOKPLOY_API_KEY" -H "Content-Type: application/json" \
  -d '{"postgresId":"BjjSOBWAwe6XpGttK4XoY"}' "$DOKPLOY_URL/api/postgres.start"
```

## Scenario 3: Volume corrupted — restore from offsite backup

```bash
# 1. Download latest snapshot
rclone lsl teslita-zitadel:/home/fgpaz/backups/zitadel | sort | tail -1
rclone copy teslita-zitadel:/home/fgpaz/backups/zitadel/zitadel-pg-<ts>.tar.gz /tmp/

# 2. Stop Postgres
curl -sS -X POST -H "x-api-key: $DOKPLOY_API_KEY" \
  -d '{"postgresId":"BjjSOBWAwe6XpGttK4XoY"}' "$DOKPLOY_URL/api/postgres.stop"

# 3. Replace volume contents (WARNING: destructive)
docker run --rm -v postgres-reboot-wireless-panel-chhbwg-data:/data -v /tmp:/backup busybox \
  sh -c 'rm -rf /data/* /data/..?* /data/.[!.]* 2>/dev/null; tar -xzf /backup/zitadel-pg-<ts>.tar.gz -C /'

# 4. Ensure Dokploy service mounts the named volume at /var/lib/postgresql/data
docker service inspect postgres-reboot-wireless-panel-chhbwg \
  --format '{{json .Spec.TaskTemplate.ContainerSpec.Mounts}}'

# 5. Start Postgres + verify healthcheck
# 6. Start Zitadel
# 7. Validate: curl OIDC discovery = 200; PAT works against /management/v1/orgs/me
```

Before restoring, check the tar is not the rejected 89-byte incident artifact and is larger than `ZITADEL_BACKUP_MIN_BYTES` (`1048576` by default).

## Scenario 4: Admin locked out (lost PAT + no browser login)

1. Restore PAT from Infisical: `mkey pull bitacora prod --stdout | grep ZITADEL_ADMIN_PAT`
2. If Infisical value wrong/expired: rotate via bootstrap re-run (destructive on shared_zitadel_v4) — see bootstrap runbook step 4.
3. As last resort: use the human admin password (`ZITADEL_ADMIN_INITIAL_PASSWORD`) to login via UI (requires login companion deployed).

## Scenario 5: Masterkey lost

Destructive — encryption keys stored in DB can no longer be decrypted. Requires destroy-and-recreate (bootstrap runbook).

Mitigation: masterkey is persisted in Infisical key `ZITADEL_MASTERKEY`. Never regenerate unless starting fresh.

## Scenario 6: Cert TLS expired / renewal fails

Let's Encrypt autorenews via Traefik. If renewal fails (logs in `dokploy-traefik` service):
- Check CAA DNS records include `letsencrypt.org`
- Check DNS not proxied by Cloudflare
- If rate-limited: wait 7d or switch to LE staging env
