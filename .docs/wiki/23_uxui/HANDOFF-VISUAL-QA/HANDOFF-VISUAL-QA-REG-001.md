# HANDOFF-VISUAL-QA-REG-001 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `REG-001`.

## Por qué el gate está activo

El gate visual está activo porque `REG-001` combina:

- escala emocional que debe sentirse instantánea;
- gesto único que no debe parecer formulario;
- confirmación factual que no debe competir con la acción siguiente;
- estados de error que no deben perder el valor ya elegido.

Una deriva visual aquí puede alterar la percepción de brevedad, control y privacidad del registro.

## Checkpoints obligatorios

### 1. Entrada y escala

- la escala -3..+3 es lo primero visible sin scroll;
- los 7 valores son tocables en mobile sin zoom;
- el valor seleccionado tiene foco visible inequívoco;
- no hay campos extra más allá del score;
- el titular `¿Cómo te sentís ahora?` es legible y no compite con la escala.

### 2. Feedback de guardado

- el estado `submitting` muestra feedback sin pantalla de loading genérica;
- no aparecen spinners agresivos o skeletons que ocupen todo el viewport;
- tras el guardado exitoso, la confirmación es factual y desaparece rápido;
- la continuidad tras guardar es clara (volver o siguiente paso).

### 3. Manejo de errores

- el error recuperable no borra el valor ya seleccionado;
- el mensaje de error es breve y localized;
- el reintento es accesible desde el mismo estado;
- ante `CONSENT_REQUIRED`, la redirrección es directa y no muestra pantalla de error de registro.

### 4. Consentimiento y sesión

- ante `CONSENT_REQUIRED`, el vínculo al panel de consentimiento es claro;
- ante sesión expirada, el prompt de reingreso preserva el valor ya elegido;
- en ningún estado aparece copy interpretativo, terapéutico o motivacional.

## Drift inaceptable

- escala con más de 7 valores;
- animaciones que retrasen la sensación de gesto instantáneo;
- confirmación decorativa o celebratoria;
- pantalla extra entre selección y confirmación;
- copy como `¿Cómo va tu día?`, `Contanos más si querés`, `Excelente, seguí así`;
- labels de escala que interpreten el valor (por ejemplo, `Muy mal` / `Muy bien`);
- mensaje de error que dramatice o culpabilice.

## Tolerancias aceptables

- pequeñas variaciones de espaciado que no cambien jerarquía de la escala;
- densidad del `PatientPageShell` entre desktop y mobile;
- split interno de componentes sin cambiar la lectura del slice;
- ajuste fino del tamaño del touch target en la escala.

## Evidencia mínima de cierre

- captura desktop de `S01-ENTRY`, `S02-SUCCESS` y `S02-ERROR`;
- captura mobile de `S01-ENTRY`, `S02-SUCCESS` y `S02-ERROR`;
- evidencia de foco visible en el valor seleccionado y en el CTA de guardado;
- confirmación de que el valor seleccionado se preserva ante error de red (`500`);
- confirmación de que la redirrección a consentimiento preserva el valor en estado local.

## Validación real diferida

La evidencia mínima de cierre arriba es el gate documental. La validación UX real con personas reales se documenta en `../UX-VALIDATION/UX-VALIDATION-REG-001.md` y queda diferida a la fase post-runtime del portfolio.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
