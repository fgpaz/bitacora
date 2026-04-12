# UI-RFC — Índice de contratos técnicos UI

## Propósito

Este índice gobierna la apertura de `UI-RFC-*` por slice visible.

No reemplaza `PROTOTYPE-*`, no declara `UX-VALIDATION` y no habilita implementación indiscriminada. Su función es dejar explícito qué slices ya tienen autoridad suficiente para bajar contrato técnico UI y cuáles siguen bloqueados por gate visual.

## Relación con el canon

Este índice depende de:

- `../../16_patrones_ui.md`
- `../../21_matriz_validacion_ux.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../INDEX.md`
- `../PROTOTYPE/PROTOTYPE-INDEX.md`
- `../UX-VALIDATION/UX-VALIDATION-INDEX.md`
- `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`
- `.docs/raw/decisiones/2026-04-10-onb-001-manual-authority-pack.md`

## Regla vigente

La regla general de esta etapa sigue siendo `strict Stitch only`.

Excepciones aprobadas:

- `ONB-001` abre `UI-RFC` y `HANDOFF-*` por authority pack manual `2026-04-10`;
- `VIN-001..004`, `VIS-001..002`, `EXP-001`, `CON-002` abren `UI-RFC` y `HANDOFF-*` bajo autoridad T3;
- estas excepciones no cambian el estado de validación UX de ningún slice;
- estas excepciones no habilitan automáticamente `REG-001` ni `REG-002`.

## Secciones obligatorias de un `UI-RFC-*`

Todo `UI-RFC-*` debe fijar:

1. alcance visible del slice;
2. referencias a `VOICE`, `UXS`, `PROTOTYPE`, `RF`, `TP`, `07` y `09`;
3. inventario de estados obligatorios;
4. composición, jerarquía y responsive;
5. gramática de componentes;
6. contratos backend y manejo de errores;
7. dependencias abiertas y límites de implementación.

## Estado por slice

| Orden | Slice | Estado UI-RFC actual | Gate vigente | Bloqueo | Siguiente acción |
| --- | --- | --- | --- | --- | --- |
| 1 | `ONB-001` | `UI-RFC abierto` | authority pack manual aprobado | no | handoff completo disponible |
| 2 | `REG-001` | `UI-RFC abierto` | authority T2 | no | handoff completo disponible |
| 3 | `REG-002` | `UI-RFC abierto` | authority T2 | no | handoff completo disponible |
| 4 | `VIN-001` | `UI-RFC abierto` | authority T3 | no | handoff completo disponible |
| 5 | `VIN-002` | `UI-RFC abierto` | authority T3 | no | handoff completo disponible |
| 6 | `VIN-003` | `UI-RFC abierto` | authority T3 | no | handoff completo disponible |
| 7 | `VIN-004` | `UI-RFC abierto` | authority T3 | no | handoff completo disponible |
| 8 | `VIS-001` | `UI-RFC abierto` | authority T3 | no | handoff completo disponible |
| 9 | `VIS-002` | `UI-RFC abierto` | authority T3 | no | handoff completo disponible |
| 10 | `EXP-001` | `UI-RFC abierto` | authority T3 | no | handoff completo disponible |
| 11 | `CON-002` | `UI-RFC abierto` | authority T3 | no | handoff completo disponible |
| 12 | `TG-001` | `UI-RFC abierto` | gap map 2026-04-10 | no | consumir en backend/telegram |
| 13 | `TG-002` | `UI-RFC abierto` | gap map 2026-04-10 | no | consumir en backend/telegram |

## Cola operativa después de ONB

1. `REG-001`
2. `REG-002`
3. `VIN-001`
4. `VIN-002`
5. `VIN-003`
6. `VIN-004`
7. `VIS-001`
8. `VIS-002`
9. `EXP-001`
10. `CON-002`
11. `TG-001`
12. `TG-002`

## Regla de trazabilidad

Cada `UI-RFC-*` abierto debe enlazar:

- `VOICE-*`;
- `UXS-*`;
- `PROTOTYPE-*`;
- `04_RF/*` y `06_pruebas/*` relevantes;
- `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`;
- `09_contratos_tecnicos.md`.

## Estado de la familia

La familia `UI-RFC` queda:

- iniciada a nivel global;
- abierta en los 13 slices visibles del MVP;
- `ONB-001` conserva su authority pack manual como antecedente de apertura;
- `REG-001` y `REG-002` quedan abiertos bajo autoridad T2;
- `VIN-001..004`, `VIS-001..002`, `EXP-001`, `CON-002` quedan abiertos bajo autoridad T3;
- `TG-001` y `TG-002` quedan abiertos bajo autoridad T4.

---

**Estado:** índice operativo actualizado con los 13 slices visibles del MVP abiertos para contrato técnico UI.
**Siguiente capa gobernada:** sus respectivos `UI-RFC-*` y `HANDOFF-*`.
