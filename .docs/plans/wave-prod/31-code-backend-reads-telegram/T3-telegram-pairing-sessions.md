# Task T3: Telegram Pairing + Sessions

## Shared Context
**Goal:** Implement the persistence and application core for Telegram pairing and session state.  
**Stack:** .NET 10, EF Core, Telegram runtime, Shared.Contract, `mi-lsp`.  
**Architecture:** Telegram runtime should remain within the current backend and reuse the existing domain/application/data layering.

## Task Metadata
```yaml
id: T3
depends_on: [T1]
agent_type: ps-dotnet10
files:
  - create: src/Bitacora.Domain/Entities/TelegramSession.cs
  - create: src/Bitacora.Domain/Enums/TelegramSessionStatus.cs
  - create: src/Bitacora.DataAccess.Interface/Repositories/ITelegramSessionRepository.cs
  - create: src/Bitacora.DataAccess.EntityFramework/Repositories/TelegramSessionRepository.cs
  - modify: src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs
  - create: src/Bitacora.Application/Commands/Telegram/*.cs
  - create: src/Bitacora.Application/Queries/Telegram/*.cs
  - create: src/Shared.Contract/Telegram/*.cs
  - modify: src/Bitacora.Application/DependencyInjection/ServiceCollectionExtensions.cs
complexity: high
done_when: "dotnet build src/Bitacora.sln exits 0 and Telegram session domain/application scaffolding exists"
```

## Reference
`src/Bitacora.Domain/Entities/PendingInvite.cs` — pairing-like state pattern to reuse.  
`src/Bitacora.DataAccess.EntityFramework/Repositories/PendingInviteRepository.cs` — repository implementation style.  
`.docs/wiki/07_tech/TECH-TELEGRAM.md` — approved technical expectations.

## Prompt
Implement the Telegram core domain and application layer. Use `mi-lsp nav search "PendingInvite" --include-content --workspace humor --format toon`, `mi-lsp nav search "TelegramSession" --workspace humor --format toon`, and `mi-lsp nav search "TECH-TELEGRAM" --include-content --workspace humor --format toon` first. Then add `TelegramSession` persistence, repository seams, commands/queries for pairing and session lifecycle, and shared contracts for Telegram runtime integration. Keep reminder delivery itself for T4; this task is about pairing, session state, and bot/runtime contract primitives.

## Skeleton
```csharp
public sealed class TelegramSession
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public string ChatId { get; private set; } = string.Empty;
}
```

## Verify
`dotnet build src/Bitacora.sln --no-restore` -> `Build succeeded`

## Commit
`feat(tg): add telegram pairing and session core`
