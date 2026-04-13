# Task T2: Write E2E Test Prompt

## Shared Context
**Goal:** Generate a comprehensive E2E test prompt using `ps-prompt` skill and write it to `.docs/raw/prompts/2026-04-13-bitacora-e2e-full-test.md`.
**Stack:** Bitacora API at `api.bitacora.nuestrascuentitas.com`, frontend at `bitacora.nuestrascuentitas.com`, Telegram bot `@mi_bitacora_personal_bot`, Supabase Auth.
**Architecture:** Patient E2E flow: onboarding -> consent grant -> mood entry -> daily checkin -> visualization. Telegram pairing via `BIT-XXXXX` codes.

## Task Metadata
```yaml
id: T2
depends_on: []
agent_type: ps-python
complexity: medium
done_when: ".docs/raw/prompts/2026-04-13-bitacora-e2e-full-test.md written to disk and committed"
```

## Reference
- `infra/smoke/backend-smoke.ps1` — existing smoke test (395 lines), use as reference for API call patterns
- `src/Bitacora.Api/Endpoints/` — 24 endpoints mapped
- `frontend/lib/api/client.ts` — patient API client (`bitacoraFetch`, `bootstrapPatient`)
- `frontend/middleware.ts` — auth JWT cookie flow
- `infra/mkey.yaml` — vault config for prod secrets

## Prompt

Create the E2E test prompt by following the `ps-prompt` skill workflow.

**The prompt must instruct the next agent to:**

1. **Bootstrap context**: Run `ps-contexto` first (mandatory before any action)
2. **DB verification**: Use `db-cli` to verify DB state before tests:
   ```bash
   db-cli query --conn bitacora-prod --sql "SELECT * FROM users LIMIT 5"
   db-cli query --conn bitacora-prod --sql "SELECT COUNT(*) FROM mood_entries"
   db-cli query --conn bitacora-prod --sql "SELECT COUNT(*) FROM consent_grants"
   ```
3. **Auth setup**: Get a real JWT from Supabase:
   - Option A: Use Supabase anon key + magic link email flow
   - Option B: Use `backend-smoke.ps1` pattern — build HS256 JWT with `SUPABASE_JWT_SECRET` from vault
   - The vault secret is in Infisical `bitacora prod` env at `teslita` vault
4. **Patient E2E walk (web frontend)**:
   - `GET /onboarding` -> verify onboarding page loads
   - `POST /api/v1/auth/bootstrap` with Supabase JWT -> creates patient user
   - `POST /api/v1/consent` with consent body -> creates ConsentGrant
   - `POST /api/v1/mood-entries` with score payload -> creates MoodEntry (consent-gated)
   - `POST /api/v1/daily-checkins` with factors payload -> creates DailyCheckin (consent-gated)
   - `GET /api/v1/visualizacion/timeline` -> retrieves timeline
   - `GET /api/v1/vinculos` -> retrieves care links
5. **Telegram pairing flow**:
   - `POST /api/v1/telegram/pairing` (authed) -> returns `BIT-XXXXX` code
   - Via Telegram bot `@mi_bitacora_personal_bot`: send `/start BIT-XXXXX`
   - `POST /api/v1/telegram/webhook` with X-Telegram-Webhook-Secret header -> webhook fires
6. **DB verification after**: verify records created in users, mood_entries, daily_checkins, consent_grants
7. **Traceability**: Use `ps-trazabilidad` to verify test validates FL → RF → TP chains:
   - FL-REG-01 (web mood registration) -> RF-REG-001, RF-REG-002, RF-REG-004, RF-REG-005
   - FL-REG-03 (daily checkin) -> RF-REG-020, RF-REG-021, RF-REG-022
   - FL-CON-01 (consent) -> RF-CON-001, RF-CON-002, RF-CON-003
   - FL-VIN-02 (auto-vinculacion) -> RF-VIN-010, RF-VIN-011, RF-VIN-012

**Write the prompt to:** `.docs/raw/prompts/2026-04-13-bitacora-e2e-full-test.md`

**The prompt file must include:**
- Exact instructions for each step
- Environment variables needed (from vault: `SUPABASE_JWT_SECRET`, `BITACORA_TELEGRAM_BOT_TOKEN`, `BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN`)
- Expected pass criteria for each API call
- Traceability chain mapping table

## Verify
File `.docs/raw/prompts/2026-04-13-bitacora-e2e-full-test.md` exists, >100 lines, includes all 7 steps above.

## Commit
`docs(prompt): add bitacora E2E full test prompt for wave-prod-mvp`
