# Login Flow Follow-ups v2 — Reporte de Cierre 2026-04-23

**Fecha:** 2026-04-23
**Rama:** `feature/login-flow-followups-2026-04-23` (extendida tras v1)
**Base v1:** `main` @ `5d91158` → HEAD v1 `5a7aff3` → HEAD v2 `b224e18` (+4 commits v2)
**Classification:** `mixed (docs + frontend + backend + schema + contract + config)`
**Compliance:** Ley 25.326 / 26.529 / 26.657 — review formal externo pendiente; sin brechas de enforcement backend.
**Prompt fuente:** `.docs/raw/prompts/2026-04-23-login-flow-followups.md` + decisión humana posterior `"arreglar todos los follow-ups remanentes"`.
**Plan fuente:** `.docs/raw/plans/2026-04-23-login-flow-followups.md` + subdocs W1-W10.

---

## 1. Cadena de commits (v1 + v2)

### v1 (primera pasada — follow-ups del rediseño 2026-04-23)

| # | Commit | Wave | Alcance |
|---|---|---|---|
| 1 | `1539588` | W1 | Legal-review R-P1-3 + canon 23_uxui deltas 2026-04-23 |
| 2 | `9b6fcc7` | W2 | PatientPageShell strict `UserFacingError` |
| 3 | `207bb4a` | W3 | Focus ring normalize |
| 4 | `fd61272` | W4 | Analytics stub `console.info` + 4 eventos |
| 5 | `5a7aff3` | W5 | Closure v1 |

### v2 (segunda pasada — follow-ups remanentes v1)

| # | Commit | Wave | Alcance |
|---|---|---|---|
| 6 | `e8240c2` | W6+W9 | Legal template + auditoría consent Art. 5 + Playwright workers=1 |
| 7 | `cf55c68` | W7 | CON-002 UI revocation materializada |
| 8 | `b224e18` | W8 | Backend analytics endpoint + Postgres + migración stub a fetch real |

**Total: 8 commits + este closure** (9 commits al momento de firmar).

---

## 2. Verdict por follow-up remanente

| # | Follow-up v1 | Decisión humana | Wave v2 | Verdict v2 |
|---|--------------|-----------------|---------|-----------|
| 1 | Validación formal legal externa R-P1-3 | `Template de carta al abogado` | W6 | **`resuelto-template-preparado`** — `.docs/raw/decisiones/2026-04-23-legal-review-request.md` self-contained con 12 preguntas + checklist firmable + normativa. Pendiente envío humano. |
| 2 | CON-002 UI revocación consent | `Implementación completa del slice` | W7 | **`resuelto-materializado`** — ruta `/configuracion/consent` + `ConsentRevocationPanel` + item `Consentimiento` en ShellMenu + 3 e2e tests verdes. Cumple promesa UI del revocationNote del ConsentGatePanel. |
| 3 | Endpoint backend `/api/analytics` + migración stub | `Endpoint con storage Postgres` | W8 | **`resuelto-materializado`** — Entity + Repository + Command/Handler + Endpoint + SQL migration plano + canon sync (04/05/06/08/09 + RF-ANA-001 + TP-ANA). Frontend stub migrado a `fetch` real a `/api/backend/analytics/events`. |
| 4 | Auditoría contenido consent Art. 5 | `incluir` | W6 | **`resuelto-documento-entregado`** — `.docs/raw/decisiones/2026-04-23-consent-content-audit.md` con verdict `requiere-ampliación-sugerida` (2/6 incisos bien cubiertos, 4/6 parcial o ausente). Propuesta v2 del consent con 6 secciones. No se modifica `appsettings.json` (responsabilidad clínico-legal). |
| 5 | Flakiness Playwright con paralelismo | `incluir` | W9 | **`resuelto`** — `playwright.config.ts` con `workers=1` default local + `retries=1` local. 13/13 verdes consistente. |
| 6 | global-error.spec.ts | `aplazar a P3` | — | **`postponed-P3`** (decisión v1) |

**Score v2:** 5/5 follow-ups de v1 cerrados o documentados. 1/6 original sigue aplazado por decisión humana.

---

## 3. Verificaciones de cierre v2

