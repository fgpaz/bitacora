# Prompt: E2E Full Producción — Post-Auth-Fix (2026-04-15)

<!--
target: Claude Code
pressure: aggressive
generated: 2026-04-15
scope: todos los flujos y journeys del producto — primer E2E real post-fix de auth
prior-evidence: artifacts/e2e/2026-04-14-e2e-agresivo/evidencia-resumen.md
-->

## Misión

Ejecutar el primer ciclo E2E agresivo completo de Bitácora **post-fix de auth** (2026-04-15).

La instancia GoTrue dedicada (`auth.bitacora.nuestrascuentitas.com`) está activa con DNS propagado desde hoy. El frontend fue reconstruido con la URL correcta. El JWT secret del backend fue sincronizado. **El bloqueo crítico del E2E anterior quedó resuelto.**

Este E2E debe verificar **todos los flujos y journeys del producto** sobre producción real:

- ONB: login programático con usuario/contraseña (sin magic link, sin SMTP)
- CON: consentimiento completo
- REG: registro de humor web + DailyCheckin web
- TG: pairing + flujo Telegram completo (mi-telegram-cli)
- VIN: vinculos profesional-paciente (binding code)
- VIS: timeline + summary (paciente y profesional)
- EXP: export CSV
- SEC: gates fail-closed y rate limiting

**Entregable obligatorio:** carpeta `artifacts/e2e/2026-04-15-e2e-full/` con evidencia de cada fase (outputs JSON, queries DB, screenshots donde aplique). Cada fase cierra con verificación en DB real antes de pasar a la siguiente.

---

## Credenciales y entorno (no reabrir)

| Variable | Valor |
|----------|-------|
| `SMOKE_TEST_EMAIL` | `smoke@bitacora.test` |
| `SMOKE_TEST_PASSWORD` | `SmokeTest2026!` |
| `SMOKE_TEST_USER_ID` (Bitácora local) | Verificar con `POST /api/v1/auth/bootstrap` — retorna `userId` |
| GoTrue endpoint | `https://auth.bitacora.nuestrascuentitas.com` |
| API endpoint | `https://api.bitacora.nuestrascuentitas.com` |
| Frontend | `https://bitacora.nuestrascuentitas.com` |
| VPS SSH alias | `turismo` (nunca `bitacora`) |

**Cómo obtener el JWT de smoke:**

```bash
curl -s -X POST \
  "https://auth.bitacora.nuestrascuentitas.com/token?grant_type=password" \
  -H "Content-Type: application/json" \
  -d '{"email":"smoke@bitacora.test","password":"SmokeTest2026!"}'
# → {"access_token":"eyJ...","token_type":"bearer","expires_in":3600,...}
```

Extraer `access_token` y usarlo como `Bearer <JWT>` en todos los endpoints API.

---

## Estado heredado — No reabrir

| Ítem | Estado |
|------|--------|
| E2E Telegram (manual) 2026-04-14 | PASSED — MoodEntry `d5114f8b`, DailyCheckin `177d23b1` en DB |
| E2E Web API (forjado) 2026-04-14 | PASSED — MoodEntry `97ac6459`, DailyCheckin `a3d87c3a` en DB |
| Auth misconfiguration | RESUELTO 2026-04-15 — nueva GoTrue dedicada, DNS propagado, frontend reconstruido |
| TP-TG | CERRADO: TG-P01, TG-N01, TG-P02, TG-N02 PASSED |
| Smoke user `smoke@bitacora.test` | CREADO en GoTrue con `email_confirm=true` (sin SMTP) |
| SMOKE_TEST_* secrets | Guardados en Infisical teslita (prod) y en `infra/.env` |
| `infra/secrets.enc.env` | Actualizado y commiteado (commit `0c94d3b`) |

**Decisiones bloqueadas — no negociar:**
- Las URLs de producción son inmutables.
- El alias SSH es `turismo`, no `bitacora`.
- La evidencia va siempre a `artifacts/e2e/YYYY-MM-DD-<slug>/` — nunca en la raíz ni en `tmp/`.
- `db-cli` verifica en tiempo real con DB de producción — nunca asumir estado.
- El JWT de smoke se obtiene **siempre** via `POST /token?grant_type=password` — nunca forjado manualmente.
- No saltar verificación de DB entre fases — es el único oracle confiable.

---

## Paso 0 — Verificar estado del repo antes de ejecutar

**Antes de cualquier acción, verifica:**

