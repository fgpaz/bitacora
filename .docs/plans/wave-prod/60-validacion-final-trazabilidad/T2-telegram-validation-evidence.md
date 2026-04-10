# Task T2: Telegram Validation Evidence

## Shared Context
**Goal:** Capture final Telegram/channel evidence after the runtime is implemented and hardened.  
**Stack:** Telegram test account, runtime validation, artifacts, `mi-lsp`.  
**Architecture:** Telegram validation is evidence-driven and channel-specific; this is the first phase allowed to mark it as validated.

## Task Metadata
```yaml
id: T2
depends_on: []
agent_type: ps-worker
files:
  - create: artifacts/e2e/2026-04-10-wave-prod-telegram-validation/
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-TG-001.md
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-TG-002.md
  - read: .docs/wiki/07_tech/TECH-TELEGRAM.md
  - read: .docs/wiki/09_contratos/CT-TELEGRAM-RUNTIME.md
complexity: high
done_when: "Validation artifacts for Telegram/channel flows exist under artifacts/e2e/2026-04-10-wave-prod-telegram-validation/"
```

## Reference
`.docs/wiki/23_uxui/UI-RFC/UI-RFC-TG-001.md` and `.docs/wiki/23_uxui/UI-RFC/UI-RFC-TG-002.md` — channel contracts to validate against.  
`.docs/wiki/07_tech/TECH-TELEGRAM.md` — runtime behavior baseline.

## Prompt
Run the final Telegram/channel validation only after the Telegram runtime and hardening phases are complete. Capture evidence under `artifacts/e2e/2026-04-10-wave-prod-telegram-validation/`, including reproduced conversations, screenshots or transcripts, and defect notes. Use the TG UI-RFC/handoff docs plus the technical and contract docs to judge correctness. If the dedicated Telegram test account or environment is not available, record that as a blocking validation gap instead of pretending success.

## Skeleton
```text
artifacts/e2e/2026-04-10-wave-prod-telegram-validation/
  - summary.md
  - transcript.md
  - defects.md
```

## Verify
`Test-Path artifacts/e2e/2026-04-10-wave-prod-telegram-validation` -> `True`

## Commit
`chore(qa): capture telegram validation evidence`
