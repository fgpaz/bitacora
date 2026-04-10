# Task T4: Canon Sync for Professional Web

## Shared Context
**Goal:** Synchronize the canon after the professional web surface lands.  
**Stack:** Markdown wiki, Next.js 16 web runtime, `mi-lsp`.  
**Architecture:** Professional UI code must feed back into the docs and test layers without claiming final validation yet.

## Task Metadata
```yaml
id: T4
depends_on: [T2, T3]
agent_type: ps-docs
files:
  - modify: .docs/wiki/06_pruebas/TP-VIN.md
  - modify: .docs/wiki/06_pruebas/TP-VIS.md
  - modify: .docs/wiki/06_pruebas/TP-EXP.md
  - modify: .docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md
  - modify: .docs/wiki/09_contratos/CT-VINCULOS.md
  - modify: .docs/wiki/09_contratos/CT-VISUALIZACION-Y-EXPORT.md
complexity: medium
done_when: "Professional web implementation details are reflected in TP and technical docs without marking final validation as complete"
```

## Reference
`.docs/wiki/06_pruebas/TP-VIN.md`, `.docs/wiki/06_pruebas/TP-VIS.md`, `.docs/wiki/06_pruebas/TP-EXP.md` — test-plan anchors for this surface.

## Prompt
After the professional web screens land, update the canon where exact implementation details changed the technical or test assumptions. Before editing, use `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "frontend/app/(professional)" --workspace humor --format toon`, `mi-lsp nav search "VinculosEndpoints" --include-content --workspace humor --format toon`, and `mi-lsp nav search "VisualizacionEndpoints" --include-content --workspace humor --format toon` to re-anchor the sync in the implemented source. Then sync TP docs, frontend system design, and the dedicated contract docs for vínculo and visualization/export. Do not close the UX validation loop here; keep that timing reserved for the final phase.

## Skeleton
```md
## Implementación web actual
## Supuestos de prueba
## Pendiente para validación final
```

## Verify
`git diff -- .docs/wiki/06_pruebas .docs/wiki/07_tech .docs/wiki/09_contratos` -> docs reflect professional web implementation without premature validation

## Commit
`docs(spec): synchronize professional web canon`
