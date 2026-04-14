# Prompt: E2E Agresivo — Web + Telegram CLI

<!--
target: Claude Code
pressure: aggressive
generated: 2026-04-14
prior-sessions: E2E web PASSED 2026-04-14 (artifacts/e2e/2026-04-14-e2e-web/), E2E Telegram PASSED 2026-04-14 (artifacts/e2e/2026-04-14-e2e-telegram/)
audit-verdict: APPROVED (commit ddca368)
-->

## Misión

Ejecutar un ciclo E2E agresivo de extremo a extremo sobre **producción**, pasando por frontend web y Telegram, y verificando cada paso en la base de datos real. Este ciclo es distinto al anterior: la parte Telegram ya no usa el usuario físico del desarrollador — usa **`mi-telegram-cli`** con una cuenta QA dedicada, lo que permite automatización completa agent-driven sin depender de manos humanas en el teclado.

**Entregable obligatorio:** carpeta `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/` con evidencia de cada paso (screenshots, DB queries, salidas JSON de mi-telegram-cli). La fecha debe ser la del día en que se ejecute.

---

## Estado heredado — No reabrir

- **E2E Telegram (manual) PASSED** 2026-04-14: mood=0, sleep=7, physical=true, social=false, anxiety=false, irritability=false, medication=true — verificado en DB.
- **E2E Web PASSED** 2026-04-14: login magic link → consent → mood entry → DB verificado.
- **Audit APPROVED** (commit `ddca368`): todos los findings C1/H1/H2/M1/M2/M3/L1/L2 resueltos en CT-TELEGRAM-RUNTIME.md, TECH-TELEGRAM.md, RF-REG-013.md.
- **TP-TG cerrado**: TG-P01, TG-N01, TG-P02, TG-N02 todos PASSED con evidencia real.

**Decisiones bloqueadas — no negociar:**
- URLs de producción son inmutables: `https://bitacora.nuestrascuentitas.com` (frontend), `https://api.bitacora.nuestrascuentitas.com` (API), `https://tg-adapter.bitacora.nuestrascuentitas.com` (adapter).
- El host SSH del VPS es **`turismo`** (no `bitacora`). Nunca usar `bitacora` como alias SSH.
- La evidencia va siempre a `artifacts/e2e/YYYY-MM-DD-<slug>/` — nunca en la raíz ni en `tmp/`.
- db-cli verifica en tiempo real con la DB de producción — nunca asumir estado.

---

## Workflow obligatorio (SDD)

```
ps-contexto                    ← PRIMERO, siempre
│
├─ Exploración paralela (5+ ps-explorer en un solo mensaje)
│
├─ brainstorming               ← lock design antes de ejecutar
│
├─ Fase 0: Setup herramientas
├─ Fase 1: E2E Web (Playwright)
├─ Fase 2: E2E Telegram CLI (mi-telegram-cli)
├─ Fase 3: Cierre de evidencia
│
├─ ps-trazabilidad             ← obligatorio antes de marcar done
└─ ps-auditar-trazabilidad     ← obligatorio (ciclo multi-paso, cross-service)
```

---

## Paso 0 — Arranque (ejecutar antes de planificar)

### 0.1 Cargar contexto

```
Skill("ps-contexto")
```

Leer obligatoriamente antes de cualquier acción:
- `.docs/wiki/02_arquitectura.md`
- `.docs/wiki/07_baseline_tecnica.md`
- `.docs/wiki/09_contratos/CT-TELEGRAM-RUNTIME.md`
- `.docs/wiki/07_tech/TECH-TELEGRAM.md`
- `infra/.env.template` — para variables de producción

### 0.2 Exploración paralela (5 subagentes en un solo mensaje)

Lanzar todos en paralelo, sin secuenciar:

