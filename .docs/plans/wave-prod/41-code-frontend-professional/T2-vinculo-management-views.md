# Task T2: Vinculo Management Views

## Shared Context
**Goal:** Implement the professional-facing vínculo routes and states.  
**Stack:** Next.js 16, React 19, backend vínculo API, `mi-lsp`.  
**Architecture:** These screens consume the vínculo backend from Phase 30 and the hardened VIN slice docs from Phase 11.

## Task Metadata
```yaml
id: T2
depends_on: [T1]
agent_type: ps-next-vercel
files:
  - create: frontend/app/(professional)/vinculos/page.tsx
  - create: frontend/app/(professional)/vinculos/[id]/page.tsx
  - create: frontend/components/vinculos/*.tsx
  - modify: frontend/lib/api/*.ts
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-001.md
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-002.md
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-003.md
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIN-004.md
complexity: high
done_when: "frontend builds and professional vínculo management screens consume the vínculo backend contracts"
```

## Reference
`.docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-VIN-001.md` — view intent and states.  
`src/Bitacora.Api/Endpoints/Vinculos/VinculosEndpoints.cs` — backend contract produced earlier.

## Prompt
Implement the professional vínculo management experience from the hardened docs. Before editing, use `mi-lsp nav search "VinculosEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "CT-VINCULOS" --include-content --workspace humor --format toon`, and review the VIN UI-RFC/handoff docs. Build the vínculo list/detail flows, action states, and recovery states without adding extra product behavior. Reuse the shared API client and auth/session foundation instead of creating route-local fetch abstractions.

## Skeleton
```tsx
export default function VinculosPage() {
  return <main>TODO render professional vínculo contract</main>;
}
```

## Verify
`npm run build` -> `Build completed successfully`

## Commit
`feat(web): implement professional vínculo management views`
