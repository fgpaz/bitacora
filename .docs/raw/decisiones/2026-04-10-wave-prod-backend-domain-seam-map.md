# Backend Domain Seam Map — Phase 30 T1

**Fecha:** 2026-04-10
**Proyecto:** Bitacora — Clinical Mood Tracker
**Workspace:** `humor` (`/mnt/c/repos/mios/humor`)
**Slug:** bitacora.nuestrascuentitas.com
**Fuente:** T1-backend-seam-map (wave-prod/30-code-backend-domain)

---

## 1. Dominio: Entidades (Domain Entities)

Todas las entidades del dominio ya existen en `src/Bitacora.Domain/Entities/`:

| Entidad | Archivo | Factory | Mutaciones de estado |
|---------|---------|---------|----------------------|
| `User` | `User.cs` | `CreatePatient(...)` | `MarkConsentGranted()`, `MarkActive()`, `RevokeSessions()` |
| `MoodEntry` | `MoodEntry.cs` | `Create(...)` | ninguna (inmutable tras creacion) |
| `DailyCheckin` | `DailyCheckin.cs` | — | — |
| `ConsentGrant` | `ConsentGrant.cs` | `CreateGranted(patientId, consentVersion, createdAtUtc)` | `Revoke(revokedAtUtc)` |
| `PendingInvite` | `PendingInvite.cs` | `Create(professionalId, inviteeEmailHash, inviteToken, expiresAt, createdAtUtc)` | `MarkAsConsumed(consumedAtUtc)` |
| `BindingCode` | `BindingCode.cs` | `Create(code, professionalId, ttlPreset, expiresAt, createdAtUtc)` | `MarkAsUsed()` |
| `CareLink` | `CareLink.cs` | `CreateInvited(...)`, `CreateActive(...)` | `Accept(acceptedAtUtc)`, `Revoke(revokedAtUtc, revokedStatus)`, `UpdateCanViewData(canViewData, utcNow)` |
| `AccessAudit` | `AccessAudit.cs` | `Create(...)` | ninguna (inmutable) |
| `EncryptionKeyVersion` | `EncryptionKeyVersion.cs` | seed data en `AppDbContext.OnModelCreating` | ninguna |
| `TelegramPairingCode` | `TelegramPairingCode.cs` | — | — |
| `TelegramSession` | `TelegramSession.cs` | — | — |
| `ReminderConfig` | `ReminderConfig.cs` | — | — |

**Patron de entidad:** constructors `private` + factory estatica. Toda validacion de dominio en factory. Entidades sin setters publicos — mutacion solo via metodos de dominio.

**Enums** en `src/Bitacora.Domain/Enums/`: `CareLinkStatus`, `PendingInviteStatus`, `ConsentStatus`, `UserRole`, `UserStatus`, `AuditActionType`, `AuditOutcome`, `TelegramSessionStatus`.

---

## 2. Capa de Aplicacion: Commands y Queries (Mediator)

### Commands (Vinculos)

| Comando | Handler | Estado |
|---------|---------|--------|
| `CreateBindingCodeCommand` | `CreateBindingCodeCommandHandler` | EXISTE |
| `AcceptCareLinkCommand` | `AcceptCareLinkCommandHandler` | EXISTE |
| `RevokeCareLinkCommand` | `RevokeCareLinkCommandHandler` | EXISTE |
| `UpdateCareLinkCanViewDataCommand` | `UpdateCareLinkCanViewDataCommandHandler` | EXISTE |
| `CreatePendingInviteCommand` | `CreatePendingInviteCommandHandler` | EXISTE |

### Queries (Vinculos)

| Query | Handler | Estado |
|-------|---------|--------|
| `GetCareLinksByPatientQuery` | `GetCareLinksByPatientQueryHandler` | EXISTE |
| `GetActiveCareLinksWithViewPermissionQuery` | `GetActiveCareLinksWithViewPermissionQueryHandler` | EXISTE |
| `GetProfessionalPatientsQuery` | `GetProfessionalPatientsQueryHandler` | EXISTE |

