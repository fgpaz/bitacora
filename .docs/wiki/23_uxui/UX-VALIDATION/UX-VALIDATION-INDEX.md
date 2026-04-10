# UX-VALIDATION — Índice de validaciones por slice

## Propósito

Este índice lista el estado de validación UX de cada slice visible.

No reemplaza `21_matriz_validacion_ux.md` ni los futuros `UX-VALIDATION-*`. Su función es centralizar el punto de entrada de la familia `UX-VALIDATION` dentro de `23_uxui`.

## Documentos activos

Todavía no existen documentos `UX-VALIDATION-*` cerrados con evidencia real.

La familia queda iniciada con este índice y con las cohortes `A` a `G` ya preparadas.

## Cohortes preparadas

| Slice | Estado | Base disponible | Próximo artefacto |
| --- | --- | --- | --- |
| `ONB-001` | waiting evidence | prototipo + operativo | `UX-VALIDATION-ONB-001.md` |
| `REG-001` | waiting evidence | prototipo + operativo | `UX-VALIDATION-REG-001.md` |
| `REG-002` | waiting evidence | prototipo + operativo | `UX-VALIDATION-REG-002.md` |
| `VIN-002` | waiting evidence | prototipo + operativo | `UX-VALIDATION-VIN-002.md` |
| `VIN-004` | waiting evidence | prototipo + operativo | `UX-VALIDATION-VIN-004.md` |
| `VIN-003` | waiting evidence | prototipo + operativo | `UX-VALIDATION-VIN-003.md` |
| `CON-002` | waiting evidence | prototipo + operativo | `UX-VALIDATION-CON-002.md` |
| `VIS-001` | waiting evidence | prototipo + operativo | `UX-VALIDATION-VIS-001.md` |
| `EXP-001` | waiting evidence | prototipo + operativo | `UX-VALIDATION-EXP-001.md` |
| `VIN-001` | waiting evidence | prototipo + operativo | `UX-VALIDATION-VIN-001.md` |
| `VIS-002` | waiting evidence | prototipo + operativo | `UX-VALIDATION-VIS-002.md` |
| `TG-001` | waiting evidence | prototipo + operativo | `UX-VALIDATION-TG-001.md` |
| `TG-002` | waiting evidence | prototipo + operativo | `UX-VALIDATION-TG-002.md` |

## Waiver ejecutado

La cobertura documental restante de profesional y Telegram se completó bajo waiver explícito para cerrar el mapa UX visible del MVP, sin evidencia UX real.

Ese waiver no promueve ningún slice a `validated`: todos siguen esperando sesiones reales.

## Waiver vigente de entrada a UI

También existe un waiver explícito de entrada a UI en `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`.

Ese waiver permite avanzar en documentación UI antes de la validación, pero no cambia la deuda de esta familia:

- todos los slices siguen `waiting evidence`;
- ningún `UX-VALIDATION-*` debe crearse sin observación real;
- la validación queda diferida a una etapa posterior sobre código funcional.

## Regla de creación

Un `UX-VALIDATION-*` solo puede crearse cuando:

- existe prototipo enlazado y testeable;
- hubo evidencia moderada u observada suficiente;
- los hallazgos y severidades están explícitos;
- el retorno a `VOICE` o `UXS` quedó resuelto o claramente marcado.

---

**Estado:** familia `UX-VALIDATION` iniciada.
**Siguiente capa gobernada:** futuros `UX-VALIDATION-*` por slice y luego `UI-RFC-*`.
