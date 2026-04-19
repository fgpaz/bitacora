# Task G5: Cleanup Legacy Postgres Safely

## Shared Context
**Goal:** Remove unused legacy Postgres without losing recoverable data.
**Stack:** Dokploy Postgres, Docker volumes, SSH, OIDC smoke.
**Architecture:** Active Zitadel uses `postgres-reboot-wireless-panel-chhbwg` and `shared_zitadel_v4`; legacy is `postgres-bypass-wireless-bus-tupzoj`.

## Locked Decisions
- Legacy PG is non-empty, so direct remove is forbidden.
- Required path: snapshot, checksum, stop, soak, active smoke, remove after explicit user approval.
- Do not remove active PG `BjjSOBWAwe6XpGttK4XoY`.

## Task Metadata
```yaml
id: G5
depends_on: [G2_G6]
agent_type: ps-worker
files:
  - create: .docs/raw/reports/2026-04-19-cerrar-gaps/G5-legacy-postgres.md
complexity: high
done_when: "Dokploy project shows only active PG and OIDC discovery remains 200 after removal"
```

## Reference
`.docs/raw/reports/2026-04-18-wave-a-audit/T1.4-backup-state.md:19` — legacy PG metadata.

## Prompt
Take a final tar snapshot of `postgres-bypass-wireless-bus-tupzoj-data`, checksum it, verify aggregate counts, stop only legacy PG, soak, smoke active Zitadel, then request explicit destructive approval before `postgres.remove`. After removal, verify OIDC/JWKS/PAT and project inventory.

## Execution Procedure
1. Confirm active app env points to `postgres-reboot-wireless-panel-chhbwg` and `shared_zitadel_v4`.
2. Snapshot legacy volume to `/var/backups/zitadel/zitadel-legacy-pg18-<ts>.tar.gz`.
3. Record size and `sha256sum`.
4. Record legacy event counts: `events2`, `instance.added`, `user.human.added`.
5. Stop legacy PG only through Dokploy.
6. Soak at least 10 minutes while checking OIDC discovery/JWKS/PAT.
7. Ask user explicitly for destructive approval to remove `postgresId=5e5bKMG5jBS30S7d8nn6c`.
8. Remove legacy PG after approval.
9. Verify project inventory no longer includes legacy PG.
10. Commit with `ops(zitadel): remove legacy postgres`.

## Skeleton
```bash
docker run --rm -v postgres-bypass-wireless-bus-tupzoj-data:/data:ro -v /var/backups/zitadel:/backup busybox \
  tar -czf /backup/zitadel-legacy-pg18-<ts>.tar.gz -C /data .
sha256sum /var/backups/zitadel/zitadel-legacy-pg18-<ts>.tar.gz
```

## Verify
`curl -sS -o /dev/null -w "%{http_code}" https://id.nuestrascuentitas.com/.well-known/openid-configuration` -> `200`

## Commit
`ops(zitadel): remove legacy postgres`
