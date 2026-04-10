# Task T3: Application Commands / Queries / Contracts

## Shared Context
**Goal:** Add the application and shared contract surface for vínculo operations and consent cascades.  
**Stack:** .NET 10, Shared.Contract, `mi-lsp`.  
**Architecture:** Reuse the existing command/query classes and keep cross-service DTOs in `Shared.Contract`.

## Task Metadata
```yaml
id: T3
depends_on: [T2]
agent_type: ps-dotnet10
files:
  - create: src/Bitacora.Application/Commands/Vinculos/*.cs
  - create: src/Bitacora.Application/Queries/Vinculos/*.cs
  - modify: src/Bitacora.Application/DependencyInjection/ServiceCollectionExtensions.cs
  - create: src/Shared.Contract/Vinculos/*.cs
  - create: src/Shared.Contract/Events/*.cs
  - modify: src/Bitacora.Application/Common/ConsentConfiguration.cs
complexity: high
done_when: "dotnet build src/Bitacora.sln exits 0 and vínculo application handlers plus shared contracts exist"
```

## Reference
`src/Bitacora.Application/Commands/Auth/BootstrapPatientCommand.cs` — command pattern.  
`src/Bitacora.Application/Commands/Consent/GrantConsentCommand.cs` — consent behavior pattern.  
`src/Shared.Contract/Events/UsuarioActualizadoEvent.cs` — shared contract style.

## Prompt
Implement the application layer for vínculo and consent cascade behavior. Use `mi-lsp nav search "BootstrapPatientCommand" --include-content --workspace humor --format toon`, `mi-lsp nav search "GrantConsentCommand" --include-content --workspace humor --format toon`, `mi-lsp nav search "GetCurrentConsentQuery" --include-content --workspace humor --format toon`, and `mi-lsp nav search "UsuarioActualizadoEvent" --include-content --workspace humor --format toon` to confirm patterns. Create commands and queries for generating binding codes, accepting/revoking care links, listing active links, and applying consent cascade rules. Add shared DTO/event contracts under `src/Shared.Contract/Vinculos/` and `src/Shared.Contract/Events/` as needed. Keep the logic inside the application layer; do not wire Minimal API endpoints yet.

## Skeleton
```csharp
public sealed record CreateBindingCodeCommand(Guid ProfessionalId, Guid PatientId);
public sealed record GetActiveCareLinksQuery(Guid PatientId);
```

## Verify
`dotnet build src/Bitacora.sln --no-restore` -> `Build succeeded`

## Commit
`feat(app): add vínculo commands, queries, and contracts`
