# HANDOFF-VISUAL-QA-TG-001 — Control visual final

## Proposito

Este documento define que debe revisarse visualmente antes de cerrar la implementacion de `TG-001`.

## Por que el gate esta activo

El gate visual esta activo porque `TG-001` combina:

- vinculacion de cuenta sensible;
- confirmacion que debe sentirse como puente, no como registro;
- manejo de errores que no debe culpar ni confundir;
- continuidad emocional entre estados del bot.

Una deriva en el copy o en el flujo conversacional puede alterar la percepcion de control, privacidad y claridad del usuario.

## Checkpoints obligatorios

### 1. Mensaje de exito de vinculacion

- el copy es breve y factual;
- no celebra ni usa emoji;
- queda claro cual es el siguiente paso;
- no aparece jerga tecnica.

### 2. Mensaje de codigo expirado

- orienta a regenerar en la web sin culpar;
- la instruccion es de una sola accion;
- no aparece tecnicismo como `pairing`, `binding`, `session sync`.

### 3. Mensaje de codigo invalido

- orienta al codigo correcto sin explicar razones tecnicas;
- no sugiere que el sistema esta roto.

### 4. Mensaje de sesion ya vinculada

- la salida es clara y sin ambiguedad;
- no sugiere que se desvincula sin mas contexto.

### 5. Mensaje de sin codigo

- la instruccion es directa y de una sola linea;
- no ofrece ayuda extensiva.

### 6. Mensaje de no reconocido

- recuerda el formato correcto sin ser tecnico;
- no excede 80 caracteres.

## Drift inaceptable

- copy con jerga tecnica (`pairing`, `binding`, `session`, `sync`, `workflow`);
- mensaje de error que culpabilice al usuario;
- confirmacion con emoji o tono enfatico;
- mas de un mensaje por transicion de estado;
- bloque o ambiguedad en la salida del flujo.

## Tolerancias aceptables

- variaciones en el texto que conserven la intencion original;
- ajuste de formato del codigo en el mensaje de orientacion;
- small split en el numero de mensajes si el flujo lo requiere explicitamente.

## Evidencia minima de cierre

- confirmacion de que cada estado conversacional tiene copy asignado segun la tabla en `HANDOFF-SPEC-TG-001.md`;
- confirmacion de que ningun copy contiene jerga tecnica;
- confirmacion de que el logging de audit cubre los 4 momentos audtables;
- confirmacion de que el gate de consentimiento revocado bloquea el vinculo.

## Validacion diferida

La evidencia minima de cierre arriba es el gate documental. La validacion UX real con personas reales se documenta en `../UX-VALIDATION/UX-VALIDATION-TG-001.md` y queda diferida a `Phase 60`.

---

**Estado:** checklist visual activa para cierre de implementacion.
**Consumidor principal:** backend + QA visual.
**Runtime ausencia:** TelegramSession no existe hoy; la validacion real espera la materializacion del modulo.
