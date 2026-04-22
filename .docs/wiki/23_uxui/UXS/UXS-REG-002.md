# UXS-REG-002 — Envío del check-in diario

## Propósito

Este documento fija el contrato UX del paso crítico del slice `REG-002`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-REG-002.md`
- `../UXI/UXI-REG-002.md`
- `../UJ/UJ-REG-002.md`
- `../VOICE/VOICE-REG-002.md`
- `../../03_FL/FL-REG-03.md`
- `../../06_pruebas/TP-REG.md`

Y prepara directamente:

- `../PROTOTYPE/PROTOTYPE-REG-002.md`
- `../UI-RFC/UI-RFC-REG-002.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-REG-002.md`

## Slice y paso dueño

- slice: `REG-002`
- paso crítico: `S03` — Revisión final y envío del check-in
- entrada: la persona ya recorrió los bloques de factores y necesita cerrar el registro sin fricción extra
- salida correcta: el DailyCheckin queda guardado y la confirmación es breve

## Sensación del paso

- sensación objetivo: un cierre claro de un formulario corto
- anti-sensación: tener que negociar con la interfaz antes de guardar

## Tarea del usuario

1. revisar lo cargado
2. detectar si falta algo realmente obligatorio
3. guardar el check-in

## Contrato de interacción

### Estructura mínima

- encabezado breve
- bloques agrupados por tema
- área final de guardado con una sola acción principal

### Acción primaria

- `Guardar check-in`

### Acción secundaria

- `Salir por ahora`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| default | bloques visibles y CTA deshabilitada o lista según completitud | permite avanzar sin ruido |
| ready_to_submit | CTA habilitada | confirma que ya puede guardar |
| submitting | feedback corto de guardado | evita doble envío |
| error_retryable | error localizado en el área final | permite corregir o reintentar |

## Contrato de copy

- titular aprobado: `Completá tu check-in de hoy`
- texto de apoyo aprobado: `Solo pedimos lo necesario para darle contexto a tus registros.`
- acción primaria aprobada: `Guardar check-in`
- error recuperable aprobado: `No pudimos guardar este check-in. Probá de nuevo.`

## Aceptación

1. el formulario no parece una encuesta extensa
2. la acción principal es inequívoca
3. el guardado no introduce una pantalla extra

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

## Deltas 2026-04-22 — impeccable-hardening

> 2026-04-22 — sync impeccable-hardening: deltas aplicados sobre implementación en rama `feature/impeccable-hardening-2026-04-22` (W2–W3–W7). Fuente de verdad: `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md`.

### DailyCheckinForm — accesibilidad y semántica

- Botones Sí/No de los 4 factores booleanos (actividad física, actividad social, ansiedad, irritabilidad) y de medicación: `aria-pressed={form[key] === true/false}` aplicado en los 10 botones (W2). WCAG 4.1.2 — estado comunicado via ARIA.
- Input `sleep_hours`: `aria-required="true"` agregado (W2).

### DailyCheckinForm — error visual no dependiente de color

- CSS `.blockError`: `border-left-color: var(--status-danger)` + `border-left-width: 4px` para que el estado de error sea perceptible sin depender únicamente del color (WCAG 1.4.1) (W3).

### Tipografía y focus

- Heading `.headline`: `clamp(1.5rem, 4vw, 2rem)` — tipografía fluida adaptada a viewports (W7).
- Focus-visible con outline real compatible con High Contrast Mode (HCM-safe) (W3).

### Error fallback

- Mensaje de error recuperable especificado como `"No pudimos guardar el check-in. Probá de nuevo."` (canon 13 §Errores) (W2).

### Notas de implementación

- Todos los cambios son `ui-only, no-schema, no-contract, no-auth`.

---

**Estado:** `UXS` activo para `REG-002`.
**Siguiente capa gobernada:** `../PROTOTYPE/PROTOTYPE-REG-002.md`, `../UI-RFC/UI-RFC-REG-002.md` y futuro `../UX-VALIDATION/UX-VALIDATION-REG-002.md`.