### Commands/Queries (Consent y Auth)

| Command/Query | Estado |
|---------------|--------|
| `BootstrapPatientCommand` / `BootstrapPatientCommandHandler` | EXISTE |
| `GrantConsentCommand` / `GrantConsentCommandHandler` | EXISTE |
| `RevokeConsentCommand` / `RevokeConsentCommandHandler` | EXISTE |
| `GetCurrentConsentQuery` / `GetCurrentConsentQueryHandler` | EXISTE |
| `CreateMoodEntryCommand` / `CreateMoodEntryCommandHandler` | EXISTE |

### Patron de comando/handler

```csharp
readonly record struct XxxCommand(...) : ICommand<XxxResponse>;
sealed record XxxResponse(...);
sealed class XxxCommandHandler(...) : ICommandHandler<XxxCommand, XxxResponse>;
```

---

## 3. Capa de Datos: Interfaces de Repositorio

Todas existen en `src/Bitacora.DataAccess.Interface/Repositories/`:

| Interfaz | Metodos |
|----------|---------|
| `IBindingCodeRepository` | `FindByCodeAsync`, `FindActiveByProfessionalIdAsync`, `AddAsync`, `UpdateAsync` |
| `ICareLinkRepository` | `GetByIdAsync`, `FindActiveByPatientAndProfessionalAsync`, `GetByPatientIdAsync`, `GetByProfessionalIdAsync`, `AddAsync`, `UpdateAsync` |
| `IPendingInviteRepository` | `FindResumableByTokenAndEmailHashAsync`, `AddAsync` |
| `IConsentGrantRepository` | `GetActiveByPatientAsync`, `AddAsync` |
| `IUserRepository` | `GetBySupabaseUserIdAsync`, `GetByIdAsync`, `AddAsync`, `Update` |
| `IMoodEntryRepository` | `FindDuplicateAsync`, `AddAsync` |
| `IDailyCheckinRepository` | — |
| `IAccessAuditRepository` | `AddAsync` |
| `ITelegramPairingCodeRepository` | — |
| `ITelegramSessionRepository` | — |
| `IReminderConfigRepository` | — |

---

## 4. Capa de Datos: Implementaciones EF

Todas existen en `src/Bitacora.DataAccess.EntityFramework/Repositories/`:

| Implementacion | Estado |
|---------------|--------|
| `BindingCodeRepository` | EXISTE |
| `CareLinkRepository` | EXISTE |
| `PendingInviteRepository` | EXISTE |
| `ConsentGrantRepository` | EXISTE |
| `UserRepository` | EXISTE |
| `MoodEntryRepository` | EXISTE |
| `DailyCheckinRepository` | EXISTE |
| `AccessAuditRepository` | EXISTE |
| `TelegramPairingCodeRepository` | EXISTE |
| `TelegramSessionRepository` | EXISTE |
| `ReminderConfigRepository` | EXISTE |

---

## 5. DbContext (AppDbContext)

Archivo: `src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs`

- **DbSets registrados:** `Users`, `MoodEntries`, `DailyCheckins`, `ConsentGrants`, `PendingInvites`, `AccessAudits`, `EncryptionKeyVersions`, `BindingCodes`, `CareLinks`, `TelegramPairingCodes`, `TelegramSessions`, `ReminderConfigs`
- **Mapeo completo** de todas las tablas en `OnModelCreating`
- **Indices:**
  - `BindingCode`: `Code` (unique), `(ProfessionalId, Used, ExpiresAt)`
  - `CareLink`: `(PatientId, ProfessionalId)`, `(ProfessionalId, Status)`
  - `PendingInvite`: `(ProfessionalId, InviteeEmailHash, Status)`
  - `ConsentGrant`: `(PatientId, ConsentVersion)`
