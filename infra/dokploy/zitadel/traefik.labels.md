# Zitadel Traefik Labels (live)

Dokploy configures Traefik automatically when you add a domain to an application. The labels are attached to the Docker Swarm service, not Docker container.

Live state as of 2026-04-19 for `teslita-shared-zitadel` (appId `zFdEECmPr1hhxwL0DKu4B`):

| Domain | Path | Port | HTTPS | Cert |
|--------|------|------|-------|------|
| `id.nuestrascuentitas.com` | `/` | `8080` | `true` | Let's Encrypt |

## How Dokploy builds Traefik labels

For each application domain Dokploy emits approximately:

```
traefik.enable=true
traefik.http.routers.<serviceName>.rule=Host(`<host>`)
traefik.http.routers.<serviceName>.entrypoints=websecure
traefik.http.routers.<serviceName>.tls.certresolver=letsencrypt
traefik.http.services.<serviceName>.loadbalancer.server.port=<port>
```

Zitadel requires `scheme=h2c` on the loadbalancer because the core uses gRPC-Web. Dokploy detects this from the service config.

## Path-based routing (Wave B requirement for login companion)

Zitadel v4 splits responsibilities between:
- Core (`/oauth/v2/*`, `/oidc/v1/*`, `/management/v1/*`, `/admin/v1/*`, `/ui/console/*`) -> port 8080
- Login companion (`/ui/v2/login/*`, `/ui/v2/device/*`) -> port 3000 (separate app)

When the login companion is deployed (Wave A2 or Wave B), add a second router with path prefix:

```
traefik.http.routers.zitadel-login.rule=Host(`id.nuestrascuentitas.com`) && PathPrefix(`/ui/v2`)
traefik.http.routers.zitadel-login.service=zitadel-login-svc
traefik.http.services.zitadel-login-svc.loadbalancer.server.port=3000
# Optional: strip prefix if the login app doesn't expect it
```

Or configure Dokploy with "Custom Internal Path" feature: set the companion's host path to `/ui/v2` and Dokploy emits the right rule.

## Known issue (as of 2026-04-19)

Without the login companion, navigation to `/ui/console/*` triggers an OIDC redirect to `/ui/v2/login/login?authRequest=...` which returns HTTP 404. This blocks browser login. OIDC discovery, JWKS, token endpoints still work normally via the core — machine clients with PAT are not affected.
