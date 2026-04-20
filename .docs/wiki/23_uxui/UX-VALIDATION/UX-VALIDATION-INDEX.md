# UX-VALIDATION — Índice de validaciones por slice

## Propósito

Este índice lista el estado de validación UX de cada slice visible.

No reemplaza `21_matriz_validacion_ux.md` ni los futuros `UX-VALIDATION-*`. Su función es centralizar el punto de entrada de la familia `UX-VALIDATION` dentro de `23_uxui`.

## Documentos activos — sesión 2026-04-12

9 slices actualizados a **`Validado (evidencia statica)`** tras Phase 60 T1/T2. 4 slices deferidos (P2).

| Slice | Estado | Evidencia | Resultado |
| --- | --- | --- | --- |
| `ONB-001` | **Validado (evidencia statica)** | web T1 `2026-04-12` | GO |
| `REG-001` | **Validado (evidencia statica)** | web T1 `2026-04-12` | GO |
| `REG-002` | **Validado (evidencia statica)** | web T1 `2026-04-12` | GO |
| `VIN-001` | **Validado (evidencia statica)** | web T1 `2026-04-12` | GO |
| `VIN-002` | **Validado (evidencia statica)** | web T1 `2026-04-12` | GO |
| `VIN-003` | `prepared_waiting_evidence` | — | deferido P2 |
| `VIN-004` | `prepared_waiting_evidence` | — | deferido P2 |
| `CON-002` | `prepared_waiting_evidence` | — | deferido P2 |
| `VIS-001` | **Validado (evidencia statica)** | web T1 `2026-04-12` | GO |
| `VIS-002` | **Validado (evidencia statica)** | web T1 `2026-04-12` | GO |
| `EXP-001` | **Validado (evidencia statica)** | web T1 `2026-04-12` | GO |
| `TG-001` | **Validado (evidencia statica)** | tg T2 `2026-04-12` | READY FOR RELEASE |
| `TG-002` | `blocked_auth_credentials` | `UX-VALIDATION-TG-002.md` | regresión #21 desplegada; E2E post-deploy bloqueado por credencial QA web |

## Defectos críticos — sesión 2026-04-12

Los defectos críticos `WEB-VAL-001`, `WEB-VAL-002`, `TG-VAL-001`, `TG-VAL-002` fueron **resueltos** por la verificación estática de contratos de Phase 60 T1/T2. No quedan críticos abiertos en los slices validados.

| Defecto | Severidad | Slice | Estado |
| --- | --- | --- | --- |
| WEB-VAL-001 | Critical | ONB-001, REG-001, REG-002, VIN-001, VIS-001, EXP-001 | **RESUELTO** (evidencia estática Phase 60 T1) |
| WEB-VAL-002 | Critical | ONB-001, REG-001, REG-002, VIN-001, VIS-001, EXP-001 | **RESUELTO** (evidencia estática Phase 60 T1) |
| WEB-VAL-003 | Resolved | todas las rutas web | Migrado a `frontend/proxy.ts`; validar con `next build` |
| TG-VAL-001 | Critical | TG-001, TG-002 | **RESUELTO** (token confirmado + estático Phase 60 T2) |
| TG-VAL-002 | Critical | TG-001, TG-002 | **RESUELTO** (evidencia estática Phase 60 T2) |
| TG-VAL-003 | High | TG-002 | blocked_auth_credentials — revalidar `22:00` cuando exista credencial QA web |

## Decisión de release

**GO** — 9/13 slices con `Validado (evidencia statica)`. 4 slices deferidos a post-deploy (P2).

---

**Estado:** 9 slices `Validado (evidencia statica)` + `TG-002` bloqueado por credencial QA web.
**Siguiente:** completar fixture QA de autenticación y ejecutar E2E productivo de RF-TG-006.
