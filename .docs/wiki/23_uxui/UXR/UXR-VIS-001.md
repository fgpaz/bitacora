# UXR-VIS-001 — Timeline longitudinal del paciente

## Propósito

Este documento define la capa de investigación UX del slice `VIS-001`.

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

- `../UXI/UXI-VIS-001.md`
- `../UJ/UJ-VIS-001.md`
- `../VOICE/VOICE-VIS-001.md`
- `../UXS/UXS-VIS-001.md`

## Caso objetivo y actor

- slice: `VIS-001`
- caso: Timeline longitudinal del paciente
- actor principal: paciente

## Problem statement

En este caso, un gráfico longitudinal pierde valor si abruma o se vuelve demasiado analítico para una lectura cotidiana.

El valor del slice depende de que la persona llega para entender su propio patrón, no para interpretar una herramienta técnica.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-VIS-01`

### RF vinculados

- `RF-VIS-001`
- `RF-VIS-002`
- `RF-VIS-003`

### Test plans vinculados

- `TP-VIS.md`

## Fricción actual

- gráfico frío sin guía mínima
- filtros que interrumpen antes de mostrar valor
- estados vacíos que parecen error en lugar de ausencia de datos

## Hypothesis

si el timeline abre con lectura simple y filtros base claros, la persona lo sentirá como una extensión útil de sus registros.

## Señal observable de éxito

la persona entiende qué está mirando y puede cambiar período sin perderse.

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

**Estado:** `UXR` activo para `VIS-001`.
**Siguiente capa gobernada:** `../UXI/UXI-VIS-001.md`, `../UJ/UJ-VIS-001.md`, `../VOICE/VOICE-VIS-001.md` y `../UXS/UXS-VIS-001.md`.
