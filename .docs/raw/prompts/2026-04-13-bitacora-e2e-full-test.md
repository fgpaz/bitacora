# Bitacora Wave-Prod E2E Full Test — Execution Report

```yaml
# platform: Codex
# pressure: aggressive
# date: 2026-04-13
# task: bitacora-wave-prod E2E full test
# parent_plan: .docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp.md
# skills: $ps-contexto, $mi-lsp, $ps-explorer, $ps-trazabilidad, $ps-auditar-trazabilidad, $ps-qa-orchestrator
# status: PARTIAL — auth blocked, unauthenticated tests PASS
# blocker: SUPABASE_JWT_SECRET unavailable from vault
```

## Execution Summary

| Step | Status | Detail |
|------|--------|--------|
| 1. ps-contexto | PASS | Loaded 01_alcance, 02_arquitectura, 03_FL, 04_RF, 05_modelo_datos, 07_baseline_tecnica |
| 2. Exploration block (5 subagents) | PASS | Endpoints, consent gate, telegram flow, frontend auth, DB schema all mapped |
| 3. Auth setup | BLOCKED | SUPABASE_JWT_SECRET not accessible from vault, Infisical, or SSH |
| 4. DB pre-check | BLOCKED | No psql/db-cli available; connection string not accessible |
| 5. Patient E2E flow | BLOCKED | Requires JWT; unauthenticated tests PASS |
| 6. Telegram pairing flow | BLOCKED | Requires JWT + bot token |
| 7. DB post-check | BLOCKED | Requires DB connection |
| 8. ps-trazabilidad | READY | All FL->RF chains mapped (see below) |
| 9. ps-auditar-trazabilidad | PENDING | Can proceed on documentation side |

## Unauthenticated Verification Results

| Endpoint | Method | Status | Response |
|----------|--------|--------|----------|
| `GET /health` | GET | 200 PASS | `{"status":"ok"}` |
| `GET /health/ready` | GET | 200 PASS | `{"status":"ready","checks":{"connection_string":"ok","supabase_jwt_secret":"ok","encryption_key":"ok","pseudonym_salt":"ok","database":"ok"}}` |

Both liveness and readiness endpoints PASS with all subsystem checks green.

## Blocker Analysis

### Root Cause
`SUPABASE_JWT_SECRET` is not available through any of these channels:
1. `infra/.env` — file exists but is empty (0 bytes)
2. Infisical — Bitacora project not configured in the local instance
3. Dokploy API — no `DOKPLOY_API_KEY` available to query application env vars
4. SSH to VPS (54.37.157.93) — no SSH key configured for this host

### Required Secrets

| Secret | Source | Status |
|--------|--------|--------|
| `SUPABASE_JWT_SECRET` | Dokploy env / Infisical | BLOCKED |
| `ConnectionStrings__BitacoraDb` | Dokploy env | BLOCKED |
| `BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN` | Dokploy env | BLOCKED (optional for webhook) |

### Mitigation Options
1. **Manual**: Export secrets from Dokploy UI (54.37.157.93:3000) into `infra/.env`
2. **Dokploy API**: Obtain DOKPLOY_API_KEY from Dokploy UI and add to `infra/.env`, then use `dkp.sh env <APP_ID>`
3. **SSH**: Add SSH key for root@54.37.157.93, then `docker exec <container> printenv`
4. **Post-smoke**: Run `infra/smoke/backend-smoke.ps1` from a Windows machine with the `.env` populated

## Exploration Results

### 1. Backend Endpoints Inventory (24 endpoints)

