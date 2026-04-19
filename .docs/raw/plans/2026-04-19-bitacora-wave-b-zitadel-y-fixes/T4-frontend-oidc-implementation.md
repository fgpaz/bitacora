# Task T4: Frontend OIDC Session Implementation

## Shared Context
**Goal:** Replace Supabase frontend auth with Zitadel OIDC PKCE and product-owned session.
**Stack:** Next.js 16 App Router, `oidc-client-ts`, server route handlers, middleware.
**Architecture:** Browser starts OIDC; server callback exchanges code and stores secure httpOnly session.

## Locked Decisions
- Remove `@supabase/supabase-js`.
- `/ingresar` is the canonical login-start route.
- API calls use a frontend server proxy that attaches bearer tokens to `Bitacora.Api`.

## Task Metadata
```yaml
id: T4
depends_on: [T3]
agent_type: ps-next-vercel
files:
  - modify: frontend/package.json
  - modify: frontend/middleware.ts:1-154
  - modify: frontend/lib/api/client.ts:1-120
  - modify: frontend/lib/auth/client.ts
  - modify: frontend/lib/auth/SessionContext.tsx
  - create: frontend/app/ingresar/route.ts
  - create: frontend/app/auth/callback/route.ts
  - create: frontend/app/auth/logout/route.ts
  - create: frontend/app/api/backend/[...path]/route.ts
complexity: high
done_when: "npm run build --prefix frontend and frontend auth tests pass"
```

## Reference
Handoff lines 132-151 define frontend OIDC+PKCE conceptual config.

## Prompt
Implement the minimum OIDC session architecture to pass T3. Use `ZITADEL_ISSUER`, `ZITADEL_WEB_CLIENT_ID`, `ZITADEL_WEB_REDIRECT_URI`, and `ZITADEL_WEB_POST_LOGOUT_REDIRECT_URI`. Route `/ingresar` redirects to Zitadel authorization endpoint with PKCE/state stored in httpOnly transient cookies. Route `/auth/callback` validates state, exchanges code, validates token metadata enough for session creation, then writes `bitacora_session`. Route `/auth/logout` clears `bitacora_session` and legacy `sb-*` cookies and redirects through Zitadel end_session when possible. Replace Supabase session context with a product session context that calls a server session endpoint or decodes only non-sensitive session summary.

## Execution Procedure
1. Install `oidc-client-ts` or use standards-compliant Web APIs if already available.
2. Remove `@supabase/supabase-js` imports/usages.
3. Implement OIDC route handlers.
4. Update middleware to check `bitacora_session`, clear legacy `sb-*`, and enforce professional role from session summary.
5. Update `bitacoraFetch` to call the Next backend proxy instead of direct browser-to-API credentials.
6. Add backend proxy route that attaches bearer access token server-side and never returns token bodies.
7. Update logout buttons to call `/auth/logout`.
8. Run tests and build until green.

## Skeleton
```ts
export async function GET(request: NextRequest) {
  const session = await readBitacoraSession(request.cookies);
  if (!session) return NextResponse.redirect(new URL('/', request.url));
  return NextResponse.next();
}
```

## Verify
`npm run build --prefix frontend && npm run test:e2e --prefix frontend` -> success

## Commit
`feat(auth): add zitadel oidc frontend session`
