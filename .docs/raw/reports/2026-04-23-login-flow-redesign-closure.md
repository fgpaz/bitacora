# Login Flow Redesign — Reporte de Cierre 2026-04-23

**Fecha:** 2026-04-23
**Rama:** `feature/login-flow-redesign-2026-04-23`
**Base:** `main` @ `0bcb11e` → HEAD `3e96f0a` (+16 commits)
**Classification:** `ui-only, no-schema, no-contract, no-auth-edit`
**Compliance:** Ley 25.326 / 26.529 / 26.657 — sin cambios en storage, access control, consent enforcement backend, ni audit logging. ⚠ **R-P1-3 requiere legal-review del wording del CTA "Ahora no" y del mensaje de decline antes de deploy a prod.**

---

## 1. Cadena de commits

| # | Commit | Wave | Alcance |
|---|---|---|---|
| 1 | `25ece04` | W0 | Cherry-pick audit read-only (2026-04-23 baseline + reporte final) del branch `audit/login-flow-2026-04-23`. |
| 2 | `3955446` | W0 | Prompts fuente commited como inputs canónicos (ux-audit + redesign). |
| 3 | `44c6533` | W0 | Plan wave-dispatchable + 4 subdocumentos por wave. |
| 4 | `f72255c` | W1 | **R-P0-1** rail de acción con CTA `+ Nuevo registro` al top del dashboard ready (impeccable-distill). |
| 5 | `c8e331d` | W1 | **R-P0-2** ShellMenu nuevo con overflow menu ⋯ para proteger logout + adelanto de **R-P1-6** (Recordatorios + Vínculos en el mismo menu). |
| 6 | `749591f` | W1 | **R-P0-3** `app/global-error.tsx` nuevo con wordmark + copy canon 13 + CTA Recargar. |
| 7 | `44ccd8b` | W1 | **R-P0-4** cierre auto modal post-success (800ms) + puente embedded + toast `role=status aria-live=polite`. |
| 8 | `b989c81` | W2 | **R-P1-1** Server Component en `app/page.tsx` con `cookies()` de `next/headers` + variant `"returning"` + soporte `?declined=1`. **R-P1-2 hero** inviteLabel bajo h1. |
| 9 | `2f77bd8` | W2 | **R-P1-2 consent** inviteHint reubicado después del bloque `.sections`. |
| 10 | `3035ebc` | W2 | **R-P1-3** CTA secundario `"Ahora no"` + **R-P1-4** revocabilidad visible (`"Podés revocarlo cuando quieras desde Mi cuenta."`). ⚠ legal-flag. |
| 11 | `9ae14b9` | W2 | **R-P1-8** heading dashboard saludo contextual (`"Hola. Acá está lo que registraste."`) + subtítulo de privacidad. |
| 12 | `6c223fc` | W3 | **R-P1-5** DashboardSummary `variant=compact` en ready (texto corrido sobrio). |
| 13 | `87f8037` | W3 | **R-P1-7** focus ring canónico en `.primaryCta` y `.footerLink` del hero + **R-P1-9** trendChart colapsable en 360px (5 columnas + gap legible). |
| 14 | `15985d2` | W3 | **R-P1-10** `frontend/lib/errors/user-facing.ts` con `UserFacingError` + `formatUserFacingError()`; migrado err.message raw en OnboardingFlow, VinculosManager, BindingCodeForm, Dashboard, app/error.tsx. |
| 15 | `6b4e1d2` | W4 | Test ampliado de dashboard-modal (cierre auto + toast) + spec nuevo `logout-menu.spec.ts` (aria-expanded + Escape + items). |
| 16 | `3e96f0a` | W4 | Cleanup: untrack `frontend/test-results/` + agregar patterns al `.gitignore`. |

**Total: 16 commits**, 0 cruces en zonas congeladas, sin deps npm nuevas.

---

## 2. Checkpoints intermedios

