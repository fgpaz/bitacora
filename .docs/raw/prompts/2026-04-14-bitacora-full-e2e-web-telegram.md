# Prompt: bitacora-wave-prod — Full E2E Testing (Web + Telegram)

## Metadata

- **Platform:** Codex
- **Pressure:** aggressive
- **Generated:** 2026-04-14
- **Target:** Resolve ALL remaining blockers for full production E2E testing (web + Telegram bot)
- **Workflow:** ps-contexto → brainstorm → writing-plans → wave execution → ps-trazabilidad → ps-auditar-trazabilidad

---

## Mission

Make bitacora production fully testable end-to-end via both:
1. **Web UI** (`https://bitacora.nuestrascuentitas.com`) — Next.js frontend
2. **Telegram bot** (`@bitacorav2_bot`) — via `mi-telegram-cli`

The backend API (`https://api.bitacora.nuestrascuentitas.com`) is already verified: **10/10 E2E tests pass** (see session `ed5daa9..cc9f62b`).

---

## Context from Previous Session

### What's working (from session `ed5daa9..cc9f62b`)

| What | Status |
|------|--------|
| Backend API (`/api/v1/*`) | 10/10 PASS |
| Auth bootstrap | Working |
| Consent grant/revoke | Working |
| Mood entry creation | Working |
| Daily checkin upsert | Working |
| Timeline + Summary queries | Working (DateTimeKind.Utc fix) |
| Vinculos list | Working |
| Credentials in `infra/.env` | Written (NOT yet in Infisical) |

### What's BROKEN or MISSING

| Item | Severity | Detail |
|------|----------|--------|
| Frontend NOT deployed | **CRITICAL** | No Dokploy spec, Node version mismatch |
| Telegram pairing UI NOT built | **CRITICAL** | Backend endpoint exists (`POST /api/v1/telegram/pairing`) but frontend has zero Telegram code |
| `Telegram:WebhookSecretToken` missing | **HIGH** | Webhook accepts any request (no secret validation) |
| `reminder_configs` table missing | **MEDIUM** | Table defined in model but NOT migrated to production DB |
| Telegram API credentials | **HIGH** | Need `MI_TELEGRAM_API_ID` + `MI_TELEGRAM_API_HASH` from my.telegram.org |
| Credentials NOT in Infisical | **MEDIUM** | infra/.env only; no SOPS backup; no age key |

---

## Mandatory First Step: ps-contexto

```bash
$ps-contexto
```

Then load:
- `.docs/wiki/01_alcance_funcional.md` — producto objetivo
- `.docs/wiki/02_arquitectura.md` — system overview
- `infra/.env` — all current credentials
- `AGENTS.md` — orchestrator mode rules

---

## SDD Workflow

```
1. $ps-contexto
2. $mi-lsp workspace list → validate workspace alias
3. $brainstorming
4. $writing-plans (large/risky task — mandatory plan)
5. Wave execution with subagents
6. $ps-trazabilidad per batch
7. Final $ps-trazabilidad + $ps-auditar-trazabilidad
```

---

## Exploration Block (run in parallel before planning)

Dispatch minimum 5 `ps-explorer` subagents with these distinct objectives:

1. **Frontend deployment**: Find frontend Dockerfile, Node version, any Dokploy/Vercel config, build args needed
2. **Telegram bot flow**: Trace the complete pairing flow (GeneratePairingCodeCommand → ConfirmPairingCommand), find what X-Telegram-Webhook-Secret is and whether it's set anywhere
3. **Reminder configs migration**: Find if there's a Flyway migration for reminder_configs, check what the ReminderWorker does if table is missing
4. **Frontend auth**: Find how frontend authenticates with Supabase (magic link flow), how onboarding bootstraps patients, what the Telegram pairing button should call
5. **mi-telegram-cli setup**: Read `/home/fgpaz/.agents/skills/mi-telegram-cli/SKILL.md` to understand what credentials are needed and how to authenticate

Use `$mi-lsp` for semantic navigation of `src/` (JWT auth, middleware, endpoint registration).

