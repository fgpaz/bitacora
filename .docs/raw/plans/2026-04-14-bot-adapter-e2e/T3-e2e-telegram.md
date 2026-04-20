# Task T3: E2E Telegram Real — /start → Pairing → Keyboard Mood → Keyboard Sleep → DailyCheckin DB

## Shared Context
**Goal:** Ejecutar el flujo E2E completo de Telegram con usuario real: pairing, mood via keyboard inline, sleep via keyboard inline, verificar DailyCheckin en DB.
**Stack:** Telegram app real (acción manual del usuario) + API de verificación via sshr/psql + Playwright para la parte web del pairing.
**Architecture:** Usuario real envía /start con código BIT-XXXXX desde Telegram → adapter transforma → API crea TelegramSession → usuario recibe keyboard inline de humor → toca un botón → recibe keyboard de sueño → toca un botón → DailyCheckin creado.
**Depende de:** T1 (adapter en producción, webhook reconfigurado).

## Task Metadata
```yaml
id: T3
depends_on: [T1]
agent_type: ps-worker
files:
  - read: infra/.env
complexity: high
done_when: "SELECT * FROM daily_checkins ORDER BY created_at DESC LIMIT 1 retorna fila con mood_score NOT NULL y sleep_hours NOT NULL creada en los últimos 10 minutos; telegram_sessions tiene una fila con status='linked' y chat_id del usuario real"
```

## Reference
- API: `https://api.bitacora.nuestrascuentitas.com`
- DB: `<redacted-postgres-uri>`
- Adapter: `https://tg-adapter.bitacora.nuestrascuentitas.com`
- sshr: `C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1`
- Bot: @<nombre-del-bot> (verificar con `/getMe`)

## Prompt

**IMPORTANTE:** Este test requiere acción manual en la app de Telegram. El executor debe coordinar con el usuario (Gabriel) para ejecutar los pasos en el teléfono real.

### Paso 0: Pre-verificación del estado de la DB

Verificar el estado actual de `telegram_sessions` para el usuario de test:

```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker exec \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) \
   psql '<redacted-postgres-uri>' \
   -c 'SELECT id, patient_id, chat_id, status, linked_at FROM telegram_sessions ORDER BY linked_at DESC;'"
```

**Si existe una sesión con `chat_id=<redacted-simulated-chat-id>` y `status=linked`:**
Esta fue creada por simulación y debe ser limpiada para que el pairing real funcione (la constraint es `UNIQUE(patient_id)`).

```bash
# LIMPIAR LA SESIÓN FALSA (confirmar con el usuario antes de ejecutar)
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker exec \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) \
   psql '<redacted-postgres-uri>' \
   -c \"UPDATE telegram_sessions SET status='unlinked' WHERE chat_id='<redacted-simulated-chat-id>';\""
```

Verificar también el `ReminderConfig` existente:
```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker exec \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) \
   psql '<redacted-postgres-uri>' \
   -c 'SELECT id, patient_id, enabled, next_fire_at_utc FROM reminder_configs;'"
```

### Paso 1: Obtener el nombre del bot

```bash
curl -sS "https://api.telegram.org/bot<redacted-telegram-bot-token>/getMe" | python3 -m json.tool
```
Anotar el `username` del bot (ej: `@BitacoraBot`).

### Paso 2: Generar código de pairing para el usuario de test

El usuario de test necesita estar autenticado en la web. Usar el JWT del usuario de test para hacer el POST de pairing. Si no se tiene el JWT, se puede hacer el pairing desde la web usando Playwright (navegar a la sección de configuración de Telegram en la UI).

**Opción A: Via UI web con Playwright:**
1. Login en `https://bitacora.nuestrascuentitas.com` con el usuario de test
2. Ir a configuración/perfil/Telegram
3. Hacer clic en "Vincular Telegram" o similar
4. Copiar el código BIT-XXXXX generado

**Opción B: Via API directamente (si se tiene JWT):**
```bash
curl -sS -X POST \
  -H "Authorization: Bearer <JWT_DEL_USUARIO_TEST>" \
  -H "Content-Type: application/json" \
  https://api.bitacora.nuestrascuentitas.com/api/v1/telegram/pairing
```
Response: `{"code": "BIT-XXXXX", "expires_in": 900}`

### Paso 3: Pairing via Telegram (ACCIÓN MANUAL)

**El executor debe pedirle al usuario (Gabriel) que:**
1. Abra la app de Telegram en su teléfono
2. Busque el bot @<username_del_bot>
3. Envíe el mensaje: `/start BIT-XXXXX` (usando el código generado en el paso 2)
4. El bot debe responder: "Cuenta vinculada. Ya podés registrar tu humor desde acá."

**Mientras tanto, monitorear logs del adapter:**
```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker logs tg-adapter --tail 20 --follow"
```

**Monitorear logs del API:**
```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker logs \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) --tail 30"
```

### Paso 4: Verificar TelegramSession creada

```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker exec \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) \
   psql '<redacted-postgres-uri>' \
   -c 'SELECT id, patient_id, chat_id, status, linked_at FROM telegram_sessions ORDER BY linked_at DESC LIMIT 3;'"
```

Verificar que:
- Hay una nueva fila con `status='linked'`
- El `chat_id` es el ID real del usuario (NO `<redacted-simulated-chat-id>`)
- `linked_at` es reciente

