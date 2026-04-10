# Task T2: Auth / Session / API Client

## Shared Context
**Goal:** Build the shared auth/session and API client layer before implementing patient routes.  
**Stack:** Next.js 16, Supabase Auth, TypeScript, `mi-lsp`.  
**Architecture:** Every later page depends on a single truthful auth/session boundary and a stable backend client.

## Task Metadata
```yaml
id: T2
depends_on: [T1]
agent_type: ps-next-vercel
files:
  - create: frontend/lib/auth/*.ts
  - create: frontend/lib/api/*.ts
  - create: frontend/lib/session/*.ts
  - create: frontend/components/providers/*.tsx
  - modify: frontend/app/layout.tsx
  - read: src/Bitacora.Api/Endpoints/Auth/AuthEndpoints.cs
  - read: src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs
complexity: high
done_when: "frontend builds and the shared auth/session/api layer is present and wired into the root layout"
```

## Reference
`src/Bitacora.Api/Endpoints/Auth/AuthEndpoints.cs` — backend auth/bootstrap contract to consume.  
`src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs` — consent contract the frontend must preserve.

## Prompt
Build the shared frontend infrastructure for auth, session resolution, and backend API access. Use `mi-lsp nav search "AuthEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "ConsentEndpoints" --include-content --workspace humor --format toon`, and `mi-lsp nav search "TECH-FRONTEND-SYSTEM-DESIGN" --include-content --workspace humor --format toon` before editing. Add a consistent auth/session abstraction, an API client layer for the backend, providers needed at the root layout, and safe environment/config handling. Do not hardcode route-specific UI here; this task exists to remove ambiguity for all later web slices.

## Skeleton
```ts
export async function getApiClient() {
  return {
    auth: {},
    consent: {},
    registro: {},
  };
}
```

## Verify
`npm run build` -> `Build completed successfully`

## Commit
`feat(web): add shared auth session and api client layer`
