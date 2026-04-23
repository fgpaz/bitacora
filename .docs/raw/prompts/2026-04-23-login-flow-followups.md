<!--
target_platform: claude-code
pressure: aggressive
generated_at: 2026-04-23
copy_via: pwsh -NoProfile -Command "Get-Content -Raw -Encoding UTF8 '.docs/raw/prompts/2026-04-23-login-flow-followups.md' | Set-Clipboard"
-->

# MISIÓN — Cerrar los 6 follow-ups del rediseño del login flow de Bitácora

Operás en `C:\repos\mios\humor`. El rediseño P0+P1 del flujo de acceso se mergeó a `main` y se pusheó a `origin/main` el 2026-04-23 (commit merge `5d91158`, feature branch original `feature/login-flow-redesign-2026-04-23`). **El deploy a prod salió.** Esta sesión cierra los 6 follow-ups que quedaron documentados en `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md` §6.

No confíes en memoria de conversaciones previas. Verificá repo state antes de creer nada de este prompt.

## Punto de partida

- Base: `main` en el commit que resulte al hacer `git fetch origin && git rev-parse origin/main`. La sesión previa dejó `main @ 5d91158`. Si hay commits posteriores, tratarlos como dados y NO reabrirlos.
- Rama de trabajo sugerida: `feature/login-flow-followups-2026-04-23`. Si surge conflicto con convenciones, adaptá.
- Artefactos canónicos (leer primero, no opinar sin ellos):
  - `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md` — closure maestro con la tabla exacta de follow-ups (§6) y deuda aceptada (§7). Documento de entrada obligatoria.
  - `.docs/raw/reports/2026-04-23-login-flow-audit.md` — audit original con 36 hallazgos y evidencia file:line.
  - `.docs/raw/plans/2026-04-23-login-flow-redesign.md` + subdocs `W1-W4` — plan ejecutado.
  - `AGENTS.md`, `CLAUDE.md` — política operativa actual.

## Scope — los 6 follow-ups (priorizados)

### 1. **🔒 LEGAL-REVIEW R-P1-3** (bloqueante de compliance)

**Estado:** el CTA secundario `"Ahora no"` y el mensaje post-decline `"Podés aceptar cuando quieras. Tu sesión sigue activa."` están en producción SIN validación legal. Cumplimiento Ley 26.529 Art. 2 (autonomía informada) requiere ajuste si el equipo legal lo pide.

**Archivos a tocar si el abogado devuelve wording alternativo:**
- `frontend/components/patient/consent/ConsentGatePanel.tsx` (CTA `.declineBtn` + `handleDecline`).
- `frontend/app/page.tsx` (mensaje post-decline via `message` prop del hero).
- `frontend/components/patient/onboarding/OnboardingEntryHero.tsx` (render del `message`).

**Workflow:**
- Si el legal-review ya vino con OK sin cambios → marcar `R-P1-3` como cerrado en un nuevo closure update y NO tocar código.
- Si vino con wording alternativo → commit quirúrgico + test e2e pasa + closure update.
- Si el legal-review aún no vino → **bloquear esta tarea** y avanzar al resto. Documentar explícitamente el bloqueo.

**Preguntá al humano al inicio** (vía `AskUserQuestion`): ¿el legal-review ya vino? ¿trae cambios? ¿se posterga? Sin respuesta, la tarea queda en "BLOCKED — pendiente legal" y seguís con los otros 5.

### 2. **Canon 23_uxui por slice** (trazabilidad documental)

**Gap:** no existen `UJ-ONB-*`, `UXS-ONB-*`, `VOICE-ONB-*` para el flujo login+dashboard+modal que acaba de rediseñarse. El audit §6 y el closure §6.2 lo flaggean como gap de traceability no bloqueante pero importante para cerrar el ciclo spec-driven.

**Workflow obligatorio:**
1. Invocar `Skill("ps-asistente-wiki")` primero para diagnosticar en qué fase está el canon 23_uxui y qué skill exacto toca (`crear-journey-ux`, `crear-spec-ux`, `crear-spec-voz-tono`).
2. Seguir la guía que devuelve ps-asistente-wiki. NO asumas paths ni nombres de archivo — el asistente te dice.
3. Generar los artefactos canónicos del slice (esperable: un UJ-ONB-0XX cubriendo el journey landing → ingresar → consent → dashboard + modal, más UXS por pantalla crítica + VOICE para consent+dashboard).
4. Docs-only. No tocar código.

