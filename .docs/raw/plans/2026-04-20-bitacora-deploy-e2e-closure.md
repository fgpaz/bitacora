# Bitacora Deploy E2E Closure Plan

**Goal:** deploy the reminder/UI hardening release, remove traceability blockers, and run a production E2E closure.

**Architecture:** keep the UTC storage contract, deploy the current `main` commit through Dokploy for API and frontend, then validate the patient journey with sanitized evidence. Documentation updates are limited to runtime drift, UX validation status, and board/issue closure state.

**Tech Stack:** .NET 10, Next.js 16, Dokploy, Zitadel OIDC, Telegram CLI, Playwright.

**Context Source:** `ps-contexto` + `mi-lsp` on 2026-04-20. Governance is valid and in sync; decision priority remains Security > Privacy > Correctness > Usability > Maintainability > Performance > Cost > Time-to-market. Active scope is RF-TG-006, TG runtime, patient UI, production deployment, E2E evidence, and traceability audit.

**Runtime:** Codex

**Available Agents:** local Codex tools and project skills; no new subagents spawned in this closure pass.

**Initial Assumptions:** production deploy is driven by GitHub `main` through Dokploy; `qa-alt` remains the dedicated Telegram QA profile; all evidence must be sanitized.

---

## Waves

1. **Local closure:** repair governance trace hierarchy, stale TG handoff runtime notes, TG-002 validation placeholder, and remaining patient UI color-token drift.
2. **Release gate:** run backend tests, frontend typecheck/lint/build, Zitadel smoke, Telegram auth status, and `mi-lsp` governance/trace checks.
3. **Deploy:** commit only scoped files, push to `main`, trigger Dokploy API and frontend deploys, and verify health/readiness.
4. **E2E:** run production patient journey for Telegram settings/reminder `22:00`, linked session persistence, logout fail-closed, dashboard/mobile screenshots, and Telegram CLI checks where needed.
5. **Closure:** update evidence, issue `#21`, board state, `ps-trazabilidad`, then `ps-auditar-trazabilidad`.

## Stop Conditions

- Any command prints or requires exposing secrets, tokens, cookies, `chat_id`, patient IDs, or clinical payloads.
- Dokploy cannot confirm app IDs without sanitized metadata.
- Production E2E cannot authenticate without an existing non-chat credential path or interactive browser session.
