# HANDOFF-VISUAL-QA-CON-002 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `CON-002`.

## Por qué el gate está activo

El gate visual está activo porque `CON-002` combina:

- revocación que puede sentirse como pérdida de control;
- impacto en cascada que debe ser concreto;
- acción secundaria que no debe competir con la primaria.

Una deriva visual aquí puede generar desconfianza o arrepentimiento post-revocación.

## Checkpoints obligatorios

### 1. Estado actual

- el consentimiento vigente se presenta sin legalismo;
- es legible en mobile;
- no se Entierro bajo navegación.

### 2. Impacto antes de confirmar

- la enumeración del impacto es concreta y escaneable;
- las 2-3 ideas son concretas: suspensión de registro y pérdida de accesos;
- no se usa copy tipo `Perderás tu seguimiento` o `No podrás volver atrás`;
- la acción secundaria `Conservar` no compite visualmente con la primaria.

### 3. Feedback de revocación

- la confirmación es factual y no punitiva;
- no hay dramatismo ni celebración;
- el resultado cierra sin pantalla extra.

### 4. Errores y estados

- `ALREADY_REVOKED` no duplica la pantalla;
- el error recuperable tiene reintento claro;
- `SESSION_EXPIRED` permite reautenticación con continuidad.

## Drift inaceptable

- copy como `Perderás tu seguimiento`, `No podrás volver atrás`, `Debés pensarlo bien`;
- tono legalista o de advertencia;
- impacto enumerado como pared de texto;
- acción secundaria que compite visualmente con la primaria;
- pantalla final celebratoria o dramática.

## Tolerancias aceptables

- pequeñas variaciones de espaciado que no cambien jerarquía de la enumeración;
- ajuste fino de densidad entre desktop y mobile;
- split interno de componentes sin cambiar la lectura del slice.

## Evidencia mínima de cierre

- captura desktop de `S01-ENTRY`, `S02-DEFAULT`, `S03-SUCCESS` y `S03-ALREADY`;
- captura mobile de `S01-ENTRY`, `S02-DEFAULT` y `S03-SUCCESS`;
- evidencia de que la asimetría entre acción primaria y secundaria es clara en mobile.

## Validación real diferida

La evidencia mínima de cierre arriba es el gate documental. La validación UX real con personas reales queda diferida a `Phase 60`.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
