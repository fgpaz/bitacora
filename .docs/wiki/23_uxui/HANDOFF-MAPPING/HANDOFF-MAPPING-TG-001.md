# HANDOFF-MAPPING-TG-001 — Correspondencia diseño-código

## Proposito

Este documento traduce `TG-001` a ownership tecnico concreto para `backend/telegram/`.

## Archivos objetivo de primera pasada

| Archivo objetivo | Responsabilidad |
| --- | --- |
| `src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs` | pairing REST, sesion REST y webhook |
| `src/Bitacora.Application/Commands/Telegram/GeneratePairingCodeCommand.cs` | generacion de codigo de vinculacion |
| `src/Bitacora.Application/Commands/Telegram/ConfirmPairingCommand.cs` | validacion de codigo desde webhook y creacion de sesion |
| `src/Bitacora.Application/Commands/Telegram/HandleWebhookUpdateCommand.cs` | parseo `/start CODE` y respuestas del bot |
| `src/Bitacora.Domain/Entities/TelegramSession.cs` | entidad de sesion vinculada |
| `src/Bitacora.Domain/Entities/TelegramPairingCode.cs` | entidad de codigo |
| `frontend/components/patient/telegram/TelegramPairingCard.tsx` | UI web de vinculo, estado y desvinculacion |

## Bloques conversacionales y destino sugerido

| Bloque UX/UI | Implementacion sugerida | Nota |
| --- | --- | --- |
| evaluacion de `/start <codigo>` | `HandleWebhookUpdateCommandHandler` | primer punto de entrada desde Telegram |
| validacion de codigo | `ConfirmPairingCommandHandler` | retorna resultado con estado |
| creacion de sesion | `TelegramSession.CreateLinked(...)` | solo si codigo valido y no vinculado |
| respuesta al usuario | `HandleWebhookUpdateCommandHandler` | un solo mensaje por transicion |
| logging de audit | command handlers + repositorios | momento auditable obligatorio |

## Contratos de transicion

### Webhook recibido desde el adapter Telegram

```
POST /api/v1/telegram/webhook
header: X-Telegram-Webhook-Secret: <secret>
body: TelegramWebhookRequest
```

### Confirmacion de vinculacion

No existe endpoint REST publico `pairing/confirm`. La confirmacion ocurre dentro del webhook: `HandleWebhookUpdateCommandHandler` detecta `/start CODE`, invoca `ConfirmPairingCommandHandler`, y el endpoint responde `TelegramWebhookResponse` siempre con HTTP 200 para controlar reentregas de Telegram.

### Lectura de sesion (para saber si ya esta vinculado)

```
GET /api/v1/telegram/session?chat_id=<chat_id>
```

Respuestas esperadas:

- `200`: `{ "linked": true, "patient_id": string }`
- `200`: `{ "linked": false }`

## Estados y su correspondencia

| Estado conversacional | Logica | Response copy |
| --- | --- | --- |
| `idle` | recibe cualquier mensaje | — |
| `code_evaluating` | valida codigo contra PairingCode | — |
| `linked` | crea TelegramSession y confirma | `Cuenta vinculada. Ya podes registrar tu humor desde aca.` |
| `expired` | orienta a web | `Ese codigo ya vencio. Genera uno nuevo desde la web.` |
| `invalid` | orienta al codigo correcto | `No reconocimos ese codigo. Mira el que aparece en la web e intentalo de nuevo.` |
| `already_linked` | salida clara | `Esta cuenta de Telegram ya esta vinculada a un registro.` |
| `no_code` | orienta a flujo web | `Envia el codigo que aparece en la seccion de Telegram de la web.` |
| `unrecognized` | recordatorio de formato | `No entendimos ese mensaje. Usa el comando /start junto con el codigo.` |

## Runtime actual

Los paths de primera pasada fueron reemplazados por el runtime materializado listado arriba. Para QA y mantenimiento, usar `TelegramEndpoints`, `GeneratePairingCodeCommand`, `ConfirmPairingCommand`, `HandleWebhookUpdateCommand`, `TelegramSession`, `TelegramPairingCode` y `TelegramPairingCard` como ownership vigente.

## Momentos auditables

