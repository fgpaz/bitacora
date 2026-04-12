# HANDOFF-MAPPING-VIN-004 — Correspondencia diseño-código

## Propósito

Este documento traduce `VIN-004` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/(patient)/vinculacion/acceso/page.tsx` | entrada + orquestación del slice paciente |
| `frontend/components/patient/vinculacion/AccessControlFlow.tsx` | flujo completo de gestión de acceso |
| `frontend/components/patient/vinculacion/AccessStatusBlock.tsx` | estado actual legible |
| `frontend/components/patient/vinculacion/AccessToggle.tsx` | control de acceso con efecto visible |
| `frontend/components/patient/vinculacion/AccessResultCard.tsx` | confirmación del nuevo estado |
| `frontend/lib/patient/vinculacion.ts` | llamadas a `PATCH /api/v1/carelinks/<id>/access` |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-DEFAULT` | `AccessStatusBlock.tsx` | efecto del vínculo sin tecnicismo |
| `S02-DEFAULT` | `AccessToggle.tsx` + `AccessStatusBlock.tsx` | cambio con efecto explícito |
| `S02-SUBMITTING` | `AccessToggle.tsx` con estado `loading` | feedback corto |
| `S03-SUCCESS` | `AccessResultCard.tsx` | resultado factual, no dramatismo |
| `S03-INACTIVE` | `AccessStatusBlock.tsx` con mensaje de inactivo | sin toggle activo |
| `S03-ERROR` | inline error con reintento | recuperable |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `updateAccess(linkId, canViewData)` | `frontend/lib/patient/vinculacion.ts` | `PATCH /api/v1/carelinks/<id>/access` body: `{ "canViewData": boolean }` |

## Rutas y salidas

| Estado final | Navegación |
| --- | --- |
| `S03-SUCCESS` | volver a home paciente o lista de vínculos |
| `S03-INACTIVE` | volver a home paciente |

## Rutas y filenames todavía no existentes en frontend

Estos paths se crean cuando `T04` defina la estructura base del routing paciente:

- `frontend/app/(patient)/vinculacion/acceso/page.tsx`
- `frontend/components/patient/vinculacion/AccessControlFlow.tsx`
- `frontend/lib/patient/vinculacion.ts`

## Backend todavía no existente

El endpoint `PATCH /api/v1/carelinks/<id>/access` es futuro — contrato marcado como especulativo en `UI-RFC-VIN-004.md`. No debe asumirse runtime CareLink activo.

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa el flujo de gestión de acceso usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

---

**Estado:** mapping listo para `frontend/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-VIN-004.md`.