### 3. **Analytics / telemetría del rediseño** (medición del impacto)

**Objetivo:** instrumentar métricas que validen el verdict del rediseño respondiendo la queja del PO.

**Mínimo:**
- `time_to_cta_ready` — tiempo desde que el dashboard ready termina de renderizar hasta que `openDialog()` se dispara. Proxy del dolor "no queda claro cómo registra algo nuevo".
- `ctr_rail_vs_checkin` — CTR comparado del CTA `"+ Nuevo registro"` vs `"Check-in diario"` en el rail superior.
- `logout_accidental_rate` — sesiones cerradas con <3 minutos de uso post-dashboard-ready. Proxy de la queja E3-F6.
- `decline_consent_rate` — CTR del `"Ahora no"` vs `"Aceptar y continuar"` en consent (útil también para el legal-review).

**Restricción:** **NO agregar dependencias npm nuevas** (stack actual: 3 prod deps). Si no hay librería de analytics ya en el ecosistema, implementar hook simple que postea a un endpoint interno existente o marcar como "requiere decisión de infra" y escalar al humano.

**Archivos candidatos:**
- `frontend/lib/analytics/track.ts` (nuevo si no existe).
- `frontend/components/patient/dashboard/Dashboard.tsx` (emitir `time_to_cta_ready` + `ctr_rail_vs_checkin`).
- `frontend/components/ui/ShellMenu.tsx` + `PatientPageShell.tsx` (emitir `logout_accidental_rate`).
- `frontend/components/patient/consent/ConsentGatePanel.tsx` (emitir `decline_consent_rate`).

**Workflow:** decisión previa con `AskUserQuestion` sobre endpoint/backend para analytics (existe un `/api/analytics` o similar? hay vendor ya configurado? o se deja solo el tracking en local storage como spike?). Sin endpoint confirmado, implementar como "tracking stub" con `console.info` + estructura lista para endpoint futuro, y marcar como P2 en closure.

### 4. **`PatientPageShell` prop `error` strict a `UserFacingError`** (contrato estricto)

**Estado actual:** `error?: string | null`. El closure §6.4 flagged como deuda. Único callsite vivo: `OnboardingFlow.tsx:122`.

**Cambio:**
- Migrar interfaz de `PatientPageShell` a `error?: UserFacingError | null`.
- Render del error pasa a usar `error.title` + `error.description` + botón `error.retry` opcional.
- Actualizar `OnboardingFlow.tsx` para construir `UserFacingError` en los 3 setError de error branches (ya existen con `formatUserFacingError()` en `lib/errors/user-facing.ts`).
- Agregar test e2e si corresponde.

**Restricción:** mantener retrocompatibilidad — si algún callsite pasa string, no romper en runtime (rechaza en compile-time con TS, lo cual es esperable).

### 5. **Focus ring normalize en `ProfessionalShell` + `SaveRail`** (consistencia a11y global)

**Estado (Explorer 4 del prompt original):**
- `components/ui/ProfessionalShell.module.css:50-76` usa `outline: 2px solid var(--surface)` sin `var(--focus-ring)`.
- `components/ui/SaveRail.module.css:32-33` usa `outline: 2px solid var(--brand-primary)` sin `var(--focus-ring)`.
- `components/patient/vinculos/VinculosForm.module.css:34` y `components/professional/InviteForm.module.css:29` tienen `outline: none` suelto.

**Cambio:**
- Aplicar el patrón canónico del closure 2026-04-22:
  ```css
  :focus-visible {
    outline: 2px solid var(--brand-primary);
    outline-offset: 2px;
    box-shadow: var(--focus-ring);
  }
  ```
- Eliminar `outline: none` sueltos que no tengan reemplazo `:focus-visible` cercano.
- Grep en `frontend/` para detectar otros gaps.

**Scope:** limitar a archivos con gap confirmado. No tocar zonas congeladas (`lib/auth/*`, `app/api/*`, `app/auth/*`, `proxy.ts`, `src/**`).

### 6. **`global-error.spec.ts`** (coverage e2e — opcional)

**Gap:** `app/global-error.tsx` se creó en Wave 1 (commit `749591f`) pero no tiene e2e. Forzar un throw desde Playwright requiere infra adicional (`throw new Error("...")` en el root layout vía navegación intencional).

**Decisión:**
- Si la infra es simple → agregar spec minimal que verifique wordmark + título canon 13 + CTA Recargar.
- Si requiere hackear layout de test → dejar como P3 follow-up y documentarlo en closure update.

