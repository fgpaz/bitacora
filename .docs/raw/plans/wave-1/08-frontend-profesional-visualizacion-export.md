# Task T08: Frontend Profesional, Visualizacion, and Export

## Shared Context
**Goal:** Add the professional-facing web experience and read/export surfaces once backend and visual gates allow it.
**Stack:** Next.js 16, Supabase Auth, Bitacora query/export backend.
**Architecture:** Build professional UI only after T02 gate resolution and T06 backend query contracts. This task should not invent visual authority that is still blocked.

## Task Metadata
```yaml
id: T08
depends_on:
  - T02
  - T04
  - T06
agent_type: ps-next-vercel
files:
  - create: frontend/app/(professional)/dashboard/page.tsx
  - create: frontend/app/(professional)/patients/[patientId]/page.tsx
  - create: frontend/app/(professional)/patients/[patientId]/export/page.tsx
  - create: frontend/components/professional/dashboard/ProfessionalDashboard.tsx
  - create: frontend/components/professional/timeline/PatientTimeline.tsx
  - create: frontend/components/professional/export/ExportPanel.tsx
  - create: frontend/lib/professional/dashboard.ts
  - create: frontend/lib/professional/export.ts
  - modify: frontend/components/app-shell/AppShell.tsx
  - modify: frontend/lib/auth/session.ts
  - read: .docs/wiki/23_uxui/INDEX.md
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md
  - read: .docs/wiki/09_contratos_tecnicos.md
complexity: high
done_when: "Professional dashboard, patient timeline, and export entry points are fully specified with backend dependencies and visual gates explicit."
```

## Reference
- `.docs/wiki/23_uxui/INDEX.md` — `VIS-*`, `EXP-*`, and any professional slice gate state
- `.docs/wiki/09_contratos_tecnicos.md` — professional query and export contracts
- `frontend/` scaffold from T04

## Prompt
Implement the professional web experience only on stable authority.

1. Do not bypass T02. If a professional slice is still blocked, keep it out of implementation scope and document the dependency instead of guessing.
2. Build the professional surface around the backend contracts from T06:
   - dashboard list
   - patient timeline
   - export action
3. Treat authorization as two layers:
   - Supabase session / role
   - backend relationship and `can_view_data`
4. Add professional empty/error states that make sense for:
   - no patients linked
   - patient exists but access disabled
   - range too broad / export failed
5. Reuse the shared app shell and API client from T04.
6. Keep all CSV/export generation server-side; the web app should request and download, not generate.
7. Add traceability hooks so support can tie a professional action back to a backend `trace_id` and audit event.

## Execution Waves
### Wave A — Professional routing and guards
- Add protected professional route group and role-aware shell behavior.

### Wave B — Dashboard and timeline
- Implement dashboard list and patient detail timeline views using T06 query contracts.

### Wave C — Export and recovery
- Implement export UX, empty states, denied-access states, and prod support hooks.

## Skeleton
```tsx
export default async function ProfessionalDashboardPage() {
  // Load professional scope, query linked patients, and render the dashboard shell.
}
```

## Verify
`npm run build --prefix frontend` -> professional routes compile and no blocked slice is silently implemented.

## Commit
`feat(frontend): implement professional dashboard timeline and export`

