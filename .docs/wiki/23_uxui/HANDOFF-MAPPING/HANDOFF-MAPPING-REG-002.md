# HANDOFF-MAPPING-REG-002 — Correspondencia diseño-código

## Propósito

Este documento traduce `REG-002` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/(patient)/registro/daily-checkin/page.tsx` | entrada + orquestación del slice |
| `frontend/components/patient/registro/DailyCheckinFlow.tsx` | composición del slice y estados |
| `frontend/components/patient/registro/FactorBlock.tsx` | bloque de factor booleano reutilizable |
| `frontend/components/patient/registro/SleepHoursBlock.tsx` | bloque de input de horas de sueño |
| `frontend/components/patient/registro/MedicationBlock.tsx` | bloque condicional de medicación |
| `frontend/components/patient/registro/DailyCheckinSubmitBar.tsx` | barra final con CTA única |
| `frontend/components/patient/registro/InlineFeedbackMessage.tsx` | feedback de error o confirmación |
| `frontend/lib/patient/registro.ts` | llamadas a `POST /api/v1/daily-checkins` |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-ENTRY` | `DailyCheckinFlow.tsx` como composición raíz | muestra header y primer bloque |
| `S02-PARTIAL` | varios `FactorBlock` + `SleepHoursBlock` | recorrido progresivo por bloques |
| `S02-MEDICATION` | `MedicationBlock.tsx` | aparece solo cuando `medication_taken = true` |
| `S03-READY` | `DailyCheckinSubmitBar` con CTA habilitada | todos los campos requeridos completados |
| `S03-SUBMITTING` | estado `loading` en `DailyCheckinSubmitBar` | datos ya cargados preservados |
| `S03-SUCCESS` | `InlineFeedbackMessage` tipo `confirm` | inline, no pantalla adicional |
| `S03-ERROR` | `InlineFeedbackMessage` tipo `error` localizada | no borra los datos ya cargados |
| `S03-CONSENT` | vínculo o redirect a consentimiento | preserva todos los datos en estado local |
| `S03-SESSION` | prompt de reingreso | preserva todos los datos del formulario |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `createOrUpdateDailyCheckin(fields)` | `frontend/lib/patient/registro.ts` | `POST /api/v1/daily-checkins` |

**Body del request:**

```typescript
interface DailyCheckinPayload {
  sleep_hours: number;          // float 0-24
  physical_activity: boolean;
  social_activity: boolean;
  anxiety: boolean;
  irritability: boolean;
  medication_taken: boolean;
  medication_time?: string;      // "HH:MM" — solo si medication_taken = true
}
```

**Respuestas del backend:**

| HTTP | Significado | Acción UI |
| --- | --- | --- |
| `201` | primer check-in del día | `S03-SUCCESS` |
| `200` | actualización del registro del día | `S03-SUCCESS` |
| `422` | error de validación | mostrar error localized por campo |
| `403` | consentimiento faltante | `S03-CONSENT` |
| `401` | sesión expirada | `S03-SESSION` con datos preservados |
| `500` | error de servidor | `S03-ERROR` con reintento |

## Rutas y salidas

| Estado final | Navegación |
| --- | --- |
| `S03-SUCCESS` | volver a la vista anterior o a home del paciente |
| `S03-CONSENT` | ir a `frontend/app/(patient)/onboarding/consent/page.tsx` cuando exista |
| reingreso post-sesión | preservar todo el estado del formulario |

## Rutas y filenames todavía no existentes en frontend

Estos paths se crean cuando `T04` defina la estructura base del routing paciente:

- `frontend/app/(patient)/registro/daily-checkin/page.tsx`
- `frontend/components/patient/registro/DailyCheckinFlow.tsx`
- `frontend/lib/patient/registro.ts`

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa el flujo de check-in diario usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela;
- `FactorBlock` debe ser reutilizable si future slices requieren bloques de factores similares.

---

**Estado:** mapping listo para `frontend/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-REG-002.md`.
