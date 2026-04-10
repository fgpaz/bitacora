# Task T4: TP + Runbook Crosslinks

## Shared Context
**Goal:** Close the matrix and runbook crosslinks so execution and validation phases inherit a truthful canonical map.  
**Stack:** Markdown wiki, infra runbooks, `mi-lsp`.  
**Architecture:** This task closes the documentation loop after functional and technical normalization.

## Task Metadata
```yaml
id: T4
depends_on: [T2, T3]
agent_type: ps-docs
files:
  - modify: .docs/wiki/06_matriz_pruebas_RF.md
  - modify: .docs/wiki/06_pruebas/TP-*.md
  - modify: infra/README.md
  - modify: infra/runbooks/*.md
  - read: .docs/plans/wave-prod/INDEX.md
complexity: medium
done_when: "The TP matrix, TP docs, and runbooks reference the normalized canon and the active wave-prod portfolio without stale wave-1 assumptions"
```

## Reference
`.docs/wiki/06_pruebas/TP-ONB.md` — pattern for slice-level TP docs.  
`infra/runbooks/production-bootstrap.md` — current operational baseline from the closed bootstrap.

## Prompt
Close the crosslinks after T2 and T3 finish. Review `.docs/wiki/06_matriz_pruebas_RF.md`, all `TP-*.md` files, `infra/README.md`, and `infra/runbooks/*.md`. Use `mi-lsp nav search "MapAuthEndpoints" --include-content --workspace humor --format toon` and `mi-lsp nav search "MapRegistroEndpoints" --include-content --workspace humor --format toon` if you need to re-verify the current runtime while writing test expectations. Update the TP layer so pending slices point to the future code phases, already-implemented backend surfaces keep truthful expectations, and final UI validation is clearly deferred to the last portfolio phase. Update runbooks only where crosslinks or execution order changed; do not reopen the already-closed production bootstrap as pending work.

## Skeleton
```md
## Cobertura actual
## Cobertura diferida
## Dependencias por fase
```

## Verify
`git diff -- .docs/wiki/06_matriz_pruebas_RF.md .docs/wiki/06_pruebas infra` -> TP docs and runbooks point to the normalized canon and wave-prod sequence

## Commit
`docs(spec): synchronize TP matrix and runbook crosslinks`
