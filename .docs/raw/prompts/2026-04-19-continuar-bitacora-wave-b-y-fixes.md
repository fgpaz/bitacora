<!--
target_platform: codex
pressure: aggressive
generated_at: 2026-04-19
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-19-continuar-bitacora-wave-b-y-fixes.md' | Set-Clipboard"
-->

# Misión

Estás en `C:\repos\mios\humor` (Bitácora — clinical mood tracker, .NET 10 + Next.js 16 + Telegram Bot + PostgreSQL, deploy Dokploy VPS `54.37.157.93` alias `turismo`).

Tu misión es **continuar Bitácora desde el estado post Wave A GREEN de Zitadel**: preparar y ejecutar Wave B del issue `fgpaz/bitacora#17` y ordenar los fixes pendientes abiertos, sin reabrir decisiones ya cerradas.

## Estado actual verificado

- Wave A Zitadel Teslita está GREEN.
- Commit reciente de referencia: `7e48087 docs(zitadel): record final board sync`.
- Cierre GREEN principal: `9ea904c docs(zitadel): close wave a remaining gaps`.
- IdP compartido: `https://id.nuestrascuentitas.com`.
- Contrato canónico: `.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md`.
- Handoff técnico para otros proyectos: `.docs/raw/reports/2026-04-19-zitadel-shared-idp-integration-handoff.md`.
- Smoke final: `artifacts/e2e/2026-04-19-cerrar-gaps-wave-a/README.md`.
- Report final: `.docs/raw/reports/2026-04-19-cerrar-gaps/T5-final-green-closure.md`.
- Issues Wave A gap closure `#18`, `#19`, `#20`: cerrados y board `Done`.
- Epic `#15`: abierto como tracking.
- Wave B `#17`: abierta y desbloqueada.
- Supabase Auth sigue activo para Bitácora hasta que Wave B haga cutover aceptado.

## Preflight obligatorio

Antes de decidir o editar:

1. `$ps-contexto`.
2. `$mi-lsp`: validar alias/path con `mi-lsp workspace list` y `mi-lsp workspace status bitacora --format toon`. Si hay `hint` o `next_hint`, seguirlo.
3. `git log -5 --oneline` debe incluir `7e48087` o un commit posterior equivalente.
4. `git status --short`: debe estar limpio o con docs/prompts untracked conocidos. Si hay WIP de código, reportar y parar.
5. `gh issue view 17 --repo fgpaz/bitacora` y `gh issue list --repo fgpaz/bitacora --state open --limit 40`.
6. `curl -sS -o /dev/null -w "%{http_code}\n" https://id.nuestrascuentitas.com/.well-known/openid-configuration` debe devolver `200`.
7. `curl -sS -o /dev/null -w "%{http_code}\n" https://id.nuestrascuentitas.com/oauth/v2/keys` debe devolver `200`.
8. `bash $HOME/.claude/skills/mi-key-cli/scripts/mkey.sh pull bitacora prod` debe traer secrets desde Infisical. No crear secrets en plaintext.

## Decisiones lockeadas

- Zitadel self-hosted v4.9.0 en `id.nuestrascuentitas.com`.
- Org Bitácora en Zitadel: `bitacora`, orgId `369305924310925670`.
- Web client Bitácora: `369306336963330406`.
- M2M Bitácora: `bitacora-api-client`.
- Protocolo frontend: OIDC Authorization Code + PKCE.
- Protocolo backend: Bearer JWT RS256 validado por JWKS.
- Login UI v2 ya funciona.
- Client credentials ya funciona para las 4 orgs.
- Backup/offsite/legacy cleanup/DKIM-SPF/passwordless admin ya están cerrados.
- No reabrir `#16`; documentar nuevos hallazgos en issues nuevos o en `#17`.
- No apagar Supabase Auth hasta cutover de Wave B aceptado y con rollback.
- No regenerar `ZITADEL_MASTERKEY`.
- No commitear JWTs, PATs, client secrets ni passwords.

## Exploración obligatoria con subagents

Después de `$ps-contexto` + `$mi-lsp`, despachar en paralelo al menos 3 `ps-explorer` en un solo turno:

