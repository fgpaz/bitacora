# RF-TG-006: Configurar horario de recordatorio con soporte de zona horaria

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-TG-006 |
| Modulo | TG |
| Endpoint | PUT /api/v1/telegram/reminder-schedule |
| Actor | Paciente (UI web + API) |
| Prioridad | Usabilidad |
| Estado | **Implementado backend + frontend** via `ConfigureReminderScheduleCommand` + `TelegramPairingCard.tsx` (Phase 40, 2026-04-16) |

## Precondiciones detalladas
- JWT valido con `User.status=active`.
- Existe un `TelegramSession` activo (linked) para el `patient_id`.
- `HourUtc` en rango 0-23; `MinuteUtc` en 0 o 30.
- `Timezone` es un IANA o Windows timezone ID valido (o null — default `America/Argentina/Buenos_Aires`).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| HourUtc | int | Request body | 0-23 |
| MinuteUtc | int | Request body | 0 o 30 |
| Timezone | string? | Request body | IANA o Windows TZ ID; null = default Buenos Aires |

## Proceso (Happy Path)
1. Extraer `patient_id` del JWT.
2. Verificar que existe `TelegramSession` activa para ese paciente; si no, lanzar 403.
3. Resolver `Timezone` a `TimeZoneInfo` (lookup dual IANA→Windows; fallback Argentina).
4. Si `Timezone` invalido, lanzar `BitacoraException("INVALID_TIMEZONE", 400)`.
5. Buscar `ReminderConfig` existente para el paciente (`FindByPatientIdAsync`).
6. Si existe: actualizar `HourUtc`, `MinuteUtc`, `ReminderTimezone`, `Enabled=true`.
7. Si no existe: crear nuevo `ReminderConfig` con los valores provistos.
8. Calcular `NextFireAtUtc` (proximo disparo a partir de now()).
9. `SaveChangesAsync` en transaccion.
10. Retornar respuesta con todos los campos del config.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| ReminderConfigId | uuid | ID del ReminderConfig creado/actualizado |
| HourUtc | int | Hora UTC configurada |
| MinuteUtc | int | Minuto UTC configurado |
| ReminderTimezone | string | Timezone resuelto (IANA) |
| Enabled | bool | Siempre true tras configurar |
| NextFireAtUtc | timestamp? | Proximo disparo calculado (nullable si no se puede calcular) |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| TG_006_NO_ACTIVE_SESSION | 403 | No hay sesion Telegram activa | `{error: "TG_006_NO_ACTIVE_SESSION"}` |
| INVALID_TIMEZONE | 400 | Timezone string no reconocido | `{error: "INVALID_TIMEZONE"}` |
| TG_006_UNAUTHORIZED | 401 | JWT invalido o ausente | `{error: "TG_006_UNAUTHORIZED"}` |

## Casos especiales y variantes
- UPSERT semantico: crea `ReminderConfig` si no existe, actualiza si ya existe.
- Timezone lookup: intenta IANA, luego Windows timezone ID. Fallback `America/Argentina/Buenos_Aires` si Timezone es null.
- `HourUtc` y `MinuteUtc` son UTC puros; la UI los convierte desde la hora local antes de enviar.
- El scheduler backend (`ReminderSchedulerBackgroundService`) usa `HourUtc` + `MinuteUtc` directamente para calcular el proximo disparo.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| ReminderConfig | INSERT o UPDATE | hour_utc, minute_utc, reminder_timezone, enabled=true |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente configura recordatorio diario a las 09:00 Buenos Aires
  Given paciente autenticado con sesion Telegram activa
  And body: { HourUtc: 9, MinuteUtc: 0, Timezone: "America/Argentina/Buenos_Aires" }
  When PUT /api/v1/telegram/reminder-schedule
  Then HTTP 200 con { HourUtc: 9, MinuteUtc: 0, ReminderTimezone: "America/Argentina/Buenos_Aires", Enabled: true }
  And ReminderConfig.enabled = true para ese patient_id

Scenario: PUT sin sesion Telegram activa
  Given paciente autenticado sin sesion Telegram vinculada
  When PUT /api/v1/telegram/reminder-schedule
  Then HTTP 403 con error="TG_006_NO_ACTIVE_SESSION"

Scenario: PUT con timezone invalido
  Given paciente autenticado con sesion Telegram activa
  And body: { HourUtc: 9, MinuteUtc: 0, Timezone: "Mars/Olympus" }
  When PUT /api/v1/telegram/reminder-schedule
  Then HTTP 400 con error="INVALID_TIMEZONE"
```

## Trazabilidad
| Elemento | Referencia |
|----------|-----------|
| Flujo fuente | FL-TG-02 |
| Test plan | TP-TG (TG-P06, TG-N05) |
| Comando backend | `ConfigureReminderScheduleCommand` + `ConfigureReminderScheduleCommandHandler` |
| Endpoint | `TelegramEndpoints.cs` MapPut("/reminder-schedule") |
| Componente frontend | `TelegramPairingCard.tsx` (reminderControls section) |
| API client | `frontend/lib/api/client.ts` → `setReminderSchedule()` |
| Contrato tecnico | `09_contratos_tecnicos.md` — PUT /api/v1/telegram/reminder-schedule |
| Migracion DB | `20260415000001_AddReminderTimezoneColumn` |
