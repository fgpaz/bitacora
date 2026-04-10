# Task T2: Auth / Error / Consent Contract Freeze

## Shared Context
**Goal:** Freeze the public and cross-cutting contracts before code expands them.  
**Stack:** Markdown specs, .NET 10 target backend, Next.js 16 target frontend, `mi-lsp`.  
**Architecture:** Backend and frontend phases need a stable auth, error, and consent contract layer.

## Task Metadata
```yaml
id: T2
depends_on: [T1]
agent_type: ps-docs
files:
  - modify: .docs/wiki/09_contratos_tecnicos.md
  - modify: .docs/wiki/09_contratos/CT-AUTH.md
  - modify: .docs/wiki/09_contratos/CT-ERRORS.md
  - create: .docs/wiki/09_contratos/CT-VINCULOS.md
  - create: .docs/wiki/09_contratos/CT-VISUALIZACION-Y-EXPORT.md
  - create: .docs/wiki/09_contratos/CT-TELEGRAM-RUNTIME.md
  - modify: .docs/wiki/04_RF/RF-CON-*.md
  - modify: .docs/wiki/04_RF/RF-VIN-*.md
  - modify: .docs/wiki/04_RF/RF-VIS-*.md
  - modify: .docs/wiki/04_RF/RF-EXP-*.md
  - modify: .docs/wiki/04_RF/RF-TG-*.md
complexity: high
done_when: "The contract layer freezes auth, errors, consent, vínculo, visualization/export, and Telegram public interfaces before code starts"
```

## Reference
`src/Bitacora.Api/Endpoints/Auth/AuthEndpoints.cs` — current auth route pattern.  
`src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs` — current consent contract pattern.  
`.docs/wiki/04_RF/RF-VIN-*.md` and `.docs/wiki/04_RF/RF-TG-*.md` — required behavior to map into contracts.

## Prompt
Freeze the public contract layer. Re-verify the current backend shape with `mi-lsp nav search "AuthEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "ConsentEndpoints" --include-content --workspace humor --format toon`, and `mi-lsp nav search "ApiExceptionMiddleware" --include-content --workspace humor --format toon`. Then update `CT-AUTH.md`, `CT-ERRORS.md`, and `09_contratos_tecnicos.md`, and create the missing dedicated contract docs for vínculo, visualization/export, and Telegram runtime. Every contract must specify request/response ownership, typed errors, authorization rules, fail-closed defaults, and what remains deferred. Sync the impacted `RF-*` docs so the functional layer and contract layer read the same truth.

## Skeleton
```md
## Endpoint surface
## Authz and consent rules
## Error taxonomy
## Deferred capabilities
```

## Verify
`git diff -- .docs/wiki/09_contratos_tecnicos.md .docs/wiki/09_contratos .docs/wiki/04_RF` -> contract surfaces are explicit and frozen

## Commit
`docs(spec): freeze public contracts before implementation`
