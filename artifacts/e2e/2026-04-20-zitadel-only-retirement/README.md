# Bitacora Zitadel-only retirement evidence

Date: 2026-04-20

## Scope

Final Supabase/GoTrue retirement pass for Bitacora production.

Goal:

- keep Zitadel as the only active auth provider;
- remove Supabase runtime rollback paths;
- prove the production login/session/backend proxy/logout path with a real browser;
- stop the legacy GoTrue Dokploy app without touching the clinical PostgreSQL database.

## Commits

- `dafaf1a` - retire Supabase runtime.
- `7514f80` - use public origin after Zitadel callback.
- `81bc5d2` - preserve Zitadel session cookie by clearing only transient OIDC cookies on successful callback.
- `327ec8e` - send product session cookie on frontend backend-proxy calls.
- `a55747f` - resolve email through Zitadel UserInfo when access token omits the email claim.
- `867da6c` - exclude `icon.svg` from auth middleware so metadata asset requests cannot clear `bitacora_session`.

## Local gates

- `dotnet test src\Bitacora.sln` passed: 2 tests.
- `dotnet build src\Bitacora.sln` passed after rerunning serially. A parallel build/test attempt hit a local file-lock race on `Shared.Contract.dll`.
- `npm run typecheck` passed.
- `npm run lint` passed with the existing two Next font warnings.
- `npm run build` passed.

Known warnings:

- Existing Scriban package vulnerability warnings remain in .NET restore/build output.
- Existing Next font warnings remain in `frontend/app/layout.tsx` and `frontend/app/(profesional)/layout.tsx`.
- Next 16 warns that the `middleware` file convention is deprecated in favor of `proxy`.

## Production deploy

- `bitacora-api` Dokploy deployment completed with status `done`.
- `bitacora-frontend` Dokploy deployment completed with status `done`.
- Active Dokploy env/build args check:
  - API Supabase/GoTrue keys: `0`.
  - Frontend Supabase/GoTrue keys: `0`.
  - API Zitadel env keys present.
  - Frontend Zitadel env/build keys present.

## Database

- Pre-cutover manual dump exists on VPS turismo:
  - `/tmp/bitacora-prod-pre-zitadel-only-20260420T003634Z.dump`
- Production manual migration applied:
  - `infra/migrations/zitadel/20260420_retire_supabase_auth.sql`
- Schema evidence:
  - `users.auth_subject` is the active identity column.
  - `users.supabase_user_id` and `users.legacy_auth_subject` are not part of the active schema.
- EF chain repair:
  - `20260420020000_RetireSupabaseAuthSubject` was added as an idempotent migration so fresh EF-created databases converge to the same schema.

## Public smoke

Command:

```powershell
pwsh -File .\infra\smoke\zitadel-cutover-smoke.ps1
```

Result: passed.

Checked:

- OIDC discovery `200`.
- JWKS `200`.
- API `/health` `200`.
- API `/health/ready` `200`.
- readiness includes `zitadel_metadata`.
- readiness omits `supabase_jwt_secret`.
- web root `200`.
- `/ingresar` redirects to Zitadel authorize with OIDC PKCE markers.
- unauthenticated `/api/auth/session` returns `{ user: null }`.
- backend proxy without session returns `401`.
- API bootstrap without bearer returns `401`.
- `/auth/logout` redirects to Zitadel end session.

## Authenticated E2E

Tooling: Playwright MCP against `https://bitacora.nuestrascuentitas.com`.

Sanitization:

- authorization codes, state, nonce, code verifier, JWTs, cookie values and direct personal identifiers were not copied into this evidence.
- cookie evidence records names and lengths only.

Verified browser path:

1. Navigated to `/ingresar`.
2. Redirected to Zitadel authorize.
3. Reused an already verified Zitadel human session through Login UI v2.
4. Returned to `/auth/callback`.
5. Callback cleared transient OIDC cookies and set `bitacora_session`.
6. `icon.svg` loaded with `200` and did not clear the app session.
7. `/api/auth/session` returned authenticated `patient`.
8. `POST /api/backend/auth/bootstrap` returned `200`.
9. `GET /api/backend/consent/current` returned `200`.
10. `/dashboard` loaded and kept `bitacora_session`.
11. `/auth/logout` cleared Bitacora cookies.
12. Opening `/dashboard` after logout redirected to `/`.

Key statuses:

| Check | Result |
|-------|--------|
| Callback session cookie | present |
| Session endpoint after login | `200`, authenticated |
| Local role mapping | `patient` |
| Bootstrap via frontend proxy | `200` |
| Consent current via frontend proxy | `200` |
| Dashboard URL | `/dashboard` |
| Dashboard title | `Mi historial | Bitácora` |
| Session after dashboard navigation | `200`, authenticated |
| Logout session endpoint | `200`, unauthenticated |
| Dashboard after logout | redirected to `/` |

## GoTrue retirement

- Legacy Dokploy app stopped:
  - application id: `O7PVCjNNqeL05HVjuRifl`
  - name: `bitacora-auth`
  - status after stop: `idle`
- The PostgreSQL resource was not deleted because it is the Bitacora clinical database.

## Traceability and audit

- `mi-lsp workspace status bitacora --format toon`: governance valid, projection in sync, index ready.
- `mi-lsp nav governance --workspace bitacora --format toon`: not blocked, `00` projection ready.
- Auth contract and technical canon reviewed:
  - `.docs/wiki/03_FL/FL-ONB-01.md`
  - `.docs/wiki/03_FL/FL-REG-01.md`
  - `.docs/wiki/07_baseline_tecnica.md`
  - `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
  - `.docs/wiki/07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md`
  - `.docs/wiki/08_modelo_fisico_datos.md`
  - `.docs/wiki/08_db/DB-MIGRACIONES-Y-BOOTSTRAP.md`
  - `.docs/wiki/09_contratos_tecnicos.md`
  - `.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md`
- `SUPABASE_JWT_SECRET` has no active workspace match.
- `supabase_user_id` matches are limited to historical EF migrations and explicit retirement migrations.

## Residuals

- Historical Supabase mentions remain only where they describe the retirement migration or historical evidence.
- Active runtime code, Dokploy API/frontend env, and canonical 03/07/08/09 auth docs now point to Zitadel-only.
