# HANDOFF-ASSETS-ONB-001 — Inventario de assets

## Propósito

Este documento explicita qué recursos visuales necesita `ONB-001`.

## Assets especiales

`ONB-001` no requiere assets binarios especiales obligatorios para abrir implementación.

No hay:

- ilustraciones dueñas del slice;
- fotografías;
- íconos custom exclusivos;
- motion assets exportados;
- archivos de diseño binarios que deban consumirse como autoridad.

## Dependencias visuales obligatorias

Aunque no haya assets especiales, frontend sí debe consumir estas dependencias canónicas:

- tipografías definidas en `../../11_identidad_visual.md`;
- tokens de color, superficie y foco del sistema;
- iconografía funcional sobria ya prevista por el sistema frontend;
- copy exacto de `../VOICE/VOICE-ONB-001.md`.

## Recursos de contenido que sí deben prepararse

| Recurso | Fuente | Uso |
| --- | --- | --- |
| headline estándar de portada | `VOICE-ONB-001.md` | hero `S01-HERO-STANDARD` |
| headline/contexto invitado | `VOICE-ONB-001.md` | hero `S01-HERO-INVITE` |
| texto fallback invitado | `VOICE-ONB-001.md` | hero `S01-HERO-INVITE-FALLBACK` |
| microcopy interstitial | `VOICE-ONB-001.md` | `S02-AUTH-INTERSTITIAL` |
| consentimiento vigente | backend runtime | `S03-CONSENT-*` |
| CTA bridge final | `VOICE-ONB-001.md` | `S04-BRIDGE` |

## Regla operativa

Si durante implementación aparece la necesidad de:

- una ilustración nueva;
- un ícono exclusivo;
- un asset exportado;
- un screenshot de referencia como fuente de verdad,

el pack debe reabrirse y actualizar este documento.

---

**Estado:** `sin assets especiales`, con dependencias tipográficas, tokens y copy explícitas.
**Siguiente artefacto:** `HANDOFF-MAPPING-ONB-001.md`.
