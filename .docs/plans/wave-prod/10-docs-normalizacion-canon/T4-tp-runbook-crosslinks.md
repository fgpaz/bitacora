# Task T4: TP + Runbook + Policy Crosslinks

## Shared Context
**Goal:** Close the matrix and runbook crosslinks so execution and validation phases inherit a truthful canonical map.  
**Stack:** Markdown wiki, infra runbooks, `mi-lsp`.  
**Architecture:** This task closes the documentation loop after functional and technical normalization, including the project policy docs that govern exploration behavior.

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
  - modify: AGENTS.md
  - modify: CLAUDE.md
  - read: .docs/plans/wave-prod/INDEX.md
complexity: medium
done_when: "The TP matrix, TP docs, runbooks, and policy docs reference the normalized canon, the active wave-prod portfolio, and the mi-lsp-first exploration rule without stale wave-1 assumptions"
```

## Reference
`.docs/wiki/06_pruebas/TP-ONB.md` — pattern for slice-level TP docs.  
`infra/runbooks/production-bootstrap.md` — current operational baseline from the closed bootstrap.

## Prompt
Close the crosslinks after T2 and T3 finish. Review `.docs/wiki/06_matriz_pruebas_RF.md`, all `TP-*.md` files, `infra/README.md`, `infra/runbooks/*.md`, `AGENTS.md`, and `CLAUDE.md`. Start by running `mi-lsp workspace list --format toon`, then validate the workspace with `mi-lsp workspace status "/mnt/c/repos/mios/humor" --format toon`. Use `mi-lsp nav search "MapAuthEndpoints" --include-content --workspace "/mnt/c/repos/mios/humor" --format toon` and `mi-lsp nav search "MapRegistroEndpoints" --include-content --workspace "/mnt/c/repos/mios/humor" --format toon` if you need to re-verify the current runtime while writing test expectations. If `mi-lsp` returns `hint` or `next_hint`, follow that rerun guidance before falling back. Update the TP layer so pending slices point to the future code phases, already-implemented backend surfaces keep truthful expectations, and final UI validation is clearly deferred to the last portfolio phase. Synchronize `AGENTS.md` and `CLAUDE.md` so `mi-lsp` is always the first exploration tool, workspace alias/path validation is explicit, and fallback only happens after `mi-lsp` has been tried. Update runbooks only where crosslinks or execution order changed; do not reopen the already-closed production bootstrap as pending work.

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
