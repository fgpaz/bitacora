# UXS-REG-001 — Commit inmediato del humor

## Propósito

Este documento fija el contrato UX del paso crítico del slice `REG-001`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-REG-001.md`
- `../UXI/UXI-REG-001.md`
- `../UJ/UJ-REG-001.md`
- `../VOICE/VOICE-REG-001.md`
- `../../03_FL/FL-REG-01.md`
- `../../06_pruebas/TP-REG.md`

Y prepara directamente:

- `../PROTOTYPE/PROTOTYPE-REG-001.md`
- `../UI-RFC/UI-RFC-REG-001.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-REG-001.md`

## Slice y paso dueño

- slice: `REG-001`
- paso crítico: `S02` — Selección y confirmación inmediata del humor
- entrada: la persona llega con intención de registrar rápido
- salida correcta: el MoodEntry queda guardado y la confirmación aparece sin interrumpir

## Sensación del paso

- sensación objetivo: un gesto breve, claro y sin juicio
- anti-sensación: tener que pensar más en la interfaz que en el dato

## Tarea del usuario

1. ubicar la escala
2. elegir un valor
3. entender que el dato quedó guardado

## Contrato de interacción

### Estructura mínima

- encabezado mínimo
- escala visible de -3..+3
- feedback breve tras el gesto principal

### Acción primaria

- `Tocar un valor de la escala`

### Acción secundaria

- `Cancelar`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| default | escala visible y lista para interacción | permite elegir un valor |
| submitting | valor seleccionado con feedback de guardado | evita doble envío |
| success_transition | confirmación factual breve | deja seguir sin pantalla extra |
| error_retryable | mensaje corto de error | permite reintentar sin perder contexto |

## Contrato de copy

- titular aprobado: `¿Cómo te sentís ahora?`
- texto de apoyo aprobado: `Elegí un valor y lo registramos enseguida.`
- acción primaria aprobada: `Registrar valor`
- error recuperable aprobado: `No pudimos guardar este registro. Probá de nuevo.`

## Aceptación

1. la escala se entiende sin explicación adicional
2. el gesto principal se resuelve en un solo paso
3. la confirmación no compite con la acción siguiente

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

## Deltas 2026-04-23 — login flow redesign

> 2026-04-23 — sync login flow redesign: deltas aplicados sobre implementación en rama `feature/login-flow-redesign-2026-04-23` (W1–W4), merged a `main` en commit `5d91158`. Fuente de verdad: `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md`.

### MoodEntryDialog — cierre automático post-success + toast

- Tras `handleEntrySaved()` en `Dashboard.tsx`, el modal cierra automáticamente 800ms post-success (delay suficiente para leer la confirmación inline sin retenerlo como pared). `closeDialog()` se invoca desde el effect del modal, no desde un click manual.
- El dashboard emite un toast `role=status aria-live=polite` con `"Registro sumado a tu historial."` (copy canon 13, factual, sin celebración). Timeout 4000ms.
- Resuelve el hallazgo E4-F1+E4-F2 P0 del audit 2026-04-23: el modal dejaba al paciente abierto sin instrucción, rompiendo el modelo mental de "registré algo nuevo".

### MoodEntryForm — puente embedded

- El bloque `{!embedded && ...}` de `MoodEntryForm.tsx` gana contraparte para `embedded=true`: puente condensado `"Volviendo al dashboard…"` con `role=status`. Patrón 16 #12 "Puente de siguiente acción" cumplido en ambos modos de renderizado.

### Modelo de estados ampliado

| Estado | Qué ve la persona | Delta 2026-04-23 |
| --- | --- | --- |
| `success_transition` (extendido) | confirmación breve seguida por cierre automático del modal tras 800ms, más toast en el dashboard | cierre auto + toast con aria-live |

### Notas de implementación

- Todos los cambios `ui-only, no-schema, no-contract`.
- Copy congelado preservado: `"Registro guardado."` (interno del modal), `"Registrar humor"`, `"Nuevo registro"`.
- Nuevo copy aprobado: `"Registro sumado a tu historial."` (toast), `"Volviendo al dashboard…"` (puente embedded).

---

**Estado:** `UXS` activo para `REG-001` con deltas 2026-04-23 (cierre auto + puente embedded + toast).
**Siguiente capa gobernada:** `../PROTOTYPE/PROTOTYPE-REG-001.md`, `../UI-RFC/UI-RFC-REG-001.md` y futuro `../UX-VALIDATION/UX-VALIDATION-REG-001.md`.
