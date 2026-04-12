# UJ-TG-002 — Journey de Recordatorio y registro conversacional por Telegram

## Propósito

Este documento modela la tarea completa del slice `TG-002`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-TG-002.md`
- `../UXI/UXI-TG-002.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-TG-002.md`
- `../UXS/UXS-TG-002.md`

## Goal del actor

recibir el recordatorio y responder solo si ese momento le sirve.

## Feeling global del journey

Este journey debe sentirse como:

- oportuno
- liviano
- opcional
- breve

Y no como:

- presión o culpa por no responder

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Llegada del recordatorio | recibe una pregunta breve con salida fácil | baja intrusión | sentirse perseguida por el sistema |
| S02 | Elegir si responde | toca un valor o decide dejarlo pasar | autonomía visible | sentir obligación de contestar |
| S03 | Continuidad conversacional | si responde, el bot confirma y puede seguir con factores | flujo natural | que el canal se corte o vuelva torpe |

## Fricción aceptable

- ofrecer una salida como “Ahora no”
- confirmar el registro en una sola línea

## Fricción indebida

- recordatorios culpabilizantes
- mensajes largos
- insistir si el consentimiento no está activo

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| consentimiento revocado | no envía el recordatorio |
| sesión Telegram no vinculada | no envía el recordatorio |
| fallo al registrar la respuesta | explica breve y permite reintentar o dejar pasar |

## Momentos sensibles

- la lectura del recordatorio
- la decisión de responder o no
- la confirmación conversacional

## Paso crítico que requiere UXS

- paso crítico: `S02`
- nombre: Respuesta al recordatorio
- por qué baja a `UXS`: concentra la autonomía y la sensación de baja fricción del canal

## Defaults transferibles

- el journey debe conservar una sola dirección dominante por tramo;
- la explicitud sube solo donde cambia acceso, datos o consentimiento;
- la confirmación posterior al gesto principal debe ser breve y factual.

## Criterio de validación rápida

Este `UJ` está bien modelado si:

- puede contarse de principio a fin sin saltos arbitrarios;
- el paso crítico queda explícito;
- los errores relevantes no se confunden con caminos principales;
- la sensación global sigue siendo consistente con `UXI`.

---

## Nota de estado runtime y validación

- `TelegramSession` y `ReminderConfig` existen en runtime (Phase 30+): entities, tablas, seam webhook y `ReminderWorker` activos segun `TECH-TELEGRAM.md`
- la validacion UX real queda diferida a `Phase 60`
- `UI-RFC-TG-002.md` y `HANDOFF-SPEC-TG-002.md` actuan como contrato conversacional para implementacion backend/telegram
- los pasos criticos declarados en este documento (S02: respuesta al recordatorio) fueron heredados en los estados y triggers del UI-RFC

**Estado:** `UJ` activo para `TG-002`.
**Siguiente capa gobernada:** `../VOICE/VOICE-TG-002.md` y `../UXS/UXS-TG-002.md`.
