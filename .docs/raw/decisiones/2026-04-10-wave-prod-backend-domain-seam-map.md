# Decisión ADR-2026-04-10: Backend Domain Seam Map

**Fecha:** 2026-04-10
**Alcance:** Wave-prod backend domain — seams existentes antes de agregar vínculo
**Fuente:** T1-backend-seam-map.md (wave-prod/30-code-backend-domain)

---

## Domain seams

### Entidades (assemblies `Bitacora.Domain`)

| Entidad | Archivo | Factory | Mutaciones de estado |
|---------|---------|---------|----------------------|
| `User` | `src/Bitacora.Domain/Entities/User.cs` | `User.CreatePatient(supabaseUserId, encryptedEmail, emailHash, keyVersion, createdAtUtc)` | `MarkConsentGranted()`, `MarkActive()`, `RevokeSessions(revokedAtUtc)` |
| `MoodEntry` | `src/Bitacora.Domain/Entities/MoodEntry.cs` | `MoodEntry.Create(patientId, encryptedPayload, safeProjection, keyVersion, encryptedAtUtc, createdAtUtc)` | ninguna — inmutable tras creación |
| `ConsentGrant` | `src/Bitacora.Domain/Entities/ConsentGrant.cs` | `ConsentGrant.CreateGranted(patientId, consentVersion, createdAtUtc)` | `Revoke(revokedAtUtc)` |
| `AccessAudit` | `src/Bitacora.Domain/Entities/AccessAudit.cs` | `AccessAudit.Create(traceId, actorId, pseudonymId, actionType, resourceType, resourceId, patientId, outcome, createdAtUtc)` | ninguna — inmutable |
| `DailyCheckin` | `src/Bitacora.Domain/Entities/DailyCheckin.cs` | — | — |
| `PendingInvite` | `src/Bitacora.Domain/Entities/PendingInvite.cs` | — | — |
| `EncryptionKeyVersion` | `src/Bitacora.Domain/Entities/EncryptionKeyVersion.cs` | seed data en `AppDbContext.OnModelCreating` | — |

**Patrón de entidad:** constructors `private` + factory estática. Toda validación de dominio en factory. Entidades sin setters públicos — mutación solo via métodos de dominio.

---

## Application seams

### Comandos (Mediator, `ICommandHandler`)

| Comando | Handler | Inyección | Dependencias de infraestructura |
|---------|---------|-----------|--------------------------------|
| `BootstrapPatientCommand` | `BootstrapPatientCommandHandler` | `IUserRepository`, `IPendingInviteRepository`, `IBitacoraUnitOfWork`, `IEncryptionService`, `ILogger` | cifrado de email, hash SHA256, búsqueda de invite |
| `GrantConsentCommand` | `GrantConsentCommandHandler` | `IUserRepository`, `IConsentGrantRepository`, `IAccessAuditRepository`, `IBitacoraUnitOfWork`, `IPseudonymizationService`, `IConfiguration` | auditoría con seudónimo, lectura de versión activa de consentimiento |
| `CreateMoodEntryCommand` | `CreateMoodEntryCommandHandler` | `IUserRepository`, `IMoodEntryRepository`, `IAccessAuditRepository`, `IBitacoraUnitOfWork`, `IEncryptionService`, `IPseudonymizationService` | cifrado de payload, dedupe a 1 minuto, auditoría |

### Patrón de comando
```csharp
// Estructura de un comando
readonly record struct XxxCommand(...) : ICommand<XxxResponse>;
sealed record XxxResponse(...);
sealed class XxxCommandHandler(...) : ICommandHandler<XxxCommand, XxxResponse>;
```

### Queries
- `GetCurrentConsentQuery` / `GetCurrentConsentQueryHandler` — reside en `Application/Queries/Consent/GetCurrentConsentQuery.cs`; se invoca desde `ConsentEndpoints.cs`.

---

## Data access seams

### DbContext

**Archivo:** `src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs`

