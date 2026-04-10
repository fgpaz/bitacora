# Task T4: Reminders / Webhook / Audit Integration

## Shared Context
**Goal:** Wire reminders and Telegram runtime integration into the backend with operational controls and auditing.  
**Stack:** .NET 10, Telegram runtime, audit layer, `mi-lsp`.  
**Architecture:** This task integrates the Telegram core with runtime entrypoints, reminder delivery, and audit-safe behavior.

## Task Metadata
```yaml
id: T4
depends_on: [T2, T3]
agent_type: ps-dotnet10
files:
  - create: src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs
  - modify: src/Bitacora.Api/Program.cs
  - create: src/Bitacora.Application/Commands/Telegram/SendReminderCommand.cs
  - create: src/Bitacora.Application/Commands/Telegram/HandleWebhookUpdateCommand.cs
  - modify: src/Bitacora.Application/Interfaces/IIntegrationEventPublisher.cs
  - modify: src/Bitacora.DataAccess.EntityFramework/Repositories/AccessAuditRepository.cs
complexity: high
done_when: "dotnet build src/Bitacora.sln exits 0 and Telegram webhook/reminder runtime is wired with audit coverage"
```

## Reference
`src/Bitacora.Domain/Entities/AccessAudit.cs` — audit model to extend.  
`.docs/wiki/07_tech/TECH-TELEGRAM.md` — runtime safety rules.  
`.docs/wiki/09_contratos/CT-TELEGRAM-RUNTIME.md` — contract rules to honor.

## Prompt
Integrate the Telegram runtime into the API/application stack. Before editing, use `mi-lsp nav search "AccessAuditRepository" --include-content --workspace humor --format toon`, `mi-lsp nav search "IIntegrationEventPublisher" --include-content --workspace humor --format toon`, and `mi-lsp nav search "TelegramSession" --include-content --workspace humor --format toon`. Then add the API entrypoint for Telegram webhook/runtime handling, integrate reminder delivery commands, ensure chat/session actions are audited, and keep fail-closed behavior for missing consent, missing vínculo context, or unknown session state. Do not add final validation or operator tooling in this task.

## Skeleton
```csharp
public static class TelegramEndpoints
{
    public static void MapTelegramEndpoints(this IEndpointRouteBuilder app)
    {
    }
}
```

## Verify
`dotnet build src/Bitacora.sln --no-restore` -> `Build succeeded`

## Commit
`feat(tg): wire telegram runtime, reminders, and audit integration`
