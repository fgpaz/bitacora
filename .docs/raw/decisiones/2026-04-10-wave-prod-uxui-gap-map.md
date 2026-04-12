# Decisión Operativa — Wave-prod UX/UI Gap Map

**Fecha:** 2026-04-12 (actualizado desde 2026-04-10)
**Tipo:** gap inventory
**Slug:** wave-prod-uxui-gap-map
**Pertenece a:** Phase 11, Task T1

## Propósito

Este documento establece el inventario explícito de gaps UX/UI por slice visible del MVP Bitácora. Su función es que el equipo y los agentes subordinados puedan consultar en una sola lectura qué existe y qué falta en cada slice, sin ambigüedad.

## Regla operativa

- **no se ejecuta UX validation en este documento**
- **ningún slice se marca como validado**
- **toda afirmación es verificable contra ruta canónica en `.docs/wiki/23_uxui/`**

---

## A) Ready for Code — UI-RFC + handoff completo

Estos slices tienen cobertura completa de la cadena: `UXR → UXI → UJ → VOICE → UXS → PROTOTYPE + UI-RFC + 4xHANDOFF`.

| Slice | Actor | UI-RFC | HANDOFF-SPEC | HANDOFF-ASSETS | HANDOFF-MAPPING | HANDOFF-VISUAL-QA | PROTOTYPE |
|-------|-------|--------|-------------|----------------|-----------------|-------------------|-----------|
| `ONB-001` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |
| `REG-001` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |
| `REG-002` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |
| `VIN-001` | Profesional | sí | sí | sí | sí | sí | sí (.html + .md) |
| `VIN-002` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |
| `VIN-003` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |
| `VIN-004` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |
| `CON-002` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |
| `VIS-001` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |
| `VIS-002` | Profesional | sí | sí | sí | sí | sí | sí (.html + .md) |
| `EXP-001` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |
| `TG-001` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |
| `TG-002` | Paciente | sí | sí | sí | sí | sí | sí (.html + .md) |

**13 de 13 slices** en esta categoría. Todos abierta bajo waiverauthority.

---

## B) Arquitectura de handoff por familia

| Familia | Índice | Estado global |
|---------|--------|---------------|
| `UXR` | `UXR/UXR-INDEX.md` | cerrada para todos los slices |
| `UXI` | `UXI/UXI-INDEX.md` | cerrada para todos los slices |
| `UJ` | `UJ/UJ-INDEX.md` | cerrada para todos los slices |
| `VOICE` | `VOICE/VOICE-INDEX.md` | cerrada para todos los slices |
| `UXS` | `UXS/UXS-INDEX.md` | cerrada para todos los slices |
| `PROTOTYPE` | `PROTOTYPE/PROTOTYPE-INDEX.md` | iniciada para los 13 slices |
| `UX-VALIDATION` | `UX-VALIDATION/UX-VALIDATION-INDEX.md` | iniciada y en espera de evidencia real |
| `UI-RFC` | `UI-RFC/UI-RFC-INDEX.md` | completa en modo pre-código para los 13 slices |
| `HANDOFF-SPEC` | `HANDOFF-SPEC/HANDOFF-SPEC-INDEX.md` | completa en modo pre-código para los 13 slices |
| `HANDOFF-ASSETS` | `HANDOFF-ASSETS/HANDOFF-ASSETS-INDEX.md` | completa en modo pre-código para los 13 slices |
| `HANDOFF-MAPPING` | `HANDOFF-MAPPING/HANDOFF-MAPPING-INDEX.md` | completa en modo pre-código para los 13 slices |
| `HANDOFF-VISUAL-QA` | `HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-INDEX.md` | completa en modo pre-código para los 13 slices |

---

## C) Validación diferida — post-code only

Ningún slice tiene `UX-VALIDATION-*` con evidencia real.

| Slice | UX-VALIDATION-* existente | Estado en matriz 21 |
|-------|---------------------------|----------------------|
| `ONB-001` | `UX-VALIDATION-ONB-001.md` | `blocked` (2026-04-10, entorno) |
| `REG-001` | `UX-VALIDATION-REG-001.md` | `blocked` (2026-04-10, entorno) |
| `REG-002` | `UX-VALIDATION-REG-002.md` | `blocked` (2026-04-10, entorno) |
| `VIN-001` | `UX-VALIDATION-VIN-001.md` | `blocked` (2026-04-10, entorno) |
| `VIN-002` | — | `prepared_waiting_evidence` |
| `VIN-003` | — | `prepared_waiting_evidence` |
| `VIN-004` | — | `prepared_waiting_evidence` |
| `CON-002` | — | `prepared_waiting_evidence` |
| `VIS-001` | `UX-VALIDATION-VIS-001.md` | `blocked` (2026-04-10, entorno) |
| `VIS-002` | — | `prepared_waiting_evidence` |
| `EXP-001` | `UX-VALIDATION-EXP-001.md` | `blocked` (2026-04-10, entorno) |
| `TG-001` | `UX-VALIDATION-TG-001.md` | `blocked` (2026-04-10, Telegram) |
| `TG-002` | — | `blocked` (2026-04-10, Telegram) |