1. `Skill("ps-contexto")` — carga contexto del proyecto.
2. Lee `infra/.env` y confirma que `SMOKE_TEST_EMAIL`, `SMOKE_TEST_PASSWORD` y `SUPABASE_JWT_SECRET` están presentes.
3. Verifica GoTrue health: `GET https://auth.bitacora.nuestrascuentitas.com/health` → debe retornar `{"version":"v2.177.0","name":"GoTrue"}`.
4. Verifica API health: `GET https://api.bitacora.nuestrascuentitas.com/health` → 200.
5. Verifica API readiness: `GET https://api.bitacora.nuestrascuentitas.com/health/ready` → 200 con todos los componentes OK.
6. Verifica frontend: `GET https://bitacora.nuestrascuentitas.com` → 200 con `OnboardingEntryHero` visible.
7. Verifica bundle JS baked: el bundle debe contener `auth.bitacora.nuestrascuentitas.com` (no `auth.tedi.nuestrascuentitas.com`). Scan desde VPS o directamente en los chunks JS del frontend.
8. Lanza **3 `ps-explorer` subagents en paralelo** para mapear el estado actual del repo:
   - Explorer 1: `artifacts/e2e/` — listar evidencia existente y fechas
   - Explorer 2: `.docs/wiki/06_pruebas/` — leer TP-REG, TP-CON, TP-VIN, TP-VIS, TP-EXP, TP-TG y sus estados actuales
   - Explorer 3: `infra/smoke/backend-smoke.ps1` — entender qué cubre el smoke script y qué gates están definidos
   - **Explorer 4** (requerido): `.docs/wiki/09_contratos/CT-AUTH.md` y `.docs/wiki/07_baseline_tecnica.md` — confirmar invariantes de JWT, middleware pipeline y gates de smoke vigentes
   - **Explorer 5** (requerido para VIN/VIS): `.docs/wiki/04_RF/RF-VIN-*.md` y `.docs/wiki/04_RF/RF-VIS-*.md` — entender los flujos de vinculos y visualización que hay que probar

Guarda la salida de los explorers en `artifacts/e2e/2026-04-15-e2e-full/00-setup-verificacion.md` antes de continuar.

---

## Fase 1 — Auth + Onboarding (ONB)

**Objetivo:** verificar que el flujo de login programático via GoTrue dedicado funciona end-to-end.

### F1-1: Login programático

```bash
curl -s -X POST \
  "https://auth.bitacora.nuestrascuentitas.com/token?grant_type=password" \
  -H "Content-Type: application/json" \
  -d '{"email":"smoke@bitacora.test","password":"SmokeTest2026!"}'
```

**Criterio PASS:** `access_token` presente, `token_type=bearer`, sin error.
**Criterio FAIL:** `{"error":"invalid_grant",...}` o cualquier 4xx/5xx.

Guarda el JWT en variable de sesión. Guarda la respuesta completa en `artifacts/e2e/2026-04-15-e2e-full/F1-01-gotrue-login.json`.

### F1-2: Bootstrap en API

```bash
curl -s -X POST \
  "https://api.bitacora.nuestrascuentitas.com/api/v1/auth/bootstrap" \
  -H "Authorization: Bearer <JWT>"
```

**Criterio PASS:** `{"userId":"<uuid>","status":"registered"|"active","needsConsent":true|false}` — HTTP 200.
**Criterio FAIL:** HTTP 401 (JWT inválido — significa que el secret del backend sigue siendo incorrecto o hay drift), 500, o cualquier respuesta de error.

Guarda respuesta en `F1-02-bootstrap.json`. Extrae y guarda el `userId` local (será necesario en fases posteriores).

### F1-3: Verificación DB (ONB)

```sql
SELECT user_id, supabase_user_id, status, email_hash
FROM users
WHERE supabase_user_id = (SELECT sub FROM <JWT decoded>);
```

Via db-cli alias `bitacora-prod`. Confirmar que el user existe y que `supabase_user_id` coincide con el `sub` del JWT.

**Evidencia:** `F1-03-db-user.txt`

---

## Fase 2 — Consentimiento (CON)

**Objetivo:** verificar el flujo completo de consentimiento.

### F2-1: GET consent/current

```bash
curl -s "https://api.bitacora.nuestrascuentitas.com/api/v1/consent/current" \
  -H "Authorization: Bearer <JWT>"
```

**Criterio PASS:** 200 con `consentText`, `version`, `patient_status`. Guarda en `F2-01-consent-current.json`.