| Momento | Datos logueados |
| --- | --- |
| generacion del codigo | patient_id, code, expires_at, created_at |
| validacion del codigo | code, result, timestamp |
| creacion de TelegramSession | patient_id, chat_id, linked_at, result |
| respuesta de vinculacion | copy enviado, timestamp |

## Restricciones cerradas para implementacion

- no usar jerga tecnica en copy del bot (`pairing`, `binding`, `sync`, `session`, `workflow`)
- no almacenar `chat_id` en logs de aplicacion
- no permitir mas de 1 validacion por codigo (rate limit interna)
- no confirmar vinculo si `ConsentGrant` esta revocada
- el codigo de vinculacion es de un solo uso
- TTL del codigo: 15 minutos
- no ofrecer reintento de validacion con el mismo codigo si ya fue usado

## Regla de ownership

- `TG-001` implementa solo el flujo de vinculacion;
- recordatorios y registro conversacional quedan fuera de este mapping (delegados a `TG-002`);
- si el codigo necesita bloques extra, deben caer dentro de estos owners y no inventar una arquitectura paralela.

## Sync runtime 2026-04-20

La implementación paciente vigente para vinculación y configuración web de Telegram vive en:

| Runtime actual | Responsabilidad |
| --- | --- |
| `frontend/app/(patient)/configuracion/telegram/page.tsx` | entrada de configuración Telegram paciente |
| `frontend/components/patient/telegram/TelegramPairingCard.tsx` | vinculación, estado, desvinculación y horario local |
| `frontend/components/patient/telegram/TelegramPairingCard.module.css` | estados responsive y tokens visuales |
| `frontend/lib/api/client.ts` | `setReminderSchedule()` convierte hora local Buenos Aires a UTC |
| `src/Bitacora.Api/Endpoints/Telegram/TelegramEndpoints.cs` | endpoints REST Telegram reales |
| `src/Bitacora.Domain/Entities/TelegramSession.cs` | entidad de sesión vinculada |
| `src/Bitacora.Domain/Entities/ReminderConfig.cs` | configuración persistida del recordatorio |

## Nota historica

La tabla `Sync runtime 2026-04-20` conserva el puente operativo entre el handoff original y los paths reales.

## Deltas 2026-04-22 — impeccable-hardening

> 2026-04-22 — sync impeccable-hardening: split presentacional de `TelegramPairingCard` aplicado en W5 de la rama `feature/impeccable-hardening-2026-04-22`. Fuente de verdad: `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md`.

### Mapping de subcomponentes presentacionales

| UI-RFC primitive | Implementación frontend | Archivo |
|---|---|---|
| `TelegramCodeBridgePanel` | `TelegramPairingCard.tsx` (~300 líneas, padre con estado) | `frontend/components/patient/telegram/TelegramPairingCard.tsx` |
| `TelegramCodeBridgePanel.codeDisplay` | `PairingCodeDisplay.tsx` (presentacional) | `frontend/components/patient/telegram/pairing/PairingCodeDisplay.tsx` |
| `TelegramCodeBridgePanel.instructions` | `PairingInstructions.tsx` (presentacional) | `frontend/components/patient/telegram/pairing/PairingInstructions.tsx` |
| `TelegramCodeBridgePanel.reminderSection` | `PairingReminderSection.tsx` (presentacional) | `frontend/components/patient/telegram/pairing/PairingReminderSection.tsx` |

Nota: el split fue declarado en W5 (impeccable-extract) para reducir `TelegramPairingCard.tsx` de 455 a ~300 líneas y permitir memoización. El estado compartido (polling, generating, unlinking, saving_schedule) vive exclusivamente en el componente padre. Los subcomponentes no tienen lógica de negocio ni acceso a API directamente.

Los 3 subcomponentes sirven también para `TG-002` (schedule picker y sección de recordatorio conversacional); ver `HANDOFF-MAPPING-TG-002.md`.

---

**Estado:** mapping listo para `backend/telegram/`.
**Siguiente artefacto:** `HANDOFF-VISUAL-QA-TG-001.md`.
**Runtime Telegram:** implementado; mapping sincronizado con paths reales el 2026-04-20; subcomponentes presentacionales agregados el 2026-04-22.
