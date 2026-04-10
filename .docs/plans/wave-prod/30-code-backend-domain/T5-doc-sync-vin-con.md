# Task T5: Canon Sync for VIN/CON

## Shared Context
**Goal:** Synchronize the canonical docs after the vínculo backend core lands.  
**Stack:** Markdown wiki, .NET 10 backend, `mi-lsp`.  
**Architecture:** Backend code is not complete until the canon reflects the new public behavior and schema.

## Task Metadata
```yaml
id: T5
depends_on: [T4]
agent_type: ps-docs
files:
  - modify: .docs/wiki/03_FL/FL-VIN-*.md
  - modify: .docs/wiki/04_RF/RF-VIN-*.md
  - modify: .docs/wiki/04_RF/RF-CON-*.md
  - modify: .docs/wiki/05_modelo_datos.md
  - modify: .docs/wiki/08_modelo_fisico_datos.md
  - modify: .docs/wiki/09_contratos_tecnicos.md
  - modify: .docs/wiki/09_contratos/CT-VINCULOS.md
complexity: medium
done_when: "VIN/CON flows, RF, data model, DB docs, and contract docs reflect the implemented backend core"
```

## Reference
`.docs/wiki/04_RF/RF-VIN-001.md` — functional contract anchor for vínculo behavior.  
`.docs/wiki/09_contratos/CT-VINCULOS.md` — contract layer to keep in sync with the API.

## Prompt
After the code lands, sync the canon. Re-verify the implemented surface with `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "VinculosEndpoints" --include-content --workspace humor --format toon`, and `mi-lsp nav search "CareLink" --include-content --workspace humor --format toon` before editing docs. Then update the impacted `FL-VIN-*`, `RF-VIN-*`, `RF-CON-*`, `05_modelo_datos.md`, `08_modelo_fisico_datos.md`, and contract docs so the written behavior matches the implementation exactly. Do not add scope that the code did not actually deliver. If the implementation differs from the earlier documentation assumptions, prefer the reviewed code plus approved hardening rules and state the delta explicitly.

## Skeleton
```md
## Implementado
## Restricciones vigentes
## Diferidos
```

## Verify
`git diff -- .docs/wiki/03_FL .docs/wiki/04_RF .docs/wiki/05_modelo_datos.md .docs/wiki/08_modelo_fisico_datos.md .docs/wiki/09_contratos_tecnicos.md .docs/wiki/09_contratos` -> docs match the implemented VIN/CON backend

## Commit
`docs(spec): synchronize vínculo backend canon`
