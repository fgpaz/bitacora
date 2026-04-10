# Task T3: Visualization + Export Views

## Shared Context
**Goal:** Implement the professional visualization and export UI on top of the read/export backend.  
**Stack:** Next.js 16, React 19, backend read/export API, `mi-lsp`.  
**Architecture:** Visualization and export share data sources but must keep distinct confirmation and risk states.

## Task Metadata
```yaml
id: T3
depends_on: [T1]
agent_type: ps-next-vercel
files:
  - create: frontend/app/(professional)/visualizacion/page.tsx
  - create: frontend/app/(professional)/visualizacion/[patientId]/page.tsx
  - create: frontend/app/(professional)/export/page.tsx
  - create: frontend/components/visualizacion/*.tsx
  - create: frontend/components/export/*.tsx
  - modify: frontend/lib/api/*.ts
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIS-001.md
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-VIS-002.md
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-EXP-001.md
complexity: high
done_when: "frontend builds and professional visualization/export screens consume the backend contracts"
```

## Reference
`.docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-VIS-001.md` — visualization state intent.  
`.docs/wiki/23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-EXP-001.md` — export behavior contract.

## Prompt
Implement the professional visualization and export screens from the prepared docs and backend contracts. Before editing, use `mi-lsp nav search "VisualizacionEndpoints" --include-content --workspace humor --format toon`, `mi-lsp nav search "ExportEndpoints" --include-content --workspace humor --format toon`, and re-open the VIS/EXP UI-RFC docs. Build timeline/summary views, export initiation UI, and confirmation/recovery states without inventing new product behavior. Keep privacy-sensitive states explicit and aligned with the backend contract.

## Skeleton
```tsx
export default function VisualizacionPage() {
  return <main>TODO render visualization contract</main>;
}
```

## Verify
`npm run build` -> `Build completed successfully`

## Commit
`feat(web): implement professional visualization and export views`
