# Bitacora Wave B Zitadel Cutover Evidence

Date: 2026-04-19
Issue: `fgpaz/bitacora#17`
Commit deployed: `b0d876c feat(auth): cut over bitacora to zitadel`

## Scope

This evidence records the production cutover from Supabase Auth to the shared Zitadel IdP at `https://id.nuestrascuentitas.com`.

Wave B result:

- Frontend auth: OIDC Authorization Code + PKCE.
- Backend auth: Bearer JWT validation through Zitadel metadata and JWKS, RS256 only.
- Session cookie: `bitacora_session` httpOnly.
- Browser API calls: proxied through `/api/backend/*`; access tokens are not exposed to browser JavaScript.
- Supabase Auth: retained only as rollback secret/source until operational acceptance.

## Deploy

Dokploy apps deployed from `main`:

- API: `UROM_r5ETX0rvs-1WZ3bi` (`bitacora-api`, service `app-copy-redundant-sensor-tv43jn`) -> deployment status `done`.
- Frontend: `BRTMuvBfWtslXHnShtrnB` (`bitacora-frontend`, service `app-quantify-digital-bandwidth-o93jgd`) -> deployment status `done`.

## Production Database Migration

Migration applied before opening the new runtime:

- `20260419000001_AddLegacyAuthSubject`
- Added nullable `users.legacy_auth_subject`.
- Copied existing auth subjects to `legacy_auth_subject`.
- Created index `IX_users_legacy_auth_subject`.
- Inserted EF migration history row for `20260419000001_AddLegacyAuthSubject`.

Production data facts at migration time:

- `users`: 14
- `mood_entries`: 12
- `daily_checkins`: 10
- `care_links`: 0

No PII, JWTs, passwords, client secrets, or clinical payloads were printed into this evidence.

## Local Gates

Commands passed before deploy:

- `dotnet build .\src\Bitacora.sln`
- `dotnet test .\src\Bitacora.sln`
- `npm run lint`
- `npm run typecheck`
- `npm run build`

Known non-blocking warnings:

- `Scriban 6.2.0` vulnerability warnings from existing .NET dependency graph.
- Next.js font warnings in layout files.
- Next.js 16 warns that `middleware.ts` convention is deprecated in favor of proxy naming; this is backlog polish, not a cutover blocker.

## Zitadel / OIDC Smoke

Repo smoke command:

- `pwsh -File .\infra\smoke\zitadel-cutover-smoke.ps1` -> passed

Public IdP checks:

- `GET https://id.nuestrascuentitas.com/.well-known/openid-configuration` -> `200`
- `GET https://id.nuestrascuentitas.com/oauth/v2/keys` -> `200`

Bitacora checks:

- `GET https://api.bitacora.nuestrascuentitas.com/health` -> `200`
- `GET https://api.bitacora.nuestrascuentitas.com/health/ready` -> `200`
- Readiness checks returned:
  - `connection_string: ok`
  - `zitadel_authority: ok`
  - `zitadel_audience: ok`
  - `zitadel_metadata: ok`
  - `encryption_key: ok`
  - `pseudonym_salt: ok`
  - `database: ok`
- Readiness no longer reports `supabase_jwt_secret`.
- `GET https://bitacora.nuestrascuentitas.com` -> `200`
- `HEAD https://bitacora.nuestrascuentitas.com/ingresar` -> `307` to Zitadel `/oauth/v2/authorize` with public markers:
  - `client_id=369306336963330406`
  - `redirect_uri=https://bitacora.nuestrascuentitas.com/auth/callback`
  - `scope=openid profile email`
  - `code_challenge_method=S256`
- `GET https://bitacora.nuestrascuentitas.com/api/auth/session` -> `200`, public null session only.
- `GET https://bitacora.nuestrascuentitas.com/api/backend/consent/current` without session -> `401`
- `POST https://api.bitacora.nuestrascuentitas.com/api/v1/auth/bootstrap` without bearer -> `401`
- `HEAD https://bitacora.nuestrascuentitas.com/auth/logout` -> `307` to Zitadel `/oidc/v1/end_session`

Interactive login with a human/test Zitadel user was not executed in this batch to avoid handling human credentials in automation. The OIDC entrypoint, public session shape, server-side proxy, backend JWKS readiness, and fail-closed unauthenticated surfaces were verified.

## Redirect URI Verification

Zitadel Management API confirmed the Bitacora web app:

- Project: `369306332534145382`
- App name: `bitacora-web`
- Client ID: `369306336963330406`
- Redirect URIs include:
  - `https://bitacora.nuestrascuentitas.com/auth/callback`
  - `http://localhost:3000/auth/callback`
- Post logout redirect includes:
  - `https://bitacora.nuestrascuentitas.com/`
- Grant types include authorization code and refresh token.
- Response type includes code.

## Rollback Notes

Rollback remains available until acceptance:

- Re-deploy the previous Supabase-auth build.
- Keep `SUPABASE_JWT_SECRET` in Dokploy during the rollback window.
- The production migration preserved `legacy_auth_subject`.
- The physical `users.supabase_user_id` column is intentionally retained during the rollback window.

Do not remove Supabase Auth assets or rollback secrets until the Wave B acceptance window closes.
