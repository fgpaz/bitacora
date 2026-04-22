<!--
target_platform: claude-code
pressure: aggressive
generated_at: 2026-04-17
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-17-migracion-bitacora-zitadel-wave-b.md' | Set-Clipboard"
-->

# Misión

Estás en `C:\repos\mios\humor` (proyecto Bitácora — clinical mood tracker, .NET 10 + Next.js 16 + Telegram Bot + PostgreSQL, deploy Dokploy VPS 54.37.157.93).

Tu misión es **ejecutar Wave B del Epic de migración a Zitadel Teslita IdP**. Concretamente: refactorizar `Bitacora.Api` y `frontend/` para consumir el IdP Zitadel vía OIDC + PKCE + JWKS RS256, migrar usuarios existentes de Supabase Auth self-hosted a la Organization `bitacora` dentro de Zitadel, y apagar la instancia auth dedicada actual.

Referencias canónicas obligatorias de lectura antes de decidir nada:

- **Epic tracking:** https://github.com/fgpaz/bitacora/issues/15
- **Wave A tracking (prerequisito):** https://github.com/fgpaz/bitacora/issues/16
- **Wave B tracking (esta misión):** https://github.com/fgpaz/bitacora/issues/17
- **Design doc canónico:** `.docs/raw/plans/2026-04-17-idp-zitadel-multi-ecosistema-design.md`
- **Prompt hermano** (para discovery de multi-tedi y gastos): `.docs/raw/prompts/2026-04-17-migracion-idp-zitadel-teslita-a-multitedi-y-gastos.md`
- **Informe ecosistema hermano Búho:** `.docs/raw/reports/2026-04-17-informe-migracion-idp-zitadel-buho-salud.md`
- **Memoria cross-proyecto:** `C:\Users\fgpaz\.claude\projects\C--repos-mios-humor\memory\project_idp_zitadel_multi_ecosistema.md`

# Verificación previa (obligatoria)

Antes de ejecutar nada:

1. `git log -1 --format="%H %s"` — anota commit base. Debe ser post `02e724a` (fix H-001 landing).
2. `git status --short` — working tree debería estar limpio o con docs untracked solamente. Si hay WIP de código, reportalo y escalá.
3. `gh issue view 16 --repo fgpaz/bitacora` — **confirmar que Wave A está CERRADA** con smoke test firmado. Si Wave A está abierta, **aborta Wave B y redirige al usuario a cerrar Wave A primero**.
4. `gh issue view 17 --repo fgpaz/bitacora` — leé los pasos y criterios de aceptación de Wave B.
5. Confirmá endpoints operacionales de Zitadel Teslita:
   - `curl -sS https://id.nuestrascuentitas.com/.well-known/openid-configuration` debería devolver 200 con `issuer`, `jwks_uri`, `authorization_endpoint`, `token_endpoint`.
   - Si no resuelve, **aborta** y reportá: Wave A no está terminada de verdad.
6. Leé el design doc completo. El resto del prompt asume que lo leíste.

# Decisiones lockeadas (NO reabrir)

1. **Stack IdP:** Zitadel self-hosted. No evaluar alternativas.
2. **Instancia Teslita:** `id.nuestrascuentitas.com` (ya levantada en Wave A).
3. **Organization en Zitadel para Bitácora:** `bitacora` dentro de Zitadel #1.
4. **Protocolo:** OIDC + PKCE para SPA (Next.js 16), Bearer RS256 + JWKS para `Bitacora.Api` (.NET 10).
5. **Claim mapping:** `urn:zitadel:iam:org:project:role:{roleKey}` para roles de Zitadel → claim `role` local.
6. **SSO intra-Teslita:** un único `user_id` por persona en los 4 productos Teslita.
7. **Supabase Auth dedicado se apaga** al completar Wave B. `SUPABASE_JWT_SECRET` queda rotado y retirado.
8. **Fix H-002 (cookie sync) descartado:** los edits locales de la sesión 2026-04-17 ya fueron revertidos. El middleware nuevo lee cookie OIDC emitida por `oidc-client-ts`.
9. **Issues superseded por Wave B:** #2, #6, #14 — cerrar al merge con comentario de referencia.

Si durante la exploración aparece una razón fundada para reabrir alguna, detente y escalá con `Skill("brainstorming")` — no actúes.

# Workflow obligatorio (SDD, Large/Risky route)

