# UXR-VIN-004 — Gestión de acceso profesional por paciente

## Propósito

Este documento define la capa de investigación UX del slice `VIN-004`.

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

- `../UXI/UXI-VIN-004.md`
- `../UJ/UJ-VIN-004.md`
- `../VOICE/VOICE-VIN-004.md`
- `../UXS/UXS-VIN-004.md`

## Caso objetivo y actor

- slice: `VIN-004`
- caso: Gestión de acceso profesional por paciente
- actor principal: paciente

## Problem statement

En este caso, un switch de acceso puede volverse técnico u opaco si no deja claro qué cambia en la visibilidad del profesional.

El valor del slice depende de que la persona ya está vinculada y necesita controlar granularmente quién puede ver sus registros.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-VIN-04`

### RF vinculados

- `RF-VIN-022`
- `RF-VIN-023`

### Test plans vinculados

- `TP-VIN.md`

## Fricción actual

- el cambio puede parecer técnico en lugar de relacional
- la consecuencia puede no ser evidente
- el estado guardado puede no diferenciarse del estado pendiente

## Hypothesis

si la acción muestra efecto concreto y reversible en lenguaje simple, el control granular se sentirá propio.

## Señal observable de éxito

la persona entiende qué profesional ve qué y puede cambiarlo sin miedo.

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

**Estado:** `UXR` activo para `VIN-004`.
**Siguiente capa gobernada:** `../UXI/UXI-VIN-004.md`, `../UJ/UJ-VIN-004.md`, `../VOICE/VOICE-VIN-004.md` y `../UXS/UXS-VIN-004.md`.
