# Task T4: Exceptions Register + Go/No-Go

## Shared Context
**Goal:** Produce the single checkpoint document that future code phases must satisfy before starting and before shipping.  
**Stack:** Markdown specs, cross-canon governance, `mi-lsp`.  
**Architecture:** This task closes the documentation-first segment by turning hardening rules into executable go/no-go gates.

## Task Metadata
```yaml
id: T4
depends_on: [T2, T3]
agent_type: ps-docs
files:
  - create: .docs/raw/decisiones/2026-04-10-wave-prod-hardening-exceptions-register.md
  - modify: .docs/plans/wave-prod/INDEX.md
  - modify: .docs/wiki/07_baseline_tecnica.md
  - modify: .docs/wiki/09_contratos_tecnicos.md
complexity: medium
done_when: "A single exceptions register and go/no-go checklist exists and is linked from the active portfolio and canonical technical docs"
```

## Reference
`.docs/plans/wave-prod/INDEX.md` — active portfolio root.  
`.docs/raw/decisiones/2026-04-10-wave-prod-canon-gap-map.md` — earlier durable gap evidence.

## Prompt
Close the pre-code hardening phase with one durable decision register. Re-open the just-frozen technical seams with `mi-lsp workspace status humor --format toon` and a targeted `mi-lsp nav search "ConsentRequiredMiddleware" --include-content --workspace humor --format toon` before writing, so the final go/no-go list stays anchored in real source seams and not only in the wiki. Create `.docs/raw/decisiones/2026-04-10-wave-prod-hardening-exceptions-register.md` and link it from the active portfolio and the main technical baseline/contract index. The document must list: accepted temporary exceptions, prohibited shortcuts, required gates before backend code, required gates before frontend code, and required gates before final validation. Keep it concrete and scoped to this MVP. Do not add new product requirements here; only operationalize the rules already frozen in T1-T3.

## Skeleton
```md
## Excepciones aceptadas
## Atajos prohibidos
## Go/No-Go antes de código
## Go/No-Go antes de release
```

## Verify
`git diff -- .docs/raw/decisiones/2026-04-10-wave-prod-hardening-exceptions-register.md .docs/plans/wave-prod/INDEX.md .docs/wiki/07_baseline_tecnica.md .docs/wiki/09_contratos_tecnicos.md` -> the register exists and is cross-linked

## Commit
`docs(spec): add hardening exceptions register and go-no-go gates`
