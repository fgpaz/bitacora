# HANDOFF-MAPPING-VIN-002 — Correspondencia diseño-código

## Propósito

Este documento traduce `VIN-002` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/(patient)/vinculacion/codigo/page.tsx` | entrada + orquestación del slice paciente |
| `frontend/components/patient/vinculacion/CodeBindFlow.tsx` | flujo completo con estados |
| `frontend/components/patient/vinculacion/CodeInput.tsx` | campo de código con validación local |
| `frontend/components/patient/vinculacion/VinculoActivoCard.tsx` | confirmación de vínculo con acceso desactivado |
| `frontend/lib/patient/vinculacion.ts` | llamadas a `POST /api/v1/carelinks/bind` |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-DEFAULT` | `CodeBindFlow.tsx` | contexto breve y código pendiente |
| `S01-READY` | `CodeInput.tsx` + CTA habilitada | código listo para enviar |
| `S01-SUBMITTING` | `CodeInput.tsx` + estado `loading` | feedback corto |
| `S02-SUCCESS` | `VinculoActivoCard.tsx` | vínculo con `canViewData=false` |
| `S03-INVALID` | `CodeInput.tsx` con inline error | código inválido recuperable |
| `S03-EXPIRED` | inline message con salida clara | sin reproceso |
| `S03-ALREADY` | estado existente sin duplicar | navegación directa |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `bindWithCode(code)` | `frontend/lib/patient/vinculacion.ts` | `POST /api/v1/carelinks/bind` body: `{ "code": string }` |

## Rutas y salidas

| Estado final | Navegación |
| --- | --- |
| `VinculoActivoCard` | volver a home paciente con vínculo visible |
| `S03-EXPIRED` | puede reingresar código nuevo |
| `S03-ALREADY` | volver a home paciente |

## Rutas y filenames todavía no existentes en frontend

Estos paths se crean cuando `T04` defina la estructura base del routing paciente:

- `frontend/app/(patient)/vinculacion/codigo/page.tsx`
- `frontend/components/patient/vinculacion/CodeBindFlow.tsx`
- `frontend/lib/patient/vinculacion.ts`

## Backend todavía no existente

El endpoint `POST /api/v1/carelinks/bind` es futuro — contrato marcado como especulativo en `UI-RFC-VIN-002.md`. No debe asumirse runtime CareLink activo.

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa el flujo de vinculación usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

---

**Estado:** mapping listo para `frontend/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-VIN-002.md`.