**Slices con UX-VALIDATION-* creado:** 7 (`ONB-001`, `REG-001`, `REG-002`, `VIN-001`, `VIS-001`, `EXP-001`, `TG-001`)
**Slices sin UX-VALIDATION-* :** 6 (`VIN-002`, `VIN-003`, `VIN-004`, `CON-002`, `VIS-002`, `TG-002`)
**Todos en estado `blocked` o `prepared_waiting_evidence` — ninguno validado.**

---

## D) Prototipo evidencia por slice

| Slice | PROTOTYPE archivo | PROTOTYPE HTML |
|-------|------------------|----------------|
| `ONB-001` | PROTOTYPE-ONB-001.md | PROTOTYPE-ONB-001.html |
| `REG-001` | PROTOTYPE-REG-001.md | PROTOTYPE-REG-001.html |
| `REG-002` | PROTOTYPE-REG-002.md | PROTOTYPE-REG-002.html |
| `VIN-001` | PROTOTYPE-VIN-001.md | PROTOTYPE-VIN-001.html |
| `VIN-002` | PROTOTYPE-VIN-002.md | PROTOTYPE-VIN-002.html |
| `VIN-003` | PROTOTYPE-VIN-003.md | PROTOTYPE-VIN-003.html |
| `VIN-004` | PROTOTYPE-VIN-004.md | PROTOTYPE-VIN-004.html |
| `CON-002` | PROTOTYPE-CON-002.md | PROTOTYPE-CON-002.html |
| `VIS-001` | PROTOTYPE-VIS-001.md | PROTOTYPE-VIS-001.html |
| `VIS-002` | PROTOTYPE-VIS-002.md | PROTOTYPE-VIS-002.html |
| `EXP-001` | PROTOTYPE-EXP-001.md | PROTOTYPE-EXP-001.html |
| `TG-001` | PROTOTYPE-TG-001.md | PROTOTYPE-TG-001.html |
| `TG-002` | PROTOTYPE-TG-002.md | PROTOTYPE-TG-002.html |

**13 de 13 slices** tienen PROTOTYPE.md + PROTOTYPE.html.

---

## E) Dependencias backend bloqueantes

| Slice | Backend | Impacto |
|-------|---------|---------|
| `VIN-001` | diferido (Phase 30) | no cerrar implementación sin backend |
| `VIN-002` | diferido (Phase 30) | no cerrar implementación sin backend |
| `VIN-003` | diferido (Phase 30) | no cerrar implementación sin backend |
| `VIN-004` | diferido (Phase 30) | no cerrar implementación sin backend |
| `VIS-001` | diferido (Phase 31) | no cerrar implementación sin backend |
| `VIS-002` | diferido (Phase 31) | no cerrar implementación sin backend |
| `EXP-001` | diferido (Phase 31) | no cerrar implementación sin backend |
| `TG-001` | diferido (Phase 31) | no cerrar implementación sin backend |
| `TG-002` | diferido (Phase 31) | no cerrar implementación sin backend |
| `ONB-001` | materializado | listo para frontend |
| `REG-001` | materializado | listo para frontend |
| `REG-002` | materializado | listo para frontend |
| `CON-002` | parcial (revoke existe; cascadas diferidas) | UI con dependencias explícitas |

---

## F) Resumen de estado

| Categoría | Cantidad |
|-----------|----------|
| Ready for code (UI-RFC + 4xHANDOFF + PROTOTYPE) | 13 / 13 |
| Con UX-VALIDATION-* creado | 7 |
| Sin UX-VALIDATION-* | 6 |
| Validado con evidencia real | 0 |
| Bloqueado por backend pendiente | 9 |
| Listo para implementación frontend (backend materializado) | 3 (`ONB-001`, `REG-001`, `REG-002`) |

---

## G) Defectos críticos desde sesión 2026-04-10

| Defecto | Severidad | Slice afectada | Impacto |
|---------|-----------|----------------|---------|
| `WEB-VAL-001` | Critical | ONB-001, REG-001, REG-002, VIS-001, EXP-001 | `frontend/.env.local` no define `NEXT_PUBLIC_API_BASE_URL`; no puede validarse la runtime web contra el backend real |
| `WEB-VAL-002` | Critical | ONB-001, REG-001, REG-002, VIS-001, EXP-001 | No existe sesión de navegador autenticada ni transcript de E2E; `UI-RFC` y `HANDOFF-VISUAL-QA` no pueden confirmarse contra la sesión renderizada |
| `WEB-VAL-003` | High | todas las rutas web | `next build` detecta que `middleware` migrará a `proxy`; deuda de migración |
| `TG-VAL-001` | Critical | TG-001, TG-002 | `TELEGRAM_BOT_TOKEN` no está presente en el shell; no puede ejecutarse bot real |
| `TG-VAL-002` | Critical | TG-001, TG-002 | No hay cuenta de test Telegram ni transcript reproducible; los flujos de vinculación y recordatorio no pueden validarse con canal real |
| `TG-VAL-003` | High | TG-002 | El path de recordatorio depende de conectividad real al Bot API sin credencial de fallback |

---

**Estado:** gap map actualizado — 2026-04-12.
**Siguiente:** usar este documento como referencia base para Task T2 (revisar si hay gaps reales en UI-RFC que necesiten cierre antes de implementación).