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
| `ONB-001` | ONB-first del paciente hasta consentimiento y puente al primer registro | Paciente | paquete manual listo para UI | authority pack manual aprobado | `UI-RFC/UI-RFC-ONB-001.md` + `HANDOFF-*` |
| `REG-001` | Registro rápido de humor vía web | Paciente | prototipo cerrado con drift pendiente | strict Stitch only | rerun Stitch corregido |
| `REG-002` | Registro de factores diarios vía web | Paciente | prototipo cerrado con drift pendiente | strict Stitch only | rerun Stitch corregido |
| `VIN-001` | Emisión de invitación profesional a paciente | Profesional | prototipo cerrado | pendiente de auditoría | `UX-VALIDATION` o rerun según auditoría |
| `VIN-002` | Auto-vinculación paciente a profesional por código | Paciente | prototipo cerrado | pendiente de auditoría | rerun con design pack derivado |
| `VIN-003` | Revocación de vínculo por paciente | Paciente | prototipo cerrado | pendiente de auditoría | rerun con design pack derivado |
| `VIN-004` | Gestión de acceso profesional por paciente | Paciente | prototipo cerrado | pendiente de auditoría | rerun con design pack derivado |
| `CON-002` | Revocación de consentimiento | Paciente | prototipo cerrado | pendiente de auditoría | rerun con design pack derivado |
| `VIS-001` | Timeline longitudinal del paciente | Paciente | prototipo cerrado | pendiente de auditoría | rerun bajo design pack derivado |
| `VIS-002` | Dashboard multi-paciente del profesional | Profesional | prototipo cerrado | pendiente de auditoría | completar cobertura y auditar |
| `EXP-001` | Exportación CSV del paciente | Paciente | prototipo cerrado | pendiente de auditoría | rerun con design pack derivado |
| `TG-001` | Vinculación de cuenta Telegram | Paciente | prototipo cerrado | pendiente de auditoría | rerun bajo design pack derivado |
| `TG-002` | Recordatorio y registro conversacional por Telegram | Paciente | prototipo cerrado | pendiente de auditoría | rerun bajo design pack derivado |

## Cadena canónica por caso

La cadena base de un slice visible sigue siendo:

`UXR -> UXI -> UJ -> VOICE -> UXS -> PROTOTYPE -> UX-VALIDATION -> UI-RFC -> HANDOFF-*`

Excepción operativa vigente:

- bajo waiver explícito, un slice puede abrir `UI-RFC` y `HANDOFF-*` antes de `UX-VALIDATION`;
- hoy esa excepción aplica únicamente a `ONB-001`.

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
| `UI-RFC` | iniciada; `ONB-001` abierto bajo excepción | `UI-RFC/UI-RFC-INDEX.md` |
| `HANDOFF-SPEC` | iniciada con `ONB-001` | `HANDOFF-SPEC/HANDOFF-SPEC-INDEX.md` |
| `HANDOFF-ASSETS` | iniciada con `ONB-001` | `HANDOFF-ASSETS/HANDOFF-ASSETS-INDEX.md` |
| `HANDOFF-MAPPING` | iniciada con `ONB-001` | `HANDOFF-MAPPING/HANDOFF-MAPPING-INDEX.md` |
| `HANDOFF-VISUAL-QA` | iniciada con `ONB-001` | `HANDOFF-VISUAL-QA/HANDOFF-VISUAL-QA-INDEX.md` |

## Regla operativa

- el waiver de entrada a UI no equivale a `UX-VALIDATION`;
- `ONB-001` puede avanzar a implementación documental y técnica;
- `REG-001` y `REG-002` siguen bloqueados por rerun/auditoría visual;
- el resto de los slices mantiene el gate previo hasta nueva evidencia.

---

**Estado:** índice raíz actualizado con excepción operativa solo para `ONB-001`.
**Slices abiertos para ejecución técnica:** `ONB-001`.
**Siguiente capa gobernada:** `UI-RFC-ONB-001.md` y `HANDOFF-*`.
