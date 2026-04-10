# UXR-VIN-002 — Auto-vinculación paciente a profesional por código

## Propósito

Este documento define la capa de investigación UX del slice `VIN-002`.

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

- `../UXI/UXI-VIN-002.md`
- `../UJ/UJ-VIN-002.md`
- `../VOICE/VOICE-VIN-002.md`
- `../UXS/UXS-VIN-002.md`

## Caso objetivo y actor

- slice: `VIN-002`
- caso: Auto-vinculación paciente a profesional por código
- actor principal: paciente

## Problem statement

En este caso, pegar un código de vinculación puede sentirse opaco o riesgoso si no queda claro qué crea y qué no crea.

El valor del slice depende de que la persona ya tiene cuenta y consentimiento, y llega con un código que debería resolver el vínculo sin fricción.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-VIN-02`

### RF vinculados

- `RF-VIN-004`
- `RF-VIN-010`
- `RF-VIN-011`
- `RF-VIN-012`

### Test plans vinculados

- `TP-VIN.md`

## Fricción actual

- códigos expirados o inválidos sin explicación suficiente
- confusión entre vínculo y acceso a datos
- confirmaciones que no explican el estado resultante

## Hypothesis

si el ingreso del código es breve y el estado posterior aclara que el acceso sigue bajo control del paciente, la auto-vinculación se sentirá segura.

## Señal observable de éxito

la persona se vincula sin sentir que cedió acceso automáticamente.

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

**Estado:** `UXR` activo para `VIN-002`.
**Siguiente capa gobernada:** `../UXI/UXI-VIN-002.md`, `../UJ/UJ-VIN-002.md`, `../VOICE/VOICE-VIN-002.md` y `../UXS/UXS-VIN-002.md`.
