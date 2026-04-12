# 2026-04-10-wave-prod-read-telegram-seam-map

> Fecha: 2026-04-11 | Autor: ps-worker | Task: T1-wave-prod-read-telegram-seam-map
> Fuente: `.docs/plans/wave-prod/31-code-backend-reads-telegram/T1-read-telegram-seam-map.md`

## Evidencia de archivos verificados

| Archivo | Ruta verificada |
|---------|----------------|
| `GetCurrentConsentQuery.cs` | `src/Bitacora.Application/Queries/Consent/GetCurrentConsentQuery.cs` |
| `UpsertDailyCheckinCommand.cs` | `src/Bitacora.Application/Commands/Registro/UpsertDailyCheckinCommand.cs` |
| `MoodEntry.cs` | `src/Bitacora.Domain/Entities/MoodEntry.cs` |
| `AccessAudit.cs` | `src/Bitacora.Domain/Entities/AccessAudit.cs` |
| `TECH-TELEGRAM.md` | `.docs/wiki/07_tech/TECH-TELEGRAM.md` |
| `TelegramSession` en `src/` | **No existe** —吻合 TECH-TELEGRAM invariante "Diferido" |

---

## Read-side seams (VIS / EXP queries)

### 1. Consent query seam — `GetCurrentConsentQuery.cs`

| Punto | Detalle |
|-------|---------|
| Insercion VIS consent | Crear `GetTelegramConsentStatusQuery` en `src/Bitacora.Application/Queries/Consent/` siguiendo el patron de `GetCurrentConsentQueryHandler` |
| Dependencias inyectadas | `IUserRepository`, `IConsentGrantRepository`, `IAccessAuditRepository`, `IBitacoraUnitOfWork`, `IPseudonymizationService` |
| Patron de response | `GetCurrentConsentResponse` (Version, Text, Sections, PatientStatus) — replicar para TG con `patient_status` del tipo `"linked" / "unlinked"` |
| Audit hook | `AccessAudit` con `AuditActionType.Read` sobre `consent_grant` — mismo patron |

### 2. MoodEntry read seam — `MoodEntry.cs`

| Punto | Detalle |
|-------|---------|
| Entity | `MoodEntry` vive en `src/Bitacora.Domain/Entities/MoodEntry.cs` — privados setters, factory `Create()` |
| Repository | `IMoodEntryRepository` en `src/Bitacora.DataAccess.Interface/Repositories/IMoodEntryRepository.cs` |
| Insercion VIS | Crear `GetMoodHistoryQuery` en `src/Bitacora.Application/Queries/Mood/` (nuevo directorio) — consulta `IMoodEntryRepository.GetByPatientAsync()` |
| Projection | `SafeProjection` (JSON publicly readable) es el unico campo legible sin decryption — **nunca** exponer `EncryptedPayload` por TG |

### 3. DailyCheckin read seam — `UpsertDailyCheckinCommand.cs`

| Punto | Detalle |
|-------|---------|
| Entity | `DailyCheckin` se crea/actualiza via `UpsertDailyCheckinCommand` — no existe un query dedicado aun |
| Insercion VIS | Crear `GetDailyCheckinHistoryQuery` en `src/Bitacora.Application/Queries/DailyCheckin/` |
| Patron de response | `UpsertDailyCheckinResponse` como modelo — replicar para query response |

### 4. Export contracts seam

| Punto | Detalle |
|-------|---------|
| Endpoint | `RegistroEndpoints.cs` — agregar `GET /api/v1/export/mood-csv` (RF-EXP-001) |
| Autorizacion | Requiere JWT del paciente owner (validar `PatientId` del token vs. recurso solicitado) |
| Decryption | Ocurre en memoria en el handler; resultado como file attachment — **no** se almacena descifrado en disco ni caches |
| Audit hook | `AccessAudit` con `AuditActionType.Export` sobre `mood_entry` — misma estructura que `GetCurrentConsentQuery` |

---

## Telegram seams

### 5. TelegramSession persistence (T3-3 — no existe aun)

| Punto | Detalle |
|-------|---------|
| Ausencia confirmada | `TelegramSession` no existe en `src/` —吻合 estado "Diferido" de TECH-TELEGRAM |
| Insercion futura | Crear `src/Bitacora.Domain/Entities/TelegramSession.cs` con campos: `TelegramSessionId`, `PatientId`, `ChatId`, `LinkedAt`, `UnlinkedAt?` |
| Repository | `ITelegramSessionRepository` en `src/Bitacora.DataAccess.Interface/Repositories/` |
| Pairing flow | Webhook `POST /api/v1/telegram/webhook` valida code, crea `TelegramSession(patient_id, chat_id, linked)` — TECH-TELEGRAM linea 67 |
| Consent gate | Todo recordatorio debe verificar `ConsentGrant.Status == Granted` antes de enviar — TECH-TELEGRAM linea 58 |

### 6. Reminder scheduling seam

