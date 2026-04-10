# Secret Source Runbook

## Purpose

Bitacora uses a local untracked `infra/.env` as the repo-local bridge for runtime and Dokploy execution.

## Source of truth

- control-plane and shared auth secrets: `C:\repos\mios\multi-tedi`
- access path: `mi-key-cli`

Validated facts:

- `mkey doctor` succeeds in `multi-tedi`
- `mkey status` can reach the `teslita` vault
- `multi-tedi/infra/secrets.enc.env` contains encrypted Dokploy control-plane keys
- `multi-tedi/.mcp.json` is not the current Dokploy truth source

## Rules

- never commit `infra/.env`
- never copy the full `multi-tedi` secret set into Bitacora
- only materialize the fields listed in `infra/.env.template`

## Minimum Bitacora local bridge

- `DOKPLOY_URL`
- `DOKPLOY_API_KEY`
- `DOKPLOY_ENVIRONMENT_ID`
- `DOKPLOY_GITHUB_PROVIDER_ID`
- `SUPABASE_JWT_SECRET`
- Bitacora-specific encryption key, pseudonym salt, and DB password
