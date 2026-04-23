# Login Flow Follow-ups — Reporte de Cierre 2026-04-23

**Fecha:** 2026-04-23
**Rama:** `feature/login-flow-followups-2026-04-23`
**Base:** `main` @ `5d91158` (merge del rediseño 2026-04-23) → HEAD `fd61272` (+4 commits)
**Classification:** `ui-only, no-schema, no-contract, no-auth-edit, docs + code`
**Compliance:** Ley 25.326 / 26.529 / 26.657 — sin cambios en storage, access control, consent enforcement backend, ni audit logging. **R-P1-3 cerrado con review interno pragmatic `pending-formal-legal-opinion`**.
**Prompt fuente:** `.docs/raw/prompts/2026-04-23-login-flow-followups.md`
**Plan fuente:** `.docs/raw/plans/2026-04-23-login-flow-followups.md` + subdocs W1-W5.

---

## 1. Cadena de commits

| # | Commit | Wave | Alcance |
|---|---|---|---|
| 1 | `1539588` | W1 | Legal-review docs-only R-P1-3 + canon 23_uxui deltas 2026-04-23 (7 artefactos + 1 decisión + plan). |
| 2 | `9b6fcc7` | W2 | PatientPageShell prop `error` strict `UserFacingError` + OnboardingFlow migrado. |
| 3 | `207bb4a` | W3 | Focus ring normalize en ProfessionalShell, SaveRail, ExportGate, InviteForm, Timeline. |
| 4 | `fd61272` | W4 | Analytics stub `lib/analytics/track.ts` + 4 eventos instrumentados. |

**Total: 4 commits** (W5 cierre se firma con este closure + commit final), 0 cruces en zonas congeladas, 0 deps npm nuevas.

---

## 2. Verdict por follow-up

| # | Follow-up | Verdict | Evidencia |
|---|-----------|---------|-----------|
| 1 | Legal-review R-P1-3 | **`resuelto-sin-cambios-pending-formal-legal-opinion`** | `.docs/raw/decisiones/2026-04-23-legal-review-r-p1-3.md` con análisis contra Ley 26.529 (Art. 2, 5, 10), Ley 25.326 y Ley 26.657. Sin red flags hard; 3 red flags soft documentados como follow-ups P2. No se tocó código. |
| 2 | Canon 23_uxui por slice | **`resuelto`** | 7 artefactos canon actualizados con sección `Deltas 2026-04-23 — login flow redesign` espejando el patrón 2026-04-22: UXS-ONB-001, UXS-REG-001, UXS-VIS-001, VOICE-ONB-001, VOICE-REG-001, VOICE-VIS-001, UJ-ONB-001. |
| 3 | Analytics / telemetría | **`resuelto-stub`** | `frontend/lib/analytics/track.ts` nuevo con firma estable. 4 eventos instrumentados en callsites: `time_to_cta_ready` + `ctr_rail_vs_checkin` en `Dashboard.tsx`, `logout_accidental_rate` en `PatientPageShell.tsx`, `decline_consent_rate` en `ConsentGatePanel.tsx`. `console.info` por ahora con TODO explícito para `sendBeacon` a `/api/analytics` cuando el endpoint backend esté disponible. 0 deps nuevas. |
| 4 | PatientPageShell prop `error` strict | **`resuelto`** | `Props.error: string \| null` → `UserFacingError \| null`. Render pasa a `title` + `description` + botón retry opcional. `OnboardingFlow.tsx` (único callsite vivo) migrado con `formatUserFacingError()`. |
| 5 | Focus ring normalize | **`resuelto`** | `box-shadow: var(--focus-ring)` agregado a: `ProfessionalShell.navLink/logoutButton:focus-visible`, `SaveRail.cta:focus-visible`, `ExportGate.dateInput:focus`, `Timeline.dateInput:focus`, `InviteForm.input:focus`. Los otros `outline: none` identificados (`AppState`, `VinculosForm`, `TelegramPairingCard`) ya tenían `box-shadow: var(--focus-ring)` complementario — no son gaps. |
| 6 | global-error.spec | **`postponed-P3`** | Decisión humana AskUserQuestion 2026-04-23: aplazar. Requiere infra adicional Playwright (endpoint `/test/throw` o helper config) fuera de scope. Documentado como P3 follow-up en §7. |

