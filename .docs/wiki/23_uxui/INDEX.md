# 23 — UX/UI por caso

## Propósito

Este directorio agrupa los artefactos UX/UI por slice visible.

Desde esta normalización, la propiedad `UX del caso` vive por familia dentro de `23_uxui`: `UXR`, `UXI`, `UJ`, `VOICE` y `UXS`. Las capas posteriores siguen en `PROTOTYPE`, `UX-VALIDATION`, `UI-RFC` y `HANDOFF-*`.

## Estado global por slice

## Cierre de la capa visual global

La dirección de la capa global ya está cerrada por decisiones explícitas y no se reabrirá aquí.  
Las futuras preguntas de UI a nivel slice, incluyendo las que afecten `UI-RFC`, solo se justifican si el cambio afecta materialmente:
- límites de componente,
- jerarquía global,
- modelo de estados,
- expectativas de backend,
- comportamiento de accesibilidad,
- reutilización entre slices.

Este cierre global no implica avance automático de slice ni relajación del gate visual vigente.
La tabla siguiente separa el estado documental del slice de su gate operativo para entrada a `UI-RFC`.

| Slice | Caso | Actor principal | Estado actual | Gate Stitch UI-RFC | Siguiente artefacto esperado |
| --- | --- | --- | --- | --- | --- |
| `ONB-001` | Onboarding invitado del paciente hasta primer MoodEntry | Paciente | Prototipo cerrado | `requiere corrección visual` | nueva corrida Stitch de `ONB-001` |
| `REG-001` | Registro rápido de humor vía web | Paciente | Prototipo cerrado | `requiere corrección visual` | nueva corrida Stitch de `REG-001` |
| `REG-002` | Registro de factores diarios vía web | Paciente | Prototipo cerrado | `requiere corrección visual` | nueva corrida Stitch de `REG-002` |
| `VIN-001` | Emisión de invitación profesional a paciente | Profesional | Prototipo cerrado | `pendiente de auditoría` | `23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-001.md` |
| `VIN-002` | Auto-vinculación paciente a profesional por código | Paciente | Prototipo cerrado | `pendiente de auditoría` | `23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-002.md` |
| `VIN-003` | Revocación de vínculo por paciente | Paciente | Prototipo cerrado | `pendiente de auditoría` | `23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-003.md` |
| `VIN-004` | Gestión de acceso profesional por paciente | Paciente | Prototipo cerrado | `pendiente de auditoría` | `23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-004.md` |
| `CON-002` | Revocación de consentimiento | Paciente | Prototipo cerrado | `pendiente de auditoría` | `23_uxui/UX-VALIDATION/UX-VALIDATION-CON-002.md` |
| `VIS-001` | Timeline longitudinal del paciente | Paciente | Prototipo cerrado | `pendiente de auditoría` | `23_uxui/UX-VALIDATION/UX-VALIDATION-VIS-001.md` |
| `VIS-002` | Dashboard multi-paciente del profesional | Profesional | Prototipo cerrado | `pendiente de auditoría` | `23_uxui/UX-VALIDATION/UX-VALIDATION-VIS-002.md` |
| `EXP-001` | Exportación CSV del paciente | Paciente | Prototipo cerrado | `pendiente de auditoría` | `23_uxui/UX-VALIDATION/UX-VALIDATION-EXP-001.md` |
| `TG-001` | Vinculación de cuenta Telegram | Paciente | Prototipo cerrado | `pendiente de auditoría` | `23_uxui/UX-VALIDATION/UX-VALIDATION-TG-001.md` |
| `TG-002` | Recordatorio y registro conversacional por Telegram | Paciente | Prototipo cerrado | `pendiente de auditoría` | `23_uxui/UX-VALIDATION/UX-VALIDATION-TG-002.md` |

## Cadena canónica por caso

La cadena base de un slice visible en Bitácora es:

