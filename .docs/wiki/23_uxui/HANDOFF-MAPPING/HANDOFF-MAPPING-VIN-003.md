# HANDOFF-MAPPING-VIN-003 — Correspondencia diseño-código

## Propósito

Este documento traduce `VIN-003` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/(patient)/vinculacion/revocar/page.tsx` | entrada + orquestación del slice paciente |
| `frontend/components/patient/vinculacion/RevokeLinkFlow.tsx` | flujo completo de revocación |
| `frontend/components/patient/vinculacion/VinculoActualBlock.tsx` | lectura breve del vínculo actual |
| `frontend/components/patient/vinculacion/ImpactCascadeBlock.tsx` | enumeración del impacto |
| `frontend/components/patient/vinculacion/RevokeLinkBar.tsx` | acción primaria de revocación |
| `frontend/lib/patient/vinculacion.ts` | llamadas a `DELETE /api/v1/carelinks/<id>` |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-DEFAULT` | `VinculoActualBlock.tsx` | saber con quién está vinculado |
| `S02-DEFAULT` | `ImpactCascadeBlock.tsx` + `RevokeLinkBar.tsx` | impacto + CTA disponible |
| `S02-SUBMITTING` | `RevokeLinkBar.tsx` con estado `loading` | feedback corto |
| `S03-SUCCESS` | inline confirmation factual | acceso cortado sin drama |
| `S03-ALREADY` | estado actual sin repetir acción | navegación directa |
| `S03-ERROR` | inline error con reintento | recuperable |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `revokeLink(linkId)` | `frontend/lib/patient/vinculacion.ts` | `DELETE /api/v1/carelinks/<id>` |

## Rutas y salidas

| Estado final | Navegación |
| --- | --- |
| `S03-SUCCESS` | volver a home paciente |
| `S03-ALREADY` | volver a home paciente |

## Rutas y filenames todavía no existentes en frontend

Estos paths se crean cuando `T04` defina la estructura base del routing paciente:

- `frontend/app/(patient)/vinculacion/revocar/page.tsx`
- `frontend/components/patient/vinculacion/RevokeLinkFlow.tsx`
- `frontend/lib/patient/vinculacion.ts`

## Backend todavía no existente

El endpoint `DELETE /api/v1/carelinks/<id>` es futuro — contrato marcado como especulativo en `UI-RFC-VIN-003.md`. No debe asumirse runtime CareLink activo.

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa el flujo de revocación usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

---

**Estado:** mapping listo para `frontend/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-VIN-003.md`.