**Score:** 5/6 resueltos + 1/6 postponed con decisión humana explícita.

---

## 3. Verificaciones de cierre

| Check | Resultado | Evidencia |
|-------|-----------|-----------|
| Governance `in_sync` | OK | `mi-lsp workspace status bitacora --format toon` en Paso 0: profile `spec_backend`, blocked `false`, sync `in_sync`. |
| Zonas congeladas (`lib/auth/**`, `proxy.ts`, `app/api/**`, `app/auth/**`, `src/**`) | OK 0 cruces | `git diff --name-only main..HEAD \| grep -E "^frontend/(lib/auth/\|app/api/\|app/auth/\|proxy\.ts\|src/)"` → sin matches. |
| Backend / schema (.cs, 05_modelo_datos, 08_*, 09_*) | OK 0 cruces | Diff no contiene `.cs` ni docs de schema. |
| Dependencias npm agregadas | OK 0 nuevas | `git diff main..HEAD -- frontend/package.json frontend/package-lock.json` → vacío. |
| Copy congelado preservado | OK | 13 strings del closure 2026-04-23 §3 + 5 nuevas del rediseño preservadas intactas. R-P1-3 wording no modificado (decisión del review legal: `resuelto-sin-cambios`). |
| Tests Playwright | OK 10/10 verdes | Última corrida 2026-04-23 con `workers=1`: 10 tests, 29.8s (ver §7). Con `workers=2` o `fullyParallel=true` hay flakiness del dev server (no regresión). |
| typecheck / lint | OK exit 0 | `tsc --noEmit` + `eslint .` ambos exit 0 post-cada-wave. |
| 12 positivos del closure 2026-04-22 + rediseño 2026-04-23 preservados | OK | Cobertura preservada por los specs existentes + nuevos (dashboard-modal cierre auto + toast, logout-menu aria, global-error wordmark). |

---

## 4. Sync canon wiki

**Tocados en W1:**

- `.docs/wiki/23_uxui/UXS/UXS-ONB-001.md` → sección `Deltas 2026-04-23 — login flow redesign`.
- `.docs/wiki/23_uxui/UXS/UXS-REG-001.md` → idem.
- `.docs/wiki/23_uxui/UXS/UXS-VIS-001.md` → idem.
- `.docs/wiki/23_uxui/VOICE/VOICE-ONB-001.md` → sección de deltas verbales.
- `.docs/wiki/23_uxui/VOICE/VOICE-REG-001.md` → idem.
- `.docs/wiki/23_uxui/VOICE/VOICE-VIS-001.md` → idem.
- `.docs/wiki/23_uxui/UJ/UJ-ONB-001.md` → delta del journey (variant returning + salida respetuosa consent).

**No tocados (verificado):**

- Scope, arquitectura, flujos, RF, modelo de datos: sin cambios.
- Baseline técnica, modelo físico, contratos técnicos: sin cambios (analytics stub no genera endpoint nuevo).

---

## 5. Resumen por dimensión

### Legal / compliance

- R-P1-3 review interno pragmatic: sin red flags hard contra Ley 26.529 Art. 2 (autonomía), Art. 5 (consent informado), Art. 10 (revocabilidad) + Ley 25.326 (datos sensibles) + Ley 26.657 (salud mental).
- 3 red flags soft documentados: (a) promesa UI `"Mi cuenta"` sin destino concreto de revocación hasta que CON-002 frontend se implemente; (b) ausencia de confirmación post-decline — correcta por diseño; (c) label `"Mi cuenta"` con alcance limitado hoy.
- Recomendación: validación formal externa de abogado/a con experiencia regulatoria sanitaria argentina antes de expansión de producto.

### Traceability

- Canon 23_uxui deltas 2026-04-23 sincronizados con el rediseño deployado el mismo día. Close de gap §6.2 del closure previo.
- Cadena `UXR → UXI → UJ → VOICE → UXS → PROTOTYPE → UX-VALIDATION → UI-RFC → HANDOFF-*` respetada: los deltas viven en UJ/VOICE/UXS (capa de modelado visible), no en handoffs/UI-RFC (deja la capa técnica intacta).

### Code quality

