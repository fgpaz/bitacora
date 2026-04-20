# Secret Source Runbook

## Purpose

Bitacora uses a local untracked `infra/.env` as the repo-local bridge for runtime and Dokploy execution.

## Source of truth

- control-plane, Zitadel, and rollback auth secrets: Infisical via `mi-key-cli` (`mkey pull bitacora prod`)
- access path: `mi-key-cli`

Validated facts:

- `mkey doctor` succeeds in `multi-tedi`
- `mkey status` can reach the `teslita` vault and `mkey pull bitacora prod` can materialize Bitacora prod values without plaintext commits
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
- `ZITADEL_AUTHORITY`
- `ZITADEL_AUDIENCE`
- `ZITADEL_WEB_CLIENT_ID`
- `ZITADEL_WEB_REDIRECT_URI`
- `ZITADEL_WEB_POST_LOGOUT_REDIRECT_URI`
- Bitacora-specific encryption key, pseudonym salt, and DB password
