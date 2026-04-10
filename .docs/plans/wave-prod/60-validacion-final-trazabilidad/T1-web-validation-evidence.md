# Task T1: Web Validation Evidence

## Shared Context
**Goal:** Capture final evidence for patient and professional web validation after implementation is complete.  
**Stack:** Browser/E2E execution, artifacts, UX/UI canon, `mi-lsp`.  
**Architecture:** This is the first phase allowed to treat web UX validation as real evidence rather than prepared intent.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-worker
files:
  - create: artifacts/e2e/2026-04-10-wave-prod-web-validation/
  - read: frontend/**
  - read: .docs/wiki/23_uxui/UI-RFC/*.md
  - read: .docs/wiki/23_uxui/HANDOFF-*/**/*.md
  - read: .docs/wiki/21_matriz_validacion_ux.md
complexity: high
done_when: "Validation artifacts for patient and professional web flows exist under artifacts/e2e/2026-04-10-wave-prod-web-validation/"
```

## Reference
`.docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md` — onboarding contract to validate against.  
`.docs/wiki/23_uxui/UI-RFC/UI-RFC-REG-001.md`, `.docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-001.md`, `.docs/wiki/23_uxui/UI-RFC/UI-RFC-VIS-001.md` — additional web validation anchors.

## Prompt
Run the final web validation pass only after all code and runtime hardening are complete. Capture evidence under `artifacts/e2e/2026-04-10-wave-prod-web-validation/` for the patient and professional flows defined by the active UX/UI contracts. Before executing, use `mi-lsp workspace status humor --format toon` and inspect the relevant `UI-RFC`/handoff docs so the evidence is judged against the intended contracts rather than memory. Save screenshots, notes, and any reproducible error evidence needed for the final validation docs. If a critical issue appears, capture it instead of masking it.

## Skeleton
```text
artifacts/e2e/2026-04-10-wave-prod-web-validation/
  - summary.md
  - screenshots/
  - defects.md
```

## Verify
`Test-Path artifacts/e2e/2026-04-10-wave-prod-web-validation` -> `True`

## Commit
`chore(qa): capture final web validation evidence`
