# Task T06: Backend Visualizacion, Export, and Professional Query Paths

## Shared Context
**Goal:** Open the read/query/export surface that is already documented in contracts but not implemented in runtime.
**Stack:** .NET 10, EF Core 10, PostgreSQL, `safe_projection`, encrypted payloads.
**Architecture:** Build patient and professional read paths on top of the existing monolith and the relationship model from T03. Keep export and query ownership in backend, not in the web app.

## Task Metadata
```yaml
id: T06
depends_on:
  - T03
agent_type: ps-dotnet10
files:
  - create: src/Bitacora.Application/Queries/Visualizacion/GetMoodTimelineQuery.cs
  - create: src/Bitacora.Application/Queries/Visualizacion/GetDailyCheckinsQuery.cs
  - create: src/Bitacora.Application/Queries/Visualizacion/GetProfessionalDashboardQuery.cs
  - create: src/Bitacora.Application/Queries/Export/ExportCsvQuery.cs
  - create: src/Bitacora.Application/Dtos/Visualizacion/GetMoodTimeline/Output.cs
  - create: src/Bitacora.Application/Dtos/Visualizacion/GetDailyCheckins/Output.cs
  - create: src/Bitacora.Application/Dtos/Export/ExportCsv/Output.cs
  - create: src/Bitacora.Api/Endpoints/Visualizacion/VisualizacionEndpoints.cs
  - create: src/Bitacora.Api/Endpoints/Export/ExportEndpoints.cs
  - create: src/Bitacora.Api/ViewModels/Visualizacion/GetMoodTimeline/Output.cs
  - create: src/Bitacora.Api/ViewModels/Export/ExportCsv/Output.cs
  - modify: src/Bitacora.Api/Program.cs
  - modify: src/Bitacora.Infrastructure/Services/AesEncryptionService.cs
  - modify: src/Bitacora.DataAccess.Interface/Repositories/IMoodEntryRepository.cs
  - modify: src/Bitacora.DataAccess.Interface/Repositories/IDailyCheckinRepository.cs
  - modify: src/Bitacora.DataAccess.EntityFramework/Repositories/MoodEntryRepository.cs
  - modify: src/Bitacora.DataAccess.EntityFramework/Repositories/DailyCheckinRepository.cs
  - read: .docs/wiki/09_contratos_tecnicos.md
  - read: .docs/wiki/05_modelo_datos.md
complexity: high
done_when: "The backend read/export surface is fully specified for patient and professional access, including authorization, audit, and CSV generation rules."
```

## Reference
- `.docs/wiki/09_contratos_tecnicos.md` — documented GET timeline/checkins/export endpoints
- `.docs/wiki/05_modelo_datos.md` — `safe_projection`, patient/professional ownership, and export invariants
- `src/Bitacora.DataAccess.EntityFramework/Repositories/MoodEntryRepository.cs` — current repository pattern

## Prompt
Implement the read/query/export surface that the docs already promise.

1. Add `GET /api/v1/mood-entries`, `GET /api/v1/daily-checkins`, and `GET /api/v1/export/csv`.
2. Separate patient and professional access rules clearly:
   - patient can read their own data directly
   - professional access must go through `CareLink` and `can_view_data`
   - every professional read must be auditable
3. Use `safe_projection` for normal timeline/dashboard queries wherever possible.
4. Only decrypt payloads when export or a contractually required projection needs full detail.
5. Freeze pagination, date-range limits, sorting, and CSV header semantics so the frontend does not invent them later.
6. Define failure modes explicitly: no link, revoked access, invalid range, export decrypt failure, empty result set, oversized range.
7. Keep query/read logic in backend; the frontend should receive stable response contracts, not business rules.
8. Sync `07/08/09`, RFs, and TPs when the implementation lands.

## Execution Waves
### Wave A — Query contracts
- Add DTOs, query handlers, and repository methods for patient timeline and daily checkins.

### Wave B — Professional reads and audit
- Add professional dashboard/query paths with strict relationship checks and append-only audit logging.

### Wave C — Export
- Add CSV generation, on-demand decrypt logic, date-range rules, and export-specific failure handling.

## Skeleton
```csharp
public sealed record GetMoodTimelineQuery(
    Guid ActorUserId,
    Guid? TargetPatientId,
    DateOnly From,
    DateOnly To);
```

## Verify
`dotnet build src/Bitacora.sln -c Release` -> build includes Visualizacion and Export endpoints without unresolved contracts.

## Commit
`feat(bitacora): implement visualizacion and export query surface`

