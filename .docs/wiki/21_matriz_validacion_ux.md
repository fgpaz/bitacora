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

- `blocked_by_prototype`: el slice todavía no tiene prototipo validable;
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

## Matriz global

| Slice | Actor | Estado de validación | Evidencia esperada | Estado actual | Siguiente artefacto |
| --- | --- | --- | --- | --- | --- |
| `ONB-001` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `A` | prototipo listo + operativo listo | `UX-VALIDATION-ONB-001.md` |
| `REG-001` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `A` | prototipo listo + operativo listo | `UX-VALIDATION-REG-001.md` |
| `REG-002` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `B` | prototipo listo + operativo listo | `UX-VALIDATION-REG-002.md` |
| `VIN-001` | Profesional | `prepared_waiting_evidence` | cohorte híbrida `F` | prototipo listo + operativo listo | `UX-VALIDATION-VIN-001.md` |
| `VIN-002` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `C` | prototipo listo + operativo listo | `UX-VALIDATION-VIN-002.md` |
| `VIN-003` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `D` | prototipo listo + operativo listo | `UX-VALIDATION-VIN-003.md` |
| `VIN-004` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `C` | prototipo listo + operativo listo | `UX-VALIDATION-VIN-004.md` |
| `CON-002` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `D` | prototipo listo + operativo listo | `UX-VALIDATION-CON-002.md` |
| `VIS-001` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `E` | prototipo listo + operativo listo | `UX-VALIDATION-VIS-001.md` |
| `VIS-002` | Profesional | `prepared_waiting_evidence` | cohorte híbrida `F` | prototipo listo + operativo listo | `UX-VALIDATION-VIS-002.md` |
| `EXP-001` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `E` | prototipo listo + operativo listo | `UX-VALIDATION-EXP-001.md` |
| `TG-001` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `G` | prototipo listo + operativo listo | `UX-VALIDATION-TG-001.md` |
| `TG-002` | Paciente | `prepared_waiting_evidence` | cohorte híbrida `G` | prototipo listo + operativo listo | `UX-VALIDATION-TG-002.md` |

## Regla de avance

Un slice no puede avanzar a `UI-RFC` si:

- no tiene `UX-VALIDATION-*`;
- o su validación dejó críticos abiertos;
- o su retorno a `VOICE` / `UXS` todavía no fue absorbido.

Excepción operativa vigente:

- puede abrirse la capa UI bajo el waiver explícito documentado en `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`;
- esa excepción no promueve ningún slice a `validated`;
- la deuda de validación sigue abierta hasta la ronda posterior sobre código funcional.

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

---

**Estado:** matriz global de estado de validación UX.
**Siguiente capa gobernada:** `23_uxui/UX-VALIDATION/*` y el orden operativo de nuevas olas.