| Checkpoint | Ubicación | Verificaciones | Resultado |
|------------|-----------|----------------|-----------|
| Pre-W1 | Post-Paso 3 (plan persistido) | 4 AskUserQuestion cerradas + Explorer 5 middleware feasibility (pivot a Server Component) | ✓ OK humano recibido ("si, continuar" + "hacer absolutamente todo") |
| Post-W1 | Tras commit `44ccd8b` | 8/8 e2e verde, typecheck + lint exit 0, 0 cruces zonas congeladas | ✓ PASS (integrado en la ejecución autónoma autorizada) |
| Post-W2 | Tras commit `9ae14b9` | 8/8 e2e verde, typecheck + lint exit 0 | ✓ PASS |
| Post-W3 | Tras commit `15985d2` | 8/8 e2e verde, typecheck + lint exit 0 | ✓ PASS |
| Post-W4 | Tras commit `3e96f0a` | 10/10 e2e verde (8 existentes + 2 nuevos), typecheck + lint exit 0, 0 cruces zonas congeladas | ✓ PASS |

Nota: el humano autorizó explícitamente la ejecución autónoma de las 4 waves sin pausas intermedias (mensaje "hacer absolutamente todo"), por lo que los checkpoints W1/W2/W3 se consolidaron en verificaciones técnicas automáticas sin pausa humana. El closure report final sí queda disponible para review antes de merge.

---

## 3. Verdict final

### Verificaciones de cierre

| Check | Resultado | Evidencia |
|-------|-----------|-----------|
| Governance `in_sync` | ✓ | mi-lsp workspace status bitacora (profile `spec_backend`, blocked `false`, sync `in_sync`) en Paso 0 |
| Zonas congeladas (`lib/auth/**`, `proxy.ts`, `app/api/**`, `app/auth/**`, `src/`) | ✓ 0 cruces | `git diff --name-only main..HEAD \| grep -E "^(frontend/lib/auth/\|frontend/app/api/\|frontend/app/auth/\|frontend/proxy\.ts\|frontend/src/)"` → sin matches |
| Backend / schema (.cs, 05_modelo_datos, 08_*, 09_*) | ✓ 0 cruces | Diff no contiene `.cs` ni docs de schema |
| Dependencias npm agregadas | ✓ 0 nuevas | `package.json` inalterado (`next ^16.0.0`, `react ^19.0.0`, `react-dom ^19.0.0`) |
| Copy congelado preservado | ✓ | 13 strings verificados — `"Ingresar"`, `"Tu espacio personal de registro"`, `"Solo vos ves lo que registrás. Tus datos son privados."`, `"Registrar humor"`, `"Empezá con tu primer registro"`, `"+ Nuevo registro"`, `"Check-in diario"`, `"Registro guardado."`, `"Tus últimos días"`, `"Recibí recordatorios por Telegram"`, `"Conectar"`, `"Ahora no"`, `"Nuevo registro"`. **Excepción autorizada**: h1 dashboard migrado a `"Hola. Acá está lo que registraste."` con decisión humana AskUserQuestion Q2 |
| Tests Playwright | ✓ 10/10 verdes | Última corrida 2026-04-23 (ver §9) |
| typecheck / lint | ✓ exit 0 | `tsc --noEmit` + `eslint .` ambos exit 0 post-cada-wave |
| 12 positivos del closure 2026-04-22 preservados | ✓ | Verificado por Explorer 2 (reality check modal/forms); ninguna regresión introducida |

**Verdict final:** `needs-redesign` → **resuelto** para el dashboard como hub del recurrente. `needs-refinement` → **resuelto** para landing, consent, onboarding, modal, shell, edge. 4/4 P0 cerrados, 10/10 P1 priorizados cerrados (incluidos R-P1-1, R-P1-2, R-P1-3 con flag legal, R-P1-4, R-P1-5, R-P1-6 adelantado en W1, R-P1-7, R-P1-8, R-P1-9, R-P1-10).

---

## 4. Sync canon wiki

**No se tocó canon `.docs/wiki/` en esta corrida.** Los cambios son UI-only. El canon 10/12/13/16 vigente sigue aplicando sin modificaciones. Los artefactos `UJ-ONB-*`, `UXS-ONB-*`, `VOICE-ONB-*` para el flujo login+dashboard+modal siguen ausentes en `.docs/wiki/23_uxui/` (gap ya documentado en el audit §6, follow-up explícito).

---

## 5. Resumen por dimensión impeccable

### `impeccable-distill` (Wave 1 T1A, Wave 3 T3A)
- Rail de acción al top del dashboard ready: CTA `"+ Nuevo registro"` dominante + `"Check-in diario"` secundario silencioso (canon 12 §"una acción dominante + secundaria silenciosa").
- DashboardSummary `variant=compact` en ready: 3 stat cards → texto corrido sobrio (`"Llevás N registros. El último, hace X días."`). Canon 10 §12.2 "evitar tableros de vigilancia".

