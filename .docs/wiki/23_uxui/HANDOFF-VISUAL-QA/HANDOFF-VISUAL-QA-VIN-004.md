# HANDOFF-VISUAL-QA-VIN-004 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `VIN-004`.

## Por qué el gate está activo

El gate visual está activo porque `VIN-004` combina:

- control granular de acceso sin jerga técnica;
- efecto del cambio que debe ser claro;
- resultado factual sin dramatización.

Una deriva visual aquí puede generar confusión sobre qué cambia realmente.

## Checkpoints obligatorios

### 1. Estado actual

- el efecto actual del vínculo se entiende sin tecnicismos;
- no se muestra `can_view_data` ni flags internos.

### 2. Cambio de estado

- el efecto del cambio se explica en una línea;
- el toggle o control no es opaco;
- no se introduce copy como `Habilitar visibilidad`.

### 3. Resultado guardado

- el nuevo estado se confirma de forma factual;
- no hay celebración ni dramatismo;
- la navegación post-cambio es directa.

## Drift inaceptable

- copy como `can_view_data`, `permisos del CareLink`, `habilitar visibilidad`;
- toggle sin explicación del efecto;
- confirmación tipo `Acceso actualizado con éxito`;
- más de una acción dominante por pantalla.

## Tolerancias aceptables

- pequeñas variaciones de espaciado que no cambien jerarquía;
- ajuste fino de densidad entre desktop y mobile;
- split interno de componentes sin cambiar la lectura del slice.

## Evidencia mínima de cierre

- captura desktop de `S01-DEFAULT`, `S02-DEFAULT`, `S03-SUCCESS` y `S03-INACTIVE`;
- captura mobile de `S01-DEFAULT`, `S02-DEFAULT` y `S03-SUCCESS`;
- evidencia de foco visible en el control de acceso y en la CTA.

## Validación real diferida

La evidencia mínima de cierre arriba es el gate documental. La validación UX real con personas reales queda diferida a `Phase 60`.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
