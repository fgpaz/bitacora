# HANDOFF-SPEC-TG-001 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `TG-001` para el equipo de backend/telegram.

## Referencias fuente

- `../UI-RFC/UI-RFC-TG-001.md`
- `../UXS/UXS-TG-001.md`
- `../VOICE/VOICE-TG-001.md`
- `../PROTOTYPE/PROTOTYPE-TG-001.md`
- `../../07_tech/TECH-TELEGRAM.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-TG.md`
- `../../03_FL/FL-TG-01.md`
- `../../04_RF/RF-TG-001.md`
- `../../04_RF/RF-TG-002.md`
- `../../04_RF/RF-TG-003.md`

## Alcance implementable

### In

- endpoint webhook `POST /api/v1/telegram/webhook` para recibir mensajes del bot;
- comando `/start` con parsing del código de vinculación;
- validación del código contra `PairingCode` en DB;
- creación de `TelegramSession` con `chat_id` y `patient_id`;
- respuesta conversacional según árbol de estados;
- logging de auditoría para momentos sensibles.

### Out

- implementación del background service de recordatorios (delegado a `TG-002`);
- implementación de desvinculación o gestión de sesiones múltiples;
- UI de generación del código en la web (delegada a slice web);
- gestión de consentimiento dentro del flujo del bot.

## Estados que deben existir

1. `idle` — bot listo para recibir `/start <codigo>`
2. `code_evaluating` — validación en curso
3. `linked` — vínculo confirmado
4. `expired` — código vencido
5. `invalid` — código no reconocido
6. `already_linked` — sesión ya vinculada
7. `no_code` — `/start` sin código
8. `unrecognized` — mensaje no esperado

## Arbol de transiciones

```
/start <code> → code_evaluating
  ├─ code válido + no vinculado → linked → cierre
  ├─ code expirado → expired → orientación a web
  ├─ code inválido → invalid → orientación a código correcto
  └─ chat ya vinculado → already_linked → salida clara

/start sin código → no_code → orientación a flujo web

<mensaje no reconocido> → unrecognized → recordatorio de formato
```

## Restricciones cerradas

- no usar jerga técnica en ningún copy del bot (`pairing`, `binding`, `sync`, `session`, `workflow`)
- no almacenar `chat_id` en logs de aplicación
- no permitir más de 1 validación por código (rate limit interna)
- no confirmar vínculo si `ConsentGrant` está revocada
- el código de vinculación es de un solo uso
- TTL del código: 15 minutos
- no ofrecer reintento de validación con el mismo código si ya fue usado

## Contratos de transición

- webhook recibe mensaje → parsear comando → dispatch a handler según árbol
- `POST /api/v1/telegram/pairing/confirm` es el único endpoint consumido por el bot
- el bot no llama a otros endpoints del sistema
- la generación del código vive en la UI web y no es responsabilidad del bot

## Blockers explícitos ya resueltos

- gap map `2026-04-10` aprueba la apertura de UI-RFC y HANDOFF para `TG-001`
- la tabla `TelegramSession` y el seam webhook estan materializados en Phase 30+ segun `TECH-TELEGRAM.md`
- la validacion UX real queda diferida a `Phase 60`
- el runtime de Telegram existe; la implementacion de backend consume este contrato como especificacion funcional

## Dependencias para implementación

- `TelegramSession` entity y tabla deben existir o ser creadas según `TECH-TELEGRAM.md`
- `PairingCode` entity y tabla deben existir o ser creadas
- `TelegramBotToken` configurado en environment
- webhook registrado en Telegram Bot API (`setWebhook`)
- logging structurado para auditoría de momentos sensibles

## Contrato de copy aprobado

| Estado | Copy |
| --- | --- |
| vínculo exitoso | `Cuenta vinculada. Ya podés registrar tu humor desde acá.` |
| código expirado | `Ese código ya venció. Generá uno nuevo desde la web.` |
| código inválido | `No reconocimos ese código. Mirá el que aparece en la web e intentá de nuevo.` |
| chat ya vinculado | `Esta cuenta de Telegram ya está vinculada a un registro.` |
| sin código | `Enviá el código que aparece en la sección de Telegram de la web.` |
| no reconocido | `No entendimos ese mensaje. Usá el comando /start junto con el código.` |

## Done when de handoff

El handoff spec está bien consumido si backend puede implementar el flujo de vinculación sin tener que decidir:

- qué dice el bot en cada estado;
- cómo se estructura el árbol de comandos;
- cómo se valida el código;
- qué se loguea como momento auditable;
- qué copy está prohibido;
- cuándo se bloquea el vínculo por consentimiento.

## Validación diferida

- este documento no reemplaza `UX-VALIDATION-TG-001.md`
- la evidencia UX real solo llega cuando el bot tenga runtime materializado
- hasta entonces, el contrato conversacional se mantiene como especificación objetivo

---

**Estado:** listo para consumo por el equipo de backend/telegram.
**Siguiente artefacto:** `HANDOFF-ASSETS-TG-001.md` (si aplica), `HANDOFF-MAPPING-TG-001.md` (si aplica), `HANDOFF-VISUAL-QA-TG-001.md` (si aplica).
**Validación UX real:** diferida a `Phase 60`.
