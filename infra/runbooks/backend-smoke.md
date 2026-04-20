# Backend Smoke Gate

Executable gate: `infra/smoke/zitadel-cutover-smoke.ps1`.

## Current Zitadel Smoke

What it verifies:

1. Zitadel OIDC discovery.
2. Zitadel JWKS.
3. `GET /health`.
4. `GET /health/ready` with `zitadel_metadata`.
5. Frontend root.
6. `/ingresar` redirects to Zitadel authorize with PKCE markers.
7. `/api/auth/session` returns a null public session without exposing tokens.
8. `/api/backend/consent/current` returns `401` without a session.
9. `POST /api/v1/auth/bootstrap` returns `401` without bearer.
10. `/auth/logout` redirects to Zitadel end_session.

Run:

```powershell
pwsh -File .\infra\smoke\zitadel-cutover-smoke.ps1
```

Success criteria:

- every step returns the expected HTTP status;
- readiness returns `200`;
- OIDC redirect contains only expected public markers;
- the script exits `0`.

Authenticated clinical writes require a real Zitadel session. Do not generate or log JWTs in smoke evidence.

## Terminal Validation Note

Smoke is a precondition, not the only terminal activity. Final acceptance also requires authenticated E2E evidence saved under `artifacts/e2e/<YYYY-MM-DD>-<task-slug>/`.
