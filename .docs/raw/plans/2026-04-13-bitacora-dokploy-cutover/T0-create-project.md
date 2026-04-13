# Task T0: Create/Seed Dokploy Project

## Shared Context
**Goal:** Create a dedicated Dokploy project `bitacora` and resolve the environment creation order.
**Stack:** Dokploy API on `http://54.37.157.93:3000`, server `sFel4YXcOTVOauF7quUUa`.
**Architecture:** Existing project is `nuestrascuentitas`; new target is an isolated `bitacora` project.

## Task Metadata
```yaml
id: T0
depends_on: []
agent_type: ps-worker
files:
  - read: infra/dokploy/bitacora-api.production.md:17-40
  - read: /home/fgpaz/.agents/skills/dokploy-cli/references/api-endpoints.md:11-18
complexity: medium
done_when: "Dokploy returns a new projectId for project 'bitacora' and the environment path is known"
```

## Reference
`infra/dokploy/bitacora-api.production.md:17-22` shows the intended `project.create` shape using an existing `DOKPLOY_ENVIRONMENT_ID`.

## Prompt
Use the Dokploy API directly with `x-api-key`.

1. Probe whether project `bitacora` already exists by reading `project.all` and filtering by name.
2. If it does not exist, POST `project.create` with:
   - `name: bitacora`
   - `description: Bitacora dedicated production lane`
   - `environmentId: kU9BSDBGb_y4IRu8AZTTd`
3. Read the resulting project payload and capture `projectId`.
4. Probe `environment.byProjectId` for the new project.
5. If no `production` environment exists under the new project, POST `environment.create` with `name=production`, `projectId=<newProjectId>`, `description=Production environment`.
6. Save the discovered ids in local notes only; do not write secrets to tracked files.
7. Report the exact `projectId` and `environmentId` chosen for downstream tasks.

## Skeleton
```bash
curl -s -H "x-api-key: $DOKPLOY_API_KEY" "$DOKPLOY_URL/api/project.all"
curl -s -X POST -H "x-api-key: $DOKPLOY_API_KEY" -H "Content-Type: application/json" -d '{...}' "$DOKPLOY_URL/api/project.create"
```

## Verify
`curl -s -H "x-api-key: $DOKPLOY_API_KEY" "$DOKPLOY_URL/api/project.one?projectId=<id>"` returns the new `bitacora` project.

## Commit
`docs(plan): add Dokploy project cutover execution plan`
