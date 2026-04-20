# PROTOTYPE-TG-002 — Prototipo de recordatorio conversacional

## Propósito

Este documento define el prototipo del slice `TG-002`.

No declara que la validación ya ocurrió ni reemplaza `UXS-TG-002.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué frames y estados cubre y qué queda abierto antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-TG-002.md`
- `../UXI/UXI-TG-002.md`
- `../UJ/UJ-TG-002.md`
- `../VOICE/VOICE-TG-002.md`
- `../UXS/UXS-TG-002.md`
- `../../03_FL/FL-TG-02.md`
- `../../03_FL/FL-REG-02.md`
- `../../06_pruebas/TP-TG.md`
- `../../06_pruebas/TP-REG.md`
- `./PROTOTYPE-TG-001.md`

Y prepara directamente:

- `../UX-VALIDATION/UX-VALIDATION-TG-002.md`

## Slice cubierto

### Caso

`TG-002`: recordatorio y registro conversacional por Telegram.

### Cobertura del prototipo

Este prototipo cubre:

- `S01` recordatorio visible con keyboard inline;
- `S02` respuesta en envío;
- `S03` confirmación o error recuperable.

### Foco principal de observación

- recordatorio opcional;
- claridad del keyboard;
- confirmación breve;
- error sin presión.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy casi final;
- mobile-first;
- gramática visual de chat;
- keyboard inline visible;
- estados sensibles visibles sin perder el tono liviano.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| documento fuente | `./PROTOTYPE-TG-002.md` | dueño del alcance del slice |
| referencia visual Stitch `reminder` | `../../../../artifacts/stitch/tg-002/2026-04-09T00-28-55-455Z/s01-reminder.html` | referencia visual principal del estado `reminder_sent` |
| referencia visual Stitch `submitting` | `../../../../artifacts/stitch/tg-002/2026-04-09T00-34-31-553Z/s01-submitting.html` | referencia visual principal del estado `reply_submitting` |
| referencia visual Stitch `success` | `../../../../artifacts/stitch/tg-002/2026-04-09T00-28-55-455Z/s02-success.html` | referencia visual principal del estado `reply_success` |
| referencia visual Stitch `error` | `../../../../artifacts/stitch/tg-002/2026-04-09T00-28-55-455Z/s03-error.html` | referencia visual principal del estado `reply_error` |
| fallback local consolidado | `./PROTOTYPE-TG-002.html` | wrapper navegable del slice completo basado en los frames Stitch descargados |

### Referencia canónica actual

- [Abrir Stitch `reminder`](../../../../artifacts/stitch/tg-002/2026-04-09T00-28-55-455Z/s01-reminder.html)
- [Abrir Stitch `submitting`](../../../../artifacts/stitch/tg-002/2026-04-09T00-34-31-553Z/s01-submitting.html)
- [Abrir Stitch `success`](../../../../artifacts/stitch/tg-002/2026-04-09T00-28-55-455Z/s02-success.html)
- [Abrir Stitch `error`](../../../../artifacts/stitch/tg-002/2026-04-09T00-28-55-455Z/s03-error.html)
- [Volver al bridge web de Telegram](./PROTOTYPE-TG-001.html)
- [Abrir fallback local consolidado](./PROTOTYPE-TG-002.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `TG-002-S01-REMINDER` | `S01` | mensaje, keyboard y salida fácil | sí |
| `TG-002-S01-SUBMITTING` | `S01` | respuesta en envío | sí |
| `TG-002-S02-SUCCESS` | `S02` | confirmación breve | sí |
| `TG-002-S03-ERROR` | `S03` | error recuperable | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona recibe el recordatorio;
- decide si responde;
- elige un valor con keyboard inline;
- recibe confirmación breve o error recuperable.

### Estados sensibles obligatorios

- reminder;
- submitting;
- success;
- error.

## Hipótesis que este prototipo debe permitir observar

1. El recordatorio se siente opcional.
2. El teclado inline permite responder sin fricción.
3. La confirmación no corta el canal ni se vuelve enfática.
4. El error mantiene un tono amable y recuperable.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-TG-002`;
- mantener estructura y estados alineados con `UXS-TG-002.md`;
- no transformar el chat en formulario pesado;
- no insistir con felicitaciones o insistencia emocional;
- no perder la salida fácil.

## Criterio de readiness antes de validar

El prototipo está listo si:

1. la referencia local es navegable;
2. Stitch cubre todos los estados principales del slice;
3. la experiencia se percibe como conversación breve y opcional;
4. la confirmación no rompe el hilo del canal;
5. el error deja reintento sin presión.

## Supuestos abiertos antes de validación

- la disposición final exacta del inline keyboard puede ajustarse luego según las restricciones reales del bot;
- el HTML local existe como wrapper navegable de los frames Stitch y no como reinterpretación paralela del slice;
- si aparece un hallazgo de wording, vuelve primero a `VOICE-TG-002.md`;
- si aparece un hallazgo de jerarquía o estados, vuelve primero a `UXS-TG-002.md`.

---

## Nota de estado runtime y validación

- `TelegramSession` y `ReminderConfig` existen en runtime (Phase 30+): entities, tablas, seam webhook y `ReminderWorker` activos segun `TECH-TELEGRAM.md`
- la validacion UX real queda diferida a `Phase 60`
- `UI-RFC-TG-002.md` y `HANDOFF-SPEC-TG-002.md` actuan como contrato conversacional para implementacion backend/telegram
- este prototipo mantiene su valor como referencia visual Stitch y HTML local, pero no constituye evidencia de validacion hasta que exista runtime

**Estado:** prototipo enlazado, navegable y testeable para `TG-002`.
**Siguiente capa gobernada:** `../UX-VALIDATION/UX-VALIDATION-TG-002.md`.
