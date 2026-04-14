# Bitacora Frontend - Production Dokploy Spec

## Truthful scope for T01

- Runtime: Next.js 16 standalone (output: standalone)
- Build source: `frontend/Dockerfile`
- Public host: `bitacora.nuestrascuentitas.com`
- Internal port: `3000`
- App ID (Dokploy): `BRTMuvBfWtslXHnShtrnB`
- Node version: 22 (matches package.json engines.node)

## Required environment variables

```
NEXT_PUBLIC_API_BASE_URL=https://api.bitacora.nuestrascuentitas.com
NEXT_PUBLIC_SUPABASE_URL=https://auth.tedi.nuestrascuentitas.com
NEXT_PUBLIC_SUPABASE_ANON_KEY=<from infra/.env or infisical vault teslita>
NODE_ENV=production
```

## Build context note

The Dockerfile is located at `frontend/Dockerfile`. Dokploy must be configured with:
- Dockerfile path: `frontend/Dockerfile`
- Build context: `frontend/` (or repo root, depending on Dokploy version)

## Configure env vars via Dokploy API

```bash
DOKPLOY_URL="http://54.37.157.93:3000"
DOKPLOY_API_KEY="<from infra/.env>"
FRONTEND_APP_ID="BRTMuvBfWtslXHnShtrnB"

curl -s -X POST -H "x-api-key: $DOKPLOY_API_KEY" \
  -H "Content-Type: application/json" \
  -d "{\"applicationId\":\"$FRONTEND_APP_ID\",\"env\":\"NEXT_PUBLIC_API_BASE_URL=https://api.bitacora.nuestrascuentitas.com\nNEXT_PUBLIC_SUPABASE_URL=https://auth.tedi.nuestrascuentitas.com\nNEXT_PUBLIC_SUPABASE_ANON_KEY=<anon_key>\nNODE_ENV=production\"}" \
  "$DOKPLOY_URL/api/application.saveEnvironment"
```

## Create public domain

```bash
curl -s -X POST -H "x-api-key: $DOKPLOY_API_KEY" \
  -H "Content-Type: application/json" \
  -d "{\"applicationId\":\"$FRONTEND_APP_ID\",\"host\":\"bitacora.nuestrascuentitas.com\",\"port\":3000,\"https\":true,\"certificateType\":\"letsencrypt\"}" \
  "$DOKPLOY_URL/api/domain.create"
```

## Deploy

```bash
curl -s -X POST -H "x-api-key: $DOKPLOY_API_KEY" \
  -H "Content-Type: application/json" \
  -d "{\"applicationId\":\"$FRONTEND_APP_ID\"}" \
  "$DOKPLOY_URL/api/application.deploy"
```

## Smoke test after deploy

```bash
curl -f https://bitacora.nuestrascuentitas.com
curl -I https://bitacora.nuestrascuentitas.com
```

## Blocking conditions

- DNS for `bitacora.nuestrascuentitas.com` must point to `54.37.157.93`
- `NEXT_PUBLIC_SUPABASE_ANON_KEY` must be set (without it the auth client fails to initialize)
- `output: 'standalone'` must be active in `next.config.mjs` (verified 2026-04-14)
- Dockerfile must use `node:22-slim` (matches package.json engines.node: 22)
