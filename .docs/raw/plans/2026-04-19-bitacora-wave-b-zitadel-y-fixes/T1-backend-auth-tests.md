# Task T1: Backend Auth And Readiness Tests

## Shared Context
**Goal:** Add failing tests for Zitadel-only auth/readiness before production code changes.
**Stack:** .NET 10, xUnit-compatible `Bitacora.Tests`, ASP.NET Core JWT bearer.
**Architecture:** Backend must reject Supabase HS256 and accept only strict Zitadel RS256/JWKS.

## Locked Decisions
- Follow TDD: write tests first and verify they fail for the expected missing behavior.
- Do not implement production auth changes in this task.
- Do not print or commit JWT bodies.

## Task Metadata
```yaml
id: T1
depends_on: [T0]
agent_type: ps-dotnet10
files:
  - modify: src/Bitacora.Tests/Bitacora.Tests.csproj
  - create: src/Bitacora.Tests/Auth/ZitadelJwtValidationTests.cs
  - create: src/Bitacora.Tests/Auth/ReadinessProbeAuthConfigTests.cs
  - read: src/Bitacora.Api/Program.cs:158-207
  - read: src/Bitacora.Api/Health/ReadinessProbe.cs:1-86
complexity: medium
done_when: "dotnet test src/Bitacora.sln fails because Zitadel auth/readiness behavior is not implemented"
```

## Reference
`src/Bitacora.Api/Program.cs:158-207` currently validates Supabase HS256 and uses `SUPABASE_JWT_SECRET`.

## Prompt
Create minimal backend tests that express the target behavior. Tests must prove: missing Zitadel issuer/audience/JWKS config makes readiness not ready; readiness no longer depends on `SUPABASE_JWT_SECRET`; HS256 Supabase-style tokens are rejected; Zitadel RS256 token validation requires issuer, audience, lifetime, `kid`, and allowed algorithm. Use in-memory test helpers and generated ephemeral RSA keys. Do not connect to production services.

## Execution Procedure
1. Open `src/Bitacora.Tests/Bitacora.Tests.csproj`; add required test packages only if missing.
2. Add `ReadinessProbeAuthConfigTests` for config-level readiness behavior.
3. Add `ZitadelJwtValidationTests` around a testable auth validator/options helper name expected from T2: `ZitadelJwtOptionsFactory`.
4. Run `dotnet test src/Bitacora.sln`.
5. Confirm failures are due to missing `ZitadelJwtOptionsFactory` or old readiness checks, not syntax/package errors.
6. Stop after RED; do not implement production code.

## Skeleton
```csharp
public sealed class ReadinessProbeAuthConfigTests
{
    [Fact]
    public async Task CheckAsync_ReportsZitadelConfig_AndDoesNotRequireSupabaseSecret()
    {
        // arrange target config without SUPABASE_JWT_SECRET
        // act
        // assert readiness auth checks use zitadel_* keys
    }
}
```

## Verify
`dotnet test src/Bitacora.sln` -> fails for expected missing Zitadel implementation

## Commit
`test(auth): add failing zitadel auth readiness tests`