`23_uxui/UXR/* -> 23_uxui/UXI/* -> 23_uxui/UJ/* -> 23_uxui/VOICE/* -> 23_uxui/UXS/* -> 23_uxui/PROTOTYPE/* -> 23_uxui/UX-VALIDATION/* -> 23_uxui/UI-RFC/* -> 23_uxui/HANDOFF-*`

## Estado por familia

| Familia | Estado | Artefacto índice |
| --- | --- | --- |
| `UXR` | cerrada para todos los slices del MVP visible | `UXR/UXR-INDEX.md` |
| `UXI` | cerrada para todos los slices del MVP visible | `UXI/UXI-INDEX.md` |
| `UJ` | cerrada para todos los slices del MVP visible | `UJ/UJ-INDEX.md` |
| `VOICE` | cerrada para todos los slices del MVP visible | `VOICE/VOICE-INDEX.md` |
| `UXS` | cerrada para todos los slices del MVP visible | `UXS/UXS-INDEX.md` |
| `PROTOTYPE` | iniciada para los `13` slices visibles del MVP | `PROTOTYPE/PROTOTYPE-INDEX.md` |
| `UX-VALIDATION` | iniciada con todos los slices preparados y en espera de evidencia real | `UX-VALIDATION/UX-VALIDATION-INDEX.md` |
| `UI-RFC` | iniciada bajo dispensa, con gate `strict Stitch only` | `UI-RFC/UI-RFC-INDEX.md` |
| `HANDOFF-*` | pendiente | — |

## Regla operativa

- cada familia futura debe referenciar el mismo slice visible y no puede contradecir su cadena `UXR -> UXI -> UJ -> VOICE -> UXS`;
- los `13` slices visibles del MVP ya quedaron adelantados a `Prototype`, pero ese estado no implica preparación automática para `UI-RFC`;
- la cobertura documental restante de profesional y Telegram se cerró bajo dispensa explícita, sin evidencia UX real;
- la capa global de diseño visual está cerrada por decisiones explícitas y solo se reabrirá a nivel slice bajo impacto material;
- el siguiente gate legítimo para todos los slices vuelve a ser `UX-VALIDATION` con evidencia real;
- ningún slice puede abrir `UI-RFC` mientras siga solo en `Prototype`, salvo dispensa explícita de entrada a UI documentada en `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`;
- esa dispensa no equivale a `UX-VALIDATION` y obliga validación posterior sobre código funcional.

## Entrada a UI bajo dispensa

La familia `UI-RFC` ya quedó abierta a nivel global, pero el gating por slice de esta etapa es `strict Stitch only`.

Eso implica:

- Stitch ya consume un pack `DESIGN.md` derivado y regenerable desde el wiki; eso ordena la entrada visual, pero no relaja el gate por slice;
- `ONB-001`, `REG-001` y `REG-002` ya alcanzaron cobertura Stitch derivada completa;
- la auditoría visual manual de esos tres slices ya detectó contradicciones con `VOICE`, `UXS` y `PROTOTYPE`;
- por eso los tres siguen sin abrir `UI-RFC-*` hasta nueva corrida Stitch corregida;
- el HTML local de `PROTOTYPE` mantiene valor documental y navegable;
- el HTML local no reemplaza la autoridad visual Stitch cuando el slice se evalúa para contrato técnico UI;
- el estado operativo detallado de la familia vive en `UI-RFC/UI-RFC-INDEX.md`.

---

**Estado:** índice raíz de la capa `23_uxui`, con familia `UI-RFC` ya iniciada bajo dispensa.
**Slices cubiertos:** `ONB-001`, `REG-001`, `REG-002`, `VIN-001`, `VIN-002`, `VIN-003`, `VIN-004`, `CON-002`, `VIS-001`, `VIS-002`, `EXP-001`, `TG-001`, `TG-002`.
**Siguiente capa gobernada:** `23_uxui/UI-RFC/*` cuando el gate Stitch por slice quede satisfecho, sin cancelar la deuda de `UX-VALIDATION`.
