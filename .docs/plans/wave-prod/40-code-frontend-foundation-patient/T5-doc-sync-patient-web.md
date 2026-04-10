# Task T5: Canon Sync for Patient Web

## Shared Context
**Goal:** Synchronize the canon after the patient web surface lands.  
**Stack:** Markdown wiki, Next.js 16 web runtime, `mi-lsp`.  
**Architecture:** UI implementation must feed back into the technical and test canon without claiming final validation yet.

## Task Metadata
```yaml
id: T5
depends_on: [T3, T4]
agent_type: ps-docs
files:
  - modify: .docs/wiki/06_pruebas/TP-ONB.md
  - modify: .docs/wiki/06_pruebas/TP-REG.md
  - modify: .docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md
  - modify: .docs/wiki/09_contratos/CT-AUTH.md
  - modify: .docs/wiki/09_contratos/CT-VISUALIZACION-Y-EXPORT.md
  - modify: .docs/wiki/23_uxui/HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-ONB-001.md
complexity: medium
done_when: "Patient web implementation details are reflected in TP and technical docs without marking final validation as complete"
```

## Reference
`.docs/wiki/06_pruebas/TP-ONB.md` — patient onboarding TP anchor.  
`.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` — frontend tech canon to keep current.

## Prompt
Review the patient web diff and update the canon only where the implementation changed concrete technical details. Before editing, re-anchor the write-up with `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "frontend/app/(patient)" --workspace humor --format toon`, and `mi-lsp nav search "RegistroEndpoints" --include-content --workspace humor --format toon`. Sync TP docs, frontend system design, and any contract doc that now needs exact frontend assumptions, but do not mark UX validation as complete yet. Keep the final validation phase as the only place that can turn prepared slices into evidence-backed validation.

## Skeleton
```md
## Implementación web actual
## Dependencias de validación final
## Restricciones vigentes
```

## Verify
`git diff -- .docs/wiki/06_pruebas .docs/wiki/07_tech .docs/wiki/09_contratos .docs/wiki/23_uxui/HANDOFF-VISUAL-QA` -> docs reflect patient web implementation without premature validation

## Commit
`docs(spec): synchronize patient web canon`
