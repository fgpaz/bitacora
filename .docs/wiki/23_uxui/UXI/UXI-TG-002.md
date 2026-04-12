# UXI-TG-002 — Intención de Recordatorio y registro conversacional por Telegram

## Propósito

Este documento fija la intención emocional y operativa del slice `TG-002`.

No describe todavía la journey completa ni el detalle de una pantalla. Su función es decidir cómo debe sentirse el caso para que `UJ`, `VOICE` y `UXS` no improvisen tono, ritmo o confianza.

## Relación con el canon

Este documento depende de:

- `../../10_manifiesto_marca_experiencia.md`
- `../../11_identidad_visual.md`
- `../../12_lineamientos_interfaz_visual.md`
- `../../13_voz_tono.md`
- `../UXR/UXR-TG-002.md`

Y prepara directamente:

- `../UJ/UJ-TG-002.md`
- `../VOICE/VOICE-TG-002.md`
- `../UXS/UXS-TG-002.md`

## Sensación deseada

- oportuno
- liviano
- opcional
- breve

La intención de este caso es que la experiencia se sienta como `toque breve`.

## Anti-sensación

La anti-sensación principal a evitar es:

**presión o culpa por no responder.**

## Postura de confianza

la confianza se sostiene respetando silencio, timing y salida fácil.

## Goal del actor

recibir el recordatorio y responder solo si ese momento le sirve.

## Fricción aceptable

- ofrecer una salida como “Ahora no”
- confirmar el registro en una sola línea

## Fricción indebida

- recordatorios culpabilizantes
- mensajes largos
- insistir si el consentimiento no está activo

## Defaults del caso

- hereda la base del sistema: casi nula fricción, seguridad implícita, simpleza radical, paciente primero cuando el actor principal es la persona usuaria, silencio útil fuera de los momentos sensibles, explicitud alta solo cuando cambian acceso, datos o consentimiento;
- la sensación dominante del slice es `oportuno`, pero sin sacrificar claridad sensible;
- la voz y la interfaz deben sostener una sola dirección dominante por paso.

## Criterio de validación rápida

Este `UXI` está bien calibrado si el caso se percibe como:

- oportuno
- liviano
- opcional
- breve

Y está mal calibrado si se percibe como:

- presión o culpa por no responder
- una experiencia que pide más esfuerzo que el valor que devuelve.

---

## Nota de estado runtime y validación

- `TelegramSession` y `ReminderConfig` existen en runtime (Phase 30+): entities, tablas, seam webhook y `ReminderWorker` activos segun `TECH-TELEGRAM.md`
- la validacion UX real queda diferida a `Phase 60`
- `UI-RFC-TG-002.md` y `HANDOFF-SPEC-TG-002.md` actuan como contrato conversacional para implementacion backend/telegram
- las sensaciones declaradas en este documento (oportunidad, liviandad, opcionalidad) fueron incorporadas en los contratos de copy del UI-RFC

**Estado:** `UXI` activo para `TG-002`.
**Siguiente capa gobernada:** `../UJ/UJ-TG-002.md`, `../VOICE/VOICE-TG-002.md` y `../UXS/UXS-TG-002.md`.
