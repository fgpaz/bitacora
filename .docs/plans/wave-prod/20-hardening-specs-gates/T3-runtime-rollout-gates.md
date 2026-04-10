# Task T3: Runtime / Rollout Gates

## Shared Context
**Goal:** Freeze fail-closed runtime and rollout rules before adding new surfaces.  
**Stack:** Markdown specs, Dokploy runbooks, .NET 10, Next.js 16, Telegram, `mi-lsp`.  
**Architecture:** Backend production already exists, so new phases need delta rollout gates rather than a blank-slate deployment narrative.

## Task Metadata
```yaml
id: T3
depends_on: [T1]
agent_type: ps-docs
files:
  - modify: .docs/wiki/07_baseline_tecnica.md
  - create: .docs/wiki/07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md
  - modify: .docs/wiki/07_tech/TECH-TELEGRAM.md
  - modify: .docs/wiki/06_matriz_pruebas_RF.md
  - modify: infra/README.md
  - modify: infra/runbooks/*.md
complexity: high
done_when: "Fail-closed behavior, rollout order, observability minimums, and operational gates are explicit for the upcoming code phases"
```

## Reference
`infra/runbooks/production-bootstrap.md` — production baseline already in place.  
`src/Bitacora.Api/Program.cs` — current middleware/order baseline for fail-closed expectations.

## Prompt
Write the runtime gates that later code and release phases must obey. Use `mi-lsp nav search "UseAuthentication" --include-content --workspace humor --format toon`, `mi-lsp nav search "UseAuthorization" --include-content --workspace humor --format toon`, and `mi-lsp nav search "ConsentRequiredMiddleware" --include-content --workspace humor --format toon` to ground the fail-closed backend story. Then update the technical baseline and operational docs so they explicitly define rollout order, minimum observability, smoke expectations, secrets/config expectations, Telegram runtime safety limits, and the rule that UI validation remains a terminal activity after code exists. Add a dedicated `TECH-ROLLOUT-Y-OPERABILIDAD.md` if the existing baseline cannot hold the operational detail cleanly.

## Skeleton
```md
## Orden de rollout
## Fail-closed runtime rules
## Observabilidad mínima
## Gating por canal y superficie
```

## Verify
`git diff -- .docs/wiki/07_baseline_tecnica.md .docs/wiki/07_tech infra .docs/wiki/06_matriz_pruebas_RF.md` -> rollout and runtime gates are explicit

## Commit
`docs(spec): freeze runtime and rollout gates`
