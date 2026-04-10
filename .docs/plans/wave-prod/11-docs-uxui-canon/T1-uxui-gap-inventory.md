# Task T1: UX/UI Gap Inventory

## Shared Context
**Goal:** Establish the active UX/UI gap map before generating more technical UI artifacts.  
**Stack:** Markdown UX/UI canon, HTML prototypes, `mi-lsp`.  
**Architecture:** `ONB-001` is already open; remaining slices need truthful inventory and explicit deferral rules.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-docs
files:
  - create: .docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md
  - modify: .docs/wiki/21_matriz_validacion_ux.md
  - read: .docs/wiki/23_uxui/INDEX.md
complexity: medium
done_when: "A durable UX/UI gap map exists and the validation matrix clearly separates prepared slices from evidence-backed validation"
```

## Reference
`.docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md` — reference depth for a slice already opened.  
`.docs/wiki/21_matriz_validacion_ux.md` — current validation state table.

## Prompt
Start with the existing UX/UI canon and do not generate new slice docs yet. Review `.docs/wiki/23_uxui/INDEX.md`, all current index files under `23_uxui`, and `.docs/wiki/21_matriz_validacion_ux.md`. Use `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "frontend/" --workspace humor --format toon`, and `mi-lsp nav search "TelegramSession" --workspace humor --format toon` to re-confirm there is still no web runtime or Telegram session runtime in code. Then create `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md` that lists: slices already implementation-ready, slices missing `UI-RFC`, slices missing `HANDOFF`, and slices that must stay in prepared-only validation status until the end. Update `.docs/wiki/21_matriz_validacion_ux.md` only to make that timing explicit; do not mark any slice as validated here.

## Skeleton
```md
# Wave-Prod UX/UI Gap Map
## Ready for code
## Missing UI-RFC or handoff
## Validation deferred until post-code
```

## Verify
`git diff -- .docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md .docs/wiki/21_matriz_validacion_ux.md` -> gap map and deferral rules are explicit

## Commit
`docs(ux): record wave-prod UX/UI gap inventory`
