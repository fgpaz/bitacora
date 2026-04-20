# HANDOFF-ASSETS-TG-001 — Inventario de assets

## Proposito

Este documento explicita que recursos visuales necesita `TG-001`.

## Assets especiales

`TG-001` no requiere assets binarios especiales obligatorios para abrir implementacion.

No hay:

- ilustraciones duesas del slice;
- fotografias;
- iconos custom exclusivos;
- motion assets exportados;
- archivos de diseno binarios que deban consumirse como autoridad.

## Dependencias visuales obligatorias

Aunque no haya assets especiales, el equipo de backend/telegram debe consumir estas dependencias canonicas:

- copy exacto de `../VOICE/VOICE-TG-001.md`;
- los estados conversacionales documentados en `UI-RFC-TG-001.md`;
- los copy aprobados tableados en `HANDOFF-SPEC-TG-001.md`;
- la convencion de keyboard inline definida en `TECH-TELEGRAM.md`.

## Recursos de contenido que sí deben prepararse

| Recurso | Fuente | Uso |
| --- | --- | --- |
| mensaje de exito de vinculacion | `VOICE-TG-001.md` | respuesta al usuario tras confirmacion |
| mensaje de codigo expirado | `VOICE-TG-001.md` | orientacion a regenerar en la web |
| mensaje de codigo invalido | `VOICE-TG-001.md` | orientacion al codigo correcto |
| mensaje de sesion ya vinculada | `VOICE-TG-001.md` | salida clara sin ambiguedad |
| mensaje de sin codigo | `VOICE-TG-001.md` | orientacion al flujo web |
| mensaje de no reconocido | `VOICE-TG-001.md` | recordatorio de formato |

## Runtime materializado

`TelegramSession`, `TelegramPairingCode` y el webhook `POST /api/v1/telegram/webhook` existen hoy en el runtime de `src/`. El slice tambien tiene puente web en `frontend/components/patient/telegram/TelegramPairingCard.tsx`.

## Regla operativa

Si durante implementacion aparece la necesidad de:

- una ilustracion nueva;
- un icono exclusivo;
- un asset exportado;
- un screenshot de referencia como fuente de verdad,

el pack debe reabrirse y actualizar este documento.

---

**Estado:** `sin assets especiales`, con copy y dependencias conversacionales explícitas.
**Siguiente artefacto:** `HANDOFF-MAPPING-TG-001.md`.
**Runtime Telegram:** implementado; evidencia E2E vigente en `artifacts/e2e/`.
