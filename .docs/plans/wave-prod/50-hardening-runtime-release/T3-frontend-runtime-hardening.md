# Task T3: Frontend Runtime Hardening

## Shared Context
**Goal:** Harden frontend runtime behavior, access guards, headers, and failure states.  
**Stack:** Next.js 16, React 19, auth/session layer, `mi-lsp`.  
**Architecture:** Apply runtime hardening without reopening UX design decisions already frozen in the docs.

## Task Metadata
```yaml
id: T3
depends_on: [T1]
agent_type: ps-next-vercel
files:
  - modify: frontend/app/**/*.tsx
  - modify: frontend/lib/auth/*.ts
  - modify: frontend/lib/session/*.ts
  - modify: frontend/lib/api/*.ts
  - modify: frontend/next.config.ts
  - create: frontend/middleware.ts
complexity: high
done_when: "frontend builds and the runtime hardening findings for guards, headers, and failure paths are addressed"
```

## Reference
`frontend/lib/auth/*.ts` — shared auth/session foundation from Phase 40.  
`.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` — frontend runtime rules to preserve.

## Prompt
Address the frontend/runtime findings from T1 without changing product scope. Before editing, use `mi-lsp nav search "frontend/lib/auth" --workspace humor --format toon`, `mi-lsp nav search "TECH-FRONTEND-SYSTEM-DESIGN" --include-content --workspace humor --format toon`, and inspect the relevant route files. Harden route protection, session expiry handling, error states, headers/middleware, and any client/server boundary that currently violates the fail-closed or privacy rules. Do not redesign the screens; keep the hardening technical.

## Skeleton
```ts
export function middleware() {
  return;
}
```

## Verify
`npm run build` -> `Build completed successfully`

## Commit
`fix(web): harden frontend runtime guards and failure paths`
