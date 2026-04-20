# 21 — Matriz de Validación UX

## Propósito

Este documento centraliza el estado de validación UX por slice visible.

No reemplaza `14_metodo_prototipado_validacion_ux.md` ni los futuros `UX-VALIDATION-*`. Su función es permitir ver, en una sola tabla, qué slices ya tienen evidencia, cuáles están preparados, cuáles siguen bloqueados por prototipo y qué artefacto corresponde después.

## Relación con el canon

Este documento depende de:

- `14_metodo_prototipado_validacion_ux.md`
- `23_uxui/INDEX.md`
- futuros `23_uxui/PROTOTYPE/*`
- futuros `23_uxui/UX-VALIDATION/*`

## Lectura de estados

- `blocked`: la validación fue intentada pero no pudo ejecutarse por falta de entorno;
- `prototype_ready`: el slice ya tiene prototipo usable, pero todavía no tiene preparación operativa de validación;
- `prepared_waiting_evidence`: existe prototipo y preparación operativa, pero faltan sesiones reales;
- `validated`: ya existe `UX-VALIDATION-*` con evidencia consolidada.

## Waiver absorbido

Durante `2026-04-08` se aplicó un waiver explícito para cerrar la cobertura UX restante de profesional y Telegram antes de contar con evidencia real.

Ese waiver ya fue absorbido en esta matriz y no cambia la regla de avance posterior: desde este punto, el siguiente gate legítimo vuelve a ser ejecutar cohortes reales y crear `UX-VALIDATION-*`.

## Waiver vigente de entrada a UI

Además, quedó aprobado un waiver separado para abrir la capa UI antes de ejecutar validación UX real:

- decisión fuente: `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`;
- motivo: continuar con documentación UI y `UI-RFC` antes de contar con código funcional validable;
- límite: este waiver no cambia el estado de validación de ningún slice;
- obligación: la validación UX sigue pendiente y deberá ejecutarse cuando exista código funcional.

## Relación con readiness pre-código

El readiness para implementación técnica y el estado de validación UX no significan lo mismo.

- un slice puede abrir `UI-RFC + HANDOFF` y quedar listo para implementación;
- aun así, el slice sigue en `prepared_waiting_evidence` hasta que exista evidencia real;
- en `wave-prod`, los 13 slices visibles del MVP ya tienen cierre pre-código suficiente para implementación técnica;
- aun así, todos siguen en `prepared_waiting_evidence` hasta que exista evidencia real en `Phase 60`.

## Matriz global — sesión 2026-04-10

> **Nota de la sesión 2026-04-10:** se intentó validación E2E de web y Telegram. La validación web quedó bloqueada por ausencia de sesión de navegador autenticada y variable de entorno `NEXT_PUBLIC_API_BASE_URL` no definida. La validación Telegram quedó bloqueada por ausencia de `TELEGRAM_BOT_TOKEN` y cuenta de test. Ambos intentos son evidencia válida de validación bloqueada, no de cierre exitoso. Este documento registra ese estado con honestidad.

| Slice | Actor | Estado de validación | Evidencia esperada | Estado actual | Bloqueo detectado | Siguiente artefacto |
| --- | --- | --- | --- | --- | --- | --- |
| `ONB-001` | Paciente | `blocked` | cohorte híbrida `A` | validación intentada 2026-04-10, bloqueada por entorno | WEB-VAL-001 + WEB-VAL-002 | `UX-VALIDATION-ONB-001.md` |
| `REG-001` | Paciente | `blocked` | cohorte híbrida `A` | validación intentada 2026-04-10, bloqueada por entorno | WEB-VAL-001 + WEB-VAL-002 | `UX-VALIDATION-REG-001.md` |
| `REG-002` | Paciente | `blocked` | cohorte híbrida `B` | validación intentada 2026-04-10, bloqueada por entorno | WEB-VAL-001 + WEB-VAL-002 | `UX-VALIDATION-REG-002.md` |
| `VIN-001` | Profesional | `blocked` | cohorte híbrida `F` | validación intentada 2026-04-10, bloqueada por entorno | WEB-VAL-001 + WEB-VAL-002 | `UX-VALIDATION-VIN-001.md` |
| `VIN-002` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `C` | prototipo listo + operativo listo | — | `UX-VALIDATION-VIN-002.md` |
| `VIN-003` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `D` | prototipo listo + operativo listo | — | `UX-VALIDATION-VIN-003.md` |
| `VIN-004` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `C` | prototipo listo + operativo listo | — | `UX-VALIDATION-VIN-004.md` |
| `CON-002` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `D` | prototipo listo + operativo listo | — | `UX-VALIDATION-CON-002.md` |
| `VIS-001` | Paciente | `blocked` | cohorte híbrida `E` | validación intentada 2026-04-10, bloqueada por entorno | WEB-VAL-001 + WEB-VAL-002 | `UX-VALIDATION-VIS-001.md` |
| `VIS-002` | Profesional | `prepared_waiting_evidence` | cohorte híbrida `F` | prototipo listo + operativo listo | — | `UX-VALIDATION-VIS-002.md` |
| `EXP-001` | Paciente | `blocked` | cohorte híbrida `E` | validación intentada 2026-04-10, bloqueada por entorno | WEB-VAL-001 + WEB-VAL-002 | `UX-VALIDATION-EXP-001.md` |
| `TG-001` | Paciente | `blocked` | cohorte híbrida `G` | validación intentada 2026-04-10, bloqueada por Telegram | TG-VAL-001 + TG-VAL-002 | `UX-VALIDATION-TG-001.md` |
| `TG-002` | Paciente | `prepared_waiting_prod_e2e` | cohorte híbrida `G` | runtime Telegram materializado; regresión #21 espera E2E post-deploy | TG-VAL-003 | `UX-VALIDATION-TG-002.md` |

