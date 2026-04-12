# PROTOTYPE-TG-001 — Prototipo de vinculación de Telegram

## Propósito

Este documento define el prototipo del slice `TG-001`.

No declara que la validación ya ocurrió ni reemplaza `UXS-TG-001.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-TG-001.md`
- `../UXI/UXI-TG-001.md`
- `../UJ/UJ-TG-001.md`
- `../VOICE/VOICE-TG-001.md`
- `../UXS/UXS-TG-001.md`
- `../../03_FL/FL-TG-01.md`
- `../../06_pruebas/TP-TG.md`

Y prepara directamente:

- futuro `../UX-VALIDATION/UX-VALIDATION-TG-001.md`
- flujo enlazado `./PROTOTYPE-TG-002.md`

## Slice cubierto

### Caso

`TG-001`: vinculación de cuenta Telegram.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` entrada al puente web;
- `S02` generación del código y expiración;
- `S03` vínculo exitoso o error recuperable.

### Foco principal de observación

- claridad del puente corto;
- legibilidad del código y vencimiento;
- siguiente paso inequívoco;
- confirmación final sin fricción.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy casi final;
- mobile-first;
- un solo paso dominante por pantalla;
- estados sensibles visibles;
- continuidad clara hacia el canal Telegram.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| documento fuente | `./PROTOTYPE-TG-001.md` | dueño del alcance del slice |
| referencia visual Stitch `idle` | `../../../../artifacts/stitch/tg-001/2026-04-09T00-25-58-263Z/s01-idle.html` | referencia visual principal del estado `idle` |
| referencia visual Stitch `code` | `../../../../artifacts/stitch/tg-001/2026-04-09T00-25-58-263Z/s02-code.html` | referencia visual principal del estado `code_generated` |
| referencia visual Stitch `expired` | `../../../../artifacts/stitch/tg-001/2026-04-09T00-34-31-411Z/s02-expired.html` | referencia visual principal del estado `expired` |
| referencia visual Stitch `linked` | `../../../../artifacts/stitch/tg-001/2026-04-09T00-25-58-263Z/s03-linked.html` | referencia visual principal del estado `linked` |
| referencia visual Stitch `error` | `../../../../artifacts/stitch/tg-001/2026-04-09T00-34-31-345Z/s03-error.html` | referencia visual principal del estado `error_retryable` |
| fallback local consolidado | `./PROTOTYPE-TG-001.html` | wrapper navegable del slice completo basado en los frames Stitch descargados |

### Referencia canónica actual

- [Abrir Stitch `idle`](../../../../artifacts/stitch/tg-001/2026-04-09T00-25-58-263Z/s01-idle.html)
- [Abrir Stitch `code`](../../../../artifacts/stitch/tg-001/2026-04-09T00-25-58-263Z/s02-code.html)
- [Abrir Stitch `expired`](../../../../artifacts/stitch/tg-001/2026-04-09T00-34-31-411Z/s02-expired.html)
- [Abrir Stitch `linked`](../../../../artifacts/stitch/tg-001/2026-04-09T00-25-58-263Z/s03-linked.html)
- [Abrir Stitch `error`](../../../../artifacts/stitch/tg-001/2026-04-09T00-34-31-345Z/s03-error.html)
- [Abrir flujo enlazado de recordatorio conversacional](./PROTOTYPE-TG-002.html)
- [Abrir fallback local consolidado](./PROTOTYPE-TG-001.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `TG-001-S01-IDLE` | `S01` | entrada al puente web | sí |
| `TG-001-S02-CODE` | `S02` | código visible y siguiente paso | sí |
| `TG-001-S02-EXPIRED` | `S02` | vencimiento y regeneración | sí |
| `TG-001-S03-LINKED` | `S03` | vínculo ya activo | sí |
| `TG-001-S03-ERROR` | `S03` | error recuperable | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona decide habilitar Telegram;
- genera un código;
- entiende el siguiente paso;
- reacciona si el código vence;
- confirma que el vínculo quedó activo.

### Estados sensibles obligatorios

- idle;
- code generated;
- expired;
- linked;
- error recuperable.

## Hipótesis que este prototipo debe permitir observar

1. El puente se entiende sin parecer wizard de setup.
2. El vencimiento se entiende sin alarmismo.
3. El siguiente paso hacia el bot es inequívoco.
4. La confirmación final cierra el flujo con calma.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-TG-001`;
- mantener estructura y estados alineados con `UXS-TG-001.md`;
- no usar lenguaje de tokens o API;
- no dispersar el siguiente paso en varias acciones equivalentes;
- no dramatizar el vencimiento.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. Stitch cubre todos los estados principales del slice;
3. el handoff al bot se entiende rápido;
4. el vencimiento mantiene tono sereno;
5. el enlace a `TG-002` existe como continuidad del canal.

## Supuestos abiertos antes de validación

- la forma exacta del CTA hacia el bot puede ajustarse luego según integración real del enlace profundo;
- el HTML local existe como wrapper navegable de los frames Stitch y no como reinterpretación paralela del slice;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-TG-001.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-TG-001.md`.

---

## Nota de estado runtime y validación

- `TelegramSession` existe en runtime (Phase 30+): entity, tabla, seam webhook y `ReminderWorker` materializados segun `TECH-TELEGRAM.md`
- la validacion UX real queda diferida a `Phase 60`
- `UI-RFC-TG-001.md` y `HANDOFF-SPEC-TG-001.md` actuan como contrato conversacional para implementacion backend/telegram
- este prototipo mantiene su valor como referencia visual Stitch y HTML local, pero no constituye evidencia de validacion hasta que exista runtime

**Estado:** prototipo enlazado, navegable y testeable para `TG-001`.
**Siguiente capa gobernada:** futuro `../UX-VALIDATION/UX-VALIDATION-TG-001.md`.
