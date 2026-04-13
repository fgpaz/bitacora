# Task T2: Recreate API App

## Shared Context
**Goal:** Provision a fresh `bitacora-api` in the new Dokploy project using GitHub+dockerfile.
**Stack:** GitHub provider `dw08YcoirLF5MI3lIE5zc`, Dockerfile build from repo root.
**Architecture:** API serves `api.bitacora.nuestrascuentitas.com` and later the parking/root host during cutover.

## Task Metadata
```yaml
id: T2
depends_on: [T0, T1]
agent_type: ps-worker
files:
  - read: infra/dokploy/bitacora-api.production.md:24-76
  - read: src/Bitacora.Api/Dockerfile:1-19
complexity: high
done_when: "A new bitacora-api app exists with GitHub provider, dockerfile build, and runtime env saved"
```

## Reference
Use the current live app `FBmBaFs9cZKgzbDICN3xo` as the source of truth for non-secret fields like description, repo, branch, dockerfile path, and domains.

## Prompt
1. Create a new application `bitacora-api` under `<newProjectId>`.
2. Save build type `dockerfile`.
3. Attach GitHub provider:
   - `repository: bitacora`
   - `owner: fgpaz`
   - `branch: main`
   - `githubId: dw08YcoirLF5MI3lIE5zc`
4. Use `application.update` to align these fields with the live app:
   - `description: Bitacora.Api production`
   - `dockerfile: Dockerfile`
   - `dockerContextPath: .`
   - `buildPath: /`
   - `buildType: dockerfile`
   - `sourceType: github` when accepted
   - `serverId: sFel4YXcOTVOauF7quUUa` and `buildServerId: sFel4YXcOTVOauF7quUUa` if required
5. Save the runtime env with at least:
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ConnectionStrings__BitacoraDb=Host=<newDbHost>;Port=5432;Database=bitacora_db;Username=bitacora;Password=<password>`
   - `SUPABASE_JWT_SECRET=<from current app or old infra source if retrievable>`
   - `BITACORA_ENCRYPTION_KEY=ERJY/JsAfer68SjiIt2CwRGAP+IeUyT7ZHlfhuMLugw=`
   - `BITACORA_PSEUDONYM_SALT=<must be sourced from current runtime before deploy; if unavailable, stop and report blocker>`
   - `BITACORA_BASE_URL=https://api.bitacora.nuestrascuentitas.com`
   - `BITACORA_TELEGRAM_BOT_TOKEN=<current token>`
   - `TELEGRAM_BOT_TOKEN=<current token>`
6. Do not attach public domains yet; that happens in T4 after validation.

## Skeleton
```bash
curl -s -X POST -H "x-api-key: $DOKPLOY_API_KEY" -H "Content-Type: application/json" \
  -d '{"name":"bitacora-api","projectId":"...","description":"Bitacora.Api production"}' \
  "$DOKPLOY_URL/api/application.create"
```

## Verify
`curl -s -H "x-api-key: $DOKPLOY_API_KEY" "$DOKPLOY_URL/api/application.one?applicationId=<id>"` shows `name=bitacora-api`, `buildType=dockerfile`, `repository=bitacora`.

## Commit
`chore(infra): recreate bitacora api in dedicated Dokploy project`