| # | Method | Route | Policy | Auth | Consent Gate |
|---|--------|-------|--------|------|-------------|
| 1 | POST | /api/v1/auth/bootstrap | auth | JWT | No |
| 2 | GET | /api/v1/consent/current | write | JWT | No |
| 3 | POST | /api/v1/consent | write | JWT | No |
| 4 | DELETE | /api/v1/consent/current | write | JWT | No |
| 5 | POST | /api/v1/mood-entries | write | JWT | **Yes** |
| 6 | POST | /api/v1/daily-checkins | write | JWT | **Yes** |
| 7 | POST | /api/v1/telegram/pairing | write | JWT | No |
| 8 | GET | /api/v1/telegram/session | auth | JWT | No |
| 9 | POST | /api/v1/telegram/webhook | webhook | X-Telegram-Webhook-Secret | No |
| 10 | GET | /api/v1/vinculos | auth | JWT | No |
| 11 | GET | /api/v1/vinculos/active | auth | JWT | No |
| 12 | POST | /api/v1/vinculos/accept | write | JWT | No |
| 13 | DELETE | /api/v1/vinculos/{id} | write | JWT | No |
| 14 | PATCH | /api/v1/vinculos/{id}/view-data | write | JWT | No |
| 15 | POST | /api/v1/professional/invites | write | JWT (professional) | No |
| 16 | GET | /api/v1/professional/patients | auth | JWT (professional) | No |
| 17 | GET | /api/v1/visualizacion/timeline | auth | JWT | No |
| 18 | GET | /api/v1/visualizacion/summary | auth | JWT | No |
| 19 | GET | /api/v1/professional/patients/{id}/summary | auth+ProAuth | JWT (professional) | No |
| 20 | GET | /api/v1/professional/patients/{id}/timeline | auth+ProAuth | JWT (professional) | No |
| 21 | GET | /api/v1/professional/patients/{id}/alerts | auth+ProAuth | JWT (professional) | No |
| 22 | GET | /api/v1/export/patient-summary | auth | JWT | No |
| 23 | GET | /api/v1/export/{id}/constraints | auth | JWT | No |
| 24 | GET | /api/v1/export/patient-summary/csv | auth | JWT | No |

### 2. Consent Gate Implementation

**File:** `src/Bitacora.Api/Middleware/ConsentRequiredMiddleware.cs`

- Blocks `POST` to `/api/v1/mood-entries` and `/api/v1/daily-checkins`
- Heuristic: any other `/api/v1/<segment>` POST also blocked unless allowlisted (`/auth`, `/telegram`, `/export`)
- Check: `ConsentGrant.Status == Granted` for current patient
- Returns: **403** with error code `CONSENT_REQUIRED`
- **CRITICAL TEST**: Attempt mood entry WITHOUT consent -> must return 403 CONSENT_REQUIRED

### 3. Telegram Pairing Flow

```
POST /api/v1/telegram/pairing (JWT auth)
  -> GeneratePairingCodeCommandHandler
  -> Invalidate previous codes for patient
  -> Generate BIT-XXXXX (5 chars, collision-safe, TTL 900s)
  -> Return { code, expiresInSeconds: 900, expiresAt }

POST /api/v1/telegram/webhook (X-Telegram-Webhook-Secret)
  -> HandleWebhookUpdateCommandHandler
  -> Parse /start BIT-XXXXX
  -> ConfirmPairingCommandHandler
  -> Validate code (not used, not expired)
  -> Check chat_id uniqueness
  -> Mark code as used, create TelegramSession (Linked)
  -> Return 200 always (Telegram delivery stops on 2xx)
```

### 4. Frontend Auth

- `frontend/middleware.ts` reads `sb-access-token` cookie
- JWT decoded client-side for expiry and role checks
- API calls use `credentials: 'include'` (cookie-based)
- Frontend NOT deployed at bitacora.nuestrascuentitas.com (empty response)
- API GET / redirects to `/scalar/v1` (Scalar docs)
- **Test strategy: API-only with Bearer token, not Playwright**

### 5. DB Schema Verified

All tables mapped and verified against migrations:
- `users` (9 columns, UNIQUE on supabase_user_id)
- `mood_entries` (7 columns, Global Query Filter on patient_id, partition index on patient_id+created_at_utc)
- `daily_checkins` (9 columns, Global Query Filter, UNIQUE on patient_id+checkin_date)
- `consent_grants` (7 columns, status enum: Pending/Granted/Revoked)
- `telegram_pairing_codes` (7 columns, UNIQUE on code, TTL 15 min)
- `telegram_sessions` (11 columns, UNIQUE on chat_id)
- `access_audits` (10 columns, append-only)

### 6. JWT Construction Pattern (from backend-smoke.ps1)

```
Header:  {"alg":"HS256","typ":"JWT"}
Payload: {"sub":"<guid>","email":"smoke+<guid>@bitacora.nuestrascuentitas.com","iat":<now>,"exp":<now+15min>}
Key:     SUPABASE_JWT_SECRET encoded as UTF-8 bytes
Sign:    HMAC-SHA256
```

Note: JWT uses `sub` claim (mapped to `User.SupabaseUserId`), NOT `role` claim. The `role` field on the `User` entity is determined at bootstrap time, not from the JWT.

## E2E Test Plan (Requires Unblocking)

### Prerequisites
```bash
export SUPABASE_JWT_SECRET="<from-vault>"
export BITACORA_BASE_URL="https://api.bitacora.nuestrascuentitas.com"
# Optional: export ConnectionStrings__BitacoraDb="Host=...;..."
# Optional: export BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN="<from-vault>"
```

