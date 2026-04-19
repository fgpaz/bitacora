# Wave A Smoke Test — Zitadel Teslita

**Fecha:** 2026-04-19
**Operador:** Claude Code en nombre de `paz.fgabriel@gmail.com`
**Issue tracking:** fgpaz/bitacora#16
**Runtime:** Zitadel v4.9.0 en `id.nuestrascuentitas.com` (VPS turismo `54.37.157.93`)

## Resumen

| Check | Status | Evidencia |
|-------|--------|-----------|
| OIDC Discovery accesible | GREEN | `01-oidc-discovery.json` |
| JWKS RS256 publicadas | GREEN | `02-jwks.json` |
| Admin SA PAT válido + `GET /management/v1/orgs/me` | GREEN | `03-orgs-me.json` |
| 4 Organizations creadas | GREEN | T2.1 report |
| 8 OAuth Clients creados (4 web PKCE + 4 API) | GREEN | T2.4 report |
| SMTP Resend wireado + activo | GREEN (DKIM/SPF pending user verify) | T2.2 |
| Password complexity policy aplicada | GREEN | T2.5 |
| Client credentials grant bitacora-api | AMBER | `04-client-credentials-raw.json` — "client not found" (project grant authorization pending) |
| Login UI v2 accesible | RED | requires `zitadel-login` companion (not deployed) |
| Backup cron activo | RED | script git-tracked, VPS install pending |
| MFA admin enrollment | RED | requires login UI |
| Secrets en Infisical | GREEN | 50+ keys ZITADEL_* persistidas |

## Detalle

### 1. OIDC Discovery

```bash
$ curl -sS https://id.nuestrascuentitas.com/.well-known/openid-configuration | jq .issuer,.jwks_uri,.token_endpoint,.id_token_signing_alg_values_supported
"https://id.nuestrascuentitas.com"
"https://id.nuestrascuentitas.com/oauth/v2/keys"
"https://id.nuestrascuentitas.com/oauth/v2/token"
["EdDSA","RS256","RS384","RS512","ES256","ES384","ES512"]
```

### 2. JWKS

```bash
$ curl -sS https://id.nuestrascuentitas.com/oauth/v2/keys | jq '.keys[0] | {kid, kty, use, alg}'
{"kid":"<redacted>","kty":"RSA","use":"sig","alg":"RS256"}
```

### 3. Admin SA PAT

```bash
$ curl -sS -H "Authorization: Bearer $ZITADEL_ADMIN_PAT" https://id.nuestrascuentitas.com/management/v1/orgs/me
{"org":{"id":"369202905795854607","name":"nuestrascuentitas","primaryDomain":"nuestrascuentitas.id.nuestrascuentitas.com","state":"ORG_STATE_ACTIVE",...}}
```

### 4. Client credentials grant (AMBER)

El grant inicial devolvió `{"error":"invalid_client","error_description":"client not found"}`. Causa probable: el API client no está "authorized" en el project (Zitadel v4 requiere project grants para permitir client_credentials). Tech debt para Wave B que puede completarlo al momento de integrar el backend Bitacora.

### 5. SMTP Resend

Configurado vía `POST /admin/v1/smtp` + `POST /admin/v1/smtp/<id>/_activate`. Test mail enviado vía `POST /admin/v1/smtp/<id>/_test` a `paz.fgabriel@gmail.com`. El user debe verificar headers DKIM/SPF del mail recibido para cerrar este check con GREEN final.

### 6. Organizations + OAuth Clients

Desde `T2.1-orgs-setup.md` + `T2.4-oauth-clients.md`:
- `nuestrascuentitas` (default bootstrap), `bitacora`, `multi-tedi`, `gastos` — 4/4 orgs activas
- Cada org: 1 project + 1 web PKCE app + 1 API app = 4 × 3 = 12 apps (secrets API en Infisical)

### 7. Pending (gaps documentados)

- **Login companion app** — Wave A2 / Wave B dependency
- **Backup cron VPS install** — user action, script ready
- **MFA enrollment human admin** — pending login UI
- **Legacy Postgres cleanup** — `postgres-bypass-wireless-bus-tupzoj`, DB `shared_zitadel` sin uso
- **Client credentials grant** — authorize project grant for API clients
- **Offsite backup remote** — user picks provider (B2/R2/Hetzner)

## Secrets persistidos en Infisical (nombres solamente)

ZITADEL_MASTERKEY, ZITADEL_EXTERNAL*, ZITADEL_DATABASE_POSTGRES_*, ZITADEL_ADMIN_PAT,
ZITADEL_LOGIN_CLIENT_PAT, ZITADEL_ADMIN_EMAIL, ZITADEL_ADMIN_INITIAL_PASSWORD,
ZITADEL_SMTP_CONFIG_ID, ZITADEL_SMTP_SENDER_EMAIL, ZITADEL_SMTP_PROVIDER,
ZITADEL_ORG_{NUESTRASCUENTITAS,BITACORA,MULTI_TEDI,GASTOS}_ID,
ZITADEL_PROJECT_{NUESTRASCUENTITAS,BITACORA,MULTI_TEDI,GASTOS}_ID,
ZITADEL_CLIENT_{NUESTRASCUENTITAS,BITACORA,MULTI_TEDI,GASTOS}_WEB_ID,
ZITADEL_CLIENT_{NUESTRASCUENTITAS,BITACORA,MULTI_TEDI,GASTOS}_API_ID,
ZITADEL_CLIENT_{NUESTRASCUENTITAS,BITACORA,MULTI_TEDI,GASTOS}_API_SECRET,
ZITADEL_DOKPLOY_PROJECT_ID, ZITADEL_DOKPLOY_APP_ID_WEB, ZITADEL_DOKPLOY_PG_ID_{ACTIVE,LEGACY}
DOKPLOY_API_KEY, DOKPLOY_URL, DOKPLOY_EMAIL, DOKPLOY_PASSWORD

## Firma

Wave A completada en modo PARTIAL 2026-04-19 por Claude Code con usuario Gabriel Paz. Wave B (#17) puede arrancar con los artefactos disponibles pero debe primero deployar el `zitadel-login` companion para habilitar login end-user de Bitacora.
