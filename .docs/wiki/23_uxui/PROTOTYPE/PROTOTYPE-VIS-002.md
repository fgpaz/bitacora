# PROTOTYPE-VIS-002 — Prototipo de dashboard multi-paciente

## Propósito

Este documento define el prototipo del slice `VIS-002`.

No declara que la validación ya ocurrió ni reemplaza `UXS-VIS-002.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-VIS-002.md`
- `../UXI/UXI-VIS-002.md`
- `../UJ/UJ-VIS-002.md`
- `../VOICE/VOICE-VIS-002.md`
- `../UXS/UXS-VIS-002.md`
- `../../03_FL/FL-VIS-02.md`
- `../../06_pruebas/TP-VIS.md`
- `../../06_pruebas/TP-SEC.md`
- `./PROTOTYPE-VIN-001.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-VIS-002.md`

## Slice cubierto

### Caso

`VIS-002`: dashboard multi-paciente del profesional.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` carga inicial del tablero;
- `S02` lectura principal y cambio de página;
- `S03` estado vacío o error recuperable.

### Foco principal de observación

- tablero sobrio y acotado;
- límite de acceso visible;
- lectura priorizable aun con varias tarjetas;
- paginación contenida.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy casi final;
- desktop-first;
- lista paginada de pacientes visibles;
- alertas básicas no dramáticas;
- estados sensibles visibles.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| documento fuente | `./PROTOTYPE-VIS-002.md` | dueño del alcance del slice |
| referencia visual Stitch `loading` | `../../../../artifacts/stitch/vis-002/2026-04-09T00-41-03-976Z/s01-loading.html` | referencia visual principal del estado `loading` |
| referencia visual Stitch `pagination` | `../../../../artifacts/stitch/vis-002/2026-04-09T00-41-03-771Z/s01-pagination.html` | referencia visual principal del cambio de página |
| referencia visual Stitch `empty` | `../../../../artifacts/stitch/vis-002/2026-04-09T00-23-00-523Z/s02-empty.html` | referencia visual principal del estado `empty` |
| referencia visual Stitch `error` | `../../../../artifacts/stitch/vis-002/2026-04-09T00-23-00-523Z/s03-error.html` | referencia visual principal del estado `error` |
| fallback local consolidado | `./PROTOTYPE-VIS-002.html` | wrapper navegable del slice completo, con frames Stitch como fuente primaria y cobertura local solo para el estado `ready` |

### Referencia canónica actual

- [Abrir Stitch `loading`](../../../../artifacts/stitch/vis-002/2026-04-09T00-41-03-976Z/s01-loading.html)
- [Abrir Stitch `pagination`](../../../../artifacts/stitch/vis-002/2026-04-09T00-41-03-771Z/s01-pagination.html)
- [Abrir Stitch `empty`](../../../../artifacts/stitch/vis-002/2026-04-09T00-23-00-523Z/s02-empty.html)
- [Abrir Stitch `error`](../../../../artifacts/stitch/vis-002/2026-04-09T00-23-00-523Z/s03-error.html)
- [Abrir fallback local consolidado](./PROTOTYPE-VIS-002.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `VIS-002-S01-LOADING` | `S01` | skeleton o placeholder sobrio | sí |
| `VIS-002-S01-READY` | `S01` | lista de pacientes visible y priorizable | sí |
| `VIS-002-S01-PAGINATION` | `S01` | cambio de página sin ruido | sí |
| `VIS-002-S02-EMPTY` | `S02` | ausencia de pacientes visibles | sí |
| `VIS-002-S03-ERROR` | `S03` | error recuperable | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- el profesional abre el dashboard;
- entiende por qué esas personas aparecen ahí;
- prioriza una lectura;
- cambia de página;
- distingue vacío de error.

### Estados sensibles obligatorios

- loading;
- ready;
- paginación;
- empty;
- error.

## Hipótesis que este prototipo debe permitir observar

1. El tablero se percibe como sobrio y acotado.
2. Queda explícito que solo aparecen pacientes con acceso activo.
3. La paginación no rompe la jerarquía de lectura.
4. El vacío no se confunde con error.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-VIS-002`;
- mantener estructura y estados alineados con `UXS-VIS-002.md`;
- no usar estética de EHR ni de pared de alertas;
- no dramatizar alertas básicas;
- no perder el límite de acceso visible.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. Stitch cubre `loading`, `pagination`, `empty` y `error`;
3. el fallback local cubre el estado `ready` sin contradicción de jerarquía;
4. el dashboard se mantiene legible aun con varias tarjetas;
5. el límite de visibilidad se lee sin explicación oral.

## Supuestos abiertos antes de validación

- Stitch todavía no devolvió una salida usable del estado `ready` luego de varios reintentos por pantalla, así que el HTML local sostiene solo ese frame dentro de un wrapper basado en Stitch para el resto del slice;
- la densidad final de alertas puede ajustarse luego según validación;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-VIS-002.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-VIS-002.md`.

---

**Estado:** prototipo enlazado, navegable y testeable para `VIS-002`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-VIS-002.md`.