### Paso 5: Crear ReminderConfig para el usuario real

```bash
# Obtener patient_id del usuario de test
PATIENT_ID=$(sshr exec --host bitacora --cmd "docker exec <container> psql '<conn>' -t -c 'SELECT id FROM patients LIMIT 1;'" | tr -d ' ')

# Crear ReminderConfig con next_fire en 2 minutos para testing inmediato
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker exec \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) \
   psql '<redacted-postgres-uri>' \
   -c \"INSERT INTO reminder_configs (id, patient_id, enabled, next_fire_at_utc, cron_expression, created_at, updated_at) \
        VALUES (gen_random_uuid(), '<PATIENT_ID>', true, NOW() + INTERVAL '2 minutes', '0 20 * * *', NOW(), NOW());\""
```

Alternativamente, si ya existe un `ReminderConfig` para este patient_id, actualizar su `next_fire_at_utc`:
```sql
UPDATE reminder_configs SET next_fire_at_utc = NOW() + INTERVAL '2 minutes', enabled = true WHERE patient_id = '<PATIENT_ID>';
```

### Paso 6: Esperar que el scheduler envíe el recordatorio

El scheduler (`ReminderWorker`) corre cada 60 segundos. Después de ~2 minutos, debe:
1. Encontrar el ReminderConfig
2. Verificar consent activo
3. Verificar TelegramSession linked
4. Enviar el mensaje con keyboard inline de humor (-3 a +3)

**Monitorear logs:**
```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker logs \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) --tail 50 2>&1 | grep -i 'reminder\|scheduler'"
```

Esperado: `Reminder sent for patient <ID>, next fire at <DATE>`

### Paso 7: Responder al recordatorio (ACCIÓN MANUAL en Telegram)

**El usuario (Gabriel) en Telegram debe:**
1. Recibir el mensaje del bot con la pregunta de humor y los botones (-3, -2, -1, 0, +1, +2, +3)
2. Tocar uno de los botones (ej: +2)
3. Esperar la siguiente pregunta: "¿Cuántas horas dormiste anoche?" con botones (4h-9h)
4. Tocar una opción de horas de sueño (ej: 7h)
5. Continuar el flujo hasta que el bot confirme el registro

Si hay más factores en el flujo (físico, social, ansiedad, irritabilidad, medicación), completarlos también.

**Monitorear logs del API durante la interacción:**
```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker logs \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) --tail 50 2>&1 | grep -i 'webhook\|daily\|checkin\|mood\|sleep'"
```

### Paso 8: Verificar DailyCheckin en DB

```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker exec \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) \
   psql '<redacted-postgres-uri>' \
   -c 'SELECT id, patient_id, mood_score, sleep_hours, checkin_date, created_at FROM daily_checkins ORDER BY created_at DESC LIMIT 3;'"
```

Verificar que:
1. Hay una fila nueva con `created_at` en los últimos 10 minutos
2. `mood_score` coincide con el botón tocado
3. `sleep_hours` coincide con las horas seleccionadas
4. `patient_id` es el usuario de test

### Paso 9: Guardar evidencia

Crear `artifacts/e2e/2026-04-14-e2e-telegram/` con:
- `adapter-logs.txt` — logs del adapter durante el flujo
- `api-logs.txt` — logs del API durante el flujo
- `db-telegram-session.txt` — resultado del SELECT en telegram_sessions
- `db-daily-checkin.txt` — resultado del SELECT en daily_checkins
- Screenshots de Telegram si es posible (del teléfono del usuario)

### Notas de troubleshooting:

**Si el adapter no recibe el update de Telegram:**
- Verificar `getWebhookInfo` — debe mostrar la URL del adapter y `pending_update_count: 0`
- Verificar que el adapter esté running: `docker ps | grep tg-adapter`
- Revisar logs del adapter: `docker logs tg-adapter --tail 50`

**Si el API rechaza el request del adapter:**
- El adapter envía `X-Telegram-Webhook-Secret: Hhl43GhDDyL0jDuoJknq8HD0UB3ukQ2HjqDDDVygZM57GHm`
- Verificar que el secreto del API coincide: `printenv | grep WebhookSecretToken`

**Si el bot responde "Código inválido o expirado":**
- El código BIT-XXXXX dura 15 minutos — regenerar si venció
- Verificar que la sesión falsa con `chat_id=<redacted-simulated-chat-id>` fue desvinculada

**Si el scheduler no envía el recordatorio:**
- Verificar que `ReminderConfig.enabled=true` y `next_fire_at_utc <= NOW()`
- Verificar consent activo: `SELECT * FROM consent_grants WHERE patient_id='<ID>' AND revoked_at IS NULL`
- Revisar logs: `docker logs <container> 2>&1 | grep -i reminder`

## Verify
```sql
-- Ejecutar via sshr en el container:
SELECT mood_score, sleep_hours, created_at 
FROM daily_checkins 
ORDER BY created_at DESC 
LIMIT 1;
```
Output esperado: `mood_score` entre -3 y 3, `sleep_hours` entre 4 y 9, `created_at` en los últimos 10 minutos.

## Commit
```
git add artifacts/e2e/2026-04-14-e2e-telegram/
git commit -m "test(e2e): Telegram real user E2E evidence — pairing→keyboard inline→daily checkin 2026-04-14"
```