### `impeccable-harden` (Wave 1 T1B/T1C/T1D, Wave 2 T2C, Wave 3 T3E)
- ShellMenu nuevo con overflow `⋯`: 2 taps deliberados para logout (aria-haspopup/expanded + Esc + click-outside).
- `app/global-error.tsx` creado con wordmark Bitácora + copy canon 13 + CTA Recargar.
- Cierre automático del modal post-success (800ms) + toast post-cierre.
- CTA `"Ahora no"` en consent con redirect `/?declined=1` sin borrar cookie (Ley 26.529 Art. 2).
- `UserFacingError` + `formatUserFacingError()` en `lib/errors/user-facing.ts`.

### `impeccable-clarify` (Wave 1 T1D, Wave 2 T2D, Wave 3 T3E)
- Puente embedded del modal success (`"Volviendo al dashboard…"`).
- Mensaje post-decline en landing (`"Podés aceptar cuando quieras. Tu sesión sigue activa."` vía role=status aria-live=polite).
- Nota de revocabilidad en consent (`"Podés revocarlo cuando quieras desde Mi cuenta."`).
- Errores canon 13 en OnboardingFlow, VinculosManager, BindingCodeForm, Dashboard, app/error.tsx (sin err.message raw).

### `impeccable-onboard` (Wave 1 T1B, Wave 2 T2A/T2B/T2E)
- OnboardingEntryHero variant `"returning"` con h1 `"Volviste."` + sub `"Seguí donde dejaste."` + CTA `"Seguir registrando"` → `/dashboard`.
- inviteHint reubicado después de sections; inviteLabel bajo h1 (canon 10 §10.1 — no instalar vigilancia antes que control).
- h1 dashboard saludo contextual + subtítulo privacidad (frame de refugio antes que archivo).
- Recordatorios + Vínculos visibles en ShellMenu (R-P1-6 adelanto en W1).

### `impeccable-normalize` (Wave 3 T3C)
- `:focus-visible` canónico con `outline: 2px solid var(--brand-primary); outline-offset: 2px; box-shadow: var(--focus-ring)` aplicado a `.primaryCta` y `.footerLink` del hero. Patron espejo del closure 2026-04-22.

### `impeccable-adapt` (Wave 3 T3D)
- trendChart en `@media (max-width: 399px)` → 5 columnas con gap `var(--space-sm)` + `nth-child(n+6) { display: none }`. Solución CSS-only, sin hooks.

---

## 6. Follow-ups (no bloqueantes)

1. **⚠ Legal-review obligatorio pre-deploy de R-P1-3** — el wording de `"Ahora no"` y del mensaje `"Podés aceptar cuando quieras. Tu sesión sigue activa."` debe ser validado por el equipo legal antes de push a producción para cumplimiento explícito de Ley 26.529 Art. 2. Hard gate funcional de RF-CON-003 preservado (sin consent no hay registro).
2. **Sync canon 23_uxui por slice** — generar `UJ-ONB-*`, `UXS-ONB-*`, `VOICE-ONB-*` para el flujo login+dashboard+modal. Gap ya identificado en audit §6. No bloqueante pero mejora trazabilidad. Requiere `crear-journey-ux` + `crear-spec-ux` + `crear-spec-voz-tono`.
3. **Analytics / telemetría** (fuera de scope): instrumentar `time-to-CTA` (desde /dashboard ready hasta openDialog), `CTR` del rail superior vs Check-in diario, y `rate de logout accidental` pre vs post-rediseño. Handoff a owner analytics.
4. **PatientPageShell prop `error` tipada como `UserFacingError`** — el plan T3E proponía tipado estricto; se decidió mantener `string | null` por compatibilidad con el único callsite (OnboardingFlow) que produce strings canon-correctos. Follow-up explícito para refactor future.
5. **Focus ring en `ProfessionalShell` y `SaveRail`** — 2 módulos CSS del flujo profesional con `outline` sin `var(--focus-ring)` detectados por Explorer 4. Fuera de scope paciente de esta corrida. Follow-up para wave normalize global.
6. **Migrar h1 del canon si corresponde** — el h1 del dashboard migró a `"Hola. Acá está lo que registraste."` con decisión humana explícita (Q2). El audit maestro §4.2 había propuesto tres caminos; se eligió (c) saludo contextual. Si el equipo decide actualizar el canon 2026-04-22 para reflejar el cambio, hacerlo en `crear-voz-tono` / doc decisión dedicado.
7. **global-error test** — se decidió NO agregar `global-error.spec.ts` en W4 porque forzar un throw desde Playwright requeriría infra adicional. Follow-up opcional para wave de QA.

