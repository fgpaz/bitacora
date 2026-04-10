# PROTOTYPE-REG-001 — Prototipo del registro rápido de humor vía web

## Propósito

Este documento define el prototipo del slice `REG-001`.

No declara que la validación ya ocurrió ni reemplaza `UXS-REG-001.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-REG-001.md`
- `../UXI/UXI-REG-001.md`
- `../UJ/UJ-REG-001.md`
- `../VOICE/VOICE-REG-001.md`
- `../UXS/UXS-REG-001.md`
- `../../03_FL/FL-REG-01.md`
- `../../06_pruebas/TP-REG.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-REG-001.md`

## Slice cubierto

### Caso

`REG-001`: registro rápido de humor vía web.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` entrada al registro;
- `S02` selección del valor y commit inmediato;
- `S03` confirmación breve.

### Foco principal de observación

- claridad de la escala;
- velocidad percibida del gesto principal;
- certeza de guardado sin pantalla extra;
- dignidad de los estados sensibles.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy casi final;
- estados visibles;
- jerarquía equivalente a la experiencia esperada;
- mobile y desktop con la misma lógica;
- confirmación factual, no decorativa.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| documento fuente | `./PROTOTYPE-REG-001.md` | dueño del alcance del slice |
| referencia navegable mínima | `./PROTOTYPE-REG-001.html` | fuente canónica local hasta que exista referencia externa equivalente |

### Referencia canónica actual

- [Abrir prototipo local](./PROTOTYPE-REG-001.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `REG-001-S01-ENTRY` | `S01` | entrada contextualizada al registro | sí |
| `REG-001-S02-DEFAULT` | `S02` | escala visible y lista para interacción | sí |
| `REG-001-S02-SUBMITTING` | `S02` | feedback breve de guardado | sí |
| `REG-001-S02-ERROR` | `S02` | error recuperable sin perder intención | sí |
| `REG-001-S02-CONSENT` | `S02` | consentimiento faltante o revocado | sí |
| `REG-001-S02-SESSION` | `S02` | sesión expirada con reingreso digno | sí |
| `REG-001-S03-CONFIRM` | `S03` | confirmación factual y continuidad | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona llega al registro con contexto;
- entiende la escala sin explicación extra;
- toca un valor;
- recibe feedback breve;
- sale con certeza de guardado.

### Estados sensibles obligatorios

- error recuperable;
- sesión expirada;
- consentimiento faltante o revocado;
- confirmación posterior al guardado.

## Hipótesis que este prototipo debe permitir observar

1. La escala se entiende de inmediato.
2. El gesto principal se siente instantáneo.
3. La confirmación es suficiente sin volverse celebratoria.
4. Los errores no convierten el slice en mini formulario.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-REG-001`;
- mantener estructura y estados alineados con `UXS-REG-001.md`;
- no agregar campos, notas ni interpretación emocional;
- no usar confirmaciones celebratorias;
- no esconder estados sensibles detrás de explicación oral.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. la escala y la confirmación pueden recorrerse sin ayuda externa;
3. los estados sensibles principales están visibles;
4. mobile y desktop conservan la misma lógica de gesto directo.

## Supuestos abiertos antes de validación

- la confirmación puede seguir siendo mínima mientras no deje dudas de guardado;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-REG-001.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-REG-001.md`.

---

**Estado:** prototipo enlazado, navegable y testeable para `REG-001`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-REG-001.md`.