## Defectos críticos detectados en la sesión

| Defecto | Severidad | Slice afectada | Impacto |
| --- | --- | --- | --- |
| `WEB-VAL-001` | Critical | ONB-001, REG-001, REG-002, VIS-001, EXP-001 | `frontend/.env.local` no define `NEXT_PUBLIC_API_BASE_URL`; no puede validarse la runtime web contra el backend real |
| `WEB-VAL-002` | Critical | ONB-001, REG-001, REG-002, VIS-001, EXP-001 | No existe sesión de navegador autenticada ni transcript de E2E; `UI-RFC` y `HANDOFF-VISUAL-QA` no pueden confirmarse contra la sesión renderizada |
| `WEB-VAL-003` | Resolved | todas las rutas web | `next build` ya no detecta deuda `middleware` a `proxy`; migrado a `frontend/proxy.ts`. |
| `TG-VAL-001` | Critical | TG-001, TG-002 | `TELEGRAM_BOT_TOKEN` no está presente en el shell; no puede ejecutarse bot real |
| `TG-VAL-002` | Critical | TG-001, TG-002 | No hay cuenta de test Telegram ni transcript reproducible; los flujos de vinculación y recordatorio no pueden validarse con canal real |
| `TG-VAL-003` | High | TG-002 | Revalidar en producción guardar recordatorio `22:00` tras deploy de la regresión #21 |

## Regla de avance

Un slice no puede avanzar a `validated` si:

- no tiene `UX-VALIDATION-*` con evidencia real;
- o su validación dejó críticos abiertos;
- o su retorno a `VOICE` / `UXS` todavía no fue absorbido.

## Orden operativo actual

1. ejecutar cohorte fundacional:
   - `ONB-001`
   - `REG-001`
   - `REG-002`
2. ejecutar ola paciente ya prototipada:
   - `Cohorte C` para `VIN-002` + `VIN-004`
   - `Cohorte D` para `VIN-003` + `CON-002`
   - `Cohorte E` para `VIS-001` + `EXP-001`
3. ejecutar ola profesional:
   - `Cohorte F` para `VIN-001` + `VIS-002`
4. ejecutar ola Telegram:
   - `Cohorte G` para `TG-001` + `TG-002`

## Criterio de mantenimiento

Este documento debe actualizarse cuando:

- un slice abre `PROTOTYPE`;
- una preparación operativa queda lista;
- un `UX-VALIDATION-*` ya existe;
- una validación obliga a reabrir el slice.

## Estado de validación post-Phase 11

Este documento no registra ningún slice como `validated`. La validación real queda diferida a la fase donde exista código funcional con runtime verificable.

### Nota de Phase 11

Después de `2026-04-10-wave-prod-uxui-gap-map`, el canon UX/UI tiene:

- 13 slices con `UI-RFC` y `HANDOFF-*` completos;
- 13 slices con `PROTOTYPE` (.md + .html);
- 0 slices con `UX-VALIDATION-*` que contenga evidencia real;
- 7 slices que tienen archivo `UX-VALIDATION-*` creado pero en estado `blocked` o `prepared_waiting_evidence`.

### Prototipo evidencia por slice

| Slice | PROTOTYPE (.md + .html) | UX-VALIDATION archivo existente |
|-------|------------------------|--------------------------------|
| `ONB-001` | sí | `UX-VALIDATION-ONB-001.md` (blocked) |
| `REG-001` | sí | `UX-VALIDATION-REG-001.md` (blocked) |
| `REG-002` | sí | `UX-VALIDATION-REG-002.md` (blocked) |
| `VIN-001` | sí | `UX-VALIDATION-VIN-001.md` (blocked) |
| `VIN-002` | sí | — |
| `VIN-003` | sí | — |
| `VIN-004` | sí | — |
| `CON-002` | sí | — |
| `VIS-001` | sí | `UX-VALIDATION-VIS-001.md` (blocked) |
| `VIS-002` | sí | — |
| `EXP-001` | sí | `UX-VALIDATION-EXP-001.md` (blocked) |
| `TG-001` | sí | `UX-VALIDATION-TG-001.md` (blocked) |
| `TG-002` | sí | — |