| Check | Resultado | Evidencia |
|-------|-----------|-----------|
| Governance `in_sync` | OK | Preservado desde v1 |
| Zonas congeladas frontend (`lib/auth/`, `app/api/`, `app/auth/`, `proxy.ts`, `src/`) | OK 0 cruces | `git diff --name-only main..HEAD \| grep -E "^frontend/(lib/auth/\|app/api/\|app/auth/\|proxy\.ts\|src/)"` → vacío |
| Cambios backend | Scope-autorizado | El humano autorizó explícitamente ampliar scope a backend + schema + contratos en W8. |
| Schema nuevo aplicado en dev | Pendiente manual | SQL migration `infra/migrations/bitacora/20260423_create_analytics_events.sql`. `psql` manual requerido en dev + prod (`infra/runbooks/manual-migrations.md`). |
| Backend build | OK | `dotnet build src/Bitacora.sln` → 0 errors, 0 warnings. |
| Frontend typecheck | OK exit 0 | Post cada wave verificado. |
| Frontend lint | OK exit 0 | Post cada wave verificado. |
| E2E tests | OK 13/13 verdes | Última corrida 2026-04-23: 3 specs nuevos CON-002 + 10 existentes. `workers=1`, 28.7s. |
| Dependencias npm agregadas | OK 0 nuevas | `package.json` inalterado. |
| Copy congelado preservado | OK | 13 strings congeladas intactas + 5 nuevas del rediseño 2026-04-23 + nuevas 2026-04-23 v2 (`Revocar consentimiento`, `Conservar consentimiento`, `Consentimiento`) como extensiones canon. |
| Canon sync completo (04/05/06/08/09 + 23_uxui) | OK | Módulo ANA nuevo; CON-002 con deltas 2026-04-23. |

---

## 4. Sync canon wiki v2

**Actualizados en W8 (canon técnico):**
- `04_RF.md` — nuevo módulo ANA con `RF-ANA-001`.
- `04_RF/RF-ANA-001.md` — execution sheet completa.
- `05_modelo_datos.md` — entidad `AnalyticsEvent` con invariantes append-only + no-PII enforcement.
- `06_matriz_pruebas_RF.md` — nueva sección ANA.
- `06_pruebas/TP-ANA.md` — 6 casos (4 positivos + 2 negativos).
- `08_modelo_fisico_datos.md` — tabla `analytics_events` con ownership Analytics.
- `09_contratos_tecnicos.md` — endpoint `POST /api/v1/analytics/events` con `RF-ANA-001`.

**Actualizado en W7 (canon 23_uxui):**
- `23_uxui/UXS/UXS-CON-002.md` — sección `Deltas 2026-04-23 — CON-002 UI materializada` con componente, ruta, ShellMenu item, copy aprobado, tests e2e.

**Sin cambios v2:**
- `00_gobierno_documental.md`, `01_alcance_funcional.md`, `02_arquitectura.md`, `03_FL.md`, `07_baseline_tecnica.md`: no tocados (scope no requiere).
- Resto de canon 23_uxui: sin nuevas deltas v2.

---

## 5. Compliance health data v2

- **Storage:** nueva tabla `analytics_events` NO almacena datos clínicos ni PII. `patient_id` como responsable + `event_name` whitelisted + `props_json` con contrato explícito no-PII. Append-only, sin UPDATE/DELETE.
- **Access control:** endpoint analytics requiere JWT Zitadel válido (`CurrentAuthenticatedPatientResolver`). Rate limited policy `write`.
- **Consent enforcement backend:** inalterado. RF-CON-003 preservado.
- **Audit logging:** inalterado. Analytics NO se persiste en `access_audits` (separación intencional).
- **Revocabilidad (Art. 10 Ley 26.529):** ahora con UI dedicada materializada (CON-002). La promesa del revocationNote tiene contraparte operativa.
- **Consent UX (Art. 2 + Art. 5 Ley 26.529):** wording actual validado pragmatic en v1; auditoría de contenido del consent v2 recomienda ampliación (4 incisos faltantes). Pending decisión clínico-legal.

---

## 6. Tests al cierre v2

### Specs

| Spec | Tests | Estado | Cambios v2 |
|------|-------|--------|-----------|
| `landing.spec.ts` | 3 | OK verde | Sin cambios |
| `dashboard-modal.spec.ts` | 2 | OK verde | Sin cambios |
| `telegram-banner.spec.ts` | 3 | OK verde | Sin cambios |
| `logout-menu.spec.ts` | 2 | OK verde | Sin cambios |
| **`consent-revocation.spec.ts`** (nuevo v2) | **3** | **OK verde** | 3 nuevos: panel visible con impact list, DELETE dispara confirmación, ShellMenu tiene item |
| **TOTAL** | **13** | **13/13 verdes** | +3 nuevos |

