# UI-RFC-TG-001 — Contrato técnico UI de vinculación Telegram

## Propósito

Este documento traduce el slice `TG-001` a contrato conversacional implementable para el bot de Telegram.

No declara `UX-VALIDATION`, no reemplaza `UXS-TG-001` ni autoriza implementación de recordatorios. Su función es congelar la estructura conversacional, estados del bot, triggers, respuestas, fallbacks y momentos auditables del paquete de vinculación que el runtime Telegram puede consumir una vez materializado.

## Estado del gate

- `slice`: `TG-001`
- `estado`: `abierto para UI-RFC + HANDOFF`
- `fundamento`: wave-prod UX/UI gap map `2026-04-10`
- `límite`: este contrato aplica solo a vinculación; no incluye recordatorios (`TG-002`)
- `deuda explícita`: la validación UX real queda diferida a `Phase 60`; no se genera evidencia simulada
- `runtime check`: `TelegramSession` existe en runtime (Phase 30+): entity, tabla, seam webhook y `ReminderWorker` materializados segun `TECH-TELEGRAM.md`

## Referencias obligatorias

- `../../16_patrones_ui.md`
- `../../21_matriz_validacion_ux.md`
- `../../07_baseline_tecnica.md`
- `../../07_tech/TECH-TELEGRAM.md`
- `../../09_contratos_tecnicos.md`
- `../../06_matriz_pruebas_RF.md`
- `../../06_pruebas/TP-TG.md`
- `../../03_FL/FL-TG-01.md`
- `../../04_RF/RF-TG-001.md`
- `../../04_RF/RF-TG-002.md`
- `../../04_RF/RF-TG-003.md`
- `../UXR/UXR-TG-001.md`
- `../UXI/UXI-TG-001.md`
- `../UJ/UJ-TG-001.md`
- `../VOICE/VOICE-TG-001.md`
- `../UXS/UXS-TG-001.md`
- `../PROTOTYPE/PROTOTYPE-TG-001.md`

## Slice cubierto

### In

- la persona genera un código de vinculación desde la web;
- la persona envía `/start <codigo>` al bot;
- el bot valida el código y crea `TelegramSession`;
- el bot confirma el vínculo exitoso;
- manejo de código expirado, inválido o sesión ya vinculada.

### Out

- registro de humor (delegado a `TG-002`);
- recordatorios programados (delegado a `TG-002`);
- revocación de vínculo (futuro `TG-003`);
- flujos profesionales.

## Resultado que la conversación debe producir

La interacción debe sentirse como un puente corto y guiado que:

1. entrega un código con instrucción única;
2. muestra el comando exacto `/start <codigo>` y permite copiar el mensaje completo;
3. valida y confirma sin fricción ni jerga;
4. deja claro el siguiente paso;
5. no confunde vinculación con registro.

### Puente web obligatorio

La pantalla web de vinculación debe:

- mostrar el comando completo `/start BIT-XXXXX`, no solo el código;
- ofrecer `Copiar mensaje`, `Abrir Telegram` y `Ya envié el mensaje`;
- comprobar la sesión con polling breve y una acción manual;
- cuando el vínculo queda activo, mostrar el siguiente paso hacia recordatorio o prueba del bot.

## Trigger

| Trigger | Origen | Condición |
| --- | --- | --- |
| `/start <codigo>` | usuario | formato válido |
| `/start` sin código | usuario | mostrar ayuda breve |
| cualquier otro mensaje | usuario | no reconocido → fallback |

## Contrato de respuesta del bot

### `code_valido` → vínculo exitoso

```
Cuenta vinculada. Ya podés registrar tu humor desde acá.
```

- acción dominante: ninguna (cierre del flujo de vinculación)
- salida: el canal queda listo para `TG-002`

### `code_expirado`

```
Ese código ya venció. Generá uno nuevo desde la web.
```

- acción dominante: derivar a la web para regenerar
- fallback si la persona insiste: repetir el mismo mensaje

### `code_invalido`

```
No reconocimos ese código. Mirá el que aparece en la web e intentá de nuevo.
```

- acción dominante: orientar al código correcto
- no usar jerga técnica (`pairing`, `binding`, `session sync`)

### `chat_ya_vinculado`

```
Esta cuenta de Telegram ya está vinculada a un registro.
```

- acción dominante: salida clara sin ambigüedad
- no sugerir que se desvincula sin más contexto

### `sin_codigo` (solo `/start`)

```
Enviá el código que aparece en la sección de Telegram de la web.
```

- acción dominante: orientar al flujo web

### `no_reconocido`

```
No entendimos ese mensaje. Usá el comando /start junto con el código.
```

- acción dominante: recordar el formato correcto
- no ofrecer ayuda extensiva

## Modelo de estados conversacionales

