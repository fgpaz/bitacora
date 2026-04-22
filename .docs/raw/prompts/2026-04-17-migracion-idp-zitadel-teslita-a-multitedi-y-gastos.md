<!--
target_platform: claude-code
pressure: aggressive
generated_at: 2026-04-17
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-17-migracion-idp-zitadel-teslita-a-multitedi-y-gastos.md' | Set-Clipboard"
-->

# Mision

Estas trabajando en uno de los dos repos del ecosistema **Teslita**: `C:\repos\mios\multi-tedi` o `C:\repos\mios\gastos`. Mira tu CWD al arrancar y adaptalo; el resto del prompt es identico para ambos.

El ecosistema Teslita (nuestrascuentitas.com, bitacora, multi-tedi, gastos) va a migrar a un **IdP unico compartido**: Zitadel self-hosted en `id.nuestrascuentitas.com`. La decision arquitectonica ya esta lockeada en otra sesion (2026-04-17) y persistida canonicamente en:

- **Design doc canonico:** `C:\repos\mios\humor\.docs\raw\plans\2026-04-17-idp-zitadel-multi-ecosistema-design.md`

Tu tarea en esta sesion NO es implementar la migracion todavia. Tu tarea es **mapear la superficie de auth actual** del repo donde estas, dejar evidencia que permita planificar la migracion en una sesion futura, y reportar hallazgos. Ni refactores, ni commits de codigo, ni cambios de contratos. Solo discovery + reporte.

# Decisiones lockeadas (NO reabrir)

1. **Stack IdP:** Zitadel self-hosted (Go + Postgres, Apache 2.0). No evaluar alternativas.
2. **Instancia Teslita:** un solo Zitadel en `id.nuestrascuentitas.com` que cubre nuestrascuentitas.com, bitacora, multi-tedi, gastos.
3. **SSO dentro de Teslita:** un solo `user_id` por persona para todos los productos del ecosistema. Si el usuario se loguea en `gastos`, queda logueado en `multi-tedi` y viceversa.
4. **Aislamiento cross-ecosistema:** el ecosistema Buho (buho/salud, buho/digitalizacion) corre un Zitadel SEPARADO en `id.buho.ar`. No se mezclan.
5. **Protocolo:** OIDC + PKCE para SPAs, Bearer JWT con JWKS (RS256 asimetrico) para resource servers. Nada de claves simetricas.
6. **Reemplaza:** cualquier uso de Supabase Auth / GoTrue / auth-helpers. Si el repo no usa auth todavia, se integra directo.

Si mientras exploras encontras una razon fundada para reabrir alguna de estas decisiones, reporta el hallazgo pero NO actues sobre el. Escalar al usuario primero.

# Verificacion previa (obligatorio antes de tocar nada)

Hace estas comprobaciones con lecturas directas antes de lanzar subagents:

1. `git status --short` → debe estar limpio o casi. Si hay WIP, reportalo.
2. `git log -1 --format="%H %s"` → anota el commit base.
3. Confirma el CWD: `pwd` (o equivalente) y verifica que es `multi-tedi` o `gastos`.
4. Lee el design doc canonico antes que cualquier otra cosa: `C:\repos\mios\humor\.docs\raw\plans\2026-04-17-idp-zitadel-multi-ecosistema-design.md`. Todo lo que sigue asume que lo leiste.
5. Confirma si existe `CLAUDE.md` o `AGENTS.md` en el repo. Si no, nota la ausencia para tu reporte final.
6. Confirma la wiki: glob `.docs/wiki/*.md`. Si no existe, el repo esta en estado pre-SDD y tu reporte debe mencionarlo.

# Workflow obligatorio (SDD)

## Paso 1 — Contexto + exploracion

