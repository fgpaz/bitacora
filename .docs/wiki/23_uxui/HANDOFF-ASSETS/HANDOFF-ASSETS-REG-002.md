# HANDOFF-ASSETS-REG-002 — Inventario de assets

## Propósito

Este documento explicita qué recursos visuales necesita `REG-002`.

## Assets especiales

`REG-002` no requiere assets binarios especiales obligatorios para abrir implementación.

No hay:

- ilustraciones dueñas del slice;
- fotografías;
- íconos custom exclusivos para los bloques;
- motion assets exportados;
- archivos de diseño binarios que deban consumirse como autoridad.

## Dependencias visuales obligatorias

Frontend sí debe consumir estas dependencias canónicas:

- tipografías definidas en `../../11_identidad_visual.md`;
- tokens de color, superficie y foco del sistema;
- iconografía funcional sobria ya prevista por el sistema frontend;
- copy exacto de `../VOICE/VOICE-REG-002.md`.

## Recursos de contenido que sí deben prepararse

| Recurso | Fuente | Uso |
| --- | --- | --- |
| headline de check-in | `VOICE-REG-002.md` → `UXS-REG-002.md` | `Completá tu check-in de hoy` en `S01-ENTRY` |
| texto de apoyo del formulario | `VOICE-REG-002.md` → `UXS-REG-002.md` | `Solo pedimos lo necesario para darle contexto a tus registros.` |
| copy de error recuperable | `VOICE-REG-002.md` → `UXS-REG-002.md` | `No pudimos guardar este check-in. Probá de nuevo.` |
| labels de factores bool | `Sí` / `No` o equivalente | cada `FactorBlock` booleano |
| label de input de sueño | `horas` o `hs` | bloque de `sleep_hours` |
| label de horario de medicación | `Horário aproximado` | `MedicationBlock` |

## Regla operativa

Si durante implementación aparece la necesidad de:

- una ilustración para alguno de los bloques;
- un ícono específico para el bloque de medicación;
- un asset exportado;
- un screenshot de referencia como fuente de verdad,

el pack debe reabrirse y actualizar este documento.

---

**Estado:** `sin assets especiales`, con dependencias tipográficas, tokens y copy explícitas.
**Siguiente artefacto:** `HANDOFF-MAPPING-REG-002.md`.
