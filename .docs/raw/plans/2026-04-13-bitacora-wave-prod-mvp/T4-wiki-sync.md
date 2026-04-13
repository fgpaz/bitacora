# Task T4: Sync Wiki Documentation

## Shared Context
**Goal:** Update FL and RF documentation to reflect actual implementation state after T1+T3 are complete.
**Stack:** Markdown wiki docs in `.docs/wiki/`
**Architecture:** FL-REG-02 was "Diferido" but webhook endpoint exists; RF-REG-010..015 were "Diferido" but some now have implementation.

## Task Metadata
```yaml
id: T4
depends_on: [T1, T2, T3]
agent_type: ps-docs
complexity: low
done_when: "FL-REG-02 status updated; RF-REG-010..015 status updated; git diff shows changes"
```

## Reference
- `.docs/wiki/03_FL/FL-REG-02.md` — Telegram flow doc
- `.docs/wiki/04_RF/RF-REG-010.md` through `.docs/wiki/04_RF/RF-REG-015.md`
- `.docs/wiki/09_contratos/CT-TELEGRAM-RUNTIME.md` — Telegram runtime contract
- `src/Bitacora.Application/Commands/Telegram/HandleWebhookUpdateCommand.cs` — new handler (T3 output)

## Prompt

After T1 (frontend deployment), T2 (E2E prompt), and T3 (Telegram handlers) are complete, update the wiki docs to reflect the actual state.

**Step 1: FL-REG-02**
- Read `.docs/wiki/03_FL/FL-REG-02.md`
- Find the "Estado actual" line
- Update from "Diferido" to "Parcial backend" — the webhook endpoint `POST /api/v1/telegram/webhook` is registered in `TelegramEndpoints.cs:83`, but the full handler (RF-REG-010..015) is now partially implemented (T3)
- Add note: "Endpoint exists (TelegramEndpoints.cs:83). Handler implemented via T3 (HandleWebhookUpdateCommandHandler). Full Telegram mood entry flow partial."

**Step 2: RF-REG-010 through RF-REG-015**
For each of these RF docs, read them and update the status:

- **RF-REG-010** (webhook receipt): update to "Parcial backend" — endpoint registered, handler stubbed then partially implemented
- **RF-REG-011** (session resolution by chat_id): update to "Parcial backend" — `GetByChatIdAsync` added to repository
- **RF-REG-012** (MoodEntry from Telegram): update to "Parcial backend" — handler implements mood entry creation
- **RF-REG-013** (sequential factors flow): keep as "Diferido" OR "Parcial backend" depending on T3 implementation completeness
- **RF-REG-014** (unlinked session handling): update to "Parcial backend" — `/start CODE` routing implemented
- **RF-REG-015** (consent denial handling): update to "Parcial backend" — consent check + reply implemented

**Step 3: CT-TELEGRAM-RUNTIME**
- Read `.docs/wiki/09_contratos/CT-TELEGRAM-RUNTIME.md`
- If T3 added retry/backoff or new error handling, update the contract doc accordingly

**Step 4: 09_contratos_tecnicos.md**
- If T3 introduced any new endpoints (should not — webhook was already there), add them to the contracts index

## Verify
`git diff --stat .docs/wiki/03_FL/FL-REG-02.md .docs/wiki/04_RF/RF-REG-010.md .docs/wiki/04_RF/RF-REG-011.md .docs/wiki/04_RF/RF-REG-012.md .docs/wiki/04_RF/RF-REG-013.md .docs/wiki/04_RF/RF-REG-014.md .docs/wiki/04_RF/RF-REG-015.md` shows changed lines.

## Commit
`docs(wiki): sync FL-REG-02 and RF-REG-010..015 status after T3 implementation`
