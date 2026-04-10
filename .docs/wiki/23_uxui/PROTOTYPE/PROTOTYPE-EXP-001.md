# PROTOTYPE-EXP-001 — Prototipo de exportación CSV del paciente

## Propósito

Este documento define el prototipo del slice `EXP-001`.

No declara que la validación ya ocurrió ni reemplaza `UXS-EXP-001.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-EXP-001.md`
- `../UXI/UXI-EXP-001.md`
- `../UJ/UJ-EXP-001.md`
- `../VOICE/VOICE-EXP-001.md`
- `../UXS/UXS-EXP-001.md`
- `../../03_FL/FL-EXP-01.md`
- `../../06_pruebas/TP-EXP.md`
- `./PROTOTYPE-VIS-001.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-EXP-001.md`

## Slice cubierto

### Caso

`EXP-001`: exportación CSV del paciente.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` entrada desde el timeline o acceso directo a exportación;
- `S02` confirmación de alcance y período;
- `S03` handoff de descarga o error recuperable.

### Foco principal de observación

- claridad sobre qué incluye el archivo;
- percepción de control simple, no consola técnica;
- feedback suficiente mientras se prepara la descarga;
- continuidad editorial con `VIS-001`.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy casi final;
- un solo CTA principal;
- selector de período simple;
- estados de preparación y error visibles;
- lenguaje de derecho de acceso en tono humano.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| documento fuente | `./PROTOTYPE-EXP-001.md` | dueño del alcance del slice |
| referencia visual Stitch `period` | `../../../../artifacts/stitch/exp-001/2026-04-08T23-55-19-433Z/s01-period.html` | referencia visual principal del estado con período seleccionado |
| referencia visual Stitch `generating` | `../../../../artifacts/stitch/exp-001/2026-04-08T23-56-20-483Z/s02-generating.html` | referencia visual principal del estado de preparación |
| referencia visual Stitch `error` | `../../../../artifacts/stitch/exp-001/2026-04-08T23-58-35-022Z/s03-error.html` | referencia visual principal del error recuperable |
| fallback local consolidado | `./PROTOTYPE-EXP-001.html` | respaldo del slice completo y cobertura de los estados que Stitch todavía no devolvió |

### Referencia canónica actual

- [Abrir Stitch `period`](../../../../artifacts/stitch/exp-001/2026-04-08T23-55-19-433Z/s01-period.html)
- [Abrir Stitch `generating`](../../../../artifacts/stitch/exp-001/2026-04-08T23-56-20-483Z/s02-generating.html)
- [Abrir Stitch `error`](../../../../artifacts/stitch/exp-001/2026-04-08T23-58-35-022Z/s03-error.html)
- [Volver al timeline prototipado](./PROTOTYPE-VIS-001.html)
- [Abrir fallback local consolidado](./PROTOTYPE-EXP-001.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `EXP-001-S01-DEFAULT` | `S01` | alcance visible y CTA disponible | sí |
| `EXP-001-S01-PERIOD` | `S01` | período seleccionado y alcance actualizado | sí |
| `EXP-001-S02-GENERATING` | `S02` | preparación breve del archivo | sí |
| `EXP-001-S02-SUCCESS` | `S03` | descarga iniciada o lista para continuar | sí |
| `EXP-001-S03-ERROR` | `S03` | error recuperable con reintento | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona entra desde el timeline;
- revisa qué datos incluye el CSV;
- confirma o ajusta el período;
- dispara la descarga;
- recibe feedback breve de preparación y resultado.

### Estados sensibles obligatorios

- período elegido;
- generación en curso;
- handoff de éxito;
- error recuperable.

## Hipótesis que este prototipo debe permitir observar

1. El alcance del archivo se entiende antes de descargar.
2. La exportación se percibe como derecho simple de acceso, no como operación técnica.
3. El feedback de preparación alcanza para no dejar a la persona esperando sin contexto.
4. El error mantiene calma y no introduce jerga de backend o compliance.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-EXP-001`;
- mantener estructura y estados alineados con `UXS-EXP-001.md`;
- no explicar cifrado, payloads ni detalle técnico de stream;
- no convertir la exportación en consola o tabla administrativa;
- no competir con una segunda acción principal;
- no romper continuidad visual con `VIS-001`.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. el alcance del archivo se entiende antes de disparar la descarga;
3. el selector de período no agrega fricción innecesaria;
4. el estado `generating` sostiene certeza tranquila;
5. el prototipo se percibe como derivación natural del timeline.

## Supuestos abiertos antes de validación

- la diferencia exacta entre “descarga iniciada” y “archivo listo” puede ajustarse luego según el comportamiento real del navegador;
- el caso “sin registros” puede resolverse como CSV con headers o copy explícito, pero el prototipo no fija aún la política técnica;
- Stitch ya cubre tres estados clave del slice y el HTML local queda como respaldo para `default` y `success_handoff`, que todavía no devolvieron salida usable por timeout del servicio;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-EXP-001.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-EXP-001.md`.

---

**Estado:** prototipo enlazado, navegable y testeable para `EXP-001`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-EXP-001.md`.
