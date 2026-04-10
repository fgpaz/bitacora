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

Excepción aprobada:

- `ONB-001` abre `UI-RFC` y `HANDOFF-*` por authority pack manual `2026-04-10`;
- esta excepción no cambia el estado de validación UX del slice;
- esta excepción no habilita automáticamente `REG-001`, `REG-002` ni el resto de los slices.

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
| 1 | `ONB-001` | `UI-RFC abierto` | authority pack manual aprobado | no | consumir `UI-RFC-ONB-001.md` + `HANDOFF-*` en `T04/T05` |
| 2 | `REG-001` | `auditado con hallazgos` | strict Stitch only | sí | rerun Stitch corregido + nueva auditoría manual |
| 3 | `REG-002` | `auditado con hallazgos` | strict Stitch only | sí | rerun Stitch corregido + nueva auditoría manual |
| 4 | `VIN-002` | `pendiente de auditoría` | strict Stitch only | pendiente | rerun con design pack derivado y auditar |
| 5 | `VIN-004` | `pendiente de auditoría` | strict Stitch only | pendiente | rerun con design pack derivado y auditar |
| 6 | `VIN-003` | `pendiente de auditoría` | strict Stitch only | pendiente | rerun con design pack derivado y auditar |
| 7 | `CON-002` | `pendiente de auditoría` | strict Stitch only | pendiente | rerun con design pack derivado y auditar |
| 8 | `VIS-001` | `pendiente de auditoría` | strict Stitch only | pendiente | rerun bajo design pack derivado y auditar |
| 9 | `EXP-001` | `pendiente de auditoría` | strict Stitch only | pendiente | rerun con design pack derivado y auditar |
| 10 | `VIN-001` | `pendiente de auditoría` | strict Stitch only | pendiente | rerun bajo design pack derivado y auditar |
| 11 | `VIS-002` | `pendiente de auditoría` | strict Stitch only | pendiente | completar cobertura y auditar |
| 12 | `TG-001` | `pendiente de auditoría` | strict Stitch only | pendiente | rerun bajo design pack derivado y auditar |
| 13 | `TG-002` | `pendiente de auditoría` | strict Stitch only | pendiente | rerun bajo design pack derivado y auditar |

## Cola operativa después de ONB

1. `REG-001`
2. `REG-002`
3. `VIN-002`
4. `VIN-004`
5. `VIN-003`
6. `CON-002`
7. `VIS-001`
8. `EXP-001`
9. `VIN-001`
10. `VIS-002`
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
- abierta en `ONB-001` bajo excepción explícita;
- todavía bloqueada o pendiente en el resto de los slices.

---

**Estado:** índice operativo actualizado con `ONB-001` abierto para contrato técnico UI.
**Siguiente capa gobernada:** `UI-RFC-ONB-001.md` y futuros `UI-RFC-*` cuando sus gates se destraben.
