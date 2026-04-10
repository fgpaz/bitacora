# Task T1: Frontend Scaffold

## Shared Context
**Goal:** Create the base `frontend/` workspace and app shell for the new web runtime.  
**Stack:** Next.js 16, React 19, TypeScript, `mi-lsp`.  
**Architecture:** The scaffold must align with the documented frontend system design and consume the existing backend instead of inventing its own API shape.

## Task Metadata
```yaml
id: T1
depends_on: []
agent_type: ps-next-vercel
files:
  - create: frontend/package.json
  - create: frontend/next.config.ts
  - create: frontend/tsconfig.json
  - create: frontend/app/layout.tsx
  - create: frontend/app/page.tsx
  - create: frontend/app/globals.css
  - create: frontend/lib/config.ts
  - read: .docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md
complexity: high
done_when: "frontend exists and npm run build from frontend exits 0"
```

## Reference
`.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` — technical frontend baseline.  
`.docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md` — first slice that the shell must support.

## Prompt
Create the initial Next.js 16 application under `frontend/`. Before scaffolding, use `mi-lsp workspace status humor --format toon`, `mi-lsp nav search "MapAuthEndpoints" --include-content --workspace humor --format toon`, and `mi-lsp nav search "TECH-FRONTEND-SYSTEM-DESIGN" --include-content --workspace humor --format toon` to re-anchor the frontend around the current backend contract and the frontend system design. Create a clean app shell, base config, environment contract, and root layout ready for patient and professional route groups. Do not implement route-specific business logic yet, and do not skip the buildable foundation.

## Skeleton
```tsx
export default function RootLayout({ children }: { children: React.ReactNode }) {
  return <html lang="es"><body>{children}</body></html>;
}
```

## Verify
`npm run build` -> `Build completed successfully`

## Commit
`feat(web): scaffold Next.js frontend foundation`