---

## All Credentials (from `infra/.env` — DO NOT LOSE)

```
DOKPLOY_API_KEY=zHdqJHChVWMCUwthJhaUepBDvJOEjMweBOkQLSklbyqmJmxSDvlHqCjBdUsgpYaI
DOKPLOY_URL=http://54.37.157.93:3000
BITACORA_PROJECT_ID=18WEM8BMIq-z_wgkrNlp8
BITACORA_API_APP_ID=UROM_r5ETX0rvs-1WZ3bi
ConnectionStrings__BitacoraDb=Host=postgres-reboot-solid-state-application-l55mww;Port=5432;Database=bitacora_db;Username=bitacora;Password=c3fd62bcf1bd6dba57682a06fbcabf93
SUPABASE_JWT_SECRET=srgGCnJ1ptHvLoleF9vb8WMlVDa1AZAqqBJs4CINAB1kqUxlrtm1-QtVfiwamDCt
BITACORA_ENCRYPTION_KEY=ERJY/JsAfer68SjiIt2CwRGAP+IeUyT7ZHlfhuMLugw=
BITACORA_PSEUDONYM_SALT=0a6e89ad2d6f6e4b6aacee6feac96891
BITACORA_TELEGRAM_BOT_TOKEN=8609908294:AAEQpubqrpf48pSL6ERAGwxx7lNgj7dUoYI
```

**ACTION: Save ALL credentials to Infisical vault (teslita or buho) FIRST before any deployment. Use `mkey` or Infisical CLI. If interactive login is blocked, create a machine identity token from the Infisical UI and use that.**

---

## Wave Plan (Large/Risky — writing-plans MANDATORY)

### Wave 1: Deploy Frontend

Goal: Frontend live at `https://bitacora.nuestrascuentitas.com`

Tasks:
1. Fix Node version mismatch — Dockerfile uses `node:20-slim` but `package.json` requires `22`. Update Dockerfile to `node:22-slim`
2. Create `infra/dokploy/bitacora-frontend.production.md` with domain `bitacora.nuestrascuentitas.com`
3. Configure env vars: `NEXT_PUBLIC_API_BASE_URL=https://api.bitacora.nuestrascuentitas.com`, `NEXT_PUBLIC_SUPABASE_URL=https://auth.tedi.nuestrascuentitas.com`
4. Deploy via Dokploy (`application.deploy` or `dkp.sh`)
5. Smoke test: verify `https://bitacora.nuestrascuentitas.com` loads
6. Smoke test: verify magic link email flow works

Deliverable: Frontend live and accessible.

### Wave 2: Build Telegram Pairing UI

Goal: Patient can link their Telegram account from the web UI

Tasks:
1. Find or create a Telegram settings section in the Next.js app
2. Create a component that calls `POST /api/v1/telegram/pairing` (authenticated patient)
3. Display the returned `BIT-XXXXX` code with copy button and Telegram bot link (`https://t.me/bitacorav2_bot`)
4. Show countdown timer (expires in 15 minutes)
5. After 15 min, show "Código expirado" with regenerate button
6. Integrate into onboarding flow OR as standalone settings page

Deliverable: User can generate a Telegram pairing code from the web UI.

### Wave 3: Fix Telegram Webhook Secret

Goal: Webhook has fail-closed secret validation

Tasks:
1. Generate a secure random secret token (32+ chars, base64)
2. Add `Telegram:WebhookSecretToken` to Dokploy env vars via `application.saveEnvironment`
3. Verify the token is NOT committed to git
4. Test: send webhook without `X-Telegram-Webhook-Secret` header → should return 200 with `Accepted: false`

Deliverable: Webhook secret configured in production.

### Wave 4: Telegram E2E via mi-telegram-cli

Goal: Full Telegram bot flow tested end-to-end

