# Task T09: Security, Audit, Dependency, and Operational Hardening

## Shared Context
**Goal:** Close the cross-cutting gaps that would make a prod-first release unsafe even if the features exist.
**Stack:** .NET 10, Next.js 16, PostgreSQL, Supabase Auth, OpenTelemetry, Dokploy.
**Architecture:** This task hardens the already-built system in place. It owns consistency, dependency risk, envelopes, traceability, and fail-closed behavior across modules.

## Task Metadata
```yaml
id: T09
depends_on:
  - T05
  - T06
  - T07
  - T08
agent_type: ps-dotnet10
files:
  - modify: src/Bitacora.Api/Program.cs
  - modify: src/Bitacora.Api/Middleware/ApiExceptionMiddleware.cs
  - modify: src/Bitacora.Api/Middleware/TraceIdMiddleware.cs
  - modify: src/Bitacora.Api/Middleware/ConsentRequiredMiddleware.cs
  - modify: src/Bitacora.Api/appsettings.json
  - modify: src/Bitacora.Application/Bitacora.Application.csproj
  - modify: src/Bitacora.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs
  - modify: src/Bitacora.Infrastructure/Services/ExternalJwtService.cs
  - modify: frontend/package.json
  - modify: frontend/lib/api/bitacora-api.ts
  - modify: .docs/wiki/07_baseline_tecnica.md
  - modify: .docs/wiki/09_contratos_tecnicos.md
  - read: .docs/wiki/07_tech/TECH-CIFRADO.md
  - read: .docs/wiki/09_contratos/CT-AUDIT.md
complexity: high
done_when: "Cross-cutting security, traceability, dependency, and fail-closed concerns are explicitly closed for prod-first release."
```

## Reference
- `.docs/wiki/07_baseline_tecnica.md` — operational invariants
- `.docs/wiki/09_contratos_tecnicos.md` — common error envelope and auth rules
- `src/Bitacora.Application/Bitacora.Application.csproj` — current dependency warnings surface

## Prompt
Harden the full system for a single production release.

1. Audit and fix dependency risks surfaced during planning, including the current `Scriban 6.2.0` warnings.
2. Make sure every user-visible and operator-visible path preserves:
   - `trace_id`
   - common error envelope
   - fail-closed behavior on auth/consent/security failures
3. Close seams that earlier tasks may leave open:
   - inconsistent backend/frontend timeouts
   - missing audit writes for professional reads or exports
   - mismatched env/default handling between web and API
   - incomplete secret validation on boot
4. Revisit startup behavior for production:
   - migrations not auto-applied on boot
   - hosted services gated by explicit env/config
   - webhook/reminder startup failures surfaced clearly
5. Ensure prod-first rollback criteria are explicit for API, web, DB, and Telegram runtime.
6. Update technical docs when hardening changes observable behavior or defaults.

## Execution Waves
### Wave A — Dependency and boot hardening
- Remove or upgrade risky dependencies and tighten startup configuration validation.

### Wave B — Traceability and audit
- Ensure all critical paths write audit evidence and propagate `trace_id`.

### Wave C — Prod-first guardrails
- Align runtime defaults, timeout budgets, rollback hooks, and documentation.

## Skeleton
```text
Hardening checklist:
- dependency audit clean
- boot validation fail closed
- audit coverage complete
- trace_id consistent
- rollback/runbook updated
```

## Verify
`dotnet build src/Bitacora.sln -c Release` -> build succeeds without unresolved dependency/security drift called out by the plan.

## Commit
`chore(bitacora): harden production security and operational seams`