- DbSets: `Users`, `MoodEntries`, `DailyCheckins`, `ConsentGrants`, `PendingInvites`, `AccessAudits`, `EncryptionKeyVersions`
- Query filters: `MoodEntry` y `DailyCheckin` filtran por `_currentPatientId` (patrón de multitenancy por fila)
- Seed: `EncryptionKeyVersion(1, 2026-04-09, true)` en línea 135

### Repositorios (interfaces en `DataAccess.Interface`, implementaciones en `DataAccess.EntityFramework`)

| Interfaz | Implementación | Métodos relevantes |
|----------|---------------|-------------------|
| `IUserRepository` | `UserRepository` | `GetBySupabaseUserIdAsync`, `GetByIdAsync`, `AddAsync`, `Update` |
| `IConsentGrantRepository` | `ConsentGrantRepository` | `GetActiveByPatientAsync`, `AddAsync` |
| `IMoodEntryRepository` | `MoodEntryRepository` | `FindDuplicateAsync`, `AddAsync` |
| `IDailyCheckinRepository` | `DailyCheckinRepository` | — |
| `IPendingInviteRepository` | `PendingInviteRepository` | `FindResumableByTokenAndEmailHashAsync` |
| `IAccessAuditRepository` | `AccessAuditRepository` | `AddAsync` |
| `IBitacoraUnitOfWork` | `EntityFrameworkBitacoraUnitOfWork` | `SaveChangesAsync` |

### Unit of Work
```csharp
// IBitacoraUnitOfWork.SaveChangesAsync — single commit point para todos los repositorios
```

---

## API seams

### Minimal API endpoints (ruteo en `Program.cs` líneas 213-215)

```
POST /api/v1/auth/bootstrap          → AuthEndpoints.MapAuthEndpoints()         [Auth]
GET  /api/v1/consent/current         → ConsentEndpoints.MapConsentEndpoints()  [Auth]
POST /api/v1/consent                 → ConsentEndpoints.MapConsentEndpoints()  [Auth]
DELETE /api/v1/consent/current       → ConsentEndpoints.MapConsentEndpoints()  [Auth]
POST /api/v1/mood-entries           → RegistroEndpoints.MapRegistroEndpoints()  [Auth]
POST /api/v1/daily-checkins          → RegistroEndpoints.MapRegistroEndpoints()  [Auth]
```

### Endpoint pattern
```csharp
app.MapPost("/api/v1/X", async Task<IResult>(
    HttpContext httpContext,
    [FromServices] CurrentAuthenticatedPatientResolver currentPatientResolver,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var currentPatient = await currentPatientResolver.ResolveAsync(httpContext, cancellationToken);
    var response = await mediator.Send(new XxxCommand(...), cancellationToken);
    return Results.Json(response, statusCode: StatusCodes.Status201Created);
})
.RequireAuthorization()
.Accepts<XxxRequest>("application/json")
.WithCommonOpenApi("Xxx", Tag);
```

### Middleware pipeline (`Program.cs` líneas 198-203)
```
TraceIdMiddleware → ApiExceptionMiddleware → Correlate → Authentication → Authorization → ConsentRequiredMiddleware
```

### ConsentRequiredMiddleware
- Ubicación: `NuestrasCuentitas.Bitacora.Api.Middleware`
- Ejecuta después de `Authorization` en el pipeline
- Requiere que el usuario tenga `UserStatus` distinto de `Registered` para acceder a `/api/v1/mood-entries` y `/api/v1/daily-checkins`

---

## Auth and audit seams

### Autenticación
- JWT Bearer con clave simétrica (Supabase JWT secret)
- Claim `sub` extraído via `GetSupabaseUserId()`
- Revocación de sesiones via `SessionsRevokedAt` (línea 104-118 de `Program.cs`)

### Autorización
- `.RequireAuthorization()` en todos los endpoints
- `CurrentAuthenticatedPatientResolver` resuelve el `User` autenticado desde `IUserRepository`

### Auditoría de acceso
- `AccessAudit.Create(...)` en handlers que modifican estado (`BootstrapPatientCommandHandler` NO auditúa; `GrantConsentCommandHandler` y `CreateMoodEntryCommandHandler` SÍ)
- Campos: `TraceId`, `ActorId`, `PseudonymId`, `ActionType`, `ResourceType`, `ResourceId`, `PatientId`, `Outcome`
- `AuditActionType` enum: valores observados `Grant`, `Create`

