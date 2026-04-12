# HANDOFF-ASSETS-VIS-002 — Inventario de assets

## Propósito

Este documento explicita qué recursos visuales necesita `VIS-002`.

## Assets especiales

`VIS-002` no requiere assets binarios especiales obligatorios para abrir implementación.

No hay:

- ilustraciones dueñas del slice;
- fotografías;
- íconos custom exclusivos;
- motion assets exportados;
- archivos de diseño binarios que deban consumirse como autoridad.

## Dependencias visuales obligatorias

Frontend sí debe consumir estas dependencias canónicas:

- tipografías definidas en `../../11_identidad_visual.md`;
- tokens de color, superficie y foco del sistema;
- iconografía funcional sobria ya prevista por el sistema frontend;
- copy exacto de `../VOICE/VOICE-VIS-002.md`.

## Recursos de contenido que sí deben prepararse

| Recurso | Fuente | Uso |
| --- | --- | --- |
| copy de lista vacía | `VOICE-VIS-002.md` | `S04-EMPTY` — copy claro, no como error |
| copy de pacientes visibles | `VOICE-VIS-002.md` | tarjetas de resumen sobrias, no alarmistas |
| copy de alerta básica | `VOICE-VIS-002.md` | distingir de alerta dramática |
| copy de paginación | `VOICE-VIS-002.md` | cambio de página sin ruido |

## Regla operativa

Si durante implementación aparece la necesidad de:

- una ilustración nueva;
- un ícono exclusivo;
- un asset exportado;
- un screenshot de referencia como fuente de verdad,

el pack debe reabrirse y actualizar este documento.

---

**Estado:** `sin assets especiales`, con dependencias tipográficas, tokens y copy explícitas.
**Siguiente artefacto:** `HANDOFF-MAPPING-VIS-002.md`.
