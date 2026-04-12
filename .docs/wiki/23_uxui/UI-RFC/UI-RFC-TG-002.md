# UI-RFC-TG-002 — Contrato técnico UI de recordatorio conversacional Telegram

## Propósito

Este documento traduce el slice `TG-002` a contrato conversacional implementable para el bot de Telegram.

No declara `UX-VALIDATION`, no reemplaza `UXS-TG-002` ni autoriza implementación de vinculación. Su función es congelar la estructura conversacional, recordatorios activos, keyboard inline, fallbacks, respuestas opcionales y momentos auditables del paquete de recordatorio que el runtime Telegram puede consumir una vez materializado.

## Estado del gate

- `slice`: `TG-002`
- `estado`: `abierto para UI-RFC + HANDOFF`
- `fundamento`: wave-prod UX/UI gap map `2026-04-10`
- `límite`: este contrato aplica solo a recordatorio y registro conversacional; no incluye vinculación (`TG-001`)
- `deuda explícita`: la validación UX real queda diferida a `Phase 60`; no se genera evidencia simulada
- `runtime check`: `TelegramSession` y `ReminderConfig` existen en runtime (Phase 30+): entities, tablas, seam webhook y `ReminderWorker` activos segun `TECH-TELEGRAM.md`

## Referencias obligatorias

- `../../16_patrones_ui.md`
- `../../21_matriz_validacion_ux.md`
- `../../07_baseline_tecnica.md`
- `../../07_tech/TECH-TELEGRAM.md`
- `../../09_contratos_tecnicos.md`
- `../../06_matriz_pruebas_RF.md`
- `../../06_pruebas/TP-TG.md`
- `../../06_pruebas/TP-REG.md`
- `../../03_FL/FL-TG-02.md`
- `../../03_FL/FL-REG-02.md`
- `../../04_RF/RF-TG-010.md`
- `../../04_RF/RF-TG-011.md`
- `../../04_RF/RF-TG-012.md`
- `../../04_RF/RF-REG-010.md`
- `../../04_RF/RF-REG-011.md`
- `../../04_RF/RF-REG-012.md`
- `../../04_RF/RF-REG-013.md`
- `../../04_RF/RF-REG-014.md`
- `../../04_RF/RF-REG-015.md`
- `../UXR/UXR-TG-002.md`
- `../UXI/UXI-TG-002.md`
- `../UJ/UJ-TG-002.md`
- `../VOICE/VOICE-TG-002.md`
- `../UXS/UXS-TG-002.md`
- `../PROTOTYPE/PROTOTYPE-TG-002.md`

## Slice cubierto

### In

- el sistema envía un recordatorio programable al paciente via Telegram;
- el paciente decide si responde o deja pasar;
- si responde, el bot registra el humor y puede continuar con factores adicionales;
- el flujo conversacional se mantiene efímero (no persiste estado entre sesiones).

### Out

- vinculación de cuenta (delegado a `TG-001`);
- configuración de recordatorios por el paciente (futuro `TG-003`);
- registro de factores sin Telegram;
- gestión de medicación.

## Resultado que la conversación debe producir

La interacción debe sentirse como un toque breve y opcional que:

1. pregunta con una sola idea;
2. ofrece salida fácil visible;
3. confirma sin insistir;
4. no genera culpa por no responder.

## Trigger

| Trigger | Origen | Condición |
| --- | --- | --- |
| `daily_reminder` | `ReminderConfig` + timer | consentimiento activo y sesión vinculada |
| `/registrar` | usuario | cualquier momento |
| cualquier otro mensaje | usuario | no reconocido → fallback |

## Contrato de respuesta del bot

### `daily_reminder` — pregunta de humor

```
¿ Cómo te sentís ahora?
[+3] [+2] [+1] [0] [-1] [-2] [-3]
```

- keyboard inline con valores discretos
- acción dominante: elegir un valor
- salida fácil visible: `Ahora no`

### `ahora_no`

El bot no responde con más mensajes de presión. Silencio útil.

### `respuesta_humor`

```
Registrado: +1. Si querés, podés contarme cuántas horas dormiste.
[<4h] [4-6h] [6-8h] [8+h] [Ahora no]
```

- continuación opcional con factor sueño
- la persona puede responder o dejar pasar

### `respuesta_factores`

```
Registrado. ¡Buen día!
```

- cierre del registro diario
- no insiste ni celebra

### `error_registro`

```
No pudimos registrar esa respuesta. Probá de nuevo si querés.
```

- permite reintentar sin presión
- no ofrece alternativa si el usuario quiere dejar pasar

### `recordatorio_bloqueado` (consentimento revocado o sesión desvinculada)

El sistema no envía el recordatorio. Gate en el background service antes del envío.

### `no_reconocido`

```
No entendimos ese mensaje. Usá /registrar para registrar tu humor o esperá tu próximo recordatorio.
```