### F2-2: POST consent (si needsConsent=true del bootstrap)

```bash
curl -s -X POST \
  "https://api.bitacora.nuestrascuentitas.com/api/v1/consent" \
  -H "Authorization: Bearer <JWT>" \
  -H "Content-Type: application/json" \
  -d '{"version":"<version-del-F2-01>","accepted":true}'
```

**Criterio PASS:** HTTP 201 con `ConsentGrant` activo, o HTTP 409 si ya existía un grant vigente.
**Criterio FAIL:** HTTP 400, 422, o cualquier error.

Guarda en `F2-02-consent-grant.json`.

### F2-3: Verificar gate CON-003 (sin consent → 403)

Con un JWT **diferente** (o sin consent revocado), verificar que `POST /api/v1/mood-entries` retorna 403 + `CONSENT_REQUIRED`.

> Si el smoke user ya tiene consent activo, crear temporalmente un segundo JWT sin consent para este gate, o usar el hallazgo de la matriz de smoke (GATE-SMOKE-003).

### F2-4: Verificación DB (CON)

```sql
SELECT consent_grant_id, status, version, granted_at
FROM consent_grants
WHERE user_id = '<userId>';
```

**Evidencia:** `F2-04-db-consent.txt`

---

## Fase 3 — Registro web (REG)

**Objetivo:** verificar MoodEntry y DailyCheckin via API con JWT real de GoTrue.

### F3-1: POST mood-entry (REG-P01)

```bash
curl -s -X POST \
  "https://api.bitacora.nuestrascuentitas.com/api/v1/mood-entries" \
  -H "Authorization: Bearer <JWT>" \
  -H "Content-Type: application/json" \
  -d '{"score": 1}'
```

**Criterio PASS:** HTTP 201 con `moodEntryId`, `score=1`, `channel=api`.
**Criterio FAIL:** HTTP 401 (drift de JWT secret), 403 (sin consent), 500.

Guarda en `F3-01-mood-entry.json`.

### F3-2: Idempotencia (REG-005)

Repetir el mismo `POST /api/v1/mood-entries` con `score=1`.

**Criterio PASS:** HTTP 200 (reutiliza el existente dentro de la ventana de idempotencia) o 201 si la ventana expiró.

### F3-3: POST daily-checkin (REG-P03)

```bash
curl -s -X POST \
  "https://api.bitacora.nuestrascuentitas.com/api/v1/daily-checkins" \
  -H "Authorization: Bearer <JWT>" \
  -H "Content-Type: application/json" \
  -d '{
    "sleepHours": 7,
    "physicalActivity": true,
    "socialActivity": false,
    "anxiety": false,
    "irritability": false,
    "medicationTaken": true,
    "medicationTime": "08:00"
  }'
```

**Criterio PASS:** HTTP 201 o 200, `DailyCheckin` creado/actualizado.

### F3-4: Score inválido (REG-N01)

```bash
curl -s -X POST ".../api/v1/mood-entries" \
  -H "Authorization: Bearer <JWT>" \
  -H "Content-Type: application/json" \
  -d '{"score": 99}'
```

**Criterio PASS:** HTTP 400 o 422.

### F3-5: Verificación DB (REG)

```sql
SELECT mood_entry_id, score, channel, created_at
FROM mood_entries
WHERE patient_id = (SELECT patient_id FROM patients WHERE user_id = '<userId>')
ORDER BY created_at DESC LIMIT 3;

SELECT daily_checkin_id, sleep_hours, medication_taken, created_at
FROM daily_checkins
WHERE patient_id = ...
ORDER BY created_at DESC LIMIT 1;
```

**Verificar:** `encrypted_payload` no es null, `safe_projection` tiene `mood_score` pero NO `encrypted_payload` raw expuesto en la respuesta API.

**Evidencia:** `F3-05-db-reg.txt`

---

## Fase 4 — Telegram (TG)

**Objetivo:** verificar pairing + flujo conversacional completo con mi-telegram-cli.

### F4-1: Generar pairing code

```bash
curl -s -X POST \
  "https://api.bitacora.nuestrascuentitas.com/api/v1/telegram/pairing" \
  -H "Authorization: Bearer <JWT>"
```

**Criterio PASS:** HTTP 200 con `pairingCode: "BIT-XXXXX"`, `expiresAt`.
**Criterio FAIL:** HTTP 403 sin consent, 401 JWT inválido.

Guarda en `F4-01-pairing-code.json`. Extrae el código `BIT-XXXXX`.

