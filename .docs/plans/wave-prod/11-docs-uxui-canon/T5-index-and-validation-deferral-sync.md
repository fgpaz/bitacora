# Task T5: Index + Validation Deferral Sync

## Shared Context
**Goal:** Synchronize indexes so later code phases can trust the UX/UI canon and the validation timing.  
**Stack:** UX/UI canon, markdown indexes, `mi-lsp`.  
**Architecture:** This task closes the docs phase by making readiness and deferral status explicit across the UX/UI tree.

## Task Metadata
```yaml
id: T5
depends_on: [T2, T3, T4]
agent_type: ps-docs
files:
  - modify: .docs/wiki/23_uxui/INDEX.md
  - modify: .docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md
  - modify: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-INDEX.md
  - modify: .docs/wiki/23_uxui/HANDOFF-ASSETS/HANDOFF-ASSETS-INDEX.md
  - modify: .docs/wiki/23_uxui/HANDOFF-MAPPING/HANDOFF-MAPPING-INDEX.md
  - modify: .docs/wiki/23_uxui/HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-INDEX.md
  - modify: .docs/wiki/21_matriz_validacion_ux.md
complexity: medium
done_when: "All UX/UI indexes reflect the new slice coverage and the validation matrix clearly remains post-code only"
```

## Reference
`.docs/wiki/23_uxui/INDEX.md` — root index for the hardened UX/UI canon.  
`.docs/wiki/21_matriz_validacion_ux.md` — timing gate for validation evidence.

## Prompt
After T2, T3, and T4 finish, synchronize the indexes. Update the root `23_uxui` index plus every relevant specialized index so the new `UI-RFC` and handoff files are discoverable. Make the validation matrix explicit that no slice is considered validated until the final post-code phase. Use `mi-lsp nav search "frontend/" --workspace humor --format toon` and `mi-lsp nav search "TelegramSession" --workspace humor --format toon` if you need to restate why live evidence is still pending.

## Skeleton
```md
## Slice status
## Ready for implementation
## Awaiting post-code validation
```

## Verify
`git diff -- .docs/wiki/23_uxui .docs/wiki/21_matriz_validacion_ux.md` -> indexes and validation timing are synchronized

## Commit
`docs(ux): synchronize indexes and validation deferral`
