# Zitadel Login Companion — Teslita

Reference for the live `zitadel-login` companion deployed in Dokploy for `id.nuestrascuentitas.com`.

## Live State

| Field | Value |
|-------|-------|
| Dokploy project | `teslita-shared-idp` (`AKHsAJScexTwhJBzfFRlk`) |
| Application | `teslita-shared-zitadel-login` (`0qRNmuYflmhQZz9vFeC8f`) |
| Swarm service | `app-connect-haptic-interface-z8m5hi` |
| Image | `ghcr.io/zitadel/zitadel-login:v4.9.0` |
| Public path | `https://id.nuestrascuentitas.com/ui/v2/login` |
| Internal port | `3000` |

## Runtime Contract

The companion is a separate Docker app. Traefik routes only `PathPrefix('/ui/v2/login')` to this app and leaves all other `id.nuestrascuentitas.com` routes on Zitadel core.

Required environment shape:

```dotenv
ZITADEL_API_URL=http://app-program-optical-matrix-xmjswe:8080
NEXT_PUBLIC_BASE_PATH=/ui/v2/login
CUSTOM_REQUEST_HEADERS=Host:id.nuestrascuentitas.com
NODE_ENV=production
ZITADEL_SERVICE_USER_TOKEN=<from Infisical ZITADEL_LOGIN_CLIENT_PAT>
```

## Routing

The route is pass-through. Do not strip `/ui/v2/login`; the Next.js app is built with that base path.

Expected Traefik rule:

```yaml
rule: Host(`id.nuestrascuentitas.com`) && PathPrefix(`/ui/v2/login`)
service: app-connect-haptic-interface-z8m5hi-service
```

## Notes

The first deployment attempted explicit memory limits through Dokploy, but Dokploy passed the display value (`512`) to Swarm and deployment failed with `invalid memory value 512`. The app was redeployed with memory fields cleared and currently uses about 68 MiB.
