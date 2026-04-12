# HANDOFF-ASSETS-TG-002 — Inventario de assets

## Proposito

Este documento explicita que recursos visuales necesita `TG-002`.

## Assets especiales

`TG-002` no requiere assets binarios especiales obligatorios para abrir implementacion.

No hay:

- ilustraciones duesas del slice;
- fotografias;
- iconos custom exclusivos;
- motion assets exportados;
- archivos de diseno binarios que deban consumirse como autoridad.

## Dependencias visuales obligatorias

Aunque no haya assets especiales, el equipo de backend/telegram debe consumir estas dependencias canonicas:

- copy exacto de `../VOICE/VOICE-TG-002.md`;
- los estados conversacionales documentados en `UI-RFC-TG-002.md`;
- los copy aprobados tableados en `HANDOFF-SPEC-TG-002.md`;
- la convencion de keyboard inline (maximo 8 opciones por fila) definida en `TECH-TELEGRAM.md`;
- el phrasing prohibido listado en `UI-RFC-TG-002.md`.

## Recursos de contenido que sí deben prepararse

| Recurso | Fuente | Uso |
| --- | --- | --- |
| pregunta de humor | `VOICE-TG-002.md` | `S01` — trigger del recordatorio |
| keyboard de escala | `-3 .. +3` + `Ahora no` | keyboard inline |
| confirmacion de registro | `VOICE-TG-002.md` | `Registrado: +1.` — factual, no celebratorio |
| pregunta opcional de factor | `VOICE-TG-002.md` | continuacion con `Ahora no` visible |
| mensaje de cierre | `VOICE-TG-002.md` | `Buen dia` — cierre breve |
| mensaje de error | `VOICE-TG-002.md` | reintento sin culpa |
| mensaje de no reconocido | `VOICE-TG-002.md` | recordatorio de formato |

## Ausencia de runtime

`TelegramSession`, `ReminderConfig`, el webhook y el background service no existen hoy en el runtime de `src/`. Esta documentacion opera como especificacion objetivo; la materializacion real sera cuando el modulo Telegram exista en runtime.

## Regla operativa

Si durante implementacion aparece la necesidad de:

- una ilustracion nueva;
- un icono exclusivo;
- un asset exportado;
- un screenshot de referencia como fuente de verdad,

el pack debe reabrirse y actualizar este documento.

---

**Estado:** `sin assets especiales`, con copy y dependencias conversacionales explícitas.
**Siguiente artefacto:** `HANDOFF-MAPPING-TG-002.md`.
**Runtime Telegram:** diferido; no existe hoy en `src/`.
