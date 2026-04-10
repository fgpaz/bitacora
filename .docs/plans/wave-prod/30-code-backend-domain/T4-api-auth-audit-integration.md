# Task T4: API / Auth / Audit Integration

## Shared Context
**Goal:** Expose the vínculo domain through the existing API pipeline with correct authorization, consent, and audit behavior.  
**Stack:** .NET 10, Minimal APIs, `mi-lsp`.  
**Architecture:** New routes must fit the current `Map*Endpoints` pattern and preserve fail-closed authz and audit behavior.

## Task Metadata
```yaml
id: T4
depends_on: [T3]
agent_type: ps-dotnet10
files:
  - create: src/Bitacora.Api/Endpoints/Vinculos/VinculosEndpoints.cs
  - modify: src/Bitacora.Api/Program.cs
  - modify: src/Bitacora.Api/Endpoints/Extensions/MinimalApiExtensions.cs
  - modify: src/Bitacora.Api/Security/CurrentAuthenticatedPatientResolver.cs
  - modify: src/Bitacora.Api/Middleware/ConsentRequiredMiddleware.cs
  - modify: src/Bitacora.Application/Commands/Vinculos/*.cs
complexity: high
done_when: "dotnet build src/Bitacora.sln exits 0 and vínculo routes are wired into Program.cs with authz and audit coverage"
```

## Reference
`src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs` — endpoint structure to follow.  
`src/Bitacora.Api/Program.cs` — current pipeline and route registration order.  
`src/Bitacora.Domain/Entities/AccessAudit.cs` — audit entity already available.

## Prompt
Wire the new backend domain into the API. Use `mi-lsp nav search "MapConsentEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "CurrentAuthenticatedPatientResolver" --include-content --workspace humor --format toon`, `mi-lsp nav search "ConsentRequiredMiddleware" --include-content --workspace humor --format toon`, and `mi-lsp nav search "AccessAudit" --include-content --workspace humor --format toon` before editing. Add a new `VinculosEndpoints` module, register it in `Program.cs`, reuse current auth helpers where possible, and ensure operations that reveal or change vínculo state are audited. Keep fail-closed behavior: if identity, consent, or ownership is unclear, reject the request instead of guessing.

## Skeleton
```csharp
public static class VinculosEndpoints
{
    public static void MapVinculosEndpoints(this IEndpointRouteBuilder app)
    {
    }
}
```

## Verify
`dotnet build src/Bitacora.sln --no-restore` -> `Build succeeded`

## Commit
`feat(api): expose vínculo endpoints with auth and audit`
