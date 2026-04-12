# Decisión de Release Readiness — 2026-04-10

## Fecha

2026-04-10

## Contexto

Durante `Phase 60` se intentó ejecutar validación E2E completa de los canales web y Telegram del MVP de Bitacora. Esta nota documenta el resultado de ese intento y la decisión de release tomada.

## Evidencia revisada

| Artefacto | Ruta | Resultado |
| --- | --- | --- |
| Web validation summary | `artifacts/e2e/2026-04-10-wave-prod-web-validation/summary.md` | build pasado; validación interactiva bloqueada |
| Web validation defects | `artifacts/e2e/2026-04-10-wave-prod-web-validation/defects.md` | 2 críticos abiertos |
| Telegram validation summary | `artifacts/e2e/2026-04-10-wave-prod-telegram-validation/summary.md` | build pasado; validación de canal bloqueada |
| Telegram validation defects | `artifacts/e2e/2026-04-10-wave-prod-telegram-validation/defects.md` | 2 críticos abiertos |
| Telegram validation transcript | `artifacts/e2e/2026-04-10-wave-prod-telegram-validation/transcript.md` | sin transcript — sin credencial de bot |

## Defectos abiertos

### Críticos

| ID | Descripción | Owner | Slice afectada |
| --- | --- | --- | --- |
| `WEB-VAL-001` | `frontend/.env.local` no define `NEXT_PUBLIC_API_BASE_URL`; no puede validarse runtime web contra backend real | runtime/frontend config | ONB-001, REG-001, REG-002, VIN-001, VIS-001, EXP-001 |
| `WEB-VAL-002` | Sin sesión de navegador autenticada ni transcript E2E; `UI-RFC` y `HANDOFF-VISUAL-QA` no pueden confirmarse contra sesión renderizada real | QA / validation harness | ONB-001, REG-001, REG-002, VIN-001, VIS-001, EXP-001 |
| `TG-VAL-001` | `TELEGRAM_BOT_TOKEN` no presente en el shell ni en `infra/.env`; no puede ejecutarse bot real | runtime/secret provisioning | TG-001, TG-002 |
| `TG-VAL-002` | Sin cuenta de test Telegram ni transcript reproducible; flujos de vinculación y recordatorio no pueden validarse con canal real | QA / Telegram E2E setup | TG-001, TG-002 |

### Altos

| ID | Descripción | Owner | slice afectada |
| --- | --- | --- | --- |
| `WEB-VAL-003` | `next build` warning: `middleware` migrará a `proxy`; no es blocker actual pero debe migrarse | frontend runtime hardening | todas las rutas web |
| `TG-VAL-003` | El path de recordatorio depende de conectividad real al Bot API sin credencial de fallback | backend runtime / QA | TG-002 |

## Veredicto de validación

**NO GO** para validación UX interactiva.

- Los builds del frontend y backend pasaron (`next build` + `dotnet build`).
- Las rutas web y los endpoints Telegram compilan y están enlazados a la superficie esperada.
- No se obtuvo evidencia de sesión renderizada real del navegador.
- No se obtuvo transcript ni respuesta del bot de Telegram.
- La deuda de validación es real y está documentada con honestidad en los `UX-VALIDATION-*` creados.

## Decisión de release

**Release NO LISTA para go-live de UX/UI interactivo.**

La implementación técnica de backend puede considerarse estable a nivel smoke gate (ver `06_matriz_pruebas_RF.md`), pero la validación UX de los slices ONB-001, REG-001, REG-002, VIN-001, VIS-001, EXP-001, TG-001 y TG-002 permanece abierta.

### Lo que SÍ está listo

- Superficie backend de Vinculos, Visualización, Export y Telegram validada por smoke gate (RF-VIN-001..004, RF-VIS-001..003, RF-EXP-001, RF-TG-001..002).
- Builds del frontend y backend pasan.
- Waivers de entrada a UI y cobertura documental de MVP están cerrados.

### Lo que NO está listo

- Validación UX interactiva de los slices web:ONB-001, REG-001, REG-002, VIS-001, EXP-001, VIN-001 — bloqueada por WEB-VAL-001 y WEB-VAL-002.
- Validación UX del canal Telegram: TG-001, TG-002 — bloqueada por TG-VAL-001 y TG-VAL-002.
- Migración de `middleware` a `proxy` (WEB-VAL-003).

## Acciones requeridas antes de siguiente validación

| # | Acción | Owner | Prioridad |
| --- | --- | --- | --- |
| 1 | Definir `NEXT_PUBLIC_API_BASE_URL` en `frontend/.env.local` y configurar entorno de validación con sesión de navegador autenticada | runtime/frontend config | P0 |
| 2 | Provisionar `TELEGRAM_BOT_TOKEN` y cuenta de test Telegram para E2E | runtime/secret provisioning | P0 |
| 3 | Configurar harness de smoke/browser automation para validación web autenticada | QA / validation harness | P0 |
| 4 | Configurar harness de transcript Telegram reproducible | QA / Telegram E2E setup | P0 |
| 5 | Migrar `middleware` de Next.js a `proxy` (WEB-VAL-003) | frontend runtime hardening | P1 |
| 6 | Ejecutar cohorte fundacional A para slices ONB-001, REG-001, REG-002 | QA / producto | P0 |
| 7 | Ejecutar cohorte híbrida F para VIN-001 | QA / producto | P1 |
| 8 | Ejecutar cohorte híbrida G para TG-001, TG-002 | QA / producto | P0 |

## Siguiente paso

Ejecutar las acciones P0, luego re-intentar validación UX con evidencia real. Actualizar `21_matriz_validacion_ux.md` y los `UX-VALIDATION-*` correspondientes con hallazgos de la nueva sesión.

---

**Estado del documento:** decisión de release tomada — NO GO para UX interactivo.
**Fecha de próxima revisión:** cuando las acciones P0 estén cerradas y se re-ejecute la validación.
