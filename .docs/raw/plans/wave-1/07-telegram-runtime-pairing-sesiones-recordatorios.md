# Task T07: Telegram Runtime, Pairing, Sessions, Reminders, and Conversational Registro

## Shared Context
**Goal:** Turn the deferred Telegram canon into a real production runtime that uses the current backend as its home.
**Stack:** .NET 10, Telegram Bot API, PostgreSQL, Dokploy, Supabase-authenticated web bootstrap.
**Architecture:** Telegram stays inside the existing backend solution, with webhook-based production delivery, persistent session/pairing state, and hosted reminder scheduling.

## Task Metadata
```yaml
id: T07
depends_on:
  - T01
  - T03
agent_type: ps-dotnet10
files:
  - create: src/Bitacora.Domain/Entities/TelegramSession.cs
  - create: src/Bitacora.Domain/Entities/TelegramPairingCode.cs
  - create: src/Bitacora.Domain/Entities/ReminderConfig.cs
  - create: src/Bitacora.Application/Commands/Telegram/CreatePairingCodeCommand.cs
  - create: src/Bitacora.Application/Commands/Telegram/HandleWebhookUpdateCommand.cs
  - create: src/Bitacora.Application/Commands/Telegram/UpsertReminderConfigCommand.cs
  - create: src/Bitacora.Application/Queries/Telegram/GetPairingStatusQuery.cs
  - create: src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs
  - create: src/Bitacora.Infrastructure/Options/TelegramOptions.cs
  - create: src/Bitacora.Infrastructure/Services/TelegramWebhookSignatureValidator.cs
  - create: src/Bitacora.Infrastructure/Services/TelegramReminderHostedService.cs
  - create: src/Bitacora.DataAccess.Interface/Repositories/ITelegramSessionRepository.cs
  - create: src/Bitacora.DataAccess.Interface/Repositories/ITelegramPairingCodeRepository.cs
  - create: src/Bitacora.DataAccess.EntityFramework/Repositories/TelegramSessionRepository.cs
  - create: src/Bitacora.DataAccess.EntityFramework/Repositories/TelegramPairingCodeRepository.cs
  - modify: src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs
  - modify: src/Bitacora.Api/Program.cs
  - modify: src/Bitacora.Api/.env.example
  - read: .docs/wiki/07_tech/TECH-TELEGRAM.md
  - read: .docs/wiki/09_contratos_tecnicos.md
complexity: high
done_when: "Telegram pairing, session resolution, reminders, and conversational entry are fully specified for production webhook delivery."
```

## Reference
- `.docs/wiki/07_tech/TECH-TELEGRAM.md` — deferred Telegram technical baseline
- `.docs/wiki/09_contratos_tecnicos.md` — pairing and Telegram error contracts
- `src/Bitacora.Api/Endpoints/Registro/RegistroEndpoints.cs` — current Registro behavior to reuse

## Prompt
Materialize the Telegram module without creating a second runtime service.

1. Implement persistent entities for `TelegramSession`, `TelegramPairingCode`, and `ReminderConfig`.
2. Freeze production mode as webhook-first because this wave is prod-first and has no staging environment.
3. Add pairing flow:
   - authenticated patient requests pairing code from web/API
   - Telegram chat redeems it
   - backend binds chat to patient
4. Implement session resolution and consent checks before any conversational entry creates data.
5. Implement reminder scheduling inside the backend with production-safe defaults and explicit disable/reenable behavior.
6. Reuse existing Registro semantics for mood entry and daily factors instead of forking Telegram-only business rules.
7. Explicitly plan Telegram failure modes:
   - invalid signature / invalid webhook payload
   - unlinked session
   - missing consent
   - revoked relationship / disabled reminders
8. Update env/example and infrastructure contract to include Telegram secrets and webhook URL expectations.

## Execution Waves
### Wave A — Storage and pairing
- Add entities, repositories, migration, and pairing commands/endpoints.

### Wave B — Conversational runtime
- Add webhook handling, session lookup, consent gating, and sequential factor collection.

### Wave C — Reminder operations
- Add hosted scheduling, retry rules, disable/enable logic, and operational guidance.

## Skeleton
```csharp
public sealed class TelegramSession
{
    public long ChatId { get; private set; }
    public Guid PatientId { get; private set; }
    public bool IsLinked { get; private set; }
}
```

## Verify
`dotnet build src/Bitacora.sln -c Release` -> Telegram module compiles with persistent session/pairing/reminder seams.

## Commit
`feat(bitacora): implement telegram runtime and reminders`

