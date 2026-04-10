# Task T5: Canon Sync for Runtime Hardening

## Shared Context
**Goal:** Synchronize the canonical docs after runtime hardening and operational extension land.  
**Stack:** Markdown wiki, runtime/ops docs, `mi-lsp`.  
**Architecture:** Hardening work is not complete until the technical canon and TP matrix reflect the final runtime behavior.

## Task Metadata
```yaml
id: T5
depends_on: [T4]
agent_type: ps-docs
files:
  - modify: .docs/wiki/06_matriz_pruebas_RF.md
  - modify: .docs/wiki/07_baseline_tecnica.md
  - modify: .docs/wiki/07_tech/TECH-*.md
  - modify: .docs/wiki/09_contratos_tecnicos.md
  - modify: .docs/wiki/09_contratos/CT-*.md
complexity: medium
done_when: "The technical canon reflects the hardened runtime, observability, and release seams"
```

## Reference
`.docs/wiki/07_baseline_tecnica.md` — main runtime baseline.  
`.docs/wiki/09_contratos_tecnicos.md` — technical contract index.

## Prompt
After the runtime hardening diff is stable, update the technical canon and TP matrix to match it. Re-open the hardened source seams with `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "Program.cs" --include-content --workspace humor --format toon`, `mi-lsp nav search "frontend/middleware.ts" --workspace humor --format toon`, and `mi-lsp nav search "TelegramEndpoints" --include-content --workspace humor --format toon` before editing docs. Sync the baseline, detailed technical docs, contract docs, and test matrix so the written runtime and release story exactly matches what now exists in code and ops. Do not perform final validation here; only document the hardened runtime truth.

## Skeleton
```md
## Runtime endurecido
## Observabilidad y smokes
## Restricciones de release
```

## Verify
`git diff -- .docs/wiki/06_matriz_pruebas_RF.md .docs/wiki/07_baseline_tecnica.md .docs/wiki/07_tech .docs/wiki/09_contratos_tecnicos.md .docs/wiki/09_contratos` -> canon reflects runtime hardening

## Commit
`docs(spec): synchronize hardened runtime canon`