**Tests backend unitarios:** el scaffold `src/Bitacora.Tests` sigue minimal (no incluye tests nuevos para AnalyticsEndpoint en esta sesión). `dotnet build` verifica integridad de compilación. Tests integración reales requieren DB con tabla aplicada y backend rebuild — follow-up operacional.

### Playwright config

- `workers=1` default local + `retries=1` local.
- CI preservado: `workers=1` + `retries=2`.
- Follow-up: cuando el equipo hostee un build de producción para tests, re-habilitar paralelismo.

---

## 7. Estado del repo al cierre v2

```
Rama: feature/login-flow-followups-2026-04-23
Base: main @ 5d91158
HEAD: b224e18 (v2) + closure v2 commit pendiente
Commits totales: 8 (v1: 5, v2: 3) + closure v2 = 9
git status --short: limpio tras closure
Frontend: typecheck OK, lint OK, 13/13 e2e verdes (workers=1)
Backend: dotnet build OK (0 errors, 0 warnings)
Schema: nueva tabla analytics_events via SQL plano (pending apply manual)
Zonas congeladas frontend: 0 cruces
Zonas congeladas backend: N/A (scope autorizado)
Dependencies: sin cambios (next ^16, react ^19, react-dom ^19)
```

### Archivos v2 (delta respecto a v1)

**Docs (11 tocados en v2):**
- `.docs/raw/decisiones/2026-04-23-legal-review-request.md` (nuevo)
- `.docs/raw/decisiones/2026-04-23-consent-content-audit.md` (nuevo)
- `.docs/raw/reports/2026-04-23-login-flow-followups-v2-closure.md` (este doc)
- `.docs/wiki/04_RF.md` (delta ANA)
- `.docs/wiki/04_RF/RF-ANA-001.md` (nuevo)
- `.docs/wiki/05_modelo_datos.md` (delta AnalyticsEvent)
- `.docs/wiki/06_matriz_pruebas_RF.md` (delta ANA)
- `.docs/wiki/06_pruebas/TP-ANA.md` (nuevo)
- `.docs/wiki/08_modelo_fisico_datos.md` (delta analytics_events)
- `.docs/wiki/09_contratos_tecnicos.md` (delta endpoint)
- `.docs/wiki/23_uxui/UXS/UXS-CON-002.md` (delta 2026-04-23)

**Frontend (7 tocados en v2):**
- `frontend/app/(patient)/configuracion/consent/page.tsx` (nuevo)
- `frontend/components/patient/consent/ConsentRevocationPanel.tsx` (nuevo)
- `frontend/components/patient/consent/ConsentRevocationPanel.module.css` (nuevo)
- `frontend/components/ui/PatientPageShell.tsx` (delta menu item)
- `frontend/lib/api/client.ts` (delta `revokeConsent`)
- `frontend/lib/analytics/track.ts` (migrado a fetch real)
- `frontend/e2e/consent-revocation.spec.ts` (nuevo)
- `frontend/playwright.config.ts` (workers=1)

**Backend (8 tocados en v2):**
- `src/Bitacora.Domain/Entities/AnalyticsEvent.cs` (nuevo)
- `src/Bitacora.DataAccess.Interface/Repositories/IAnalyticsEventRepository.cs` (nuevo)
- `src/Bitacora.DataAccess.EntityFramework/Repositories/AnalyticsEventRepository.cs` (nuevo)
- `src/Bitacora.DataAccess.EntityFramework/Persistence/AppDbContext.cs` (delta)
- `src/Bitacora.DataAccess.EntityFramework/DependencyInjection/ServiceCollectionExtensions.cs` (delta)
- `src/Bitacora.Application/Commands/Analytics/TrackAnalyticsEventCommand.cs` (nuevo)
- `src/Bitacora.Api/Endpoints/Analytics/AnalyticsEndpoints.cs` (nuevo)
- `src/Bitacora.Api/Program.cs` (delta import + Map)

**Infra (1 nuevo en v2):**
- `infra/migrations/bitacora/20260423_create_analytics_events.sql` (nuevo)

