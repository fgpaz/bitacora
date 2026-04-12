# HANDOFF-VISUAL-QA-REG-002 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `REG-002`.

## Por qué el gate está activo

El gate visual está activo porque `REG-002` combina:

- formulario que no debe sentirse como encuesta extensa;
- bloque de medicación condicional que debe aparecer con claridad;
- acción final que debe ser inequívoca;
- datos que no deben perderse ante error o reingreso de sesión.

Una deriva visual aquí puede convertir el check-in en una experiencia larga, clínica o moralizante.

## Checkpoints obligatorios

### 1. Entrada y estructura de bloques

- el primer bloque es visible sin scroll;
- no todos los bloques aparecen a la vez — recorrido progresivo;
- el formulario no parece una pared de campos;
- la densidad es compacta pero legible;
- `Completá tu check-in de hoy` es el titular y domina el inicio.

### 2. Bloque de medicación

- el bloque de medicación aparece solo cuando `medication_taken = true`;
- la expansión es suave y no confunde;
- el campo de horario es numérico con formato `HH:MM`;
- la relación entre `medication_taken` y `medication_time` es clara.

### 3. Acción final y estados

- `Guardar check-in` es la única acción dominante en `S03-READY`;
- la barra de envío es visible y accesible sin importar cuántos bloques haya;
- el estado `submitting` muestra feedback breve;
- la confirmación factual es inline y no introduce pantalla adicional;
- ante `CONSENT_REQUIRED`, la redirrección preserva todos los datos del formulario;
- ante error de validación, el mensaje es localized y no borra los datos.

### 4. Copy y tono

- no aparece copy clínico, moralizante o evaluativo;
- no hay frases como `¿Cumpliste con tu autocuidado?` o `Evolución del paciente`;
- los labels usan lenguaje cotidiano (`Sí` / `No`, `horas`, `horario aproximado`);
- la confirmación no celebra ni dramatiza.

## Drift inaceptable

- formulario completo visible a la primera sin recorrido progresivo;
- bloque de medicación siempre visible aunque `medication_taken = false`;
- CTA secundaria que compita con `Guardar check-in`;
- confirmación decorativa o ceremonial;
- mensaje de error que culpabilice o dramatice;
- pantalla extra entre envío y confirmación;
- copy bilingüe o demasiadopoético;
-提示 o microcopy que asuma juicio clínico.

## Tolerancias aceptables

- pequeñas variaciones de espaciado entre bloques que no cambien la percepción de compacidad;
- densidad del `PatientPageShell` entre desktop y mobile;
- split interno de `FactorBlock` sin cambiar la lectura del formulario;
- ajuste fino del tamaño del touch target en los bloques booleanos.

## Evidencia mínima de cierre

- captura desktop de `S01-ENTRY`, `S02-MEDICATION`, `S03-READY`, `S03-SUCCESS` y `S03-ERROR`;
- captura mobile de `S01-ENTRY`, `S02-MEDICATION`, `S03-READY`, `S03-SUCCESS` y `S03-ERROR`;
- evidencia de foco visible en el CTA `Guardar check-in`;
- confirmación de que el bloque de medicación se expande y colapsa correctamente;
- confirmación de que los datos se preservan ante `401` y `500`.

## Validación real diferida

La evidencia mínima de cierre arriba es el gate documental. La validación UX real con personas reales se documenta en `../UX-VALIDATION/UX-VALIDATION-REG-002.md` y queda diferida a la fase post-runtime del portfolio.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
