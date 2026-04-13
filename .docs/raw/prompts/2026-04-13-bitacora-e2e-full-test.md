# Bitacora Wave-Prod MVP: Full E2E Test Prompt

**Generated:** 2026-04-13  
**Platform:** Codex  
**Pressure:** aggressive  
**Task size:** large / multi-module  
**Owner:** bitacora-wave-prod-mvp plan — `.docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp.md`

---

```
Start a new session for bitacora-wave-prod E2E full test.

Treat this as repo-first work. Verify the actual state before trusting any sentence in this prompt.

Use $skill syntax (Codex):
- $ps-contexto — load project context (MANDATORY first action)
- $mi-lsp — semantic code navigation under src/ (MANDATORY for any code work)
- $ps-explorer — subagent dispatch for parallel exploration
- $ps-trazabilidad — traceability closure (MANDATORY)
- $ps-auditar-trazabilidad — cross-document audit for large/risky tasks
- $ps-qa-orchestrator — QA orchestration if quality gates needed
- $ps-asistente-wiki — wiki phase navigation if docs work opens

---

## Mission

Run a complete end-to-end smoke + flow test for Bitacora MVP against:
- Frontend: https://bitacora.nuestrascuentitas.com (may not be deployed yet — check T1 status first)
- Backend API: https://api.bitacora.nuestrascuentitas.com
- Telegram bot: @mi_bitacora_personal_bot (token: use TELEGRAM_BOT_TOKEN from vault)
- Database: bitacora-prod via db-cli

The test must walk the FULL patient E2E flow AND the Telegram pairing flow, verify DB records created, and close with ps-trazabilidad.

---

## Mandatory Exploration Block (BEFORE planning or execution)

Dispatch 5+ ps-explorer subagents IN PARALLEL (single message) with these exact objectives:

1. **Backend smoke test reference**: Read `infra/smoke/backend-smoke.ps1` — extract exact API call patterns, JWT building logic, expected status codes, consent-gated endpoints
2. **Frontend routing and auth**: Find all patient-facing routes in `frontend/app/(patient)/`, read `frontend/middleware.ts`, find how `sb-access-token` cookie is set/used
3. **Telegram pairing flow**: Trace `POST /api/v1/telegram/pairing` -> `BIT-XXXXX` code generation -> how `/start CODE` via bot resolves to `TelegramSession`
4. **Database verification**: Use db-cli to query `bitacora-prod` — find table schemas for `users`, `mood_entries`, `daily_checkins`, `consent_grants`, `telegram_pairing_codes`, `telegram_sessions`
5. **Consent gate implementation**: Find `ConsentRequiredMiddleware` or equivalent — verify exactly which endpoints are blocked without active ConsentGrant

Then use $mi-lsp to trace these symbols:
- `ICurrentPatientContextAccessor` — how PatientId is resolved from JWT
- `MoodEntry` — entity fields and multi-tenant filter
- `ConsentGrant` — how active consent is checked

---

## Workflow

1. **$ps-contexto** — load project context from `.docs/wiki/` — read 01_alcance, 02_arquitectura, 03_FL, 04_RF, 05_modelo_datos, 07_baseline_tecnica

2. **Mandatory exploration block** (5+ ps-explorer + $mi-lsp — see above)

3. **Auth setup** — get prod secrets from vault:
   ```bash
   # Option A: mkey
   bash ~/.agents/skills/mi-key-cli/scripts/mkey.sh pull bitacora prod
   # Option B: infisical CLI
   infisical vault pull --project=bitacora --env=prod
   ```
   Required env vars: `SUPABASE_JWT_SECRET`, `BITACORA_TELEGRAM_BOT_TOKEN`, `BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN`, `ConnectionStrings__BitacoraDb` (or `DATABASE_URL`)

4. **DB pre-check** — verify clean state before testing:
   ```bash
   db-cli query --conn bitacora-prod --sql "SELECT COUNT(*) as total FROM users"
   db-cli query --conn bitacora-prod --sql "SELECT COUNT(*) as total FROM mood_entries"
   db-cli query --conn bitacora-prod --sql "SELECT * FROM consent_grants LIMIT 1"
   ```

5. **Build JWT** — use HS256 JWT with SUPABASE_JWT_SECRET (from backend-smoke.ps1 pattern):
   - Header: `{"alg":"HS256","typ":"JWT"}`
   - Payload: `{"sub":"<patient-uuid>","email":"test@bitacora.local","role":"patient","exp":<futureUnixTimestamp>}`
   - Sign with `SUPABASE_JWT_SECRET` as base64-decoded key
   - Or: use Supabase anon key + magic link email flow if simpler

6. **Patient E2E flow (web API layer)** — test via direct API calls (frontend may not be deployed yet):
   a. `POST /api/v1/auth/bootstrap` with Supabase JWT -> creates/links patient user
   b. `GET /api/v1/consent/current` -> should return 200 even without consent
   c. `POST /api/v1/consent` with body `{"version":"1.0","accepted":true}` -> creates ConsentGrant (201)
   d. `POST /api/v1/mood-entries` with body `{"score":1}` -> creates MoodEntry (201 or 200)
   e. `POST /api/v1/daily-checkins` with body `{"sleepHours":7.5,"physicalActivity":true,"socialInteraction":false,"anxietyLevel":2,"irritabilityLevel":1,"tookMedication":true,"medicationTime":"22:00"}` -> creates DailyCheckin
   f. `GET /api/v1/visualizacion/timeline?from=2026-04-01&to=2026-04-30` -> returns timeline entries
   g. `GET /api/v1/visualizacion/summary?from=2026-04-01&to=2026-04-30` -> returns summary
   h. `GET /api/v1/vinculos` -> returns empty array or existing care links
   i. `POST /api/v1/telegram/pairing` -> returns `BIT-XXXXX` code with TTL

7. **Telegram pairing flow** (only if bot token available):
   a. `POST /api/v1/telegram/pairing` (authed) -> get `BIT-XXXXX`
   b. Note: cannot actually send Telegram message without real phone/device — skip actual bot interaction if not possible
   c. Verify `telegram_pairing_codes` table has the code: `db-cli query --conn bitacora-prod --sql "SELECT * FROM telegram_pairing_codes WHERE code='BIT-XXXXX'"`
   d. If bot test possible: send `/start BIT-XXXXX` to @mi_bitacora_personal_bot, then POST to webhook with X-Telegram-Webhook-Secret

8. **DB post-check** — verify records created:
   ```bash
   db-cli query --conn bitacora-prod --sql "SELECT id, patient_id, created_at FROM mood_entries ORDER BY created_at DESC LIMIT 3"
   db-cli query --conn bitacora-prod --sql "SELECT id, patient_id, sleep_hours, created_at FROM daily_checkins ORDER BY created_at DESC LIMIT 3"
   db-cli query --conn bitacora-prod --sql "SELECT id, patient_id, consent_version, granted_at FROM consent_grants LIMIT 3"
   ```

9. **ps-trazabilidad** — close traceability:
   - Map each API call to FL -> RF -> TP chain
   - FL-REG-01 -> RF-REG-001, RF-REG-002, RF-REG-003, RF-REG-004, RF-REG-005
   - FL-REG-03 -> RF-REG-020, RF-REG-021, RF-REG-022, RF-REG-023, RF-REG-024, RF-REG-025
   - FL-CON-01 -> RF-CON-001, RF-CON-002, RF-CON-003
   - FL-VIN-02 -> RF-VIN-010, RF-VIN-011, RF-VIN-012
   - FL-TG-01 (Telegram pairing) -> RF-REG-011, RF-REG-014

10. **ps-auditar-trazabilidad** — cross-document audit for large/risky

---

## Locked Decisions

- Test at API layer, not Playwright (frontend not yet reliably deployed)
- JWT built manually using SUPABASE_JWT_SECRET (HS256), not via Supabase client
- Telegram bot test is best-effort (real Telegram API requires device)
- DB verification via db-cli is authoritative for record creation
- Consent gate verified by testing 403 on mood entry WITHOUT prior consent grant

---

## Primary Sources

- `.docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp.md` — this plan's parent
- `infra/smoke/backend-smoke.ps1` — API call patterns reference (395 lines)
- `frontend/lib/api/client.ts` — patient API client
- `src/Bitacora.Api/Endpoints/` — 24 endpoint definitions
- `.docs/wiki/03_FL.md` + `03_FL/FL-REG-*.md` — FL inventory
- `.docs/wiki/04_RF.md` + `04_RF/RF-REG-*.md` — RF inventory

---

## Boundaries

- API-only test (not full Playwright E2E — frontend deployment T1 still blocked)
- No more than 20 API calls total
- No destructive DB operations (no DELETE, TRUNCATE)
- Secrets from vault only — never hardcode tokens

---

## Severity Rules

- if API returns unexpected status code, report as FAIL with actual vs expected
- if DB record not created after successful API call, report as CRITICAL
- if consent gate bypassed (mood entry succeeds without consent), report as CRITICAL security defect
- if Telegram pairing code not found in DB, report as FAIL
- treat any 5xx as infrastructure defect — escalate

---

## Deliverable

End the session with:
- File: `.docs/raw/prompts/2026-04-13-bitacora-e2e-full-test.md` written to disk (this file itself — it IS the prompt)
- Console: summary table of each API call result (PASS/FAIL/CRITICAL)
- Console: ps-trazabilidad closure summary showing all FL->RF chains verified
- Console: ps-auditar-trazabilidad APPROVED or GAPS FOUND verdict
```

---

## File Header

```yaml
# platform: Codex
# pressure: aggressive
# date: 2026-04-13
# task: bitacora-wave-prod E2E full test
# parent_plan: .docs/raw/plans/2026-04-13-bitacora-wave-prod-mvp.md
# skills: $ps-contexto, $mi-lsp, $ps-explorer, $ps-trazabilidad, $ps-auditar-trazabilidad, $ps-qa-orchestrator
```
