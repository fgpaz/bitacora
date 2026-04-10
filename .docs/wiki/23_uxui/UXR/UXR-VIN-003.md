# UXR-VIN-003 — Revocación de vínculo por paciente

## Propósito

Este documento define la capa de investigación UX del slice `VIN-003`.

No describe todavía layout ni comportamiento fino. Su función es fijar el problema humano, el contexto de uso, las señales del repo y la hipótesis que deben heredar `UXI`, `UJ`, `VOICE` y `UXS`.

## Relación con el canon

Este documento depende de:

- `../../10_manifiesto_marca_experiencia.md`
- `../../11_identidad_visual.md`
- `../../12_lineamientos_interfaz_visual.md`
- `../../13_voz_tono.md`
- `../../14_metodo_prototipado_validacion_ux.md`
- `../../15_handoff_operacional_uxui.md`

Y prepara directamente:

- `../UXI/UXI-VIN-003.md`
- `../UJ/UJ-VIN-003.md`
- `../VOICE/VOICE-VIN-003.md`
- `../UXS/UXS-VIN-003.md`

## Caso objetivo y actor

- slice: `VIN-003`
- caso: Revocación de vínculo por paciente
- actor principal: paciente

## Problem statement

En este caso, cortar un vínculo con un profesional puede sentirse punitivo o demasiado ambiguo si no se explica bien el impacto inmediato.

El valor del slice depende de que la persona llega porque necesita recuperar control o cerrar una relación sin revocar todo el consentimiento.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-VIN-03`

### RF vinculados

- `RF-VIN-020`
- `RF-VIN-021`
- `RF-VIN-022`

### Test plans vinculados

- `TP-VIN.md`

## Fricción actual

- el impacto puede sonar más dramático que claro
- la revocación puede confundirse con revocar consentimiento
- la confirmación puede dejar dudas sobre cuándo deja de haber acceso

## Hypothesis

si la revocación se presenta como decisión serena y el efecto inmediato queda visible, el control se sentirá claro en lugar de amenazante.

## Señal observable de éxito

la persona revoca el vínculo con certeza y sin culpa.

## Boundaries and defaults

- el slice se documenta con ownership visible y una sola tarea dominante;
- hereda como defaults: casi nula fricción, seguridad implícita, simpleza radical, paciente primero cuando el actor principal es la persona usuaria, silencio útil fuera de los momentos sensibles, explicitud alta solo cuando cambian acceso, datos o consentimiento;
- no debe mezclar decisiones de casos adyacentes si el flujo fuente no las exige.

## Criterio de validación rápida

Este `UXR` está bien planteado si:

- describe un problema humano antes que una pantalla;
- deja claro quién es el actor principal;
- usa señales reales de `FL`, `RF` y `TP`;
- deja preparada la cadena `UXI -> UJ -> VOICE -> UXS` sin inventar más adelante el conflicto central.

---

**Estado:** `UXR` activo para `VIN-003`.
**Siguiente capa gobernada:** `../UXI/UXI-VIN-003.md`, `../UJ/UJ-VIN-003.md`, `../VOICE/VOICE-VIN-003.md` y `../UXS/UXS-VIN-003.md`.
