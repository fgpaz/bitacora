# Task T01: Production Infra, Dokploy, PostgreSQL, Secrets, Migrations, and Observability

## Shared Context
**Goal:** Bootstrap the single production environment that every later task will target.
**Stack:** Dokploy, Traefik, PostgreSQL, Supabase Auth, .NET 10, Next.js 16, Telegram.
**Architecture:** `prod-first` on `turismo`, no staging. This task creates the repo-local infra contract that is missing today and freezes deploy/secret/backup decisions before feature work consumes them.

## Task Metadata
```yaml
id: T01
depends_on: []
agent_type: ps-worker
files:
  - create: infra/README.md
  - create: infra/.env.template
  - create: infra/dokploy/bitacora-api.production.md
  - create: infra/dokploy/bitacora-web.production.md
  - create: infra/dokploy/bitacora-db.production.md
  - create: infra/dokploy/production-checklist.md
  - create: infra/runbooks/backup-and-restore.md
  - create: infra/runbooks/production-bootstrap.md
  - create: infra/observability/otlp-contract.md
  - modify: src/Bitacora.Api/.env.example
  - modify: src/Bitacora.Api/appsettings.json
  - read: AGENTS.md
  - read: .docs/wiki/07_baseline_tecnica.md
  - read: .docs/wiki/08_modelo_fisico_datos.md
  - read: .docs/wiki/09_contratos_tecnicos.md
complexity: high
done_when: "Production bootstrap artifacts, Dokploy contract, DB/bootstrap, observability, and backup runbooks exist in-repo and no production decision is left implicit."
```

## Reference
- `AGENTS.md` — deploy target is Dokploy on `turismo` and must use `dokploy-cli` plus `ssh-remote`
- `.docs/wiki/07_baseline_tecnica.md` — current production is only planned, not materialized
- `src/Bitacora.Api/appsettings.json` — current runtime defaults and secret surface

## Prompt
Create the missing production substrate in-repo. Do not assume staging, Kubernetes, or a second host.

1. Create `infra/` because it does not exist today.
2. Add a repo-local `.env.template` that documents only the variables needed for Bitacora production bootstrap: `DOKPLOY_URL`, `DOKPLOY_API_KEY`, API/web app IDs, database credentials, `SUPABASE_JWT_SECRET`, encryption key, pseudonym salt, Telegram token, reminder cadence, OTLP endpoint, and backup destination.
3. Freeze a production topology with:
   - one Dokploy app for `Bitacora.Api`
   - one Dokploy app for `frontend/`
   - one dedicated PostgreSQL service / database for Bitacora
   - Traefik-managed public routing
4. Use the GitHub App path already configured in Dokploy; do not assume image registries or manual tar uploads.
5. Specify the exact production-first rollout order: DB, API secrets, API deploy, migrations/bootstrap, web deploy, Telegram webhook registration, smoke checks, backup verification.
6. Document how `dokploy-cli` and `ssh-remote` are used from this repo once `infra/.env` exists.
7. Freeze migration policy: startup migrations stay disabled by default; production migrations run explicitly from release operations, not implicitly on boot.
8. Define readiness and liveness checks beyond `/health` if required for Dokploy gating, and specify the exact smoke sequence before traffic is considered open.
9. Define observability: structured logs, `trace_id`, OTLP exporter, minimal dashboards/alerts, and what constitutes a production incident for this app.
10. Define backup and restore expectations for the dedicated PostgreSQL database, including cadence, retention, restore drill, and ownership.
11. Update `src/Bitacora.Api/.env.example` and `src/Bitacora.Api/appsettings.json` only to align local examples with the finalized production secret contract; do not put real secrets in repo.
12. Explicitly state that no staging environment exists in this wave and compensate with stronger prod-first smoke and rollback criteria.

## Execution Waves
### Wave A — Repo-local bootstrap
- Create `infra/` structure, secret templates, Dokploy bootstrap checklist, and production topology docs.
- Freeze canonical names for the API app, web app, DB service, domain, and backup artifacts.

### Wave B — Production deploy contract
- Document exact Dokploy creation flow through GitHub App, env vars, build commands, ports, domains, and health gates.
- Define explicit migration/bootstrap sequence and rollback points.

### Wave C — Operability
- Add backup/restore runbook, OTLP contract, alert list, prod smoke runbook, and on-call style failure matrix.

## Skeleton
```text
infra/
  .env.template
  README.md
  dokploy/
    bitacora-api.production.md
    bitacora-web.production.md
    bitacora-db.production.md
    production-checklist.md
  observability/
    otlp-contract.md
  runbooks/
    production-bootstrap.md
    backup-and-restore.md
```

## Verify
`pwsh -Command "& '$HOME\\.agents\\skills\\dokploy-cli\\scripts\\dkp.ps1' doctor"` -> local bootstrap resolves `DOKPLOY_API_KEY` and `DOKPLOY_URL` once `infra/.env` is created.

## Commit
`docs(plan): implement T01 production infra bootstrap`