- **Query filters:** `MoodEntry` y `DailyCheckin` filtran por `_currentPatientId` (patron multitenancy por fila)
- **Seed:** `EncryptionKeyVersion(1, 2026-04-09, true)`

---

## 6. Endpoints de API

### Vinculos Endpoints

Archivo: `src/Bitacora.Api/Endpoints/Vinculos/VinculosEndpoints.cs`

| Endpoint | Metodo | Ruta | Estado |
|----------|--------|------|--------|
| `GetVinculos` | GET | `/api/v1/vinculos` | EXISTE |
| `GetActiveVinculosWithViewPermission` | GET | `/api/v1/vinculos/active` | EXISTE |
| `AcceptVinculo` | POST | `/api/v1/vinculos/accept` | EXISTE |
| `RevokeVinculo` | DELETE | `/api/v1/vinculos/{id:guid}` | EXISTE |
| `UpdateVinculoCanViewData` | PATCH | `/api/v1/vinculos/{id:guid}/view-data` | EXISTE |
| `CreateInvite` | POST | `/api/v1/professional/invites` | EXISTE |
| `GetProfessionalPatients` | GET | `/api/v1/professional/patients` | EXISTE |

### Consent Endpoints

Archivo: `src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs`

| Endpoint | Metodo | Ruta | Estado |
|----------|--------|------|--------|
| `GetCurrentConsent` | GET | `/api/v1/consent/current` | EXISTE |
| `GrantConsent` | POST | `/api/v1/consent` | EXISTE |
| `RevokeConsent` | DELETE | `/api/v1/consent/current` | EXISTE |

### Auth Endpoints

- `POST /api/v1/auth/bootstrap` (en `AuthEndpoints.cs`)