### Test Sequence (11 API calls)

| Step | Method | Path | Expected | FL/RF | Security Check |
|------|--------|------|----------|-------|---------------|
| 1 | GET | /health | 200 | - | Liveness |
| 2 | GET | /health/ready | 200 | - | All checks ok |
| 3 | POST | /api/v1/auth/bootstrap | 200 | FL-ONB-01 -> RF-ONB-001,002 | Auth |
| 4 | GET | /api/v1/consent/current | 200 | FL-CON-01 -> RF-CON-001 | Returns version |
| 5 | POST | /api/v1/mood-entries (no consent) | 403 CONSENT_REQUIRED | FL-CON-01 -> RF-CON-003 | **CRITICAL** |
| 6 | POST | /api/v1/consent | 201 | FL-CON-01 -> RF-CON-002 | Consent grant |
| 7 | POST | /api/v1/mood-entries | 200/201 | FL-REG-01 -> RF-REG-001..005 | With consent |
| 8 | POST | /api/v1/daily-checkins | 200/201 | FL-REG-03 -> RF-REG-020..025 | Upsert |
| 9 | GET | /api/v1/visualizacion/timeline | 200 | FL-VIS-01 -> RF-VIS-001..003 | Date range |
| 10 | GET | /api/v1/vinculos | 200 | FL-VIN-02 -> RF-VIN-010 | Empty array |
| 11 | POST | /api/v1/telegram/pairing | 200 | FL-TG-01 -> RF-TG-001 | BIT-XXXXX code |

### Consent Gate Security Test (Critical)

1. **Test 5**: POST mood-entries WITHOUT prior consent grant MUST return 403 with error code `CONSENT_REQUIRED`
2. **Test 7**: POST mood-entries WITH consent grant MUST return 200/201
3. If test 7 succeeds WITHOUT consent grant: **CRITICAL SECURITY DEFECT**

## Traceability: FL -> RF -> TP Chain Verification

### FL-ONB-01: Onboarding completo del paciente
| RF | Title | Status | Verified |
|----|-------|--------|----------|
| RF-ONB-001 | Crear User desde JWT de Supabase (bootstrap) | Implementado backend | POST /api/v1/auth/bootstrap |
| RF-ONB-002 | Detectar usuario nuevo vs existente | Implementado backend | Bootstrap idempotent |
| RF-ONB-003 | Forzar pantalla de consent | Implementado backend | GET /api/v1/consent/current |

### FL-REG-01: Registro de humor via web
| RF | Title | Status | Verified |
|----|-------|--------|----------|
| RF-REG-001 | Crear MoodEntry con score validado | Implementado backend | POST /api/v1/mood-entries |
| RF-REG-002 | Validar score en rango -3..+3 | Implementado backend | Implicit in endpoint |
| RF-REG-003 | Cifrar payload y generar safe_projection | Implementado backend | Server-side |
| RF-REG-004 | Verificar consent activo antes de registro | Implementado backend | ConsentRequiredMiddleware |
| RF-REG-005 | Idempotencia por patient_id + timestamp | Implementado backend | 200 on duplicate |

### FL-REG-03: Registro de factores diarios
| RF | Title | Status | Verified |
|----|-------|--------|----------|
| RF-REG-020 | Crear DailyCheckin con campos validados | Implementado backend | POST /api/v1/daily-checkins |
| RF-REG-021 | Validar rangos de factores diarios | Implementado backend | Implicit in endpoint |
| RF-REG-022 | UPSERT DailyCheckin (uno por dia) | Implementado backend | 200 update, 201 create |
| RF-REG-023 | Cifrar payload y generar safe_projection | Implementado backend | Server-side |
| RF-REG-024 | Registrar audit de creacion/actualizacion | Implementado backend | AccessAudit entry |
| RF-REG-025 | Incluir medicacion con horario aproximado | Implementado backend | medicationTime field |

### FL-CON-01: Otorgamiento de consentimiento informado
| RF | Title | Status | Verified |
|----|-------|--------|----------|
| RF-CON-001 | Presentar texto de consentimiento vigente | Implementado backend | GET /api/v1/consent/current |
| RF-CON-002 | Registrar aceptacion con version y timestamp | Implementado backend | POST /api/v1/consent |
| RF-CON-003 | Hard gate: bloquear registro sin consent granted | Implementado backend | 403 CONSENT_REQUIRED |

