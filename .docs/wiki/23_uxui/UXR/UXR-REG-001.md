# UXR-REG-001 — Registro rápido de humor vía web

## Propósito

Este documento define la capa de investigación UX del slice `REG-001`.

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

- `../UXI/UXI-REG-001.md`
- `../UJ/UJ-REG-001.md`
- `../VOICE/VOICE-REG-001.md`
- `../UXS/UXS-REG-001.md`

## Caso objetivo y actor

- slice: `REG-001`
- caso: Registro rápido de humor vía web
- actor principal: paciente

## Problem statement

En este caso, registrar el humor pierde valor si el sistema pide más atención que la acción principal.

El valor del slice depende de que la persona suele llegar desde home, timeline o una CTA cercana y quiere resolver el registro en segundos.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-REG-01`

### RF vinculados

- `RF-REG-001`
- `RF-REG-002`
- `RF-REG-003`
- `RF-REG-004`
- `RF-REG-005`

### Test plans vinculados

- `TP-REG.md`

## Fricción actual

- la escala puede sentirse demasiado interpretada
- la confirmación puede demorar más que el gesto principal
- el copy puede sonar terapéutico o evaluativo

## Hypothesis

si el registro se resuelve con una escala visible, un gesto directo y una confirmación factual, la persona sentirá que Bitácora acompaña sin interrumpir.

## Señal observable de éxito

la persona registra un MoodEntry sin dudar si la acción se guardó ni sentir que inició un formulario.

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

**Estado:** `UXR` activo para `REG-001`.
**Siguiente capa gobernada:** `../UXI/UXI-REG-001.md`, `../UJ/UJ-REG-001.md`, `../VOICE/VOICE-REG-001.md` y `../UXS/UXS-REG-001.md`.
