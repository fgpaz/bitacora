# HANDOFF-ASSETS-REG-001 — Inventario de assets

## Propósito

Este documento explicita qué recursos visuales necesita `REG-001`.

## Assets especiales

`REG-001` no requiere assets binarios especiales obligatorios para abrir implementación.

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
- copy exacto de `../VOICE/VOICE-REG-001.md`.

## Recursos de contenido que sí deben prepararse

| Recurso | Fuente | Uso |
| --- | --- | --- |
| titular de escala | `VOICE-REG-001.md` → `UXS-REG-001.md` | `¿Cómo te sentís ahora?` en `S01-ENTRY` y `S02-DEFAULT` |
| texto de apoyo de escala | `VOICE-REG-001.md` → `UXS-REG-001.md` | `Elegí un valor y lo registramos enseguida.` |
| copy de error recuperable | `VOICE-REG-001.md` → `UXS-REG-001.md` | `No pudimos guardar este registro. Probá de nuevo.` |
| copy de redirrección a consent | `VOICE-REG-001.md` | mensaje breve de por qué necesita consentimiento |
| labels de escala | `-3`, `-2`, `-1`, `0`, `+1`, `+2`, `+3` | valores numéricos accesibles |

## Regla operativa

Si durante implementación aparece la necesidad de:

- una ilustración nueva;
- un ícono exclusivo para la escala;
- un asset exportado;
- un screenshot de referencia como fuente de verdad,

el pack debe reabrirse y actualizar este documento.

---

**Estado:** `sin assets especiales`, con dependencias tipográficas, tokens y copy explícitas.
**Siguiente artefacto:** `HANDOFF-MAPPING-REG-001.md`.
