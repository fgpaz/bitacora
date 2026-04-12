# HANDOFF-VISUAL-QA-VIN-002 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `VIN-002`.

## Por qué el gate está activo

El gate visual está activo porque `VIN-002` combina:

- ingreso de código sin contexto técnico;
- resultado donde acceso todavía no está habilitado;
- transiciones entre estados que no deben parecer errores.

Una deriva visual aquí puede alterar la percepción de control y claridad sobre el vínculo.

## Checkpoints obligatorios

### 1. Entrada y campo

- el contexto breve se entiende sin leer mucho;
- el campo de código es el protagonista;
- no se piden más datos que el código.

### 2. Estados de error y expiración

- código inválido tiene recuperación clara y digna;
- código expirado no se siente como error técnico;
- no se usan tecnicismos como `binding`, `token` o `emparejamiento`.

### 3. Confirmación de vínculo

- queda claro que el vínculo está hecho pero el acceso está desactivado;
- no se usa copy tipo `Acceso habilitado`;
- la navegación post-vínculo es directa.

## Drift inaceptable

- copy como `Emparejar profesional`, `Binding code`, `Acceso habilitado`;
- pantalla extra entre ingreso de código y confirmación;
- dramatización del estado pendiente de acceso;
- confirmación celebratoria.

## Tolerancias aceptables

- pequeñas variaciones de espaciado que no cambien jerarquía;
- ajuste fino de densidad entre desktop y mobile;
- split interno de componentes sin cambiar la lectura del slice.

## Evidencia mínima de cierre

- captura desktop de `S01-DEFAULT`, `S01-READY`, `S02-SUCCESS`, `S03-INVALID` y `S03-EXPIRED`;
- captura mobile de `S01-DEFAULT` y `S02-SUCCESS`;
- evidencia de foco visible en el campo de código y en la CTA.

## Validación real diferida

La evidencia mínima de cierre arriba es el gate documental. La validación UX real con personas reales queda diferida a `Phase 60`.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