| Punto | Detalle |
|-------|---------|
| Runtime | `IHostedService` en `src/Bitacora.Workers/` (directorio a crear) — timer cada 1 minuto |
| Query | `ReminderConfig` (tabla/entidad a crear) filtrando `WHERE next_fire_at <= now()` |
| Rate limit | Max 30 msg/segundo via Telegram Bot API — backoff exponencial max 3 intentos |
| Skip conditions | `ConsentGrant` revocado o `TelegramSession.UnlinkedAt != null` — TECH-TELEGRAM linea 58 |

### 7. Webhook handling seam

| Punto | Detalle |
|-------|---------|
| Endpoint | `POST /api/v1/telegram/webhook` — declarado en TECH-TELEGRAM Tabla 1 |
| Signature | HMAC-SHA256 requerido — rechazar payloads sin firma valida |
| Modo dev | Long-polling via `GetUpdatesReceiver` sin HTTPS — `TELEGRAM_USE_WEBHOOK=false` |
| Modo prod | Webhook via `setWebhook` — `TELEGRAM_WEBHOOK_URL` |
| Insercion | Crear `src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs` |

### 8. Conversation state seam

| Punto | Detalle |
|-------|---------|
| Estado | En memoria — `Dictionary<chatId, ConversationState>` con TTL 10 min |
| Persistencia | **No se persiste en DB** — efimero — TECH-TELEGRAM linea 51 |
| Registro parcial | Si el paciente no completa los factores, solo se registra el `MoodEntry` |

---

## Audit and operability seams

### 9. AccessAudit hook generico

| Punto | Detalle |
|-------|---------|
| Entity | `AccessAudit` en `src/Bitacora.Domain/Entities/AccessAudit.cs` — factory `Create()` |
| Campos | `TraceId`, `ActorId`, `PseudonymId`, `ActionType`, `ResourceType`, `ResourceId?`, `PatientId?`, `Outcome`, `CreatedAtUtc` |
| Patron de uso | Cada command/query que modifica o lee datos clinicos debe invocar `IAccessAuditRepository.AddAsync()` antes de `unitOfWork.SaveChangesAsync()` |
| ActionTypes existentes | `AuditActionType.Read`, `Create`, `Update` — extender con `Export`, `TelegramLink`, `TelegramUnlink` segun necesidad |
| Restriccion logs | Logs del modulo Telegram **no** pueden contener `safe_projection` ni `encrypted_payload` — solo `session ID` y `trace_id` |

### 10. Pseudonymization seam

| Punto | Detalle |
|-------|---------|
| Servicio | `IPseudonymizationService` inyectado en handlers |
| Uso en AccessAudit | `pseudonymizationService.CreatePseudonym(actorId)` — nunca almacenar `ActorId` real |
| Extension TG | `TelegramSession` debe usar el mismo patron de `PatientId` pseudonymized en logs |

---

## Resumen de inserccion directa

| Seam | Archivo / directorio a crear | Linea/patron de referencia |
|------|-------------------------------|---------------------------|
| VIS consent | `src/Bitacora.Application/Queries/Consent/GetTelegramConsentStatusQuery.cs` | `GetCurrentConsentQuery.cs` lineas 20-65 |
| VIS mood history | `src/Bitacora.Application/Queries/Mood/GetMoodHistoryQuery.cs` | `MoodEntry.cs` + `IMoodEntryRepository` |
| VIS checkin history | `src/Bitacora.Application/Queries/DailyCheckin/GetDailyCheckinHistoryQuery.cs` | `UpsertDailyCheckinCommand.cs` lineas 85-98 |
| Export endpoint | `src/Bitacora.Api/Endpoints/Export/ExportEndpoints.cs` | `RegistroEndpoints.cs` pattern |
| TelegramSession entity | `src/Bitacora.Domain/Entities/TelegramSession.cs` | `MoodEntry.cs` factory pattern |
| TelegramSession repo | `src/Bitacora.DataAccess.Interface/Repositories/ITelegramSessionRepository.cs` | `IMoodEntryRepository.cs` |
| Reminder worker | `src/Bitacora.Workers/ReminderWorker.cs` (IHostedService) | TECH-TELEGRAM linea 56 |
| Webhook endpoint | `src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs` | `ConsentEndpoints.cs` |
| Audit extensions | `AuditActionType.TelegramLink`, `TelegramUnlink`, `Export` | `AccessAudit.cs` lineas 11, 45-86 |

---

## Invariantes no-fuga (confirmadas)

1. `encrypted_payload` no sale del proceso .NET
2. `safe_projection` no se incluiye en respuestas del bot — bot solo confirma receipt
3. Export CSV requiere JWT del paciente owner (verificable en cada export handler)
4. Descifrado para export ocurre en memoria; no se almacena descifrado en disco ni caches
5. Logs TG no contienen `safe_projection` ni `encrypted_payload`
6. Consent revocation o session unlink corta inmediatamente el recordatorio