---

## 7. Deuda explícitamente aceptada

1. **DashboardSummary `variant=cards` preservado** para compatibilidad con vista profesional y llamadas futuras; no se refactoriza el componente para eliminar el variant no-usado en paciente.
2. **Server Component vs Middleware** — se optó por Server Component en `app/page.tsx` para no tocar `proxy.ts` (zona congelada). Si el equipo auth habilita tocar `proxy.ts` en el futuro, se puede migrar la detección al middleware server-side sin cambios de UX perceptibles (la detección sigue ocurriendo server-side en ambos casos).
3. **Cookie name hardcoded** como string literal `'bitacora_session'` en `app/page.tsx` para evitar import desde `lib/auth/constants.ts` (zona congelada). Si la constante cambia upstream, hay que sincronizar manualmente (bajo riesgo: es constante muy estable).
4. **PatientPageShell error prop string** — ver follow-up 4.

---

## 8. Compliance health data (Ley 25.326 / 26.529 / 26.657)

- **Storage**: inalterado.
- **Access control**: inalterado. `proxy.ts` sigue gestionando rutas protegidas; la detección de sesión en `app/page.tsx` es lectura observacional sin mutación.
- **Consent enforcement backend**: inalterado. `RF-CON-003` hard gate preservado.
- **Audit logging**: inalterado.
- **Data minimization**: cookie `bitacora_session` solo leída para `has()`, no parseada ni decodificada en Server Component.
- **⚠ Consent UX (Ley 26.529 Art. 2)**: R-P1-3 introduce CTA `"Ahora no"` que DEBE pasar review legal antes de deploy. El wording actual es propuesta técnica/UX; la validación legal del texto exacto y del mensaje post-decline es bloqueante para prod.

---

## 9. Tests Playwright

### Specs en el feature branch

| Spec | Tests | Estado | Cambios |
|------|-------|--------|---------|
| `landing.spec.ts` | 3 | ✓ Verde | Sin cambios (selectors text-based toleran Server Component + variant) |
| `dashboard-modal.spec.ts` | 2 | ✓ Verde | Ampliado: assertion cierre auto modal + toast canon 13 |
| `telegram-banner.spec.ts` | 3 | ✓ Verde | Sin cambios |
| `logout-menu.spec.ts` (nuevo) | 2 | ✓ Verde | Nuevo spec: aria-expanded, items Recordatorios + Vínculos + Cerrar sesión, Escape cierra |
| **TOTAL** | **10** | **10/10 verdes** | +2 tests nuevos |

Última corrida: 2026-04-23, 10/10 passed en ~12s.

---

## 10. Estado del repo al cierre

```
Rama: feature/login-flow-redesign-2026-04-23
Base: main @ 0bcb11e
HEAD: 3e96f0a
Commits: +16 (3 docs W0 + 4 W1 + 4 W2 + 3 W3 + 2 W4)
git status --short: limpio (tras untrack de test-results/)
typecheck: exit 0
lint: exit 0
e2e: 10/10 verdes
Zonas congeladas: 0 cruces
Copy congelado: preservado (1 excepción autorizada por humano)
Dependencies: sin cambios (next ^16, react ^19, react-dom ^19)
```

### Métricas de archivos tocados

