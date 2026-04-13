# Task T4: Cutover Domains And Validate

## Shared Context
**Goal:** Deploy the recreated stack, attach public domains, and validate health before considering cutover complete.
**Stack:** Dokploy domains, Let’s Encrypt, Bitacora API health endpoints.
**Architecture:** Public domains should only move once the new API and frontend are healthy.

## Task Metadata
```yaml
id: T4
depends_on: [T1, T2, T3]
agent_type: ps-worker
files:
  - read: infra/dokploy/production-checklist.md:20-47
  - read: .docs/raw/prompts/2026-04-13-bitacora-e2e-full-test.md:1-40
complexity: high
done_when: "New apps are deployed, domains attached, and /health returns 200 on the new API"
```

## Reference
`production-checklist.md:28-47` defines the runtime and smoke gates.

## Prompt
1. Deploy the new postgres if it is not already running.
2. If DB migrations are still manual, stop and report the blocker; otherwise deploy the new API.
3. Verify the new API's deployment status is `done`.
4. Attach domains to the new API:
   - `api.bitacora.nuestrascuentitas.com` on port `8080`, HTTPS `letsencrypt`
   - optionally temporary parking host only if the chosen rollout still needs it
5. Deploy the new frontend and verify its deployment status.
6. Attach `bitacora.nuestrascuentitas.com` to the new frontend on port `3000`, HTTPS `letsencrypt`.
7. Validate:
   - `GET https://api.bitacora.nuestrascuentitas.com/health` returns 200
   - `GET https://bitacora.nuestrascuentitas.com/` returns 200 or expected frontend HTML
   - current app statuses in Dokploy are `done` or `running`, not `error`
8. If validation fails, report exact failure and stop before deleting or disabling any old resources.

## Skeleton
```bash
curl -s -X POST -H "x-api-key: $DOKPLOY_API_KEY" -H "Content-Type: application/json" \
  -d '{"applicationId":"..."}' "$DOKPLOY_URL/api/application.deploy"
```

## Verify
`curl -fsS https://api.bitacora.nuestrascuentitas.com/health` exits 0 and Dokploy shows the new apps healthy.

## Commit
`chore(infra): cut over bitacora domains to dedicated Dokploy project`
