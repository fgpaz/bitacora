# VOICE-VIS-001 — Voz de Timeline longitudinal del paciente

## Propósito

Este documento define la voz específica del slice `VIS-001`.

No reemplaza `UXS` ni fija todo el microcopy final. Su función es volver explícita la regla verbal del caso para que framing, intensidad y wording sensible no queden implícitos.

## Relación con el canon

Este documento depende de:

- `../../13_voz_tono.md`
- `../UXI/UXI-VIS-001.md`
- `../UJ/UJ-VIS-001.md`

Y prepara directamente:

- `../UXS/UXS-VIS-001.md`
- futuro `../PROTOTYPE/PROTOTYPE-VIS-001.md`

## Regla verbal central

ayuda a leer, no interpreta ni diagnostica.

## Objetivos verbales

- nombrar el período y el dato
- no sacar conclusiones por la persona
- mantener calma visual y verbal

## Framing emocional heredado

La voz de este slice debe sonar:

- legible
- calmo
- útil
- propio

No debe sonar como:

- un panel analítico frío

## Direcciones aprobadas

- priorizar una idea principal por bloque;
- usar verbos directos y lenguaje cotidiano;
- volver explícito el control solo cuando cambia algo sensible;
- confirmar hechos sin tono celebratorio.

## Phrasing prohibido

- Tu evolución
- Patrón clínico
- Mejora sostenida

## Terminología del caso

### Preferir

- timeline
- registros
- período

### Evitar

- analítica
- diagnóstico
- monitorización

## Defaults transferibles

- la voz debe reducir fricción, no acompañar de más;
- la explicitud sube en el momento sensible y vuelve a comprimirse después;
- si el slice tiene actor profesional, la propiedad del dato sigue narrada desde límites claros y no desde posesión.

## Criterio de validación rápida

La voz de este slice está bien calibrada si se percibe como breve, clara y coherente con la tarea principal. Está mal calibrada si agrega dramatización, juicio o tecnicismo donde no hace falta.

## Deltas 2026-04-23 — login flow redesign

> 2026-04-23 — sync login flow redesign: deltas verbales aplicados sobre implementación en rama `feature/login-flow-redesign-2026-04-23` (W1–W4), merged a `main` en commit `5d91158`. Fuente de verdad: `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md`. Cubre dashboard paciente.

### Heading del dashboard — saludo contextual

- Titular aprobado: `"Hola. Acá está lo que registraste."` (saludo contextual, canon 10 §refugio; excepción autorizada del copy congelado 2026-04-22 via AskUserQuestion).
- Subtítulo aprobado: `"Solo vos ves lo que registrás. Tus datos son privados."` (copy congelado 2026-04-22, reusado como ancla emocional y de privacidad).

### Rail de acción — CTAs dominante + silencioso

- CTA dominante: `"+ Nuevo registro"` (copy congelado).
- CTA secundario silencioso: `"Check-in diario"` (copy congelado).
- Canon 12 §una acción dominante.

### ShellMenu — labels de cuenta

- Trigger: `"Mi cuenta"` (label default del componente).
- Items: `"Recordatorios"` (→ `/configuracion/telegram`), `"Vínculos"` (→ `/configuracion/vinculos`), `"Cerrar sesión"` (destructive).
- Tono: sustantivos breves, sin verbos al trigger (reserva el verbo al item de acción).

### Formulaciones nuevas aprobadas

- `"Hola. Acá está lo que registraste."` (h1 dashboard — excepción autorizada).
- `"Mi cuenta"` (ShellMenu trigger).
- `"Recordatorios"`, `"Vínculos"`, `"Cerrar sesión"` (items del menu).

### Phrasing prohibido ratificado

- Sin `"Tu evolución"`, `"Tu progreso"`, `"Mejora sostenida"`.
- Sin `"Bienvenido/a"` formal o institucional (el saludo es `"Hola."` sereno y directo).

---

**Estado:** `VOICE` activo para `VIS-001` con deltas 2026-04-23 (saludo dashboard + ShellMenu labels).
**Siguiente capa gobernada:** `../UXS/UXS-VIS-001.md` y futuro `../PROTOTYPE/PROTOTYPE-VIS-001.md`.
