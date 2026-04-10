# PROTOTYPE-VIN-002 — Prototipo de auto-vinculación paciente a profesional por código

## Propósito

Este documento define el prototipo del slice `VIN-002`.

No declara que la validación ya ocurrió ni reemplaza `UXS-VIN-002.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-VIN-002.md`
- `../UXI/UXI-VIN-002.md`
- `../UJ/UJ-VIN-002.md`
- `../VOICE/VOICE-VIN-002.md`
- `../UXS/UXS-VIN-002.md`
- `../../03_FL/FL-VIN-02.md`
- `../../06_pruebas/TP-VIN.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-VIN-002.md`

## Slice cubierto

### Caso

`VIN-002`: auto-vinculación paciente a profesional por código.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` entrada al flujo de vínculo;
- `S02` ingreso y validación del código;
- `S03` confirmación del vínculo con acceso todavía bajo control del paciente.

### Foco principal de observación

- claridad del contexto antes del código;
- comprensión de que vínculo no equivale a acceso;
- precisión percibida del paso principal;
- dignidad de errores y variantes sensibles.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy casi final;
- estados sensibles visibles;
- una sola acción primaria dominante;
- mobile y desktop con la misma lógica;
- confirmación factual, no celebratoria.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| documento fuente | `./PROTOTYPE-VIN-002.md` | dueño del alcance del slice |
| referencia navegable mínima | `./PROTOTYPE-VIN-002.html` | fuente canónica local mientras Stitch no deje un resultado usable |
| intento Stitch | `../../../raw/plans/2026-04-08-stitch-first-wave-paciente.md` | el fallback local queda habilitado si la corrida no produce salida estable |

### Referencia canónica actual

- [Abrir prototipo local](./PROTOTYPE-VIN-002.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `VIN-002-S01-ENTRY` | `S01` | entrada con contexto breve y promesa explícita de control | sí |
| `VIN-002-S02-DEFAULT` | `S02` | campo vacío con efecto del vínculo visible | sí |
| `VIN-002-S02-READY` | `S02` | código listo para enviar | sí |
| `VIN-002-S02-SUBMITTING` | `S02` | feedback breve durante validación | sí |
| `VIN-002-S02-INVALID` | `S02` | código inválido con recuperación digna | sí |
| `VIN-002-S02-EXPIRED` | `S02` | código expirado con salida clara | sí |
| `VIN-002-S03-SUCCESS` | `S03` | vínculo activo con acceso aún desactivado | sí |
| `VIN-002-S03-EXISTING` | `S03` | vínculo ya existente sin duplicación confusa | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona entra al flujo;
- entiende en una lectura breve qué va a pasar;
- ingresa o pega el código;
- valida el vínculo;
- sale entendiendo que el acceso a los datos sigue bajo su control.

### Estados sensibles obligatorios

- código inválido;
- código expirado;
- vínculo ya existente;
- confirmación posterior al vínculo con `can_view_data=false`.

## Hipótesis que este prototipo debe permitir observar

1. El contexto previo al código alcanza sin volverse introductorio de más.
2. El paso principal se siente corto y preciso.
3. La persona entiende que el vínculo no activa acceso automático a sus datos.
4. Los errores sostienen claridad y no convierten el slice en soporte técnico.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-VIN-002`;
- mantener estructura y estados alineados con `UXS-VIN-002.md`;
- no pedir información extra además del código;
- no usar tecnicismos sobre binding, token o habilitación;
- no confirmar el vínculo sin explicar el límite de acceso;
- no agregar más de una acción primaria por paso.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. el flujo principal `S01 -> S03` puede recorrerse sin explicación compensatoria;
3. el efecto del vínculo y el límite de acceso son visibles antes y después del envío;
4. los estados sensibles principales están presentes;
5. mobile y desktop conservan la misma lógica de precisión y control.

## Supuestos abiertos antes de validación

- el nombre visible del profesional puede cambiar según contexto real sin alterar la estructura del slice;
- el formato exacto del código puede ajustarse si el contrato técnico cambia, mientras siga siendo un único campo;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-VIN-002.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-VIN-002.md`;
- si Stitch luego entrega una salida estable, puede anexarse como referencia adicional sin desplazar este fallback hasta confirmar equivalencia.

---

**Estado:** prototipo enlazado, navegable y testeable para `VIN-002`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-VIN-002.md`.
