# Task T1: Compliance Invariants

## Shared Context
**Goal:** Freeze privacy, consent, audit, and retention invariants before implementation.  
**Stack:** Markdown specs, health-data compliance context, `mi-lsp`.  
**Architecture:** Sensitive-data rules must be explicit before expanding the domain or exposing more channels.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-docs
files:
  - modify: .docs/wiki/02_arquitectura.md
  - modify: .docs/wiki/05_modelo_datos.md
  - modify: .docs/wiki/07_baseline_tecnica.md
  - modify: .docs/wiki/07_tech/TECH-CIFRADO.md
  - modify: .docs/wiki/09_contratos_tecnicos.md
  - modify: .docs/wiki/09_contratos/CT-AUDIT.md
complexity: high
done_when: "Architecture, data model, baseline, and audit contracts explicitly define privacy, consent, audit, and retention invariants for future code phases"
```

## Reference
`src/Bitacora.Api/Middleware/ConsentRequiredMiddleware.cs` — current consent enforcement seam.  
`src/Bitacora.Api/Security/CurrentAuthenticatedPatientResolver.cs` — current identity seam.  
`.docs/wiki/04_RF/RF-SEC-*.md` — security requirement set to keep aligned.

## Prompt
Harden the compliance baseline before any code expansion. Use `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "ConsentRequiredMiddleware" --include-content --workspace humor --format toon`, `mi-lsp nav search "CurrentAuthenticatedPatientResolver" --include-content --workspace humor --format toon`, and `mi-lsp nav search "AccessAudit" --include-content --workspace humor --format toon` to ground the write-up in the current runtime seams. Update the architecture, data model, technical baseline, encryption detail, and audit contract so they explicitly define: what data is sensitive, when consent is required, what must be audited, what can be exported, what must never leak to Telegram, and what retention/deletion boundaries later code must preserve.

## Skeleton
```md
## Invariantes de privacidad
## Invariantes de consentimiento
## Invariantes de auditoría
## Retención y supresión
```

## Verify
`git diff -- .docs/wiki/02_arquitectura.md .docs/wiki/05_modelo_datos.md .docs/wiki/07_baseline_tecnica.md .docs/wiki/07_tech/TECH-CIFRADO.md .docs/wiki/09_contratos_tecnicos.md .docs/wiki/09_contratos/CT-AUDIT.md` -> invariants are explicit and consistent

## Commit
`docs(spec): freeze compliance invariants for wave-prod`
