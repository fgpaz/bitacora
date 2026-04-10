# PROTOTYPE-VIN-003 — Prototipo de revocación de vínculo por paciente

## Propósito

Este documento define el prototipo del slice `VIN-003`.

No declara que la validación ya ocurrió ni reemplaza `UXS-VIN-003.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-VIN-003.md`
- `../UXI/UXI-VIN-003.md`
- `../UJ/UJ-VIN-003.md`
- `../VOICE/VOICE-VIN-003.md`
- `../UXS/UXS-VIN-003.md`
- `../../03_FL/FL-VIN-03.md`
- `../../06_pruebas/TP-VIN.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-VIN-003.md`

## Slice cubierto

### Caso

`VIN-003`: revocación de vínculo por paciente.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` entrada con contexto del vínculo actual;
- `S02` confirmación de revocación;
- `S03` resultado con acceso cortado o error recuperable.

### Foco principal de observación

- claridad del impacto antes de confirmar;
- serenidad del momento sensible;
- asimetría clara entre confirmar y conservar;
- dignidad del error sin culpa ni dramatización.

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
| documento fuente | `./PROTOTYPE-VIN-003.md` | dueño del alcance del slice |
| referencia navegable mínima | `./PROTOTYPE-VIN-003.html` | consolida el flujo completo y los estados del slice |
| soporte visual Stitch `success` | `../../../../artifacts/stitch/vin-003/2026-04-08T23-07-28-368Z/s02-success.html` | referencia visual real de la corrida `core` parcial |

### Referencia canónica actual

- [Abrir prototipo local](./PROTOTYPE-VIN-003.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `VIN-003-S01-ENTRY` | `S01` | lectura breve del vínculo actual antes de decidir | sí |
| `VIN-003-S02-DEFAULT` | `S02` | impacto visible y CTA principal disponible | sí |
| `VIN-003-S02-SUBMITTING` | `S02` | feedback corto durante revocación | sí |
| `VIN-003-S03-SUCCESS` | `S03` | vínculo revocado y acceso cortado | sí |
| `VIN-003-S03-ALREADY` | `S03` | caso ya revocado sin duplicar la acción | sí |
| `VIN-003-S03-ERROR` | `S03` | error recuperable con reintento claro | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona identifica el vínculo correcto;
- entiende el impacto antes de tocar la acción principal;
- confirma la revocación;
- sale sabiendo que el acceso quedó cortado.

### Estados sensibles obligatorios

- vínculo ya revocado;
- revocación en curso;
- error recuperable;
- confirmación posterior con acceso cortado.

## Hipótesis que este prototipo debe permitir observar

1. El impacto se entiende antes de confirmar y no recién después.
2. La acción secundaria conserva el vínculo sin competir visualmente con la primaria.
3. El resultado confirma acceso cortado sin volver el paso dramático.
4. El error mantiene calma y no convierte el slice en soporte técnico.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-VIN-003`;
- mantener estructura y estados alineados con `UXS-VIN-003.md`;
- no usar framing culpabilizante ni de pérdida emocional;
- no pedir más datos para revocar;
- no introducir una tercera acción principal;
- no presentar la revocación como castigo o ruptura terapéutica.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. el flujo `S01 -> S03` puede recorrerse sin explicación compensatoria;
3. el impacto del corte de acceso se entiende antes de confirmar;
4. los estados sensibles principales están presentes;
5. mobile y desktop conservan la misma lógica de decisión firme y serena.

## Supuestos abiertos antes de validación

- el nombre visible del profesional puede cambiar según contexto real sin alterar la estructura del slice;
- la corrida Stitch actual sirve como soporte parcial, pero el pack local sigue siendo la referencia consolidada del slice;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-VIN-003.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-VIN-003.md`;
- si más adelante Stitch devuelve el resto de los estados, puede anexarse como referencia adicional sin desplazar este fallback.

---

**Estado:** prototipo enlazado, navegable y testeable para `VIN-003`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-VIN-003.md`.
