# PROTOTYPE-VIN-004 — Prototipo de gestión de acceso profesional por paciente

## Propósito

Este documento define el prototipo del slice `VIN-004`.

No declara que la validación ya ocurrió ni reemplaza `UXS-VIN-004.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-VIN-004.md`
- `../UXI/UXI-VIN-004.md`
- `../UJ/UJ-VIN-004.md`
- `../VOICE/VOICE-VIN-004.md`
- `../UXS/UXS-VIN-004.md`
- `../../03_FL/FL-VIN-04.md`
- `../../06_pruebas/TP-VIN.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-VIN-004.md`

## Slice cubierto

### Caso

`VIN-004`: gestión de acceso profesional por paciente.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` lectura del estado actual;
- `S02` decisión y guardado del cambio;
- `S03` confirmación del nuevo estado.

### Foco principal de observación

- legibilidad del estado actual antes de tocar nada;
- claridad del efecto del cambio antes del guardado;
- reversibilidad percibida sin dramatizar;
- dignidad del error localizado.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy casi final;
- estados sensibles visibles;
- una sola acción principal por paso;
- mobile y desktop con la misma lógica;
- confirmación factual, no celebratoria.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| documento fuente | `./PROTOTYPE-VIN-004.md` | dueño del alcance del slice |
| referencia navegable mínima | `./PROTOTYPE-VIN-004.html` | consolida el flujo completo y los estados del slice |
| soporte visual Stitch `default` | `../../../../artifacts/stitch/vin-004/2026-04-08T22-47-11-393Z/s01-default.html` | referencia visual real de la corrida `core` |
| soporte visual Stitch `saved` | `../../../../artifacts/stitch/vin-004/2026-04-08T22-47-11-393Z/s02-saved.html` | referencia visual real de la corrida `core` |
| soporte visual Stitch `error` | `../../../../artifacts/stitch/vin-004/2026-04-08T22-47-11-393Z/s03-error.html` | referencia visual real de la corrida `core` |

### Referencia canónica actual

- [Abrir prototipo local](./PROTOTYPE-VIN-004.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `VIN-004-S01-DEFAULT` | `S01` | estado actual legible antes del cambio | sí |
| `VIN-004-S01-INACTIVE` | `S01` | vínculo inactivo que no admite cambios | sí |
| `VIN-004-S02-DIRTY` | `S02` | nuevo estado seleccionado y efecto explícito | sí |
| `VIN-004-S02-SAVING` | `S02` | feedback corto durante guardado | sí |
| `VIN-004-S03-SAVED` | `S03` | nuevo estado guardado con efecto visible | sí |
| `VIN-004-S03-ERROR` | `S03` | error recuperable con reintento claro | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona ve el estado actual del acceso;
- entiende qué cambiaría si modifica ese estado;
- guarda la decisión;
- confirma el nuevo límite sin confundir acceso con vínculo.

### Estados sensibles obligatorios

- vínculo inactivo;
- guardado en curso;
- error recuperable;
- confirmación posterior al cambio.

## Hipótesis que este prototipo debe permitir observar

1. El estado actual se entiende antes de interactuar.
2. La consecuencia del cambio se lee antes del guardado y no después.
3. Activar o restringir acceso se percibe como ajuste reversible, no como revocación total del vínculo.
4. El error mantiene visible el estado previo y no tecnifica el problema.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-VIN-004`;
- mantener estructura y estados alineados con `UXS-VIN-004.md`;
- no usar lenguaje interno como `can_view_data`;
- no igualar acceso con vínculo;
- no convertir el control en un switch opaco sin explicación;
- no introducir una segunda acción primaria.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. el flujo `S01 -> S03` puede recorrerse sin explicación compensatoria;
3. el efecto del cambio se entiende antes de guardar;
4. los estados sensibles principales están presentes;
5. mobile y desktop conservan la misma lógica de control explícito.

## Supuestos abiertos antes de validación

- el patrón final puede confirmarse como guardado explícito o auto-save solo si no altera comprensión del efecto;
- el nombre visible del profesional puede cambiar según contexto real sin alterar la estructura del slice;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-VIN-004.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-VIN-004.md`;
- la corrida Stitch actual ya sirve como base visual, pero el complemento local sigue siendo la referencia consolidada del slice.

---

**Estado:** prototipo enlazado, navegable y testeable para `VIN-004`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-VIN-004.md`.