Este es un task grande y riesgoso: cruza backend + frontend + DB + wiki + deploy + DNS. Ruta completa SDD.

## Paso 1 — Contexto + exploración

1. `Skill("ps-contexto")` — bloqueante. Carga scope + arquitectura + FL + RF + datos + baseline técnica. También carga hardened UX/UI si aplica.
2. `mi-lsp workspace status bitacora --format toon` — validar workspace. Si devuelve `hint` o `next_hint`, ejecutá esa guía antes de fallback.
3. **Dispatch EN PARALELO en UN solo mensaje al menos 3 `ps-explorer`** (subagent_type=`ps-explorer`). Objetivos obligatorios:

   - **Explorer 1 — Backend auth surface (.NET 10):** mapear `src/Bitacora.Api/` todo lo que toca JWT.
     - `Program.cs`: config `AddJwtBearer`, `Authority`, `SUPABASE_JWT_SECRET`, claim mapping.
     - Middlewares que leen `sub`, `email`, `role`, `user_metadata.role`.
     - `src/**/Auth/*` si existe.
     - `ConsentRequiredMiddleware` (qué claims asume).
     - Tests de auth (unit + integration).
     - Reportar `file:line` exacto, nombres de env vars, y el flujo actual de validación.
   - **Explorer 2 — Frontend auth surface (Next.js 16):** mapear `frontend/` todo lo que toca Supabase.
     - `frontend/lib/auth/*` (`client.ts`, `SessionContext.tsx`).
     - `frontend/middleware.ts` (lee cookie `sb-access-token`).
     - Imports de `@supabase/supabase-js` en toda la carpeta.
     - Flujos: `/onboarding`, `/dashboard`, `/profesional/*`, logout.
     - `frontend/package.json` — dependencias actuales.
     - Reportar `file:line` y flujo de creación/destrucción de sesión.
   - **Explorer 3 — DB + usuarios + migración:** mapear la surface de datos.
     - Tablas en PostgreSQL que referencian `supabase_user_id`, `User.user_id`, `sub`, email.
     - Si `Bitacora.Api` tiene su propia tabla `users` vs. usa directamente Supabase `auth.users`.
     - FKs desde dominio (MoodEntry, DailyCheckin, CareLink, etc.) hacia el user.
     - Seeders, fixtures, migraciones EF Core.
     - Contar cuántos usuarios hay hoy en Supabase Auth dedicado (query `auth.users` vía psql o gRPC).
     - Reportar el shape del mapping `supabase.auth.users → Bitacora.users` y dónde tendría que insertarse el nuevo `zitadel_user_id`.

4. **Explorer 4 y 5** son obligatorios si aparece cualquiera de estos signals durante 1-3:
   - El backend valida claims que Zitadel no emite por default (`pseudonym_id`, `patient_id` custom).
   - El frontend usa Supabase para más que auth (ej. realtime, storage, DB).
   - Hay sesiones activas de Telegram (`TelegramSession`) que referencian al user — se necesita explorer dedicado.
   - Los tests E2E (Playwright) tienen patrones de forja de JWT incompatibles con RS256.

   Si aplica, Explorer 4 = Telegram + sesiones cross-canal. Explorer 5 = E2E + smoke gates.

5. Toda cita `file:line` debe estar verificada con `mi-lsp` o `Grep`. Nada inventado.

## Paso 2 — Brainstorming (obligatorio)

`Skill("brainstorming")` para lockear las decisiones tácticas que aún quedan abiertas. Protocolo obligatorio: learning context → applied impact → ASCII diagram → pros/cons → recomendación → **una** pregunta con `AskUserQuestion`. Las decisiones a lockear mínimas:

1. **Estrategia de migración de usuarios:**
   - (A) **Bulk import**: script one-shot que lee `auth.users` y crea en Zitadel con `sub` mapping preservado. Rollback: volver a Supabase con backup.
   - (B) **Opt-in gradual (link-on-first-login)**: dejar Supabase funcionando, cada user que se loguee por primera vez en Zitadel hace un link automático. Riesgo: dual-auth durante transición.
   - Recomendada por default: (A) con feature flag + ventana de mantenimiento de ~1h.

2. **Nombre de cookie/storage de sesión post-OIDC:**
   - Mantener `sb-access-token` en el middleware (compat camouflage) o renombrar a `bitacora-session` o `zitadel-session`.
   - Impacto en tests E2E, Playwright, documentación.

