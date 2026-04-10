# UXR-CON-002 — Revocación de consentimiento

## Propósito

Este documento define la capa de investigación UX del slice `CON-002`.

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

- `../UXI/UXI-CON-002.md`
- `../UJ/UJ-CON-002.md`
- `../VOICE/VOICE-CON-002.md`
- `../UXS/UXS-CON-002.md`

## Caso objetivo y actor

- slice: `CON-002`
- caso: Revocación de consentimiento
- actor principal: paciente

## Problem statement

En este caso, revocar consentimiento suspende registro y acceso profesional; si el impacto no es claro, la decisión se vuelve temerosa o accidental.

El valor del slice depende de que la persona está revisando un límite sensible y necesita entender cascadas reales sin caer en pánico ni burocracia.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-CON-02`

### RF vinculados

- `RF-CON-010`
- `RF-CON-011`
- `RF-CON-012`
- `RF-CON-013`

### Test plans vinculados

- `TP-CON.md`

## Fricción actual

- consecuencias múltiples difíciles de leer
- riesgo de confundir vínculo con consentimiento general
- tono legal o punitivo que rompe la confianza

## Hypothesis

si la revocación se explica con impacto concreto y una confirmación sobria, la persona podrá decidir desde control real.

## Señal observable de éxito

la persona entiende qué se suspende y qué puede reactivar más adelante.

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

**Estado:** `UXR` activo para `CON-002`.
**Siguiente capa gobernada:** `../UXI/UXI-CON-002.md`, `../UJ/UJ-CON-002.md`, `../VOICE/VOICE-CON-002.md` y `../UXS/UXS-CON-002.md`.
