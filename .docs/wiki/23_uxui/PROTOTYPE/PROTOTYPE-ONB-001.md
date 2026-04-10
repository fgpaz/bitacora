# PROTOTYPE-ONB-001 — Prototipo del ONB-first del paciente

## Propósito

Este documento define el prototipo del slice `ONB-001` después del desbloqueo `ONB-first`.

No declara validación UX ni reemplaza `UXS` o `UI-RFC`. Su función es dejar explícito qué estados visibles debe cubrir el pack y qué autoridad documental manda mientras el HTML local existente queda como referencia archivada pre-override.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-ONB-001.md`
- `../UXI/UXI-ONB-001.md`
- `../UJ/UJ-ONB-001.md`
- `../VOICE/VOICE-ONB-001.md`
- `../UXS/UXS-ONB-001.md`
- `../UI-RFC/UI-RFC-ONB-001.md`

Y prepara directamente:

- `../UX-VALIDATION/UX-VALIDATION-ONB-001.md`
- la cadena `HANDOFF-*`

## Slice cubierto

### Caso

`ONB-001`: entrada `ONB-first` del paciente hasta consentimiento y puente al primer registro.

### Cobertura del prototipo

Este prototipo cubre:

- portada estándar;
- hero adaptado por invitación;
- fallback genérico de invitación;
- interstitial de auth/bootstrap;
- consentimiento base;
- recordatorio contextual de invitación;
- fricción principal por invitación/contexto;
- conflicto de versión;
- confirmación + puente.

No cubre:

- el formulario del primer `MoodEntry`;
- daily check-in;
- otras rutas de paciente o profesional.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad` y cubrir el mismo state pack en:

- desktop;
- mobile.

No alcanza:

- un solo breakpoint;
- happy path sin fricciones;
- HTML heredado que contradiga `UXS/UI-RFC`.

## Autoridad visual vigente

### Autoridad actual

Para este slice, la autoridad vigente de T02 ya no es el HTML local heredado ni el gate Stitch previo.

La autoridad documental operativa queda en:

1. `UXS-ONB-001.md`
2. `UI-RFC-ONB-001.md`
3. la cadena `HANDOFF-*`

### Estado del HTML existente

- `./PROTOTYPE-ONB-001.html` se conserva como referencia archivada pre-override;
- puede servir como wrapper navegable histórico;
- no debe usarse como fuente de verdad para copy, jerarquía o estados si contradice el pack nuevo.

## Inventario mínimo de frames

Cada frame obligatorio debe existir en desktop y mobile.

| Frame ID | Qué muestra | Obligatorio |
| --- | --- | --- |
| `ONB-001-S01-HERO-STANDARD` | portada estándar con `Empezar ahora` | sí |
| `ONB-001-S01-HERO-INVITE` | hero adaptado por invitación con vínculo + propósito explícitos | sí |
| `ONB-001-S01-HERO-INVITE-FALLBACK` | hero adaptado genérico por falta de datos | sí |
| `ONB-001-S01-INVITE-EXPIRED` | salida clara para contexto invitado inválido o vencido | sí |
| `ONB-001-S02-INTERSTITIAL` | retorno breve de auth/bootstrap | sí |
| `ONB-001-S03-CONSENT-DEFAULT` | consentimiento base con resumen de control | sí |
| `ONB-001-S03-CONSENT-REMINDER` | consentimiento con recordatorio ligero del contexto invitado | sí |
| `ONB-001-S03-CONSENT-CONTEXT` | resolución de confusión por invitación/contexto | sí |
| `ONB-001-S03-VERSION-CONFLICT` | conflicto de versión vigente | sí |
| `ONB-001-S03-ERROR` | error recuperable de submit | sí |
| `ONB-001-S04-BRIDGE` | confirmación con CTA `Hacer mi primer registro` | sí |

## Hipótesis que el prototipo debe permitir observar

1. La portada estándar se entiende como guía personal.
2. La portada invitada baja la confusión sin sentirse como otro producto.
3. El interstitial mantiene continuidad y no agrega carga técnica.
4. El consentimiento se percibe como `resguardo claro`.
5. La confirmación deja una siguiente acción evidente.

## Reglas de construcción

- respetar `VOICE-ONB-001.md`;
- respetar `UXS-ONB-001.md`;
- sostener una sola acción dominante por estado;
- no recuperar copy poético ni celebratorio del material anterior;
- no introducir una experiencia separada para la invitación.

## Criterio de readiness

El prototipo está listo para validación/handoff si:

1. el state pack completo es visible en desktop y mobile;
2. el material ya no depende del HTML legado para explicar jerarquía o copy;
3. los estados sensibles son consistentes entre `UXS`, `UI-RFC` y handoff;
4. el bridge final deja claro que el siguiente paso es el primer registro, no otra pantalla del mismo slice.

---

**Estado:** prototipo redefinido para `ONB-001` con autoridad manual del pack `UXS/UI-RFC/HANDOFF`.
**Precedencia:** depende de `UXS-ONB-001.md` y `UI-RFC-ONB-001.md`.
**Siguiente capa gobernada:** `UX-VALIDATION-ONB-001.md` y la cadena `HANDOFF-*`.