**Esta es opcional, no bloquea cierre de la sesión.**

## Invariantes protegidos (violación = abort)

- **Zonas congeladas NO TOCAR**: `frontend/lib/auth/*`, `frontend/app/api/*`, `frontend/app/auth/*`, `frontend/proxy.ts`, `frontend/src/**`. Grep final de cada wave debe devolver 0 cruces.
- **Copy congelado sigue siendo ley** (closure §3 lo lista): `"Ingresar"`, `"Tu espacio personal de registro"`, `"Solo vos ves lo que registrás. Tus datos son privados."`, `"Registrar humor"`, `"Empezá con tu primer registro"`, `"+ Nuevo registro"`, `"Check-in diario"`, `"Registro guardado."`, `"Tus últimos días"`, `"Recibí recordatorios por Telegram"`, `"Conectar"`, `"Ahora no"`, `"Nuevo registro"`, `"Volviste."`, `"Seguir registrando"`, `"Hola. Acá está lo que registraste."`. Cualquier cambio requiere decisión humana explícita. **R-P1-3 puede cambiar el wording de `"Ahora no"` y del mensaje post-decline solo si el abogado lo pide.**
- **Compliance salud mental**: Ley 25.326 / 26.529 / 26.657. Cualquier cambio en consent flow o storage requiere legal-review.
- **Dependencias npm**: 3 prod deps actuales (next, react, react-dom). NO agregar ninguna. Si analytics necesita lib, escalar al humano.
- **12 positivos del closure 2026-04-22** preservados + **nuevos invariantes del rediseño 2026-04-23**: `ShellMenu` con aria-haspopup/aria-expanded + Esc + click-outside, modal cierra auto 800ms post-success, toast `role=status aria-live=polite`, `global-error.tsx` con wordmark, Server Component en `app/page.tsx` con variant returning, consent con CTA secundario + revocabilidad, DashboardSummary variant compact. Grep/test verifica.
- **Git hygiene**: NO `taskkill`, NO `--no-verify`, NO `--force push`, NO `--amend` sobre commits empujados. Cada wave = 1 o más commits atómicos con trailer `- Gabriel Paz -`. NO mergear a `main` sin OK humano.

## Paso 0 — Anclaje + verificación

1. **Cargar contexto obligatorio** — `Skill("ps-contexto")`. Scope, arquitectura, flujos, modelo de datos, canon UX/UI 10-23, canon técnico 07-09.
2. **Leer con Read directo** (no delegar):
   - `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md` (documento maestro de follow-ups).
   - `.docs/raw/reports/2026-04-23-login-flow-audit.md` (hallazgos originales con file:line).
   - `.docs/raw/plans/2026-04-23-login-flow-redesign.md` + subdocs (patrón de referencia).
   - `.docs/wiki/10_manifiesto_marca_experiencia.md`, `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md`, `16_patrones_ui.md`.
   - `frontend/lib/errors/user-facing.ts` (fuente del contrato `UserFacingError`).
   - `frontend/components/ui/PatientPageShell.tsx` (caller a migrar).
   - `frontend/components/ui/ProfessionalShell.module.css` + `SaveRail.module.css` (gaps a cerrar).
   - `AGENTS.md`, `CLAUDE.md`.
3. **Validar workspace mi-lsp** — `mi-lsp workspace list` → alias `bitacora`. Si devuelve `hint` o `next_hint`, seguirlos antes de avanzar. `mi-lsp` es la herramienta principal de navegación para todo lo que cae bajo `frontend/` y `src/`.
4. **Verificar estado git** — `git fetch origin && git log --oneline origin/main -5`. Confirmar que `5d91158` (merge de login-flow-redesign) está presente. Si hay commits posteriores, tratarlos como dados y verificar que no rompen este scope.
5. **Crear rama**: `git checkout main && git pull && git checkout -b feature/login-flow-followups-2026-04-23`.

## Paso 1 — Exploración obligatoria (mínimo 3 `ps-explorer`, hasta 5)

Despachá en UN SOLO mensaje, no secuencial. Scope multi-dominio (docs 23_uxui + frontend components + tests) justifica 4-5 explorers.