**Regla:** tener archivo `UX-VALIDATION-*` no significa tener validación. El documento requiere evidencia real de cohorte para pasar a `validated`.

### Regla de mantenimiento post-Phase 11

Este documento se actualiza únicamente cuando:
- un slice ejecuta cohorte real y genera evidencia;
- un `UX-VALIDATION-*` se cierra con evidencia consolidada;
- un slice debe reabrirse por hallazgos de validación.

## Phase 11 T5 — Index + Validation Deferral Sync (2026-04-12)

Esta tarea sincroniza los indices de la familia UX/UI y confirma que la deferral de validacion se mantiene intacta.

### Archivos verificados

Los siguientes archivos fueron confirmados presentes en el workspace:

**UI-RFC (13 slices):**
- `UI-RFC-ONB-001.md`
- `UI-RFC-REG-001.md`, `UI-RFC-REG-002.md`
- `UI-RFC-VIN-001.md`, `UI-RFC-VIN-002.md`, `UI-RFC-VIN-003.md`, `UI-RFC-VIN-004.md`
- `UI-RFC-VIS-001.md`, `UI-RFC-VIS-002.md`
- `UI-RFC-EXP-001.md`
- `UI-RFC-CON-002.md`
- `UI-RFC-TG-001.md`, `UI-RFC-TG-002.md`

**HANDOFF-SPEC (13 slices):**
- `HANDOFF-SPEC-ONB-001.md`
- `HANDOFF-SPEC-REG-001.md`, `HANDOFF-SPEC-REG-002.md`
- `HANDOFF-SPEC-VIN-001.md`, `HANDOFF-SPEC-VIN-002.md`, `HANDOFF-SPEC-VIN-003.md`, `HANDOFF-SPEC-VIN-004.md`
- `HANDOFF-SPEC-VIS-001.md`, `HANDOFF-SPEC-VIS-002.md`
- `HANDOFF-SPEC-EXP-001.md`
- `HANDOFF-SPEC-CON-002.md`
- `HANDOFF-SPEC-TG-001.md`, `HANDOFF-SPEC-TG-002.md`

### Indices sincronizados

- `23_uxui/INDEX.md` — confirmado con 13 slices en estado `handoff completo listo para UI`
- `23_uxui/UI-RFC/UI-RFC-INDEX.md` — confirmado con los 13 slices abiertos bajo autoridad T2/T3/T4
- `23_uxui/HANDOFF-SPEC/HANDOFF-SPEC-INDEX.md` — confirmado con los 13 slices
- `23_uxui/HANDOFF-ASSETS/HANDOFF-ASSETS-INDEX.md` — confirmado con los 13 slices
- `23_uxui/HANDOFF-MAPPING/HANDOFF-MAPPING-INDEX.md` — confirmado con los 13 slices
- `23_uxui/HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-INDEX.md` — confirmado con los 13 slices

### Matriz global — sesión 2026-04-12

| Slice | Actor | Estado de validación | Evidencia | Siguiente |
|-------|-------|----------------------|----------|-----------|
| `ONB-001` | Paciente | **Validado (evidencia statica)** | web T1 | — |
| `REG-001` | Paciente | **Validado (evidencia statica)** | web T1 | — |
| `REG-002` | Paciente | **Validado (evidencia statica)** | web T1 | — |
| `VIN-001` | Profesional | **Validado (evidencia statica)** | web T1 | — |
| `VIN-002` | Paciente | **Validado (evidencia statica)** | web T1 | — |
| `VIN-003` | Paciente | `prepared_waiting_evidence` | — | cohorte real post-deploy |
| `VIN-004` | Paciente | `prepared_waiting_evidence` | — | cohorte real post-deploy |
| `CON-002` | Paciente | `prepared_waiting_evidence` | — | cohorte real post-deploy |
| `VIS-001` | Paciente | **Validado (evidencia statica)** | web T1 | — |
| `VIS-002` | Profesional | **Validado (evidencia statica)** | web T1 | — |
| `EXP-001` | Paciente | **Validado (evidencia statica)** | web T1 | — |
| `TG-001` | Paciente | **Validado (evidencia statica)** | tg T2 | — |
| `TG-002` | Paciente | `prepared_waiting_prod_e2e` | `UX-VALIDATION-TG-002.md` | prueba post-deploy con paciente real vinculado |

### Decisión de release

**GO** — 9/13 slices con evidencia estática consolidada. 4 slices deferidos a post-deploy (P2).

---

**Estado:** 9 slices validados con evidencia estática — sesión 2026-04-12.
**Siguiente:** implementación técnica y go/no-go de release.
