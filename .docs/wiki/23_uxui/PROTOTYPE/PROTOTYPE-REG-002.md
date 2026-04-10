# PROTOTYPE-REG-002 — Prototipo del registro de factores diarios vía web

## Propósito

Este documento define el prototipo del slice `REG-002`.

No declara que la validación ya ocurrió ni reemplaza `UXS-REG-002.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-REG-002.md`
- `../UXI/UXI-REG-002.md`
- `../UJ/UJ-REG-002.md`
- `../VOICE/VOICE-REG-002.md`
- `../UXS/UXS-REG-002.md`
- `../../03_FL/FL-REG-03.md`
- `../../06_pruebas/TP-REG.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-REG-002.md`

## Slice cubierto

### Caso

`REG-002`: registro de factores diarios vía web.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` entrada al check-in;
- `S02` recorrido por bloques y aparición de medicación;
- `S03` revisión final, envío y confirmación.

### Foco principal de observación

- escaneo inicial del formulario;
- percepción de carga cognitiva;
- claridad del bloque condicional de medicación;
- cierre del check-in sin sensación de encuesta larga.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy casi final;
- bloques visibles y agrupados;
- estados clave de error y guardado;
- mobile y desktop consistentes;
- una sola dirección dominante antes del guardado.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| documento fuente | `./PROTOTYPE-REG-002.md` | dueño del alcance del slice |
| referencia navegable mínima | `./PROTOTYPE-REG-002.html` | fuente canónica local hasta que exista referencia externa equivalente |

### Referencia canónica actual

- [Abrir prototipo local](./PROTOTYPE-REG-002.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `REG-002-S01-ENTRY` | `S01` | entrada al check-in con framing ligero | sí |
| `REG-002-S02-PARTIAL` | `S02` | formulario con algunos bloques ya visibles | sí |
| `REG-002-S02-MEDICATION` | `S02` | aparición del bloque condicional de medicación | sí |
| `REG-002-S03-READY` | `S03` | formulario listo para guardar | sí |
| `REG-002-S03-SUBMITTING` | `S03` | feedback breve de guardado | sí |
| `REG-002-S03-ERROR` | `S03` | error recuperable cerca del área final | sí |
| `REG-002-S03-CONFIRM` | `S03` | confirmación factual del guardado | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona entra al check-in;
- reconoce rápido los bloques;
- ve aparecer medicación solo cuando corresponde;
- llega al guardado sin sentir encuesta extensa;
- recibe una confirmación breve.

### Estados sensibles obligatorios

- aparición del bloque de medicación;
- guardado listo para enviar;
- error recuperable en el cierre;
- confirmación posterior al guardado.

## Hipótesis que este prototipo debe permitir observar

1. El formulario no se siente como una pared de campos.
2. Los bloques agrupados sostienen una sola dirección dominante.
3. La medicación aparece con claridad y sin ruido.
4. El guardado final no introduce fricción innecesaria.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-REG-002`;
- mantener estructura y estados alineados con `UXS-REG-002.md`;
- no mostrar todos los campos a la vez si no agregan valor;
- no moralizar ni usar tono clínico pesado;
- no depender de explicación oral para entender el bloque condicional.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. el formulario puede recorrerse como check-in breve y no como encuesta larga;
3. el bloque de medicación queda claro cuando aparece;
4. el área final de guardado mantiene una sola acción principal.

## Supuestos abiertos antes de validación

- la cantidad exacta de microcopys de ayuda puede ajustarse si aparece sobrecarga verbal;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-REG-002.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-REG-002.md`.

---

**Estado:** prototipo enlazado, navegable y testeable para `REG-002`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-REG-002.md`.
