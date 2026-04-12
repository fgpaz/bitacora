# HANDOFF-VISUAL-QA-VIN-001 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `VIN-001`.

## Por qué el gate está activo

El gate visual está activo porque `VIN-001` combina:

- formulario con campo sensible de email;
- promesa de vínculo pendiente sin acceso automático;
- conflicto que no debe parecer error genérico.

Una deriva visual aquí puede alterar la percepción de responsabilidad profesional y control de datos.

## Checkpoints obligatorios

### 1. Entrada y formulario

- el email es lo primero que se pide;
- la CTA `Enviar invitación` se habilita solo con email válido;
- el alcance del vínculo pendiente se lee en una línea;
- no hay segunda acción dominante compitiendo.

### 2. Feedback de envío

- el estado `submitting` es breve y no parece pantalla técnica;
- no aparecen spinners agresivos;
- la confirmación de estado pendiente no es celebratoria.

### 3. Conflicto

- la carta de conflicto explica sin dramatizar;
- no presenta el conflicto como error técnico;
- no repite la acción de invitar.

## Drift inaceptable

- copy tipo `Alta de paciente` o `Agregar a tu cartera`;
- promesa de acceso automático a datos;
- confirmación celebratoria tipo `Invitación enviada con éxito`;
- campo extra oltre el email;
- íconos de éxito exagerados.

## Tolerancias aceptables

- pequeñas variaciones de espaciado que no cambien jerarquía;
- ajuste fino de densidad entre desktop y mobile;
- split interno de componentes sin cambiar la lectura del slice.

## Evidencia mínima de cierre

- captura desktop de `S01-DEFAULT`, `S01-READY`, `S02-SUCCESS` y `S03-CONFLICT`;
- captura mobile de `S01-DEFAULT`, `S01-READY` y `S02-SUCCESS`;
- evidencia de foco visible en CTA y en el campo de email.

## Validación real diferida

La evidencia mínima de cierre arriba es el gate documental. La validación UX real con personas reales queda diferida a `Phase 60`.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
