# PROTOTYPE-CON-002 — Prototipo de revocación de consentimiento

## Propósito

Este documento define el prototipo del slice `CON-002`.

No declara que la validación ya ocurrió ni reemplaza `UXS-CON-002.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-CON-002.md`
- `../UXI/UXI-CON-002.md`
- `../UJ/UJ-CON-002.md`
- `../VOICE/VOICE-CON-002.md`
- `../UXS/UXS-CON-002.md`
- `../../03_FL/FL-CON-02.md`
- `../../06_pruebas/TP-CON.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-CON-002.md`

## Slice cubierto

### Caso

`CON-002`: revocación de consentimiento.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` entrada al consentimiento activo;
- `S02` revisión del impacto antes de revocar;
- `S03` resultado con consentimiento revocado o error recuperable.

### Foco principal de observación

- claridad de la cascada real antes de confirmar;
- serenidad del momento sensible sin pared de advertencias;
- comprensión de suspensión de registro y accesos;
- dignidad del error con reintento claro.

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
| documento fuente | `./PROTOTYPE-CON-002.md` | dueño del alcance del slice |
| referencia navegable mínima | `./PROTOTYPE-CON-002.html` | consolida el flujo completo y los estados del slice |
| soporte visual Stitch `success` | `../../../../artifacts/stitch/con-002/2026-04-08T23-10-37-785Z/s02-success.html` | referencia visual real de la corrida `core` parcial |
| soporte visual Stitch `error` | `../../../../artifacts/stitch/con-002/2026-04-08T23-10-37-785Z/s03-error.html` | referencia visual real de la corrida `core` parcial |

### Referencia canónica actual

- [Abrir prototipo local](./PROTOTYPE-CON-002.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `CON-002-S01-ENTRY` | `S01` | consentimiento vigente antes de revisar impacto | sí |
| `CON-002-S02-DEFAULT` | `S02` | impacto visible y CTA principal disponible | sí |
| `CON-002-S02-SUBMITTING` | `S02` | feedback corto durante revocación | sí |
| `CON-002-S03-SUCCESS` | `S03` | consentimiento revocado con cascada entendible | sí |
| `CON-002-S03-ALREADY` | `S03` | caso ya revocado sin duplicar acción | sí |
| `CON-002-S03-ERROR` | `S03` | error recuperable con reintento claro | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona llega con consentimiento vigente;
- revisa el impacto en pocas ideas;
- confirma la revocación;
- sale entendiendo suspensión de registro y pérdida de acceso profesional.

### Estados sensibles obligatorios

- consentimiento ya revocado;
- revocación en curso;
- error recuperable;
- confirmación posterior con cascada visible.

## Hipótesis que este prototipo debe permitir observar

1. La cascada de impacto se entiende sin sentirse punitiva.
2. La pausa previa a revocar es seria pero no pesada.
3. El resultado final deja claro que se suspenden registro y accesos.
4. El error mantiene claridad sin volver legalista el paso.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-CON-002`;
- mantener estructura y estados alineados con `UXS-CON-002.md`;
- no convertir el paso en una pared de advertencias;
- no usar tono legalista ni de castigo;
- no ocultar que la revocación afecta tanto registro como acceso;
- no introducir más de una acción principal.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. el flujo `S01 -> S03` puede recorrerse sin explicación compensatoria;
3. la cascada principal se entiende antes de confirmar;
4. los estados sensibles principales están presentes;
5. mobile y desktop conservan la misma lógica de pausa seria y serena.

## Supuestos abiertos antes de validación

- el detalle exacto de la reactivación futura puede ajustarse después si cambia el contrato operativo;
- la corrida Stitch actual sirve como soporte parcial, pero el pack local sigue siendo la referencia consolidada del slice;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-CON-002.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-CON-002.md`;
- si Stitch más adelante devuelve la entrada completa, puede anexarse como referencia adicional sin desplazar este fallback.

---

**Estado:** prototipo enlazado, navegable y testeable para `CON-002`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-CON-002.md`.
