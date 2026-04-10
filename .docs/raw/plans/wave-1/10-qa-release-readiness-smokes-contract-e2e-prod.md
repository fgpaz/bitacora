# Task T10: QA, Smokes, Contract Checks, E2E, and Prod-First Release Readiness

## Shared Context
**Goal:** Define the executable quality gate that must pass before opening real production traffic.
**Stack:** xUnit, Playwright, backend smokes, prod smoke runbooks, Telegram checks.
**Architecture:** There is no staging environment in this wave, so the release gate must be stronger and explicit. This task owns runnable verification, not just aspirational TP coverage.

## Task Metadata
```yaml
id: T10
depends_on:
  - T09
agent_type: ps-qa
files:
  - create: src/Bitacora.Tests/Api/AuthBootstrapTests.cs
  - create: src/Bitacora.Tests/Api/ConsentTests.cs
  - create: src/Bitacora.Tests/Api/RegistroTests.cs
  - create: src/Bitacora.Tests/Api/VinculosTests.cs
  - create: src/Bitacora.Tests/Api/VisualizacionTests.cs
  - create: src/Bitacora.Tests/Api/ExportTests.cs
  - create: src/Bitacora.Tests/Api/TelegramTests.cs
  - create: frontend/tests/e2e/patient-onboarding.spec.ts
  - create: frontend/tests/e2e/patient-registro.spec.ts
  - create: frontend/tests/e2e/professional-dashboard.spec.ts
  - create: frontend/tests/e2e/export.spec.ts
  - create: artifacts/release/production-smoke-checklist.md
  - modify: .docs/wiki/06_matriz_pruebas_RF.md
  - modify: .docs/wiki/06_pruebas/TP-ONB.md
  - modify: .docs/wiki/06_pruebas/TP-CON.md
  - modify: .docs/wiki/06_pruebas/TP-REG.md
  - modify: .docs/wiki/06_pruebas/TP-VIN.md
  - modify: .docs/wiki/06_pruebas/TP-VIS.md
  - modify: .docs/wiki/06_pruebas/TP-EXP.md
  - modify: .docs/wiki/06_pruebas/TP-TG.md
complexity: high
done_when: "The repo has executable backend tests, E2E coverage for the real MVP paths, and a prod-first go-live checklist that can block release."
```

## Reference
- `.docs/wiki/06_matriz_pruebas_RF.md` — current TP/RF matrix is ahead of runtime and must be brought down to executable truth
- `src/Bitacora.Tests/Bitacora.Tests.csproj` — current scaffold-only test project
- `.docs/wiki/06_pruebas/TP-*.md` — existing canonical test plans by module

## Prompt
Turn the current QA canon into a real release gate.

1. Replace scaffold-only `Bitacora.Tests` with runnable backend coverage for the implemented API surface.
2. Add test helpers/fixtures as needed, but keep test ownership aligned with real runtime modules.
3. Build frontend E2E only for flows that the repo actually implements in this wave:
   - patient onboarding/consent/registro
   - professional dashboard/timeline/export
4. Add Telegram coverage at the level that is realistic for this repo:
   - command/handler tests
   - webhook contract checks
   - smoke procedure for real production webhook once deployed
5. Update the TP matrix to distinguish:
   - executable current coverage
   - still-deferred cases, if any remain after Wave 1 execution
6. Create a prod-first smoke checklist that must pass before public launch:
   - API health and auth bootstrap
   - consent grant/revoke
   - patient registro
   - professional read access
   - export
   - Telegram pairing and reminder sanity
   - backup confirmation
   - observability confirmation
7. Make the release gate block go-live if any required smoke or executable test fails.

## Execution Waves
### Wave A — Backend executable tests
- Build real xUnit coverage for API/domain flows already implemented in this portfolio.

### Wave B — Frontend E2E
- Add Playwright flows for the real web paths and contract assumptions.

### Wave C — Prod-first release gate
- Create the production smoke checklist, release sign-off format, and pass/fail criteria.

## Skeleton
```csharp
[Fact]
public async Task AuthBootstrap_CreatesOrReusesUser_When_JwtIsValid()
{
    // Arrange / Act / Assert
}
```

## Test
```ts
test("patient completes onboarding and first mood entry", async ({ page }) => {
  // Auth, bootstrap, consent, mood entry, success confirmation.
});
```

**Test File:** `frontend/tests/e2e/patient-onboarding.spec.ts`

## Verify
`dotnet test src/Bitacora.Tests/Bitacora.Tests.csproj -c Release` and `npm run test:e2e --prefix frontend` -> required suites pass before go-live.

## Commit
`test(bitacora): add production release readiness coverage`