### F4-2: Vincular chat vía /start

```bash
# Usar mi-telegram-cli con cuenta QA (@tedi_responde)
mi-telegram-cli send @mi_bitacora_personal_bot "/start BIT-XXXXX"
```

**Criterio PASS:** el bot responde confirmando la vinculación.

Verificar DB:
```sql
SELECT session_id, chat_id, is_linked, linked_at
FROM telegram_sessions
WHERE patient_id = ...;
```

### F4-3: Flujo conversacional completo (REG-010..015)

Ejecutar la secuencia completa usando mi-telegram-cli:

1. `send @mi_bitacora_personal_bot "+1"` → bot debe registrar MoodEntry (channel=telegram)
2. `press-button "<msg_id>" "7h"` → sueño 7h
3. `press-button "<msg_id>" "Sí"` → actividad física
4. `press-button "<msg_id>" "No"` → actividad social
5. `press-button "<msg_id>" "No"` → ansiedad
6. `press-button "<msg_id>" "No"` → irritabilidad
7. `press-button "<msg_id>" "Sí"` → medicación
8. `send @mi_bitacora_personal_bot "09:30"` → hora medicación

**Criterio PASS:** bot responde con mensaje de confirmación tipo "Check-in actualizado! Humor: +1 | Sueño: 7h | ..."

### F4-4: Invariante de no-fuga

Verificar que **ningún mensaje del bot** contiene `encrypted_payload`, datos clínicos raw, o el `patient_id`. Solo confirmación de receipt.

### F4-5: Verificación DB (TG)

```sql
SELECT mood_entry_id, score, channel, created_at
FROM mood_entries
WHERE channel = 'telegram' AND patient_id = ...
ORDER BY created_at DESC LIMIT 1;

SELECT daily_checkin_id, sleep_hours, medication_taken, medication_time, updated_at
FROM daily_checkins
WHERE patient_id = ...
ORDER BY updated_at DESC LIMIT 1;

SELECT conversation_state, updated_at
FROM telegram_sessions
WHERE patient_id = ...;
```

**Criterio PASS:** MoodEntry con `channel=telegram`, DailyCheckin actualizado, `conversation_state=0` (Idle).

**Evidencia:** `F4-05-db-telegram.txt`, `F4-03-conversation-log.json`

### F4-6: Chat no vinculado → guidance (REG-014)

Con la **cuenta QA** (mi-telegram-cli, chat_id diferente sin vincular), enviar `"+2"` al bot.

**Criterio PASS:** bot responde con instrucciones de vinculación web, sin crear MoodEntry.

---

## Fase 5 — Vínculos profesional-paciente (VIN)

**Objetivo:** verificar el flujo de binding code y acceso profesional.

> Si no existe un usuario profesional en producción, documentar como GAP y ejecutar solo los endpoints de paciente.

### F5-1: GET /api/v1/vinculos (VIN-001)

```bash
curl -s "https://api.bitacora.nuestrascuentitas.com/api/v1/vinculos" \
  -H "Authorization: Bearer <JWT>"
```

**Criterio PASS:** HTTP 200 con lista (puede ser vacía).

### F5-2: GET /api/v1/vinculos/active

```bash
curl -s "https://api.bitacora.nuestrascuentitas.com/api/v1/vinculos/active" \
  -H "Authorization: Bearer <JWT>"
```

**Criterio PASS:** HTTP 200.

### F5-3: Binding code flow (VIN-010..012)

Si hay un profesional disponible en producción:

1. `POST /api/v1/professional/invites` con JWT profesional → `pendingInviteId`
2. `GET /api/v1/professional/patients` → lista de pacientes

Si no hay profesional disponible, documentar el gap y marcar como pendiente de test manual.

**Evidencia:** `F5-03-vinculos.json`

---

## Fase 6 — Visualización y Export (VIS + EXP)

**Objetivo:** verificar que los datos registrados en fases anteriores son legibles via timeline/summary/export.

### F6-1: Timeline paciente (VIS-001)

```bash
curl -s "https://api.bitacora.nuestrascuentitas.com/api/v1/visualizacion/timeline?from=2026-04-15&to=2026-04-15" \
  -H "Authorization: Bearer <JWT>"
```

**Criterio PASS:** HTTP 200 con al menos 1 MoodEntry del día (los creados en F3-1 y F4-3).

### F6-2: DailyCheckin timeline (VIS-002)

