# UXR-TG-001 — Vinculación de cuenta Telegram

## Propósito

Este documento define la capa de investigación UX del slice `TG-001`.

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

- `../UXI/UXI-TG-001.md`
- `../UJ/UJ-TG-001.md`
- `../VOICE/VOICE-TG-001.md`
- `../UXS/UXS-TG-001.md`

## Caso objetivo y actor

- slice: `TG-001`
- caso: Vinculación de cuenta Telegram
- actor principal: paciente

## Problem statement

En este caso, vincular Telegram puede volverse setup técnico si la web y el bot no se sienten como un mismo puente corto.

El valor del slice depende de que la persona ya usa la web y quiere habilitar el canal conversacional sin fricción.

## Señales ya presentes en el proyecto

### Flujos fuente

- `FL-TG-01`

### RF vinculados

- `RF-TG-001`
- `RF-TG-002`
- `RF-TG-003`

### Test plans vinculados

- `TP-TG.md`

## Fricción actual

- no saber qué hacer después de generar el código
- códigos expirados sin salida clara
- demasiadas instrucciones en una sola pantalla

## Hypothesis

si el flujo se divide en pasos secuenciales muy claros, la vinculación a Telegram se sentirá liviana.

## Señal observable de éxito

la persona genera el código, va al bot y completa la vinculación sin necesitar ayuda externa.

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

## Nota de estado runtime y validación

- `TelegramSession` existe en runtime (Phase 30+): entity, tabla, seam webhook y `ReminderWorker` materializados segun `TECH-TELEGRAM.md`
- la validacion UX real queda diferida a `Phase 60`
- `UI-RFC-TG-001.md` y `HANDOFF-SPEC-TG-001.md` actuan como contrato conversacional para implementacion backend/telegram
- este documento mantiene su valor como investigacion y no necesita modificacion de contenido

**Estado:** `UXR` activo para `TG-001`.
**Siguiente capa gobernada:** `../UXI/UXI-TG-001.md`, `../UJ/UJ-TG-001.md`, `../VOICE/VOICE-TG-001.md` y `../UXS/UXS-TG-001.md`.
