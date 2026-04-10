# Task T01: Production Infra, Dokploy, PostgreSQL, Secrets, Migrations, Observability, and Minimum Backend Smoke

## Shared Context
**Goal:** Bootstrap the truthful production environment for the backend runtime that exists today and pull forward the minimum smoke gate needed for a no-staging rollout.
**Stack:** Dokploy, Traefik, PostgreSQL, Supabase Auth, .NET 10.
**Architecture:** `prod-first` on `turismo`, no staging. This task creates the repo-local infra contract that was missing, freezes deploy/secret/backup decisions, and materializes the minimum backend-only operability needed before later waves consume production.

## Task Metadata
```yaml
id: T01
depends_on: []
agent_type: ps-worker
files:
  - create: infra/README.md
  - create: infra/.env.template
  - create: infra/dokploy/bitacora-api.production.md
  - create: infra/dokploy/bitacora-db.production.md
  - create: infra/dokploy/production-checklist.md
  - create: infra/runbooks/production-bootstrap.md
  - create: infra/runbooks/manual-migrations.md
  - create: infra/runbooks/backend-smoke.md
  - create: infra/runbooks/backup-and-restore.md
  - create: infra/runbooks/secret-source.md
  - create: infra/observability/otlp-contract.md
  - create: infra/smoke/backend-smoke.ps1
  - create: Dockerfile
  - create: src/Bitacora.Api/Dockerfile
  - create: src/Bitacora.Api/Health/ReadinessProbe.cs
  - modify: src/Bitacora.Api/.env.example
  - modify: src/Bitacora.Api/Program.cs
  - modify: src/Bitacora.Api/appsettings.json
  - read: AGENTS.md
  - read: .docs/wiki/07_baseline_tecnica.md
  - read: .docs/wiki/08_modelo_fisico_datos.md
  - read: .docs/wiki/09_contratos_tecnicos.md
complexity: high
done_when: "Backend-only production bootstrap artifacts, Dokploy contract, DB/bootstrap, readiness, smoke, observability, and backup runbooks exist in-repo and no production decision is left implicit."
```

## Reference
- `AGENTS.md` — deploy target is Dokploy on `turismo` and must use `dokploy-cli` plus `ssh-remote`
- `.docs/wiki/07_baseline_tecnica.md` — current production is only planned, not materialized
- `src/Bitacora.Api/appsettings.json` — current runtime defaults and secret surface

## Prompt
Create the missing production substrate in-repo. Do not assume staging, Kubernetes, a second host, a frontend runtime, or a Telegram runtime.

1. Create `infra/` because it does not exist today.
2. Add a repo-local `.env.template` that documents only the variables needed for Bitacora backend production bootstrap: `DOKPLOY_URL`, `DOKPLOY_API_KEY`, API app ID, database credentials, `SUPABASE_JWT_SECRET`, encryption key, pseudonym salt, OTLP endpoint, and smoke inputs.
3. Freeze a production topology with:
   - one Dokploy app for `Bitacora.Api`
   - one dedicated PostgreSQL service / database for Bitacora
   - Traefik-managed public routing
   - no fake frontend or Telegram deployables in this session
4. Use the GitHub App path already configured in Dokploy; do not assume image registries or manual tar uploads.
5. Specify the exact production-first rollout order: DB, API secrets, migrations/bootstrap, API deploy, readiness check, smoke checks, backup verification.
6. Document how `dokploy-cli` and `ssh-remote` are used from this repo once `infra/.env` exists.
7. Freeze migration policy: startup migrations stay disabled by default; production migrations run explicitly from release operations, not implicitly on boot.
8. Close the readiness gap beyond `/health`, and specify the exact smoke sequence before traffic is considered open.
9. Define observability: structured logs, `trace_id`, OTLP exporter, minimal dashboards/alerts, and what constitutes a production incident for this app.
10. Define backup and restore expectations for the dedicated PostgreSQL database, including cadence, retention, restore drill, and ownership.
11. Update `src/Bitacora.Api/.env.example`, `src/Bitacora.Api/appsettings.json`, and startup wiring only to align local examples with the finalized production secret contract; do not put real secrets in repo.
12. Explicitly state that no staging environment exists in this wave and compensate with stronger prod-first smoke and rollback criteria.

## Execution Waves
### Wave A — Repo-local bootstrap
- Create `infra/` structure, secret templates, Dokploy bootstrap checklist, and production topology docs.
- Freeze canonical names for the API app, DB service, domain, and backup artifacts.

### Wave B — Production deploy contract
- Document exact Dokploy creation flow through GitHub App, env vars, Dockerfile path, ports, domains, and health gates.
- Define explicit migration/bootstrap sequence and rollback points.

### Wave C — Operability
- Add backup/restore runbook, OTLP contract, alert list, prod smoke runbook, and on-call style failure matrix.
- Pull forward the minimum executable backend smoke gate that matters for no-staging rollout.

## Skeleton
```text
infra/
  .env.template
  README.md
  dokploy/
    bitacora-api.production.md
    bitacora-db.production.md
    production-checklist.md
  observability/
    otlp-contract.md
  runbooks/
    production-bootstrap.md
    backup-and-restore.md
    manual-migrations.md
    backend-smoke.md
    secret-source.md
  smoke:
    backend-smoke.ps1
```

## Verify
`pwsh -Command "& '$HOME\\.agents\\skills\\dokploy-cli\\scripts\\dkp.ps1' doctor"` -> local bootstrap resolves `DOKPLOY_API_KEY` and `DOKPLOY_URL` once `infra/.env` is created.
`pwsh -File .\\infra\\smoke\\backend-smoke.ps1` -> minimum backend smoke gate is executable once runtime secrets exist.

## Commit
`feat(infra): implement T01 backend production bootstrap`
