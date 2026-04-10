# Task T02: Visual Unblock, Stitch Reruns, Manual Audit, and UI-RFC Gates

## Shared Context
**Goal:** Turn the current visual/documentary blockers into an explicit gate matrix that later frontend work can trust.
**Stack:** Stitch artifacts, `23_uxui`, design-pack docs, visual audit notes.
**Architecture:** This task does not reopen the global visual baseline. It only fixes slice-level authority so later implementation knows exactly which slices are green, blocked, or pending rerun.

## Task Metadata
```yaml
id: T02
depends_on: []
agent_type: ps-worker
files:
  - modify: .docs/stitch/STITCH-ARTIFACTS-AUDIT.md
  - modify: .docs/wiki/23_uxui/INDEX.md
  - modify: .docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md
  - modify: .docs/stitch/DESIGN.md
  - modify: .docs/stitch/DESIGN.patient.md
  - modify: .docs/stitch/DESIGN.professional.md
  - modify: .docs/stitch/DESIGN.telegram.md
  - create: .docs/raw/decisiones/2026-04-10-wave-1-visual-gate-matrix.md
  - read: .docs/wiki/23_uxui/VOICE/VOICE-*.md
  - read: .docs/wiki/23_uxui/UXS/UXS-*.md
  - read: .docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-*.md
complexity: high
done_when: "Every visible slice is explicitly classified as green, rerun-required, manual-audit-pending, or blocked, and no UI implementation task has to guess whether `UI-RFC-*` can open."
```

## Reference
- `.docs/wiki/23_uxui/INDEX.md` — authoritative per-slice gate table
- `.docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md` — current strict Stitch gate
- `.docs/stitch/STITCH-ARTIFACTS-AUDIT.md` — current coverage and failure causes

## Prompt
Resolve the slice-level visual ambiguity without inventing fake UX validation or opening `UI-RFC-*` prematurely.

1. Preserve the current rule set: global visual layer stays closed, no new `UX-VALIDATION-*`, no new `UI-RFC-*` in this task.
2. Rebuild the per-slice gate matrix using the existing evidence:
   - `ONB-001`, `REG-001`, `REG-002` have coverage but failed manual audit
   - remaining slices are pending audit or need rerun with design-pack-derived inputs
3. Separate three states clearly:
   - full coverage but manual audit failed
   - rerun required because the last usable run did not use the derived design pack
   - pending manual audit after a valid rerun
4. Document the exact rerun prerequisites for each blocked family: corrected copy, corrected hierarchy, quota availability, runner behavior, and artifact acceptance criteria.
5. Freeze exact pass/fail criteria for opening a future `UI-RFC-*`:
   - required Stitch coverage
   - required manual visual audit checks
   - required slice docs that must agree
6. Produce an explicit execution queue for reruns so the frontend wave can reference it without rethinking priorities.
7. Record any double authority or ambiguous design source as a defect and resolve it inside the docs before closure.
8. Ensure the outcome is usable by T05 and T08: those tasks should be able to depend on a yes/no gate per slice rather than prose.

## Execution Waves
### Wave A — Evidence normalization
- Consolidate current Stitch artifact status and correct any stale wording about coverage vs approval.
- Confirm which slices need a design-pack-derived rerun before any manual audit.

### Wave B — Manual audit gate model
- Freeze the checklist used to approve or block a slice visually.
- Capture the exact reasons core slices are blocked and what must change before rerun.

### Wave C — Implementation handoff gate
- Update `23_uxui/INDEX.md` and `UI-RFC-INDEX.md` with the final gate matrix and rerun order.
- Emit one decision note summarizing the operational gate state for Wave 1.

## Skeleton
```text
Gate states:
- Green for future UI-RFC
- Rerun required
- Audit pending
- Blocked by manual findings
```

## Verify
`git diff -- .docs/stitch .docs/wiki/23_uxui` -> every slice has one explicit gate state and one explicit next action.

## Commit
`docs(plan): implement T02 visual gate matrix`