### Patron de endpoint

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
.RequireAuthorization("write")
.Accepts<XxxRequest>("application/json")
.WithCommonOpenApi("Xxx", Tag);
```

---

## 7. Middleware Pipeline

**Orden en `Program.cs` (lineas 346-354):**

```
TraceIdMiddleware → ApiExceptionMiddleware → Correlate → Authentication → Authorization → RateLimiter → ConsentRequiredMiddleware
```

### ConsentRequiredMiddleware

- Ubicacion: `NuestrasCuentitas.Bitacora.Api.Middleware`
- Ejecuta despues de `Authorization` en el pipeline
- Requiere que el usuario tenga `UserStatus` distinto de `Registered` para acceder a endpoints protegidos
- **Verificar:** necesita confirmarse que bloquea correctamente a usuarios sin `ConsentGrant` activo

---

## 8. Registro de DI

### AddDataAccess (`DataAccess.EntityFramework/DependencyInjection/ServiceCollectionExtensions.cs`)

```csharp
services.AddDbContext<AppDbContext>(...);  // PostgreSQL via Npgsql, line 20
services.AddScoped<ICurrentPatientContextAccessor, NullCurrentPatientContextAccessor>();  // line 28
services.AddScoped<IUserRepository, UserRepository>();                     // line 29
services.AddScoped<IConsentGrantRepository, ConsentGrantRepository>();     // line 30
services.AddScoped<IMoodEntryRepository, MoodEntryRepository>();           // line 31
services.AddScoped<IDailyCheckinRepository, DailyCheckinRepository>();     // line 32
services.AddScoped<IPendingInviteRepository, PendingInviteRepository>();    // line 33
services.AddScoped<IAccessAuditRepository, AccessAuditRepository>();        // line 34
services.AddScoped<IBindingCodeRepository, BindingCodeRepository>();        // line 35
services.AddScoped<ICareLinkRepository, CareLinkRepository>();              // line 36
services.AddScoped<ITelegramPairingCodeRepository, TelegramPairingCodeRepository>();  // line 37
services.AddScoped<ITelegramSessionRepository, TelegramSessionRepository>();          // line 38
services.AddScoped<IReminderConfigRepository, ReminderConfigRepository>();  // line 39
services.AddScoped<IBitacoraUnitOfWork, EntityFrameworkBitacoraUnitOfWork>();          // line 40
```

### AddInfrastructure (`Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`)

```csharp
services.AddHttpClient("external-default", ...);
services.AddScoped<IExternalProfileClient, ExternalProfileClient>();
services.AddScoped<IEncryptionService, AesEncryptionService>();
services.AddScoped<IPseudonymizationService, PseudonymizationService>();
```

### AddApplication (`Application/DependencyInjection/ServiceCollectionExtensions.cs`)

```csharp
services.AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Scoped; });
```

### Orden en Program.cs (lineas 75-78)

```csharp
builder.Services.AddDataAccess(builder.Configuration);        // 1o — DbContext + repos
builder.Services.AddInfrastructure(builder.Configuration);    // 2o — infrastructure
builder.Services.AddApplication(builder.Configuration);        // 3o — Mediator
builder.Services.AddSetupEventBus(builder.Configuration);      // 4o — event bus
```

---

## 9. DTOs de Contrato

Archivo: `src/Shared.Contract/Vinculos/CareLinkDto.cs`

| DTO | Estado |
|-----|--------|
| `CareLinkDto` | EXISTE |
| `BindingCodeDto` | EXISTE |

---

## 10. Migration Workflow

- **Migration assembly:** `typeof(AppDbContext).Assembly.FullName` (i.e., `Bitacora.DataAccess.EntityFramework`)
- **Apply on startup condicional:** `DataAccess:ApplyMigrationsOnStartup` (solo en Development, Program.cs linea 356-362)
- **Comando de migracion manual:** `dotnet ef database update --project src/Bitacora.DataAccess.EntityFramework`
- **Conexion:** `ConnectionStrings:BitacoraDb` en `appsettings.json`
- **Snapshots:** `AppDbContextModelSnapshot.cs` confirma que `ConsentGrant`, `PendingInvite`, `BindingCode`, `CareLink` ya estan modelados en migraciones

---

## 11. Lo que REALMENTE FALTA (Gap Analysis)

### 11.1 Verificar Migraciones Pendientes

**Accion requerida:** Ejecutar `dotnet ef migrations list --project src/Bitacora.DataAccess.EntityFramework` para confirmar que las tablas `binding_codes`, `care_links`, `pending_invites`, `consent_grants` ya estan creadas en la base de datos.

### 11.2 ConsentRequiredMiddleware — Logica de Consentimiento

**Estado:** El middleware esta registrado en el pipeline (linea 354 de `Program.cs`). **Verificar** que:
- Consulta `IConsentGrantRepository.GetActiveByPatientAsync` correctamente
- El flujo de bootstrap (primer login) crea automaticamente un `ConsentGrant`
- Si el usuario no tiene `ConsentGrant` activo, el middleware retorna 403 o redirige

### 11.3 Endpoint de Telegram — Integracion con Vinculos

El `TelegramEndpoints.cs` existe pero no se revisaron los archivos de Telegram en este task. Verificar si existe un flujo de vinculacion de paciente con profesional via Telegram bot.

### 11.4 Resumen: Lo que NO existe y hay que construir

Segun el analisis del codigo leido, **todo el nucleo de dominio de Vinculos y Consent ya existe**:

- Entidades: `BindingCode`, `CareLink`, `PendingInvite`, `ConsentGrant`
- Repositorios interface + implementacion EF
- Commands y Queries Mediator
- Endpoints REST
- DI registration

**Lo que podria requerir trabajo adicional:**
1. Verificar que las migraciones esten aplicadas y las tablas existan
2. Validar que `ConsentRequiredMiddleware` tenga la logica correcta de verificacion de consentimiento
3. Confirmar el flujo de vinculacion profesional-paciente end-to-end (BindingCode -> AcceptCareLink -> CareLink)
4. Integracion Telegram para vinculacion (si aplica)

---

*Documento generado como parte de T1 de Phase 30 — Bitacora MVP Backend Seam Map*