1. **`Skill("ps-contexto")`** — bloqueante. Carga el contexto del repo actual. Si no hay wiki (`.docs/wiki/`), documentalo como gap y continua con lo que haya.
2. **`mi-lsp workspace list`** y **`mi-lsp workspace status <alias> --format toon`**. Si `mi-lsp` devuelve `hint` o `next_hint`, segui esa guia antes de hacer fallback. Si `mi-lsp` no esta indexado para este repo, documenta y cae a `rg` + `Grep`.
3. **Dispatch EN PARALELO en UN SOLO mensaje al menos 3 `ps-explorer` subagentes** (subagent_type=`ps-explorer`). Objetivos minimos:

   - **Explorer 1 — Auth surface mapping:** localizar todo el codigo que toca autenticacion hoy.
     - Buscar imports de `@supabase/supabase-js`, `@supabase/ssr`, `@supabase/auth-helpers-nextjs`, `next-auth`, `@clerk/*`, `@auth0/*`, `oidc-client-ts`, `openid-client`, `@okta/*`, y patrones propios.
     - Buscar `signIn`, `signOut`, `getSession`, `signInWithOtp`, `signInWithPassword`, `signInWithOAuth`.
     - Buscar `middleware.ts` (si es Next.js) o equivalentes (Express middleware, ASP.NET `AddJwtBearer`, etc.).
     - Reportar `file:line` para cada hallazgo. Si no hay auth todavia, reportarlo.
   - **Explorer 2 — JWT + session storage trace:** trazar como se persiste la sesion.
     - localStorage keys, cookies HTTP (nombre, domain, SameSite, Secure, path, maxAge).
     - server-side sessions (Redis, DB tables).
     - JWT validation (si el repo tiene API: donde se valida el token, con que secret, que claims se esperan).
     - Reportar el nombre exacto de cookies, keys de localStorage, y cualquier secret esperado via env vars.
   - **Explorer 3 — User model + migration surface:** mapear el modelo de usuario actual.
     - Tablas DB o documentos que referencian `user_id`, `email`, `supabase_user_id`, `sub`.
     - Relacion entre tablas de dominio y la tabla de usuarios.
     - Foreign keys que bloquearian una migracion cruda de usuarios.
     - Seeders, fixtures, tests que crean usuarios hardcoded.
     - Si hay auth externo (Supabase en otra instancia): URL, env var name, si se comparte con otro repo del ecosistema.

4. **Escalacion a Explorer 4 y 5:**
   - **Explorer 4 — infra + env + deploy** si: Explorer 1 detecta que el repo corre en Dokploy/Vercel con variables de entorno que referencian URLs de auth; o si el repo tiene `infra/`, `deploy/`, `.do/`, `dokploy/`, `vercel.json`, `Dockerfile` con auth secrets.
     - Mapear donde viven las env vars (Infisical, .env.template, dokploy.md).
     - Reportar que env vars tendrian que reemplazarse por variables Zitadel (`ZITADEL_ISSUER`, `ZITADEL_CLIENT_ID`, `ZITADEL_CLIENT_SECRET` si aplica).
   - **Explorer 5 — tests + smoke gates** si: Explorer 1 detecta test suites que mockean auth, o Explorer 3 detecta usuarios hardcoded en seeders.
     - Listar tests que asumen un shape de JWT/sesion especifico.
     - Listar smoke gates que hacen login real o forjan JWT con `SUPABASE_JWT_SECRET`.
     - Reportar cuales tests tendrian que refactorearse cuando se migre.

5. Para cada cita de codigo en tu reporte, validar con `mi-lsp` (preferido) o `Grep`. Nada de `file:line` inventado.

## Paso 2 — Brainstorming (condicional)

Si los explorers reportan algo que contradice alguna de las decisiones lockeadas (p.ej. el repo ya usa Clerk, o ya usa OIDC distinto, o tiene un IdP propio), **detenete y ejecuta `Skill("brainstorming")`** para escalar al usuario con la contradiccion antes de avanzar.

Si los explorers confirman que el repo usa Supabase Auth (lo esperado) o esta pre-auth, salta este paso.

## Paso 3 — Writing-plans (opcional en esta sesion)

Solo si el usuario explicitamente te pide generar el plan de migracion detallado en esta sesion, usa `Skill("writing-plans")` con persistencia a `.docs/raw/plans/2026-04-17-<slug>-migracion-zitadel.md` (donde `<slug>` es `multitedi` o `gastos`). Por defecto, tu entregable es el reporte de discovery, no el plan. El plan detallado se redacta en otra sesion una vez que los hallazgos esten consolidados.

## Paso 4 — Ejecucion (discovery + reporte)

**No toques codigo de produccion.** Tu ejecucion es escritura de documentacion:

1. Crea (si no existe) el directorio `.docs/raw/reports/`.
2. Genera `.docs/raw/reports/2026-04-17-<slug>-auth-surface.md` con secciones:
   - `## Resumen ejecutivo` (1 parrafo de 3-4 oraciones: que stack de auth usa hoy, blast radius de la migracion).
   - `## Stack auth actual` (tabla: biblioteca, version, rol).
   - `## Superficie de codigo` (lista `file:line` para cada touchpoint).
   - `## Modelo de usuario` (tablas/documentos, FKs, seeders).
   - `## Persistencia de sesion` (cookies, localStorage, server sessions).
   - `## Env vars afectadas` (mapa de vars actuales → vars Zitadel propuestas).
   - `## Tests y smoke gates afectados` (si aplicable, desde Explorer 5).
   - `## Riesgos y unknowns` (cualquier gap que no pudiste cerrar).
   - `## Estimacion de esfuerzo` (rangos dia/persona para: refactor de client, refactor de server/API, migracion de usuarios, update de tests).
   - `## Referencias` (link al design canonico + issues GitHub si los hay).
