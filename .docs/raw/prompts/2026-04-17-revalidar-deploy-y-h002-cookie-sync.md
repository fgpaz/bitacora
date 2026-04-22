<!--
target_platform: claude-code
pressure: aggressive
generated_at: 2026-04-17
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-17-revalidar-deploy-y-h002-cookie-sync.md' | Set-Clipboard"
-->

# Mision

Trabajamos en `C:\repos\mios\humor` (Bitacora — clinical mood tracker, .NET 10 + Next.js 16 + Telegram Bot + PostgreSQL, deploy Dokploy VPS 54.37.157.93). Branch activo: `main`. Sesion previa cerro 2026-04-17 con dos pistas abiertas que vos vas a ejecutar **en paralelo** y cerrar en esta sesion:

- **Pista A — Revalidar deploy del fix H-001 en produccion** (issue `fgpaz/bitacora#1`, project board `fgpaz/Bitacora #4`). El commit `02e724a` ya esta en `origin/main` y el deploy fue disparado via `application.deploy` HTTP 200 a las ~18:55 UTC. Build estimado 2-4 min. Tenes que confirmar que termino, validar el fix con Playwright MCP en prod, cerrar el issue y mover la tarjeta a `Done` (option id `f5fea9f8`).
- **Pista B — Atacar H-002 cookie/localStorage divergence (P0)** (issue `fgpaz/bitacora#2`). Hay sospecha fundada de que el bug tapa fallas mayores que el landing solo. Hay que reproducir, disenar fix, implementarlo, validar end-to-end, cerrar issue y mover tarjeta a `Done`.

No abras tarjetas nuevas salvo que aparezca un bug realmente nuevo y verificado. No reabras decisiones cerradas (ver `## Decisiones bloqueadas`).

# Verificacion de estado del repo (obligatorio antes de tocar nada)

Confirma con lecturas directas que el cierre de la sesion previa se mantiene. No te creas el prompt: comprobalo:

1. `git log -1 --format="%H %s"` debe arrancar con `02e724a fix(landing): render email input + error slot on OnboardingEntryHero`. Si no, abortas y reportas drift.
2. `git status --short` debe estar vacio salvo posibles cambios auto-generados (`frontend/next-env.d.ts`).
3. Confirmar que el board y los issues existen: `gh project view 4 --owner fgpaz` y `gh issue view 1 --repo fgpaz/bitacora`. Tienen que existir y issue #1 estar abierto.
4. Leer `artifacts/e2e/2026-04-17-empezar-e2e-full/DIAGNOSTICO.md` (es la base de los 13 hallazgos) y `artifacts/e2e/2026-04-17-empezar-e2e-full/issues/H-002.md` (cuerpo del issue que vas a cerrar).
5. Confirmar que `.pj-crear-tarjeta.conf` apunta a `fgpaz/bitacora` y project number `4`.

# Workflow obligatorio (SDD)

## Paso 1 — Contexto + exploracion

1. `Skill("ps-contexto")` — carga `.docs/wiki/01..09` y devolvera la estructura. Es bloqueante.
2. **Despachar EN PARALELO en un solo mensaje al menos 3 `ps-explorer` subagentes** (subagent_type=`ps-explorer`). Tus objetivos minimos:
   - **Explorer 1 — Auth surface mapping:** localizar todo el codigo que toca cookies o sesion Supabase. Buscar en `frontend/middleware.ts`, `frontend/lib/auth/*`, `frontend/providers/*`, `frontend/app/**/*.tsx`. Reportar quien lee `sb-access-token`, quien la escribe (si alguien), donde esta el `createClient` y si el package.json incluye `@supabase/ssr`. Devolver `file:line` de cada hallazgo.
   - **Explorer 2 — Magic-link flow trace:** trazar desde el click "Empezar ahora" en `/` hasta el render de `/dashboard`. Listar handlers, redirecciones (cliente y middleware), useEffects que disparen network, y donde se persistiria la sesion post-redirect. Reportar el orden de eventos esperado y los puntos donde puede romperse la sincronizacion cookie/localStorage.
   - **Explorer 3 — Test-pattern + smoke creds:** localizar runbooks de E2E (`infra/runbooks/`), patrones previos de Playwright en `artifacts/e2e/2026-04-1*`, y como se usan las smoke creds (`SMOKE_TEST_EMAIL`, `SMOKE_PROF_EMAIL`) en validaciones existentes. Reportar el flujo Supabase password grant ya validado.
