# HANDOFF-MAPPING-VIN-001 — Correspondencia diseño-código

## Propósito

Este documento traduce `VIN-001` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/(professional)/vinculacion/invitar/page.tsx` | entrada + orquestación del slice profesional |
| `frontend/components/professional/vinculacion/InviteFormBlock.tsx` | formulario con email y alcance |
| `frontend/components/professional/vinculacion/EmailInput.tsx` | campo de email con validación local |
| `frontend/components/professional/vinculacion/ScopeReminderInline.tsx` | recordatorio del alcance |
| `frontend/components/professional/vinculacion/PendingStatusCard.tsx` | estado pendiente post-envío |
| `frontend/components/professional/vinculacion/ConflictResolutionCard.tsx` | carta de conflicto |
| `frontend/lib/professional/vinculacion.ts` | llamadas a `POST /api/v1/carelinks/invite` |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-DEFAULT` | `InviteFormBlock.tsx` | campo de email y alcance visible |
| `S01-READY` | `InviteFormBlock.tsx` + estado `ready` en CTA | email válido habilita `Enviar invitación` |
| `S01-SUBMITTING` | `InviteFormBlock.tsx` + estado `loading` | evita doble envío |
| `S02-SUCCESS` | `PendingStatusCard.tsx` | estado pendiente, sin drama |
| `S03-CONFLICT` | `ConflictResolutionCard.tsx` | vínculo o invitación existente |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `invitePatient(email)` | `frontend/lib/professional/vinculacion.ts` | `POST /api/v1/carelinks/invite` body: `{ "patientEmail": string }` |

## Rutas y salidas

| Estado final | Navegación |
| --- | --- |
| `S02-SUCCESS` | volver a home profesional o a la lista de vínculos |
| `S03-CONFLICT` | resolver desde la misma carta sin repetir acción de invitar |

## Rutas y filenames todavía no existentes en frontend

Estos paths se crean cuando `T04` defina la estructura base del routing profesional:

- `frontend/app/(professional)/vinculacion/invitar/page.tsx`
- `frontend/components/professional/vinculacion/InviteFormBlock.tsx`
- `frontend/lib/professional/vinculacion.ts`

## Backend todavía no existente

El endpoint `POST /api/v1/carelinks/invite` es futuro — contrato marcado como especulativo en `UI-RFC-VIN-001.md`. No debe asumirse runtime CareLink activo.

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa el flujo de invitación usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

---

**Estado:** mapping listo para `frontend/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-VIN-001.md`.
