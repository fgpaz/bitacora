# Task T3: Frontend Auth Tests And E2E Scaffold

## Shared Context
**Goal:** Add failing frontend tests for OIDC routes, product session cookie, and stale Supabase cookie cleanup.
**Stack:** Next.js 16, React 19, Playwright.
**Architecture:** Frontend owns OIDC PKCE route handlers and browser sees only product session state.

## Locked Decisions
- Browser code must not persist access/refresh token bodies.
- Product cookie name is `bitacora_session`.
- Legacy `sb-access-token` and `sb-supabase-session` must never grant access.

## Task Metadata
```yaml
id: T3
depends_on: [T2]
agent_type: ps-next-vercel
files:
  - modify: frontend/package.json
  - create: frontend/playwright.config.ts
  - create: frontend/tests/e2e/auth-oidc.spec.ts
  - create: frontend/tests/auth/session-contract.test.ts
  - read: frontend/middleware.ts:1-154
  - read: frontend/lib/auth/client.ts
  - read: frontend/lib/auth/SessionContext.tsx
complexity: medium
done_when: "frontend auth tests fail because OIDC/session implementation is missing"
```

## Reference
`frontend/middleware.ts:116-128` currently reads and clears Supabase cookie names.

## Prompt
Add tests first. The tests must assert `/ingresar` starts login, `/auth/callback` creates `bitacora_session`, logout clears `bitacora_session` and legacy Supabase cookies, stale `sb-*` cookies do not pass middleware, and professional access requires Zitadel role mapping. If no test runner exists for unit tests, add the smallest local setup that the repo can support without introducing broad refactors.

## Execution Procedure
1. Open `frontend/package.json`; add `test:e2e` and unit test script only as needed.
2. Create Playwright config targeting local Next dev server.
3. Add E2E tests with mocked OIDC endpoints or route stubs; never use real secrets.
4. Add session contract tests for cookie names and stale Supabase cleanup.
5. Run `npm run test:e2e --prefix frontend` or the added unit test command.
6. Confirm tests fail because routes/session implementation is missing.
7. Stop after RED.

## Skeleton
```ts
test('stale Supabase cookies do not authenticate protected routes', async ({ page, context }) => {
  await context.addCookies([{ name: 'sb-access-token', value: 'legacy', domain: 'localhost', path: '/' }]);
  await page.goto('/dashboard');
  await expect(page).toHaveURL(/\/$/);
});
```

## Verify
`npm run test:e2e --prefix frontend` -> fails for expected missing OIDC/session behavior

## Commit
`test(auth): add failing oidc frontend coverage`
