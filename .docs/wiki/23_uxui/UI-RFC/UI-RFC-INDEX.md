# UI-RFC — Índice de contratos técnicos UI

## Propósito

Este índice abre la familia `UI-RFC` de Bitácora.

No reemplaza `PROTOTYPE-*`, no declara validación UX y no habilita implementación por sí mismo. Su función es gobernar cuándo puede abrirse un `UI-RFC-*` por slice, qué referencias debe citar y cómo se bloquea la familia cuando falta autoridad visual suficiente.

## Relación con el canon

Este índice depende de:

- `../../16_patrones_ui.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../21_matriz_validacion_ux.md`
- `../INDEX.md`
- `../PROTOTYPE/PROTOTYPE-INDEX.md`
- `../UX-VALIDATION/UX-VALIDATION-INDEX.md`
- `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`
- `.docs/raw/decisiones/03_waiver_entrada_ui_antes_validacion.md`

No debe contradecir:

- que la dispensa de entrada a UI no equivale a validación UX;
- que Stitch es la autoridad visual primaria de esta etapa;
- que el HTML local consolidado no reemplaza cobertura Stitch faltante cuando el gating del slice es estricto.

La capa global de diseño visual ya está cerrada por decisiones explícitas; por ahora, solo se reabren preguntas de slice cuando impactan materialmente:
- límites de componentes,
- jerarquía visual mayor,
- modelo de estados,
- expectativas de backend,
- comportamiento de accesibilidad,
- reutilización entre slices.

Ese cierre global no implica preparación de slice ni apertura automática de `UI-RFC-*`; solo evita reabrir la política editorial global sin una ambigüedad material real.

## Regla de entrada a UI bajo dispensa

La familia `UI-RFC` queda abierta bajo dispensa explícita.

Eso habilita:

- definir la gramática técnica global;
- evaluar slices para apertura de contrato técnico UI;
- bloquear explícitamente los slices que todavía no cumplen el gate visual requerido.

Eso no habilita:

- crear `UX-VALIDATION-*` sin evidencia real;
- llamar `validated` a un slice solo porque tenga `UI-RFC`;
- tratar envoltorios locales como autoridad visual independiente.

## Gate operativo vigente

Para esta etapa, Bitácora adopta `strict Stitch only` como gate de apertura de `UI-RFC-*` por slice.

El design system que Stitch debe consumir en esta etapa vive en `.docs/stitch/DESIGN*.md` como paquete derivado y regenerable desde el wiki. Ese paquete ordena la autoridad visual operativa, pero no reemplaza el gate por cobertura real de slice.

### Regla práctica

Un slice solo puede abrir `UI-RFC-*` si:

- la dispensa vigente lo cubre;
- `VOICE`, `UXS` y `PROTOTYPE` del slice están estables;
- todos los estados obligatorios del `PROTOTYPE-*` tienen cobertura Stitch suficiente;
- no hay contradicción entre Stitch, canon global y prototipo documental.

Si alguna de esas condiciones falla:

- el slice queda `bloqueado`;
- no se crea `UI-RFC-*` del slice;
- el bloqueo se deja explícito en este índice.

## Secciones obligatorias de un `UI-RFC-*`

Todo contrato técnico UI por slice deberá incluir como mínimo:

1. propósito del slice y alcance visible;
2. referencias obligatorias (`VOICE`, `UXS`, `PROTOTYPE`, `RF`, `TP`, `07`, `09`);
3. inventario de estados obligatorios;
4. referencias Stitch exactas por estado;
5. regiones de composición y jerarquía visual;
6. gramática de componentes y composición;
7. comportamiento de estados, retroalimentación y movimiento;
8. responsive y accesibilidad;
9. expectativas de contrato backend y manejo de errores;
10. dependencias abiertas o bloqueos.

## Orden congelado de slices

