# UXR-TG-002 — Recordatorio y registro conversacional por Telegram

## Propósito

Este documento define la capa de investigación UX del slice `TG-002`.

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

- `../UXI/UXI-TG-002.md`
- `../UJ/UJ-TG-002.md`
- `../VOICE/VOICE-TG-002.md`
- `../UXS/UXS-TG-002.md`

## Caso objetivo y actor

- slice: `TG-002`
- caso: Recordatorio y registro conversacional por Telegram
- actor principal: paciente

## Problem statement

En este caso, un recordatorio conversacional puede sentirse insistente o culposo si el mensaje no preserva ligereza y opcionalidad.

El valor del slice depende de que la persona no abrió la app: el sistema la busca en Telegram y debe hacerlo con mucho tacto.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-TG-02`
- `FL-REG-02`

### RF vinculados

- `RF-TG-010`
- `RF-TG-011`
- `RF-TG-012`
- `RF-REG-010`
- `RF-REG-011`
- `RF-REG-012`
- `RF-REG-013`
- `RF-REG-014`
- `RF-REG-015`

### Test plans vinculados

- `TP-TG.md`
- `TP-REG.md`

## Fricción actual

- mensajes que suenan demandantes
- falta de salida fácil si la persona no quiere responder ahora
- transición torpe entre recordatorio y primer tap de registro

## Hypothesis

si el recordatorio es breve, opcional y lleva directo al gesto principal, el canal conversacional se sentirá útil y no invasivo.

## Señal observable de éxito

la persona puede responder al recordatorio sin sentir presión y el registro ocurre con continuidad.

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

**Estado:** `UXR` activo para `TG-002`.
**Siguiente capa gobernada:** `../UXI/UXI-TG-002.md`, `../UJ/UJ-TG-002.md`, `../VOICE/VOICE-TG-002.md` y `../UXS/UXS-TG-002.md`.
