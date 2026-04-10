# Task T3: ONB + Consent Web Flows

## Shared Context
**Goal:** Implement the patient onboarding and consent routes directly from the hardened docs.  
**Stack:** Next.js 16, React 19, Supabase Auth, `mi-lsp`.  
**Architecture:** `ONB-001` already has `UI-RFC + HANDOFF`; consent must follow the frozen contracts and voice rules rather than ad hoc UI decisions.

## Task Metadata
```yaml
id: T3
depends_on: [T2]
agent_type: ps-next-vercel
files:
  - create: frontend/app/(public)/page.tsx
  - create: frontend/app/(patient)/onboarding/page.tsx
  - create: frontend/app/(patient)/consent/page.tsx
  - create: frontend/components/onboarding/*.tsx
  - create: frontend/components/consent/*.tsx
  - modify: frontend/lib/api/*.ts
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md
  - read: .docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-ONB-001.md
complexity: high
done_when: "frontend builds and ONB plus consent routes are implemented from the authority pack without unresolved TODOs"
```

## Reference
`.docs/wiki/23_uxui/UI-RFC/UI-RFC-ONB-001.md` — technical UI contract to implement.  
`.docs/wiki/23_uxui/HANDOFF-MAPPING/HANDOFF-MAPPING-ONB-001.md` — design-to-code mapping.  
`src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs` — backend consent API.

## Prompt
Implement the onboarding and consent patient web flow strictly from the approved docs. Before editing, use `mi-lsp nav search "MapConsentEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "MapAuthEndpoints" --include-content --workspace humor --format toon`, and review the ONB authority pack files under `.docs/wiki/23_uxui`. Create the landing, onboarding, and consent routes plus their components. Keep hierarchy, CTA priority, states, and copy aligned to the existing `UI-RFC` and handoff docs. Do not invent new product behavior or run validation in this task.

## Skeleton
```tsx
export default function OnboardingPage() {
  return <main>TODO render ONB-001 contract</main>;
}
```

## Verify
`npm run build` -> `Build completed successfully`

## Commit
`feat(web): implement onboarding and consent patient flows`
