# Bitacora qa-dev Full Smoke — Report

Date: 2026-04-20
Environment: production
Auth: Zitadel OIDC + PKCE
Telegram QA profile: `qa-dev`
Bot: `@mi_bitacora_personal_bot`

## Result

GREEN after fixes and deploy.

## Fixed During Smoke

1. Frontend Telegram adapter expected snake_case while backend returns ASP.NET Core camelCase.
2. Telegram UI pointed to `@bitacorav2_bot`; production QA uses `@mi_bitacora_personal_bot`.
3. Dashboard expected `entries[]`; backend returns `days[]`.
4. Dashboard rendered date-only values as UTC and displayed 2026-04-20 as 2026-04-19 in America/Buenos_Aires.
5. Telegram bot responses echoed clinical factor values back to chat; post-fix replies are confirmation-only.
6. `POST /api/v1/telegram/pairing` now declares `.RequireAuthorization()`.

## Validation

- `npm run typecheck`: pass
- `npm run lint`: pass
- `npm run build`: pass
- `dotnet test src\Bitacora.sln --configuration Release`: pass
- `pwsh -File .\infra\smoke\zitadel-cutover-smoke.ps1`: pass
- Browser E2E:
  - `/configuracion/telegram` shows linked state and canonical bot username.
  - `/dashboard` shows Telegram-created records.
  - Date-only records render as 20 Apr 2026.
- Telegram E2E:
  - `qa-dev` sends mood/factor flow.
  - Bot final reply after deploy: "Check-in actualizado. Ya podés verlo en tu historial web."

Evidence directory: `artifacts/e2e/2026-04-20-qa-dev-full-smoke/`
