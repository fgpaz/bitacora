# HANDOFF-VISUAL-QA-TG-002 — Control visual final

## Proposito

Este documento define que debe revisarse visualmente antes de cerrar la implementacion de `TG-002`.

## Por que el gate esta activo

El gate visual esta activo porque `TG-002` combina:

- recordatorio programable que debe sentirse breve y opcional;
- teclado inline que debe ser tocable en mobile sin zoom;
- confirmacion factual que no celebra ni insiste;
- continuacion opcional que no domina la lectura;
- manejo de `Ahora no` que no genera presion.

Una deriva en el copy o en el flujo conversacional puede generar culpa, confusion o abandono del registro.

## Checkpoints obligatorios

### 1. Pregunta de humor

- la pregunta es breve y con una sola idea;
- el teclado inline (-3 .. +3 + `Ahora no`) es lo primero visible;
- `Ahora no` tiene el mismo peso visual que las opciones de humor;
- no hay mas de 8 opciones por fila en el teclado.

### 2. Confirmacion de registro

- el copy es factual y no celebratorio;
- no usa emoji ni tono enfatico;
- la continuacion opcional se ofrece sin presion.

### 3. Pregunta de factores (si aplica)

- es opcional y visible como tal;
- `Ahora no` esta presente con el mismo peso;
- no domina la lectura sobre la confirmacion ya enviada.

### 4. Cierre final

- es breve y no insiste;
- no aparece copy interpretativo, terapeutico o motivacional.

### 5. Error recuperable

- el mensaje no culpa al usuario;
- ofrece reintento sin gener anxiety;
- no sugiere que el usuario debe responder de otra manera.

### 6. Mensaje de no reconocido

- no ofrece ayuda extensiva;
- recuerda el comando correcto `/registrar`;
- no excede 100 caracteres.

## Drift inaceptable

- phrasing prohibido: `No te olvides de registrarte`, `Es importante que respondas ahora`, `Seguimos esperando`, `No cumples con tu registro`, `pendiente`;
- confirmacion con emoji, celebracion o tono enfatico;
- pregunta de humor que se sienta como obligacion;
- `Ahora no` que genere un mensaje adicional de presion;
- teclado inline con mas de 8 opciones por fila;
- animacion o feedback visual que retrasen la sensacion de brevedad;
- copy bilingue o demasiado poetico;
- mensaje de error que dramatice o culpabilice.

## Tolerancias aceptables

- variaciones en el texto que conserven la intencion original;
- ajuste fino del tamano del touch target en el teclado inline;
- split interno de componentes sin cambiar la lectura del slice;
- pequeño ajuste en el numero de opciones del teclado si se mantiene debajo de 8.

## Evidencia minima de cierre

- confirmacion de que cada estado conversacional tiene copy asignado segun la tabla en `HANDOFF-SPEC-TG-002.md`;
- confirmacion de que no se usa phrasing prohibido en ninguna respuesta;
- confirmacion de que el logging de audit cubre los 6 momentos audtables;
- confirmacion de que `Ahora no` no genera mensaje adicional;
- confirmacion de que el gate de consentimiento y sesion bloquea el envio del recordatorio.

## Validacion diferida

La evidencia minima de cierre arriba es el gate documental. La validacion UX real con personas reales se documenta en `../UX-VALIDATION/UX-VALIDATION-TG-002.md` y queda diferida a `Phase 60`.

---

**Estado:** checklist visual activa para cierre de implementacion.
**Consumidor principal:** backend + QA visual.
**Runtime ausencia:** TelegramSession, ReminderConfig y el background service no existen hoy; la validacion real espera la materializacion del modulo.
