# UX-VALIDATION-REG-002 — Validación UX del slice REG-002

## Slice y actor

- **Slice:** `REG-002` — registro de factores diarios vía web
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
- **Impacto:** la ruta `/registro/daily-checkin` no puede validarse contra el backend real desde la sesión actual.
- **Owner:** runtime/frontend config
- **Estado:** Open

### WEB-VAL-002 — No interactive browser validation evidence

- **Severidad:** Critical
- **Evidencia:** no screenshots ni transcript de navegador autenticado fueron producidos en esta ejecución.
- **Impacto:** `UI-RFC` y `HANDOFF-VISUAL-QA` no pueden confirmarse contra la sesión renderizada real.
- **Owner:** QA / validation harness
- **Estado:** Open

### WEB-VAL-003 — Next.js proxy convention migrated

- **Severidad:** High
- **Evidencia:** `frontend/proxy.ts` reemplaza la convención `middleware` sin cambiar la lógica de auth/routing.
- **Impacto:** deuda de compatibilidad Next.js 16 resuelta; mantener `next build` sin warning de migración.
- **Owner:** frontend runtime hardening
- **Estado:** Resolved

## Veredicto de validación

La validación UX del slice `REG-002` queda en estado **`blocked`** en esta sesión.

- El build del frontend y del backend pasó (`next build` + `dotnet build`).
- La ruta `/registro/daily-checkin` existe y compila.
- No fue posible ejecutar validación interactiva real por los bloqueos documentados en `WEB-VAL-001` y `WEB-VAL-002`.
- Este documento es evidencia de un intento de validación bloqueada y no debe interpretarse como cierre exitoso.

## Defectos abiertos

| Defecto | Severidad | slice | Estado |
| --- | --- | --- | --- |
| WEB-VAL-001 | Critical | REG-002 | Open |
| WEB-VAL-002 | Critical | REG-002 | Open |
| WEB-VAL-003 | High | todas | Resolved |

## Retorno a la cadena canónica

El slice `REG-002` tiene `UI-RFC + HANDOFF-*` completos bajo waiver de entrada a UI. Esa apertura no modifica el estado de validación: sigue `blocked` hasta que exista evidencia real de cohorte.

## Siguiente paso

Ejecutar cohorte híbrida `B` con sesión de navegador autenticada y `NEXT_PUBLIC_API_BASE_URL` configurada. Actualizar este documento con evidencia real.

---

**Estado:** validación bloqueada — sesión 2026-04-10.
**Próximo gate:** ejecución de cohorte real con entorno configurado.
