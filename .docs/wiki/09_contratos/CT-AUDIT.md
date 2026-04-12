# CT-AUDIT: Auditoria, Pseudonimizacion y Fail-Closed

> Root: `09_contratos_tecnicos.md` errores globales, `07_baseline_tecnica.md` invariantes.

## Audit log (AccessAudit)

## Estado actual

- Implementado en `Wave 1`: audit para `bootstrap` indirecto de identidad local, lectura de consentimiento, grant/revoke de consentimiento, bloqueo por falta de consentimiento, creacion de `MoodEntry` y create/update de `DailyCheckin`.
- Diferido: auditoria de lecturas profesionales, `CareLink`, `BindingCode`, export y Telegram.

### Reglas

1. **Append-only:** sin UPDATE ni DELETE. Jamas.
2. **Toda operacion transaccional** genera al menos un AccessAudit con el mismo trace_id.
3. **Fail-closed:** si INSERT de audit falla → la operacion completa falla (no se retornan datos).
4. **trace_id obligatorio:** si el request no trae X-Trace-Id, se genera al ingreso.

### Campos

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| audit_id | UUID | PK generado |
| trace_id | UUID | Traza end-to-end del request |
| actor_id | UUID | ID real del actor (SOLO en esta tabla) |
| pseudonym_id | string | HASH para logs operacionales |
| action_type | enum | create/read/update/delete/grant/revoke/export |
| resource_type | string | mood_entry/daily_checkin/consent_grant/care_link/etc |
| resource_id | UUID? | ID del recurso afectado |
| patient_id | UUID? | Paciente cuyos datos se afectan |
| outcome | enum | ok/failed/denied |
| created_at_utc | timestamp | Momento exacto |

### Invariantes de auditoria

1. **actor_id solo en AccessAudit.** Ninguna otra tabla, log operacional ni telemetria lleva el identificador real del actor.
2. **PseudonymizationService.fail-closed:** si BITACORA_PSEUDONYM_SALT no resuelve → 500 PSEUDONYM_SALT_MISSING, ninguna operacion que dependa del servicio prosigue.
3. **Audit de bloqueo por falta de consentimiento:** ConsentRequiredMiddleware genera AccessAudit outcome=Denied sobre consent_grant; es la evidencia de que el sistema denego acceso por no tener consentimiento.
4. **Export CSV genera audit de tipo 'export':** action_type='export' sobre resource_type='mood_entry' con patient_id del dueño. Diferido hasta que modulo Export exista en runtime.
5. **SaveChangesAsync requerido:** todos los handlers que generan AccessAudit (incluyendo SendReminderCommand y HandleWebhookUpdateCommand) invocan SaveChangesAsync explicitamente para garantizar persistencia inmediata antes de retornar. Si el INSERT de audit falla, la operacion completa falla (fail-closed).
6. **Retencion:** AccessAudit >= 2 anos. tabla append-only sin TTL ni auto-expiracion. Archivo regulatorio bajo Ley 25.326.

### Operaciones que generan audit

| Operacion | action_type | resource_type | Implementado |
|-----------|-------------|---------------|--------------|
| Crear MoodEntry | create | mood_entry | Wave 1 |
| Crear DailyCheckin | create | daily_checkin | Wave 1 |
| Actualizar DailyCheckin del mismo dia | update | daily_checkin | Wave 1 |
| Leer consentimiento actual | read | consent_grant | Wave 1 |
| Bloqueo por falta de consentimiento en writes clinicos | read | consent_grant (`outcome=denied`) | Wave 1 |
| Otorgar consent | grant | consent_grant | Wave 1 |
| Revocar consent | revoke | consent_grant | Wave 1 |
| Emitir PendingInvite | create | pending_invite | Wave 30 |
| Generar BindingCode | create | binding_code | Wave 30 |
| Crear CareLink | create | care_link | Wave 30 |
| Revocar CareLink | revoke | care_link | Wave 30 |
| Profesional lee dashboard | read | care_link | Wave 30 |
| Profesional lee summary/alerts | read | mood_entry | Wave 30 |
| Export CSV | export | mood_entry | Wave 1 |
| Vincular Telegram session | create | telegram_session | Phase 31 |
| Desvincular Telegram session | revoke | telegram_session | Phase 31 |
| Recordatorio Telegram enviado (sin acceso a datos) | create | reminder_config | Phase 31 |

> Las filas desde `PendingInvite` hacia abajo son contrato canonico. Las marcadas "Implementado" ya tienen auditoria activa en runtime.

## Pseudonimizacion (T3-8)

```text
pseudonym_id = SHA256(actor_id + env_salt)
```

- `env_salt` se carga desde variable de entorno `BITACORA_PSEUDONYM_SALT`.
- Si `BITACORA_PSEUDONYM_SALT` no existe → fail-closed (`PSEUDONYM_SALT_MISSING`, 500).
- **No existe tabla de mapeo** pseudonym_id → actor_id en la capa operacional.
- actor_id solo aparece en AccessAudit (acceso restringido).
- Los logs operacionales y la telemetria deben usar `pseudonym_id`; en `Wave 1` esto ya aplica al audit de negocio y queda pendiente endurecerlo sobre toda la salida estructurada del proceso.

## Operacion Telegram: requisitos de auditoria

El modulo Telegram tiene un patron de auditoria especifico por su naturaleza asincronica y por fuera del flujo HTTP clasico:

1. **Webhook Telegram genera audit con action_type='create' y resource_type='telegram_session'** al vincularse exitosamente. El audit usa el `trace_id` generado o propagado por TraceIdMiddleware.
2. **ReminderWorker genera audit de tipo 'create' sobre 'reminder_config'** cada vez que se dispara un recordatorio. No accede a datos clinicos; el audit solo registra que se envio un recordatorio sin contenido.
3. **ConsentRequiredMiddleware dentro del flujo Telegram:** si el paciente intenta registrar via Telegram sin consentimiento, se genera AccessAudit con outcome=Denied y resource_type='consent_grant'. El bot recibe un mensaje de redireccion a la web.
4. **Logs del modulo Telegram siguen la regla de no-fuga:** nunca incluyen `encrypted_payload`, `safe_projection` con datos clinicos, ni contenido de `encrypted_payload`. Solo `chat_id` (si sesion vinculada) y `trace_id`.

## Gate actual implementado

Implementado hoy como combinacion de middleware + handlers:
1. `TraceIdMiddleware` garantiza `trace_id`.
2. `ConsentRequiredMiddleware` bloquea `POST /api/v1/mood-entries` y `POST /api/v1/daily-checkins` sin consentimiento activo.
3. Los handlers de `Consent` y `Registro` persisten `AccessAudit` dentro de la misma unidad de trabajo.
4. Si el write de audit falla, la operacion completa falla (fail-closed).

La verificacion profesional basada en `CareLink.can_view_data` sigue diferida hasta que exista ese modulo en runtime.

## Sync gates

Cambios en audit fuerzan revision de:
- RF-SEC-001..003
- RF-VIS-014 (audit en dashboard)
- 07_baseline_tecnica.md si cambia pseudonimizacion