3. **Sumar Explorer 4 y 5 obligatoriamente** si:
   - Explorer 1 reporta que ya existe `@supabase/ssr` instalado pero no enchufado, o
   - Explorer 2 detecta dos paths divergentes hacia `/dashboard` (lo que multiplicaria el blast radius del fix), o
   - Explorer 3 encuentra runbooks contradictorios sobre como inyectar sesion en E2E.
   - Explorer 4 sugerido: rastrear como Bitacora.Api valida el JWT (busqueda en `Bitacora.Api/**/*.cs` por `JwtBearer` y `Authority`) — esto define si tu fix puede mover la cookie de nombre o no.
   - Explorer 5 sugerido: rastrear si Telegram Bot consume la misma cookie/sesion (`Bitacora.Telegram/**/*.cs`) — si si, el fix debe contemplar compatibilidad cross-canal.
4. Para cada cita de codigo en tu reporte, validar con `mi-lsp` (preferido) o `Grep`. Nada de `file:line` sin verificar.

## Paso 2 — Brainstorming (obligatorio)

Antes de tocar codigo del fix de H-002, abrir `Skill("brainstorming")` con dos opciones explicitas y pedir confirmacion del usuario via `AskUserQuestion`:

- **Opcion A — Migrar a `@supabase/ssr`** (`createBrowserClient` con cookie storage). Fix correcto, toca varios archivos, requiere validar Bitacora.Api JWT validation, posible refactor de `SessionContext`.
- **Opcion B — Patch quirurgico en `SessionContext`**: en `onAuthStateChange` y en mount inicial, escribir `document.cookie = "sb-access-token=<jwt>; path=/; SameSite=Lax; Secure; max-age=<exp-now>"`. Borrar en `signOut`. Compatible con codigo actual, scope minimo.

Diagrama ASCII obligatorio del cambio. Pros/cons por opcion. Recomendacion explicita. Una sola decision; no proceder sin la respuesta del usuario o sin que el usuario haya autorizado autonomia.

## Paso 3 — Plan (solo si la decision es Opcion A o si scope crece)

Si la opcion elegida toca >=4 archivos o multi-modulo: `Skill("writing-plans")` con persistencia a `.docs/raw/plans/2026-04-17-h002-cookie-sync.md`. Si scope es Opcion B con <=3 archivos, saltar este paso.

## Paso 4 — Ejecucion (en paralelo donde no haya dependencia)

### Pista A — Revalidar deploy fix H-001

1. Cargar env y poll del status del deploy. Comando exacto:
   ```bash
   set -a; source infra/.env; set +a
   until [ "$(curl -sS -H "x-api-key: $DOKPLOY_API_KEY" "$DOKPLOY_URL/api/application.one?applicationId=BRTMuvBfWtslXHnShtrnB" | python -c 'import sys,json;print(json.load(sys.stdin).get("applicationStatus",""))')" = "done" ]; do sleep 10; done
   echo "deploy DONE"
   ```
   Si despues de 10 minutos sigue en `running`, abrir issue nuevo via `Skill("pj-crear-tarjeta")` y reportar al usuario.

2. Cuando este `done`, abrir Playwright MCP y validar el fix en prod:
   - `browser_navigate` a `https://bitacora.nuestrascuentitas.com/?_=<unix-ts>` (cache bust).
   - `browser_snapshot` debe contener `textbox "Tu correo electronico"`. Si NO esta el input, el deploy no aplico el commit `02e724a`. Abortar Pista A y abrir incidente.
   - `browser_click` "Empezar ahora" SIN tipear -> esperar `alert "Ingresá tu correo electronico."` con `role="alert"`. Screenshot.
   - `browser_type` un email descartable (`e2e-2026-04-17-revalidacion@example.test`, NO uses smoke creds aqui porque dispara email real). `browser_click` -> esperar la pantalla "Revisá tu correo" o un mensaje de error coherente. Screenshot.
   - Guardar evidencia en `artifacts/e2e/2026-04-17-h001-revalidacion-prod/{01-input.png,02-error.png,03-sent.png,console.log,network.log}`.