1. **ps-explorer** — "Listar rutas del frontend Next.js (`frontend/app/`) y confirmar que `/consent`, `/registro/mood-entry`, `/registro/daily-checkin`, `/configuracion/telegram` existen en producción build"
2. **ps-explorer** — "Confirmar que `src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs` y `src/Bitacora.Application/Commands/Telegram/HandleWebhookUpdateCommand.cs` existen y listar los handlers de cada estado conversacional (Idle, AwaitingFactorSleep, etc.)"
3. **ps-explorer** — "Leer `src/TelegramBotAdapter/app.py` completo y extraer: variables de entorno requeridas, endpoint `/webhook`, lógica de transformación del DTO interno `{Update, ChatId, TraceId, CallbackQueryId}`"
4. **ps-explorer** — "Buscar en `C:\Users\fgpaz\.claude\skills\mi-telegram-cli\` y `C:\Users\fgpaz\.agents\skills\mi-telegram-cli\`: leer el README o SKILL.md de la skill, listar subcomandos disponibles (`profiles`, `auth`, `me`, `dialogs`, `messages`), y extraer el ejemplo exacto para enviar un mensaje a un bot y para enviar un callback_query"
5. **ps-explorer** — "Leer `C:\Users\fgpaz\.claude\skills\db-cli\` o `C:\Users\fgpaz\.agents\skills\db-cli\`: extraer el comando exacto para agregar un alias de conexión PostgreSQL al catálogo global y el comando para ejecutar una query SQL read-only. Verificar si ya existe un alias `bitacora` o `bitacora-prod` en `C:\Users\fgpaz\.db-cli\connections.yaml`"

### 0.3 mi-lsp — verificación semántica

```bash
mi-lsp workspace list
mi-lsp nav search "TelegramConversationState" --workspace humor --format toon
mi-lsp nav search "HandleWebhookUpdateCommandHandler" --workspace humor --format toon
mi-lsp nav search "BuildYesNoKeyboard" --workspace humor --format toon
```

Si el workspace no está registrado: `mi-lsp init . --name humor`

---

## Fase 0 — Setup de herramientas

### F0-A: Configurar alias db-cli para Bitácora

Si no existe el alias `bitacora-prod` en el catálogo global de db-cli:

1. Leer `infra/.env` (o pedir pull via mkey si no existe localmente):
   - Extraer `ConnectionStrings__BitacoraDb` o componer desde `BITACORA_DB_HOST`, `BITACORA_DB_PORT`, `BITACORA_DB_NAME`, `BITACORA_DB_USER`, `BITACORA_DB_PASSWORD`
2. Agregar alias al catálogo:
   ```bash
   # Adaptar el comando exacto según la skill db-cli descubierta en 0.2
   db-cli catalog add --name bitacora-prod --dsn "Host=...;Port=5432;Database=bitacora_db;Username=bitacora;Password=..."
   ```
3. Verificar: `db-cli catalog list` debe mostrar `bitacora-prod`

**Smoke check de conectividad:**
```bash
db-cli query --conn bitacora-prod --sql "SELECT COUNT(*) FROM telegram_sessions WHERE status='Linked'"
```

### F0-B: Verificar mi-telegram-cli

1. Verificar binario accesible:
   ```powershell
   # Windows
   & "$HOME\.agents\skills\mi-telegram-cli\bin\mi-telegram-cli.exe" --help
   # o
   & "$HOME\.claude\skills\mi-telegram-cli\bin\mi-telegram-cli.exe" --help
   ```

2. Verificar perfil QA autenticado:
   ```bash
   mi-telegram-cli profiles list --json
   mi-telegram-cli me --json    # debe retornar el usuario QA, no el desarrollador real
   ```

3. Si el perfil QA no existe: usar `mi-telegram-cli auth` para autenticar la cuenta QA dedicada (requiere `MI_TELEGRAM_API_ID` y `MI_TELEGRAM_API_HASH` del `.env`).

4. Verificar que el bot es alcanzable:
   ```bash
   mi-telegram-cli dialogs list --json | grep -i bitacora   # o el nombre del bot
   ```

---

## Fase 1 — E2E Web (Playwright)

**Objetivo:** verificar el golden path completo desde el browser hasta la DB.

**Herramienta:** MCP Playwright (`mcp__plugin_playwright_playwright__*`)

**URLs de producción:**
- Frontend: `https://bitacora.nuestrascuentitas.com`
- API: `https://api.bitacora.nuestrascuentitas.com`

### F1-1: Landing y login

