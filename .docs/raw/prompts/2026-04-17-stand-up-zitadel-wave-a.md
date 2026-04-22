<!--
target_platform: claude-code
pressure: aggressive
generated_at: 2026-04-17
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-17-stand-up-zitadel-wave-a.md' | Set-Clipboard"
-->

# Misión

Estás en `C:\repos\mios\humor` (proyecto Bitácora — clinical mood tracker, .NET 10 + Next.js 16 + Telegram Bot + PostgreSQL, deploy Dokploy VPS 54.37.157.93 "turismo").

Tu misión es **ejecutar Wave A del Epic de migración a Zitadel Teslita IdP**: levantar la instancia Zitadel self-hosted en `id.nuestrascuentitas.com`, provisionar su Postgres dedicada, configurar TLS + SMTP, crear las Organizations del ecosistema Teslita (`nuestrascuentitas`, `bitacora`, `multi-tedi`, `gastos`), crear el primer admin con MFA, correr un smoke test firmado y dejar todos los secrets en Infisical via `mi-key-cli`. **No tocás código de Bitácora en esta Wave.** El refactor del backend y frontend vive en Wave B (issue #17), que está **bloqueado** por este task.

Referencias canónicas obligatorias antes de decidir nada:

- **Epic tracking:** https://github.com/fgpaz/bitacora/issues/15
- **Wave A tracking (esta misión):** https://github.com/fgpaz/bitacora/issues/16
- **Wave B (bloqueada por esta):** https://github.com/fgpaz/bitacora/issues/17
- **Design doc canónico:** `.docs/raw/plans/2026-04-17-idp-zitadel-multi-ecosistema-design.md` — sección "Wave A".
- **Memoria cross-proyecto:** `C:\Users\fgpaz\.claude\projects\C--repos-mios-humor\memory\project_idp_zitadel_multi_ecosistema.md`
- **Mapa de contenedores Dokploy actual:** `C:\Users\fgpaz\.claude\projects\C--repos-mios-humor\memory\project_containers_map.md` (validar antes de usar — es un snapshot).
- **Política de secrets (Infisical obligatorio):** `C:\Users\fgpaz\.claude\projects\C--repos-mios-humor\memory\feedback_env_vars_infisical.md`
- **Workaround mkey strict mode:** `C:\Users\fgpaz\.claude\projects\C--repos-mios-humor\memory\feedback_mkey_lastexitcode.md`
- **Docs oficiales Zitadel:** https://zitadel.com/docs (consultar via `Skill("ceo-tools-backend")` o `plugin:context7:context7` si hace falta).

# Verificación previa (obligatoria)

Antes de tocar nada:

1. `git log -1 --format="%H %s"` — commit base debería ser post `02e724a` (fix H-001 landing).
2. `git status --short` — working tree limpio o con docs untracked (`.docs/raw/plans/`, `prompts/`, `reports/`). Si hay WIP de código, reportá y escalá.
3. `gh issue view 16 --repo fgpaz/bitacora --json state,body` — leé los pasos y criterios de aceptación. Si ya está CLOSED, **abortá**: Wave A estaba hecha y el prompt está viejo.
4. `gh issue view 15 --repo fgpaz/bitacora --json body` — epic paraguas, entender contexto multi-ecosistema.
5. Confirmá pre-requisitos de infra y DNS:
   - `dig +short id.nuestrascuentitas.com A` (o `nslookup id.nuestrascuentitas.com 8.8.8.8` si `dig` no está en Windows bash) — debería resolver a `54.37.157.93`. Si no resuelve, **paralizá deploy y pedile al usuario que configure el record** antes de continuar (no podés seguir sin DNS; TLS Let's Encrypt requiere HTTP-01 challenge).
   - **Cloudflare proxy check (bloqueante — detectado 2026-04-17):** si la IP resuelta cae en rangos CF (`104.16.0.0/13`, `172.64.0.0/13`, IPv6 `2606:4700::/32`), el record está con proxy "orange cloud" activado. La verificación del 2026-04-17 mostró `id.nuestrascuentitas.com` resolviendo a `104.21.82.159` / `172.67.203.132` — **proxied**. Esto rompe HTTP-01 de Let's Encrypt porque CF intercepta el challenge. Tres estrategias:
     - (A) **RECOMENDADA:** pasar el record a "DNS only" (nube gris) en Cloudflare. Dokploy/Traefik emite cert Let's Encrypt normalmente. Pros: setup estándar, un solo cert a manejar. Cons: IP del VPS queda expuesta (no hay DDoS protection CF).
     - (B) mantener proxy CF + modo "Full (strict)": emitir cert propio en CF edge y cert Let's Encrypt en Traefik. Pros: DDoS protection CF. Cons: 2 certs, ambos deben renovarse.
     - (C) mantener proxy CF + usar DNS-01 challenge con API token CF en Traefik (config `cloudflareDnsChallenge`). Pros: DDoS protection + single cert. Cons: config Traefik más compleja, API token CF con permisos Zone:DNS:Edit.
     Decidir (default A) y confirmar con el usuario antes de deployar Zitadel. Si se elige A, el usuario debe pasar el record a gris en el dashboard CF (o vía API) antes de arrancar A3 del plan.
   - `curl -sS -o /dev/null -w "status=%{http_code}\n" https://id.nuestrascuentitas.com/.well-known/openid-configuration` — **debería dar 404** (si devuelve 200, Zitadel ya está deployado y Wave A está hecha — abortá y cerrá el issue).
   - `curl -sS -o /dev/null -w "status=%{http_code}\n" http://54.37.157.93/` — VPS debe responder.
6. Confirmá capacidad del VPS turismo:
   - SSH (o Dokploy UI) → `free -h` y `df -h`. Zitadel necesita ~512MB RAM y ~2GB disco para la instancia + Postgres. Si el VPS está apretado (<1GB RAM libre), **escalá al usuario** antes de seguir.
7. Leé el design doc (`.docs/raw/plans/2026-04-17-idp-zitadel-multi-ecosistema-design.md`) sección "Wave A" + "Politicas diferenciadas por Org". El resto del prompt asume que lo leíste.

# Decisiones lockeadas (NO reabrir)

1. **Stack IdP:** Zitadel self-hosted (Go + Postgres, Apache 2.0). No se evalúan alternativas (WorkOS, Keycloak, Auth0, Logto, Clerk — todas descartadas en brainstorming del 2026-04-17).
2. **Dominio:** `id.nuestrascuentitas.com`. DNS apunta al VPS turismo (`54.37.157.93`).
3. **Deploy platform:** Dokploy (docker-compose managed). Mismo VPS que Bitacora y otras apps Teslita.
4. **Postgres:** **dedicada** para Zitadel — NO reutilizar la Postgres de Bitacora (`postgres-reboot-solid-state-application-l55mww`). Riesgo de blast radius.
5. **TLS:** autoemitido por Dokploy/Traefik via Let's Encrypt. No certs manuales.
6. **Versión Zitadel:** última estable v2.x al momento del deploy (pinneada en docker-compose — no `:latest`).
7. **Organizations iniciales:** `nuestrascuentitas`, `bitacora`, `multi-tedi`, `gastos`. Se crean en Wave A; los OAuth Clients específicos se crean al arrancar cada migración (Wave B para Bitacora, Wave C' para gastos, etc.).
8. **Policies baseline Teslita:** MFA opcional, session 1h access / 30d refresh, password 10 chars min, audit retention 2 años. (Las policies hardened de buho/salud viven en otra instancia — Wave D, fuera de scope aquí).
9. **Secrets:** todos en Infisical via `mi-key-cli`. Nunca solo en `infra/.env`.
10. **Ecosistemas separados:** esta Wave levanta SOLO el Zitadel Teslita. El Zitadel Búho (`id.buho.ar`) es Wave D en otro repo.

Si durante exploración aparece una razón fundada para reabrir alguna decisión, detente y escalá con `Skill("brainstorming")`. No actúes.

# Workflow obligatorio (SDD, Large/Risky route)

Esta wave es infra-heavy y riesgosa: deploy nuevo servicio crítico (auth), DNS público, TLS, SMTP con DKIM/SPF, credenciales admin que cuando se pierden se pierde todo. Ruta completa SDD.

## Paso 1 — Contexto + exploración

1. `Skill("ps-contexto")` — bloqueante. Aunque el cambio es infra y no toca `src/`, igual cargá scope, arquitectura y baseline técnica porque el wiki va a actualizarse en el Paso 5.
2. `mi-lsp workspace status bitacora --format toon` — validar workspace. Si devuelve `hint` o `next_hint`, seguí esa guía. Mínimo uso en esta wave (poco código propio), pero la regla del CLAUDE.md aplica igual.
3. **Dispatch EN PARALELO en UN solo mensaje ≥4 subagents.** Objetivos obligatorios:

   - **Explorer 1 — Infra actual Dokploy:** subagent `ps-explorer`.
     - SSH al VPS turismo o API Dokploy: listar apps, compose stacks, Traefik routers.
     - Verificar puertos ocupados (8080, 443, 5432).
     - Mapear qué subdominios ya están configurados en Dokploy (`auth.bitacora.nuestrascuentitas.com`, `bitacora.nuestrascuentitas.com`, etc.) para evitar conflicto.
     - Reportar: nombre del app/compose donde se va a agregar Zitadel, si hay un reverse proxy compartido o uno nuevo, capacidad disponible (RAM, disco, vCPU).
     - Cross-check con `project_containers_map.md` (memoria) — esa memoria tiene 3 días, **verificá contra estado real**.

   - **Explorer 2 — DNS + TLS state:** subagent `ps-explorer` o `ps-worker`.
     - `dig +short id.nuestrascuentitas.com A`, `dig +short id.nuestrascuentitas.com AAAA`, `dig +short id.nuestrascuentitas.com CAA`.
     - Si hay CAA, validar que permite `letsencrypt.org`.
     - `whois nuestrascuentitas.com` para identificar registrar (por si hay que escalar cambios DNS al usuario).
     - Si existe un cert wildcard `*.nuestrascuentitas.com` en Dokploy, reportalo — cambia el flujo TLS (reutilizar vs autoemitir).
     - Reportar: lista de changes DNS requeridos si falta el A/AAAA de `id.nuestrascuentitas.com`.

   - **Explorer 3 — SMTP provider candidate:** subagent `ps-explorer` + `ceo-tools-backend` skill si hace falta.
     - Evaluar Resend vs AWS SES vs Postmark para envío transaccional desde Zitadel (signup, password reset, MFA fallback).
     - Criterios: free tier (Zitadel necesita ~50-200 mails/mes para ecosistema Teslita inicial), soporte DKIM/SPF automáticos, compliance AR (idealmente server argentino o con DPA firmable).
     - Reportar: recomendación (default Resend del design doc), credenciales necesarias, pasos para validar DKIM + SPF + DMARC del dominio `nuestrascuentitas.com`.

   - **Explorer 4 — Backup & disaster recovery strategy:** subagent `ps-explorer` o `ceo-tools-databases`.
     - Zitadel guarda **todos** los users + sessions + audit en su Postgres. Perderla es catastrófico (todos los productos Teslita pierden auth).
     - Proponer: frecuencia de backup (sugerido diario + WAL continuo), destino (S3-compatible: Backblaze B2, R2, o incluso local + rsync al desktop del usuario), retention (30 días mín), restore test procedure.
     - Reportar: propuesta concreta con costo estimado y quién la ejecuta (cron en Dokploy vs job externo).

4. **Explorer 5 (condicional)** si aparece alguno de estos signals en 1-4:
   - El VPS turismo está cerca de saturación (→ evaluar segundo VPS chico para Zitadel).
   - Existe un certificado wildcard reutilizable (→ cambia config Traefik).
   - El registrar DNS tiene restricciones raras (CloudFlare proxy habilitado, etc.) que pueden romper HTTP-01 challenge.
   - El usuario ya tiene cuenta activa en uno de los SMTP candidatos (→ reutilizar credenciales vs crear nuevo).

5. Toda cita `file:line` o config path debe estar verificada con `mi-lsp`, `Grep` o lectura directa. Nada inventado.

## Paso 2 — Brainstorming (obligatorio)

`Skill("brainstorming")` para lockear las decisiones tácticas que aún quedan abiertas. Protocolo obligatorio: learning context → applied impact → ASCII diagram → pros/cons → recomendación → **una** pregunta con `AskUserQuestion`. Decisiones a lockear mínimas:

1. **SMTP provider final:** **LOCKEADO 2026-04-17 — Resend.** Free tier 3k/mes cubre 30x volumen Fase 1, DKIM con 1 TXT record, DPA + SOC2 Type 2 firmables, escape hatch a AWS SES `sa-east-1` documentado (~2h migration window). **No reabrir en brainstorming — saltar a decisión 2.**

2. **Backup strategy Postgres Zitadel:**
   - (A) `pg_dump` diario → Backblaze B2 (~$0.006/GB/mes, retention 30d manual).
   - (B) `pg_dump` + WAL streaming a S3 (PITR capability, mayor complejidad ops).
   - (C) Snapshot diario del volumen Docker + offsite (más simple, menos granular).
   - Default: (A) para Fase 1. (B) si el usuario quiere PITR desde el día 1.

3. **Email del primer admin Zitadel:**
   - (A) `buhosoft70@gmail.com` (cuenta actual del usuario).
   - (B) crear `admin@nuestrascuentitas.com` dedicado (requiere mailbox — Zoho free o Fastmail).
   - (C) usar email de Infisical service account.
   - Default: (A) para arrancar; migrar a (B) cuando el dominio tenga mailbox. Documentar en el smoke que la cuenta es personal temporalmente.

4. **Policy de MFA en baseline Teslita:**
   - Design doc dice "MFA opcional (recomendada para profesionales)". ¿Forzada para admins desde el día 1 o solo recomendada?
   - Default recomendada: **MFA forzada para usuarios con rol `admin` / `superadmin` en Zitadel desde el día 1**. End users opcional.

5. **Creación de los OAuth Clients por app:**
   - ¿Los 4 clients (`bitacora`, `nuestrascuentitas`, `multi-tedi`, `gastos`) se crean en Wave A para dejar todo listo, o se crean bajo demanda en cada Wave de migración?
   - Default: **crear los 4 en Wave A** con redirect URIs placeholder y documentar que cada Wave de migración los actualiza. Permite que el smoke test incluya creación de client real.

6. **Tipo de VPS:**
   - ¿Zitadel comparte el VPS turismo (54.37.157.93) o necesita uno dedicado?
   - Default: **mismo VPS** si el Explorer 1 confirma ≥1GB RAM libre y ≥5GB disco. Separar si está ajustado.

## Paso 3 — Writing-plans (obligatorio por ser Large/Risky)

`Skill("writing-plans")` con persistencia a `.docs/raw/plans/2026-04-17-wave-a-stand-up-zitadel-teslita.md`. El plan debe:

- Descomponer Wave A en 6-8 tasks atómicas dispatchables a subagents donde sea seguro (la mayor parte es infra secuencial manual, pero los pasos de docs/wiki/Infisical pueden paralelizarse).
- Sub-waves sugeridas:
  - **A1 — Pre-deploy:** DNS records, SMTP account, backup destination provisionado.
  - **A2 — Postgres dedicada:** crear servicio en Dokploy con volume persistente, credenciales en Infisical.
  - **A3 — Zitadel deploy:** docker-compose pinneado, variables desde Infisical, masterkey generado, Traefik router configurado.
  - **A4 — First boot + admin bootstrap:** consola admin Zitadel, MFA setup, reset inicial de password del admin-autogenerated.
  - **A5 — Organizations + policies:** crear las 4 orgs, aplicar policies baseline, crear 4 OAuth Clients con placeholders.
  - **A6 — SMTP provider wiring:** conectar Resend (o el elegido), DKIM + SPF, test mail.
  - **A7 — Backup automation:** cron o job externo que respalda Postgres y copia offsite.
  - **A8 — Smoke test firmado:** login admin, crear user test, revocar user, export audit log, guardar evidencia en `artifacts/e2e/2026-04-17-zitadel-wave-a-smoke/`.
  - **A9 — Wiki + memoria:** actualizar `.docs/wiki/02_arquitectura.md`, `07_baseline_tecnica.md`, `09_contratos/CT-AUTH.md` con el nuevo IdP. Actualizar `project_idp_zitadel_multi_ecosistema.md` con endpoints reales (issuer, jwks_uri, token_endpoint).
- Por task marcar: subagent recomendado (`ps-worker`, `ps-docs`, `ps-explorer`), archivos involucrados, criterios de aceptación, drift risk, rollback explícito.

## Paso 4 — Creación de tarjetas hijas (opcional si el plan lo justifica)

`Skill("pj-crear-tarjeta")` para cada sub-wave mayor que merezca tracking en GitHub Issues. Project board Bitácora: `fgpaz/Bitacora #4`. Prefijo `[MIGR-IDP Wave A]`. Cada tarjeta:

- Referencia el parent epic #15 y este issue #16.
- Criterios de aceptación atómicos.
- Marca bloqueos cruzados (ej. A4 bloquea A5 bloquea A8).

Para Wave A, las sub-tareas son típicamente pocas (6-8) y tan secuenciales que pueden vivir dentro de #16 como checklist en el body. **Crear tarjetas hijas solo si el usuario prefiere granularidad o si alguna sub-wave es delegable a otra persona.**

## Paso 5 — Ejecución

La mayoría de esta wave es **infra manual via Dokploy UI + SSH + mi-key-cli**. No hay masivo dispatch de subagents de código. Aun así, aprovechá paralelismo donde se pueda:

- `ps-worker`: scripts de automatización (docker-compose, SMTP test, backup cron).
- `ps-docs`: updates de wiki en paralelo mientras corre el deploy.
- `ps-explorer`: re-verificar endpoints post-deploy.

**Coordinación obligatoria con el usuario en tiempo real para:**
- Cambios DNS que requieren login al registrar (el usuario los hace, vos reportás el registro exacto requerido).
- Crear cuenta SMTP y entregar API key (el usuario la genera, vos la guardás via `mkey set SMTP_RESEND_API_KEY ***`).
- Aprobación del masterkey de Zitadel una vez que se genere (perder el masterkey = perder capacidad de migrar/rotar).

Después de cada sub-wave:
- `Skill("ps-trazabilidad")` — chequeo incremental (sobre todo en A9 cuando tocamos wiki).
- Cross-check de evidencia: comando de validación corrido + output (masked si trae secrets).

Archivos permitidos de tocar:
- `infra/dokploy/zitadel/*` (docker-compose, env.example, traefik labels) — **crear** esta carpeta.
- `infra/runbooks/zitadel-*` (bootstrap, recovery, backup) — **crear**.
- `infra/backups/zitadel/*` (scripts de backup, no keyfiles).
- `.docs/wiki/02_arquitectura.md`, `.docs/wiki/07_baseline_tecnica.md`, `.docs/wiki/09_contratos/CT-AUTH.md`.
- `.docs/wiki/09_contratos_tecnicos.md` (índice, si agregás CT-AUTH nuevo).
- `artifacts/e2e/2026-04-17-zitadel-wave-a-smoke/` (evidencia del smoke — screenshots, curl logs, JWT decodeado con `sub` masked).
- `C:\Users\fgpaz\.claude\projects\C--repos-mios-humor\memory\project_idp_zitadel_multi_ecosistema.md` (actualizar con endpoints reales).

Archivos **prohibidos** en esta wave (sin aprobación previa):
- `src/Bitacora.Api/**` — Wave B, no ahora.
- `frontend/**` — Wave B, no ahora.
- Supabase Auth stack actual (`infra/dokploy/supabase-auth/*` o equivalente) — **no lo apagás en Wave A**. Sigue corriendo hasta Wave B.
- Código de dominio (`src/Bitacora.Domain/`).

## Paso 6 — Closure

1. **Smoke test firmado** en `artifacts/e2e/2026-04-17-zitadel-wave-a-smoke/README.md` con:
   - URL `https://id.nuestrascuentitas.com/.well-known/openid-configuration` devolviendo 200 + JSON con `issuer`, `jwks_uri`, `authorization_endpoint`, `token_endpoint`.
   - Screenshot del admin UI con la Organization `bitacora` creada.
   - Evidencia de test mail entregado con DKIM + SPF pass (headers masked).
   - Output de `curl` al token endpoint con un client credentials grant de un OAuth Client test (JWT masked, solo mostrar `kid`, `iss`, `aud`, firma válida).
   - Evidencia del backup: primer `pg_dump` completado + listado S3/B2 con el file.
2. `Skill("ps-trazabilidad")` — closure final. Verificar wiki actualizado.
3. `Skill("ps-auditar-trazabilidad")` — obligatorio. Cross-doc consistency check (arquitectura ↔ baseline ↔ CT-AUTH ↔ memoria).
4. **Actualizar memoria** `project_idp_zitadel_multi_ecosistema.md` reemplazando "pendiente" por los endpoints reales y el status "Wave A: completada 2026-04-XX".
5. **Cerrar issue #16** con comentario:
   - Link al smoke evidence.
   - Endpoints operacionales (issuer, jwks_uri).
   - Lista de secrets guardados en Infisical (solo nombres, no valores).
   - Mención explícita: "Wave B (#17) desbloqueada".
6. **Comentar en issue #17** (Wave B) que está desbloqueada y que el prompt paste-ready está en `.docs/raw/prompts/2026-04-17-migracion-bitacora-zitadel-wave-b.md`.
7. **Comentar en epic #15** actualizando el estado: Wave A ✅.

# Boundaries (explicit)

- **NO apagar Supabase Auth** bajo ninguna circunstancia en esta wave. Sigue corriendo intacto.
- **NO tocar** `src/Bitacora.*` ni `frontend/`.
- **NO reutilizar** la Postgres de Bitacora para Zitadel (riesgo blast radius).
- **NO usar `:latest`** en tags docker — pinnear versión Zitadel y Postgres.
- **NO commitear** masterkey, DB password, SMTP API key, session secrets. Todo va a Infisical via `mkey set`. Recordá inicializar `$LASTEXITCODE = 0` en sesión pwsh fresca (ver memoria `feedback_mkey_lastexitcode.md`).
- **NO exponer** el port Postgres Zitadel al exterior. Solo accesible desde el network Docker interno.
- **NO habilitar** signup público en Zitadel para Teslita en esta wave. Los users se crean on-demand por Wave B/C/etc.
- **NO crear** el Zitadel de Búho (`id.buho.ar`) acá — esa es Wave D en otro repo.
- **NO emitir** certificados TLS manuales. Dokploy/Traefik + Let's Encrypt, autorenovación.
- **NO cerrar issue #16** hasta que el smoke completo esté firmado y los entregables del checklist estén tildados.

# Severity / drift policy

- Si `dig id.nuestrascuentitas.com` no resuelve al VPS: **pausá deploy**, entregale al usuario el record DNS exacto (A → `54.37.157.93`) y esperá confirmación.
- Si Let's Encrypt rate-limitea por intentos fallidos (más de 5 failed issuances por semana en el dominio): usá staging env de Let's Encrypt hasta resolver, y documentá la causa raíz antes de forzar prod cert.
- Si SMTP Resend (u otro) falla DKIM/SPF validation: **no cerrés Wave A**. Zitadel funciona sin SMTP pero emails de signup/reset quedan pending. Documentá en el smoke y abrí un sub-issue.
- Si durante el primer boot Zitadel consume >2GB RAM y tira OOM: **escalá al usuario** — probablemente el VPS turismo no alcanza y hay que migrar a VPS dedicado (o ajustar `ZITADEL_WEBSOCKET_CONNECTIONS`).
- Si el admin login falla post-deploy: **NO regenerés el masterkey** (destruye encryption de DB). Usá `zitadel setup` con el masterkey original para resetear admin password. Si el masterkey se perdió, es destroy-and-recreate — escalá.
- Si el backup al S3/B2 destino falla: no cerrés Wave A con tick verde. Un IdP sin backup es una bomba de tiempo.
- Si el smoke muestra que el JWT emitido usa algoritmo distinto a RS256: reportá — esto rompe Wave B.
- Si alguien pide "apagar Supabase mientras tanto porque duplicamos costos": **rechazá**. Wave B es la que apaga Supabase, no Wave A.

# Entregables esperados (checklist)

- [ ] `Skill("ps-contexto")` ejecutado.
- [ ] ≥4 exploradores dispatched en paralelo (infra, DNS/TLS, SMTP, backup). Reportes consolidados.
- [ ] `Skill("brainstorming")` con las 6 decisiones tácticas lockeadas via `AskUserQuestion`.
- [ ] Plan persistido en `.docs/raw/plans/2026-04-17-wave-a-stand-up-zitadel-teslita.md`.
- [ ] DNS `id.nuestrascuentitas.com` → `54.37.157.93` propagado (validado con `dig`).
- [ ] Postgres dedicada Zitadel corriendo en Dokploy con volume persistente, credenciales en Infisical.
- [ ] Zitadel v2.x deployado en `https://id.nuestrascuentitas.com` con TLS válido (Let's Encrypt).
- [ ] `curl https://id.nuestrascuentitas.com/.well-known/openid-configuration` devuelve 200 con JSON OIDC válido.
- [ ] Organizations creadas: `nuestrascuentitas`, `bitacora`, `multi-tedi`, `gastos`.
- [ ] Policies baseline aplicadas a las 4 Organizations.
- [ ] OAuth Clients creados (uno por app) con redirect URIs placeholder.
- [ ] Primer admin user creado con MFA forzada. Credenciales en Infisical.
- [ ] SMTP provider conectado (Resend u otro lockeado en brainstorming). Test mail enviado. DKIM + SPF pass.
- [ ] Backup job corriendo (`pg_dump` diario → S3/B2). Primer backup verificado.
- [ ] Smoke test firmado en `artifacts/e2e/2026-04-17-zitadel-wave-a-smoke/` con evidencia completa.
- [ ] Wiki actualizada: `02_arquitectura.md`, `07_baseline_tecnica.md`, `09_contratos/CT-AUTH.md`.
- [ ] Memoria `project_idp_zitadel_multi_ecosistema.md` actualizada con endpoints reales y status.
- [ ] Todos los secrets en Infisical via `mi-key-cli` (masterkey, DB creds, SMTP API key, admin password, backup S3 creds).
- [ ] `Skill("ps-trazabilidad")` + `Skill("ps-auditar-trazabilidad")` limpios sin gaps.
- [ ] Issue #16 CLOSED con evidencia.
- [ ] Issue #17 (Wave B) comenteado como "desbloqueado".
- [ ] Epic #15 actualizado con Wave A ✅.

# Referencias rápidas

| Item | Valor |
|------|-------|
| Repo | `C:\repos\mios\humor` (fgpaz/bitacora) |
| Branch | `main` |
| Project board | https://github.com/users/fgpaz/projects/4 |
| Epic parent | https://github.com/fgpaz/bitacora/issues/15 |
| Wave A (esta misión) | https://github.com/fgpaz/bitacora/issues/16 |
| Wave B (bloqueada por esta) | https://github.com/fgpaz/bitacora/issues/17 |
| VPS turismo | `54.37.157.93` |
| Dominio objetivo | `id.nuestrascuentitas.com` |
| OIDC discovery endpoint esperado | `https://id.nuestrascuentitas.com/.well-known/openid-configuration` |
| Zitadel version target | `v2.x` último estable (pinneado, no `:latest`) |
| SMTP provider default | Resend (lockeable en brainstorming) |
| Dokploy mgmt URL | (ver Infisical o runbook `infra/runbooks/dokploy-access.md` si existe) |
| Acceso Dokploy recomendado | skill `dokploy-cli` con API key del host `turismo` (en Infisical). Invocar con `Skill("dokploy-cli")`. |
| SMTP provider (lockeado 2026-04-17) | Resend (free 3k/mes, DKIM 1 TXT, DPA + SOC2) — escape hatch a AWS SES `sa-east-1` documentado |
| DNS state (verif. 2026-04-17) | record A existe pero **proxied por Cloudflare** — decidir estrategia TLS (default: pasar a "DNS only") antes de deploy |
| Design doc | `.docs/raw/plans/2026-04-17-idp-zitadel-multi-ecosistema-design.md` (sección "Wave A") |
| Fecha de lockeo arquitectónico | 2026-04-17 |
| Estimación | 1-2 días |