1. **Backend auth surface:** mapear `src/Bitacora.Api/**` para JWT actual, `SUPABASE_JWT_SECRET`, claims usados, `Program.cs`, middleware, readiness y tests. Responder con `file:line`, env vars y flujo actual.
2. **Frontend auth surface:** mapear `frontend/**` para Supabase, middleware, cookies, rutas protegidas, `/ingresar`, logout, dependencias y tests. Responder con `file:line` y flujo actual.
3. **Issues/docs trace:** cruzar `#17`, `#2`, `#3`, `#6`, `#7`, `#9`, `#11`, `#12`, `#13`, `#14`, `#15` contra `.docs/wiki/02_arquitectura.md`, `.docs/wiki/07_baseline_tecnica.md`, `.docs/wiki/09_contratos/CT-AUTH-ZITADEL.md` y el handoff nuevo. Identificar qué queda superseded por Wave B y qué es backlog independiente.

Agregar Explorer 4 si Telegram depende de claims o sesiones de auth. Agregar Explorer 5 si Playwright/E2E forja JWTs o hay conflicto entre cookies actuales y OIDC.

## `$brainstorming` obligatorio

Lockear estas decisiones antes de planificar:

1. Estrategia Wave B: dual-auth feature flag primero vs cutover directo. Recomendado: dual-auth con rollback.
2. Migración de usuarios: bulk import vs link-on-first-login. Recomendado inicial: explorar datos reales y elegir con evidencia.
3. Cookie/session name: mantener compatibilidad temporal o renombrar a sesión OIDC clara.
4. Claim roles: mapping de `urn:zitadel:iam:org:project:roles` a roles locales `patient`/`professional`.
5. Fix bundling: resolver primero auth blockers superseded (`#2`, `#3`, `#6`, `#14`) y dejar DX/polish en batch posterior salvo que bloqueen tests.
6. Cutover safety: ventana, smoke, rollback y criterios para retirar Supabase Auth.

## `$writing-plans`

Persistir el plan en:

```text
.docs/raw/plans/2026-04-19-bitacora-wave-b-zitadel-y-fixes.md
```

El plan debe separar:

- B0 preflight/secrets/redirect URIs.
- B1 backend dual JWT validation JWKS RS256.
- B2 frontend OIDC PKCE + callback/logout/session.
- B3 user migration/linking.
- B4 tests/smoke Playwright.
- B5 docs/runbooks/Infisical sync.
- B6 cutover/rollback.
- F1 fixes independientes abiertos: `#7`, `#9`, `#11`, `#12`, `#13` y validar `#10` si sigue vigente.

Cada task debe tener subagent, archivos, aceptación, riesgo de drift y rollback.

## Ejecución

Usar subagents por ownership:

- `ps-dotnet10`: `src/Bitacora.Api/**`, auth backend, tests.
- `ps-next-vercel`: `frontend/**`, OIDC frontend, middleware, Playwright.
- `ps-worker`: Infisical via `mkey`, scripts, GitHub issues, smoke, shell.
- `ps-docs`: wiki, runbooks, evidencia y cierre.

Archivos permitidos ahora:

- `src/Bitacora.Api/**`
- `frontend/**`
- `infra/runbooks/**`
- `infra/smoke/**`
- `infra/migrations/zitadel/**`
- `.docs/wiki/02_arquitectura.md`
- `.docs/wiki/07_baseline_tecnica.md`
- `.docs/wiki/09_contratos/**`
- `.docs/raw/plans/**`
- `.docs/raw/reports/**`
- `artifacts/e2e/**`

Mantener especial cuidado con datos de salud: no loguear PII, datos clínicos, JWTs completos ni payloads sensibles.

## Cierre obligatorio

- `$ps-trazabilidad` después de cada batch importante.
- `$ps-auditar-trazabilidad` antes de marcar Wave B o un batch grande como terminado.
- Actualizar issue `#17` con evidencia.
- Cerrar issues superseded solo cuando el cambio que los reemplaza esté mergeado/verificado.
- Dejar smoke evidence en `artifacts/e2e/<YYYY-MM-DD>-<slug>/`.
- Mantener el epic `#15` actualizado.

## Resultado esperado de la próxima sesión

Primero debe producir un plan ejecutable basado en exploración real. Si el plan queda aprobado o la tarea permite autonomía, continuar con el primer batch de implementación de Wave B sin tocar decisiones cerradas de Wave A.