```bash
curl -s "https://api.bitacora.nuestrascuentitas.com/api/v1/visualizacion/timeline?from=2026-04-15&to=2026-04-15&type=daily-checkin" \
  -H "Authorization: Bearer <JWT>"
```

**Criterio PASS:** HTTP 200 con el DailyCheckin del día.

### F6-3: Summary (VIS-003)

```bash
curl -s "https://api.bitacora.nuestrascuentitas.com/api/v1/visualizacion/summary?from=2026-04-09&to=2026-04-15" \
  -H "Authorization: Bearer <JWT>"
```

**Criterio PASS:** HTTP 200.

### F6-4: Export CSV (EXP-001)

```bash
curl -s "https://api.bitacora.nuestrascuentitas.com/api/v1/export/patient-summary/csv?from=2026-04-01&to=2026-04-15" \
  -H "Authorization: Bearer <JWT>" \
  -o "artifacts/e2e/2026-04-15-e2e-full/F6-04-export.csv"
```

**Criterio PASS:** archivo CSV descargado, headers presentes, al menos 1 fila de datos.

### F6-5: Export constraints (EXP)

```bash
curl -s "https://api.bitacora.nuestrascuentitas.com/api/v1/export/<userId>/constraints" \
  -H "Authorization: Bearer <JWT>"
```

**Criterio PASS:** HTTP 200 con `{"allowed":true|false}` según rol.

**Evidencia:** `F6-01-timeline.json`, `F6-03-summary.json`, `F6-04-export.csv`

---

## Fase 7 — Gates fail-closed y rate limiting (SEC)

**Objetivo:** verificar que los mecanismos de seguridad funcionan en producción.

### F7-1: Health checks (GATE-SMOKE-001, 002)

```bash
curl -s "https://api.bitacora.nuestrascuentitas.com/health"
curl -s "https://api.bitacora.nuestrascuentitas.com/health/ready"
```

**Criterio PASS:** ambos retornan 200. `health/ready` debe indicar todos los subsistemas OK (DB, JWT secret, encryption key, pseudonym salt).

### F7-2: Sin consent → 403 (GATE-SMOKE-003)

Intentar `POST /api/v1/mood-entries` con un JWT válido pero sin `ConsentGrant` activo.

**Criterio PASS:** HTTP 403 con code `CONSENT_REQUIRED` en la respuesta.

### F7-3: JWT inválido → 401

```bash
curl -s -X POST \
  "https://api.bitacora.nuestrascuentitas.com/api/v1/auth/bootstrap" \
  -H "Authorization: Bearer eyJfake.token.here"
```

**Criterio PASS:** HTTP 401.

### F7-4: Rate limiting (GATE-RL-001)

Enviar 11 requests a `POST /api/v1/auth/bootstrap` en < 1 minuto desde la misma IP.

**Criterio PASS:** el 11° request retorna HTTP 429 con header `Retry-After`.

> Usar `for i in $(seq 1 11); do curl -s -o /dev/null -w "%{http_code}\n" -X POST https://api.bitacora.nuestrascuentitas.com/api/v1/auth/bootstrap -H "Authorization: Bearer <JWT>"; done`

### F7-5: GoTrue bundle check

Verificar que el bundle JS del frontend en producción contiene `auth.bitacora.nuestrascuentitas.com` y **NO** contiene `auth.tedi.nuestrascuentitas.com`.

```bash
# Desde VPS via sshr
sshr exec --host turismo --cmd "docker exec <frontend-container> grep -r 'auth.bitacora' /app/.next/static/ | head -3"
sshr exec --host turismo --cmd "docker exec <frontend-container> grep -r 'auth.tedi' /app/.next/static/ | head -3"
```

**Criterio PASS:** `auth.bitacora` encontrado, `auth.tedi` NO encontrado.

**Evidencia:** `F7-05-bundle-check.txt`

---

## Fase 8 — Resumen y cierre

### F8-1: Tabla de resultados

Al finalizar todas las fases, generar `artifacts/e2e/2026-04-15-e2e-full/evidencia-resumen.md` con:

