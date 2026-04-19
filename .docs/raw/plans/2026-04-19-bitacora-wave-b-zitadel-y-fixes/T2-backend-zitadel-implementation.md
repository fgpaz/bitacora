# Task T2: Backend Zitadel Auth Implementation

## Shared Context
**Goal:** Replace Supabase backend auth with Zitadel-only RS256/JWKS validation.
**Stack:** .NET 10 minimal API, EF Core, PostgreSQL.
**Architecture:** Browser/frontend obtains Zitadel access token; API validates bearer token only.

## Locked Decisions
- No dual-auth runtime.
- Supabase secrets and HS256 tokens must not authenticate requests.
- Local role remains authoritative after bootstrap/provisioning.

## Task Metadata
```yaml
id: T2
depends_on: [T1]
agent_type: ps-dotnet10
files:
  - modify: src/Bitacora.Api/Program.cs:158-207
  - modify: src/Bitacora.Api/Health/ReadinessProbe.cs:1-86
  - modify: src/Bitacora.Api/Extensions/ClaimsPrincipalExtensions.cs:1-21
  - modify: src/Bitacora.Api/Security/CurrentAuthenticatedPatientResolver.cs
  - modify: src/Bitacora.Api/Security/CurrentAuthenticatedProfessionalResolver.cs
  - modify: src/Bitacora.Domain/Entities/User.cs:5-72
  - modify: src/Bitacora.DataAccess.Interface/Repositories/IUserRepository.cs:5-10
  - modify: src/Bitacora.DataAccess.EntityFramework/Repositories/UserRepository.cs:1-29
  - create: src/Bitacora.Api/Auth/ZitadelJwtOptionsFactory.cs
  - create: src/Bitacora.DataAccess.EntityFramework/Persistence/Migrations/<timestamp>_RenameSupabaseUserIdToAuthSubject.cs
complexity: high
done_when: "dotnet test src/Bitacora.sln passes"
```

## Reference
`CT-AUTH-ZITADEL.md:38-49` defines required claims; handoff lines 160-169 define mandatory JWT validation.

## Prompt
Implement exactly enough production code to pass T1 tests and compile the API. Configure JWT bearer for Zitadel only using `Authority=https://id.nuestrascuentitas.com`, metadata/JWKS, strict issuer, strict audience, RS256-only token validation, and `MapInboundClaims=false`. Rename code-facing identity concepts from Supabase to provider-neutral names. Add an EF migration that renames `supabase_user_id` to `auth_subject`; do not drop data. Bootstrap must create local users from Zitadel `sub`, `email`, and role claim mapping.

## Execution Procedure
1. Run `mi-lsp nav search "AddJwtBearer" --include-content --workspace bitacora --format toon`.
2. Create `ZitadelJwtOptionsFactory` with config keys `ZITADEL_ISSUER`, `ZITADEL_API_AUDIENCE`, `ZITADEL_PROJECT_ID`, and optional `ZITADEL_REQUIRE_HTTPS_METADATA`.
3. Replace Supabase JWT secret validation in `Program.cs` with Zitadel-only bearer options.
4. Update revocation logic so a revoked user token without valid `iat` fails closed.
5. Rename extension method `GetSupabaseUserId` to `GetAuthSubject`.
6. Rename repository method `GetBySupabaseUserIdAsync` to `GetByAuthSubjectAsync`.
7. Rename domain property `SupabaseUserId` to `AuthSubject`; keep migration data-preserving.
8. Update readiness checks to require Zitadel issuer/audience/project config and not require `SUPABASE_JWT_SECRET`.
9. Run `dotnet test src/Bitacora.sln`; fix until green.

## Skeleton
```csharp
public static class ZitadelJwtOptionsFactory
{
    public static void Configure(JwtBearerOptions options, IConfiguration configuration)
    {
        options.MapInboundClaims = false;
        options.Authority = configuration["ZITADEL_ISSUER"];
        options.TokenValidationParameters.ValidAudience =
            configuration["ZITADEL_API_AUDIENCE"] ?? configuration["ZITADEL_PROJECT_ID"];
        options.TokenValidationParameters.ValidAlgorithms = [SecurityAlgorithms.RsaSha256];
    }
}
```

## Verify
`dotnet test src/Bitacora.sln` -> `Passed`

## Commit
`feat(auth): validate zitadel jwt bearer tokens`
