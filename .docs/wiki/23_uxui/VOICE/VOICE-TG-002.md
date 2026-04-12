# VOICE-TG-002 — Voz de Recordatorio y registro conversacional por Telegram

## Propósito

Este documento define la voz específica del slice `TG-002`.

No reemplaza `UXS` ni fija todo el microcopy final. Su función es volver explícita la regla verbal del caso para que framing, intensidad y wording sensible no queden implícitos.

## Relación con el canon

Este documento depende de:

- `../../13_voz_tono.md`
- `../UXI/UXI-TG-002.md`
- `../UJ/UJ-TG-002.md`

Y prepara directamente:

- `../UXS/UXS-TG-002.md`
- futuro `../PROTOTYPE/PROTOTYPE-TG-002.md`

## Regla verbal central

pregunta breve, salida fácil y cero presión.

## Objetivos verbales

- sonar opcional
- llevar directo al gesto principal
- confirmar sin insistencia

## Framing emocional heredado

La voz de este slice debe sonar:

- oportuno
- liviano
- opcional
- breve

No debe sonar como:

- presión o culpa por no responder

## Direcciones aprobadas

- priorizar una idea principal por bloque;
- usar verbos directos y lenguaje cotidiano;
- volver explícito el control solo cuando cambia algo sensible;
- confirmar hechos sin tono celebratorio.

## Phrasing prohibido

- No te olvides de registrarte
- Es importante que respondas ahora
- Seguimos esperando

## Terminología del caso

### Preferir

- ¿Cómo te sentís?
- Ahora no
- Registrado

### Evitar

- cumplir
- seguimiento obligatorio
- pendiente

## Defaults transferibles

- la voz debe reducir fricción, no acompañar de más;
- la explicitud sube en el momento sensible y vuelve a comprimirse después;
- si el slice tiene actor profesional, la propiedad del dato sigue narrada desde límites claros y no desde posesión.

## Criterio de validación rápida

La voz de este slice está bien calibrada si se percibe como breve, clara y coherente con la tarea principal. Está mal calibrada si agrega dramatización, juicio o tecnicismo donde no hace falta.

---

## Nota de estado runtime y validación

- `TelegramSession` y `ReminderConfig` existen en runtime (Phase 30+): entities, tablas, seam webhook y `ReminderWorker` activos segun `TECH-TELEGRAM.md`
- la validacion UX real queda diferida a `Phase 60`
- `UI-RFC-TG-002.md` y `HANDOFF-SPEC-TG-002.md` actuan como contrato conversacional para implementacion backend/telegram
- la terminologia aprobada y el phrasing prohibido de este documento fueron incorporados directamente en los contratos de copy del UI-RFC y HANDOFF-SPEC

**Estado:** `VOICE` activo para `TG-002`.
**Siguiente capa gobernada:** `../UXS/UXS-TG-002.md` y futuro `../PROTOTYPE/PROTOTYPE-TG-002.md`.
