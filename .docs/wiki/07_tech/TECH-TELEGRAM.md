# TECH-TELEGRAM: Bot de Telegram — Modos y Flujo Conversacional

> Root: `07_baseline_tecnica.md` invariante #4.

## Estado actual

`Implementado (Phase 30+)`.

El runtime de Telegram esta materializado: webhook entrypoint, pairing code, session state y `ReminderWorker` activo. `SendTelegramMessageAsync` hace POST real a Telegram Bot API via `HttpClient`. Los recordatorios scheduler (RF-TG-010..012) estan implementados en Phase 30+.

## Configuracion dual (T3-3)

| Entorno | Modo | Endpoint | Configuracion |
|---------|------|----------|--------------|
| Produccion | Webhook | POST /api/v1/telegram/webhook | Registrado via Telegram Bot API setWebhook |
| Desarrollo | Long-polling | (interno) | GetUpdatesReceiver, sin HTTPS |

### Variables de entorno

```env
TELEGRAM_BOT_TOKEN=<token>
TELEGRAM_WEBHOOK_URL=https://tg-adapter.bitacora.nuestrascuentitas.com/webhook
TELEGRAM_USE_WEBHOOK=true  # false para dev (long-polling)
```

**Arquitectura del webhook (produccion):** Telegram Bot API → `tg-adapter.bitacora.nuestrascuentitas.com/webhook` (bot adapter Python/FastAPI) → `api.bitacora.nuestrascuentitas.com/api/v1/telegram/webhook` (con header `X-Telegram-Webhook-Secret`). El adapter transforma el JSON nativo de Telegram al DTO interno `{Update, ChatId, TraceId, CallbackQueryId}`.

## Flujo conversacional (registro rapido)

```text
Bot: "¿Como te sentis ahora?"
[Keyboard inline: +3 | +2 | +1 | 0 | -1 | -2 | -3]
→ Paciente tap → Crear MoodEntry (RF-REG-012)

Bot: "¿Cuantas horas dormiste?"
[Keyboard: 4h | 5h | 6h | 7h | 8h | 9h]
→ Paciente tap → Acumular en estado conversacional

Bot: "¿Hiciste actividad fisica?"
[Keyboard: Si | No]

Bot: "¿Tomaste la medicacion?"
[Keyboard: Sí | No]

Bot: "Registrado. Tu humor hoy: +1. ¡Buen dia!"
→ Crear DailyCheckin (RF-REG-013)
```

### Estado conversacional

- Se mantiene en `telegram_sessions.conversation_state` (enum TelegramConversationState) y `telegram_sessions.pending_factors_json` (JSONB acumulador de factores)
- Persistido en DB tras cada paso; sobrevive reinicios del contenedor
- En memoria: Dictionary<chatId, TelegramFactorAccumulator> re-hidratado desde `pending_factors_json` si el contenedor se reinicia
- Si el paciente no completa los factores, solo se registra el MoodEntry; `conversation_state` queda != Idle hasta completar o reiniciar

## Recordatorios (RF-TG-010..012) — Phase 31+

- Background hosted service (.NET IHostedService)
- Timer cada 1 minuto: query ReminderConfig WHERE next_fire_at <= now()

### Fail-closed reminder logic (T3-RL-02, T3-RL-03)

```
1. check ConsentGrant activo → skip si revoked (T3-RL-03)
2. check TelegramSession.linked → skip si unlinked (T3-RL-03)
3. check throttle: ya se envio recordatorio hoy? → skip si ya se envio (T3-RL-02)
4. retry loop: max 3 intentos con backoff exponencial
5. AccessAudit solo si envio exitoso
6. Nunca registrar encrypted_payload en logs de recordatorio
```

### Reminder throttle (T3-RL-02)

**Max 1 recordatorio por paciente por dia**, independientemente de la configuracion en ReminderConfig.

El throttle se aplica en el ReminderWorker a nivel de scheduling. Si ReminderConfig tiene configurado multiple recordatorios en el mismo dia, solo el primero se ejecuta; los demais se skipping sin generar AccessAudit de intento.

### Recordatorio skip conditions

| Condition | Accion | AccessAudit |
|-----------|--------|-------------|
| ConsentGrant revocado | Skip | No (T3-RL-03: no generar audit de intento) |
| TelegramSession unlinked | Skip | No |
| Ya se envio recordatorio hoy (throttle) | Skip | No |
| Telegram API falla (max 3 retries) | Skip con warning | No |

## Vinculacion (RF-TG-001..003)

```text
Web: POST /api/v1/telegram/pairing → {code: "BIT-7K2Q9", expires_in: 900}
Paciente: envia /start BIT-7K2Q9 al bot
Webhook: valida code, crea TelegramSession(patient_id, chat_id, linked)
Bot: "Cuenta vinculada. Ya podes registrar tu humor desde aca."
```

## Runtime safety limits

| Limite | Valor | Regla | Justificacion |
|--------|-------|-------|---------------|
| Rate limit envio | 30 msg/segundo (Telegram Bot API) | SendTelegramMessageAsync con semaforo | Evitar bloqueo de API |
### Retry en recordatorios

**Exponential backoff:** `SendTelegramMessageAsync` implements 3 retries with exponential backoff — 1s, 2s, 4s — before returning false. Errores de un recordatorio no bloquean el procesamiento de los demas.

| Attempt | Delay |
|---------|-------|
| 1 | 1s |
| 2 | 2s |
| 3 | 4s |
| Estado conversacional | Persistido en DB (`conversation_state` + `pending_factors_json`) | telegram_sessions | Sobrevive reinicios del contenedor |
| Pairing code TTL | 15 minutos | ReminderWorker genera codigo | Limitar ventana de ataque |
| chat_id unico | Un `chat_id` por paciente | TelegramSession unique constraint | Rechazar vinculacion multiple |
| Recordatorios por paciente/dia | Max 1 | T3-RL-02 throttle en ReminderWorker | No saturar al paciente |
| Consent check por recordatorio | ConsentGrant activo | T3-RL-03 ReminderWorker checkea antes de cada envio | Revocacion inmediata |
| Datos clinicos en respuestas bot | Prohibido | T3-11衍生 | encrypted_payload, safe_projection, y cualquier derivado no puede salir por Telegram |
| Webhook signature | HMAC-SHA256 requerido + header X-Telegram-Bot-Api-Secret-Token | TECH-ROLLOUT-Y-OPERABILIDAD.md | Rechazar payloads sin firma valida |

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
