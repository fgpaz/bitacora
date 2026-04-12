# HANDOFF-MAPPING-VIS-001 — Correspondencia diseño-código

## Propósito

Este documento traduce `VIS-001` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/(patient)/timeline/page.tsx` | entrada + orquestación del slice paciente |
| `frontend/components/patient/timeline/TimelinePageShell.tsx` | shell del slice con estados de carga |
| `frontend/components/patient/timeline/TimelineChart.tsx` | gráfico longitudinal suavizado |
| `frontend/components/patient/timeline/PeriodFilter.tsx` | selector de período simple |
| `frontend/components/patient/timeline/ChartSkeleton.tsx` | placeholder editorial durante carga |
| `frontend/components/patient/timeline/EmptyStateBlock.tsx` | orientación cuando no hay datos |
| `frontend/components/patient/timeline/InlineFeedback.tsx` | error localized con reintento |
| `frontend/lib/patient/timeline.ts` | llamadas a `GET /api/v1/mood-entries/timeline` |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-LOADING` | `ChartSkeleton.tsx` | placeholder editorial |
| `S01-READY` | `TimelineChart.tsx` con período `month` | chart dominante |
| `S02-PERIOD` | `TimelineChart.tsx` + `PeriodFilter.tsx` | chart actualizado |
| `S03-EMPTY` | `EmptyStateBlock.tsx` | copy útil, no error |
| `S03-ERROR` | `InlineFeedback.tsx` con reintento | error breve |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `getTimeline(period)` | `frontend/lib/patient/timeline.ts` | `GET /api/v1/mood-entries/timeline?period=<week\|month\|quarter>` |

## Rutas y salidas

| Estado final | Navegación |
| --- | --- |
| desde cualquier estado | exportar CSV accesible como acción secundaria desde timeline |

## Rutas y filenames todavía no existentes en frontend

Estos paths se crean cuando `T04` defina la estructura base del routing paciente:

- `frontend/app/(patient)/timeline/page.tsx`
- `frontend/components/patient/timeline/TimelineChart.tsx`
- `frontend/lib/patient/timeline.ts`

## Backend todavía no existente

El endpoint `GET /api/v1/mood-entries/timeline` es futuro — contrato marcado como especulativo en `UI-RFC-VIS-001.md`. No debe asumirse runtime de mood entries activo.

La librería del chart puede variar según decisión técnica de `T04`.

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa el timeline usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

---

**Estado:** mapping listo para `frontend/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-VIS-001.md`.
