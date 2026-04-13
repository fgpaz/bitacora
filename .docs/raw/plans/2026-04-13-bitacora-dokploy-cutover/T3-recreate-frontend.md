# Task T3: Recreate Frontend App With Nixpacks

## Shared Context
**Goal:** Provision a fresh `bitacora-frontend` in the new Dokploy project using GitHub+nixpacks.
**Stack:** Next.js 16 in repo subfolder `frontend`, buildPath `frontend`, port `3000`.
**Architecture:** Frontend should stop depending on the custom Dockerfile path and use Dokploy's native GitHub+nixpacks flow.

## Task Metadata
```yaml
id: T3
depends_on: [T0]
agent_type: ps-worker
files:
  - read: frontend/package.json:1-25
  - read: frontend/next.config.js:1-12
  - read: frontend/.env.example:1-12
complexity: high
done_when: "A new bitacora-frontend app exists with buildType=nixpacks, buildPath=frontend, and frontend env saved"
```

## Reference
`frontend/package.json` and `frontend/next.config.js` confirm a standard Next.js build. Explorer findings say Dokploy buildPath should be `frontend` without a leading slash.

## Prompt
1. Create a new application `bitacora-frontend` under `<newProjectId>`.
2. Save build type `nixpacks`.
3. Attach GitHub provider:
   - `repository: bitacora`
   - `owner: fgpaz`
   - `branch: main`
   - `githubId: dw08YcoirLF5MI3lIE5zc`
4. Use `application.update` to align these fields:
   - `description: Bitacora frontend production`
   - `buildPath: frontend`
   - `buildType: nixpacks`
   - `serverId: sFel4YXcOTVOauF7quUUa` and `buildServerId: sFel4YXcOTVOauF7quUUa` if required
   - `replicas: 1`
5. Save the runtime env and build args equivalents with:
   - `NEXT_PUBLIC_API_BASE_URL=https://api.bitacora.nuestrascuentitas.com`
   - `NEXT_PUBLIC_SUPABASE_URL=https://auth.tedi.nuestrascuentitas.com`
   - `NEXT_PUBLIC_SUPABASE_ANON_KEY=<same value as current frontend app>`
6. Do not attach the public domain yet; that happens in T4 after the app is green.
7. If Dokploy rejects `buildPath`, stop and report the exact API error instead of guessing alternative fields.

## Skeleton
```bash
curl -s -X POST -H "x-api-key: $DOKPLOY_API_KEY" -H "Content-Type: application/json" \
  -d '{"name":"bitacora-frontend","projectId":"...","description":"Bitacora frontend production"}' \
  "$DOKPLOY_URL/api/application.create"
```

## Verify
`curl -s -H "x-api-key: $DOKPLOY_API_KEY" "$DOKPLOY_URL/api/application.one?applicationId=<id>"` shows `buildType=nixpacks` and frontend env vars present.

## Commit
`chore(infra): recreate bitacora frontend with nixpacks`
