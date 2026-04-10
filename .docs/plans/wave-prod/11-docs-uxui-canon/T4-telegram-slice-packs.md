# Task T4: Telegram Slice Packs

## Shared Context
**Goal:** Close the pre-code conversational UX/UI layer for Telegram without treating it as already validated.  
**Stack:** UX/UI canon, Telegram target runtime, `mi-lsp`.  
**Architecture:** Telegram flows must be implementation-ready at the spec level, but final evidence only arrives after the bot runtime exists.

## Task Metadata
```yaml
id: T4
depends_on: [T1]
agent_type: ps-docs
files:
  - modify: .docs/wiki/23_uxui/UXR/UXR-TG-001.md
  - modify: .docs/wiki/23_uxui/UXR/UXR-TG-002.md
  - modify: .docs/wiki/23_uxui/UXI/UXI-TG-001.md
  - modify: .docs/wiki/23_uxui/UXI/UXI-TG-002.md
  - modify: .docs/wiki/23_uxui/UJ/UJ-TG-001.md
  - modify: .docs/wiki/23_uxui/UJ/UJ-TG-002.md
  - modify: .docs/wiki/23_uxui/VOICE/VOICE-TG-001.md
  - modify: .docs/wiki/23_uxui/VOICE/VOICE-TG-002.md
  - modify: .docs/wiki/23_uxui/UXS/UXS-TG-001.md
  - modify: .docs/wiki/23_uxui/UXS/UXS-TG-002.md
  - modify: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-TG-001.md
  - modify: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-TG-002.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-TG-001.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-TG-002.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-TG-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-TG-002.md
complexity: high
done_when: "TG-001 and TG-002 have implementation-ready pre-code conversational contracts and remain marked as awaiting post-code evidence"
```

## Reference
`.docs/wiki/23_uxui/VOICE/VOICE-TG-001.md` — tone baseline for channel interactions.  
`.docs/wiki/07_tech/TECH-TELEGRAM.md` — technical backdrop for Telegram runtime expectations.

## Prompt
Take the Telegram slices from discovery through technical UI/conversation contract, but stop before validation. Use `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "TelegramSession" --workspace humor --format toon`, and `mi-lsp nav search "TECH-TELEGRAM" --workspace humor --format toon` only to confirm the runtime is still absent and to keep the docs truthful. Update the conversational canon docs, generate the missing `UI-RFC` and `HANDOFF-SPEC` files for `TG-001` and `TG-002`, and make every doc explicit about expected prompts, fallbacks, recovery, and audit-sensitive moments. Validation evidence remains deferred to the final portfolio phase.

## Skeleton
```md
## Trigger
## Bot response contract
## Recovery path
## Audit-sensitive copy
## Validation deferred
```

## Verify
`git diff -- .docs/wiki/23_uxui` -> Telegram slices have technical closure but still await final validation evidence

## Commit
`docs(ux): harden telegram slice packs`
