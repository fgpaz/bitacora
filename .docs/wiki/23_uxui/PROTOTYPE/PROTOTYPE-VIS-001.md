# PROTOTYPE-VIS-001 — Prototipo de timeline longitudinal del paciente

## Propósito

Este documento define el prototipo del slice `VIS-001`.

No declara que la validación ya ocurrió ni reemplaza `UXS-VIS-001.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-VIS-001.md`
- `../UXI/UXI-VIS-001.md`
- `../UJ/UJ-VIS-001.md`
- `../VOICE/VOICE-VIS-001.md`
- `../UXS/UXS-VIS-001.md`
- `../../03_FL/FL-VIS-01.md`
- `../../06_pruebas/TP-VIS.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-VIS-001.md`
- flujo enlazado `./PROTOTYPE-EXP-001.md`

## Slice cubierto

### Caso

`VIS-001`: timeline longitudinal del paciente.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` apertura del timeline con un período base;
- `S02` lectura inicial y ajuste de período;
- `S03` estado vacío o error recuperable;
- salida secundaria hacia `EXP-001` como flujo separado.

### Foco principal de observación

- legibilidad del gráfico antes que los filtros;
- calma editorial del chart suavizado;
- claridad del período actual;
- exportación visible como camino secundario, no dominante.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy casi final;
- un chart longitudinal suave como pieza principal;
- filtros simples y contenidos;
- estados sensibles visibles;
- mobile y desktop con la misma lógica de lectura.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| documento fuente | `./PROTOTYPE-VIS-001.md` | dueño del alcance del slice |
| referencia visual Stitch `loading` | `../../../../artifacts/stitch/vis-001/2026-04-08T23-49-24-752Z/s01-loading.html` | referencia visual principal del estado `loading` |
| referencia visual Stitch `ready` | `../../../../artifacts/stitch/vis-001/2026-04-08T23-48-12-302Z/s01-ready.html` | referencia visual principal del estado `ready` |
| referencia visual Stitch `period` | `../../../../artifacts/stitch/vis-001/2026-04-08T23-50-25-259Z/s01-period-change.html` | referencia visual principal del ajuste de período |
| referencia visual Stitch `empty` | `../../../../artifacts/stitch/vis-001/2026-04-08T23-51-39-073Z/s02-empty.html` | referencia visual principal del estado `empty` |
| referencia visual Stitch `error` | `../../../../artifacts/stitch/vis-001/2026-04-08T23-52-43-358Z/s03-error.html` | referencia visual principal del estado `error` |
| fallback local consolidado | `./PROTOTYPE-VIS-001.html` | respaldo navegable del slice completo si hace falta revisar el pack en una sola pieza |

### Referencia canónica actual

- [Abrir Stitch `loading`](../../../../artifacts/stitch/vis-001/2026-04-08T23-49-24-752Z/s01-loading.html)
- [Abrir Stitch `ready`](../../../../artifacts/stitch/vis-001/2026-04-08T23-48-12-302Z/s01-ready.html)
- [Abrir Stitch `period`](../../../../artifacts/stitch/vis-001/2026-04-08T23-50-25-259Z/s01-period-change.html)
- [Abrir Stitch `empty`](../../../../artifacts/stitch/vis-001/2026-04-08T23-51-39-073Z/s02-empty.html)
- [Abrir Stitch `error`](../../../../artifacts/stitch/vis-001/2026-04-08T23-52-43-358Z/s03-error.html)
- [Abrir flujo enlazado de exportación](./PROTOTYPE-EXP-001.html)
- [Abrir fallback local consolidado](./PROTOTYPE-VIS-001.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `VIS-001-S01-LOADING` | `S01` | skeleton o placeholder editorial del timeline | sí |
| `VIS-001-S01-READY` | `S01` | timeline base con período actual visible | sí |
| `VIS-001-S02-PERIOD` | `S02` | ajuste de período con el chart aún dominante | sí |
| `VIS-001-S03-EMPTY` | `S03` | vacío útil y distinguible de error | sí |
| `VIS-001-S03-ERROR` | `S03` | error breve de carga con reintento | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona abre su timeline;
- entiende rápido qué período está viendo;
- lee el chart base sin sentirse en un dashboard analítico;
- ajusta el período si hace falta;
- reconoce la exportación como acción secundaria separada.

### Estados sensibles obligatorios

- loading calmo;
- período ajustado;
- estado vacío útil;
- error recuperable.

## Hipótesis que este prototipo debe permitir observar

1. El chart suavizado se siente como lectura acompañada, no como panel técnico.
2. Los filtros de período no compiten con la lectura inicial.
3. La persona distingue vacío de error sin explicación extra.
4. La exportación aparece disponible sin robarle protagonismo al timeline.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-VIS-001`;
- mantener estructura y estados alineados con `UXS-VIS-001.md`;
- no usar jerga analítica ni framing clínico;
- no enterrar el chart detrás de controles;
- no convertir `EXP-001` en CTA principal del slice;
- no confundir ausencia de datos con falla del sistema.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. el chart principal aparece antes que controles accesorios;
3. el ajuste de período mantiene una sola dirección dominante;
4. el estado vacío se distingue con claridad del error;
5. el enlace a `EXP-001` existe, pero se comporta como salida secundaria.

## Supuestos abiertos antes de validación

- la forma exacta del chart puede variar si el equipo técnico luego necesita otra librería, mientras conserve la misma gramática de lectura;
- la paginación o recorte para períodos largos no se resuelve todavía a nivel técnico y solo se sugiere como variante futura;
- la corrida Stitch actual ya cubre el pack visual principal del slice y el HTML local queda solo como respaldo consolidado;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-VIS-001.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-VIS-001.md`.

---

**Estado:** prototipo enlazado, navegable y testeable para `VIS-001`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-VIS-001.md`.
