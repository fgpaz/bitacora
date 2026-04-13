# Task T3: Implement Telegram Webhook Handlers (RF-REG-010..015)

## Shared Context
**Goal:** Replace the `HandleWebhookUpdateCommandHandler` stub with real business logic implementing RF-REG-010 through RF-REG-015.
**Stack:** .NET 10, C#, Telegram Bot API, PostgreSQL via EF Core, Multi-tenant `PatientId` filter.
**Architecture:** Telegram webhook -> `HandleWebhookUpdateCommand` -> routes to session resolution or pairing/mood entry creation.

## Task Metadata
```yaml
id: T3
depends_on: []
agent_type: ps-dotnet10
files:
  - read: src/Bitacora.Application/Commands/Telegram/HandleWebhookUpdateCommand.cs
  - read: src/Bitacora.Application/Commands/Telegram/SendReminderCommand.cs
  - read: src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs
  - read: src/Bitacora.Domain/Entities/TelegramSession.cs
  - read: src/Bitacora.Domain/Entities/TelegramPairingCode.cs
  - read: src/Bitacora.DataAccess.EntityFramework/Repositories/TelegramSessionRepository.cs
  - read: src/Bitacora.DataAccess.EntityFramework/Repositories/TelegramPairingCodeRepository.cs
complexity: high
done_when: "dotnet build --no-restore src/Bitacora.sln exits 0; handler has real logic not stub"
```

## Reference
`SendReminderCommand.cs:121` — Telegram Bot API HTTP call pattern (`SendViaTelegramApiAsync`)
`TelegramEndpoints.cs:83-117` — webhook endpoint + `X-Telegram-Webhook-Secret` validation
`TelegramPairingCodeRepository.cs` — existing pairing code TTL validation (15-min)

## What to Implement

### RF-REG-010: Receive and validate Telegram webhook
The endpoint already exists and validates `X-Telegram-Webhook-Secret`. The handler must parse the `Update` payload.

### RF-REG-011: Resolve TelegramSession by chat_id
Add `GetByChatIdAsync(chatId)` to `ITelegramSessionRepository` and implement in `TelegramSessionRepository`.

### RF-REG-012: Create MoodEntry from Telegram callback
When a linked `TelegramSession` (status `Active`) receives a message with a mood score:
1. Validate score is integer -3..+3
2. Create `MoodEntry` for the linked `PatientId`
3. Check consent active (RF-CON-003) before allowing
4. Reply to Telegram with confirmation

### RF-REG-013: Sequential factors flow via Telegram conversation state
Telegram sessions can have state. Use `TelegramSession.LastCommand` or conversation state to guide:
1. After mood score, ask for factors (sleep, activity, anxiety, irritability, medication)
2. Handle partial responses (some fields optional)
3. Create `DailyCheckin` after factors collected

### RF-REG-014: Handle unlinked TelegramSession
When `TelegramSession` is `Pending` or not found:
1. If `/start CODE` -> route to pairing flow
2. If any other message -> reply with "Vincula tu cuenta primero" + pairing instructions

### RF-REG-015: Handle consent denial via Telegram
When patient tries to log mood but `ConsentGrant` is missing or revoked:
1. Reply with consent request message
2. Provide one-click consent link or instruction

## Prompt

You are a .NET 10 backend specialist. Your job is to implement the Telegram webhook business logic replacing the stub in `HandleWebhookUpdateCommandHandler`.

**Step 1: Read all reference files (listed in Task Metadata above). Use mi-lsp to navigate:**
```
mi-lsp nav context HandleWebhookUpdateCommand --workspace humor
mi-lsp nav context SendReminderCommand --workspace humor
mi-lsp nav context TelegramEndpoints --workspace humor
```

**Step 2: Replace the stub in `HandleWebhookUpdateCommandHandler`**
- Open `src/Bitacora.Application/Commands/Telegram/HandleWebhookUpdateCommand.cs`
- The current handler at line ~28 just returns `Accepted=false`. Replace it with real logic.
- Follow the SendReminderCommand pattern for Telegram API calls.

**Step 3: Implement session resolution (RF-REG-011)**
- Add `GetByChatIdAsync(string chatId)` to `ITelegramSessionRepository`
- Implement in `TelegramSessionRepository` using EF Core with multi-tenant filter

**Step 4: Implement pairing flow (RF-REG-014)**
- When `/start BIT-XXXXX` received: validate pairing code, create/link `TelegramSession`, reply with confirmation
- Handle expired/invalid codes gracefully

**Step 5: Implement mood entry creation (RF-REG-012)**
- When linked session + consent active + score received: create `MoodEntry`
- Score validation: integer -3..+3, reject outside range
- Reply to Telegram with mood confirmation

**Step 6: Implement factors flow (RF-REG-013)**
- Use `TelegramSession.ConversationState` to track: AwaitingMood -> AwaitingFactors -> Complete
- After score, ask for factors inline
- Create `DailyCheckin` when factors received

**Step 7: Handle consent denial (RF-REG-015)**
- If no active `ConsentGrant` for patient: reply with consent instruction

**Key constraints:**
- All Telegram API replies must use `SendViaTelegramApiAsync` pattern from `SendReminderCommand`
- Multi-tenant filter: `MoodEntry` and `DailyCheckin` use `PatientId` query filter — must set `ICurrentPatientContextAccessor`
- Return `Accepted=false` with error code only for business rejections; always return HTTP 200 to Telegram
- No clinical data in error logs

## Skeleton
```csharp
public class HandleWebhookUpdateCommandHandler : IRequestHandler<HandleWebhookUpdateCommand, AcceptsWebhookCommand>
{
    public async Task<AcceptsWebhookCommand> Handle(HandleWebhookUpdateCommand request, CancellationToken ct)
    {
        var update = request.Update;
        
        if (update.CallbackQuery != null)
            return await HandleCallbackQuery(update.CallbackQuery, ct);
        
        if (update.Message?.Text != null)
            return await HandleMessage(update.Message, ct);
        
        return AcceptsWebhookCommand.Accepted(); // no-op
    }
}
```

## Verify
`dotnet build --no-restore src/Bitacora.sln` exits 0 with no warnings on the new handler code.

## Commit
`feat(telegram): implement RF-REG-010..015 webhook handlers`