| Fase | Escenario | TC ID | Estado | HTTP | Notas |
|------|-----------|-------|--------|------|-------|
| F1 | Login programático GoTrue | ONB-P01 | PASS/FAIL | ... | ... |
| F1 | Bootstrap API con JWT real | ONB-P02 | PASS/FAIL | ... | ... |
| F2 | GET consent/current | CON-P01 | PASS/FAIL | ... | ... |
| F2 | POST consent | CON-P02 | PASS/FAIL | ... | ... |
| F2 | Sin consent → 403 | CON-N01 | PASS/FAIL | ... | ... |
| F3 | MoodEntry web (score=1) | REG-P01 | PASS/FAIL | ... | ... |
| F3 | Idempotencia MoodEntry | REG-P01b | PASS/FAIL | ... | ... |
| F3 | DailyCheckin web | REG-P03 | PASS/FAIL | ... | ... |
| F3 | Score inválido → 400 | REG-N01 | PASS/FAIL | ... | ... |
| F4 | Pairing code | TG-P01 | PASS/FAIL | ... | ... |
| F4 | /start vinculación | TG-P01b | PASS/FAIL | ... | ... |
| F4 | Flujo conversacional completo | TG-P02 | PASS/FAIL | ... | ... |
| F4 | No-fuga datos clínicos | TG-SEC | PASS/FAIL | ... | ... |
| F4 | Chat no vinculado → guidance | TG-N02 | PASS/FAIL | ... | ... |
| F5 | GET vinculos | VIN-P01 | PASS/FAIL | ... | ... |
| F6 | Timeline paciente | VIS-P01 | PASS/FAIL | ... | ... |
| F6 | Export CSV | EXP-P01 | PASS/FAIL | ... | ... |
| F7 | Health + readiness | GATE-001/002 | PASS/FAIL | ... | ... |
| F7 | Rate limiting 429 | GATE-RL-001 | PASS/FAIL | ... | ... |
| F7 | Bundle check (no auth.tedi) | AUTH-BUNDLE | PASS/FAIL | ... | ... |

### F8-2: Gaps detectados

Documentar cualquier comportamiento inesperado, endpoint que retorna distinto de lo esperado, o funcionalidad que requiere seguimiento.

### F8-3: Actualización de TP docs

Por cada TC marcado PASSED, actualizar el TP correspondiente (`.docs/wiki/06_pruebas/TP-*.md`) con:
- Estado: PASSED
- Fecha: 2026-04-15
- Ambiente: produccion
- Evidencia: `artifacts/e2e/2026-04-15-e2e-full/<archivo>`

### F8-4: Actualizar matriz (06_matriz_pruebas_RF.md)

Agregar la ejecución de 2026-04-15 a la sección "Cobertura E2E produccion ejecutada" de `.docs/wiki/06_matriz_pruebas_RF.md`.

Nota: el GAP crítico de auth de 2026-04-14 debe marcarse como **RESUELTO** en esa misma tabla.

---

## Cierre del ciclo

Ejecutar `Skill("ps-trazabilidad")` al finalizar para verificar:

1. TP-ONB, TP-CON, TP-REG, TP-TG, TP-VIN, TP-VIS, TP-EXP actualizados con evidencia.
2. `06_matriz_pruebas_RF.md` con la ejecución 2026-04-15 registrada.
3. `CT-AUTH.md` refleja que el invariante de JWT secret está cumplido y verificado.
4. `07_baseline_tecnica.md` no tiene pendientes relacionados con el fix de auth.
5. Sin artefactos huérfanos en `tmp/`.

---

## Herramientas disponibles

| Herramienta | Uso |
|-------------|-----|
| `sshr exec --host turismo --cmd "..."` | Comandos en VPS |
| `db-cli` alias `bitacora-prod` | Queries SQL en DB de producción |
| `mi-telegram-cli send/press-button` | Automatización del bot Telegram |
| `Skill("playwright-cli")` o MCP Playwright | Screenshots del frontend si se necesita evidencia visual |
| `curl` | Todos los requests HTTP |
| `$DKP` (dokploy-cli) | Estado de contenedores si hay regresión de infra |

---

## Reglas de ejecución

1. **Verificar en DB antes de pasar de fase.** No asumir que un HTTP 201 implica persistencia.
2. **Si una fase falla, documentar el hallazgo y continuar** con las fases restantes. No abortar. El resumen final es el entregable.
3. **Si el JWT del smoke expira**, renovarlo con `POST /token?grant_type=password` y continuar.
4. **No forjar JWTs** — este E2E prueba el flujo real con GoTrue, no el bypass anterior.
5. **Toda evidencia en `artifacts/e2e/2026-04-15-e2e-full/`** — nombrar archivos con prefijo de fase (F1-, F2-, etc.).
6. **Si se detecta `auth.tedi.nuestrascuentitas.com` en el bundle**: es un hallazgo crítico — documentarlo como bloqueo y notificar antes de continuar.
