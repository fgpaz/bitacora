# TP-TG — Plan de Pruebas del Modulo TG

## Alcance

- RF cubiertos: RF-TG-001..003, RF-TG-010..012
- Flujos origen: FL-TG-01, FL-TG-02

## Estado de ejecucion actual

- `Parcialmente ejecutado` — TG-P01 y TG-N01 ejecutados y aprobados en produccion (E2E 2026-04-14).
- TG-P02 y TG-N02 (scheduler/recordatorios) pendientes de ejecucion; el scheduler esta implementado pero no se ejecuto en este ciclo de prueba.

### Resultados de ejecucion

| TC ID | Estado | Ambiente | Fecha | Evidencia |
|-------|--------|----------|-------|-----------|
| TG-P01 | PASSED | produccion | 2026-04-14 | Pairing con BIT-RGSG2, TelegramSession linked, DailyCheckin persistido |
| TG-N01 | PASSED | produccion | 2026-04-14 | /start con codigo invalido rechazado; consentimiento previo requerido |
| TG-P02 | PENDIENTE | — | — | Scheduler ReminderWorker no ejecutado en este ciclo |
| TG-N02 | PENDIENTE | — | — | Skip conditions no verificadas |

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| TG-P01 | RF-TG-001, RF-TG-002, RF-TG-003 | Positivo | Pairing completo con codigo BIT-XXXXX y chat_id unico |
| TG-N01 | RF-TG-001, RF-TG-002, RF-TG-003 | Negativo | Rechaza generacion sin consentimiento y /start con codigo invalido o chat duplicado |
| TG-P02 | RF-TG-010, RF-TG-011, RF-TG-012 | Positivo | Scheduler envia recordatorio con keyboard inline a sesiones validas |
| TG-N02 | RF-TG-010, RF-TG-011, RF-TG-012 | Negativo | Skip si consentimiento revocado, session unlinked o falta token del bot |

## Gherkin expandido

```gherkin
Scenario: Pairing Telegram exitoso de punta a punta
  Given patient autenticado con consentimiento vigente
  When POST /api/v1/telegram/pairing
  Then se retorna code="BIT-XXXXX" con expires_in=900
  When el usuario envia /start con ese codigo desde un chat nuevo
  Then se crea TelegramSession linked
  And el codigo queda consumido

Scenario: Pairing rechazado por codigo invalido o chat duplicado
  Given existe un chat_id ya vinculado a otro paciente
  When llega /start con codigo invalido o expirado desde ese chat
  Then no se crea una nueva TelegramSession
  And el bot responde guidance al usuario

Scenario: Scheduler omite recordatorios sin condiciones de envio
  Given existe ReminderConfig activo
  And el paciente tiene consentimiento revocado o TelegramSession unlinked
  When corre el scheduler
  Then no se envia mensaje al bot
  And el skip queda trazado en logs operacionales
```

## Criterios de salida

- Cobertura positiva y negativa de los 6 RF del modulo.
- Evidencia de pairing seguro y de scheduler con skips correctos.
