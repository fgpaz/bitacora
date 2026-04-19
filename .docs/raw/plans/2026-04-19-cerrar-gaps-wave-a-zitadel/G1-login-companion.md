# Task G1: Deploy Zitadel Login Companion

## Shared Context
**Goal:** Make Zitadel Login UI v2 accessible for Wave B and MFA.
**Stack:** Dokploy Docker app, Traefik, `ghcr.io/zitadel/zitadel-login:v4.9.0`.
**Architecture:** Login companion serves `/ui/v2/login`; Zitadel core remains on port 8080.

## Locked Decisions
- Deploy official image `ghcr.io/zitadel/zitadel-login:v4.9.0`, never `:latest`.
- Use pass-through Traefik route; do not strip `/ui/v2/login`.
- Keep OIDC discovery and JWKS green after routing changes.

## Task Metadata
```yaml
id: G1
depends_on: [G0]
agent_type: ps-worker
files:
  - create: infra/dokploy/zitadel-login/README.md
  - create: infra/dokploy/zitadel-login/compose.reference.yml
  - modify: infra/.env.template
  - create: .docs/raw/reports/2026-04-19-cerrar-gaps/G1-login-companion.md
complexity: high
done_when: "curl https://id.nuestrascuentitas.com/ui/v2/login returns 200 and OIDC discovery remains 200"
```

## Reference
`infra/dokploy/zitadel/compose.yml` — mirror style for pinned Docker image and env reference.

## Prompt
Create a second Dokploy app in project `AKHsAJScexTwhJBzfFRlk`. Use `ZITADEL_API_URL=http://app-program-optical-matrix-xmjswe:8080`, `NEXT_PUBLIC_BASE_PATH=/ui/v2/login`, `CUSTOM_REQUEST_HEADERS=Host:id.nuestrascuentitas.com`, and service user token from `ZITADEL_LOGIN_CLIENT_PAT`. Prefer token file if Dokploy supports mounted secret files; otherwise set env and do not paste raw app status into docs. Add route priority so `/ui/v2/login` wins over the host-only core route.

## Execution Procedure
1. Read `dokploy-cli` endpoint reference before creating the app.
2. Inspect existing app/domain/Traefik config for `zFdEECmPr1hhxwL0DKu4B`.
3. Create app `teslita-shared-zitadel-login` with image `ghcr.io/zitadel/zitadel-login:v4.9.0`, port `3000`, and memory constraints.
4. Configure env without printing token values.
5. Configure Traefik path router `Host(id.nuestrascuentitas.com) && PathPrefix(/ui/v2/login)`, no strip, higher priority.
6. Deploy app and wait for healthy container.
7. Smoke: OIDC discovery `200`, JWKS `200`, `/ui/console/` reachable, `/ui/v2/login` `200`.
8. Write docs and evidence.
9. Commit only G1 artifacts with `feat(zitadel): deploy login companion`.

## Skeleton
```yaml
services:
  zitadel-login:
    image: ghcr.io/zitadel/zitadel-login:v4.9.0
    environment:
      ZITADEL_API_URL: http://app-program-optical-matrix-xmjswe:8080
      NEXT_PUBLIC_BASE_PATH: /ui/v2/login
```

## Verify
`curl -sS -o /dev/null -w "%{http_code}" https://id.nuestrascuentitas.com/ui/v2/login` -> `200`

## Commit
`feat(zitadel): deploy login companion`
