# HANDOFF-SPEC-TG-002 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `TG-002` para el equipo de backend/telegram.

## Referencias fuente

- `../UI-RFC/UI-RFC-TG-002.md`
- `../UXS/UXS-TG-002.md`
- `../VOICE/VOICE-TG-002.md`
- `../PROTOTYPE/PROTOTYPE-TG-002.md`
- `../../07_tech/TECH-TELEGRAM.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-TG.md`
- `../../06_pruebas/TP-REG.md`
- `../../03_FL/FL-TG-02.md`
- `../../03_FL/FL-REG-02.md`
- `../../04_RF/RF-TG-010.md`
- `../../04_RF/RF-TG-011.md`
- `../../04_RF/RF-TG-012.md`
- `../../04_RF/RF-REG-010.md`
- `../../04_RF/RF-REG-011.md`
- `../../04_RF/RF-REG-012.md`
- `../../04_RF/RF-REG-013.md`
- `../../04_RF/RF-REG-014.md`
- `../../04_RF/RF-REG-015.md`

## Alcance implementable

### In

- background service de recordatorios (`IHostedService` en .NET);
- timer cada 1 minuto consultando `ReminderConfig WHERE next_fire_at <= now()`;
- gate de consentimiento y sesión antes de enviar;
- envío de mensaje via Telegram API con keyboard inline;
- parsing de respuesta del usuario via webhook;
- registro de `MoodEntry` y `DailyCheckin` via API;
- logging de auditoría para cada recordatorio y cada respuesta.

### Out

- vinculación de cuenta (delegado a `TG-001`);
- UI web de configuración de recordatorios (delegada a slice web);
- lógica de programación de recordatorios (fuera del scope conversacional).

## Estados que deben existir

1. `reminder_sent` — pregunta de humor enviada, esperando respuesta
2. `reply_submitting` — respuesta en envío a la API
3. `reply_success` — confirmación breve enviada
4. `factors_prompt` — pregunta opcional de factor (sueño)
5. `reply_error` — error recuperable enviado
6. `reminder_skipped` — usuario eligió `Ahora no` (silencio útil)
7. `unrecognized` — mensaje no esperado del usuario

## Arbol de transiciones principal

```
daily_reminder trigger (timer)
  ├─ gate: consentimiento activo + sesión vinculada
  │   └─ enviar reminder → reminder_sent
  └─ gate fallido → no enviar (silencio)

reminder_sent + respuesta humor
  └─ reply_submitting
      ├─ 201 → reply_success → factors_prompt
      └─ error → reply_error → permite reintentar

reminder_sent + "Ahora no"
  └─ reminder_skipped → cierre sin presión

factors_prompt + respuesta factor
  └─ reply_success → cierre

factors_prompt + "Ahora no"
  └─ reminder_skipped → cierre sin presión

reply_error + reintento
  └─ reply_submitting → reevalúa

<mensaje no reconocido> → unrecognized → recordatorio de formato
```

## Restricciones cerradas

- no usar phrasing prohibido: `No te olvides de registrarte`, `Es importante que respondas ahora`, `Seguimos esperando`, `No cumples con tu registro`, `pendiente`
- no enviar recordatorio si `ConsentGrant` está revocada
- no enviar recordatorio si `TelegramSession` no está vinculada
- no registrar respuesta si el usuario eligió `Ahora no`
- Telegram API rate limit: max 30 mensajes/segundo; retry con backoff exponencial (max 3 intentos)
- no confirmar con emoji o tono enfático
- keyboard inline: máximo 8 opciones por fila
- el estado `reply_submitting` debe proteger contra double-tap

## Contratos de transición

- el background service consume `ReminderConfig` y llama directo a Telegram API
- el webhook de respuesta recibe el callback query del inline keyboard
- `POST /api/v1/mood-entries` para registrar humor
- `POST /api/v1/daily-checkin` para registrar factores (parcial permitido)
- el flujo conversacional no persiste estado entre sesiones; usa memoria efímera con TTL 10 min

## Blockers explícitos ya resueltos

- gap map `2026-04-10` aprueba la apertura de UI-RFC y HANDOFF para `TG-002`
- `TelegramSession`, `ReminderConfig` y los seams de recordatorio estan materializados en Phase 30+ segun `TECH-TELEGRAM.md`
- la validacion UX real queda diferida a `Phase 60`
- el runtime de Telegram existe; la implementacion de backend consume este contrato como especificacion funcional

## Dependencias para implementación

- `TelegramSession` entity y tabla deben existir según `TECH-TELEGRAM.md`
- `ReminderConfig` entity y tabla deben existir
- `MoodEntry` y `DailyCheckin` endpoints deben existir y aceptar `source: "telegram"`
- `TelegramBotToken` configurado en environment
- webhook registrado para recibir callbacks del inline keyboard
- logging structurado para auditoría de cada recordatorio enviado y cada respuesta recibida

## Contrato de copy aprobado

| Estado | Copy |
| --- | --- |
| pregunta de humor | `¿Cómo te sentís ahora?` |
| keyboard | `[+3] [+2] [+1] [0] [-1] [-2] [-3]` + `[Ahora no]` |
| confirmación registro | `Registrado: +1.` |
| factors prompt | `Si querés, podés contarme cuántas horas dormiste.` + keyboard de horas + `[Ahora no]` |
| cierre final | `¡Buen día!` |
| error de registro | `No pudimos registrar esa respuesta. Probá de nuevo si querés.` |
| no reconocido | `No entendimos ese mensaje. Usá /registrar para registrar tu humor o esperá tu próximo recordatorio.` |

## Momentos auditables

| Momento | Datos logueados |
| --- | --- |
| recordatorio enviado | patient_id, reminder_config_id, timestamp, resultado_telegram |
| respuesta recibida | patient_id, mood_value, timestamp |
| registro creado | mood_entry_id, patient_id, mood_value, source=telegram |
| factores registrados | daily_checkin_id, patient_id, factors, source=telegram |
| omisión (`Ahora no`) | patient_id, timestamp, sin detalle del valor |
| error de API | patient_id, endpoint, error_code, timestamp |

## Done when de handoff

El handoff spec está bien consumido si backend puede implementar el flujo de recordatorio sin tener que decidir:

- qué dice el bot en cada estado;
- cómo se estructura el keyboard inline;
- cuándo se gatingue el envío por consentimiento o sesión;
- qué copy está prohibido;
- qué se loguea como momento auditable;
- cómo se maneja el silencio del usuario (`Ahora no`);
- cuándo se corta el flujo sin presión.

## Validación diferida

- este documento no reemplaza `UX-VALIDATION-TG-002.md`
- la evidencia UX real solo llega cuando el bot tenga runtime materializado
- hasta entonces, el contrato conversacional se mantiene como especificación objetivo

---

**Estado:** listo para consumo por el equipo de backend/telegram.
**Siguiente artefacto:** `HANDOFF-ASSETS-TG-002.md` (si aplica), `HANDOFF-MAPPING-TG-002.md` (si aplica), `HANDOFF-VISUAL-QA-TG-002.md` (si aplica).
**Validación UX real:** diferida a `Phase 60`.
