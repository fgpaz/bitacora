# TECH-TELEGRAM: Bot de Telegram — Modos y Flujo Conversacional

> Root: `07_baseline_tecnica.md` invariante #4.

## Configuracion dual (T3-3)

| Entorno | Modo | Endpoint | Configuracion |
|---------|------|----------|--------------|
| Produccion | Webhook | POST /api/v1/telegram/webhook | Registrado via Telegram Bot API setWebhook |
| Desarrollo | Long-polling | (interno) | GetUpdatesReceiver, sin HTTPS |

### Variables de entorno

```env
TELEGRAM_BOT_TOKEN=<token>
TELEGRAM_WEBHOOK_URL=https://bitacora.nuestrascuentitas.com/api/v1/telegram/webhook
TELEGRAM_USE_WEBHOOK=true  # false para dev (long-polling)
```

## Flujo conversacional (registro rapido)

```text
Bot: "¿Como te sentis ahora?"
[Keyboard inline: +3 | +2 | +1 | 0 | -1 | -2 | -3]
→ Paciente tap → Crear MoodEntry (RF-REG-012)

Bot: "¿Cuantas horas dormiste?"
[Keyboard: <4h | 4-6h | 6-8h | 8+h]
→ Paciente tap → Acumular en estado conversacional

Bot: "¿Hiciste actividad fisica?"
[Keyboard: Si | No]

Bot: "¿Tomaste la medicacion?"
[Keyboard: Si | No | No tomo]

Bot: "Registrado. Tu humor hoy: +1. ¡Buen dia!"
→ Crear DailyCheckin (RF-REG-013)
```

### Estado conversacional

- Se mantiene en memoria (Dictionary<chatId, ConversationState>)
- TTL: 10 minutos de inactividad → se descarta
- No se persiste en DB (efimero)
- Si el paciente no completa los factores, solo se registra el MoodEntry

## Recordatorios (RF-TG-010..012)

- Background hosted service (.NET IHostedService)
- Timer cada 1 minuto: query ReminderConfig WHERE next_fire_at <= now()
- Skip si ConsentGrant revocado o TelegramSession unlinked
- Telegram API rate limit: max 30 mensajes/segundo
- Retry con backoff exponencial (max 3 intentos)

## Vinculacion (RF-TG-001..003)

```text
Web: POST /api/v1/telegram/pairing → {code: "BIT-A7X3K2", expires_in: 900}
Paciente: envia /start BIT-A7X3K2 al bot
Webhook: valida code, crea TelegramSession(patient_id, chat_id, linked)
Bot: "Cuenta vinculada. Ya podes registrar tu humor desde aca."
```

## Sync gates

Cambios en Telegram fuerzan revision de:
- RF-REG-010..013 (webhook y registro via bot)
- RF-TG-001..012 (vinculacion y recordatorios)
- 09_contratos_tecnicos.md (endpoint de webhook)
