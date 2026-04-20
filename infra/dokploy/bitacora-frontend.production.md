# Bitacora Frontend - Production Dokploy Spec

## Truthful scope for T01

- Runtime: Next.js 16 standalone (output: standalone)
- Build source: `frontend/Dockerfile`
- Public host: `bitacora.nuestrascuentitas.com`
- Internal port: `3000`
- App ID (Dokploy): `BRTMuvBfWtslXHnShtrnB`
- Docker Node version: 22 (`package.json` allows Node `>=22 <25` for local tooling compatibility)

## Required environment variables

```
NEXT_PUBLIC_API_BASE_URL=https://api.bitacora.nuestrascuentitas.com
API_BASE_URL=https://api.bitacora.nuestrascuentitas.com
ZITADEL_AUTHORITY=https://id.nuestrascuentitas.com
ZITADEL_ISSUER=https://id.nuestrascuentitas.com
ZITADEL_WEB_CLIENT_ID=369306336963330406
ZITADEL_PROJECT_BITACORA_ID=369306332534145382
ZITADEL_WEB_REDIRECT_URI=https://bitacora.nuestrascuentitas.com/auth/callback
ZITADEL_WEB_POST_LOGOUT_REDIRECT_URI=https://bitacora.nuestrascuentitas.com/
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
  -d "{\"applicationId\":\"$FRONTEND_APP_ID\",\"env\":\"NEXT_PUBLIC_API_BASE_URL=https://api.bitacora.nuestrascuentitas.com\nAPI_BASE_URL=https://api.bitacora.nuestrascuentitas.com\nZITADEL_AUTHORITY=https://id.nuestrascuentitas.com\nZITADEL_ISSUER=https://id.nuestrascuentitas.com\nZITADEL_WEB_CLIENT_ID=369306336963330406\nZITADEL_PROJECT_BITACORA_ID=369306332534145382\nZITADEL_WEB_REDIRECT_URI=https://bitacora.nuestrascuentitas.com/auth/callback\nZITADEL_WEB_POST_LOGOUT_REDIRECT_URI=https://bitacora.nuestrascuentitas.com/\nNODE_ENV=production\"}" \
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
pwsh -File .\infra\smoke\zitadel-cutover-smoke.ps1
```

## Blocking conditions

- DNS for `bitacora.nuestrascuentitas.com` must point to `54.37.157.93`
- Zitadel redirect URI must include `https://bitacora.nuestrascuentitas.com/auth/callback`
- `output: 'standalone'` must be active in `next.config.mjs` (verified 2026-04-14)
- Dockerfile must use `node:22-slim` (inside the supported `package.json` engine range `>=22 <25`)
