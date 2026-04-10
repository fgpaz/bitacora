# Task T5: Canon Sync for VIS / EXP / TG

## Shared Context
**Goal:** Synchronize the canon after the read/export/Telegram backend surfaces land.  
**Stack:** Markdown wiki, .NET 10 backend, `mi-lsp`.  
**Architecture:** Backend implementation is incomplete until the canon reflects actual read-side, export, and Telegram behavior.

## Task Metadata
```yaml
id: T5
depends_on: [T4]
agent_type: ps-docs
files:
  - modify: .docs/wiki/03_FL/FL-TG-*.md
  - modify: .docs/wiki/03_FL/FL-VIS-*.md
  - modify: .docs/wiki/04_RF/RF-TG-*.md
  - modify: .docs/wiki/04_RF/RF-VIS-*.md
  - modify: .docs/wiki/04_RF/RF-EXP-*.md
  - modify: .docs/wiki/05_modelo_datos.md
  - modify: .docs/wiki/09_contratos/CT-TELEGRAM-RUNTIME.md
  - modify: .docs/wiki/09_contratos/CT-VISUALIZACION-Y-EXPORT.md
complexity: medium
done_when: "VIS/EXP/TG flows, RF docs, data model, and contracts reflect the implemented backend behavior"
```

## Reference
`.docs/wiki/04_RF/RF-TG-001.md` — Telegram requirements anchor.  
`.docs/wiki/04_RF/RF-VIS-001.md` and `.docs/wiki/04_RF/RF-EXP-001.md` — read/export requirements anchors.

## Prompt
Review the implemented diff and update the canon to match it exactly. Re-verify the backend surface with `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "TelegramEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "VisualizacionEndpoints" --include-content --workspace humor --format toon`, and `mi-lsp nav search "ExportEndpoints" --include-content --workspace humor --format toon` before editing. Sync the affected `FL-TG-*`, `FL-VIS-*`, `RF-TG-*`, `RF-VIS-*`, `RF-EXP-*`, `05_modelo_datos.md`, and the dedicated contract docs for Telegram and visualization/export. Be explicit about any remaining deferred pieces instead of silently broadening the implementation claim. If the code deliberately narrowed a behavior for privacy or operational safety, preserve that narrowing in the docs.

## Skeleton
```md
## Implementado
## Restricciones operativas
## Pendientes explícitos
```

## Verify
`git diff -- .docs/wiki/03_FL .docs/wiki/04_RF .docs/wiki/05_modelo_datos.md .docs/wiki/09_contratos` -> VIS/EXP/TG docs match the implemented backend

## Commit
`docs(spec): synchronize read, export, and telegram backend canon`