Prerequisites:
- Get `MI_TELEGRAM_API_ID` and `MI_TELEGRAM_API_HASH` from `https://my.telegram.org`
- Have a dedicated Telegram test account (NOT personal)
- Authenticate via `mi-telegram-cli`: `mi-telegram-cli auth login --method code --api-id XXX --api-hash XXX`

Test scenarios:
1. **Pairing flow**: Send `/start BIT-XXXXX` → bot replies "Cuenta vinculada"
2. **Mood entry flow**: Send `+2` → bot asks sleep → reply `7` → asks physical → reply `si` → ... → completes
3. **Consent denied via bot**: If patient has no consent, bot should redirect to web
4. **Reminder flow**: After pairing, verify next-day reminder is sent

### Wave 5: reminder_configs Migration

Goal: reminder_configs table created in production DB

Tasks:
1. Check if table exists in production DB (connect via `psql` through the bastion or `db-cli`)
2. If missing, create Flyway migration or run raw SQL to create the table
3. Verify `ReminderWorker` no longer throws `relation "reminder_configs" does not exist`

### Wave 6: Full E2E Smoke Test (Web + Telegram)

Run complete E2E across both channels:
1. Web: bootstrap → consent → mood entry → checkin → timeline
2. Telegram: generate pairing code from web → send `/start CODE` to bot → confirm pairing → send mood score → verify mood entry appears in web timeline

---

## Locked Decisions (DO NOT REOPEN)

1. Auth is Supabase JWT (magic link) — confirmed in frontend code
2. Telegram bot is `@bitacorav2_bot` — token: `8609908294:AAEQpubqrpf48pSL6ERAGwxx7lNgj7dUoYI`
3. Webhook path: `POST /api/v1/telegram/webhook`
4. Pairing code format: `BIT-XXXXX` (5 alphanumeric chars)
5. Pairing TTL: 15 minutes
6. Mood scale: -3 to +3
7. Telegram conversation state machine: Idle → AwaitingFactorSleep → ... → AwaitingFactorMedicationTime → Idle

---

## Known Tech Debt (DO NOT FIX in this session — document only)

- DailyCheckinRepository DateTime issue: **NOT A BUG** — `CheckinDate` is `DateOnly`, not `DateTime`, so no UTC conversion needed. (False alarm from auditor, verified by subagent.)
- Credentials NOT in Infisical: **PENDING** — machine identity token or interactive login required

---

## Success Criteria

| Criterion | Verification |
|-----------|-------------|
| `https://bitacora.nuestrascuentitas.com` loads | HTTP 200 |
| Magic link email sent | Check via test email inbox |
| Consent granted via web | HTTP POST /consent returns 201 |
| Mood entry created via web | HTTP POST /mood-entries returns 201 |
| Telegram pairing code generated | HTTP POST /telegram/pairing returns 200 with BIT-XXXXX |
| Telegram `/start CODE` linked | mi-telegram-cli: send message, verify bot reply |
| Mood entry via Telegram appears in web timeline | HTTP GET /visualizacion/timeline shows the entry |
| reminder_configs error gone | `sudo docker logs` shows no `relation "reminder_configs" does not exist` |

---

## Files to Read First

- `.docs/wiki/02_arquitectura.md` — system graph, Telegram is `webhook HTTPS`
- `infra/.env` — all current credentials
- `frontend/src/app/onboarding/page.tsx` — onboarding flow
- `src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs` — webhook + pairing endpoint
- `src/Bitacora.Api/Endpoints/Telegram/HandleWebhookUpdateCommand.cs` — bot conversation state machine
- `frontend/next.config.js` — output: standalone
- `frontend/Dockerfile` — Node version 20 vs package.json Node 22 mismatch
- `/home/fgpaz/.agents/skills/mi-telegram-cli/SKILL.md` — Telegram CLI usage

---

## Closure

Close with `$ps-trazabilidad` and `$ps-auditar-trazabilidad`. Verify:
- All 6 waves delivered
- Frontend URL live
- Telegram bot E2E passes
- Docs updated (deployment runbook, Telegram flow docs)
- No regression in backend API (re-run E2E smoke test to confirm 10/10 still pass)
