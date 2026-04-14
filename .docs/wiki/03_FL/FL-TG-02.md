# FL-TG-02: Recordatorio programado

## Goal
El sistema envia recordatorios automaticos via Telegram a los pacientes que configuraron horarios de check-in.

## Scope
**In:** Scheduler, envio de mensaje, manejo de respuesta inline.
**Out:** Registro de humor (→ FL-REG-02).
**Estado: IMPLEMENTADO.** El `ReminderWorker` y `SendReminderCommand` estan materializados. `SendTelegramMessageAsync` invoca la API de Telegram Bot con el token configurado (`TELEGRAM_BOT_TOKEN`); la integracion es real segun CT-TELEGRAM-RUNTIME y TECH-TELEGRAM (Phase 30+). Ejecucion E2E del scheduler (TG-P02/TG-N02) pendiente de verificacion.

## Actores y ownership
| Actor | Rol en el flujo |
|-------|----------------|
| Sistema | Ejecuta scheduler, envia mensajes |
| Paciente | Recibe recordatorio, responde (opcional) |
| Modulo Telegram | Gestiona envio y recepcion |

## Precondiciones
- TelegramSession en estado `linked`
- Paciente tiene al menos un horario de recordatorio configurado (ReminderConfig con enabled=true)
- ConsentGrant activo (si el consent fue revocado, no se envian recordatorios)

## Postcondiciones
- Mensaje de recordatorio enviado al paciente
- Si el paciente responde, flujo continua en FL-REG-02

## Secuencia principal

```mermaid
sequenceDiagram
    participant WORKER as ReminderWorker (cada 60s)
    participant API as Bitacora.Api
    participant DB as bitacora_db
    participant TG as Telegram
    actor P as Paciente

    WORKER->>DB: SELECT ReminderConfig WHERE enabled=true AND next_fire_at_utc <= now_utc
    loop Por cada recordatorio pendiente
        WORKER->>API: SendReminderCommand(reminder_config_id)
        API->>DB: SELECT ReminderConfig, TelegramSession, ConsentGrant
        alt Consent revocado o sesion unlinked
            API->>API: Disable() + audit denied
            API-->>WORKER: Sent=false
        end
        alt Envio exitoso
            API->>TG: SendTelegramMessageAsync(chat_id, "Es hora de registrar...")
            TG-->>P: Mensaje de recordatorio
            API->>DB: AdvanceNextFire() + audit ok
            API-->>WORKER: Sent=true, NextFireAtUtc=...
        end
    end
```

## Paths alternativos / errores

| Condicion | Resultado |
|-----------|----------|
| TelegramSession unlinked | Skip recordatorio (no desactiva ReminderConfig) |
| ConsentGrant revocado | Skip recordatorio (hard invariant) |
| Telegram API falla | Retry con backoff (max 3 intentos) |
| Paciente no responde | No action, proximo recordatorio en siguiente horario |

## Architecture slice
- **Modulos:** Telegram (background scheduler)
- **Patron:** Background service con timer (hosted service .NET)
- **Invariante:** No enviar si consent revocado o sesion unlinked
- **Entidades implementadas:** `ReminderConfig`, `SendReminderCommand` (wireado pero no invocado automaticamente)

## Data touchpoints
| Entidad | Operacion | Estado |
|---------|----------|--------|
| ReminderConfig | READ (GetDueRemindersAsync) + UPDATE (AdvanceNextFire o Disable) | Implementado |
| TelegramSession | READ (FindLinkedByPatientIdAsync) | Implementado |
| ConsentGrant | READ (GetActiveByPatientAsync) | Implementado |
| SendReminderCommand | Invocado por ReminderWorker | Implementado |
| AccessAudit | INSERT | Implementado |

## Pendientes explícitos
- El mensaje de recordatorio no usa inline keyboard (-3..+3); es un texto generico "Es hora de registrar tu humor del dia.". Keyboard inline pendiente de implementacion.
- TG-P02 y TG-N02 (scheduler E2E) no ejecutados en el ciclo de prueba 2026-04-14.

## RF candidatos
- RF-TG-010: Scheduler background para recordatorios (**Implementado**)
- RF-TG-011: Enviar mensaje con keyboard inline a Telegram (**Implementado — texto basico; keyboard inline pendiente**)
- RF-TG-012: Skip si consent revocado o session unlinked (**Implementado**)

## Bottlenecks y mitigaciones
| Riesgo | Mitigacion |
|--------|-----------|
| Muchos recordatorios al mismo minuto | Batch de envios + rate limit de Telegram API (30 msg/seg) |
| Telegram API down | Retry con backoff exponencial (max 3) |

## RF handoff checklist
- [x] Actores y ownership explicitos
- [x] Diagrama explica el flujo sin prosa
- [x] Bottlenecks y mitigaciones explicitos
- [x] Traducible a RF atomicos y testeables
- [x] Dentro del limite de 1 pagina
- [x] Pendientes explícitos documentados
