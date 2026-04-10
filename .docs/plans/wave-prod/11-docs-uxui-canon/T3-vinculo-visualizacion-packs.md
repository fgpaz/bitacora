# Task T3: Vinculo + Visualizacion Packs

## Shared Context
**Goal:** Close the pre-code UX/UI canon for vínculo, visualization, export, and professional-visible slices.  
**Stack:** UX/UI canon, Next.js 16 target runtime, `mi-lsp`.  
**Architecture:** Web professional views and vínculo flows need technical UI closure before code and before runtime validation.

## Task Metadata
```yaml
id: T3
depends_on: [T1]
agent_type: ps-docs
files:
  - modify: .docs/wiki/23_uxui/UXR/UXR-VIN-*.md
  - modify: .docs/wiki/23_uxui/UXI/UXI-VIN-*.md
  - modify: .docs/wiki/23_uxui/UJ/UJ-VIN-*.md
  - modify: .docs/wiki/23_uxui/VOICE/VOICE-VIN-*.md
  - modify: .docs/wiki/23_uxui/UXS/UXS-VIN-*.md
  - modify: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-*.md
  - modify: .docs/wiki/23_uxui/UXR/UXR-VIS-*.md
  - modify: .docs/wiki/23_uxui/UXI/UXI-VIS-*.md
  - modify: .docs/wiki/23_uxui/UJ/UJ-VIS-*.md
  - modify: .docs/wiki/23_uxui/VOICE/VOICE-VIS-*.md
  - modify: .docs/wiki/23_uxui/UXS/UXS-VIS-*.md
  - modify: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIS-*.md
  - modify: .docs/wiki/23_uxui/UXR/UXR-EXP-001.md
  - modify: .docs/wiki/23_uxui/UXI/UXI-EXP-001.md
  - modify: .docs/wiki/23_uxui/UJ/UJ-EXP-001.md
  - modify: .docs/wiki/23_uxui/VOICE/VOICE-EXP-001.md
  - modify: .docs/wiki/23_uxui/UXS/UXS-EXP-001.md
  - modify: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-EXP-001.md
  - modify: .docs/wiki/23_uxui/UXR/UXR-CON-002.md
  - modify: .docs/wiki/23_uxui/UXI/UXI-CON-002.md
  - modify: .docs/wiki/23_uxui/UJ/UJ-CON-002.md
  - modify: .docs/wiki/23_uxui/VOICE/VOICE-CON-002.md
  - modify: .docs/wiki/23_uxui/UXS/UXS-CON-002.md
  - modify: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-CON-002.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-001.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-002.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-003.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-004.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIS-001.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIS-002.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-EXP-001.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-CON-002.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-VIN-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-VIN-002.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-VIN-003.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-VIN-004.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-VIS-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-VIS-002.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-EXP-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-CON-002.md
complexity: high
done_when: "Vínculo, visualization, export, and professional-visible slices have implementation-ready UI contracts at least through UI-RFC and handoff spec"
```

## Reference
`.docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md` — reference depth for component contracts.  
`.docs/wiki/23_uxui/UXS/UXS-VIN-001.md` and `.docs/wiki/23_uxui/UXS/UXS-VIS-001.md` — current slice seeds.

## Prompt
Harden the remaining non-Telegram visible slices for implementation. Use the existing canon chain as seed material, but create the missing technical UI layer (`UI-RFC` and at least `HANDOFF-SPEC`; add the other handoff docs when a slice clearly needs them). Before writing, verify technical plausibility using `mi-lsp nav search "CareLink" --workspace humor --format toon`, `mi-lsp nav search "PendingInvite" --include-content --workspace humor --format toon`, `mi-lsp nav search "ConsentGrant" --include-content --workspace humor --format toon`, and `mi-lsp nav search "MapConsentEndpoints" --include-content --workspace humor --format toon`. If a slice depends on future backend contracts, keep that dependency explicit in the docs instead of pretending the runtime already exists.

## Skeleton
```md
## Entry state
## Main path
## Error and empty states
## Technical UI contract
## Backend dependencies
```

## Verify
`git diff -- .docs/wiki/23_uxui` -> vínculo, visualization, export, and professional-visible slices gained the missing technical UI layer

## Commit
`docs(ux): harden vinculo and visualization slice packs`
