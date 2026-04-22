# UXS-VIS-001 — Lectura inicial del timeline

## Propósito

Este documento fija el contrato UX del paso crítico del slice `VIS-001`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIS-001.md`
- `../UXI/UXI-VIS-001.md`
- `../UJ/UJ-VIS-001.md`
- `../VOICE/VOICE-VIS-001.md`
- `../../03_FL/FL-VIS-01.md`
- `../../06_pruebas/TP-VIS.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-VIS-001.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-VIS-001.md`

## Slice y paso dueño

- slice: `VIS-001`
- paso crítico: `S02` — Lectura inicial y ajuste de período
- entrada: la persona abre su historial y necesita entender rápido qué está viendo
- salida correcta: puede leer el período actual o cambiarlo sin perderse

## Sensación del paso

- sensación objetivo: una lectura calma y utilizable
- anti-sensación: un gráfico frío que exige demasiada interpretación

## Tarea del usuario

1. ubicar el período actual
2. leer el gráfico base
3. cambiar período si hace falta

## Contrato de interacción

### Estructura mínima

- encabezado corto
- gráfico principal
- controles de período simples
- estado vacío claro cuando corresponda

### Acción primaria

- `Aplicar período`

### Acción secundaria

- `Restablecer`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| loading | skeleton o placeholder calmo | anticipa el gráfico sin ruido |
| ready | gráfico y filtros visibles | permite lectura y ajuste |
| empty | mensaje claro de ausencia de datos | evita confusión con error |
| error_retryable | error breve de carga | permite reintentar |

## Contrato de copy

- titular aprobado: `Tu timeline`
- texto de apoyo aprobado: `Revisá tus registros por período sin perder el hilo.`
- acción primaria aprobada: `Aplicar período`
- error recuperable aprobado: `No pudimos cargar este período. Probá de nuevo.`

## Aceptación

1. el gráfico principal aparece antes que controles accesorios
2. los filtros no interrumpen la lectura inicial
3. el estado vacío se distingue del error

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

## Deltas 2026-04-22 — impeccable-hardening

> 2026-04-22 — sync impeccable-hardening: deltas aplicados sobre implementación en rama `feature/impeccable-hardening-2026-04-22` (W2–W3–W6–W9–W10). Fuente de verdad: `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md`. Cubre también RF-VIS-015 (dashboard paciente inline entry).

### Dashboard — empty state rediseñado

- `DashboardSummary` ocultado cuando `entries.length === 0` (canon 10 §Anti-señales): mostrar "Registros totales: 0" antes del primer registro del paciente es un anti-patrón de vigilancia detectado en critique T1 (W9).
- Empty state wrapper: `role="region" aria-label="Historial vacío"` — semántica permanente (no transitoria) (W9).
- Sub-copy del empty state: `"Cuando cargues tu primer registro, lo vas a ver acá."` — elimina repetición con el h2 del área principal (W9).
- Espaciado: padding/margin del ritmo de empty state usa `--space-xl` por defecto; `.emptyTitle` y `.errorTitle` separados en clases distintas para evitar colisión de estilos (W9).

### Dashboard — language y aria

- `aria-label` en `scoreBadge`: `"Estado de ánimo"` reemplaza `"Puntaje de humor"` (scoring language eliminado; canon 13) (W2).
- Entries null-fallback: `"Sin registro"` reemplaza `"Sin puntaje"` (W2).

### MoodEntryDialog — accesibilidad y microanimación

- Fade-in de 200ms sobre `.dialog[open]` (W10).
- `aria-modal="true"` en el elemento dialog (W3).
- Focus restore al elemento disparador via `useRef` + `requestAnimationFrame` al cerrar el modal (W3).
- Backdrop: `var(--overlay-backdrop)` reemplaza el `rgba` hardcoded anterior (W4).

### Timeline.tsx (profesional) — performance

- `useMemo` sobre `extractMoodPoints(entries)` para evitar recálculo en cada render (W5/W6).

### Zonas congeladas

- Copy `"+ Nuevo registro"`, `"Check-in diario"`, `"Empezá con tu primer registro"` y `"Registrar humor"` no fueron modificados.
- `DashboardSummary` se sigue mostrando en estado `ready` (con registros); solo se oculta en estado `empty`.

### Notas de implementación

- Todos los cambios son `ui-only, no-schema, no-contract, no-auth`.

---

**Estado:** `UXS` activo para `VIS-001`.
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-VIS-001.md` y `../UX-VALIDATION/UX-VALIDATION-VIS-001.md`.
