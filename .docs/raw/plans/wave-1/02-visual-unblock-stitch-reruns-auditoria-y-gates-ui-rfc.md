# Task T02: ONB-first visual unblock, UI-RFC, and handoff package

## Shared Context
**Goal:** Turn `ONB-001` into an implementation-ready visual/documentary package without pretending UX validation already exists.
**Stack:** `23_uxui`, global UX/UI canon, Stitch audit history, frontend system design, handoff chain.
**Architecture:** This task does not implement web runtime or reopen the global visual baseline. It refreshes the ONB slice truth, opens `UI-RFC-ONB-001`, creates the specialized handoff pack, and leaves the remaining slices under their previous gate.

## Task Metadata
```yaml
id: T02
depends_on: []
agent_type: ps-docs
files:
  - modify: .docs/wiki/17_UXR.md
  - modify: .docs/wiki/18_UXI.md
  - modify: .docs/wiki/19_UJ.md
  - modify: .docs/wiki/20_UXS.md
  - modify: .docs/wiki/16_patrones_ui.md
  - modify: .docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md
  - modify: .docs/wiki/09_contratos_tecnicos.md
  - modify: .docs/wiki/23_uxui/INDEX.md
  - modify: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-INDEX.md
  - modify: .docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md
  - create: .docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-INDEX.md
  - create: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-ONB-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-ASSETS/HANDOFF-ASSETS-INDEX.md
  - create: .docs/wiki/23_uxui/HANDOFF-ASSETS/HANDOFF-ASSETS-ONB-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-MAPPING/HANDOFF-MAPPING-INDEX.md
  - create: .docs/wiki/23_uxui/HANDOFF-MAPPING/HANDOFF-MAPPING-ONB-001.md
  - create: .docs/wiki/23_uxui/HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-INDEX.md
  - create: .docs/wiki/23_uxui/HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-ONB-001.md
  - modify: .docs/stitch/STITCH-ARTIFACTS-AUDIT.md
  - create: .docs/raw/decisiones/2026-04-10-onb-001-manual-authority-pack.md
complexity: high
done_when: "ONB-001 has a coherent chain from UXR to specialized handoff, T04/T05 can implement the slice without reopening hierarchy or state decisions, and the repo still makes it explicit that REG-001 and REG-002 remain visually blocked."
```

## Reference
- `.docs/wiki/23_uxui/INDEX.md`
- `.docs/wiki/23_uxui/UXR/UXR-ONB-001.md`
- `.docs/wiki/23_uxui/UXI/UXI-ONB-001.md`
- `.docs/wiki/23_uxui/UJ/UJ-ONB-001.md`
- `.docs/wiki/23_uxui/VOICE/VOICE-ONB-001.md`
- `.docs/wiki/23_uxui/UXS/UXS-ONB-001.md`
- `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-ONB-001.md`
- `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `.docs/wiki/15_handoff_operacional_uxui.md`

## Prompt
Implement the ONB-first visual unblock as a near-complete UI delivery pack.

1. Refresh the ONB chain so the slice tells one consistent story: personal guide, invite-aware entry, clear consent, and a bridge to the first record.
2. Open `UI-RFC-ONB-001.md` under an explicit authority-pack decision instead of pretending Stitch already passed.
3. Create the full specialized handoff quartet:
   - `HANDOFF-SPEC`
   - `HANDOFF-ASSETS`
   - `HANDOFF-MAPPING`
   - `HANDOFF-VISUAL-QA`
4. Synchronize supporting global docs (`16`, `07_tech`, `09`) only where they directly gate this slice.
5. Keep the rest of the slices truthful:
   - `REG-001` and `REG-002` remain blocked
   - the remaining slices remain pending rerun/audit
6. Record the decision explicitly so later agents do not assume the old gate matrix is still the truth.

## Execution Waves
### Wave A — ONB truth refresh
- Rewrite UXR/UXI/UJ/VOICE/UXS/PROTOTYPE for the ONB-first slice.

### Wave B — Technical UI contract
- Open `UI-RFC-ONB-001.md`.
- Freeze backend dependencies, state inventory, component grammar, and responsive rules.

### Wave C — Specialized handoff
- Create `HANDOFF-SPEC`, `HANDOFF-ASSETS`, `HANDOFF-MAPPING`, and `HANDOFF-VISUAL-QA`.
- Update indexes and decisions so the exception is explicit and bounded.

## Verify
`git diff -- .docs/wiki .docs/stitch .docs/raw/plans .docs/raw/decisiones` -> `ONB-001` can be handed to frontend without hidden design questions, while blocked slices remain blocked.

## Commit
`docs(plan): implement T02 ONB-first visual unblock pack`
