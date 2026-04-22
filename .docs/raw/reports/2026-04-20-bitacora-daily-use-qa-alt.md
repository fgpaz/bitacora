# Bitacora Daily Patient E2E — qa-alt

Date: 2026-04-20
Environment: production
Result: AMBER / not GREEN
GitHub bug: `#21`

## Summary

The common daily patient journey was executed end to end with the dedicated Telegram QA profile `qa-alt`.

Passed:
- Zitadel login and logout.
- Consent/onboarding gate.
- Fresh Telegram pairing with the canonical bot.
- Telegram check-in persisted and visible from web dashboard APIs for local date `2026-04-20`.
- Web mood entry and daily check-in persisted.
- Dashboard/timeline/summary remained accessible after records.
- Protected backend proxy returned `401` after logout.
- Relogin preserved Telegram link and dashboard access.
- Telegram evidence check found no clinical value or identifier echo in stored sanitized verdict.

Failed:
- Telegram reminder schedule save returned `500 UNEXPECTED_ERROR` for a valid UI time option.

## Evidence

Evidence directory:
`artifacts/e2e/2026-04-20-bitacora-daily-use-qa-alt/`

Key files:
- `README.md`
- `telegram-sanitized.json`
- `01-dashboard-after-login.png`
- `02-telegram-linked.png`
- `03-dashboard-after-telegram.png`
- `04-dashboard-after-web-records.png`
- `05-reminder-2200-attempt.png`
- `06-logout-protected-redirect.png`
- `07-relogin-telegram-persisted.png`

## Privacy

No `chat_id`, phone, personal username, pairing code, cookies, auth codes, JWTs, refresh tokens, PATs, passwords, bot tokens, session blobs, DB URIs, patient ids, or clinical payloads were intentionally stored in the textual evidence.

## Follow-up

Fix `#21` before calling this daily-use journey GREEN.