1. Navegar a `https://bitacora.nuestrascuentitas.com`
2. Screenshot → `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/web-01-landing.png`
3. Localizar campo de email e ingresar email de la cuenta de prueba
4. Hacer click en "Ingresar" / "Enviar link"
5. Verificar mensaje de confirmación ("Revisá tu email" o similar)
6. Screenshot → `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/web-02-magic-link-sent.png`

**Obtener magic link:** Supabase envía el link al email. Para E2E agresivo:
- Opción A: consultar directamente la API de Supabase Admin para obtener el OTP (si el JWT del admin está en `.env`)
- Opción B: usar sshr para ejecutar query en la DB de Supabase: `SELECT confirmation_token FROM auth.users WHERE email='...'`
- Opción C (fallback): preguntar al usuario que ingrese el link manualmente y continuar desde `/onboarding`

Documentar cuál opción se usó en la evidencia.

### F1-2: Consentimiento

1. Una vez autenticado, navegar a `/consent` si no redirige automáticamente
2. Verificar que el panel de consentimiento (`ConsentGatePanel`) es visible
3. Aceptar el consentimiento
4. Screenshot → `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/web-03-consent.png`
5. **Verificar en DB:**
   ```bash
   db-cli query --conn bitacora-prod --sql "SELECT patient_id, status, granted_at_utc FROM consent_grants WHERE status='granted' ORDER BY granted_at_utc DESC LIMIT 1"
   ```
   Guardar salida → `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/db-consent.txt`

### F1-3: Mood Entry

1. Navegar a `/registro/mood-entry`
2. Seleccionar un mood score (usar `+2` para distinguirlo del E2E previo que usó `0`)
3. Confirmar el registro
4. Screenshot → `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/web-04-mood-entry.png`
5. **Verificar en DB:**
   ```bash
   db-cli query --conn bitacora-prod --sql "SELECT mood_entry_id, score, channel, created_at_utc FROM mood_entries WHERE channel='web' ORDER BY created_at_utc DESC LIMIT 1"
   ```
   Guardar → `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/db-mood-entry.txt`

### F1-4: Daily Checkin (web)

1. Si el flujo redirige a `/registro/daily-checkin`, completar los factores
2. Screenshot → `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/web-05-daily-checkin.png`
3. **Verificar en DB:**
   ```bash
   db-cli query --conn bitacora-prod --sql "SELECT daily_checkin_id, checkin_date, safe_projection::text, updated_at_utc FROM daily_checkins ORDER BY updated_at_utc DESC LIMIT 1"
   ```
   Guardar → `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/db-daily-checkin-web.txt`

---

## Fase 2 — E2E Telegram CLI (mi-telegram-cli)

**Objetivo:** ejecutar el flujo completo de registro via Telegram usando la CLI agent-driven, sin intervención humana.

**CRÍTICO — invariante de no-fuga:** Los mensajes del bot no contendrán nunca `encrypted_payload`, `safe_projection`, ni datos clínicos. Si el bot devuelve datos clínicos en claro, es un bug bloqueante — detener el ciclo y reportar.

### F2-1: Obtener pairing code

El pairing code se genera con el JWT del paciente. Opciones:
- Opción A: usar el JWT obtenido en Fase 1 (cookie/localStorage → extraer via Playwright `page.evaluate`)
- Opción B: llamar directamente al API con el Bearer token:
  ```bash
  curl -s -X POST https://api.bitacora.nuestrascuentitas.com/api/v1/telegram/pairing \
    -H "Authorization: Bearer <JWT>" \
    -H "Content-Type: application/json" | jq .
  ```
  Respuesta esperada: `{"code": "BIT-XXXXX", "expires_in_seconds": 900, "expires_at": "..."}`

Guardar el código en variable y en `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/pairing-code.json`

### F2-2: Vincular cuenta Telegram

Enviar `/start BIT-XXXXX` al bot usando mi-telegram-cli:

```bash
# Resolver el peer del bot (obtener su username del .env o docs)
mi-telegram-cli messages send --to @BitacoraBot --text "/start BIT-XXXXX" --json

# Esperar respuesta del bot (~2s) y leer
mi-telegram-cli messages receive --from @BitacoraBot --limit 1 --json
```

