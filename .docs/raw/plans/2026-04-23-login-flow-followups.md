# Plan — Login Flow Follow-ups 2026-04-23

**Fecha:** 2026-04-23
**Rama:** `feature/login-flow-followups-2026-04-23`
**Base:** `main` @ `5d91158` (merge del rediseño 2026-04-23)
**Classification:** `ui-only, no-schema, no-contract, no-auth-edit, docs + code`
**Scope:** 5 follow-ups priorizados del closure `2026-04-23-login-flow-redesign-closure.md` §6 (el #6 global-error.spec aplazado a P3 por decisión humana).
**Shape de referencia:** `.docs/raw/plans/2026-04-23-login-flow-redesign.md` + subdocs W1/W2/W3/W4.

---

## 1. Decisiones humanas cerradas (AskUserQuestion 2026-04-23)

| Decisión | Respuesta humana | Consecuencia |
|----------|------------------|--------------|
| Legal-review R-P1-3 | `hacer vos review` | Wave 1 incluye review legal docs-only desde perspectiva Ley 26.529 + Ley 25.326, con recomendación de validación formal. Sin cambio de código si el wording pasa el review. |
| Analytics endpoint | `Stub console.info + estructura lista` | Wave 4 crea `lib/analytics/track.ts` con firma `track(event, props)` → `console.info('[analytics]', ...)` + TODO sendBeacon. 0 deps nuevas. |
| Canon 23_uxui alcance | `Delta 2026-04-23 en artefactos existentes` | Wave 1 agrega secciones `Deltas 2026-04-23 — login flow redesign` en UXS-ONB-001, UXS-REG-001, UXS-VIS-001 y VOICE correspondientes. Patrón idéntico al delta 2026-04-22 ya presente. |
| Follow-up #6 (global-error.spec) | `Aplazar a P3` | NO incluido. Documentado en closure como P3 follow-up. |

---

## 2. Invariantes protegidos

- **Zonas congeladas:** `frontend/lib/auth/*`, `frontend/app/api/*`, `frontend/app/auth/*`, `frontend/proxy.ts`, `frontend/src/**`. Grep final por wave debe dar 0 cruces.
- **Copy congelado preservado:** todas las cadenas del closure 2026-04-23 §3 intactas. R-P1-3 puede cambiar solo bajo decisión legal explícita.
- **Compliance Ley 25.326/26.529/26.657:** sin cambios en storage, access control, consent enforcement backend ni audit logging.
- **Dependencias npm:** 3 actuales (next, react, react-dom). 0 nuevas.
- **Git hygiene:** cada wave = 1+ commits atómicos con trailer `- Gabriel Paz -`. NO `--no-verify`, NO `--force push`, NO `--amend`.

---

## 3. Waves

| # | Alcance | Tipo | Subagentes | Evidencia obligatoria |
|---|---------|------|------------|-----------------------|
| W1 | Legal-review docs + canon 23_uxui deltas 2026-04-23 | docs-only | 1× `ps-docs` | 1 archivo nuevo (decisión legal) + 6 archivos modificados (UXS/VOICE ONB-001, REG-001, VIS-001) |
| W2 | `PatientPageShell` prop `error` strict `UserFacingError` + migración callsite único | code | 1× `ps-next-vercel` | 2 archivos modificados (PatientPageShell.tsx + OnboardingFlow.tsx). Typecheck exit 0. |
| W3 | Focus ring normalize (`ProfessionalShell`, `SaveRail`) + `outline: none` sueltos | code | 1× `ps-next-vercel` | Archivos CSS modificados con `:focus-visible` canónico. Grep final verifica. |
| W4 | Analytics stub `lib/analytics/track.ts` + 4 eventos instrumentados | code | 1× `ps-next-vercel` | 1 archivo nuevo + 4 callsites instrumentados. 0 deps nuevas. Typecheck exit 0. |
| W5 | Tests + cierre (typecheck + lint + e2e + ps-trazabilidad + closure update) | mixed | `ps-worker` | 10/10+ e2e verdes, closure doc, no merge. |

Cada wave cierra con:
- Typecheck exit 0 + lint exit 0.
- Grep 0 cruces zonas congeladas.
- Grep 0 deps npm nuevas (package.json inalterado salvo en W4 si surge — pero plan dice 0).
- Commit atómico con trailer.
- **CHECKPOINT humano** antes de pasar a la siguiente wave.

---

## 4. Subdocumentos por wave

- [`W1 — Legal-review + canon 23_uxui`](./2026-04-23-login-flow-followups/W1-legal-canon.md)
- [`W2 — PatientPageShell strict UserFacingError`](./2026-04-23-login-flow-followups/W2-shell-strict.md)
- [`W3 — Focus ring normalize`](./2026-04-23-login-flow-followups/W3-focus-ring.md)
- [`W4 — Analytics stub`](./2026-04-23-login-flow-followups/W4-analytics.md)
- [`W5 — Tests + cierre`](./2026-04-23-login-flow-followups/W5-cierre.md)

---

## 5. Gate pre-merge

1. Review humano del branch.
2. Closure update `2026-04-23-login-flow-followups-closure.md` firmado.
3. PR a main con `git merge --no-ff`.
4. Deploy a Dokploy post-OK.

**NO mergear automáticamente.** La rama queda para PR humano.
