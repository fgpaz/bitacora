# G5 — Legacy Postgres Cleanup

**Date:** 2026-04-19  
**Status:** AMBER / blocked for destructive approval  
**Tracking:** fgpaz/bitacora#18

## Result

Legacy Postgres is **not safe to remove automatically**. It is still running and contains real Zitadel eventstore data.

| Check | Result |
|-------|--------|
| Service | `postgres-bypass-wireless-bus-tupzoj` |
| Dokploy postgresId | `5e5bKMG5jBS30S7d8nn6c` |
| Service state | `1/1` |
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

## Recommendation

Do not call `postgres.remove` yet. Safe cleanup requires explicit owner approval after one of these paths:

- Snapshot legacy volume, stop service, keep a soak window, then remove.
- Keep legacy service as a cold forensic fallback and rename/document it clearly in Dokploy.

No destructive action was taken in this gap closure pass.