Respuesta esperada del bot: `"Cuenta vinculada. Ya podes registrar tu humor desde aca."`

**Verificar en DB:**
```bash
db-cli query --conn bitacora-prod --sql "SELECT chat_id, status, linked_at_utc, conversation_state FROM telegram_sessions WHERE status='Linked' ORDER BY linked_at_utc DESC LIMIT 1"
```
Guardar → `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/db-telegram-session.txt`

### F2-3: Trigger recordatorio (o mood directo)

Opciones para disparar el keyboard de humor:

**Opción A — vía ReminderWorker (si el scheduling está alineado):**
- Verificar `reminder_configs` en DB para la cuenta vinculada
- Esperar hasta que el worker dispare (polling cada 60s)
- Alternativa: usar sshr para verificar logs del worker:
  ```bash
  sshr exec --host turismo --cmd "docker logs <bitacora-api-container> --tail 50 | grep ReminderWorker"
  ```

**Opción B — enviar texto de mood directamente (estado Idle):**
```bash
# El bot acepta texto libre como mood input cuando conversation_state=Idle
mi-telegram-cli messages send --to @BitacoraBot --text "me siento bien" --json
mi-telegram-cli messages receive --from @BitacoraBot --limit 1 --json
```
Respuesta esperada: teclado inline de humor (-3..+3)

### F2-4: Seleccionar mood score

Enviar callback_query simulando tap en el botón `+2`:

```bash
# Adaptar el comando exacto según la API de mi-telegram-cli descubierta en 0.2
mi-telegram-cli messages callback --to @BitacoraBot --data "2" --json
mi-telegram-cli messages receive --from @BitacoraBot --limit 1 --json
```

Respuesta esperada: keyboard de sueño (4h..9h)

### F2-5: Seleccionar horas de sueño

```bash
mi-telegram-cli messages callback --to @BitacoraBot --data "7" --json
mi-telegram-cli messages receive --from @BitacoraBot --limit 1 --json
```

Respuesta esperada: "¿Tuviste actividad física hoy?" con keyboard Sí/No

### F2-6: Factores binarios (5 preguntas)

Repetir para cada factor. Valores: `si` o `no` (minúscula, sin tilde — compatible con `TryParseYesNo`).

```bash
# Actividad física
mi-telegram-cli messages callback --to @BitacoraBot --data "si" --json
mi-telegram-cli messages receive --from @BitacoraBot --limit 1 --json

# Actividad social
mi-telegram-cli messages callback --to @BitacoraBot --data "no" --json
mi-telegram-cli messages receive --from @BitacoraBot --limit 1 --json

# Ansiedad
mi-telegram-cli messages callback --to @BitacoraBot --data "no" --json
mi-telegram-cli messages receive --from @BitacoraBot --limit 1 --json

# Irritabilidad
mi-telegram-cli messages callback --to @BitacoraBot --data "no" --json
mi-telegram-cli messages receive --from @BitacoraBot --limit 1 --json

# Medicación
mi-telegram-cli messages callback --to @BitacoraBot --data "si" --json
mi-telegram-cli messages receive --from @BitacoraBot --limit 1 --json
```

Respuesta final esperada del bot: "Recibido. Lo registramos en tu bitácora." o similar.

### F2-7: Verificar DailyCheckin en DB

```bash
db-cli query --conn bitacora-prod --sql "
SELECT
  daily_checkin_id,
  checkin_date,
  safe_projection::text,
  created_at_utc,
  updated_at_utc
FROM daily_checkins
ORDER BY updated_at_utc DESC
LIMIT 1"
```

**Valores esperados:**
- `mood_score`: 2 (el valor enviado en F2-4)
- `sleep_hours`: 7
- `has_physical`: true
- `has_social`: false
- `has_anxiety`: false
- `has_irritability`: false
- `has_medication`: true

Guardar → `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/db-daily-checkin-telegram.txt`

También verificar que `conversation_state = 0` (Idle):
```bash
db-cli query --conn bitacora-prod --sql "SELECT conversation_state, pending_factors_json FROM telegram_sessions WHERE status='Linked' ORDER BY linked_at_utc DESC LIMIT 1"
```

