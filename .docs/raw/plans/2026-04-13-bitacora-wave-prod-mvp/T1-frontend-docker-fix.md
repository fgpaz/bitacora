# Task T1: Resolve Docker Hub Blocking on Dokploy

## Shared Context
**Goal:** Get `bitacora-frontend` Docker image successfully built and running on Dokploy VPS at `bitacora.nuestrascuentitas.com`.
**Stack:** Next.js 16 standalone Docker, Dokploy VPS at `54.37.157.93:3000`, Docker Hub.
**Architecture:** `node:20-slim` base image, `NEXT_PUBLIC_*` env vars injected at runtime. App ID: `ApFt0xks7Z2uycsz_ogl1`.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-worker
files:
  - read: frontend/Dockerfile
  - read: frontend/next.config.js
complexity: high
done_when: "curl https://bitacora.nuestrascuentitas.com returns 200; Dokploy redeploy succeeds"
```

## Problem Statement
Dokploy server at `54.37.157.93` has `credsStore: desktop.exe` globally configured. Every `docker pull` or `docker build` fails with `error getting credentials`. Three prior redeploys all failed.

## Reference
- `frontend/Dockerfile` — existing multi-stage build using `node:20-slim`
- `frontend/next.config.js` — `output: 'standalone'` confirmed at line 4
- Dokploy app: `bitacora-frontend` ID `ApFt0xks7Z2uycsz_ogl1`, project `nuestrascuentitas` ID `L9dN52J9eqSNwC_HXPbHZ`
- `~/.agents/skills/dokploy-cli/scripts/dkp.sh` — Dokploy API CLI

## Three-Step Fix Strategy (try in order)

### Option A: Dokploy Dashboard Registry Config (PRIORITY 1)
1. Open Dokploy web UI: `http://54.37.157.93:3000`
2. Navigate to `bitacora-frontend` app settings
3. Find "Registry", "Docker Settings", or "Image Pull Settings"
4. Add Docker Hub credentials (username + password/access token)
5. Save and trigger redeploy via `dkp.sh`: `~/.agents/skills/dokploy-cli/scripts/dkp.sh redeploy bitacora-frontend`
6. Monitor: `~/.agents/skills/dokploy-cli/scripts/dkp.sh logs bitacora-frontend --tail 50`

### Option B: GitHub Container Registry (PRIORITY 2)
If Option A fails or is not available in UI:
1. Build image locally: `docker build -t bitacora-frontend:$(git rev-parse --short HEAD) -t bitacora-frontend:latest ./frontend/`
2. Tag for GHCR: `docker tag bitacora-frontend:latest ghcr.io/fgpaz/bitacora-frontend:latest`
3. Push to GHCR: `docker push ghcr.io/fgpaz/bitacora-frontend:latest` (requires GH CLI `gh auth login`)
4. If GH CLI not available, try `docker login ghcr.io -u fgpaz -p $(gh auth token)` via GH CLI
5. Update Dokploy app to pull from `ghcr.io/fgpaz/bitacora-frontend:latest` instead of Dockerfile build

### Option C: Inline Registry Auth via Dokploy API (PRIORITY 3)
If neither A nor B works:
1. Try `dkp.sh registries list` to see available registries
2. Try `dkp.sh registries add dockerhub --username X --password Y`
3. Or set registry at app level via Dokploy API

## Prompt

You are a DevOps specialist. Your job is to get `bitacora-frontend` deployed and live at `https://bitacora.nuestrascuentitas.com`.

**Start with Option A (Dokploy dashboard).**

Use the `dokploy-mcp` skill or `dkp.sh` CLI to:
1. Check if there is a way to configure registry credentials via API
2. Try `~/.agents/skills/dokploy-cli/scripts/dkp.sh registries list`
3. Try `~/.agents/skills/dokploy-cli/scripts/dkp.sh apps list` to find the frontend app
4. Attempt Option A/B/C in order

**If Option A (dashboard) is needed:**
- This requires human interaction in a browser — report clearly what steps a human must perform in the Dokploy web UI
- Document the exact navigation: Settings > Registry > Add Credentials > Docker Hub > username + password

**If Option B (GHCR) is viable:**
- Verify if `gh` CLI is available: `which gh && gh auth status`
- If available, execute the GHCR push workflow
- If not available, document the exact commands needed

**If Option C (API registry):**
- Try `dkp.sh registries --help` or equivalent to find registry config commands

**Report:**
- Which option was attempted
- What happened (success/failure with exact error)
- Exact commands run and their output
- What the next human step is if blocked

## Verify
`curl -I https://bitacora.nuestrascuentitas.com` returns HTTP 200.

## Commit
`fix(dokploy): resolve Docker Hub credential blocking for bitacora-frontend`
