# Task T1: Read/Telegram Seam Map

## Shared Context
**Goal:** Record the exact extension points for the read side and Telegram runtime before writing code.  
**Stack:** .NET 10, EF Core, Telegram runtime, `mi-lsp`.  
**Architecture:** This phase extends the current backend rather than inventing a parallel service structure.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-worker
files:
  - create: .docs/raw/decisiones/2026-04-10-wave-prod-read-telegram-seam-map.md
  - read: src/Bitacora.Application/Queries/Consent/GetCurrentConsentQuery.cs
  - read: src/Bitacora.Application/Commands/Registro/UpsertDailyCheckinCommand.cs
  - read: src/Bitacora.Domain/Entities/MoodEntry.cs
  - read: src/Bitacora.Domain/Entities/AccessAudit.cs
  - read: .docs/wiki/07_tech/TECH-TELEGRAM.md
complexity: medium
done_when: "A seam map exists under .docs/raw/decisiones/2026-04-10-wave-prod-read-telegram-seam-map.md with exact insertion points for VIS/EXP/TG"
```

## Reference
`src/Bitacora.Application/Queries/Consent/GetCurrentConsentQuery.cs` — existing query seam.  
`src/Bitacora.Api/Endpoints/Registro/RegistroEndpoints.cs` — current read/write route style to extend from.

## Prompt
Create a durable seam map for the next backend phase. Use `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "GetCurrentConsentQuery" --include-content --workspace humor --format toon`, `mi-lsp nav search "UpsertDailyCheckinCommand" --include-content --workspace humor --format toon`, `mi-lsp nav search "AccessAudit" --include-content --workspace humor --format toon`, and `mi-lsp nav search "TelegramSession" --workspace humor --format toon`. Then write `.docs/raw/decisiones/2026-04-10-wave-prod-read-telegram-seam-map.md` identifying where to plug visualization queries, export contracts, Telegram session persistence, reminder scheduling, webhook handling, and audit hooks. Do not modify application code in this task.

## Skeleton
```md
## Read-side seams
## Export seams
## Telegram seams
## Audit and operability seams
```

## Verify
`git diff -- .docs/raw/decisiones/2026-04-10-wave-prod-read-telegram-seam-map.md` -> seam map exists and cites exact files

## Commit
`docs(plan): record read and telegram seam map`
