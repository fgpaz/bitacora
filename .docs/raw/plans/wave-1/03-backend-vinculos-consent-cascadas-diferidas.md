# Task T03: Backend Vinculos, PendingInvite Consumption, and Consent Cascades

## Shared Context
**Goal:** Finish the missing relationship domain so onboarding, professional access, and consent revocation stop depending on deferred seams.
**Stack:** .NET 10, EF Core 10, PostgreSQL, existing modular monolith.
**Architecture:** Extend the current backend inside the existing solution by adding the missing Vinculos domain and wiring it into onboarding and consent flows with fail-closed behavior.

## Task Metadata
```yaml
id: T03
depends_on:
  - T01
agent_type: ps-dotnet10
files:
  - create: src/Bitacora.Domain/Entities/BindingCode.cs
  - create: src/Bitacora.Domain/Entities/CareLink.cs
  - create: src/Bitacora.Domain/Enums/CareLinkStatus.cs
  - create: src/Bitacora.Application/Commands/Vinculos/IssueBindingCodeCommand.cs
  - create: src/Bitacora.Application/Commands/Vinculos/CreateCareLinkFromInviteCommand.cs
  - create: src/Bitacora.Application/Commands/Vinculos/RevokeCareLinkCommand.cs
  - create: src/Bitacora.Application/Commands/Vinculos/ToggleProfessionalAccessCommand.cs
  - create: src/Bitacora.Application/Queries/Vinculos/GetMyCareLinksQuery.cs
  - create: src/Bitacora.Api/Endpoints/Vinculos/VinculosEndpoints.cs
  - create: src/Bitacora.DataAccess.Interface/Repositories/IBindingCodeRepository.cs
  - create: src/Bitacora.DataAccess.Interface/Repositories/ICareLinkRepository.cs
  - create: src/Bitacora.DataAccess.EntityFramework/Repositories/BindingCodeRepository.cs
  - create: src/Bitacora.DataAccess.EntityFramework/Repositories/CareLinkRepository.cs
  - modify: src/Bitacora.Application/Commands/Auth/BootstrapPatientCommand.cs
  - modify: src/Bitacora.Application/Commands/Consent/RevokeConsentCommand.cs
  - modify: src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs
  - modify: src/Bitacora.DataAccess.EntityFramework/Persistence/Migrations/AppDbContextModelSnapshot.cs
  - modify: src/Bitacora.Api/Program.cs
  - read: .docs/wiki/04_RF.md
  - read: .docs/wiki/05_modelo_datos.md
  - read: .docs/wiki/09_contratos_tecnicos.md
complexity: high
done_when: "CareLink and BindingCode are fully planned in code, onboarding consumes PendingInvite deterministically, and consent revocation cascades are specified with no hidden ownership gaps."
```

## Reference
- `src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs` — follow the current endpoint pattern
- `src/Bitacora.Application/Commands/Auth/BootstrapPatientCommand.cs` — current onboarding seam
- `.docs/wiki/05_modelo_datos.md` — canonical fields and invariants for `BindingCode` and `CareLink`

## Prompt
Implement the missing Vinculos module inside the existing monolith. Do not create a second backend service.

1. Add `BindingCode` and `CareLink` to the domain model and physical model using the existing entity/repository/AppDbContext patterns.
2. Preserve `PendingInvite` as a pre-registration seam, but stop leaving invite consumption implicit:
   - bootstrap must detect reusable invite context
   - consent/onboarding completion must deterministically create or resume the correct relationship state
3. Implement the patient/professional ownership rules exactly:
   - professional access defaults to `false`
   - patient is the final authority for activation/deactivation
   - revocation must be auditable and fail closed
4. Expand consent revocation from the current baseline into explicit cascades:
   - revoke active `CareLink` visibility
   - invalidate or close caches/read models that would otherwise leak access
   - record append-only audit evidence
5. Add the minimal API surface for invite/binding/access management and relationship inspection.
6. Keep all endpoints and handlers aligned with `trace_id`, common error envelopes, and current auth/claims resolution.
7. Add migrations and update `07/08/09` plus RF/TP docs when implementation lands; do not leave the relationship model as code-only truth.
8. Avoid over-expanding into dashboard or export logic here; T06 owns read/query/export paths.

## Execution Waves
### Wave A — Domain and storage
- Add `BindingCode` and `CareLink` entities, statuses, repositories, EF wiring, and migration.

### Wave B — Commands and API
- Implement invite/binding/access commands and Vinculos endpoints.
- Wire onboarding and consent handlers to consume the new model.

### Wave C — Cascade hardening and documentation
- Add audit/cascade behavior for consent revocation.
- Sync docs, tests, and error envelopes.

## Skeleton
```csharp
public sealed class CareLink
{
    public Guid CareLinkId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid ProfessionalId { get; private set; }
    public bool CanViewData { get; private set; }
}
```

## Verify
`dotnet build src/Bitacora.sln -c Release` -> `Build succeeded`

## Commit
`feat(bitacora): implement vinculos and consent cascades`

