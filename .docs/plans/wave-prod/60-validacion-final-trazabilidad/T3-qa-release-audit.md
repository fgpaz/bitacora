# Task T3: QA + Release Audit

## Shared Context
**Goal:** Produce the final QA verdict tied to code, contracts, runtime hardening, and captured evidence.  
**Stack:** Read-only QA audit, validation artifacts, `mi-lsp`.  
**Architecture:** This task does not generate product code; it decides whether the implementation plus evidence is release-ready.

## Task Metadata
```yaml
id: T3
depends_on: [T1, T2]
agent_type: ps-qa
files:
  - read: src/**
  - read: frontend/**
  - read: infra/**
  - read: artifacts/e2e/2026-04-10-wave-prod-web-validation/**
  - read: artifacts/e2e/2026-04-10-wave-prod-telegram-validation/**
  - read: .docs/wiki/**
complexity: high
done_when: "The task response returns a severity-ordered QA verdict with clear go/no-go recommendation and evidence references"
```

## Reference
`.docs/wiki/06_matriz_pruebas_RF.md` — test/readiness frame.  
`.docs/wiki/21_matriz_validacion_ux.md` — validation matrix to compare against.

## Prompt
Run the final QA audit after evidence capture is complete. Review backend code, frontend code, runtime docs, and the artifact folders created by T1 and T2. Use `mi-lsp` for targeted code navigation under `src/` when auditing backend claims. Return a severity-ordered verdict that covers correctness, security, privacy, operability, and remaining validation gaps, and conclude with a clear go/no-go recommendation. Do not modify files in this task.

## Skeleton
```md
## Critical findings
## High findings
## Medium findings
## Release recommendation
```

## Verify
`Read-only QA response includes severity ordering, evidence references, and release recommendation` -> audit completed

## Commit
`No commit (read-only QA audit task)`