| Estado | Qué espera el bot | Transición |
| --- | --- | --- |
| `idle` | cualquier mensaje | evalúa formato |
| `code_evaluating` | validación del código | → `linked`, `expired`, `invalid`, `already_linked` |
| `linked` | fin del flujo | cierre |
| `error_retryable` | reintento del usuario | → `code_evaluating` |

## Contrato de copy auditado

| Momento | Copy aprobado | Restricción |
| --- | --- | --- |
| vínculo exitoso | `Cuenta vinculada. Ya podés registrar tu humor desde acá.` | no celebrar, no agregar emoji |
| código expirado | `Ese código ya venció. Generá uno nuevo desde la web.` | no culpar al usuario |
| código inválido | `No reconocimos ese código. Mirá el que aparece en la web e intentá de nuevo.` | no explicar razones técnicas |
| chat ya vinculado | `Esta cuenta de Telegram ya está vinculada a un registro.` | no ambigüedad |
| sin código | `Enviá el código que aparece en la sección de Telegram de la web.` | una sola instrucción |
| no reconocido | `No entendimos ese mensaje. Usá el comando /start junto con el código.` | no exceder 80 caracteres |

## Momentos auditables

1. **generación del código**: cuándo, quién, session vinculada
2. **validación del código**: código usado, resultado, timestamp
3. **creación de TelegramSession**: patient_id, chat_id, resultado
4. **respuesta de vínculo**: copy enviado, idioma, timestamp

## Contratos backend que el bot debe consumir

### Verificación y creación de vínculo (vía webhook)

> **Nota:** No existe un endpoint REST `POST /api/v1/telegram/pairing/confirm`. La confirmación de vínculo ocurre únicamente a través del webhook. El bot de Telegram envía `/start CODE` como un update de Telegram; este update se reenvía al webhook que internamente procesa el código.

- request: `POST /api/v1/telegram/webhook`
  - header: `X-Telegram-Webhook-Secret: <token>` (obligatorio; fail-closed)
  - body: `{ "update": "/start BIT-XXXXX", "chat_id": "<telegram_chat_id>", "trace_id": "<guid>" }`
- respuesta esperada (siempre HTTP 200 para Telegram):
  - `200`: `{ "accepted": true, "error_code": null, "bot_message": "Te vinculaste exitosamente a Bitácora" }` (caso exito)
  - `200`: `{ "accepted": false, "error_code": "code_invalid|code_expired|chat_already_linked", "bot_message": null }` (caso fail-closed — error interno, no se revela detalle)
- **Comportamiento fail-closed:** si el secreto del webhook no coincide, retorna HTTP 200 con `accepted=false` y `error_code=FORBIDDEN` sin `bot_message`. El bot no envía respuesta al usuario.

### Lectura de sesión (para saber si ya está vinculado)

### Lectura de sesión (para saber si ya está vinculado)

- request: `GET /api/v1/telegram/session?chat_id=<chat_id>`
- respuesta esperada:
  - `200`: `{ "linked": true, "patient_id": string }`
  - `200`: `{ "linked": false }`

## Reglas de seguridad y privacidad

- el `chat_id` no se almacena en logging
- el `code` tiene TTL de 15 minutos (expira en el borde, no antes)
- si `ConsentGrant` está revocada, el vínculo no se permite y el bot responde con mensaje genérico
- Rate limit: 1 validación por código; no se permite reintento con el mismo código

## Dependencias abiertas

- `TelegramSession` existe en runtime (Phase 30+); la tabla, el seam webhook y el flujo conversacional son operables segun `TECH-TELEGRAM.md`
- `TG-002` (recordatorios) queda fuera de este contrato
- `UX-VALIDATION-TG-001.md` puede reabrir este contrato si la evidencia observada contradice el diseño

## Criterio de implementación

La implementación cumple este contrato si:

1. el bot responde a `/start <code>` según el árbol de estados above
2. ningun copy usa jerga tecnica o mensajes de error tecnicos al usuario
3. `TelegramSession` se crea solo cuando el código es válido y el `chat_id` no está en uso
4. los momentos auditables están cubiertos por logging structurado
5. el flujo de vinculación no mezcla estados de `TG-002`

---

**Estado:** `UI-RFC` activo para `TG-001` bajo gap map `2026-04-10`.
**Siguiente capa gobernada:** `HANDOFF-SPEC-TG-001.md`, `HANDOFF-ASSETS-TG-001.md`, `HANDOFF-MAPPING-TG-001.md`, `HANDOFF-VISUAL-QA-TG-001.md`.
**Validación UX:** `UX-VALIDATION-TG-001.md` cubre la evidencia estática disponible; el E2E conversacional completo queda sujeto a evidencia post-deploy sanitizada.
