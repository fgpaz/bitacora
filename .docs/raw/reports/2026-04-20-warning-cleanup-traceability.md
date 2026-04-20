# Warning cleanup traceability and audit

Date: 2026-04-20

## Scope

Resolved the existing warning set left after the Zitadel-only closure:

- NuGet vulnerability warnings for transitive `Scriban 6.2.0`.
- Next.js lint warnings for manual Google Font links.
- Next.js 16 build warning for the deprecated `middleware.ts` convention.
- Local npm install engine warning under Node 24 while production stays on Node 22.

No auth contract, clinical data model, API envelope, or protected route behavior was intentionally changed.

## Changes

- Upgraded `Mediator.SourceGenerator` and `Mediator.Abstractions` to `3.0.2`.
- Migrated frontend request interception from `frontend/middleware.ts` to `frontend/proxy.ts`.
- Replaced manual Google Fonts `<link>` tags with self-hosted `@fontsource` packages.
- Removed duplicate `<html>`, `<head>`, `<body>`, and provider nesting from the professional route-group layout.
- Changed frontend engine range to `>=22 <25` while preserving Docker `node:22-slim`.
- Synced warning debt docs for `WEB-VAL-003` and `07_baseline_tecnica.md`.

## Validation

Commands run successfully:

```powershell
dotnet list src\Bitacora.sln package --vulnerable --include-transitive
dotnet build src\Bitacora.sln
dotnet test src\Bitacora.sln
npm install --package-lock-only
npm run lint -- --max-warnings=0
npm run typecheck
npm run build
npm audit --audit-level=low
```

Results:

- NuGet vulnerable packages: none.
- .NET build: `0 Advertencia(s)`, `0 Errores`.
- .NET tests: 2 passed.
- ESLint: 0 warnings under `--max-warnings=0`.
- Next build: passed without middleware-to-proxy or font warnings.
- npm audit: 0 vulnerabilities.
- npm install: no `EBADENGINE` warning with current local Node 24.

## Traceability

- Governance: `mi-lsp workspace status bitacora --format toon --full` reports governance valid, projection ready, index current.
- Governance audit: `mi-lsp nav governance --workspace bitacora --format toon` reports not blocked and in sync.
- Technical docs reviewed and updated where needed:
  - `.docs/wiki/07_baseline_tecnica.md`
  - `.docs/wiki/21_matriz_validacion_ux.md`
  - `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-REG-002.md`
  - `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-INDEX.md`
  - `infra/dokploy/bitacora-frontend.production.md`
- Technical docs reviewed with no update required:
  - `.docs/wiki/09_contratos_tecnicos.md`
  - `.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md`
  - `infra/runbooks/backend-smoke.md`
  - `infra/dokploy/production-checklist.md`

Residual search checks found no active matches for:

- `middleware.ts`
- `middleware migr`
- `Next.js middleware convention deprecated`
- `fonts.googleapis`
- `fonts.gstatic`
- `no-page-custom-font`
- `Scriban`
- `NU1902`, `NU1903`, `NU1904`

## Audit Verdict

Approved.

There are no blocking traceability findings for this warning cleanup. Board sync was not required because no GitHub issue/card was opened or closed by this task; the repo board remains active for unrelated backlog items.