### FL-VIS-01: Timeline longitudinal
| RF | Title | Status | Verified |
|----|-------|--------|----------|
| RF-VIS-001 | Query mood_entries por rango de fechas | Implementado frontend* | GET /api/v1/visualizacion/timeline |
| RF-VIS-002 | Query daily_checkins por rango de fechas | Implementado frontend* | GET /api/v1/visualizacion/summary |
| RF-VIS-003 | Paginacion para periodos largos | Implementado frontend* | Query params |

*Frontend not deployed; API endpoints verified in code

### FL-VIN-02: Auto-vinculacion paciente-profesional
| RF | Title | Status | Verified |
|----|-------|--------|----------|
| RF-VIN-010 | Generar BindingCode para auto-vinculacion | Parcial backend | POST /api/v1/vinculos/accept |
| RF-VIN-011 | Validar BindingCode y resolver professional_id | Implementado backend | Code validation |
| RF-VIN-012 | Crear CareLink por auto-vinculacion | Implementado backend | CareLink creation |

### FL-TG-01: Vinculacion de cuenta Telegram
| RF | Title | Status | Verified |
|----|-------|--------|----------|
| RF-TG-001 | Generar pairing code con TTL | Implementado backend | POST /api/v1/telegram/pairing |
| RF-TG-002 | Vincular chat_id via /start + code | Implementado backend | Webhook /start BIT-XXXXX |
| RF-TG-003 | Validar unicidad de chat_id | Implementado backend | UNIQUE constraint on chat_id |

## E2E Test Script (Python)

Requires `SUPABASE_JWT_SECRET` environment variable. Run with:
```bash
export SUPABASE_JWT_SECRET="<jwt-secret>"
python3 e2e_test.py
```

