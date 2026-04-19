# G5 — Legacy Postgres Cleanup

**Date:** 2026-04-19  
**Status:** GREEN
**Tracking:** fgpaz/bitacora#18

## Result

Legacy Postgres was removed after explicit owner approval on 2026-04-19. A stopped-volume snapshot was created before removal and copied offsite.

| Check | Result |
|-------|--------|
| Service | `postgres-bypass-wireless-bus-tupzoj` |
| Dokploy postgresId | `5e5bKMG5jBS30S7d8nn6c` |
| Initial service state | `1/1` |
| Volume | `postgres-bypass-wireless-bus-tupzoj-data` |
| Volume size | `66M` |
| Database | `shared_zitadel` |
| `eventstore.events2` count | `801` |

Top event groups observed:

| aggregate_type | event_type | count |
|----------------|------------|-------|
| permission | permission.added | `483` |
| system | system.migration.started | `94` |
| instance | instance.customtext.set | `72` |
| system | system.migration.failed | `69` |
| system | system.migration.done | `25` |
| instance | instance.secret.generator.added | `9` |
| project | project.application.added | `4` |
| project | project.application.config.api.added | `3` |

## Cleanup Evidence

| Check | Result |
|-------|--------|
| Owner approval | explicit approval in session: "para g5 apruebo expresamente a hacerlo" |
| Stop | Dokploy `POST postgres.stop`, service reached `0/0` |
| Post-stop OIDC discovery | `200` |
| Post-stop JWKS | `200` |
| Post-stop admin PAT | `200`, org `nuestrascuentitas` |
| Snapshot | `/opt/zitadel-backup/legacy/legacy-zitadel-pg18-20260419-200003.tar.gz` |
| Snapshot size | `8,090,041` bytes |
| Snapshot sha256 | `447bca8e914512cb7f167c31b177998c3ccfa4d7a35b3d832ae5823e3e5f4be0` |
| Offsite copy | `teslita-zitadel:/home/fgpaz/backups/zitadel/legacy/legacy-zitadel-pg18-20260419-200003.tar.gz` |
| Dokploy removal | `POST postgres.remove` for `5e5bKMG5jBS30S7d8nn6c` |
| Final service count | `0` |
| Final container count | `0` |
| Final volume count | `0` |
| Final OIDC discovery | `200` |
| Final JWKS | `200` |
| Final admin PAT | `200`, org `nuestrascuentitas` |

No active Zitadel surface regressed during stop or removal.
