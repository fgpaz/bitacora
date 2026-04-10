# PROTOTYPE-VIN-001 — Prototipo de invitación profesional

## Propósito

Este documento define el prototipo del slice `VIN-001`.

No declara que la validación ya ocurrió ni reemplaza `UXS-VIN-001.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-VIN-001.md`
- `../UXI/UXI-VIN-001.md`
- `../UJ/UJ-VIN-001.md`
- `../VOICE/VOICE-VIN-001.md`
- `../UXS/UXS-VIN-001.md`
- `../../03_FL/FL-VIN-01.md`
- `../../06_pruebas/TP-VIN.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-VIN-001.md`
- flujo enlazado `./PROTOTYPE-VIS-002.md`

## Slice cubierto

### Caso

`VIN-001`: emisión de invitación profesional a paciente.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` entrada al flujo y carga del email;
- `S02` preparación, envío y estado pendiente;
- `S03` conflicto o caso existente;
- salida encadenada hacia `VIS-002` como siguiente superficie profesional.

### Foco principal de observación

- claridad de vínculo pendiente;
- explicitud de control del paciente;
- brevedad responsable de la acción;
- claridad del conflicto sin duplicar acciones.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy casi final;
- desktop-first con lectura sobria;
- un solo campo principal;
- estados sensibles visibles;
- resultado legible sin explicación oral.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| documento fuente | `./PROTOTYPE-VIN-001.md` | dueño del alcance del slice |
| referencia visual Stitch `default` | `../../../../artifacts/stitch/vin-001/2026-04-09T00-20-06-865Z/s01-default.html` | referencia visual principal del estado `default` |
| referencia visual Stitch `ready` | `../../../../artifacts/stitch/vin-001/2026-04-09T00-33-23-352Z/s01-ready.html` | referencia visual principal del estado `ready_to_submit` |
| referencia visual Stitch `submitting` | `../../../../artifacts/stitch/vin-001/2026-04-09T00-33-23-194Z/s02-submitting.html` | referencia visual principal del estado `submitting` |
| referencia visual Stitch `success` | `../../../../artifacts/stitch/vin-001/2026-04-09T00-20-06-865Z/s02-success.html` | referencia visual principal del estado `success` |
| referencia visual Stitch `conflict` | `../../../../artifacts/stitch/vin-001/2026-04-09T00-20-06-865Z/s03-conflict.html` | referencia visual principal del estado `error_or_conflict` |
| fallback local consolidado | `./PROTOTYPE-VIN-001.html` | wrapper navegable del slice completo basado en los frames Stitch descargados |

### Referencia canónica actual

- [Abrir Stitch `default`](../../../../artifacts/stitch/vin-001/2026-04-09T00-20-06-865Z/s01-default.html)
- [Abrir Stitch `ready`](../../../../artifacts/stitch/vin-001/2026-04-09T00-33-23-352Z/s01-ready.html)
- [Abrir Stitch `submitting`](../../../../artifacts/stitch/vin-001/2026-04-09T00-33-23-194Z/s02-submitting.html)
- [Abrir Stitch `success`](../../../../artifacts/stitch/vin-001/2026-04-09T00-20-06-865Z/s02-success.html)
- [Abrir Stitch `conflict`](../../../../artifacts/stitch/vin-001/2026-04-09T00-20-06-865Z/s03-conflict.html)
- [Abrir flujo enlazado de dashboard profesional](./PROTOTYPE-VIS-002.html)
- [Abrir fallback local consolidado](./PROTOTYPE-VIN-001.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `VIN-001-S01-DEFAULT` | `S01` | entrada con campo y alcance visible | sí |
| `VIN-001-S01-READY` | `S01` | email listo y CTA habilitada | sí |
| `VIN-001-S02-SUBMITTING` | `S02` | envío breve y sin doble acción | sí |
| `VIN-001-S02-SUCCESS` | `S02` | invitación pendiente emitida | sí |
| `VIN-001-S03-CONFLICT` | `S03` | invitación o vínculo existente | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- el profesional llega con intención de invitar;
- entiende que crea un vínculo pendiente;
- carga el email;
- emite la invitación;
- lee el resultado o un conflicto contextualizado.

### Estados sensibles obligatorios

- entrada;
- listo para enviar;
- envío en curso;
- éxito;
- conflicto.

## Hipótesis que este prototipo debe permitir observar

1. La invitación no se interpreta como alta clínica definitiva.
2. El control del paciente sobre acceso clínico queda claro.
3. La interfaz se percibe como breve y responsable, no burocrática.
4. El conflicto evita acciones repetidas y reduce incertidumbre.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-VIN-001`;
- mantener estructura y estados alineados con `UXS-VIN-001.md`;
- no usar jerga de onboarding administrativo ni de permisos técnicos;
- no insinuar acceso clínico automático;
- no agregar una segunda acción principal.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. Stitch cubre todos los estados principales del slice;
3. el vínculo pendiente se entiende antes del envío;
4. el conflicto evita repetir acciones innecesarias;
5. el enlace a `VIS-002` existe como continuidad profesional.

## Supuestos abiertos antes de validación

- el detalle exacto del mensaje de conflicto puede ajustarse luego según la casuística real;
- el HTML local existe como wrapper navegable de los frames Stitch y no como reinterpretación paralela del slice;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-VIN-001.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-VIN-001.md`.

---

**Estado:** prototipo enlazado, navegable y testeable para `VIN-001`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-VIN-001.md`.
