# Bitacora Infra Bootstrap

This directory is the repo-local bootstrap layer for the current production surface:

- `Bitacora.Api`
- dedicated PostgreSQL
- backend-only smoke and operability assets

Out of scope for this bootstrap:

- `frontend/`
- Telegram runtime
- staging

Pending work after this backend-only bootstrap is governed by `.docs/plans/wave-prod/`.
`.docs/raw/plans/wave-1/` remains historical context only.

## Files

- `.env.template`: local-only environment contract for Dokploy, runtime, and smoke execution
- `dokploy/`: production specs for the API app and PostgreSQL service
- `runbooks/`: operator steps for bootstrap, migraciones, secretos, humo, backup y restauracion
- `observability/`: runtime telemetry and incident contract
- `smoke/`: executable backend smoke gate

## Secret Source

Bitacora does not keep control-plane secrets in git.

Use the shared `mi-key-cli` setup from `C:\repos\mios\multi-tedi` to source:

- `DOKPLOY_URL`
- `DOKPLOY_API_KEY`
- `DOKPLOY_ENVIRONMENT_ID`
- `DOKPLOY_GITHUB_PROVIDER_ID`
- shared auth secrets when needed for `SUPABASE_JWT_SECRET`

Then copy only the required values into the local untracked `infra/.env`.

Do not commit `infra/.env`.

## Runtime Secrets Contract

Every production deployment must have these variables set via Dokploy:

| Variable | Validation | Fail-closed |
|----------|------------|-------------|
| `SUPABASE_JWT_SECRET` | Non-empty string | Startup throws if missing |
| `BITACORA_ENCRYPTION_KEY` | 32 bytes Base64-decoded | `/health/ready` returns 503 if invalid; writes blocked |
| `BITACORA_PSEUDONYM_SALT` | Non-empty string | Any operation needing pseudonym throws 500 |
| `ConnectionStrings__BitacoraDb` | Valid PostgreSQL connection string | `/health/ready` returns 503 if unreachable |
| `DataAccess:ApplyMigrationsOnStartup` | `false` in production | Migrations run via `infra/runbooks/manual-migrations.md` only |

See `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md` for the complete fail-closed gate catalog.

## Rollout Order (terminal validation)

1. Secrets materialized in Dokploy.
2. `bitacora-db` deployed and reachable.
3. Manual migrations via runbook.
4. `GET /health/ready` returns 200.
5. `bitacora-api` deployed.
6. Smoke gate `infra/smoke/backend-smoke.ps1` passes (exit 0).
7. Only then: open traffic or start next surface rollout phase.

**Validation de UI es actividad terminal.** No se marca ninguna fase como completa hasta que la validacion UX tenga evidencia.

## Rollout Order (Phase 30 → 31 → 40 → 41 → 50 → 60)

Consulte `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md` para el catalogo completo de gates de rollout.

### Phase 30 — Backend basico

- [ ] Secrets en Dokploy: `SUPABASE_JWT_SECRET`, `BITACORA_ENCRYPTION_KEY`, `BITACORA_PSEUDONYM_SALT`, `ConnectionStrings__BitacoraDb`
- [ ] `bitacora-db` deployado y reachable
- [ ] Migraciones aplicadas via `infra/runbooks/manual-migrations.md`
- [ ] `GET /health/ready` retorna 200
- [ ] `infra/smoke/backend-smoke.ps1` pasa GATE-SMOKE-001..006 (exit 0)

### Phase 31 — Telegram webhook + recordatorios

- [ ] `TELEGRAM_BOT_TOKEN` y `BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN` en Dokploy
- [ ] Webhook registrado en Telegram Bot API
- [ ] Smoke Telegram pasa: GATE-SMOKE-013 (pairing), GATE-SMOKE-014 (session), GATE-SMOKE-015 (webhook)
- [ ] ReminderWorker activo: GATE-SMOKE-TG-001..003 pasando

### Phase 40 — Frontend web Next.js 16

- [ ] Secrets Next.js: `SUPABASE_URL`, `NEXT_PUBLIC_SUPABASE_URL`, `SUPABASE_JWT_SECRET`
- [ ] Deployment a Vercel o Dokploy
- [ ] Smoke web pasa: GATE-SMOKE-007..012 (exit 0)
- [ ] **UX validation evidencia en wiki**

### Phase 41 — Profesional dashboard

- [ ] Smoke profesional pasa: GATE-SMOKE-VIN-PROF-001..002, GATE-SMOKE-VIS-PROF-001
- [ ] **UX validation evidencia en wiki**

### Phase 50 — Alertas y notificaciones push

- [ ] Canal push configurado y consentido
- [ ] Smoke notificaciones pasando

### Phase 60 — UX validation terminal

- [ ] Evidencia de todas las surfaces integrada en wiki
- [ ] Decision de go/no-go documentada

## Phase Readiness Checklist (antes de abrir trafico)

Por cada phase, verificar:

1. **Secrets:** todas las variables requeridas para la phase estan en Dokploy
2. **Migraciones:** aplicadas antes del deploy, nunca en startup
3. **Readiness:** `GET /health/ready` retorna 200
4. **Smoke:** `infra/smoke/backend-smoke.ps1` pasa con exit 0
5. **UX validation:** evidencia documentada en wiki (Phase 40, 41, 60)
6. **Rollback plan:** restauracion desde backup si smoke falla

See `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md` for the full rollout gate catalog.
