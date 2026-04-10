# Task T2: Patient Slice Packs

## Shared Context
**Goal:** Make the remaining patient-facing slices implementation-ready before any frontend code starts.  
**Stack:** UX/UI canon, Next.js 16 target runtime, `mi-lsp`.  
**Architecture:** `REG-001` and `REG-002` need the same level of technical closure that `ONB-001` already has.

## Task Metadata
```yaml
id: T2
depends_on: [T1]
agent_type: ps-docs
files:
  - modify: .docs/wiki/23_uxui/UXR/UXR-REG-001.md
  - modify: .docs/wiki/23_uxui/UXR/UXR-REG-002.md
  - modify: .docs/wiki/23_uxui/UXI/UXI-REG-001.md
  - modify: .docs/wiki/23_uxui/UXI/UXI-REG-002.md
  - modify: .docs/wiki/23_uxui/UJ/UJ-REG-001.md
  - modify: .docs/wiki/23_uxui/UJ/UJ-REG-002.md
  - modify: .docs/wiki/23_uxui/VOICE/VOICE-REG-001.md
  - modify: .docs/wiki/23_uxui/VOICE/VOICE-REG-002.md
  - modify: .docs/wiki/23_uxui/UXS/UXS-REG-001.md
  - modify: .docs/wiki/23_uxui/UXS/UXS-REG-002.md
  - modify: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-REG-001.md
  - modify: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-REG-002.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-REG-001.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-REG-002.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-REG-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-REG-002.md
  - create: .docs/wiki/23_uxui/HANDOFF-ASSETS/HANDOFF-ASSETS-REG-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-ASSETS/HANDOFF-ASSETS-REG-002.md
  - create: .docs/wiki/23_uxui/HANDOFF-MAPPING/HANDOFF-MAPPING-REG-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-MAPPING/HANDOFF-MAPPING-REG-002.md
  - create: .docs/wiki/23_uxui/HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-REG-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-REG-002.md
complexity: high
done_when: "REG-001 and REG-002 each have a complete pre-code UX/UI chain through UI-RFC and the four handoff docs"
```

## Reference
`.docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md` — copy the depth, not the content.  
`.docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-ONB-001.md` — handoff baseline for a patient slice.

## Prompt
Using the gap map from T1, harden `REG-001` and `REG-002` into implementation-ready patient slices. Reuse the current `UXR/UXI/UJ/VOICE/UXS/PROTOTYPE` docs as inputs, and match the technical depth of `ONB-001` for `UI-RFC` plus the four handoff documents. Before writing, re-check relevant backend reality with `mi-lsp nav search "MapRegistroEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "MoodEntry" --include-content --workspace humor --format toon`, and `mi-lsp nav search "DailyCheckin" --include-content --workspace humor --format toon` so the docs do not promise fields or transitions the backend cannot plausibly support. Do not run UX validation. Instead, leave explicit notes about which evidence must be collected in the final validation phase.

## Skeleton
```md
## Scope and entry conditions
## Critical states
## Copy slots and CTA rules
## Responsive behavior
## Validation deferred
```

## Verify
`git diff -- .docs/wiki/23_uxui` -> REG-001 and REG-002 now have full pre-code UI-RFC and handoff coverage

## Commit
`docs(ux): open REG patient slice packs`
