# Task T2: Visualization + Export Read Layer

## Shared Context
**Goal:** Implement professional and patient read models plus export-capable query endpoints.  
**Stack:** .NET 10, EF Core, Minimal APIs, Shared.Contract, `mi-lsp`.  
**Architecture:** Read-side work should reuse the existing repository/query stack and the vínculo core delivered in Phase 30.

## Task Metadata
```yaml
id: T2
depends_on: [T1]
agent_type: ps-dotnet10
files:
  - create: src/Bitacora.Application/Queries/Visualizacion/*.cs
  - create: src/Bitacora.Application/Queries/Export/*.cs
  - create: src/Shared.Contract/Visualizacion/*.cs
  - create: src/Shared.Contract/Export/*.cs
  - create: src/Bitacora.Api/Endpoints/Visualizacion/VisualizacionEndpoints.cs
  - create: src/Bitacora.Api/Endpoints/Export/ExportEndpoints.cs
  - modify: src/Bitacora.Api/Program.cs
  - modify: src/Bitacora.Application/DependencyInjection/ServiceCollectionExtensions.cs
complexity: high
done_when: "dotnet build src/Bitacora.sln exits 0 and visualization/export routes are wired into the API"
```

## Reference
`src/Bitacora.Application/Queries/Consent/GetCurrentConsentQuery.cs` — query pattern.  
`src/Bitacora.Api/Endpoints/Registro/RegistroEndpoints.cs` — endpoint module shape.

## Prompt
Implement the read side for visualization and export. Use `mi-lsp nav search "GetCurrentConsentQuery" --include-content --workspace humor --format toon`, `mi-lsp nav search "MoodEntry" --include-content --workspace humor --format toon`, `mi-lsp nav search "DailyCheckin" --include-content --workspace humor --format toon`, and `mi-lsp nav search "CareLink" --workspace humor --format toon` to anchor the design in the current source. Create dedicated query handlers and shared contracts for summary/timeline/export use cases, then expose them through new `VisualizacionEndpoints` and `ExportEndpoints` modules registered in `Program.cs`. Keep exports privacy-aware and auditable; do not add Telegram runtime logic in this task.

## Skeleton
```csharp
public sealed record GetPatientTimelineQuery(Guid PatientId);
public sealed record ExportPatientSummaryQuery(Guid PatientId, DateOnly From, DateOnly To);
```

## Verify
`dotnet build src/Bitacora.sln --no-restore` -> `Build succeeded`

## Commit
`feat(api): add visualization and export read layer`
