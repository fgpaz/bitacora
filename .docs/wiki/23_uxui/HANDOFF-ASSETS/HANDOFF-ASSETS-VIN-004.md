# HANDOFF-ASSETS-VIN-004 — Inventario de assets

## Propósito

Este documento explicita qué recursos visuales necesita `VIN-004`.

## Assets especiales

`VIN-004` no requiere assets binarios especiales obligatorios para abrir implementación.

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
- copy exacto de `../VOICE/VOICE-VIN-004.md`.

## Recursos de contenido que sí deben prepararse

| Recurso | Fuente | Uso |
| --- | --- | --- |
| estado actual legible | `VOICE-VIN-004.md` | `S01-DEFAULT` — efecto del vínculo sin tecnicismo |
| efecto del cambio | `VOICE-VIN-004.md` | `S02-DEFAULT` — qué cambia cuando se activa o desactiva |
| confirmación de guardado | `VOICE-VIN-004.md` | `S03-SUCCESS` — resultado factual sin dramatismo |
| error recuperable | `VOICE-VIN-004.md` | inline feedback con reintento |

## Regla operativa

Si durante implementación aparece la necesidad de:

- una ilustración nueva;
- un ícono exclusivo;
- un asset exportado;
- un screenshot de referencia como fuente de verdad,

el pack debe reabrirse y actualizar este documento.

---

**Estado:** `sin assets especiales`, con dependencias tipográficas, tokens y copy explícitas.
**Siguiente artefacto:** `HANDOFF-MAPPING-VIN-004.md`.
