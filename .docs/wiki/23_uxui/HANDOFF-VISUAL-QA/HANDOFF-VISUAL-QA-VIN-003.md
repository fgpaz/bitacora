# HANDOFF-VISUAL-QA-VIN-003 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `VIN-003`.

## Por qué el gate está activo

El gate visual está activo porque `VIN-003` combina:

- revocación que puede sentirse como pérdida;
- impacto concreto en acceso;
- resultado final que no debe parecer castigo.

Una deriva visual aquí puede generar desconfianza o regrets post-revocación.

## Checkpoints obligatorios

### 1. Vínculo actual

- queda claro con quién está vinculado antes de decidir;
- no se presenta como error ni como warning.

### 2. Impacto antes de confirmar

- el impacto se enumera en 2-3 ideas concretas;
- no se usa copy tipo `¿Seguro que querés dejar de recibir ayuda?`;
- no se iguala vínculo con consentimiento.

### 3. Resultado

- el acceso cortado se presenta como hecho, no como consecuencia negativa;
- no hay copy tipo `Has revocado el vínculo`;
- la navegación post-revocación es directa.

## Drift inaceptable

- copy como `Perderás seguimiento`, `Eliminar relación terapéutica`, `Abandono`;
- dramatización del impacto;
- promesa de que se puede volver atrás si no es reversible;
- pantalla final de confirmación extensa.

## Tolerancias aceptables

- pequeñas variaciones de espaciado que no cambien jerarquía;
- ajuste fino de densidad entre desktop y mobile;
- split interno de componentes sin cambiar la lectura del slice.

## Evidencia mínima de cierre

- captura desktop de `S01-DEFAULT`, `S02-DEFAULT`, `S03-SUCCESS` y `S03-ALREADY`;
- captura mobile de `S01-DEFAULT`, `S02-DEFAULT` y `S03-SUCCESS`;
- evidencia de foco visible en la CTA de revocación.

## Validación real diferida

La evidencia mínima de cierre arriba es el gate documental. La validación UX real con personas reales queda diferida a `Phase 60`.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