```python
#!/usr/bin/env python3
"""Bitacora E2E Full Test - Patient Flow"""
import base64, hashlib, hmac, json, os, sys, time, uuid
from datetime import datetime, timezone
import urllib.request, urllib.error

BASE_URL = os.environ.get("BITACORA_BASE_URL", "https://api.bitacora.nuestrascuentitas.com")
JWT_SECRET = os.environ.get("SUPABASE_JWT_SECRET", "")
DB_CONN = os.environ.get("ConnectionStrings__BitacoraDb", "")
WEBHOOK_SECRET = os.environ.get("BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN", "")

def b64url(data: bytes) -> str:
    return base64.urlsafe_b64encode(data).rstrip(b"=").decode()

def make_jwt(sub: str, email: str, secret: str, exp_minutes: int = 15) -> str:
    header = b64url(json.dumps({"alg": "HS256", "typ": "JWT"}).encode())
    now = int(datetime.now(timezone.utc).timestamp())
    payload = b64url(json.dumps({"sub": sub, "email": email, "iat": now, "exp": now + exp_minutes * 60}).encode())
    sig = b64url(hmac.new(secret.encode(), f"{header}.{payload}".encode(), hashlib.sha256).digest())
    return f"{header}.{payload}.{sig}"

def api_call(method, path, token="", body=None, expected=None):
    url = f"{BASE_URL}{path}"
    headers = {"Accept": "application/json", "Content-Type": "application/json"}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    data = json.dumps(body).encode() if body else None
    req = urllib.request.Request(url, data=data, headers=headers, method=method)
    try:
        resp = urllib.request.urlopen(req, timeout=15)
        status = resp.status
        try:
            content = json.loads(resp.read())
        except:
            content = {}
    except urllib.error.HTTPError as e:
        status = e.code
        try:
            content = json.loads(e.read())
        except:
            content = {}
    expected = expected or [200]
    result = "PASS" if status in expected else ("CRITICAL" if status >= 500 else "FAIL")
    return {"method": method, "path": path, "status": status, "expected": expected, "result": result, "body": content}

if not JWT_SECRET:
    print("FATAL: SUPABASE_JWT_SECRET not set.")
    sys.exit(1)

patient_sub = str(uuid.uuid4())
patient_email = f"e2e+{patient_sub[:8]}@bitacora.nuestrascuentitas.com"
token = make_jwt(patient_sub, patient_email, JWT_SECRET)
results = []

print(f"Patient sub: {patient_sub}")
print(f"Base URL: {BASE_URL}\n")

# Health checks
print("--- Health Checks ---")
results.append(api_call("GET", "/health", expected=[200]))
results.append(api_call("GET", "/health/ready", expected=[200]))

# Step 1: Bootstrap
print("--- Step 1: Auth Bootstrap ---")
r = api_call("POST", "/api/v1/auth/bootstrap", token, expected=[200])
results.append(r)
user_id = r.get("body", {}).get("userId") or r.get("body", {}).get("user", {}).get("userId", "")

# Step 2: Consent current
print("--- Step 2: Consent Current ---")
r = api_call("GET", "/api/v1/consent/current", token, expected=[200])
results.append(r)
consent_version = r.get("body", {}).get("version", "1.0")

# Step 3: Negative test - mood without consent (MUST be 403)
print("--- Step 3: Consent Gate (NEGATIVE) ---")
r = api_call("POST", "/api/v1/mood-entries", token, {"score": 1}, expected=[403])
results.append(r)
if r["status"] != 403:
    print("CRITICAL: Mood entry succeeded WITHOUT consent! Security defect!")
elif r.get("body", {}).get("error", {}).get("code") != "CONSENT_REQUIRED":
    print(f"WARN: 403 but error code is not CONSENT_REQUIRED: {r.get('body', {})}")

# Step 4: Grant consent
print("--- Step 4: Grant Consent ---")
results.append(api_call("POST", "/api/v1/consent", token, {"version": consent_version, "accepted": True}, expected=[201, 409]))

# Step 5: Mood entry WITH consent
print("--- Step 5: Mood Entry ---")
results.append(api_call("POST", "/api/v1/mood-entries", token, {"score": 1}, expected=[200, 201]))

# Step 6: Daily checkin
print("--- Step 6: Daily Checkin ---")
results.append(api_call("POST", "/api/v1/daily-checkins", token, {
    "sleepHours": 7.5, "physicalActivity": True, "socialInteraction": False,
    "anxietyLevel": 2, "irritabilityLevel": 1, "tookMedication": True, "medicationTime": "22:00"
}, expected=[200, 201]))

# Step 7: Timeline
print("--- Step 7: Timeline ---")
results.append(api_call("GET", "/api/v1/visualizacion/timeline?from=2026-04-01&to=2026-04-30", token, expected=[200]))

# Step 8: Summary
print("--- Step 8: Summary ---")
results.append(api_call("GET", "/api/v1/visualizacion/summary?from=2026-04-01&to=2026-04-30", token, expected=[200]))

# Step 9: Vinculos
print("--- Step 9: Vinculos ---")
results.append(api_call("GET", "/api/v1/vinculos", token, expected=[200]))

# Step 10: Telegram pairing
print("--- Step 10: Telegram Pairing ---")
r = api_call("POST", "/api/v1/telegram/pairing", token, expected=[200])
results.append(r)

# Step 11: Telegram session
print("--- Step 11: Telegram Session ---")
results.append(api_call("GET", "/api/v1/telegram/session", token, expected=[200]))

# --- Results ---
print("\n" + "=" * 80)
print("E2E Test Results")
print("=" * 80)
print(f"{'Method':<7} {'Path':<55} {'Status':<8} {'Expected':<15} {'Result'}")
print("-" * 100)
for r in results:
    print(f"{r['method']:<7} {r['path']:<55} {r['status']:<8} {str(r['expected']):<15} {r['result']}")

pass_count = sum(1 for r in results if r["result"] == "PASS")
fail_count = sum(1 for r in results if r["result"] == "FAIL")
critical_count = sum(1 for r in results if r["result"] == "CRITICAL")
print(f"\nPASS: {pass_count} | FAIL: {fail_count} | CRITICAL: {critical_count}")
sys.exit(2 if critical_count > 0 else 1 if fail_count > 0 else 0)
```

## Resolution Steps to Unblock

### Option A: Populate infra/.env from Dokploy UI
1. Log into Dokploy at http://54.37.157.93:3000
2. Navigate to Bitacora project > bitacora-api application > Environment
3. Copy `SUPABASE_JWT_SECRET`, `ConnectionStrings__BitacoraDb`, and `BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN`
4. Paste into `infra/.env` on local machine
5. Re-run this E2E test

### Option B: Use Dokploy API
1. Generate a Dokploy API key from the UI
2. Set `DOKPLOY_API_KEY` in `infra/.env`
3. Use `dkp.sh env <BITACORA_API_APP_ID>` to list env vars
4. Extract secrets and run E2E test

### Option C: SSH to VPS
1. Add SSH key for root@54.37.157.93
2. `docker exec <bitacora-container> printenv | grep SUPABASE_JWT_SECRET`
3. Extract and run E2E test

### Recommended Next Session Prompt

```
Resume bitacora E2E test. Secrets have been provided in infra/.env.
Run: python3 .docs/raw/prompts/e2e_test.py
Then close with $ps-trazabilidad and $ps-auditar-trazabilidad.
```