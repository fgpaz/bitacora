# TECH-TELEGRAM: Bot de Telegram — Modos y Flujo Conversacional

> Root: `07_baseline_tecnica.md` invariante #4.

## Estado actual

`Implementado (Phase 30+)`.

El runtime de Telegram esta materializado: webhook entrypoint, pairing code, session state y `ReminderWorker` activo. `SendTelegramMessageAsync` hace POST real a Telegram Bot API via `HttpClient`. Los recordatorios scheduler (RF-TG-010..012) quedan diferidos a Phase 31+.

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
Web: POST /api/v1/telegram/pairing → {code: "BIT-7K2Q9", expires_in: 900}
Paciente: envia /start BIT-7K2Q9 al bot
Webhook: valida code, crea TelegramSession(patient_id, chat_id, linked)
Bot: "Cuenta vinculada. Ya podes registrar tu humor desde aca."
```

## Runtime safety limits

| Limite | Valor | Justificacion |
|--------|-------|---------------|
| Rate limit envio | 30 msg/segundo (Telegram Bot API) | Evitar bloqueo de API |
| Retry en recordatorios | Max 3 intentos con backoff exponencial | Evitar spam en outage |
| TTL estado conversacional | 10 minutos de inactividad | No persiste estado efimero en DB |
| Pairing code TTL | 15 minutos | Limitar ventana de ataque |
| chat_id unico | Un `chat_id` por paciente | Rechazar vinculacion multiple |
| Datos clinicos en respuestas bot | Prohibido | encrypted_payload, safe_projection, y cualquier derivado no puede salir por Telegram |
| Webhook signature | HMAC-SHA256 requerido | Rechazar payloads sin firma valida |

## Invariantes de no-fuga (Telegram y canales externos)

1. **encrypted_payload no sale del proceso .NET** bajo ninguna circunstancia.
2. **safe_projection no se incluiye en respuestas del bot.** El bot solo confirma receipt y pide siguiente input.
3. **Export CSV es solo para el paciente owner.** El endpoint RF-EXP-001 requiere JWT del paciente.
4. **Descifrado para export ocurre en memoria y se transmite como attachment**; no se almacena descifrado en disco ni caches intermedias.
5. **Logs del modulo Telegram no contienen safe_projection ni encrypted_payload.** Solo session ID y trace_id.
6. **Consent revocation o session unlink corta inmediatamente el recordatorio** sin enviar datos clinicos.

## Sync gates

Cambios en Telegram fuerzan revision de:
- RF-REG-010..013 (webhook y registro via bot)
- RF-TG-001..012 (vinculacion y recordatorios)
- 09_contratos_tecnicos.md (endpoint de webhook)
- 06_matriz_pruebas_RF.md (smoke y gates Telegram)