**Explorer 1 — Legal-review R-P1-3 reality check**
Leer `frontend/components/patient/consent/ConsentGatePanel.tsx` y confirmar que el CTA `.declineBtn` + `handleDecline` + `handleAccept` + `revocationNote` siguen exactos a la documentación del closure. Confirmar el mensaje post-decline renderiza desde `frontend/app/page.tsx` → `OnboardingEntryHero.tsx` prop `message` + `.heroMessage` CSS. Reportar file:line + copy literal. Confirmar que los tests e2e relevantes no dependen del wording actual (si sí, listarlos).

**Explorer 2 — Estado del canon 23_uxui + ps-asistente-wiki**
Hacer `Skill("ps-asistente-wiki")` y reportar qué devuelve: qué fase detecta, qué skill recomienda, qué paths sugiere. Adicionalmente, hacer `Glob` sobre `.docs/wiki/23_uxui/**` para inventariar qué artefactos existen y cuáles faltan para el slice login+dashboard+modal. Reportar con árbol de archivos + contenido parcial de INDEX.md si existe.

**Explorer 3 — Analytics feasibility**
Leer `frontend/lib/api/client.ts` (si existe) y `frontend/proxy.ts` (NO MODIFICAR, solo leer para entender el patrón de endpoints). Buscar endpoints existentes que soporten analytics (`/api/analytics`, `/api/events`, etc.). Grep en `frontend/` de `track|analytics|telemetry|mixpanel|amplitude|plausible|datadog|sentry` para detectar infra ya instalada. Reportar: hay endpoint listo sí/no, hay vendor configurado sí/no, riesgo de tocar zona congelada sí/no.

**Explorer 4 — PatientPageShell migration + focus ring global audit**
Leer `frontend/components/ui/PatientPageShell.tsx` + `PatientPageShell.module.css`. Identificar el render actual de `error` branch. Leer `frontend/components/patient/onboarding/OnboardingFlow.tsx` para confirmar el único callsite activo y cómo construye el `string` hoy. Adicionalmente, grep `outline:` en `frontend/**/*.module.css` para inventario completo de focus-ring gaps (closure §6.5 mencionó 2-4 sitios; confirmar la lista exacta hoy). Reportar paths + line numbers + snippet del outline suelto.

**Explorer 5 — global-error.tsx test feasibility (condicional)**
Solo lanzar si el humano confirma que quiere abordar follow-up #6. Investigar cómo forzar un error en root layout desde Playwright. Ver si `playwright.config.ts` o helpers existentes soportan inyectar un throw. Reportar: feasible con infra actual sí/no, complejidad estimada.

Cada explorer: máximo 10 puntos, file:line + snippet literal, cross-check antes de integrar. Sin evidencia file:line → descartar el hallazgo.

## Paso 2 — Brainstorming + AskUserQuestion (decisiones críticas)

`Skill("brainstorming")`. Usar `AskUserQuestion` con el protocolo completo (contexto + impacto + diagrama ASCII + pros/cons + recomendación) para cerrar al menos estas decisiones:

1. **🔒 Legal-review R-P1-3**: ¿el abogado ya devolvió? ¿wording alternativo o OK sin cambios? ¿o la tarea queda bloqueada?
2. **Analytics endpoint**: ¿usar `/api/analytics` existente (si el Explorer 3 lo confirma), implementar stub con `console.info` + estructura lista, o escalar decisión de infra a otro sprint?
3. **Canon 23_uxui alcance**: ¿generar UJ único cubriendo el journey completo o separar en UJ-ONB-001 (landing→ingresar) + UJ-ONB-002 (onboarding→consent) + UJ-ONB-003 (dashboard+modal)? Decisión depende del output de `ps-asistente-wiki`.
4. **Follow-up #6 (global-error.spec.ts)**: ¿abordar en esta sesión o aplazar a P3?

Cada decisión sensible adicional debe marcarse `🔒 needs-human-decision` y pausarse.

## Paso 3 — Plan wave-dispatchable

`Skill("writing-plans")`. Persistir a `.docs/raw/plans/2026-04-23-login-flow-followups.md` + subdocumentos por wave en `.docs/raw/plans/2026-04-23-login-flow-followups/`.

Estructura sugerida (ajustar según las decisiones del brainstorming):