3. Cerrar issue #1 y mover tarjeta a Done. Comando exacto:
   ```bash
   gh issue close 1 --repo fgpaz/bitacora --comment "Validado en produccion $(date -u +%FT%TZ). Evidencia: artifacts/e2e/2026-04-17-h001-revalidacion-prod/. Status board -> Done."
   gh api graphql -f query='mutation{updateProjectV2ItemFieldValue(input:{projectId:"PVT_kwHOAUIR6s4BU7_D", itemId:"PVTI_lAHOAUIR6s4BU7_DzgqSB8I", fieldId:"PVTSSF_lAHOAUIR6s4BU7_DzhLehO0", value:{singleSelectOptionId:"f5fea9f8"}}){projectV2Item{id}}}'
   ```

### Pista B — Implementar fix H-002

1. Reproducir el bug en prod (post-Pista A): magic-link real con email tuyo, click el link recibido, anotar si llegas a `/dashboard` o si volves a `/`. Si el deploy aplico el fix de input pero seguis quedando en `/`, la sospecha de H-002 esta confirmada.

2. Aplicar el fix segun la opcion lockeada en Paso 2. Para Opcion B, el patch tipico vive en `frontend/lib/auth/SessionContext.tsx` dentro del useEffect que escucha `onAuthStateChange`:

   ```tsx
   const writeCookie = (session: Session | null) => {
     if (typeof document === 'undefined') return;
     if (session?.access_token) {
       const exp = session.expires_at ? Math.max(0, session.expires_at - Math.floor(Date.now()/1000)) : 3600;
       const secure = location.protocol === 'https:' ? '; Secure' : '';
       document.cookie = `sb-access-token=${session.access_token}; path=/; SameSite=Lax${secure}; max-age=${exp}`;
     } else {
       document.cookie = 'sb-access-token=; path=/; max-age=0';
     }
   };
   // llamarlo en getSession().then(...) y en onAuthStateChange((_, s) => { writeCookie(s); ... })
   ```

3. Validar end-to-end con Playwright MCP. Reusar el patron de la sesion previa: password grant a `https://auth.bitacora.nuestrascuentitas.com/token?grant_type=password` con header `apikey: $NEXT_PUBLIC_SUPABASE_ANON_KEY` (en `infra/.env`). Body `{"email":"smoke@bitacora.test","password":"SmokeTest2026!"}`. Token volvera en `access_token` del JSON. Inyectar en localStorage del cliente (key default Supabase) y dejar que el `useEffect` escriba la cookie. Navegar `/dashboard` -> debe NO redirigir.

4. Repetir con `smoke-prof@bitacora.test` / `SmokeProfTest2026!` para `/profesional/pacientes`.

5. Probar logout: click "Cerrar sesion" -> confirmar que la cookie quedo borrada (`document.cookie` no contiene `sb-access-token`).

6. Guardar evidencia en `artifacts/e2e/2026-04-17-h002-cookie-sync/{01-pre-fix.png,02-post-fix-dashboard.png,03-post-fix-prof.png,04-logout-cookie-cleared.png,console.log,diff.txt}`.

7. Commit con mensaje `fix(auth): sync supabase session into sb-access-token cookie\n\nCloses #2.` y `git push origin main`. Disparar deploy:
   ```bash
   curl -sS -X POST -H "x-api-key: $DOKPLOY_API_KEY" -H "Content-Type: application/json" \
     -d "{\"applicationId\":\"BRTMuvBfWtslXHnShtrnB\"}" \
     "$DOKPLOY_URL/api/application.deploy"
   ```
   Esperar `done` y revalidar el flow completo en prod.

8. Cerrar issue #2 + mover tarjeta a Done (option id `f5fea9f8`, item id `PVTI_lAHOAUIR6s4BU7_DzgqSB9c`).

## Paso 5 — Closure

