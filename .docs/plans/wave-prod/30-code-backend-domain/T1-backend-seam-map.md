# Task T1: Backend Seam Map

## Shared Context
**Goal:** Record the exact backend seams to reuse before adding the vínculo domain.  
**Stack:** .NET 10, EF Core, Minimal APIs, `mi-lsp`.  
**Architecture:** The repo already has stable patterns for commands, repositories, and Minimal APIs; this task maps them before code changes start.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-worker
files:
  - create: .docs/raw/decisiones/2026-04-10-wave-prod-backend-domain-seam-map.md
  - read: src/Bitacora.Api/Program.cs
  - read: src/Bitacora.Api/Endpoints/Auth/AuthEndpoints.cs
  - read: src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs
  - read: src/Bitacora.Api/Endpoints/Registro/RegistroEndpoints.cs
  - read: src/Bitacora.Application/Commands/Auth/BootstrapPatientCommand.cs
  - read: src/Bitacora.Application/Commands/Consent/GrantConsentCommand.cs
  - read: src/Bitacora.Application/Commands/Registro/CreateMoodEntryCommand.cs
  - read: src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs
complexity: medium
done_when: "A seam map exists under .docs/raw/decisiones/2026-04-10-wave-prod-backend-domain-seam-map.md with exact files, patterns, and insertion points for VIN/CON"
```

## Reference
`src/Bitacora.Application/DependencyInjection/ServiceCollectionExtensions.cs` — current application registration pattern.  
`src/Bitacora.DataAccess.EntityFramework/DependencyInjection/ServiceCollectionExtensions.cs` — current data access registration pattern.

## Prompt
Create a durable seam map before editing backend code. Use `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "BootstrapPatientCommand" --include-content --workspace humor --format toon`, `mi-lsp nav search "GrantConsentCommand" --include-content --workspace humor --format toon`, `mi-lsp nav search "GetCurrentConsentQuery" --include-content --workspace humor --format toon`, `mi-lsp nav search "AppDbContext" --include-content --workspace humor --format toon`, and `mi-lsp nav search "ServiceCollectionExtensions" --include-content --workspace humor --format toon`. Write `.docs/raw/decisiones/2026-04-10-wave-prod-backend-domain-seam-map.md` with the current seams for domain entities, repositories, DI registration, Minimal APIs, authz/audit hooks, and migration workflow. Do not modify production code in this task.

## Skeleton
```md
## Domain seams
## Application seams
## Data access seams
## API seams
## Auth and audit seams
```

## Verify
`git diff -- .docs/raw/decisiones/2026-04-10-wave-prod-backend-domain-seam-map.md` -> seam map exists and cites exact files

## Commit
`docs(plan): record backend domain seam map`
