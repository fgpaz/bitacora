# RF-TG-010: Scheduler background para recordatorios

## Execution Sheet
- Modulo: TG
- Mecanismo: IHostedService (BackgroundService) en .NET
- Actor: Sistema (sin actor externo)
- Prioridad PDP: Correctness > Usability
- Estado: **Implementado** via `ReminderWorker` + `SendReminderCommand`
- **PENDIENTE:** `SendTelegramMessageAsync` es un stub que solo loguea; no llama a la API de Telegram real

## Precondiciones detalladas
- ReminderConfig existe con `is_active=true` y `next_fire_at` en el pasado o presente
- TelegramSession del paciente tiene `status=linked`
- ConsentGrant del paciente tiene `status=granted`
- El servicio se ejecuta en un loop con intervalo de 1 minuto

## Inputs
- Sin inputs externos. El servicio consulta la DB por su cuenta.

## Proceso (Happy Path)
1. Loop infinito con `await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken)`
2. Query: `SELECT * FROM reminder_configs WHERE is_active = true AND next_fire_at <= now()`
3. Para cada ReminderConfig encontrado:
   a. Verificar consent y sesion (RF-TG-012) — si falla, skip silencioso
   b. Obtener `chat_id` desde TelegramSession WHERE patient_id AND status=linked
   c. Invocar RF-TG-011 para enviar mensaje con keyboard inline
   d. Calcular proximo `next_fire_at` (segun `time_of_day`, dia siguiente)
   e. UPDATE `reminder_configs SET next_fire_at = @proxima_fecha`
4. Si hay errores en el paso de envio: loguear, no detener el loop

## Outputs
- Efecto: mensajes enviados via Telegram Bot API
- UPDATE de `next_fire_at` en cada ReminderConfig procesado

## Errores tipados
| Codigo | Descripcion |
|--------|-------------|
| TG_010_DB_QUERY_FAILED | Error al consultar reminder_configs; reintentar en proximo ciclo |
| TG_010_UPDATE_FAILED | next_fire_at no actualizado; recordatorio podria re-dispararse |

## Casos especiales y variantes
- ReminderConfig con `is_active=false`: nunca consultado
- Servicio reiniciado con `next_fire_at` en el pasado: procesa los pendientes en el primer ciclo
- Multiples instancias del servicio: usar advisory lock en DB o `SELECT FOR UPDATE SKIP LOCKED`
- Error en envio de un recordatorio: no bloquea el procesamiento de los demas

## Impacto en modelo de datos
- Solo lectura en `reminder_configs` (query inicial)
- UPDATE `next_fire_at` tras cada envio exitoso

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Recordatorio due se dispara en el siguiente ciclo
  Given ReminderConfig con next_fire_at=hace 2 minutos y is_active=true
  When el scheduler ejecuta su ciclo
  Then se envia mensaje Telegram al paciente
  And next_fire_at se actualiza al dia siguiente a time_of_day

Scenario: ReminderConfig inactivo no se dispara
  Given ReminderConfig con is_active=false
  When el scheduler ejecuta su ciclo
  Then no se envia ningun mensaje

Scenario: Error de envio no detiene el procesamiento
  Given 3 reminders due, el segundo falla al enviar
  When el scheduler procesa los 3
  Then se envian el primero y el tercero
  And el segundo se loguea como error
```

## Trazabilidad de tests
- UT: TG010_Query_OnlyDueAndActive
- UT: TG010_NextFireAt_CalculatedCorrectly
- IT: TG010_SendError_DoesNotBlockOthers
- IT: TG010_SkipLocked_MultipleInstances

## Sin ambiguedades pendientes
- El intervalo del loop es exactamente 1 minuto; no configurable en runtime
- `next_fire_at` se calcula como `today + 1 day` a la hora `time_of_day` en UTC