3. Si el repo usa GitHub Issues y existe un project board Teslita, crea (o pedi autorizacion para crear) una issue-epic titulada `[MIGR-IDP] Zitadel migration scope — <slug>` con el reporte como body. Si no estas seguro si hay project board, preguntale al usuario antes de crear issues.
4. NO modifiques codigo de runtime en este paso. Eso es para una sesion posterior con el plan detallado.

## Paso 5 — Closure

1. `Skill("ps-trazabilidad")` — verifica que el reporte quedo persistido y referenciado correctamente.
2. Si el repo tiene wiki SDD y la migracion IdP impactaria RF/FL/modelo de datos, menciona explicitamente que RF/FL habria que tocar, pero **no los toques en esta sesion**.
3. Reporta al usuario en chat:
   - Path del reporte generado.
   - Top 3 hallazgos sorpresivos (si los hay).
   - Recomendacion de siguiente paso (p.ej. "avanzar con writing-plans en sesion dedicada", "resolver X antes de planificar").

# Boundaries (explicit)

- **Codigo de runtime prohibido:** no hay cambios al cliente auth, al middleware, a las migraciones DB, ni al endpoint API en esta sesion. Solo discovery + reporte documental.
- **Docs permitidas:** crear/modificar bajo `.docs/raw/reports/` (y opcionalmente `.docs/raw/plans/` si el usuario lo pide explicito).
- **Tocar otros repos prohibido:** no leer/editar `C:\repos\mios\humor`, `C:\repos\buho\salud`, ni cualquier otro path fuera del CWD. El design doc canonico lo leer UNA vez al principio (esta fuera pero es lectura autorizada).
- **Secretos:** no imprimir valores de env vars en logs ni en el reporte. Referenciar por NOMBRE unicamente.
- **Commits/pushes:** no commitear cambios. El reporte queda como archivo staged para que el usuario decida cuando commit.

# Severity / drift policy

- Si el repo NO tiene git configurado o el `git log` falla: aborta y reporta.
- Si el repo ya esta integrado con Clerk/Auth0/un IdP distinto a Supabase: aborta el flow estandar, ejecuta `Skill("brainstorming")` para escalar la contradiccion con la decision lockeada antes de hacer cualquier reporte.
- Si los 3 explorers devuelven resultados contradictorios (p.ej. Explorer 1 dice que usa Supabase y Explorer 3 no encuentra tabla de usuarios), lanza un Explorer 6 de resolucion antes de consolidar el reporte.
- Si Explorer 2 reporta secrets en plaintext en el repo, detenete, reportalo como incidente de seguridad al usuario, y no lo copies al reporte.
- Tratar cualquier ambiguedad sobre infra compartida (VPS, Dokploy apps, DNS zones) como riesgo alto y preguntar al usuario antes de asumir.

# Entregables esperados (checklist)

- [ ] `.docs/raw/reports/2026-04-17-<slug>-auth-surface.md` creado y completo.
- [ ] `ps-trazabilidad` ejecutado sin gaps criticos.
- [ ] Issue epic creada en GitHub (solo si hay project board y usuario autoriza).
- [ ] Reporte final al usuario con top hallazgos y recomendacion de siguiente paso.
- [ ] Ningun cambio en codigo de runtime.

# Referencias rapidas

| Item | Valor |
|------|-------|
| Design doc canonico | `C:\repos\mios\humor\.docs\raw\plans\2026-04-17-idp-zitadel-multi-ecosistema-design.md` |
| Memoria cross-proyecto | `C:\Users\fgpaz\.claude\projects\C--repos-mios-humor\memory\project_idp_zitadel_multi_ecosistema.md` |
| Ecosistema Teslita | nuestrascuentitas.com, bitacora, multi-tedi, gastos |
| IdP instancia Teslita | `id.nuestrascuentitas.com` (a levantar, Dokploy VPS) |
| Stack IdP | Zitadel self-hosted (Go + Postgres + Apache 2.0) |
| Fecha decision | 2026-04-17 |
