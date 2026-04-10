# Task T1: Professional Shell + Guards

## Shared Context
**Goal:** Create the professional route group, shell, and access guards on top of the existing frontend foundation.  
**Stack:** Next.js 16, React 19, Supabase Auth, `mi-lsp`.  
**Architecture:** Professional screens need a dedicated shell and role-aware guard layer before feature routes are added.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-next-vercel
files:
  - create: frontend/app/(professional)/layout.tsx
  - create: frontend/app/(professional)/page.tsx
  - create: frontend/components/professional/*.tsx
  - modify: frontend/lib/auth/*.ts
  - modify: frontend/lib/session/*.ts
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-001.md
complexity: high
done_when: "frontend builds and the professional route group plus role-aware guard shell exist"
```

## Reference
`.docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-001.md` — initial professional-facing slice contract.  
`.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` — route and component system design baseline.

## Prompt
Create the professional web shell on top of the Phase 40 frontend foundation. Use `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "CT-AUTH" --include-content --workspace humor --format toon`, and `mi-lsp nav search "UI-RFC-VIN-001" --workspace humor --format toon` to re-anchor around role, contract, and UX/UI rules. Add the professional route group, layout, entry page, and role-aware guards using the shared auth/session layer. Do not implement the feature-specific views yet.

## Skeleton
```tsx
export default function ProfessionalLayout({ children }: { children: React.ReactNode }) {
  return <section>{children}</section>;
}
```

## Verify
`npm run build` -> `Build completed successfully`

## Commit
`feat(web): add professional shell and access guards`
