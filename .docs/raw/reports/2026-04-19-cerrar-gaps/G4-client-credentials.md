# G4 — Client Credentials Grant

**Date:** 2026-04-19  
**Status:** GREEN  
**Tracking:** fgpaz/bitacora#18

## Result

The original API application client IDs created in Wave A did not work for `client_credentials`. In Zitadel v4.9, the working M2M path is a machine user with a generated user secret.

Each ecosystem org now has:

- A Web PKCE application for browser login.
- A machine service account for backend/job `client_credentials`.
- A generated secret persisted only in Infisical as `ZITADEL_CLIENT_<ORG>_API_SECRET`.

## Live Clients

| Org | projectId | web clientId | M2M clientId | M2M userId |
|-----|-----------|--------------|--------------|------------|
| nuestrascuentitas | `369306314246979942` | `369306318709784934` | `nuestrascuentitas-api-client` | `369306707119047014` |
| bitacora | `369306332534145382` | `369306336963330406` | `bitacora-api-client` | `369306719433523558` |
| multi-tedi | `369306350636761446` | `369306355065946470` | `multi-tedi-api-client` | `369306729382412646` |
| gastos | `369306369645347174` | `369306374074597734` | `gastos-api-client` | `369306738643435878` |

## Smoke

`POST https://id.nuestrascuentitas.com/oauth/v2/token` with HTTP Basic auth, `grant_type=client_credentials`, and project audience scope returned:

| Org | HTTP | token_type | JWT alg | `kid` | issuer |
|-----|------|------------|---------|-------|--------|
| nuestrascuentitas | `200` | `Bearer` | `RS256` | present | `https://id.nuestrascuentitas.com` |
| bitacora | `200` | `Bearer` | `RS256` | present | `https://id.nuestrascuentitas.com` |
| multi-tedi | `200` | `Bearer` | `RS256` | present | `https://id.nuestrascuentitas.com` |
| gastos | `200` | `Bearer` | `RS256` | present | `https://id.nuestrascuentitas.com` |

## Secret Handling

Only non-secret IDs are recorded. Client secrets remain in Infisical under `ZITADEL_CLIENT_<ORG>_API_SECRET`.
