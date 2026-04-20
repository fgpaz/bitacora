---
id: RF-TG-006
title: Configurar horario de recordatorio con soporte de zona horaria
implements:
  - src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs
  - src/Bitacora.Application/Commands/Telegram/ConfigureReminderScheduleCommandHandler.cs
  - src/Bitacora.Domain/Entities/ReminderConfig.cs
  - src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs
  - frontend/lib/api/client.ts
  - frontend/components/patient/telegram/TelegramPairingCard.tsx
tests:
  - src/Bitacora.Tests/ReminderScheduleTests.cs
---

# RF-TG-006: Configurar horario de recordatorio con soporte de zona horaria

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-TG-006 |
| Modulo | TG |
| Endpoint | PUT /api/v1/telegram/reminder-schedule; GET /api/v1/telegram/reminder-schedule |
| Actor | Paciente (UI web + API) |
| Prioridad | Usabilidad |
| Estado | **Implementado backend + frontend** via `ConfigureReminderScheduleCommand`, `GetReminderScheduleQuery` y `TelegramPairingCard.tsx` (Phase 40, actualizado 2026-04-20) |

## Precondiciones detalladas
- JWT valido con `User.status=active`.
- Existe un `TelegramSession` activo (linked) para el `patient_id`.
- La UI muestra horario local de Buenos Aires y convierte a UTC antes de invocar la API.
- `HourUtc` en rango 0-23; `MinuteUtc` en 0 o 30.
- `Timezone` es un IANA o Windows timezone ID valido (o null — default `America/Argentina/Buenos_Aires`).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| HourUtc | int | Request body | 0-23; ya convertido desde hora local UI |
| MinuteUtc | int | Request body | 0 o 30; ya convertido desde hora local UI |
| Timezone | string? | Request body | IANA o Windows TZ ID; null = default Buenos Aires |

## Proceso (Happy Path)
1. Extraer `patient_id` del JWT.
2. Verificar que existe `TelegramSession` activa para ese paciente; si no, lanzar 403.
3. Validar `HourUtc` y `MinuteUtc`; si son invalidos, lanzar error tipado 400.
4. Resolver `Timezone` a `TimeZoneInfo` (IANA o Windows). Si viene null/blanco, usar `America/Argentina/Buenos_Aires`.
5. Si `Timezone` invalido, lanzar `BitacoraException("TG_006_INVALID_TIMEZONE", 400)`.
6. Buscar `ReminderConfig` existente para el paciente (`FindByPatientIdAsync`).
7. Si existe: actualizar `HourUtc`, `MinuteUtc`, `ReminderTimezone`, `Enabled=true`.
8. Si no existe: crear nuevo `ReminderConfig` con los valores provistos.
9. Calcular `NextFireAtUtc` (proximo disparo a partir de now()).
10. `SaveChangesAsync`.
11. Retornar respuesta con todos los campos del config.

## Outputs

### PUT /api/v1/telegram/reminder-schedule

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| ReminderConfigId | uuid | ID del ReminderConfig creado/actualizado |
| HourUtc | int | Hora UTC configurada |
| MinuteUtc | int | Minuto UTC configurado |
| ReminderTimezone | string | Timezone resuelto (IANA) |
| Enabled | bool | Siempre true tras configurar |
| NextFireAtUtc | timestamp? | Proximo disparo calculado (nullable si no se puede calcular) |

### GET /api/v1/telegram/reminder-schedule

