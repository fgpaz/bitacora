# G1 — Zitadel Login Companion

**Date:** 2026-04-19  
**Status:** GREEN  
**Tracking:** fgpaz/bitacora#18

## Live Deployment

| Field | Value |
|-------|-------|
| Dokploy applicationId | `0qRNmuYflmhQZz9vFeC8f` |
| App name | `teslita-shared-zitadel-login` |
| Swarm service | `app-connect-haptic-interface-z8m5hi` |
| Image | `ghcr.io/zitadel/zitadel-login:v4.9.0` |
| Route | `https://id.nuestrascuentitas.com/ui/v2/login` |
| Route mode | pass-through, `stripPath=false` |

## Smoke

| Check | Result |
|-------|--------|
| OIDC discovery | `200` |
| JWKS | `200` |
| Console | `200` |
| Login UI v2 | `200` |
| Real authorize redirect | `200` at `/ui/v2/login/loginname?requestId=oidc_V2_369307575725850982` |
| Login page title | `Welcome back!` |
| Swarm replicas | `1/1` |
| Observed memory | `68.24MiB` |

## Notes

- The first deploy failed because Dokploy passed display memory value `512` to Swarm, which rejected it as below 4 MiB.
- Memory fields were cleared and the app runs successfully. This is recorded as an operational note; the current container is lightweight.
- Non-secret app metadata was stored in Infisical:
  - `ZITADEL_DOKPLOY_APP_ID_LOGIN`
  - `ZITADEL_DOKPLOY_APP_NAME_LOGIN`
  - `ZITADEL_LOGIN_BASE_PATH`
- After the Postgres mount incident in G2, the `login-client` PAT was refreshed in Infisical and the companion app was redeployed with the current token.

## Secret Handling

`ZITADEL_SERVICE_USER_TOKEN` comes from `ZITADEL_LOGIN_CLIENT_PAT`. The token value was not recorded in this report.
