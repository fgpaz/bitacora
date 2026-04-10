# Task T4: Registro Web Flows

## Shared Context
**Goal:** Implement the patient registro routes after the REG docs are hardened.  
**Stack:** Next.js 16, React 19, backend API contracts, `mi-lsp`.  
**Architecture:** Registro web must follow the REG technical UI packs and the existing backend registro endpoints.

## Task Metadata
```yaml
id: T4
depends_on: [T2]
agent_type: ps-next-vercel
files:
  - create: frontend/app/(patient)/registro/page.tsx
  - create: frontend/app/(patient)/registro/nuevo/page.tsx
  - create: frontend/components/registro/*.tsx
  - modify: frontend/lib/api/*.ts
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-REG-001.md
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-REG-002.md
  - read: src/Bitacora.Api/Endpoints/Registro/RegistroEndpoints.cs
complexity: high
done_when: "frontend builds and the patient registro routes implement the REG docs and backend contracts"
```

## Reference
`.docs/wiki/23_uxui/UI-RFC/UI-RFC-REG-001.md` and `.docs/wiki/23_uxui/UI-RFC/UI-RFC-REG-002.md` — technical UI contracts.  
`src/Bitacora.Api/Endpoints/Registro/RegistroEndpoints.cs` — backend registro contract.

## Prompt
Implement the patient registro experience after the REG docs are available. Use `mi-lsp nav search "RegistroEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "CreateMoodEntryCommand" --include-content --workspace humor --format toon`, and `mi-lsp nav search "UpsertDailyCheckinCommand" --include-content --workspace humor --format toon` before editing. Build the registro routes and components according to the REG UI-RFC/handoff docs, and wire them to the existing backend contracts through the shared API client layer. Keep error and recovery states faithful to the docs; do not add final UX validation artifacts here.

## Skeleton
```tsx
export default function RegistroPage() {
  return <main>TODO render registro contract</main>;
}
```

## Verify
`npm run build` -> `Build completed successfully`

## Commit
`feat(web): implement patient registro flows`
