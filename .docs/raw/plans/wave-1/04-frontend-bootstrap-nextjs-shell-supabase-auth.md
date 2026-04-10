# Task T04: Frontend Bootstrap, Next.js Shell, and Shared Supabase Auth

## Shared Context
**Goal:** Create the missing web runtime foundation that all patient and professional slices will sit on.
**Stack:** Next.js 16, React 19, Supabase Auth, Dokploy.
**Architecture:** Add a new `frontend/` app at repo root, wired to the existing backend and the shared auth provider. This task owns shell, auth boundary, routing skeleton, and production deploy fit, but not slice-specific UX.

## Task Metadata
```yaml
id: T04
depends_on:
  - T01
agent_type: ps-next-vercel
files:
  - create: frontend/package.json
  - create: frontend/next.config.ts
  - create: frontend/tsconfig.json
  - create: frontend/app/layout.tsx
  - create: frontend/app/page.tsx
  - create: frontend/app/(auth)/login/page.tsx
  - create: frontend/lib/supabase/client.ts
  - create: frontend/lib/supabase/server.ts
  - create: frontend/lib/auth/session.ts
  - create: frontend/lib/api/bitacora-api.ts
  - create: frontend/components/app-shell/AppShell.tsx
  - create: frontend/components/auth/SessionBoundary.tsx
  - create: frontend/middleware.ts
  - create: frontend/.env.example
  - read: .docs/wiki/07_baseline_tecnica.md
  - read: .docs/wiki/09_contratos_tecnicos.md
complexity: high
done_when: "A production-ready `frontend/` skeleton exists on paper with auth, shell, API wiring, and Dokploy fit defined well enough for a fresh agent to build it without making architecture decisions."
```

## Reference
- `.docs/wiki/02_arquitectura.md` — web channel and shared auth responsibility
- `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` — technical UI system baseline
- `src/Bitacora.Api/appsettings.json` — backend auth and API assumptions

## Prompt
Bootstrap the entire web app foundation. Do not start slice-specific screens beyond placeholders.

1. Create a new `frontend/` workspace at repo root; do not mix Next.js files into `src/`.
2. Use Next.js 16 with App Router and production-oriented defaults.
3. Wire shared Supabase Auth:
   - server and browser clients
   - session hydration and refresh boundaries
   - route protection for patient/professional areas
   - graceful handling for missing session or revoked backend access
4. Create the minimal app shell:
   - root layout
   - loading/error/not-found boundaries
   - authenticated shell frame
   - unauthenticated login/entry flow
5. Create a typed API client layer that talks to the existing backend endpoints and preserves `trace_id` headers / error envelopes.
6. Freeze the environment contract needed by production Dokploy:
   - public base URL
   - server-side backend URL
   - Supabase URL / publishable key
   - auth cookie/session expectations
7. Keep slice UI generic here; T05 and T08 own actual patient/professional workflows.
8. Document any auth mismatch between Supabase session state and backend bootstrap expectations instead of improvising around it.

## Execution Waves
### Wave A — Scaffold
- Create the `frontend/` workspace, App Router shell, and environment contract.

### Wave B — Shared auth and API
- Add Supabase clients, session boundary, protected route model, and typed API client.

### Wave C — Production fit
- Align build/start commands, Dokploy expectations, and deploy-time environment usage.

## Skeleton
```text
frontend/
  app/
    layout.tsx
    page.tsx
    (auth)/login/page.tsx
  lib/
    supabase/
    auth/
    api/
  components/
    app-shell/
    auth/
```

## Verify
`npm run build --prefix frontend` -> build completes without unresolved environment or routing decisions.

## Commit
`feat(frontend): bootstrap nextjs shell and shared auth`

