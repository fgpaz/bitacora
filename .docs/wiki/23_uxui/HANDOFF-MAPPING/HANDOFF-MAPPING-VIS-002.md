# HANDOFF-MAPPING-VIS-002 — Correspondencia diseño-código

## Propósito

Este documento traduce `VIS-002` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/(professional)/dashboard/page.tsx` | entrada + orquestación del slice profesional |
| `frontend/components/professional/dashboard/ProfessionalDashboardShell.tsx` | shell del slice con estados |
| `frontend/components/professional/dashboard/PatientCard.tsx` | tarjeta de resumen por paciente |
| `frontend/components/professional/dashboard/PatientList.tsx` | lista paginada |
| `frontend/components/professional/dashboard/PaginationControls.tsx` | controles de paginación |
| `frontend/components/professional/dashboard/EmptyPatientsBlock.tsx` | copy claro cuando no hay pacientes |
| `frontend/components/professional/dashboard/InlineFeedback.tsx` | error de carga con reintento |
| `frontend/lib/professional/dashboard.ts` | llamadas a `GET /api/v1/carelinks/patients` |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-LOADING` | shell con skeleton o placeholder | anticipa la lista |
| `S02-READY` | `PatientList.tsx` con `PatientCard`s | lista priorizable |
| `S02-PAGINATION` | `PaginationControls.tsx` | siguiente página sin recarga |
| `S04-EMPTY` | `EmptyPatientsBlock.tsx` | copy claro, no error |
| `S03-ERROR` | `InlineFeedback.tsx` con reintento | error breve |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `getPatients(page?)` | `frontend/lib/professional/dashboard.ts` | `GET /api/v1/carelinks/patients?page=<n>` |

## Rutas y salidas

| Estado final | Navegación |
| --- | --- |
| click en `PatientCard` | deriva a detalle del paciente cuando exista `VIS-002` |
| desde dashboard | acceder a vinculación si corresponde |

## Rutas y filenames todavía no existentes en frontend

Estos paths se crean cuando `T04` defina la estructura base del routing profesional:

- `frontend/app/(professional)/dashboard/page.tsx`
- `frontend/components/professional/dashboard/PatientList.tsx`
- `frontend/lib/professional/dashboard.ts`

## Backend todavía no existente

Los endpoints de CareLink para `GET /api/v1/carelinks/patients` son futuros — contratos marcados como especulativos en `UI-RFC-VIS-002.md`. No debe asumirse runtime CareLink activo.

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa el dashboard usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

---

**Estado:** mapping listo para `frontend/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-VIS-002.md`.