Consulta la configuracion vigente del paciente autenticado para que la UI pueda mostrar el horario local guardado despues de recargar.

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| Configured | bool | `false` si todavia no hay ReminderConfig para el paciente |
| ReminderConfigId | uuid? | ID del ReminderConfig cuando existe |
| HourUtc | int? | Hora UTC guardada |
| MinuteUtc | int? | Minuto UTC guardado |
| ReminderTimezone | string? | Timezone de referencia guardado |
| Enabled | bool? | Estado operativo del recordatorio |
| NextFireAtUtc | timestamp? | Proximo disparo calculado cuando existe |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| TG_006_NO_ACTIVE_SESSION | 403 | No hay sesion Telegram activa | `{error: "TG_006_NO_ACTIVE_SESSION"}` |
| TG_006_INVALID_HOUR | 400 | `HourUtc` fuera de 0..23 | `{error: "TG_006_INVALID_HOUR"}` |
| TG_006_INVALID_MINUTE | 400 | `MinuteUtc` distinto de 0 o 30 | `{error: "TG_006_INVALID_MINUTE"}` |
| TG_006_INVALID_TIMEZONE | 400 | Timezone string no reconocido | `{error: "TG_006_INVALID_TIMEZONE"}` |
| TG_006_UNAUTHORIZED | 401 | JWT invalido o ausente | `{error: "TG_006_UNAUTHORIZED"}` |

## Casos especiales y variantes
- UPSERT semantico: crea `ReminderConfig` si no existe, actualiza si ya existe.
- Timezone lookup: acepta IANA o Windows timezone ID. Solo `null`/blanco usa default `America/Argentina/Buenos_Aires`; un timezone invalido no tiene fallback silencioso.
- `HourUtc` y `MinuteUtc` son UTC puros; la UI los convierte desde la hora local antes de enviar.
- El scheduler backend (`ReminderSchedulerBackgroundService`) usa `HourUtc` + `MinuteUtc` directamente para calcular el proximo disparo.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| ReminderConfig | INSERT o UPDATE | hour_utc, minute_utc, reminder_timezone, enabled=true |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente configura recordatorio diario a las 22:00 Buenos Aires
  Given paciente autenticado con sesion Telegram activa
  And la UI muestra "22:00" en hora local Buenos Aires
  And el cliente envia body: { HourUtc: 1, MinuteUtc: 0, Timezone: "America/Argentina/Buenos_Aires" }
  When PUT /api/v1/telegram/reminder-schedule
  Then HTTP 200 con { HourUtc: 1, MinuteUtc: 0, ReminderTimezone: "America/Argentina/Buenos_Aires", Enabled: true }
  And ReminderConfig.enabled = true para ese patient_id
  When la persona recarga /configuracion/telegram
  Then GET /api/v1/telegram/reminder-schedule retorna Configured=true
  And la UI vuelve a mostrar "22:00" en hora local Buenos Aires

Scenario: PUT sin sesion Telegram activa
  Given paciente autenticado sin sesion Telegram vinculada
  When PUT /api/v1/telegram/reminder-schedule
  Then HTTP 403 con error="TG_006_NO_ACTIVE_SESSION"

Scenario: PUT con timezone invalido
  Given paciente autenticado con sesion Telegram activa
  And body: { HourUtc: 9, MinuteUtc: 0, Timezone: "Mars/Olympus" }
  When PUT /api/v1/telegram/reminder-schedule
  Then HTTP 400 con error="TG_006_INVALID_TIMEZONE"

Scenario: PUT con minuto invalido
  Given paciente autenticado con sesion Telegram activa
  And body: { HourUtc: 9, MinuteUtc: 15, Timezone: "America/Argentina/Buenos_Aires" }
  When PUT /api/v1/telegram/reminder-schedule
  Then HTTP 400 con error="TG_006_INVALID_MINUTE"
```

## Trazabilidad
| Elemento | Referencia |
|----------|-----------|
| Flujo fuente | FL-TG-02 |
| Test plan | TP-TG (TG-P06, TG-N05, TG-N06) |
| Comando backend | `ConfigureReminderScheduleCommand` + `ConfigureReminderScheduleCommandHandler` |
| Query backend | `GetReminderScheduleQuery` + `GetReminderScheduleQueryHandler` |
| Endpoint | `TelegramEndpoints.cs` MapPut("/reminder-schedule") + MapGet("/reminder-schedule") |
| Componente frontend | `TelegramPairingCard.tsx` (reminderControls section) |
| API client | `frontend/lib/api/client.ts` → `setReminderSchedule()`, `getReminderSchedule()` |
| Contrato tecnico | `09_contratos_tecnicos.md` — PUT /api/v1/telegram/reminder-schedule |
| Migracion DB | `20260415000001_AddReminderTimezoneColumn` |