---

## Fase 3 — Cierre de evidencia

### F3-1: Resumen de evidencia

Crear `artifacts/e2e/YYYY-MM-DD-e2e-agresivo/evidencia-resumen.md` con:
- Flujo web ejecutado (pasos F1-1..F1-4 con timestamps UTC)
- Flujo Telegram CLI ejecutado (pasos F2-1..F2-7 con timestamps UTC)
- Lista de archivos de evidencia generados
- Estado PASSED / FAILED por fase

### F3-2: ps-trazabilidad

```
Skill("ps-trazabilidad")
```

Verificar específicamente:
- RF-REG-012 y RF-REG-013 — ¿tienen campo `Estado: Implementado` en su Execution Sheet? Si no, actualizar.
- `06_matriz_pruebas_RF.md` — registrar el nuevo ciclo E2E como evidencia de cobertura
- Si este ciclo produce resultados PASSED nuevos, actualizarlos en TP-TG.md

### F3-3: ps-auditar-trazabilidad

```
Skill("ps-auditar-trazabilidad")
```

Scope: módulo Telegram + flujo web → DB. Verificar que no surgió drift nuevo.

---

## Árbol de decisión ante fallos

| Fallo | Acción |
|-------|--------|
| Bot no responde al `/start` | Verificar `docker ps` en turismo via sshr, logs del adapter y del API |
| Keyboard no aparece | Verificar `conversation_state` en DB; revisar logs de `HandleWebhookUpdateCommandHandler` |
| `reply_markup:null` 400 | Ya resuelto en commit `303a993` — si reaparece, verificar que el contenedor está en la versión correcta |
| DB query falla | Verificar alias `bitacora-prod` en db-cli; re-configurar desde infra/.env |
| mi-telegram-cli no puede enviar callback | Leer docs de la skill para el formato exacto de callback — adaptar comando |
| Frontend no carga | Verificar Dokploy via `dkp.ps1 status <appId>` y sshr `docker ps` en turismo |
| Magic link no llega | Usar Supabase Admin API para extraer OTP, o pedir intervención manual |

---

## Paths de referencia rápida

```
Frontend prod:          https://bitacora.nuestrascuentitas.com
API prod:               https://api.bitacora.nuestrascuentitas.com
Bot adapter prod:       https://tg-adapter.bitacora.nuestrascuentitas.com
VPS SSH:                turismo (54.37.157.93)
Bot adapter src:        src/TelegramBotAdapter/app.py
Webhook handler:        src/Bitacora.Application/Commands/Telegram/HandleWebhookUpdateCommand.cs
Frontend login:         frontend/lib/auth/client.ts → signInWithMagicLink()
Frontend mood:          frontend/components/patient/mood/MoodEntryForm.tsx
CT-TELEGRAM-RUNTIME:    .docs/wiki/09_contratos/CT-TELEGRAM-RUNTIME.md
TECH-TELEGRAM:          .docs/wiki/07_tech/TECH-TELEGRAM.md
Evidencia anterior:     artifacts/e2e/2026-04-14-e2e-telegram/
mi-telegram-cli:        ~/.agents/skills/mi-telegram-cli/ (o ~/.claude/skills/mi-telegram-cli/)
db-cli:                 ~/.agents/skills/db-cli/ (o ~/.claude/skills/db-cli/)
sshr:                   ~/.agents/skills/ssh-remote/scripts/sshr.ps1
```

---

## Reglas de presión

- **No asumir estado**: verificar en DB después de cada paso, no confiar en respuestas del bot solas.
- **No inventar comandos de mi-telegram-cli**: leer la skill completa en Paso 0.2 antes de invocar.
- **No saltear ps-trazabilidad**: el ciclo no está cerrado hasta que ps-trazabilidad y ps-auditar-trazabilidad corren con veredicto APPROVED.
- **Evidencia es obligatoria**: si un paso no tiene screenshot o DB query guardado, ese paso no cuenta como ejecutado.
- **Fail fast**: si la Fase 0 (setup de herramientas) falla, detener y reportar antes de ejecutar el E2E.