`Skill("ps-trazabilidad")` despues de cerrar ambos issues. Verificar sync FL/RF/TP donde aplique. Si `Skill("ps-auditar-trazabilidad")` aplica (cambio multi-modulo), correrlo tambien.

## Paso 6 — Cleanup

- `rm -rf /tmp/e2e-tokens` (sesion previa los limpio, pero por si quedaron).
- Confirmar que ningun JWT plaintext quedo en `artifacts/`.
- Reportar al usuario URLs de issues cerrados, screenshots y commit hashes.

# Decisiones bloqueadas (no reabrir)

- **Repo unificado:** issues + codigo viven en `fgpaz/bitacora`. No mover.
- **Project board #4:** ya tiene Status field con opciones `Roadmap | Todo | Doing | Done`. No renombrar ni recrear.
- **Hallazgos H-001..H-013:** ya estan creados como issues. No duplicar.
- **Smoke creds:** son las del `infra/.env`. No rotar en esta sesion.
- **Deploy target:** Dokploy VPS 54.37.157.93, app id `BRTMuvBfWtslXHnShtrnB` para frontend.
- **CLAUDE.md / AGENTS.md:** vigentes. Cualquier edit requiere `Skill("ps-crear-agentsclaudemd")`.

# Boundaries

- Codigo permitido: `frontend/lib/auth/**`, `frontend/middleware.ts` (solo si Opcion A lo requiere).
- Codigo prohibido sin escalar: `Bitacora.Api/**` (tocar la auth backend cambia contrato JWT) y `Bitacora.Telegram/**` (puede romper el bot).
- Documentacion: actualizar wiki si el fix cambia un contrato (`Skill("ps-asistente-wiki")` para decidir cual doc tocar). Sin cambios de contrato, no tocar wiki.
- Secretos: NO commitear `frontend/.env.local`, `infra/.env`, ni JWTs en evidencia (mask con `***` los tokens en logs antes de commitear).
- Despliegue: solo el frontend en esta sesion. NO redeployar API ni Telegram salvo que H-002 lo exija.

# Severidad / drift policy

- Si `Explorer 1` o `Explorer 2` revelan que ya existe un fix en otra rama o WIP, **detenerse y reportar** antes de re-implementar.
- Si el deploy de la Pista A no termina en 10 min, abrir tarjeta y reportar — no esperar indefinidamente.
- Si el fix de H-002 cambia el nombre de la cookie o el contrato JWT, escalar al usuario antes de pushear (impacta backend + Telegram).
- Tratar cualquier ambiguedad sobre `@supabase/ssr` como riesgo: leer `package.json` y `node_modules/@supabase/` antes de proceder.
- No marcar Done una tarjeta sin evidencia visual (screenshot) en `artifacts/`.

# Referencias rapidas

| Item | Valor |
|------|-------|
| Branch | `main` |
| Commit base | `02e724a` |
| Repo issues + codigo | `fgpaz/bitacora` |
| Project | https://github.com/users/fgpaz/projects/4 |
| Project ID | `PVT_kwHOAUIR6s4BU7_D` |
| Status field ID | `PVTSSF_lAHOAUIR6s4BU7_DzhLehO0` |
| Status options | Roadmap `38d8fcc4` · Todo `09227932` · Doing `e2034599` · Done `f5fea9f8` |
| Issue #1 item id | `PVTI_lAHOAUIR6s4BU7_DzgqSB8I` |
| Issue #2 item id | `PVTI_lAHOAUIR6s4BU7_DzgqSB9c` |
| Dokploy frontend appId | `BRTMuvBfWtslXHnShtrnB` |
| URL prod | https://bitacora.nuestrascuentitas.com |
| GoTrue prod | https://auth.bitacora.nuestrascuentitas.com |
| Smoke patient | `smoke@bitacora.test` / `SmokeTest2026!` |
| Smoke prof | `smoke-prof@bitacora.test` / `SmokeProfTest2026!` |
| Diagnostico previo | `artifacts/e2e/2026-04-17-empezar-e2e-full/DIAGNOSTICO.md` |
| Bodies de issues | `artifacts/e2e/2026-04-17-empezar-e2e-full/issues/H-001..H-013.md` |
