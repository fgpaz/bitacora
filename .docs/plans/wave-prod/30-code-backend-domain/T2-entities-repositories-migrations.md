# Task T2: Entities / Repositories / Migrations

## Shared Context
**Goal:** Materialize the vínculo domain in the domain and persistence layers.  
**Stack:** .NET 10, EF Core, PostgreSQL, `mi-lsp`.  
**Architecture:** New domain entities and repository seams should follow the existing `PendingInvite`, `ConsentGrant`, and repository patterns.

## Task Metadata
```yaml
id: T2
depends_on: [T1]
agent_type: ps-dotnet10
files:
  - create: src/Bitacora.Domain/Entities/BindingCode.cs
  - create: src/Bitacora.Domain/Entities/CareLink.cs
  - create: src/Bitacora.Domain/Enums/CareLinkStatus.cs
  - create: src/Bitacora.DataAccess.Interface/Repositories/IBindingCodeRepository.cs
  - create: src/Bitacora.DataAccess.Interface/Repositories/ICareLinkRepository.cs
  - modify: src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs
  - create: src/Bitacora.DataAccess.EntityFramework/Repositories/BindingCodeRepository.cs
  - create: src/Bitacora.DataAccess.EntityFramework/Repositories/CareLinkRepository.cs
  - modify: src/Bitacora.DataAccess.EntityFramework/DependencyInjection/ServiceCollectionExtensions.cs
  - create: src/Bitacora.DataAccess.EntityFramework/Persistence/Migrations/*BindingCode*CareLink*.cs
complexity: high
done_when: "dotnet build src/Bitacora.sln exits 0 and the vínculo entities, repositories, and migration files exist"
```

## Reference
`src/Bitacora.Domain/Entities/PendingInvite.cs` — domain entity style.  
`src/Bitacora.DataAccess.Interface/Repositories/IPendingInviteRepository.cs` — repository interface pattern.  
`src/Bitacora.DataAccess.EntityFramework/Repositories/PendingInviteRepository.cs` — repository implementation pattern.

## Prompt
Implement the persistence side of the vínculo domain. Start with `mi-lsp nav search "PendingInvite" --include-content --workspace humor --format toon`, `mi-lsp nav search "ConsentGrant" --include-content --workspace humor --format toon`, and `mi-lsp nav search "Repository" --include-content --workspace humor --format toon` to confirm the local patterns. Then add `BindingCode` and `CareLink` entities plus any status enum required, add matching repository interfaces and EF implementations, register them in DI, materialize them in `AppDbContext`, and create an explicit migration that extends the current initial schema rather than rewriting it. Preserve manual production migration workflow. Do not touch API endpoints in this task.

## Skeleton
```csharp
public sealed class CareLink
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid ProfessionalId { get; private set; }
}
```

## Verify
`dotnet build src/Bitacora.sln --no-restore` -> `Build succeeded`

## Commit
`feat(domain): add binding code and care link persistence layer`
