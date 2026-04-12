# HANDOFF-ASSETS-VIN-002 — Inventario de assets

## Propósito

Este documento explicita qué recursos visuales necesita `VIN-002`.

## Assets especiales

`VIN-002` no requiere assets binarios especiales obligatorios para abrir implementación.

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
- copy exacto de `../VOICE/VOICE-VIN-002.md`.

## Recursos de contenido que sí deben prepararse

| Recurso | Fuente | Uso |
| --- | --- | --- |
| contexto breve antes del código | `VOICE-VIN-002.md` | `S01-DEFAULT` — contexto mínimo que no pide más datos |
| copy de código inválido | `VOICE-VIN-002.md` | recuperación digna sin tecnicismo |
| copy de código expirado | `VOICE-VIN-002.md` | salida clara cuando el código ya no sirve |
| copy de vínculo activo | `VOICE-VIN-002.md` | explica acceso todavía desactivado |

## Regla operativa

Si durante implementación aparece la necesidad de:

- una ilustración nueva;
- un ícono exclusivo;
- un asset exportado;
- un screenshot de referencia como fuente de verdad,

el pack debe reabrirse y actualizar este documento.

---

**Estado:** `sin assets especiales`, con dependencias tipográficas, tokens y copy explícitas.
**Siguiente artefacto:** `HANDOFF-MAPPING-VIN-002.md`.
