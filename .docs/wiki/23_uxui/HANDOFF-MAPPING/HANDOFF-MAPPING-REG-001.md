# HANDOFF-MAPPING-REG-001 — Correspondencia diseño-código

## Propósito

Este documento traduce `REG-001` a ownership técnico concreto para `frontend/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `frontend/app/(patient)/registro/mood-entry/page.tsx` | entrada + orquestación del slice |
| `frontend/components/patient/registro/MoodEntryFlow.tsx` | composición del slice y estados |
| `frontend/components/patient/registro/MoodScale.tsx` | escala -3..+3 con selección inmediata |
| `frontend/components/patient/registro/MoodEntrySubmitButton.tsx` | CTA de guardado |
| `frontend/components/patient/registro/InlineFeedbackMessage.tsx` | feedback de error o confirmación |
| `frontend/lib/patient/registro.ts` | llamadas a `POST /api/v1/mood-entries` |

## Bloques visuales y destino sugerido

| Bloque UX/UI | Implementación sugerida | Nota |
| --- | --- | --- |
| `S01-ENTRY` | `MoodEntryFlow.tsx` como composición raíz | muestra escala y header mínimo |
| `S02-DEFAULT` | `MoodScale.tsx` | 7 valores, selectable, sin envío automático |
| `S02-SUBMITTING` | `MoodScale.tsx` + estado `loading` en `MoodEntrySubmitButton` | valor ya seleccionado visible |
| `S02-SUCCESS` | `InlineFeedbackMessage.tsx` tipo `confirm` | no requiere pantalla adicional |
| `S02-ERROR` | `InlineFeedbackMessage.tsx` tipo `error` + `MoodEntrySubmitButton` con reintento | preserva el valor seleccionado |
| `S02-CONSENT` | vínculo o redirect a flujo de consentimiento | preserva el valor seleccionado en estado local |
| `S02-SESSION` | `SessionRecoverPrompt` con reingreso | preserva el valor seleccionado en estado local |

## Capa de datos

| Acción | Archivo sugerido | Contrato |
| --- | --- | --- |
| `createMoodEntry(score: number)` | `frontend/lib/patient/registro.ts` | `POST /api/v1/mood-entries` body: `{ "score": integer }` |

## Rutas y salidas

| Estado final | Navegación |
| --- | --- |
| `S02-SUCCESS` | volver a la vista anterior o a home del paciente |
| `S02-CONSENT` | ir a `frontend/app/(patient)/onboarding/consent/page.tsx` cuando exista |
| reingreso post-sesión | preservar el estado local del valor ya seleccionado |

## Rutas y filenames todavía no existentes en frontend

Estos paths se crean cuando `T04` defina la estructura base del routing paciente:

- `frontend/app/(patient)/registro/mood-entry/page.tsx`
- `frontend/components/patient/registro/MoodEntryFlow.tsx`
- `frontend/lib/patient/registro.ts`

## Regla de ownership

- `T04` puede crear shell y bases compartidas, pero no debe redefinir el slice;
- `T05` implementa el flujo de registro rápido usando este mapping;
- si el código necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

---

**Estado:** mapping listo para `frontend/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-REG-001.md`.
