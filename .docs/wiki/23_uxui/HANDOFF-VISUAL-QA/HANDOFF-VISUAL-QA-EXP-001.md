# HANDOFF-VISUAL-QA-EXP-001 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `EXP-001`.

## Por qué el gate está activo

El gate visual está activo porque `EXP-001` combina:

- descarga que debe sentirse como derecho propio y no como opcion técnica;
- feedback de preparación que sostiene certeza;
- resultado que cierra sin pantalla extra.

Una deriva visual aquí puede alterar la percepción del dato propio como accesible.

## Checkpoints obligatorios

### 1. Entrada y alcance

- el alcance del archivo se entiende antes de disparar la descarga;
- no aparece jerga técnica como `payloads` o `compliance data export`;
- el selector de período no agrega fricción innecesaria.

### 2. Preparación

- el feedback de generación sostiene certeza tranquila;
- no se Entierro detrás de un spinner genérico;
- no se muestra porcentaje ni detalle técnico.

### 3. Descarga

- la descarga arranca sin pantalla extra;
- el cierre es directo y no hay confirmación posterior;
- se distingue de una generación fallida.

### 4. Error

- el error recuperable propone reintento claro;
- no se confunde con un problema del archivo;
- el mensaje es breve y localized.

## Drift inaceptable

- copy como `payloads`, `descifrado por versión de clave`, `compliance data export`;
- pantalla de confirmación post-descarga;
- barra de progreso técnica;
- exportación presentada como herramienta administrativa.

## Tolerancias aceptables

- pequeñas variaciones de espaciado que no cambien jerarquía;
- ajuste fino de densidad entre desktop y mobile;
- split interno de componentes sin cambiar la lectura del slice.

## Evidencia mínima de cierre

- captura desktop de `S01-DEFAULT`, `S01-PERIOD`, `S02-GENERATING` y `S02-SUCCESS`;
- captura mobile de `S01-DEFAULT` y `S02-GENERATING`;
- evidencia de que la descarga arranca sin pantalla extra.

## Validación real diferida

La evidencia mínima de cierre arriba es el gate documental. La validación UX real con personas reales queda diferida a `Phase 60`.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