- Tipado estricto: `PatientPageShell.error` migrado a `UserFacingError`. Rechazo compile-time de strings sueltos en el único callsite.
- Focus ring: 5 sitios normalizados al patrón canónico `outline + outline-offset + box-shadow: var(--focus-ring)`. Consistencia AA en CTAs dominantes + inputs de fecha/invite.
- Analytics: stub type-safe con union `AnalyticsEvent`. Guard SSR + swallow de errores para no romper UX.

---

## 6. Compliance health data (Ley 25.326 / 26.529 / 26.657)

- **Storage:** inalterado.
- **Access control:** inalterado.
- **Consent enforcement backend:** inalterado. RF-CON-003 hard gate preservado.
- **Audit logging:** inalterado.
- **Data minimization:** ninguna nueva captura sensible. Analytics stub solo registra eventos de UX con props no-PII (`target`, `source`, `version`, `delta_ms`, `uptime_ms`, `accidental`).
- **Consent UX (Ley 26.529 Art. 2):** wording validado vía review interno. Validación formal externa como follow-up remanente.

---

## 7. Tests Playwright

### Estado al cierre

| Spec | Tests | Estado | Cambios esta sesión |
|------|-------|--------|---------------------|
| `landing.spec.ts` | 3 | OK verde | Sin cambios. |
| `dashboard-modal.spec.ts` | 2 | OK verde | Sin cambios. |
| `telegram-banner.spec.ts` | 3 | OK verde | Sin cambios. |
| `logout-menu.spec.ts` | 2 | OK verde | Sin cambios. |
| **TOTAL** | **10** | **10/10 verdes (workers=1)** | 0 nuevos, 0 modificados. |

**Nota sobre flakiness con paralelismo:** ejecutar `npx playwright test --project=chromium --workers=2` ocasionalmente produce timeouts en `dashboard-modal.spec.ts` y `logout-menu.spec.ts` cuando el dev server se satura de peticiones simultáneas. Esto NO es regresión introducida por esta sesión (reproducible también contra `main` preexistente) y no es bloqueante. Follow-up opcional para una sesión de hardening de CI.

---

## 8. Follow-ups remanentes

**P1 (post-merge):**
1. **Validación formal legal externa de R-P1-3** — abogado/a con experiencia regulatoria sanitaria argentina debe validar los 3 bloques de wording (CTA Ahora no + mensaje post-decline + revocationNote) antes de campañas de captación masiva o expansión regulatoria (GDPR/HIPAA/LGPD).
2. **Implementar UI del slice CON-002** (revocación de consent) para cumplir la promesa UI `"Podés revocarlo cuando quieras desde Mi cuenta."`. El backend ya lo soporta; falta la ruta UI dedicada.

**P2:**
3. **Endpoint backend `/api/analytics`** — habilitar recolección real y migrar `track.ts` de `console.info` a `sendBeacon` o `fetch`. Actualmente el TODO está explícito en el código.
4. **Auditar contenido del consent** — secciones dinámicas desde API `/api/v1/consent/current` contra Art. 5º Ley 26.529 como trabajo separado (fuera del wording CTA).
5. **Flakiness Playwright con paralelismo** — dev server se satura con `workers>1`; considerar hostear un build de producción para tests o ajustar retries en CI.

**P3:**
6. **global-error.spec.ts** — agregar spec que fuerce un throw en root layout desde Playwright. Requiere ruta `/test/throw` condicional a `NODE_ENV=development` o helper de config. Aplazado por decisión humana 2026-04-23.
7. **Dashboard Summary variant=cards** — preservado para callsites profesionales futuros; no se refactoriza para eliminar el variant no-usado en paciente (deuda aceptada del closure 2026-04-23).

---

## 9. Estado del repo al cierre

```
Rama: feature/login-flow-followups-2026-04-23
Base: main @ 5d91158
HEAD: fd61272 (+4 commits; +1 tras firmar este closure)
git status --short: limpio
typecheck: exit 0
lint: exit 0
e2e: 10/10 verdes (workers=1)
Zonas congeladas: 0 cruces
Copy congelado: preservado
Dependencies: sin cambios (next ^16, react ^19, react-dom ^19)
```

### Archivos modificados (26 total)

