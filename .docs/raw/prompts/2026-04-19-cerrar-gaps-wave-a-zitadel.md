<!--
target_platform: codex
pressure: aggressive
generated_at: 2026-04-19
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-19-cerrar-gaps-wave-a-zitadel.md' | Set-Clipboard"
-->

# Misión

Estás en `C:\repos\mios\humor` (proyecto Bitácora — clinical mood tracker, .NET 10 + Next.js 16 + Telegram Bot + PostgreSQL, deploy Dokploy VPS `54.37.157.93` alias `turismo`). Wave A del Epic IdP Zitadel Teslita cerró **PARTIAL** el 2026-04-19 y dejó 7 gaps específicos documentados.

Tu misión es **cerrar los 7 gaps** para que Wave A pase de PARTIAL a GREEN completo antes de que Wave B (fgpaz/bitacora#17) arranque. No estás reabriendo la Wave A; estás completando lo que quedó pendiente con evidencia firmada y commit por gap.

Referencias obligatorias antes de tocar nada:

- **Epic tracking:** https://github.com/fgpaz/bitacora/issues/15
- **Wave A cerrada PARTIAL:** https://github.com/fgpaz/bitacora/issues/16 (CLOSED, pero los gaps los cerrás en issues nuevos o sub-tareas, NO reabras #16)
- **Wave B bloqueada por esto:** https://github.com/fgpaz/bitacora/issues/17
- **Contrato técnico Zitadel:** `.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md` sección 11 "Gaps conocidos"
- **Reports Wave A:** `.docs/raw/reports/2026-04-18-wave-a-audit/T1.{1,2,3,4}-*.md`
- **Plan Wave A:** `.docs/raw/plans/2026-04-18-wave-a-stand-up-zitadel-teslita.md`
- **Smoke evidence:** `artifacts/e2e/2026-04-19-zitadel-wave-a-smoke/README.md`
- **Memoria cross-proyecto:** `~/.claude/projects/C--repos-mios-humor/memory/project_idp_zitadel_multi_ecosistema.md` (sección "Wave A — completada PARTIAL 2026-04-19")
- **Policy env Infisical:** `~/.claude/projects/C--repos-mios-humor/memory/feedback_env_vars_infisical.md` — **todo secret va a Infisical via `mi-key-cli`, nunca solo en `.env`**.

# Verificación previa (obligatoria antes de decidir nada)

1. `git log -5 --oneline` — commit base debe incluir `61f17b6 feat(wave-a): Zitadel IdP Teslita stood-up`. Si no, rama equivocada o sesión equivocada.
2. `git status --short` — working tree limpio o solo con prompts/reports untracked. Si hay WIP, reportá y pará.
3. `gh issue view 16 --repo fgpaz/bitacora --json state` — state debe ser `CLOSED`. Si sigue OPEN, algo salió mal, parar.
4. `curl -sS -o /dev/null -w "%{http_code}\n" https://id.nuestrascuentitas.com/.well-known/openid-configuration` — debe devolver **200**. Si 404, Zitadel cayó, escalar.
5. Cargar secrets: `bash $HOME/.claude/skills/mi-key-cli/scripts/mkey.sh pull bitacora prod` — debe pullear >=60 keys (50+ de Wave A + preexistentes).
6. Verificar PAT admin funciona:
   ```bash
   PAT=$(grep '^ZITADEL_ADMIN_PAT=' infra/.env | cut -d= -f2-)
   curl -sS -H "Authorization: Bearer $PAT" https://id.nuestrascuentitas.com/management/v1/orgs/me | jq .org.name
   ```
   Debe devolver `"nuestrascuentitas"`. Si 401, el PAT vino vacío o expiró, parar y escalar.
7. Leé el smoke README (`artifacts/e2e/2026-04-19-zitadel-wave-a-smoke/README.md`) sección "Pending (gaps documentados)" — esa es la lista canónica de los 7 gaps.

# Decisiones lockeadas de Wave A (NO reabrir)

1. **Stack IdP:** Zitadel v4.9.0 self-hosted en `id.nuestrascuentitas.com`. No reevaluar.
2. **Postgres:** dedicada `shared_zitadel_v4` en pg17, container `postgres-reboot-wireless-panel-chhbwg.1.*`, volumen `postgres-reboot-wireless-panel-chhbwg-data`.
3. **Masterkey:** `ZITADEL_MASTERKEY` en Infisical. **Nunca regenerar** (destruye encryption DB).
4. **Admin:** `paz.fgabriel@gmail.com` (HUMAN, IAM Owner default) + `zitadel-admin-sa` + `login-client` (MACHINE, PATs en Infisical).
5. **4 Orgs:** `nuestrascuentitas` (default), `bitacora`, `multi-tedi`, `gastos`.
6. **OAuth Clients:** 4 projects + 4 Web PKCE + 4 API client_credentials (IDs/secrets en Infisical con prefix `ZITADEL_CLIENT_<ORG>_*`).
7. **SMTP:** Resend activo (config id `369203306586702095` via `/admin/v1/smtp`).
8. **Backup strategy:** docker volume snapshot + offsite. NO pg_dump. Script en `infra/backups/zitadel/snapshot.sh`.
9. **Secrets:** todos en Infisical `teslita/bitacora/prod` via `mkey`. Nunca solo en `.env`.
10. **Sobre Supabase Auth:** **NO apagarlo**. Sigue corriendo para Bitácora. Wave B lo migra después.

Si durante exploración aparece evidencia fundada para reabrir alguna de estas decisiones, detenete y escalá via `$brainstorming`. No actúes.

# Workflow obligatorio (SDD, Large/Risky route por ser multi-gap + infra)

## Paso 1 — Contexto + exploración

1. `$ps-contexto` — bloqueante. Aunque los gaps son infra + docs + policy, igual cargá scope + arquitectura + baseline técnica + CT-AUTH-ZITADEL.
2. `$mi-lsp` — validá workspace alias `bitacora`. Si `mi-lsp workspace status bitacora --format toon` retorna `hint` o `next_hint`, seguilo antes de fallback. Uso mínimo (poco código propio), pero la regla del CLAUDE.md aplica.
3. **Dispatch EN PARALELO en UN solo turno ≥3 subagents `ps-explorer` (ampliar a 5 si encontrás signals).** Objetivos obligatorios:

   - **Explorer 1 — Zitadel live state re-audit:** leer `.docs/raw/reports/2026-04-18-wave-a-audit/T1.2-zitadel-mgmt-audit.md` + consultar `GET /management/v1/orgs/_search`, `GET /management/v1/users/_search`, `GET /admin/v1/policies/*` contra el live. Reportar: diff desde el snapshot del 2026-04-19, usuarios nuevos (esperable: 0), policies activas, apps por project.
   - **Explorer 2 — Legacy Postgres inventory:** validar que `postgres-bypass-wireless-bus-tupzoj` (postgresId `5e5bKMG5jBS30S7d8nn6c`, DB `shared_zitadel` en pg18) está realmente vacío vía `$ssh-remote` (`docker exec` + `psql -c '\l+'` + `SELECT count(*) FROM eventstore.events2`). Reportar: tamaño del volumen, si tiene instance.added events, recomendación para cleanup seguro.
   - **Explorer 3 — Dokploy stack capacity & login companion feasibility:** via `$dokploy-cli` (`dkp.sh` con `DOKPLOY_API_KEY` de Infisical) inspeccionar capacidad VPS (free RAM, disk via `$ssh-remote` `free -h && df -h`), verificar que no hay conflicts para agregar un 2do app Docker en project `teslita-shared-idp`, evaluar image `ghcr.io/zitadel/zitadel-login:v4.9.0` requirements (env vars, port 3000). Reportar: compatibilidad, env vars requeridos para el companion, Traefik path-routing strategy.

   **Explorer 4 (condicional, requerido si cualquiera de estos aparece):**
   - El VPS está cerca de saturación (<1GB RAM libre) → evaluar alternativas a deploy companion (ej. integrar UI simple en backend Bitácora).
   - La legacy Postgres tiene eventos/datos reales → no es safe removal.
   - Cualquier driver del companion Zitadel v4.9.0 requiere cambios retroactivos en la Zitadel core (ej. env var nueva en core).

   **Explorer 5 (condicional, requerido si):**
   - DKIM/SPF tests fallan (el user todavía no verificó) → investigar DNS nuestrascuentitas.com a nivel registrar + CloudFlare para re-emitir records.
   - La policy de client credentials grant requiere actualizar `appType` o `projectAuthorization` en cada API client existente → evaluar si hay que recrear clients (impacto a ZITADEL_CLIENT_*_API_SECRET persistidos).

4. Toda cita `file:line`, commit hash, secret key name, o container name debe estar verificada con `$mi-lsp`, read directo, `$ssh-remote`, o `$dokploy-cli`. Nada inventado.

## Paso 2 — Brainstorming (obligatorio)

`$brainstorming` con protocolo mandatorio por cada decisión táctica: learning context → applied impact → ASCII diagram → pros/cons → recommended → **una** pregunta vía `request_user_input`.

Decisiones a lockear en este orden:

1. **Estrategia login UI v2** (el gap #1, bloqueante para Wave B end-user).
   - (A) Deploy `zitadel-login:v4.9.0` como 2do App Docker en Dokploy, Traefik path-routing `/ui/v2/*` → puerto 3000 companion. **Requiere token `login-client` PAT ya persistido.** Default recomendado.
   - (B) Omitir login UI por ahora. Wave B inicia con solo M2M flows (`client_credentials` + JWT validation), login end-user viene después.
   - (C) Fork companion con auto-config para eliminar service user token requirement.
   Default (A). Si (A), decidir Traefik routing: middleware stripPrefix o pass-through?

2. **Offsite backup provider.**
   - Backblaze B2 ($0.006/GB-mes, rclone nativo) — recomendado MVP.
   - Cloudflare R2 (free egress, $0.015/GB-mes).
   - Hetzner Storage Box ($3.81/mes 1TB flat).
   - rsync al desktop del user via Tailscale (gratis pero depende de uptime desktop).
   Default B2.

3. **Legacy Postgres cleanup** (gap #5).
   - (A) Remove via Dokploy API `postgres.remove` después de verificar que DB vacía. Default si explorer 2 confirma vacío.
   - (B) Mantener como fallback warm — costo disco ~100MB, vale mantenerlo? NO recomendado (confusion risk).
   Default (A).

4. **Client credentials grant fix** (gap #4).
   - (A) Actualizar cada API client para habilitar `projectRoleCheck=false` + `hasProjectCheck=false` en el project. Requiere PATCH `/management/v1/projects/{id}` + PATCH `/management/v1/projects/{id}/apps/{appId}/api_config`.
   - (B) Dejar client_credentials deshabilitado por ahora, Wave B usa solo JWT validation con el web PKCE client.
   Default (A) para hardening — facilita observability backend y jobs CI.

5. **MFA admin enrollment** (gap #3) — requiere login UI funcionando.
   Depende de decisión #1. Si (A) lockeada, planificar enrollment TOTP en el flow del paso 5. Si (B), diferir a Wave B.

6. **DKIM/SPF validación** (gap #7).
   - User pega headers del test mail recibido (de T2.2 sessión anterior a `paz.fgabriel@`) — validación manual.
   - Si DKIM fail: investigar con Explorer 5 (registrar DNS, CloudFlare records).
   - Si no tiene el mail en inbox, re-trigger: `POST /admin/v1/smtp/369203306586702095/_test` con `receiverAddress: paz.fgabriel@gmail.com`.

## Paso 3 — Planning (obligatorio por ser Large/Risky)

`$writing-plans` persistido a `.docs/raw/plans/2026-04-19-cerrar-gaps-wave-a-zitadel.md`. El plan debe:

- Descomponer los 7 gaps en tasks atómicos dispatchables (típicamente G1..G7 + closure).
- Sub-wave structure:
  - **G0 — Pre-work:** actualizar secrets Infisical con decisiones lockeadas (ej. `ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE`).
  - **G1 — Deploy login companion** (si decisión #1 = A): crear 2do App Dokploy, env vars, Traefik path routing, smoke test `/ui/v2/login/login?...` devuelve 200, commit doc update.
  - **G2 — Backup cron install:** via `$ssh-remote` al VPS turismo, crear `/opt/zitadel-backup/`, copiar `snapshot.sh`, config `.env`, `rclone config` (interactive con user si hace falta), cron `/etc/cron.d/zitadel-backup`, ejecutar 1er backup manual, verificar offsite file listable.
  - **G3 — MFA admin enrollment:** requiere G1 done. Login browser → `paz.fgabriel@gmail.com` → Authenticators → Add Authenticator App → TOTP QR scan → persistir recovery codes en Infisical `ZITADEL_ADMIN_TOTP_RECOVERY`.
  - **G4 — Client credentials grant fix:** PATCH API clients para project auth config, re-run smoke test `POST /oauth/v2/token` con client_credentials grant, token JWT válido con `kid` + `alg=RS256`.
  - **G5 — Legacy Postgres cleanup:** via `$dokploy-cli` `postgres.remove` con postgresId `5e5bKMG5jBS30S7d8nn6c`. Validar que Zitadel sigue responding OIDC. Update `CT-AUTH-ZITADEL.md` sección 1 y memoria.
  - **G6 — Offsite remote configured:** post G2, validar que cron corrió una vez (24h wait o forzar manual), verificar archivo en offsite remote, mkey set de `ZITADEL_BACKUP_OFFSITE_RCLONE_REMOTE` si no estaba ya.
  - **G7 — DKIM/SPF verified:** user pega headers, si pass → commit evidence; si fail → sub-issue + explorer 5.
  - **G8 — Closure:** smoke test consolidado, update CT-AUTH-ZITADEL.md sección 11 (tachar cada gap cerrado), update memoria, commit PR, comentar en issue #17 "gaps cerrados".
- Por task marcar: subagent (`$ps-worker`, `$ps-docs`), archivos involucrados, criterios de aceptación, drift risk, rollback explícito.

## Paso 4 — Ejecución

La mayoría de los gaps son infra manual (Dokploy API + SSH VPS) + documentación. Aprovechá paralelismo donde puedas:

- `$ps-worker`: Dokploy API calls, SSH commands, env updates, mkey set.
- `$ps-docs`: updates a `CT-AUTH-ZITADEL.md` sección 11, runbooks, memoria.

**Coordinación obligatoria con user en tiempo real para:**
- Generar TOTP admin en browser (tu no podés — requiere QR scan con phone authenticator).
- Confirmar headers del SMTP test mail (inbox `paz.fgabriel@gmail.com`).
- Decidir offsite provider + pegar API keys B2/R2/Hetzner.
- Approve destructive actions: remove legacy Postgres.

Después de cada sub-wave: `$ps-trazabilidad` — chequeo incremental. Cross-check evidencia: command output + exit code + masked secret si aplica.

Archivos permitidos:
- `.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md` (update sección 11 Gaps por cada cierre).
- `infra/runbooks/zitadel-{bootstrap,recovery,backup}.md` (actualizar Known gaps cuando apliquen).
- `infra/dokploy/zitadel/compose.yml` (si se agrega login companion).
- `infra/dokploy/zitadel/traefik.labels.md` (si se agrega path routing).
- `infra/dokploy/zitadel-login/` (nueva carpeta con compose reference para el companion).
- `infra/.env.template` (agregar keys nuevas del companion + backup remote).
- `artifacts/e2e/2026-04-19-cerrar-gaps-wave-a/` (smoke evidence por gap).
- `.docs/raw/plans/2026-04-19-cerrar-gaps-wave-a-zitadel.md` + subdocs.
- `.docs/raw/reports/2026-04-19-cerrar-gaps/*.md`.
- Memoria `project_idp_zitadel_multi_ecosistema.md`.

Archivos **prohibidos** sin aprobación explícita:
- `src/Bitacora.Api/**` — Wave B, no ahora.
- `frontend/**` — Wave B, no ahora.
- Supabase Auth stack (`auth.bitacora.nuestrascuentitas.com`) — sigue corriendo intacto.
- `infra/secrets.enc.env` sin haber corrido `mkey backup bitacora` antes.
- Regenerar masterkey Zitadel, rotar admin PAT sin razón documentada.

## Paso 5 — Closure

1. **Smoke test consolidado** en `artifacts/e2e/2026-04-19-cerrar-gaps-wave-a/README.md`:
   - OIDC discovery 200 (sin cambios desde Wave A, solo re-validar).
   - Login UI v2 accesible (GREEN si decisión A; AMBER documentado si B).
   - Client credentials grant devuelve JWT RS256 válido.
   - MFA TOTP enrolled on admin (evidencia: `GET /management/v1/users/{id}/auth-factors/_search` retorna `otp`).
   - Backup cron corrió al menos 1 vez, archivo en offsite listable vía rclone.
   - DKIM+SPF pass headers embebidos (masked).
   - Legacy Postgres removida (evidencia: `dkp.sh GET project.one` sin la 2da postgres).
2. `$ps-trazabilidad` — closure final.
3. `$ps-auditar-trazabilidad` — obligatorio por ser multi-module. Cross-doc consistency check arquitectura ↔ baseline ↔ CT-AUTH-ZITADEL ↔ runbooks ↔ memoria ↔ smoke.
4. **Update memoria** `project_idp_zitadel_multi_ecosistema.md` sección "Wave A — completada PARTIAL 2026-04-19": cambiar a "completada 2026-04-19 (gaps cerrados 2026-04-1X)". Tachar cada gap resuelto con evidencia.
5. **Comentar en epic #15** actualizando estado: Wave A completada GREEN (antes PARTIAL).
6. **Comentar en issue #17 (Wave B)** que los gaps de Wave A están cerrados y Wave B puede arrancar sin caveats.
7. **NO reabrir #16.** Los gaps cerrados se documentan en este ciclo y se linkean desde PR o comentarios.

# Boundaries (explicit)

- **NO apagar Supabase Auth.**
- **NO tocar** `src/Bitacora.*` ni `frontend/`.
- **NO reutilizar** la Postgres de Bitácora para nada de Zitadel.
- **NO usar `:latest`** en tags docker — pinnear versión companion a `v4.9.0`.
- **NO commitear** secrets en plaintext. Todo via `mkey set`.
- **NO exponer** port Postgres Zitadel al exterior.
- **NO habilitar** signup público Teslita todavía.
- **NO crear** Zitadel Búho (`id.buho.ar`) — Wave D en otro repo.
- **NO regenerar** masterkey Zitadel ni rotar admin PAT sin razón documentada.

# Severity / drift policy

- Si `curl /openid-configuration` falla a mitad de los fixes: **pausar** y diagnosticar. Posible causa: el deploy del companion rompió Traefik routing del core.
- Si backup cron install falla porque rclone config remote es inválido: no marcar G2 GREEN hasta resolver. IdP sin backup >72h es riesgo.
- Si MFA enrollment falla porque login UI tira error: esperar hasta G1 GREEN. MFA es crítico pero no bloquea backups/secrets/cleanups.
- Si legacy Postgres tiene datos inesperados (instance.added events con > 0): NO remove, escalá explorer 4.
- Si DKIM/SPF fallan post investigación: dejar G7 AMBER, abrir sub-issue, no bloquear cierre de los otros 6 gaps.
- Si la decisión (A) de login companion se elige pero el companion v4.9.0 no arranca por bug: documentar sub-issue, escalar a decisión (B) diferida.

# Entregables esperados (checklist)

- [ ] `$ps-contexto` ejecutado.
- [ ] ≥3 exploradores dispatched paralelo (Zitadel state, Postgres legacy, Dokploy capacity).
- [ ] `$brainstorming` con 6 decisiones tácticas lockeadas.
- [ ] Plan persistido `.docs/raw/plans/2026-04-19-cerrar-gaps-wave-a-zitadel.md`.
- [ ] G1: login companion deployed (o decisión B documentada).
- [ ] G2: backup cron instalado, 1er backup verificado offsite.
- [ ] G3: MFA TOTP enrolled admin, recovery codes en Infisical.
- [ ] G4: client_credentials grant funcional con JWT RS256.
- [ ] G5: legacy Postgres removida.
- [ ] G6: offsite remote configurado + archivo listable.
- [ ] G7: DKIM/SPF verified o sub-issue abierto.
- [ ] Smoke test `artifacts/e2e/2026-04-19-cerrar-gaps-wave-a/` firmado.
- [ ] `CT-AUTH-ZITADEL.md` sección 11 actualizada (gaps tachados).
- [ ] Memoria `project_idp_zitadel_multi_ecosistema.md` updated.
- [ ] `$ps-trazabilidad` + `$ps-auditar-trazabilidad` limpios.
- [ ] Epic #15 comentado con Wave A GREEN.
- [ ] Issue #17 comentado (caveat Wave B removido).

# Referencias rápidas

| Item | Valor |
|------|-------|
| Repo | `C:\repos\mios\humor` (fgpaz/bitacora) |
| Branch | `main` |
| Commit base | `61f17b6 feat(wave-a): Zitadel IdP Teslita stood-up` |
| VPS | `turismo` (`54.37.157.93`) — SSH via `$ssh-remote` host `turismo` |
| Dokploy project | `teslita-shared-idp` (`AKHsAJScexTwhJBzfFRlk`) |
| Zitadel app | `zFdEECmPr1hhxwL0DKu4B` (`app-program-optical-matrix-xmjswe`) |
| Postgres activa | `BjjSOBWAwe6XpGttK4XoY` (`postgres-reboot-wireless-panel-chhbwg`) |
| Postgres legacy (a remover) | `5e5bKMG5jBS30S7d8nn6c` (`postgres-bypass-wireless-bus-tupzoj`) |
| Admin PAT key en Infisical | `ZITADEL_ADMIN_PAT` |
| Login Client PAT key | `ZITADEL_LOGIN_CLIENT_PAT` |
| SMTP config id | `369203306586702095` |
| OIDC discovery | `https://id.nuestrascuentitas.com/.well-known/openid-configuration` |
| Epic | https://github.com/fgpaz/bitacora/issues/15 |
| Wave A (cerrado) | https://github.com/fgpaz/bitacora/issues/16 |
| Wave B (bloqueado) | https://github.com/fgpaz/bitacora/issues/17 |
| Contrato | `humor:.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md` |
| Smoke Wave A | `humor:artifacts/e2e/2026-04-19-zitadel-wave-a-smoke/README.md` |
| Estimación total | 3-5 horas (G2 + G3 + G4 blockean, resto paralelo) |
