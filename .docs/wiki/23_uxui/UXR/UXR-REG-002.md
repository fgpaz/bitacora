# UXR-REG-002 — Registro de factores diarios vía web

## Propósito

Este documento define la capa de investigación UX del slice `REG-002`.

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

- `../UXI/UXI-REG-002.md`
- `../UJ/UJ-REG-002.md`
- `../VOICE/VOICE-REG-002.md`
- `../UXS/UXS-REG-002.md`

## Caso objetivo y actor

- slice: `REG-002`
- caso: Registro de factores diarios vía web
- actor principal: paciente

## Problem statement

En este caso, el check-in diario pierde adopción si se vive como encuesta clínica larga.

El valor del slice depende de que la persona ya confía en el producto, pero no quiere invertir demasiada energía para dejar contexto sobre su día.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-REG-03`

### RF vinculados

- `RF-REG-020`
- `RF-REG-021`
- `RF-REG-022`
- `RF-REG-023`
- `RF-REG-024`
- `RF-REG-025`

### Test plans vinculados

- `TP-REG.md`

## Fricción actual

- demasiadas preguntas visibles a la vez
- ramas como medicación que aparecen sin orden
- submit final que se siente pesado para el valor que devuelve

## Hypothesis

si los factores se agrupan en bloques cortos y el guardado mantiene una sola dirección dominante, el check-in seguirá sintiéndose liviano.

## Señal observable de éxito

la persona completa el DailyCheckin sin sentir que rindió un cuestionario.

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

**Estado:** `UXR` activo para `REG-002`.
**Siguiente capa gobernada:** `../UXI/UXI-REG-002.md`, `../UJ/UJ-REG-002.md`, `../VOICE/VOICE-REG-002.md` y `../UXS/UXS-REG-002.md`.
