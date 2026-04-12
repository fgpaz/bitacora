# Task T3: Technical Canon Sync

## Shared Context
**Goal:** Align the technical baseline, DB layer, and contract layer with the real repo state and remaining implementation seams.  
**Stack:** Markdown wiki, .NET 10, PostgreSQL, Dokploy, Supabase, Telegram, `mi-lsp`.  
**Architecture:** The backend is live and truthful, but the technical canon must stop implying missing code already exists.

## Task Metadata
```yaml
id: T3
depends_on: [T1]
agent_type: ps-docs
files:
  - modify: .docs/wiki/07_baseline_tecnica.md
  - modify: .docs/wiki/07_tech/TECH-*.md
  - modify: .docs/wiki/08_modelo_fisico_datos.md
  - modify: .docs/wiki/08_db/DB-MIGRACIONES-Y-BOOTSTRAP.md
  - modify: .docs/wiki/09_contratos_tecnicos.md
  - modify: .docs/wiki/09_contratos/CT-*.md
  - read: .docs/raw/decisiones/2026-04-10-wave-prod-canon-gap-map.md
complexity: high
done_when: "07/08/09 and detailed technical docs describe current runtime truth, deferred interfaces, and upcoming implementation seams without contradiction"
```

## Reference
`src/Bitacora.Api/Endpoints/Auth/AuthEndpoints.cs` — current API contract pattern.  
`src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs` — existing consent routing.  
`src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs` — current persistence baseline.

## Prompt
Normalize the technical canon in place. Start with `mi-lsp workspace list --format toon`, validate the workspace with `mi-lsp workspace status "/mnt/c/repos/mios/humor" --format toon`, then inspect the repo using `mi-lsp nav search "AuthEndpoints" --include-content --workspace "/mnt/c/repos/mios/humor" --format toon`, `mi-lsp nav search "ConsentEndpoints" --include-content --workspace "/mnt/c/repos/mios/humor" --format toon`, `mi-lsp nav search "CurrentAuthenticatedPatientResolver" --include-content --workspace "/mnt/c/repos/mios/humor" --format toon`, and `mi-lsp nav search "AppDbContext" --include-content --workspace "/mnt/c/repos/mios/humor" --format toon`. If `mi-lsp` returns `hint` or `next_hint`, follow that rerun guidance before falling back. Update `.docs/wiki/07_baseline_tecnica.md`, `.docs/wiki/07_tech/TECH-*.md`, `.docs/wiki/08_modelo_fisico_datos.md`, `.docs/wiki/08_db/DB-MIGRACIONES-Y-BOOTSTRAP.md`, `.docs/wiki/09_contratos_tecnicos.md`, and existing `CT-*` docs so that current runtime, future interfaces, deployment truth, and deferred domain pieces are separated cleanly. Create new `CT-*` or `TECH-*` docs only if the existing layer cannot hold the missing contract surface cleanly. Keep the write-up explicit about `frontend/` not existing yet and `CareLink` / `TelegramSession` being future implementation work.

## Skeleton
```md
## Runtime actual
## Interfaces pendientes
## Reglas fail-closed
## Dependencias de implementación
```

## Verify
`git diff -- .docs/wiki/07_baseline_tecnica.md .docs/wiki/07_tech .docs/wiki/08_modelo_fisico_datos.md .docs/wiki/08_db .docs/wiki/09_contratos_tecnicos.md .docs/wiki/09_contratos` -> technical canon reflects implemented truth and pending seams

## Commit
`docs(spec): normalize technical canon for wave-prod`
