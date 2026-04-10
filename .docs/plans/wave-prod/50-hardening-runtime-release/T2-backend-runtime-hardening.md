# Task T2: Backend Runtime Hardening

## Shared Context
**Goal:** Harden backend runtime behavior for security, audit, and fail-closed operation.  
**Stack:** .NET 10, Minimal APIs, audit layer, `mi-lsp`.  
**Architecture:** Apply the read-only audit findings to the backend pipeline and sensitive endpoints without adding new product scope.

## Task Metadata
```yaml
id: T2
depends_on: [T1]
agent_type: ps-dotnet10
files:
  - modify: src/Bitacora.Api/Program.cs
  - modify: src/Bitacora.Api/Middleware/*.cs
  - modify: src/Bitacora.Api/Security/*.cs
  - modify: src/Bitacora.Application/**/*.cs
  - modify: src/Bitacora.DataAccess.EntityFramework/Repositories/AccessAuditRepository.cs
complexity: high
done_when: "dotnet build src/Bitacora.sln exits 0 and backend runtime hardening findings are addressed"
```

## Reference
`src/Bitacora.Api/Program.cs` — pipeline ordering and middleware.  
`src/Bitacora.Api/Middleware/ConsentRequiredMiddleware.cs` — current consent fail-closed seam.  
`src/Bitacora.DataAccess.EntityFramework/Repositories/AccessAuditRepository.cs` — audit persistence seam.

## Prompt
Take the approved findings from T1 and implement backend runtime hardening only. Re-run `mi-lsp nav search "Program.cs" --include-content --workspace humor --format toon`, `mi-lsp nav search "ConsentRequiredMiddleware" --include-content --workspace humor --format toon`, `mi-lsp nav search "AccessAuditRepository" --include-content --workspace humor --format toon`, and any other targeted searches needed. Fix backend fail-closed behavior, tighten authz and audit where needed, and ensure new endpoints and Telegram runtime paths obey the frozen privacy and operational rules. Do not introduce new feature scope.

## Skeleton
```csharp
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ConsentRequiredMiddleware>();
```

## Verify
`dotnet build src/Bitacora.sln --no-restore` -> `Build succeeded`

## Commit
`fix(runtime): harden backend security and audit paths`
