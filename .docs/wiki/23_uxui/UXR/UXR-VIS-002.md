# UXR-VIS-002 — Dashboard multi-paciente del profesional

## Propósito

Este documento define la capa de investigación UX del slice `VIS-002`.

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

- `../UXI/UXI-VIS-002.md`
- `../UJ/UJ-VIS-002.md`
- `../VOICE/VOICE-VIS-002.md`
- `../UXS/UXS-VIS-002.md`

## Caso objetivo y actor

- slice: `VIS-002`
- caso: Dashboard multi-paciente del profesional
- actor principal: profesional

## Problem statement

En este caso, un dashboard multi-paciente puede derivar en sensación de vigilancia si no acota bien visibilidad, prioridad y tono.

El valor del slice depende de que el profesional necesita una lectura resumida de pacientes con acceso habilitado, no un panel de control sobre personas.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-VIS-02`

### RF vinculados

- `RF-VIS-010`
- `RF-VIS-011`
- `RF-VIS-012`
- `RF-VIS-013`
- `RF-VIS-014`

### Test plans vinculados

- `TP-VIS.md`
- `TP-SEC.md`

## Fricción actual

- sobrecarga visual al listar muchos pacientes
- alertas que suenan dramáticas
- falta de señal clara de que solo se ve lo habilitado

## Hypothesis

si el tablero prioriza claridad, filtros sobrios y límites explícitos de acceso, la experiencia será profesional sin volverse invasiva.

## Señal observable de éxito

el profesional entiende qué puede ver y dónde enfocar sin sentir acceso irrestricto.

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

**Estado:** `UXR` activo para `VIS-002`.
**Siguiente capa gobernada:** `../UXI/UXI-VIS-002.md`, `../UJ/UJ-VIS-002.md`, `../VOICE/VOICE-VIS-002.md` y `../UXS/UXS-VIS-002.md`.
