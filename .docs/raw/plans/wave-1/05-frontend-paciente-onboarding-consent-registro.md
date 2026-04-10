# Task T05: Frontend Paciente — Onboarding, Consent, and Registro

## Shared Context
**Goal:** Turn the patient-facing MVP path into a real web experience once the shell and visual gates are ready.
**Stack:** Next.js 16, Supabase Auth, existing Bitacora backend.
**Architecture:** This task consumes T02 gate decisions, T03 backend relationship completion, and T04 frontend foundation. It owns patient flows only.

## Task Metadata
```yaml
id: T05
depends_on:
  - T02
  - T03
  - T04
agent_type: ps-next-vercel
files:
  - create: frontend/app/(patient)/onboarding/page.tsx
  - create: frontend/app/(patient)/consent/page.tsx
  - create: frontend/app/(patient)/registro/mood-entry/page.tsx
  - create: frontend/app/(patient)/registro/daily-checkin/page.tsx
  - create: frontend/components/patient/onboarding/OnboardingFlow.tsx
  - create: frontend/components/patient/consent/ConsentFlow.tsx
  - create: frontend/components/patient/registro/MoodEntryForm.tsx
  - create: frontend/components/patient/registro/DailyCheckinForm.tsx
  - create: frontend/lib/patient/onboarding.ts
  - create: frontend/lib/patient/registro.ts
  - modify: frontend/app/page.tsx
  - modify: frontend/components/app-shell/AppShell.tsx
  - read: .docs/wiki/23_uxui/INDEX.md
  - read: .docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md
  - read: .docs/wiki/09_contratos_tecnicos.md
complexity: high
done_when: "Patient onboarding, consent, and registration flows are fully specified with backend dependencies, slice gates, and edge cases explicit."
```

## Reference
- `.docs/wiki/23_uxui/INDEX.md` — current gate state for `ONB-001`, `REG-001`, `REG-002`
- `.docs/wiki/09_contratos_tecnicos.md` — current backend endpoint surface
- `frontend/` foundation from T04

## Prompt
Implement the patient-facing web MVP without bypassing slice gates.

1. Do not start patient slice implementation until T02 marks the relevant slices visually clear enough for execution.
2. Build the patient path in the real sequence users experience:
   - auth/session resolution
   - bootstrap / invite continuity
   - consent current view and grant/revoke flow
   - first mood entry
   - daily factors
3. Treat onboarding and consent as hard gates:
   - missing bootstrap state
   - missing invite context
   - no active consent
   - revoked consent after prior use
4. Use the shared API client from T04; do not duplicate transport logic in components.
5. Preserve current backend reality:
   - existing POST endpoints exist now
   - GET timeline / export do not belong here
6. Plan the forms around fast completion and clinical safety, but do not invent new UX authority if T02 still blocks a slice.
7. Capture all backend/API gaps surfaced while building patient flows and route them to T03/T06/T09 instead of silently hacking around them.

## Execution Waves
### Wave A — Shared patient state
- Add patient route group, shared loaders, auth handoff, and bootstrap state resolution.

### Wave B — Core flows
- Implement onboarding, consent, mood entry, and daily checkin flows.

### Wave C — Recovery and production fit
- Add empty/error/session-expired states, retry behavior, and analytics/telemetry hooks needed for prod support.

## Skeleton
```tsx
export default async function MoodEntryPage() {
  // Resolve session, bootstrap user, load consent status, then render the guided form.
}
```

## Verify
`npm run build --prefix frontend` -> patient routes compile and all backend contracts used are explicit.

## Commit
`feat(frontend): implement patient onboarding consent and registro`
