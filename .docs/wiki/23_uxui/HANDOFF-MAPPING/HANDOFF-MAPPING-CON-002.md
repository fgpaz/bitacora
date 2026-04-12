# HANDOFF-MAPPING-CON-002 — Correspondencia diseño-código

## Propósito

Este documento traduce `CON-002` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/(patient)/consentimiento/revocar/page.tsx` | entrada + orquestación del slice paciente |
| `frontend/components/patient/consentimiento/RevokeConsentFlow.tsx` | flujo completo de revocación |
| `frontend/components/patient/consentimiento/ConsentCurrentBlock.tsx` | estado actual del consentimiento |
| `frontend/components/patient/consentimiento/ImpactCascadeBlock.tsx` | enumeración del impacto en 2-3 ideas |
| `frontend/components/patient/consentimiento/RevokeConsentBar.tsx` | acción primaria de revocación |
| `frontend/components/patient/consentimiento/SecondaryActionRow.tsx` | acción de conservar |
| `frontend/components/patient/consentimiento/SuccessCascadeBlock.tsx` | confirmación factual |
| `frontend/lib/patient/consentimiento.ts` | llamadas a `DELETE /api/v1/consent` |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-ENTRY` | `ConsentCurrentBlock.tsx` | sin legalismo, estado legible |
| `S02-DEFAULT` | `ImpactCascadeBlock.tsx` + `RevokeConsentBar.tsx` + `SecondaryActionRow.tsx` | asimetría clara entre acciones |
| `S02-SUBMITTING` | `RevokeConsentBar.tsx` con estado `loading` | evita doble submit |
| `S03-SUCCESS` | `SuccessCascadeBlock.tsx` | factual, no punitivo |
| `S03-ALREADY` | estado actual sin repetir pantalla | navegación directa |
| `S03-ERROR` | inline error con reintento | recuperable |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `getCurrentConsent()` | `frontend/lib/patient/consentimiento.ts` | `GET /api/v1/consent/current` |
| `revokeConsent()` | `frontend/lib/patient/consentimiento.ts` | `DELETE /api/v1/consent` |

## Rutas y salidas

| Estado final | Navegación |
| --- | --- |
| `S03-SUCCESS` | volver a home paciente |
| `S03-ALREADY` | volver a home paciente |
| `S02-SUBMITTING` con `SESSION_EXPIRED` | reautenticación con continuidad |

## Rutas y filenames todavía no existentes en frontend

Estos paths se crean cuando `T04` defina la estructura base del routing paciente:

- `frontend/app/(patient)/consentimiento/revocar/page.tsx`
- `frontend/components/patient/consentimiento/RevokeConsentFlow.tsx`
- `frontend/lib/patient/consentimiento.ts`

## Backend todavía no existente

El endpoint `DELETE /api/v1/consent` es futuro — contrato marcado como especulativo en `UI-RFC-CON-002.md`. No debe asumirse runtime de consentimiento activo.

El detalle de la cascada a vínculos requiere coordinación con `T05` cuando existan endpoints CareLink.

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa el flujo de revocación usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

---

**Estado:** mapping listo para `frontend/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-CON-002.md`.
