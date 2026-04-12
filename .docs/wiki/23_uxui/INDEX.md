# 23 — UX/UI por caso

## Propósito

Este directorio agrupa los artefactos UX/UI por slice visible.

Desde esta normalización, la propiedad `UX del caso` vive por familia dentro de `23_uxui`: `UXR`, `UXI`, `UJ`, `VOICE`, `UXS`, `PROTOTYPE`, `UX-VALIDATION`, `UI-RFC` y `HANDOFF-*`.

## Cierre de la capa visual global

La dirección visual global ya está cerrada.

Las decisiones nuevas a nivel slice solo se justifican si impactan materialmente:

- jerarquía;
- modelo de estados;
- contratos backend;
- accesibilidad;
- reutilización;
- handoff hacia implementación.

## Estado global por slice

| Slice | Caso | Actor principal | Estado actual | Gate operativo | Siguiente artefacto operativo |
| --- | --- | --- | --- | --- | --- |
| `ONB-001` | ONB-first del paciente hasta consentimiento y puente al primer registro | Paciente | paquete completo listo para UI | authority pack manual aprobado | UI-RFC + HANDOFF-* completos |
| `REG-001` | Registro rápido de humor vía web | Paciente | handoff completo listo para UI | authority T2 | implementación frontend paciente |
| `REG-002` | Registro de factores diarios vía web | Paciente | handoff completo listo para UI | authority T2 | implementación frontend paciente |
| `VIN-001` | Emisión de invitación profesional a paciente | Profesional | handoff completo listo para UI | authority T3 | implementación frontend profesional |
| `VIN-002` | Auto-vinculación paciente a profesional por código | Paciente | handoff completo listo para UI | authority T3 | implementación frontend profesional |
| `VIN-003` | Revocación de vínculo por paciente | Paciente | handoff completo listo para UI | authority T3 | implementación frontend profesional |
| `VIN-004` | Gestión de acceso profesional por paciente | Paciente | handoff completo listo para UI | authority T3 | implementación frontend profesional |
| `CON-002` | Revocación de consentimiento | Paciente | handoff completo listo para UI | authority T3 | implementación frontend paciente/profesional según ruta |
| `VIS-001` | Timeline longitudinal del paciente | Paciente | handoff completo listo para UI | authority T3 | implementación frontend paciente |
| `VIS-002` | Dashboard multi-paciente del profesional | Profesional | handoff completo listo para UI | authority T3 | implementación frontend profesional |
| `EXP-001` | Exportación CSV del paciente | Paciente | handoff completo listo para UI | authority T3 | implementación frontend profesional/paciente según contrato |
| `TG-001` | Vinculación de cuenta Telegram | Paciente | handoff completo listo para UI | authority T4 | implementación backend/telegram + puente web |
| `TG-002` | Recordatorio y registro conversacional por Telegram | Paciente | handoff completo listo para UI | authority T4 | implementación backend/telegram |

## Cadena canónica por caso

La cadena base de un slice visible sigue siendo:

`UXR -> UXI -> UJ -> VOICE -> UXS -> PROTOTYPE -> UX-VALIDATION -> UI-RFC -> HANDOFF-*`

Excepción operativa vigente:

- bajo waiver explícito, un slice puede abrir `UI-RFC` y `HANDOFF-*` antes de `UX-VALIDATION`;
- en `wave-prod`, ese readiness pre-código ya quedó abierto para los 13 slices visibles del MVP;
- esa apertura no cambia el estado de validación UX de ningún slice.

## Estado por familia

| Familia | Estado | Artefacto índice |
| --- | --- | --- |
| `UXR` | cerrada para todos los slices visibles del MVP | `UXR/UXR-INDEX.md` |
| `UXI` | cerrada para todos los slices visibles del MVP | `UXI/UXI-INDEX.md` |
| `UJ` | cerrada para todos los slices visibles del MVP | `UJ/UJ-INDEX.md` |
| `VOICE` | cerrada para todos los slices visibles del MVP | `VOICE/VOICE-INDEX.md` |
| `UXS` | cerrada para todos los slices visibles del MVP | `UXS/UXS-INDEX.md` |
| `PROTOTYPE` | iniciada para los `13` slices visibles del MVP | `PROTOTYPE/PROTOTYPE-INDEX.md` |
| `UX-VALIDATION` | iniciada y en espera de evidencia real | `UX-VALIDATION/UX-VALIDATION-INDEX.md` |
| `UI-RFC` | completa en modo pre-código para los 13 slices visibles del MVP | `UI-RFC/UI-RFC-INDEX.md` |
| `HANDOFF-SPEC` | completa en modo pre-código para los 13 slices visibles del MVP | `HANDOFF-SPEC/HANDOFF-SPEC-INDEX.md` |
| `HANDOFF-ASSETS` | completa en modo pre-código para los 13 slices visibles del MVP | `HANDOFF-ASSETS/HANDOFF-ASSETS-INDEX.md` |
| `HANDOFF-MAPPING` | completa en modo pre-código para los 13 slices visibles del MVP | `HANDOFF-MAPPING/HANDOFF-MAPPING-INDEX.md` |
| `HANDOFF-VISUAL-QA` | completa en modo pre-código para los 13 slices visibles del MVP | `HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-INDEX.md` |

## Regla operativa

- el waiver de entrada a UI no equivale a `UX-VALIDATION`;
- los 13 slices visibles ya tienen cierre pre-código suficiente para implementación técnica;
- ningún slice pasa a `validated` por tener `UI-RFC + HANDOFF-*` completos;
- toda validación real sigue diferida a `Phase 60`.

## Dependencia backend por slice

| Slice | Estado backend actual | Impacto sobre implementación |
| --- | --- | --- |
| `ONB-001` | backend ya materializado (`auth/bootstrap`, `consent`) | listo para implementación frontend |
| `REG-001` | backend ya materializado (`mood-entries`) | listo para implementación frontend |
| `REG-002` | backend ya materializado (`daily-checkins`) | listo para implementación frontend |
| `CON-002` | backend parcial (`consent revoke` existe; cascadas diferidas) | implementar UI con dependencias backend explícitas |
| `VIN-001` | backend diferido | no cerrar implementación sin Phase 30 |
| `VIN-002` | backend diferido | no cerrar implementación sin Phase 30 |
| `VIN-003` | backend diferido | no cerrar implementación sin Phase 30 |
| `VIN-004` | backend diferido | no cerrar implementación sin Phase 30 |
| `VIS-001` | backend diferido | no cerrar implementación sin Phase 31 |
| `VIS-002` | backend diferido | no cerrar implementación sin Phase 31 |
| `EXP-001` | backend diferido | no cerrar implementación sin Phase 31 |
| `TG-001` | backend/telegram diferido | no cerrar implementación sin Phase 31 |
| `TG-002` | backend/telegram diferido | no cerrar implementación sin Phase 31 |

---

**Estado:** índice raíz actualizado con cadena handoff completa para los 13 slices visibles del MVP.
**Siguiente capa gobernada:** implementación técnica y luego `UX-VALIDATION-*` cuando exista runtime y evidencia real.
