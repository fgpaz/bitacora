<!--
target: codex
pressure: aggressive
generated: 2026-04-10
-->

Start a new Codex session for the next execution step of Wave 1 in Bitacora.

Mission:
- Execute the real next step of the wave under `prod-first`: build the production environment for the runnable backend that exists today.
- Treat this as `T01` execution with one explicit follow-up pulled forward: bootstrap the minimum executable backend smoke gate needed for a no-staging rollout.
- Do not invent or deploy runtimes that the repo does not have yet.

Use:
- `$ps-contexto` — mandatory first action
- `$mi-lsp` — mandatory for semantic navigation under `src/`
- `$brainstorming` — mandatory before edits; use it to resolve the follow-ups listed below
- `$ps-asistente-wiki` — mandatory before updating wiki/technical canon or plan state
- `$writing-plans` — use only if repo verification shows `T01` must be split into a smaller execution subwave or if the pulled-forward smoke bootstrap needs its own subplan
- `$dokploy-cli` — mandatory for Dokploy operations
- `$ssh-remote` — mandatory for remote host checks on `turismo`
- `$ps-trazabilidad` — mandatory before closure
- `$ps-auditar-trazabilidad` — mandatory because this is infra + runtime + docs and there is no staging

Treat this as repo-first work. Verify the actual state before trusting any sentence in this prompt.

Mandatory exploration before planning or execution:
- Use `$mi-lsp` as the primary semantic tool under `src/`.
- Dispatch a minimum of 5 `ps-explorer` subagents in parallel in a single message. If `ps-explorer` is unavailable, use common explorer agents, but still require `$mi-lsp`.
- Use these exploration objectives:
  1. Verify startup/config/bootstrap anchors in `src/Bitacora.Api`:
     `Program.cs`, `appsettings.json`, `appsettings.Development.json`, `.env.example`, middleware, migration toggle, health route.
  2. Verify PostgreSQL, EF Core, encryption, pseudonymization, and seed behavior:
     `Bitacora.DataAccess.EntityFramework`, `AppDbContext`, service registration, required env/config contract.
  3. Verify the operational seam for messaging/event bus:
     whether `EventBusSettings:HostAddress` keeps the app in `NoOp`, and whether RabbitMQ should stay disabled for this session.
  4. Verify the minimum executable test/smoke gap that matters for a prod-first environment:
     `.docs/wiki/06_matriz_pruebas_RF.md`, `TP-ONB`, `TP-CON`, `TP-REG`, `src/Bitacora.Tests`, current endpoint surface.
  5. Verify repo-local infra/bootstrap reality:
     whether `infra/` exists, what Dokploy/bootstrap artifacts exist, what prompt artifacts already exist, and what untracked files must not be auto-staged.
- Open these files early in the same pass because they are the strongest current anchors:
  - `src/Bitacora.Api/Program.cs`
  - `src/Bitacora.DataAccess.EntityFramework/DependencyInjection/ServiceCollectionExtensions.cs`
  - `src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs`
  - `src/Bitacora.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`
  - `src/Bitacora.Infrastructure/Services/AesEncryptionService.cs`
  - `src/Bitacora.Infrastructure/Services/PseudonymizationService.cs`
  - `src/Bitacora.Infrastructure/Services/ExternalProfileClient.cs`
  - `src/Bitacora.EventBus/EventBusConfigExtension.cs`
  - `src/Bitacora.EventBus/Options/EventBusSettings.cs`
  - `src/Bitacora.EventBus/EventBusServices/NoOpIntegrationEventPublisher.cs`
  - `src/Bitacora.EventBus/EventBusServices/EventBusServicePublisher.cs`
  - `src/Bitacora.Api/Middleware/ConsentRequiredMiddleware.cs`
  - `src/Bitacora.Api/Endpoints/Auth/AuthEndpoints.cs`
  - `src/Bitacora.Api/Endpoints/Consent/ConsentEndpoints.cs`
  - `src/Bitacora.Api/Endpoints/Registro/RegistroEndpoints.cs`
  - `src/Bitacora.Api/appsettings.json`
  - `src/Bitacora.Api/appsettings.Development.json`
- Cross-check contradictions before proceeding.

Workflow:
1. `$ps-contexto`
2. Mandatory exploration block: 5+ subagents + `$mi-lsp`
3. `$brainstorming`
4. `$ps-asistente-wiki`
5. Use `$writing-plans` only if needed to refine or split `T01` based on verified repo reality
6. Execute `T01`
7. Run the pulled-forward minimum smoke bootstrap
8. `$ps-trazabilidad`
9. `$ps-auditar-trazabilidad`

Repo-verified state to preserve on April 10, 2026:
- Remote repo is `git@github.com:fgpaz/bitacora.git`.
- Active branch is `main`.
- The branch is ahead by 1 commit because the planning portfolio was already committed.
- There is an untracked prompt artifact that must not be auto-staged:
  `.docs/raw/prompts/2026-04-10-wave-1-productivizacion-bootstrap.md`
- Current runnable surface is backend-only:
  - `POST /api/v1/auth/bootstrap`
  - `GET /api/v1/consent/current`
  - `POST /api/v1/consent`
  - `DELETE /api/v1/consent/current`
  - `POST /api/v1/mood-entries`
  - `POST /api/v1/daily-checkins`
  - `GET /health`