3. **Claim `role`:** de dónde leerlo.
   - Zitadel emite `urn:zitadel:iam:org:project:role:{roleKey}`. Hay que mapear a `role: "patient" | "professional"`.
   - En el frontend el middleware actual decodea `user_metadata.role`. El claim mapping debe migrar sin romper el rol.

4. **Página `/ingresar` (issue #3):**
   - Usar el hosted UI de Zitadel (`authorization_endpoint` redirige y vuelve con code).
   - O mantener UI propia que llama a Zitadel via OIDC API.
   - Recomendada: hosted UI para Fase 1, UI propia en Fase 2 si justifica.

## Paso 3 — Writing-plans (obligatorio por ser Large/Risky)

`Skill("writing-plans")` con persistencia a `.docs/raw/plans/2026-04-17-wave-b-migracion-bitacora-zitadel.md`. El plan debe:

- Descomponer Wave B en 5-8 tasks atómicas dispatchables a subagents en paralelo donde sea seguro.
- Definir orden de waves dentro de Wave B (ej. B1 backend JWT validation, B2 frontend OIDC client, B3 DB migration script, B4 wiki update, B5 E2E + deploy, B6 Supabase shutdown).
- Marcar cada task con: subagent recomendado (`ps-dotnet10`, `ps-next-vercel`, `ps-worker`, `ps-docs`), archivos involucrados, criterios de aceptación, drift risk.
- Incluir plan de rollback explícito por task.

## Paso 4 — Creación de tarjetas hijas (si el plan lo justifica)

`Skill("pj-crear-tarjeta")` para cada sub-task mayor del plan que merezca tracking propio en GitHub Issues. El project board Bitácora es `fgpaz/Bitacora #4`. Usar el prefijo `[MIGR-IDP Wave B]` en el título. Cada tarjeta debe:

- Referenciar el parent epic #15 y issue Wave B #17.
- Incluir criterios de aceptación atómicos.
- Vincular archivos que se van a tocar con `file:line` verificado.
- Marcar bloqueos cruzados entre tarjetas (B2 bloquea a B5, etc.).

Mover las tarjetas nuevas a columna `Todo` o `Roadmap` según corresponda con sus dependencias.

## Paso 5 — Ejecución en waves

Dispatch de los subagents en paralelo donde el plan lo permite. Un solo mensaje por batch. Después de cada batch:

- `Skill("ps-trazabilidad")` — chequeo incremental para captar drift temprano.
- Cross-check de resultados: si 2 subagents se contradicen, lanzá un explorer extra para resolver antes de integrar.

Archivos permitidos de tocar:
- `src/Bitacora.Api/**` (backend auth)
- `frontend/lib/auth/**`, `frontend/middleware.ts`, `frontend/app/**` (frontend auth + UI)
- `frontend/package.json` (deps)
- `.docs/wiki/02_arquitectura.md`, `.docs/wiki/07_baseline_tecnica.md`, `.docs/wiki/09_contratos/CT-AUTH.md`
- `infra/runbooks/*` (actualizar production-bootstrap, secret-source)
- `infra/smoke/backend-smoke.ps1` (actualizar smoke con nuevo auth)
- Scripts de migración de users (ubicar en `infra/migrations/zitadel/`)

Archivos prohibidos sin aprobación previa:
- `Bitacora.Telegram/**` (si existe separado) — Telegram webhook/polling no depende del user auth directamente, pero si aparece relación, escalá.
- Código de dominio (`src/Bitacora.Domain/`) — la migración es de auth, no de modelos clínicos.

## Paso 6 — Closure

1. `Skill("ps-trazabilidad")` — closure final. Verificar sync FL/RF/TP con los cambios de auth.
2. `Skill("ps-auditar-trazabilidad")` — obligatorio por ser multi-módulo y multi-riesgo. Cross-document consistency check read-only.
3. Cerrar issues #2, #6, #14 con comentario "Closed by Wave B migration — superseded".
4. Cerrar #17 Wave B con evidencia (smoke test + E2E Playwright).
5. Si Wave B cierra el epic #15 (es decir, Waves C/C'/C'' no dependen de este repo), actualizar el epic con estado "bloqueado por trabajo en otros repos".

# Boundaries (explicit)

- **NO tocar** `src/Bitacora.Domain/*` — aggregates y entidades clínicas.
- **NO cambiar** contratos de error existentes (`CT-ERRORS.md`) — solo agregar nuevos para Zitadel específicos.
- **NO rotar secrets** sin feature flag activo en prod.
- **NO apagar Supabase Auth** hasta que el feature flag esté al 100% durante al menos 48h sin incidentes.
- **NO commitear JWTs plaintext** en evidencia. Mask con `***` antes de commit.

# Severity / drift policy

- Si Wave A resulta **no estar cerrada en realidad** (issue #16 abierto, smoke faltando, o `well-known/openid-configuration` no responde): **aborta Wave B** y redirigí al usuario a cerrar Wave A.
- Si `Bitacora.Telegram/*` depende del JWT validado por `Bitacora.Api` y el cambio de secret rompe el webhook: **escalá al usuario antes de pushear**.
- Si los explorers detectan que la estrategia de migración de usuarios (bulk vs opt-in) tiene un blocker no contemplado en el design doc, `Skill("brainstorming")` inmediato.
- Si el pentest informal post-migration muestra que el nuevo flujo OIDC tiene una regresión de seguridad vs. el Supabase actual: **aborta deploy**, rollback via feature flag, reportá.
- Si durante deploy `GET /health/ready` no devuelve 200: rollback automático a Supabase Auth (feature flag off), investigación antes de reintentar.

# Entregables esperados (checklist)

- [ ] `Skill("ps-contexto")` ejecutado y contexto cargado.
- [ ] ≥3 `ps-explorer` dispatched en paralelo, reportes consolidados.
- [ ] `Skill("brainstorming")` con las 4+ decisiones tácticas lockeadas via `AskUserQuestion`.
- [ ] Plan persistido en `.docs/raw/plans/2026-04-17-wave-b-migracion-bitacora-zitadel.md`.
- [ ] Tarjetas hijas creadas en GitHub con `Skill("pj-crear-tarjeta")` — las mayores, no todas las micro-tasks.
- [ ] `Bitacora.Api` validando JWT via JWKS RS256, env vars migradas en Infisical (via `mi-key-cli`).
- [ ] Frontend usando `oidc-client-ts` (o equivalente validado en brainstorming), `middleware.ts` actualizado.
- [ ] Script de migración de usuarios ejecutado en staging y validado idempotente.
- [ ] Wiki actualizada (`02_arquitectura.md`, `07_baseline_tecnica.md`, `09_contratos/CT-AUTH.md`).
- [ ] Smoke E2E Playwright pasando con smoke creds (`smoke@bitacora.test`, `smoke-prof@bitacora.test`) bajo el nuevo IdP.
- [ ] Deploy a prod con feature flag rolled out gradualmente (10% → 50% → 100%).
- [ ] `SUPABASE_JWT_SECRET` rotado y retirado de Infisical.
- [ ] `auth.bitacora.nuestrascuentitas.com` DNS retirado o redirigido a Zitadel.
- [ ] Issues #2, #6, #14 cerradas con comentario de supersedencia.
- [ ] Issue #17 (Wave B) cerrada con evidencia.
- [ ] `Skill("ps-trazabilidad")` + `Skill("ps-auditar-trazabilidad")` limpios sin gaps.

# Referencias rápidas

| Item | Valor |
|------|-------|
| Repo | `C:\repos\mios\humor` (fgpaz/bitacora) |
| Branch | `main` |
| Project board | https://github.com/users/fgpaz/projects/4 |
| Epic parent | https://github.com/fgpaz/bitacora/issues/15 |
| Wave A prerequisito | https://github.com/fgpaz/bitacora/issues/16 |
| Wave B (esta misión) | https://github.com/fgpaz/bitacora/issues/17 |
| Dokploy frontend appId | `BRTMuvBfWtslXHnShtrnB` |
| URL prod | https://bitacora.nuestrascuentitas.com |
| IdP Teslita (post-Wave A) | https://id.nuestrascuentitas.com |
| Design doc | `.docs/raw/plans/2026-04-17-idp-zitadel-multi-ecosistema-design.md` |
| Smoke creds patient | `smoke@bitacora.test` / `SmokeTest2026!` (via Infisical) |
| Smoke creds prof | `smoke-prof@bitacora.test` / `SmokeProfTest2026!` |
| Fecha de lockeo arquitectónico | 2026-04-17 |
