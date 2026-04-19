# Zitadel Teslita — Bootstrap Runbook

> Use this runbook to deploy Zitadel from scratch. Not needed for incremental updates (see `zitadel-recovery.md`).

## Pre-requisitos

- DNS `id.nuestrascuentitas.com` A-record → `54.37.157.93`, NOT proxied by Cloudflare (breaks LE HTTP-01)
- DOKPLOY_API_KEY in Infisical `teslita/bitacora/prod`
- Fresh `ZITADEL_MASTERKEY` (32 bytes base64): `openssl rand -base64 32`
- Resend API key + DKIM/SPF DNS records for `nuestrascuentitas.com`
- SSH access to VPS turismo (`ubuntu@54.37.157.93`)

## Pasos

1. **Dokploy project:** create `teslita-shared-idp` (or equivalent) in Dokploy. Projects are containers for apps.
2. **Postgres service:**
   - Type `PostgreSQL`, image `postgres:17`
   - DB name: `postgres` (bootstrap DB), Zitadel will create `shared_zitadel_v4` on first run via admin creds
   - User: `zitadel` with CREATEDB privilege
   - Volume: persistent, Dokploy auto-names (e.g. `postgres-*-data`)
   - Capture DB credentials to Infisical
3. **Zitadel Application:**
   - Source: Docker image `ghcr.io/zitadel/zitadel:v4.9.0`
   - Command: `/app/zitadel start-from-init --masterkeyFromEnv --tlsMode external`
   - Environment: import from `infra/dokploy/zitadel/env.example` with Infisical values; include the full `ZITADEL_FIRSTINSTANCE_*` block (org name, human admin, machine user for Admin SA, machine user for login client) + `ZITADEL_FIRSTINSTANCE_PATPATH=/dev/stdout`
   - Domain: `id.nuestrascuentitas.com`, port 8080, HTTPS, certType `letsencrypt`
   - Deploy
4. **Capture PATs:** immediately after deploy completes, SSH to VPS and:
   ```bash
   CONTAINER=$(docker ps --filter name=<zitadel-appname> --format '{{.ID}}')
   docker logs "$CONTAINER" 2>&1 | head -100 | grep -E '^[A-Za-z0-9_-]{40,}$'
   ```
   The two PATs appear as two 68-char base64-url-safe strings between the migration logs (one for admin-sa, one for login-client). Order: admin-sa first, login-client second. Verify by calling `/auth/v1/users/me` with each PAT.
5. **Persist in Infisical:**
   ```powershell
   mkey set bitacora ZITADEL_ADMIN_PAT "<first>" --env prod
   mkey set bitacora ZITADEL_LOGIN_CLIENT_PAT "<second>" --env prod
   ```
6. **Create remaining orgs + OAuth clients** via API (see `T2.1-organizations.md`, `T2.4-oauth-clients.md` in the plan subdocs).
7. **Wire SMTP** via POST `/admin/v1/email/smtp` + `POST /admin/v1/email/{id}/_activate` + test mail through `/admin/v1/smtp/{id}/_test` (see T2.2 subdoc).
8. **Apply baseline policies** via PUT `/admin/v1/policies/password/complexity` (min 10 + upper + lower + number).
9. **Install backup cron** on VPS turismo from `infra/backups/zitadel/README.md`.
10. **Deploy login companion app** in Dokploy (`ghcr.io/zitadel/zitadel-login:v4.9.0`) with env `ZITADEL_API_URL=http://app-program-optical-matrix-xmjswe:8080`, `NEXT_PUBLIC_BASE_PATH=/ui/v2/login`, `CUSTOM_REQUEST_HEADERS=Host:id.nuestrascuentitas.com`, `ZITADEL_SERVICE_USER_TOKEN=<login-client-PAT>`. Add Traefik rule to route `/ui/v2/login` pass-through to port 3000 of the companion.
11. **Smoke test:**
    - `curl https://id.nuestrascuentitas.com/.well-known/openid-configuration` → 200
    - `curl -H "Authorization: Bearer $ZITADEL_ADMIN_PAT" https://id.nuestrascuentitas.com/management/v1/orgs/me` → 200 with `name: nuestrascuentitas`
    - Login via browser at `/ui/console` -> redirects to `/ui/v2/login` -> works
    - `client_credentials` smoke uses machine users with user secrets, not the placeholder API apps.

## Rollback

Si bootstrap falla a mitad:
- `dkp.sh POST application.stop <appId>` + `postgres.stop` similar
- Drop DB `DROP DATABASE shared_zitadel_v4;` on the Postgres
- Fix env vars
- Redeploy

Si el masterkey se corrompe o se pierde: destructive — redeploy requires dropping all Zitadel DBs.

## Escalación

- Rate-limit Let's Encrypt (>5 failed issuances/week): switch to LE staging (`--caserver https://acme-staging-v02.api.letsencrypt.org/directory`), then back to prod.
- DKIM/SPF fail test mail: not a blocker, but notification emails (verification, reset) won't deliver. Fix DNS before running user-facing flows.
- Container OOM: bump memory limit via Dokploy; Zitadel needs ~512MB baseline.

## Referencias

- Design doc: `.docs/raw/plans/2026-04-17-idp-zitadel-multi-ecosistema-design.md`
- Contract: `.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md`
- Reports: `.docs/raw/reports/2026-04-18-wave-a-audit/`
