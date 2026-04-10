# Task T4: Observability / Smokes / Ops Automation

## Shared Context
**Goal:** Extend observability, smoke coverage, and release automation to the newly-added surfaces.  
**Stack:** PowerShell smokes, runbooks, Dokploy/runtime ops, `mi-lsp`.  
**Architecture:** This task updates operational seams rather than product code.

## Task Metadata
```yaml
id: T4
depends_on: [T2, T3]
agent_type: ps-worker
files:
  - modify: infra/smoke/*.ps1
  - modify: infra/README.md
  - modify: infra/runbooks/*.md
  - modify: .docs/wiki/06_matriz_pruebas_RF.md
  - modify: .docs/wiki/07_baseline_tecnica.md
complexity: high
done_when: "Operational smokes, runbooks, and observability/release instructions cover the new backend, web, and Telegram surfaces"
```

## Reference
`infra/smoke/backend-smoke.ps1` — current smoke baseline already used in production.  
`infra/runbooks/production-bootstrap.md` — operational baseline to extend.

## Prompt
Extend the operational layer after backend and frontend hardening land. Before editing, re-verify the newly-added backend/web seams with `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "VinculosEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "VisualizacionEndpoints" --include-content --workspace humor --format toon`, and `mi-lsp nav search "TelegramEndpoints" --include-content --workspace humor --format toon`. Then update smoke scripts, runbooks, and operational docs so the new patient/professional web routes, vínculo endpoints, export endpoints, and Telegram runtime have explicit smoke coverage and operator guidance. Keep the bootstrap truth intact; this task extends the operational surface rather than rewriting it. Add or adjust commands only where the new surfaces require them.

## Skeleton
```powershell
param()
# Add new surface checks here
```

## Verify
`git diff -- infra .docs/wiki/06_matriz_pruebas_RF.md .docs/wiki/07_baseline_tecnica.md` -> operational coverage now includes the new surfaces

## Commit
`chore(ops): extend smokes and release automation for new surfaces`
