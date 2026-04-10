# Task T1: Runtime Gap Audit

## Shared Context
**Goal:** Run a focused runtime audit before touching hardening code.  
**Stack:** Read-only audit, .NET 10, Next.js 16, Telegram runtime, `mi-lsp`.  
**Architecture:** This task identifies concrete runtime hardening gaps so later edits stay targeted and evidence-based.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-gap-terminator
files:
  - read: src/Bitacora.Api/Program.cs
  - read: src/Bitacora.Api/Endpoints/**
  - read: frontend/**
  - read: infra/**
  - read: .docs/wiki/07_baseline_tecnica.md
  - read: .docs/wiki/09_contratos_tecnicos.md
complexity: medium
done_when: "The task response returns a severity-ordered runtime hardening gap list with exact file references and no code changes"
```

## Reference
`.docs/wiki/07_baseline_tecnica.md` — runtime baseline to audit against.  
`.docs/wiki/09_contratos_tecnicos.md` — contract layer to compare against implementation.

## Prompt
Run a read-only runtime hardening audit before any fixes. Use `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "UseAuthentication" --include-content --workspace humor --format toon`, `mi-lsp nav search "UseAuthorization" --include-content --workspace humor --format toon`, `mi-lsp nav search "ConsentRequiredMiddleware" --include-content --workspace humor --format toon`, and relevant `frontend/` / `infra/` file inspection. Return a severity-ordered list of runtime gaps covering fail-closed behavior, audit, headers, session handling, observability, and operational seams. Do not modify files in this task.

## Skeleton
```md
## Critical gaps
## High gaps
## Medium gaps
## Recommended fix order
```

## Verify
`Read-only audit response includes file references and fix order` -> audit completed without code changes

## Commit
`No commit (read-only audit task)`
