# HANDOFF-MAPPING-EXP-001 — Correspondencia diseño-código

## Propósito

Este documento traduce `EXP-001` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/(patient)/exportar/page.tsx` | entrada + orquestación del slice paciente |
| `frontend/components/patient/exportar/ExportPageShell.tsx` | shell del slice |
| `frontend/components/patient/exportar/ExportScopeBlock.tsx` | resumen del alcance del archivo |
| `frontend/components/patient/exportar/PeriodSelector.tsx` | selector simple de período |
| `frontend/components/patient/exportar/ExportActionBar.tsx` | CTA de descarga |
| `frontend/components/patient/exportar/GeneratingFeedback.tsx` | feedback durante preparación |
| `frontend/components/patient/exportar/InlineFeedback.tsx` | error localized con reintento |
| `frontend/lib/patient/exportar.ts` | llamadas a `POST /api/v1/mood-entries/export` |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-DEFAULT` | `ExportScopeBlock.tsx` + `ExportActionBar.tsx` | alcance visible y CTA disponible |
| `S01-PERIOD` | `PeriodSelector.tsx` + `ExportScopeBlock.tsx` actualizado | alcance actualizado |
| `S02-GENERATING` | `GeneratingFeedback.tsx` | certeza tranquila |
| `S02-SUCCESS` | cierre automático o inline confirm | sin pantalla extra |
| `S03-ERROR` | `InlineFeedback.tsx` con reintento | error recuperable |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `exportCsv(period)` | `frontend/lib/patient/exportar.ts` | `POST /api/v1/mood-entries/export` body: `{ "period": string }` |

## Rutas y salidas

| Estado final | Navegación |
| --- | --- |
| `S02-SUCCESS` | volver al timeline o cerrar inline |
| `S01-DEFAULT` | puede accederse desde timeline como ruta independiente |

## Rutas y filenames todavía no existentes en frontend

Estos paths se crean cuando `T04` defina la estructura base del routing paciente:

- `frontend/app/(patient)/exportar/page.tsx`
- `frontend/components/patient/exportar/ExportScopeBlock.tsx`
- `frontend/lib/patient/exportar.ts`

## Backend todavía no existente

El endpoint `POST /api/v1/mood-entries/export` es futuro — contrato marcado como especulativo en `UI-RFC-EXP-001.md`. No debe asumirse runtime de mood entries activo.

El formato exacto del CSV debe definirse en coordinación con `T05`.

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa la exportación usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

---

**Estado:** mapping listo para `frontend/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-EXP-001.md`.