| Orden | Slice | Estado UI-RFC actual | Gate Stitch | Bloqueo por gate | Motivo / observación |
| --- | --- | --- | --- | --- | --- |
| 1 | `ONB-001` | `auditado con hallazgos` | cobertura completa, auditoría visual fallida | sí | contradicción con `VOICE/UXS`: consentimiento pesado y bilingüe; continuidad final demasiado lírica |
| 2 | `REG-001` | `auditado con hallazgos` | cobertura completa, auditoría visual fallida | sí | contradicción con `VOICE/UXS`: sesión expirada sobredimensionada y confirmación con exceso de señales |
| 3 | `REG-002` | `auditado con hallazgos` | cobertura completa, auditoría visual fallida | sí | contradicción con `VOICE/UXS`: entrada bilingüe y confirmación demasiado ceremonial |
| 4 | `VIN-002` | `pendiente de auditoría` | no auditado en esta ola | pendiente | se evalúa después de destrabar la ola núcleo |
| 5 | `VIN-004` | `pendiente de auditoría` | no auditado en esta ola | pendiente | se evalúa después de destrabar la ola núcleo |
| 6 | `VIN-003` | `pendiente de auditoría` | no auditado en esta ola | pendiente | se evalúa después de destrabar la ola núcleo |
| 7 | `CON-002` | `pendiente de auditoría` | no auditado en esta ola | pendiente | se evalúa después de destrabar la ola núcleo |
| 8 | `VIS-001` | `pendiente de auditoría` | no auditado en esta ola | pendiente | se evalúa después de destrabar la ola núcleo |
| 9 | `EXP-001` | `pendiente de auditoría` | no auditado en esta ola | pendiente | se evalúa después de destrabar la ola núcleo |
| 10 | `VIN-001` | `pendiente de auditoría` | no auditado en esta ola | pendiente | se evalúa después de destrabar la ola núcleo |
| 11 | `VIS-002` | `pendiente de auditoría` | no auditado en esta ola | pendiente | se evalúa después de destrabar la ola núcleo |
| 12 | `TG-001` | `pendiente de auditoría` | no auditado en esta ola | pendiente | se evalúa después de destrabar la ola núcleo |
| 13 | `TG-002` | `pendiente de auditoría` | no auditado en esta ola | pendiente | se evalúa después de destrabar la ola núcleo |

## Estado operativo explícito de la ola núcleo

### `ONB-001`

- el slice ya quedó cubierto con artifacts Stitch bajo `DESIGN.patient.md`;
- `STITCH-ARTIFACTS-AUDIT.md` ya marca `12/12` y `full-candidate`;
- la auditoría visual manual ya detectó contradicciones con `VOICE-ONB-001`, `UXS-ONB-001` y `PROTOTYPE-ONB-001`;
- antes de abrir `UI-RFC-*` el slice necesita una nueva corrida Stitch corregida.

### `REG-001`

- el slice ya quedó cubierto con artifacts Stitch bajo `DESIGN.patient.md`;
- `STITCH-ARTIFACTS-AUDIT.md` ya marca `7/7` y `full-candidate`;
- la auditoría visual manual ya detectó contradicciones con `VOICE-REG-001`, `UXS-REG-001` y `PROTOTYPE-REG-001`;
- antes de abrir `UI-RFC-*` el slice necesita una nueva corrida Stitch corregida.

### `REG-002`

- el slice ya quedó cubierto con artifacts Stitch bajo `DESIGN.patient.md`;
- `STITCH-ARTIFACTS-AUDIT.md` ya marca `7/7` y `full-candidate`;
- la auditoría visual manual ya detectó contradicciones con `VOICE-REG-002`, `UXS-REG-002` y `PROTOTYPE-REG-002`;
- antes de abrir `UI-RFC-*` el slice necesita una nueva corrida Stitch corregida.

## Regla de trazabilidad

Cada futuro `UI-RFC-*` deberá enlazar:

- `VOICE-*` del slice;
- `UXS-*` del slice;
- `PROTOTYPE-*` del slice;
- `04_RF` y `06_pruebas` relevantes;
- `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`;
- `09_contratos_tecnicos.md` y contratos hijos cuando apliquen.

## Estado de la familia

La familia `UI-RFC` queda:

- `iniciada` a nivel global;
- `sin contratos de slice abiertos` en esta sesión;
- `sin contratos abiertos todavía`; la ola núcleo ya cumple cobertura Stitch derivada completa en `ONB-001`, `REG-001` y `REG-002`, pero la auditoría visual manual ya devolvió hallazgos y mantiene bloqueada la apertura de `UI-RFC-*` hasta nueva corrida Stitch corregida.

---

**Estado:** índice inicial de `UI-RFC` creado bajo dispensa explícita.
**Siguiente capa gobernada:** futuros `UI-RFC-*` por slice cuando el gate Stitch quede satisfecho.