---

## 8. Recomendación para merge v2

1. **Review humano del branch completo** (v1 + v2) + este closure.
2. **Aplicar SQL migration en dev**: `psql "$env:ConnectionStrings__BitacoraDb" -f infra/migrations/bitacora/20260423_create_analytics_events.sql`.
3. **Rebuild backend** en dev (`dotnet run` o restart container) para cargar el endpoint nuevo.
4. **Verificar telemetría end-to-end**: navegar al dashboard, clickear CTA "+ Nuevo registro", confirmar que llega un row a `analytics_events` via psql.
5. **Aplicar SQL migration en prod** cuando el equipo lo decida, siguiendo `infra/runbooks/manual-migrations.md`.
6. **Coordinar validación formal legal** de R-P1-3 + auditoría consent antes de expansión regulatoria.
7. **PR a main** con `git merge --no-ff`.
8. **Deploy a Dokploy** post-OK humano + SQL aplicada en prod.

**NO mergear automáticamente.** La rama queda para PR humano.

---

## 9. Follow-ups remanentes v2

**P1 (mantiene del v1):**
1. Validación formal legal externa enviando el template `.docs/raw/decisiones/2026-04-23-legal-review-request.md` + auditoría `.docs/raw/decisiones/2026-04-23-consent-content-audit.md` al abogado/a.

**P2 operacionales:**
2. Aplicar SQL migration analytics en dev + prod.
3. Decidir retention policy (180d sugerido) y materializar cron task de cleanup.
4. Backend tests unitarios para `TrackAnalyticsEventCommandHandler` (whitelist enforcement + props length validation) en `src/Bitacora.Tests`.
5. E2E backend real del endpoint `POST /api/v1/analytics/events` con auth + DB real (actualmente los e2e frontend stubean el endpoint).
6. Auditoría clínico-legal del contenido del consent (decisión sobre ampliación v2 propuesta) → si positiva, nueva sesión para incrementar version + manejar re-aceptación.

**P3 (mantiene del v1):**
7. `global-error.spec.ts` — postponed por decisión humana, requiere ruta `/test/throw` condicional a `NODE_ENV=development`.

**P2 futuro cuando haya build de producción para tests:**
8. Restaurar `workers=undefined` en `playwright.config.ts` (re-habilitar paralelismo).

---

## 10. Última palabra v2

En esta segunda pasada se resolvieron todos los follow-ups remanentes del closure v1 dentro de lo que Claude puede ejecutar autónomamente:

- **Legal formal externo** es trabajo humano; esta sesión entregó template firmable con 12 preguntas precisas + auditoría del contenido del consent contra Art. 5 con verdict `requiere-ampliación-sugerida`.
- **CON-002 UI** se materializó end-to-end: página + componente + item en menú + 3 e2e verdes. La promesa "revocar desde Mi cuenta" tiene contraparte operativa cumpliendo Ley 26.529 Art. 10.
- **Analytics endpoint + storage Postgres** quedó implementado: entity + repository + command + endpoint + SQL plano + canon sync (6 docs + 1 RF nuevo + 1 TP nuevo). El stub frontend ahora persiste eventos reales a `/api/backend/analytics/events`.
- **Playwright flakiness** resuelto con `workers=1` default local; 13/13 verdes consistente.

Lo único no ejecutado por decisión humana sigue siendo `global-error.spec.ts` como P3.

**Invariantes preservados:**
- Compliance Ley 25.326/26.529/26.657 sin regresión (analytics no-PII, consent enforcement backend intacto, audit logging intacto).
- Zonas congeladas frontend (`lib/auth/`, `proxy.ts`, `app/api/` catchall, etc.) intocadas.
- Copy congelado preservado; nuevos copys del CON-002 son extensiones canon.
- 13/13 e2e verdes. typecheck + lint exit 0. `dotnet build` OK.

La rama queda lista para review humano + aplicación manual de SQL + rebuild backend + PR a main.

---

*Reporte de cierre v2 de los follow-ups del rediseño login flow 2026-04-23. Fuente v1: `.docs/raw/reports/2026-04-23-login-flow-followups-closure.md`. Prompt: `.docs/raw/prompts/2026-04-23-login-flow-followups.md` + decisión humana "arreglar todos los follow-ups remanentes" 2026-04-23.*