- **W1 — Canon 23_uxui** (docs-only, no toca código; puede ir primero o en paralelo con otras)
- **W2 — Fix R-P1-3 wording** (condicional a legal-review; skip si OK sin cambios)
- **W3 — PatientPageShell strict + focus ring normalize** (dos tareas de code quality en paralelo)
- **W4 — Analytics stub o implementación** (depende del brainstorming)
- **W5 — global-error.spec.ts** (opcional; solo si follow-up #6 se aprueba)
- **W6 — Tests + cierre** (typecheck + lint + e2e + ps-trazabilidad + ps-auditar-trazabilidad + closure update)

Shape de referencia: `.docs/raw/plans/2026-04-23-login-flow-redesign.md` + subdocs (patrón ejecutado exitosamente esta semana).

## Paso 4 — Ejecución por wave

Cada wave:
1. Mostrar al humano el diff propuesto antes de aplicar cambios grandes (>5 archivos).
2. Dispatch de subagentes cuando la wave se pueda paralelizar:
   - `ps-next-vercel` para código Next.js / React.
   - `ps-docs` para canon 23_uxui (docs wiki).
   - `ps-worker` para ajustes de config, scripts, git.
   - `ps-code-reviewer` al final de cada wave para audit P>D>S.
3. Correr tests relevantes: typecheck + lint + e2e subset afectado.
4. Grep final de zonas congeladas (0 cruces obligatorio): `lib/auth/|app/api/|app/auth/|proxy\.ts|^frontend/src/`.
5. Commit atómico con trailer `- Gabriel Paz -`.
6. **CHECKPOINT humano** al final de cada wave: pausa + resumen + espera OK antes de continuar.

## Paso 5 — Cierre

1. `Skill("ps-trazabilidad")` final — full sync RF/FL/canon/tests. Verificar que los artefactos 23_uxui nuevos referencian los findings del audit 2026-04-23 y los commits de esta sesión.
2. `Skill("ps-auditar-trazabilidad")` full — cross-check closure previo (2026-04-23) + plan + commits + canon.
3. typecheck + lint + 10/10+ e2e verdes obligatorio.
4. Escribir closure update: `.docs/raw/reports/2026-04-23-login-flow-followups-closure.md` con shape similar al closure previo. Debe listar cada follow-up con estado `resuelto` / `parcial` / `bloqueado` / `postponed`.
5. **No mergear a main sin OK humano**. La rama queda para PR humano.
6. Devolver al humano: path del closure, conteo de commits, verdict por follow-up, próxima acción.

## Reglas de operación

- **Orchestrator mode siempre**. Exploración + implementación por wave van en subagentes. El hilo principal sintetiza, decide y commitea.
- **Cross-check ≥ 2 fuentes** para cualquier finding antes de tocar código.
- **Evidencia o no existe**. Si un subagent reporta sin file:line, pedilo de vuelta o descartá el hallazgo.
- **Timeboxing flexible**. Si una wave se estira, cortá y pedí OK humano para splittear.
- **Si algo queda inconcluso**, closure honesto con "no validado: X, Y" > inventar verdicts.
- **Si un follow-up destapa algo mayor** no documentado en el closure previo, NO lo absorbas: reportá, abrí card, seguí con el scope original.

## Referencias

- `CLAUDE.md` (raíz) — política operativa completa.
- `AGENTS.md` (raíz) — catálogo de subagents.
- `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md` — documento maestro (LEER PRIMERO).
- `.docs/raw/reports/2026-04-23-login-flow-audit.md` — audit original con file:line.
- `.docs/raw/plans/2026-04-23-login-flow-redesign.md` + subdocs — shape de referencia.
- `.docs/wiki/10-16` — canon UX/UI endurecido.
- `.docs/wiki/23_uxui/INDEX.md` (si existe) — para canon 23_uxui.
- `frontend/lib/errors/user-facing.ts` — contrato `UserFacingError`.
- `frontend/components/ui/PatientPageShell.tsx` — caller a migrar.
- `frontend/components/patient/consent/ConsentGatePanel.tsx` — consent con CTA Ahora no.

## Última palabra

Los 6 follow-ups son deuda explícita documentada del rediseño que se deployó el 2026-04-23. No son ambiciones; son gap known de compliance, trazabilidad canon, medición del impacto, y consistencia de código. Cerrarlos en orden de prioridad:

1. **Legal-review R-P1-3 es bloqueante de compliance** — si el abogado devolvió con cambios, es lo primero.
2. **Canon 23_uxui** — la trazabilidad del spec-driven model depende de que exista el documento que refleja el slice implementado.
3. **Analytics** — sin métricas el equipo no puede validar que el rediseño respondió la queja del PO.
4. **Tipado estricto + focus ring** — code quality acumulada que se paga en consistencia.
5. **global-error.spec** — nice-to-have si la infra lo permite.

Empezá ya. Paso 0 primero. Nada de preámbulos.
