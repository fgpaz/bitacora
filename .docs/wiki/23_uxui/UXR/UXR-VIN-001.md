# UXR-VIN-001 — Emisión de invitación profesional a paciente

## Propósito

Este documento define la capa de investigación UX del slice `VIN-001`.

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

- `../UXI/UXI-VIN-001.md`
- `../UJ/UJ-VIN-001.md`
- `../VOICE/VOICE-VIN-001.md`
- `../UXS/UXS-VIN-001.md`

## Caso objetivo y actor

- slice: `VIN-001`
- caso: Emisión de invitación profesional a paciente
- actor principal: profesional

## Problem statement

En este caso, emitir una invitación puede leerse como alta automática o promesa de acceso clínico cuando en realidad solo abre un vínculo pendiente.

El valor del slice depende de que el profesional necesita invitar a una persona sin convertir la acción en un gesto de apropiación sobre sus datos.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-VIN-01`

### RF vinculados

- `RF-VIN-001`
- `RF-VIN-002`
- `RF-VIN-003`
- `RF-VIN-004`

### Test plans vinculados

- `TP-VIN.md`

## Fricción actual

- el estado posterior al envío puede ser ambiguo
- la UI puede sonar a alta de paciente estilo CRM
- el producto puede insinuar acceso automático a datos

## Hypothesis

si la invitación se presenta como solicitud responsable y el estado pendiente queda claro, el profesional actuará con la expectativa correcta.

## Señal observable de éxito

el profesional entiende que invitó, no que obtuvo acceso.

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

**Estado:** `UXR` activo para `VIN-001`.
**Siguiente capa gobernada:** `../UXI/UXI-VIN-001.md`, `../UJ/UJ-VIN-001.md`, `../VOICE/VOICE-VIN-001.md` y `../UXS/UXS-VIN-001.md`.
