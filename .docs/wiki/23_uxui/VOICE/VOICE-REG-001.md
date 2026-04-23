# VOICE-REG-001 — Voz de Registro rápido de humor vía web

## Propósito

Este documento define la voz específica del slice `REG-001`.

No reemplaza `UXS` ni fija todo el microcopy final. Su función es volver explícita la regla verbal del caso para que framing, intensidad y wording sensible no queden implícitos.

## Relación con el canon

Este documento depende de:

- `../../13_voz_tono.md`
- `../UXI/UXI-REG-001.md`
- `../UJ/UJ-REG-001.md`

Y prepara directamente:

- `../UXS/UXS-REG-001.md`
- `../UI-RFC/UI-RFC-REG-001.md`
- `../PROTOTYPE/PROTOTYPE-REG-001.md`

## Regla verbal central

pregunta directa, cero interpretación y confirmación factual.

## Objetivos verbales

- preguntar lo justo
- no interpretar el estado de ánimo
- confirmar sin celebrar

## Framing emocional heredado

La voz de este slice debe sonar:

- instantáneo
- liviano
- propio
- seguro
- sin juicio

No debe sonar como:

- un mini formulario emocional que demora más de lo que ayuda

## Direcciones aprobadas

- priorizar una idea principal por bloque;
- usar verbos directos y lenguaje cotidiano;
- volver explícito el control solo cuando cambia algo sensible;
- confirmar hechos sin tono celebratorio.

## Phrasing prohibido

- ¿Cómo va tu día?
- Contanos más si querés
- Excelente, seguí así

## Terminología del caso

### Preferir

- registro
- humor
- valor

### Evitar

- evaluación
- progreso
- estado clínico

## Defaults transferibles

- la voz debe reducir fricción, no acompañar de más;
- la explicitud sube en el momento sensible y vuelve a comprimirse después;
- si el slice tiene actor profesional, la propiedad del dato sigue narrada desde límites claros y no desde posesión.

## Criterio de validación rápida

La voz de este slice está bien calibrada si se percibe como breve, clara y coherente con la tarea principal. Está mal calibrada si agrega dramatización, juicio o tecnicismo donde no hace falta.

## Deltas 2026-04-23 — login flow redesign

> 2026-04-23 — sync login flow redesign: deltas verbales aplicados sobre implementación en rama `feature/login-flow-redesign-2026-04-23` (W1–W4), merged a `main` en commit `5d91158`. Fuente de verdad: `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md`.

### Toast post-success del dashboard

- Copy aprobado: `"Registro sumado a tu historial."` (factual, canon 13, sin celebración).
- Se emite en el dashboard tras el cierre automático del modal (no en el modal mismo, para evitar doble confirmación).
- Accesibilidad: `role=status aria-live=polite`, timeout 4000ms.

### Puente embedded post-success del modal

- Copy aprobado: `"Volviendo al dashboard…"` (factual, concreto, sin ceremonia).
- Se muestra durante la transición de 800ms antes del cierre del modal.
- No es celebración; es confirmación de continuidad.

### Formulaciones nuevas aprobadas

- `"Registro sumado a tu historial."` (toast dashboard post-success).
- `"Volviendo al dashboard…"` (puente embedded modal).

### Phrasing prohibido ratificado

- Nada de `"¡Excelente!"` ni `"¡Bien hecho!"`.
- Nada de `"Seguí así"` ni `"Un día más registrado"`.
- Nada de `"Tu registro fue un éxito"`.

---

**Estado:** `VOICE` activo para `REG-001` con deltas 2026-04-23 (toast dashboard + puente embedded).
**Siguiente capa gobernada:** `../UXS/UXS-REG-001.md`, `../UI-RFC/UI-RFC-REG-001.md` y `../PROTOTYPE/PROTOTYPE-REG-001.md`.