- no ofrecer ayuda extensiva
- recordar el comando correcto

## Modelo de estados conversacionales

| Estado | Qué espera el bot | Transición |
| --- | --- | --- |
| `reminder_sent` | selección de valor o salida fácil | → `reply_success`, `reminder_skipped` |
| `reply_submitting` | API en curso | protección contra double-tap |
| `reply_success` | confirmación breve | cierre o continuación factors |
| `factors_prompt` | selección de factor o salida fácil | → `reply_success` o `reminder_skipped` |
| `reply_error` | reintento o silencio del usuario | permite reintentar |
| `reminder_skipped` | silencio del usuario | cierre sin presión |

## Contrato de copy auditado

| Momento | Copy aprobado | Restricción |
| --- | --- | --- |
| pregunta de humor | `¿Cómo te sentís ahora?` | no preguntar por obligación |
| salida facil | `Ahora no` | visible, sin presion |
| confirmación registro | `Registrado: +1.` | factual, no celebratorio |
| cierre final | `¡Buen día!` | breve, no enfático |
| error de registro | `No pudimos registrar esa respuesta. Probá de nuevo si querés.` | no culpar, ofrecer reintento |
| no reconocido | `No entendimos ese mensaje. Usá /registrar para registrar tu humor o esperá tu próximo recordatorio.` | no exceder 100 caracteres |

### Phrasing prohibido

- `No te olvides de registrarte`
- `Es importante que respondas ahora`
- `Seguimos esperando`
- `No cumples con tu registro`
- `pendiente`

## Momentos auditables

1. **envío del recordatorio**: patient_id, reminder_config_id, timestamp, resultado de envío
2. **respuesta del paciente**: valor seleccionado, timestamp, session_id
3. **registro de humor**: mood_entry_id, valor, timestamp
4. **registro de factores**: factor_type, valor, timestamp
5. **omisión**: cuándo se usó `ahora no`, sin guardar detalle

## Contratos backend que el bot debe consumir

### Envío de recordatorio (background service)

- source: `ReminderConfig` WHERE `next_fire_at <= now()`
- gate: `ConsentGrant.active=true` AND `TelegramSession.linked=true`
- API interna: no expuesta al bot; el background service llama directo a la DB y a Telegram API

### Registro de humor

- request: `POST /api/v1/mood-entries`
  - body: `{ "mood_value": number, "source": "telegram", "chat_id": number }`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada:
  - `201`: `{ "mood_entry_id": string, "mood_value": number, "recorded_at": string }`
  - `401`: sin sesión válida → responder con mensaje genérico
  - `422`: valor fuera de rango

### Registro de factores

- request: `POST /api/v1/daily-checkin`
  - body: `{ "sleep_hours": string, "physical_activity": boolean, "medication_taken": string, "chat_id": number }`
  - header: `Authorization: Bearer <access_token>`
- el body es parcial si el usuario no completa todos los factores

### Rate limit

- Telegram API: max 30 mensajes/segundo global
- Retry con backoff exponencial: max 3 intentos
- Si el límite persiste, registrar el fallo y no insistir

## Reglas de seguridad y privacidad

- el `chat_id` no se almacena en logging
- si `ConsentGrant` está revocada, no se envía ningún recordatorio (gate en background service)
- si `TelegramSession` está desvinculada, no se envía ningún recordatorio
- los factores registradaos respectan la misma política de consentimiento que `MoodEntry`
- no se registra el humor si el usuario eligió `Ahora no`; el silencio no se almacena como dato

## Dependencias abiertas

- `TelegramSession` y `ReminderConfig` existen en runtime (Phase 30+); los seams de recordatorio son operables segun `TECH-TELEGRAM.md`
- `TG-001` (vinculación) queda fuera de este contrato
- cuando exista runtime real, `UX-VALIDATION-TG-002.md` podrá reabrir este contrato si la evidencia observada contradice el diseño

## Criterio de implementación

La implementación cumple este contrato si:

1. el recordatorio se envía solo cuando consentimiento activo y sesión vinculada
2. la pregunta es siempre breve con keyboard inline
3. `Ahora no` no genera respuesta de presión
4. la confirmación no celebra ni insiste
5. el error permite reintentar sin culpa
6. el logging de auditoría cubre cada trigger y cada transición de estado
7. el flujo de recordatorio no mezcla estados de `TG-001`

---

**Estado:** `UI-RFC` activo para `TG-002` bajo gap map `2026-04-10`.
**Siguiente capa gobernada:** `HANDOFF-SPEC-TG-002.md`, `HANDOFF-ASSETS-TG-002.md`, `HANDOFF-MAPPING-TG-002.md`, `HANDOFF-VISUAL-QA-TG-002.md`.
**Validación UX real:** diferida a `Phase 60`; no se genera evidencia anticipada.