### Cifrado
- `IEncryptionService` (`AesEncryptionService`) — cifrado simétrico AES de payloads y email
- `IPseudonymizationService` (`PseudonymizationService`) — generación de seudónimos para auditoría
- Versionado de claves via `EncryptionKeyVersion` entity (key version activo embebido en cada entidad)

---

## DI registration seams

### `AddDataAccess` (`DataAccess.EntityFramework/DependencyInjection/ServiceCollectionExtensions.cs`)

```csharp
services.AddDbContext<AppDbContext>(...);                          // PostgreSQL via Npgsql
services.AddScoped<ICurrentPatientContextAccessor, NullCurrentPatientContextAccessor>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IConsentGrantRepository, ConsentGrantRepository>();
services.AddScoped<IMoodEntryRepository, MoodEntryRepository>();
services.AddScoped<IDailyCheckinRepository, DailyCheckinRepository>();
services.AddScoped<IPendingInviteRepository, PendingInviteRepository>();
services.AddScoped<IAccessAuditRepository, AccessAuditRepository>();
services.AddScoped<IBitacoraUnitOfWork, EntityFrameworkBitacoraUnitOfWork>();
```

### `AddInfrastructure` (`Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`)

```csharp
services.AddHttpClient("external-default", ...);
services.AddScoped<IExternalProfileClient, ExternalProfileClient>();
services.AddScoped<IEncryptionService, AesEncryptionService>();
services.AddScoped<IPseudonymizationService, PseudonymizationService>();
```

### `AddApplication` (`Application/DependencyInjection/ServiceCollectionExtensions.cs`)

```csharp
services.AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Scoped; });
```

### Orden en `Program.cs` (líneas 64-67)
```csharp
builder.Services.AddDataAccess(builder.Configuration);   // 1° — DbContext
builder.Services.AddInfrastructure(builder.Configuration); // 2° — infrastructure
builder.Services.AddApplication(builder.Configuration);     // 3° — Mediator
builder.Services.AddSetupEventBus(builder.Configuration);  // 4° — event bus
```

---

## Migration workflow

- Migration assembly: `typeof(AppDbContext).Assembly.FullName` (i.e., `Bitacora.DataAccess.EntityFramework`)
- Apply on startup condicional: `DataAccess:ApplyMigrationsOnStartup` (línea 206, solo en Development)
- Comando de migración manual: `dotnet ef database update --project src/Bitacora.DataAccess.EntityFramework`
- Conexión: `ConnectionStrings:BitacoraDb` en `appsettings.json`

---

## Puntos de extensión para vínculo (VIN/CON)

| Seam | Punto de extensión | Acción requerida |
|------|-------------------|-----------------|
| Dominio | Agregar `Vincculo` entity | Crear en `Bitacora.Domain/Entities/Vincculo.cs` con factory `Create`, entidad hijo de `User` |
| Dominio | `User` como root aggregate | Agregar colección `Vincculos` en `User` si es aggregate root |
| App | `GrantConsentCommandHandler` | Auditar seudónimo de profesional (ya existe `AccessAudit` con `ActorId`) |
| App | `BootstrapPatientCommandHandler` | Agregar lógica de vínculo si invite_token tiene tipo `professional` |
| Data | `AppDbContext` | Agregar `DbSet<Vincculo>` + entity configuration con query filter |
| Data | Repository interface | Crear `IVincculoRepository` en `DataAccess.Interface/Repositories/` |
| Data | DI registration | `AddScoped<IVincculoRepository, VincculoRepository>()` en `ServiceCollectionExtensions` |
| API | `ConsentEndpoints` | Ningún cambio — vínculo es dominio de Application |
| API | Endpoint de vínculo profesional | Nuevo endpoint `POST /api/v1/professionals/{id}/link` en `RegistroEndpoints` o nuevo archivo |
| Migration | Nueva migración EF Core | `dotnet ef migrations add AddVincculo --project src/Bitacora.DataAccess.EntityFramework` |