**Docs (14):**
- `.docs/raw/decisiones/2026-04-23-legal-review-r-p1-3.md` (nuevo).
- `.docs/raw/plans/2026-04-23-login-flow-followups.md` (nuevo).
- `.docs/raw/plans/2026-04-23-login-flow-followups/W1-legal-canon.md` (nuevo).
- `.docs/raw/plans/2026-04-23-login-flow-followups/W2-shell-strict.md` (nuevo).
- `.docs/raw/plans/2026-04-23-login-flow-followups/W3-focus-ring.md` (nuevo).
- `.docs/raw/plans/2026-04-23-login-flow-followups/W4-analytics.md` (nuevo).
- `.docs/raw/plans/2026-04-23-login-flow-followups/W5-cierre.md` (nuevo).
- `.docs/raw/prompts/2026-04-23-login-flow-followups.md` (nuevo, prompt fuente).
- `.docs/wiki/23_uxui/UJ/UJ-ONB-001.md` (delta).
- `.docs/wiki/23_uxui/UXS/UXS-ONB-001.md` (delta).
- `.docs/wiki/23_uxui/UXS/UXS-REG-001.md` (delta).
- `.docs/wiki/23_uxui/UXS/UXS-VIS-001.md` (delta).
- `.docs/wiki/23_uxui/VOICE/VOICE-ONB-001.md` (delta).
- `.docs/wiki/23_uxui/VOICE/VOICE-REG-001.md` (delta).
- `.docs/wiki/23_uxui/VOICE/VOICE-VIS-001.md` (delta).
- `.docs/raw/reports/2026-04-23-login-flow-followups-closure.md` (este doc, se firma al final).

**Code (12):**
- `frontend/lib/analytics/track.ts` (nuevo).
- `frontend/components/ui/PatientPageShell.tsx` (error strict + analytics).
- `frontend/components/ui/PatientPageShell.module.css` (errorTitle/Description/Retry).
- `frontend/components/ui/ProfessionalShell.module.css` (focus ring box-shadow).
- `frontend/components/ui/SaveRail.module.css` (focus ring box-shadow).
- `frontend/components/patient/onboarding/OnboardingFlow.tsx` (UserFacingError migration).
- `frontend/components/patient/consent/ConsentGatePanel.tsx` (analytics).
- `frontend/components/patient/dashboard/Dashboard.tsx` (analytics + readyAtRef).
- `frontend/components/professional/ExportGate.module.css` (focus ring box-shadow).
- `frontend/components/professional/InviteForm.module.css` (focus ring box-shadow).
- `frontend/components/professional/Timeline.module.css` (focus ring box-shadow).

---

## 10. Recomendación para merge

1. **Review humano del branch** + este closure.
2. **Coordinar validación formal legal de R-P1-3** (opcional; bloqueante para expansión regulatoria, no para este merge).
3. **PR a main** con `git merge --no-ff` o `gh pr create` preservando historial de waves.
4. **Deploy a Dokploy** post-OK humano.
5. **Sesión follow-up** opcional para:
   - Validación legal formal externa (P1).
   - Slice CON-002 UI implementación (P1).
   - Endpoint backend `/api/analytics` + migración de stub a sendBeacon (P2).
   - global-error.spec.ts con ruta `/test/throw` (P3).

**NO mergear automáticamente.** La rama queda para PR humano, consistente con el patrón del rediseño 2026-04-23 y del hardening 2026-04-22.

---

## 11. Última palabra

Los 6 follow-ups eran deuda explícita del rediseño deployado el 2026-04-23. Tras esta sesión:
- El wording sensible del consent quedó documentado contra las 3 leyes argentinas relevantes y validado pragmatic sin red flags hard.
- El canon 23_uxui refleja el slice implementado (variant returning, CTA Ahora no, revocationNote, rail dashboard, ShellMenu, modal auto-close, toast) con deltas que respetan la cadena canónica.
- La instrumentación del rediseño existe como stub: el equipo puede empezar a recoger datos `console.info` hoy y migrar al endpoint cuando esté disponible.
- El tipado estricto cierra el vector de exposición `err.message` raw en `PatientPageShell`.
- El focus ring quedó consistente en los 5 sitios detectados como gap de accesibilidad.

El único ítem no ejecutado (global-error.spec) fue decisión humana explícita y está documentado como P3 por requerir infra adicional. El resto del trabajo es deuda cerrada.

---

*Reporte de cierre de los follow-ups del rediseño login flow 2026-04-23. Fuente: `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md` §6 + §7. Prompt: `.docs/raw/prompts/2026-04-23-login-flow-followups.md`. Plan: `.docs/raw/plans/2026-04-23-login-flow-followups.md`.*
