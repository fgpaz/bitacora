# Task T4: UX-VALIDATION + Go/No-Go Sync

## Shared Context
**Goal:** Synchronize the final validation evidence into the canon and record the release decision.  
**Stack:** Markdown validation docs, evidence artifacts, release governance, `mi-lsp`.  
**Architecture:** This is the only task allowed to update `UX-VALIDATION` from prepared to evidence-backed status.

## Task Metadata
```yaml
id: T4
depends_on: [T3]
agent_type: ps-docs
files:
  - modify: .docs/wiki/21_matriz_validacion_ux.md
  - create: .docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-ONB-001.md
  - create: .docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-REG-001.md
  - create: .docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-REG-002.md
  - create: .docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-001.md
  - create: .docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIS-001.md
  - create: .docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-EXP-001.md
  - create: .docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-TG-001.md
  - modify: .docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-INDEX.md
  - modify: .docs/wiki/06_matriz_pruebas_RF.md
  - create: .docs/raw/decisiones/2026-04-10-wave-prod-release-readiness.md
complexity: high
done_when: "The validation matrix, UX-VALIDATION docs, and release decision are synchronized to the captured evidence and QA verdict"
```

## Reference
`artifacts/e2e/2026-04-10-wave-prod-web-validation/summary.md` — web evidence source.  
`artifacts/e2e/2026-04-10-wave-prod-telegram-validation/summary.md` — Telegram evidence source.  
`.docs/wiki/21_matriz_validacion_ux.md` — status matrix to update.

## Prompt
Take the evidence from T1/T2 and the QA verdict from T3 and sync them into the validation canon. Before editing, use `mi-lsp workspace status humor --format toon`, plus targeted searches such as `mi-lsp nav search "VinculosEndpoints" --include-content --workspace humor --format toon` and `mi-lsp nav search "TelegramEndpoints" --include-content --workspace humor --format toon`, to anchor the write-up in the implemented source and not only in the artifacts. Then update `.docs/wiki/21_matriz_validacion_ux.md`, create the required `UX-VALIDATION-*` docs under `23_uxui/UX-VALIDATION/`, update the validation index, and write a durable release decision note at `.docs/raw/decisiones/2026-04-10-wave-prod-release-readiness.md`. Every validated claim must cite evidence, and every unresolved issue must be carried into the release note with severity and owner. If the release is not ready, say so explicitly.

## Skeleton
```md
## Evidence reviewed
## Findings
## Validation verdict
## Release decision
```

## Verify
`git diff -- .docs/wiki/21_matriz_validacion_ux.md .docs/wiki/23_uxui/UX-VALIDATION .docs/wiki/06_matriz_pruebas_RF.md .docs/raw/decisiones/2026-04-10-wave-prod-release-readiness.md` -> validation canon and release decision reflect evidence and QA

## Commit
`docs(qa): synchronize final validation evidence and release readiness`