- **Archivos modificados**: 15
  - `frontend/app/page.tsx` (Server Component + variant returning)
  - `frontend/app/error.tsx` (sub canon 13)
  - `frontend/app/(patient)/dashboard/page.tsx` (h1 saludo contextual)
  - `frontend/app/(patient)/dashboard/DashboardPage.module.css` (subtitle + clamp)
  - `frontend/components/patient/dashboard/Dashboard.tsx` (rail top + toast)
  - `frontend/components/patient/dashboard/Dashboard.module.css` (actionRail + toast + trendChart adapt)
  - `frontend/components/patient/dashboard/DashboardSummary.tsx` (variant compact)
  - `frontend/components/patient/dashboard/DashboardSummary.module.css` (.compactSummary)
  - `frontend/components/patient/dashboard/MoodEntryDialog.tsx` (cierre auto post-save)
  - `frontend/components/patient/mood/MoodEntryForm.tsx` (puente embedded)
  - `frontend/components/patient/mood/MoodEntryForm.module.css` (.embeddedBridge)
  - `frontend/components/patient/consent/ConsentGatePanel.tsx` (CTA Ahora no + revocabilidad + inviteHint reubicado)
  - `frontend/components/patient/consent/ConsentGatePanel.module.css` (.declineBtn + .revocationNote + mobile stack)
  - `frontend/components/patient/onboarding/OnboardingEntryHero.tsx` (variant returning + message + inviteLabel bajo h1)
  - `frontend/components/patient/onboarding/OnboardingEntryHero.module.css` (.heroMessage + :focus-visible)
  - `frontend/components/patient/onboarding/OnboardingFlow.tsx` (err.message eliminado, copy canon 13)
  - `frontend/components/patient/vinculos/VinculosManager.tsx` (err.message eliminado)
  - `frontend/components/patient/vinculos/BindingCodeForm.tsx` (err.message eliminado)
  - `frontend/components/ui/PatientPageShell.tsx` (shellHeader + ShellMenu)
  - `frontend/components/ui/PatientPageShell.module.css` (shellHeader layout)
  - `frontend/e2e/dashboard-modal.spec.ts` (assertion cierre + toast)
  - `.gitignore` (patterns test-results)

- **Archivos nuevos**: 6
  - `frontend/app/global-error.tsx`
  - `frontend/components/ui/ShellMenu.tsx`
  - `frontend/components/ui/ShellMenu.module.css`
  - `frontend/lib/errors/user-facing.ts`
  - `frontend/e2e/logout-menu.spec.ts`
  - `.docs/raw/plans/2026-04-23-login-flow-redesign.md` + 4 subdocumentos W1/W2/W3/W4

---

## 11. Recomendación para merge

1. **Review humano del branch** + closure report (este documento).
2. **Coordinar legal-review de R-P1-3** (wording CTA "Ahora no" + mensaje decline). Bloqueante para deploy a prod.
3. **PR a main** con `git merge --no-ff` o `gh pr create` preservando historial de waves.
4. **Deploy a Dokploy** post-OK legal.
5. **Sesión follow-up** opcional para:
   - Canon 23_uxui por slice (UJ-ONB-* / UXS-ONB-* / VOICE-ONB-*).
   - Analytics del rediseño (time-to-CTA + rate logout accidental pre/post).
   - Migrar `PatientPageShell error` prop a `UserFacingError` strict.
   - Focus ring normalize en `ProfessionalShell` + `SaveRail`.

**NO mergear automáticamente**. La rama queda para PR humano, consistente con el patrón del hardening 2026-04-22.

---

## 12. Última palabra

Este rediseño responde la queja literal del product owner: la UX de login para el recurrente era malísima. Ahora:
- El recurrente con cookie viva ve un hero de retorno server-rendered sin flash (`"Volviste." + "Seguir registrando"`).
- Al entrar al dashboard, el CTA `"+ Nuevo registro"` está en el viewport inicial (no tras 600-700px de scroll).
- El logout requiere 2 taps deliberados.
- El modal cierra automáticamente tras guardar con un toast que confirma `"Registro sumado a tu historial."`.
- Errores catastróficos ya no son pantalla cruda runtime, son refugio con wordmark + CTA Recargar.
- Los mensajes de error no exponen `err.message` raw (canon 13).
- El consent ofrece salida respetuosa (`"Ahora no"`) con revocabilidad visible (Ley 26.529 Art. 2).

Si el paciente en vulnerabilidad abre el dashboard a las 23:47 en mobile cansado, ahora ve la acción que importa. Si se arrepiente del consent, tiene salida sin romper su sesión. Si todo colapsa, ve Bitácora cuidándolo.

---

*Reporte de cierre del rediseño login flow 2026-04-23. Audit fuente: `.docs/raw/reports/2026-04-23-login-flow-audit.md`. Plan: `.docs/raw/plans/2026-04-23-login-flow-redesign.md`. Shape de referencia: `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md`.*
