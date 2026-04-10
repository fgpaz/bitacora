# UXR-EXP-001 — Exportación CSV del paciente

## Propósito

Este documento define la capa de investigación UX del slice `EXP-001`.

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

- `../UXI/UXI-EXP-001.md`
- `../UJ/UJ-EXP-001.md`
- `../VOICE/VOICE-EXP-001.md`
- `../UXS/UXS-EXP-001.md`

## Caso objetivo y actor

- slice: `EXP-001`
- caso: Exportación CSV del paciente
- actor principal: paciente

## Problem statement

En este caso, la exportación puede sentirse opaca o demasiado técnica si no queda claro qué datos salen y cómo empieza la descarga.

El valor del slice depende de que la persona quiere ejercer portabilidad o revisar sus datos fuera del sistema sin fricción ni miedo.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-EXP-01`

### RF vinculados

- `RF-EXP-001`
- `RF-EXP-002`
- `RF-EXP-003`

### Test plans vinculados

- `TP-EXP.md`

## Fricción actual

- dudas sobre alcance del archivo
- descarga que parece no arrancar o queda silenciosa
- lenguaje demasiado técnico sobre CSV o cifrado

## Hypothesis

si la exportación se presenta como salida simple y controlada, la portabilidad se sentirá parte natural del producto.

## Señal observable de éxito

la persona entiende qué va a descargar y reconoce cuándo la descarga empezó.

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

**Estado:** `UXR` activo para `EXP-001`.
**Siguiente capa gobernada:** `../UXI/UXI-EXP-001.md`, `../UJ/UJ-EXP-001.md`, `../VOICE/VOICE-EXP-001.md` y `../UXS/UXS-EXP-001.md`.
