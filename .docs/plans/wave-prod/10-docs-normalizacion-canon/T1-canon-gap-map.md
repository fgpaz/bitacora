# Task T1: Canon Gap Map

## Shared Context
**Goal:** Normalize the canon from the current repo truth before any remaining implementation work.  
**Stack:** Markdown wiki, .NET 10 backend, PostgreSQL, Supabase, Telegram, `mi-lsp`.  
**Architecture:** The repo currently exposes auth, consent, and registro endpoints only; `CareLink`, `TelegramSession`, and `frontend/` are still absent.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-docs
files:
  - create: .docs/raw/decisiones/2026-04-10-wave-prod-canon-gap-map.md
  - read: .docs/wiki/02_arquitectura.md
  - read: .docs/wiki/05_modelo_datos.md
  - read: .docs/wiki/07_baseline_tecnica.md
  - read: .docs/wiki/09_contratos_tecnicos.md
  - read: .docs/wiki/06_matriz_pruebas_RF.md
  - read: .docs/plans/wave-prod/INDEX.md
complexity: medium
done_when: "A gap map exists under .docs/raw/decisiones/2026-04-10-wave-prod-canon-gap-map.md and explicitly lists implemented truth, pending scope, precedence, and docs to normalize"
```

## Reference
`src/Bitacora.Api/Program.cs` — current runtime only maps auth, consent, and registro endpoints.  
`src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs` — current materialized entities.  
`.docs/raw/plans/wave-1/` — historical portfolio to reference but not reactivate.

## Prompt
Create a durable canon gap map before editing any canonical wiki file. Start with `mi-lsp workspace status humor --format toon`, then use `mi-lsp nav search "MapAuthEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "MapConsentEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "MapRegistroEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "CareLink" --workspace humor --format toon`, and `mi-lsp nav search "TelegramSession" --workspace humor --format toon` to capture the current backend truth directly from `src/`. Compare that evidence against `.docs/wiki/02_arquitectura.md`, `.docs/wiki/05_modelo_datos.md`, `.docs/wiki/07_baseline_tecnica.md`, `.docs/wiki/09_contratos_tecnicos.md`, `.docs/wiki/06_matriz_pruebas_RF.md`, and the new `.docs/plans/wave-prod/INDEX.md`. Write `.docs/raw/decisiones/2026-04-10-wave-prod-canon-gap-map.md` with these sections: implemented repo truth, documented pending scope, precedence (`wave-prod` active, `wave-1` historical), normalization targets, and explicit non-goals. Do not change the canonical wiki yet; this task is only the durable gap map.

## Skeleton
```md
# Wave-Prod Canon Gap Map
## Implemented Repo Truth
## Pending Scope Still Not In Code
## Active Precedence
## Normalization Targets
## Non-Goals
```

## Verify
`git diff -- .docs/raw/decisiones/2026-04-10-wave-prod-canon-gap-map.md` -> the gap map exists and names the exact docs to normalize

## Commit
`docs(plan): record wave-prod canon gap map`
