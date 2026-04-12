# UX-VALIDATION-EXP-001 — Validación UX del slice EXP-001

## Slice y actor

- **Slice:** `EXP-001` — exportación CSV del paciente
- **Actor:** Paciente
- **Fecha de validación intentada:** 2026-04-10
- **Resultado:** validación **bloqueada**

## Evidencia revisada

- `artifacts/e2e/2026-04-10-wave-prod-web-validation/summary.md`
- `artifacts/e2e/2026-04-10-wave-prod-web-validation/defects.md`

## Hallazgos

### WEB-VAL-001 — Missing frontend API base environment

- **Severidad:** Critical
- **Evidencia:** `frontend_env_missing_api_base` — `frontend/.env.local` no define `NEXT_PUBLIC_API_BASE_URL`.
- **Impacto:** la exportación CSV del paciente no puede validarse contra el backend real desde la sesión actual.
- **Owner:** runtime/frontend config
- **Estado:** Open

### WEB-VAL-002 — No interactive browser validation evidence

- **Severidad:** Critical
- **Evidencia:** no screenshots ni transcript de navegador autenticado fueron producidos en esta ejecución.
- **Impacto:** `UI-RFC` y `HANDOFF-VISUAL-QA` no pueden confirmarse contra la sesión renderizada real.
- **Owner:** QA / validation harness
- **Estado:** Open

### WEB-VAL-003 — Next.js middleware convention deprecated

- **Severidad:** High
- **Evidencia:** `next build` muestra warning sobre `middleware` migrando a `proxy`.
- **Impacto:** no es un blocker actual de build, pero debe migrarse antes de la estabilización.
- **Owner:** frontend runtime hardening
- **Estado:** Open

## Veredicto de validación

La validación UX del slice `EXP-001` queda en estado **`blocked`** en esta sesión.

- El build del frontend y del backend pasó (`next build` + `dotnet build`).
- Las rutas de exportación existen y compilan.
- No fue posible ejecutar validación interactiva real por los bloqueos documentados en `WEB-VAL-001` y `WEB-VAL-002`.
- La superficie backend de Export está materializada (`GET /export/patient-summary`, `GET /export/patient-summary/csv`), lo cual se valida en smoke gate ( RF-EXP-001).
- Este documento es evidencia de un intento de validación bloqueada y no debe interpretarse como cierre exitoso.

## Defectos abiertos

| Defecto | Severidad | slice | Estado |
| --- | --- | --- | --- |
| WEB-VAL-001 | Critical | EXP-001 | Open |
| WEB-VAL-002 | Critical | EXP-001 | Open |
| WEB-VAL-003 | High | todas | Open |

## Retorno a la cadena canónica

El slice `EXP-001` tiene `UI-RFC + HANDOFF-*` completos bajo waiver de entrada a UI. Esa apertura no modifica el estado de validación: sigue `blocked` hasta que exista evidencia real de cohorte.

## Siguiente paso

Ejecutar cohorte híbrida `E` con sesión de navegador autenticada y `NEXT_PUBLIC_API_BASE_URL` configurada. Actualizar este documento con evidencia real.

---

**Estado:** validación bloqueada — sesión 2026-04-10.
**Próximo gate:** ejecución de cohorte real con entorno configurado.
