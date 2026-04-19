# Bitacora Auth - Production Dokploy Spec

> Legacy post Wave B: Bitacora no longer uses this GoTrue app as active authentication runtime. Active IdP is Zitadel at `https://id.nuestrascuentitas.com`. Keep this file only as rollback reference until the Wave B acceptance window closes.

## Service info

- Runtime: GoTrue v2.177.0 (supabase/gotrue:v2.177.0)
- Public host: `auth.bitacora.nuestrascuentitas.com`
- Internal port: `9999`
- App ID (Dokploy): `O7PVCjNNqeL05HVjuRifl`
- App name (Swarm): `app-reboot-primary-pixel-xclgrf`
- Project: `bitacora` (project ID: `18WEM8BMIq-z_wgkrNlp8`)

## Status (legacy, verified 2026-04-15)

- `GET https://auth.bitacora.nuestrascuentitas.com/health` → `{"version":"v2.177.0","name":"GoTrue",...}` ✓
- `applicationStatus: "done"` ✓
- HTTPS via letsencrypt ✓

## Required environment variables

These variables apply only if rolling back to the pre-Zitadel Supabase Auth build.

```
PORT=9999
GOTRUE_API_HOST=0.0.0.0
API_EXTERNAL_URL=https://auth.bitacora.nuestrascuentitas.com
GOTRUE_SITE_URL=https://bitacora.nuestrascuentitas.com
GOTRUE_URI_ALLOW_LIST=https://bitacora.nuestrascuentitas.com/auth/callback
GOTRUE_CORS_ALLOWED_ORIGINS=https://bitacora.nuestrascuentitas.com
GOTRUE_JWT_SECRET=<from infra/.env GOTRUE_JWT_SECRET>
GOTRUE_JWT_ISSUER=supabase
GOTRUE_JWT_AUD=authenticated
GOTRUE_JWT_DEFAULT_GROUP_NAME=authenticated
GOTRUE_JWT_ADMIN_ROLES=service_role
GOTRUE_JWT_EXP=3600
GOTRUE_DB_DRIVER=postgres
GOTRUE_DB_DATABASE_URL=postgres://bitacora_auth:<password>@<db-host>:5432/bitacora_auth?sslmode=disable
GOTRUE_DB_NAMESPACE=auth
GOTRUE_DB_AFTER_CONNECT_QUERY=SET search_path TO auth,public
GOTRUE_DB_MIGRATIONS_PATH=/usr/local/etc/auth/migrations
GOTRUE_DISABLE_SIGNUP=false
GOTRUE_EXTERNAL_EMAIL_ENABLED=true
GOTRUE_MAILER_AUTOCONFIRM=false
GOTRUE_EXTERNAL_GOOGLE_ENABLED=true
GOTRUE_EXTERNAL_GOOGLE_CLIENT_ID=<from infra/.env GOOGLE_OAUTH_CLIENT_ID_BITACORA>
GOTRUE_EXTERNAL_GOOGLE_SECRET=<from infra/.env GOOGLE_OAUTH_CLIENT_SECRET_BITACORA>
GOTRUE_EXTERNAL_GOOGLE_REDIRECT_URI=https://auth.bitacora.nuestrascuentitas.com/callback
GOTRUE_SMTP_HOST=smtp.resend.com
GOTRUE_SMTP_PORT=587
GOTRUE_SMTP_USER=resend
GOTRUE_SMTP_PASS=<from Infisical>
GOTRUE_SMTP_ADMIN_EMAIL=noreply@bitacora.nuestrascuentitas.com
GOTRUE_SMTP_SENDER_NAME=Bitacora
```

## Anon key (NEXT_PUBLIC_SUPABASE_ANON_KEY)

The anon key for the frontend is `GOTRUE_ANON_KEY` from `infra/.env`. It must match
`GOTRUE_JWT_SECRET` as the signing key with role=anon.

## Blocking conditions

These conditions are legacy rollback-only:

- DNS for `auth.bitacora.nuestrascuentitas.com` must point to `54.37.157.93`
- `bitacora_auth` database and `auth` schema must exist with GoTrue migrations applied
- `GOTRUE_JWT_SECRET` must match `SUPABASE_JWT_SECRET` in `bitacora-api` (same value)

## Smoke test

```bash
curl -f https://auth.bitacora.nuestrascuentitas.com/health
# Expected: {"version":"v2.177.0","name":"GoTrue","description":"..."}
```
