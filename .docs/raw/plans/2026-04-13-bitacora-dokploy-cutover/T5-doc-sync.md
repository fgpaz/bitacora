# Task T5: Sync Docs After Cutover

## Shared Context
**Goal:** Update release/readiness docs to reflect the dedicated `bitacora` Dokploy project and new resource ids.
**Stack:** Markdown docs under `.docs/raw/decisiones/` and `infra/dokploy/`.
**Architecture:** Documentation must stop saying Bitacora lives inside `nuestrascuentitas` once cutover succeeds.

## Task Metadata
```yaml
id: T5
depends_on: [T4]
agent_type: ps-docs
files:
  - modify: .docs/raw/decisiones/2026-04-13-bitacora-release-readiness.md
  - modify: infra/dokploy/bitacora-api.production.md
  - modify: infra/dokploy/bitacora-db.production.md
complexity: low
done_when: "Docs reference the new project name and updated app/postgres ids"
```

## Reference
`2026-04-13-bitacora-release-readiness.md` currently names project `nuestrascuentitas`; that becomes stale after cutover.

## Prompt
1. Update the release readiness note to state that Bitacora now lives in its own Dokploy project `bitacora`.
2. Replace old app/postgres ids with the recreated ones.
3. Note the final frontend strategy as `GitHub + nixpacks`.
4. Keep secrets out of docs.

## Skeleton
```md
- Dokploy project: `bitacora`
- Environment: `production`
- Frontend build type: `nixpacks`
```

## Verify
`git diff --stat .docs/raw/decisiones/2026-04-13-bitacora-release-readiness.md infra/dokploy/bitacora-api.production.md infra/dokploy/bitacora-db.production.md` shows the sync changes.

## Commit
`docs(infra): sync Dokploy cutover state for dedicated bitacora project`