- `frontend/` does not exist in the repo.
- There is no real Telegram runtime in the repo.
- `src/Bitacora.Tests` is scaffold-only today.
- `infra/` does not exist today.
- `src/Bitacora.Api/.env.example` only exposes:
  - `SUPABASE_JWT_SECRET`
  - `BITACORA_ENCRYPTION_KEY`
  - `BITACORA_PSEUDONYM_SALT`
- `src/Bitacora.Api/appsettings.json` keeps `ApplyMigrationsOnStartup=false`, OTLP disabled by default, and still exposes `EventBusSettings`.
- Dokploy host exists on `turismo`, but local Dokploy bootstrap is missing because `infra/.env` or `DOKPLOY_API_KEY` is not yet available in the repo-local execution flow.
- UI slice gates remain blocked:
  - `ONB-001`, `REG-001`, `REG-002` have Stitch coverage but are still visually blocked
  - do not open `UI-RFC-*` in this session

Primary sources to read first:
- `AGENTS.md`
- `CLAUDE.md`
- `.docs/raw/plans/2026-04-10-wave-1-productivizacion.md`
- `.docs/raw/plans/wave-1/01-prod-infra-ops-dokploy-postgres-secrets-migraciones-observabilidad.md`
- `.docs/raw/plans/wave-1/10-qa-release-readiness-smokes-contract-e2e-prod.md`
- `.docs/wiki/07_baseline_tecnica.md`
- `.docs/wiki/09_contratos_tecnicos.md`
- `.docs/wiki/06_matriz_pruebas_RF.md`
- `src/README.md`
- `src/Bitacora.Api/Program.cs`
- `src/Bitacora.Api/appsettings.json`
- `src/Bitacora.Api/appsettings.Development.json`
- `src/Bitacora.Api/.env.example`
- `src/Bitacora.DataAccess.EntityFramework/DependencyInjection/ServiceCollectionExtensions.cs`
- `src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs`
- `src/Bitacora.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`
- `src/Bitacora.Infrastructure/Services/AesEncryptionService.cs`
- `src/Bitacora.Infrastructure/Services/PseudonymizationService.cs`
- `src/Bitacora.EventBus/EventBusConfigExtension.cs`

Mandatory `$brainstorming` follow-ups to resolve before implementation:
1. Production topology for this session.
   Recommended: create production only for what exists now, meaning dedicated PostgreSQL + `Bitacora.Api`. Do not create fake `frontend/` or fake Telegram apps. Preserve future slots as documented follow-up, not as live deployables.
2. Repo-local bootstrap contract for Dokploy and SSH.
   Recommended: create `infra/` as the first concrete repo-local bootstrap artifact set, including env templates and runbooks that let `$dokploy-cli` and `$ssh-remote` operate from this repo without hidden local state.
3. Minimal smoke bootstrap timing.
   Recommended: pull it forward into this session instead of leaving all executable smoke work for `T10`, because there is no staging.
4. Event bus posture in the first prod environment.
   Recommended: keep `EventBusSettings:HostAddress` unset and preserve the `NoOp` path unless repo verification shows a hard requirement for RabbitMQ in the environment bootstrap itself.
5. Readiness gate.
   Recommended: if `/health` is not enough for Dokploy or operational safety, treat that as a real gap and close it now; do not hand-wave it to a later wave.

Exact scope of this session:
- Create the repo-local production bootstrap artifacts required for Bitacora.
- Create or wire the Dokploy production environment for the current backend runtime on `turismo`, using the GitHub App path already configured in Dokploy.
- Create or wire the dedicated PostgreSQL service/database for Bitacora production.
- Freeze the secret contract and env contract without committing real secrets.
- Preserve manual/explicit migration policy for production.
- Pull forward and implement the minimum backend smoke bootstrap for the current API surface.
- Update documentation and plan state only where required by the executed truth.

Out of scope for this session:
- Do not build or deploy a Next.js runtime.
- Do not build Telegram runtime, pairing, reminders, or webhook handlers.
- Do not open or create `UI-RFC-*`.
- Do not reopen the global visual layer.
- Do not fabricate staging.
- Do not implement the whole `T10` suite; only pull forward the minimum backend smoke bootstrap needed for prod-first safety.

Severity rules:
- If the repo contradicts this prompt, trust the repo and name the contradiction.
- If a plan assumes `frontend/` or Telegram runtime already exist, correct that assumption before implementation.
- If `infra/.env`, `DOKPLOY_API_KEY`, or SSH host config is missing, treat that as a real blocker to live Dokploy execution and surface it immediately.
- If readiness, migrations, backups, or smoke ownership remain ambiguous, treat that as a blocker, not a TODO.
- If you are tempted to deploy a fake app just to "complete the topology", stop. Production environment here means truthful environment for the software that exists today.
- Do not stage or commit the untracked old prompt artifact unless explicitly asked.

Expected deliverables of the session:
- Repo-local `infra/` bootstrap artifacts created and saved to disk.
- Dokploy production app/database wiring completed for the current backend, or an explicit blocker documented with concrete evidence if credentials/config are missing.
- Production env contract documented and aligned with actual code/config anchors.
- Minimum executable backend smoke bootstrap added and run against the real current API surface.
- Relevant wiki/plan sync completed if runtime/config truth changed.
- A commit containing only the execution artifacts created in this session.

Close the session only when:
- the environment bootstrap outcome is concrete and evidence-backed,
- the pulled-forward minimum smoke bootstrap exists and is runnable,
- `$ps-trazabilidad` is complete,
- `$ps-auditar-trazabilidad` is complete.
