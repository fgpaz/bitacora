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

### Operaciones que generan audit

| Operacion | action_type | resource_type |
|-----------|-------------|---------------|
| Crear MoodEntry | create | mood_entry |
| Crear DailyCheckin | create | daily_checkin |
| Actualizar DailyCheckin del mismo dia | update | daily_checkin |
| Leer consentimiento actual | read | consent_grant |
| Bloqueo por falta de consentimiento en writes clinicos | read | consent_grant (`outcome=denied`) |
| Otorgar consent | grant | consent_grant |
| Revocar consent | revoke | consent_grant |
| Emitir PendingInvite | create | pending_invite | 
| Generar BindingCode | create | binding_code |
| Crear CareLink | create | care_link |
| Revocar CareLink | revoke | care_link |
| Profesional lee dashboard | read | care_link |
| Profesional lee summary/alerts | read | mood_entry |
| Export CSV | export | mood_entry |
| Vincular Telegram | create | telegram_session |

> Las filas desde `PendingInvite` hacia abajo siguen siendo contrato canonico, pero no estan implementadas todavia en el runtime actual.

## Pseudonimizacion (T3-8)

```text
pseudonym_id = SHA256(actor_id + env_salt)
```

- `env_salt` se carga desde variable de entorno `BITACORA_PSEUDONYM_SALT`.
- Si `BITACORA_PSEUDONYM_SALT` no existe → fail-closed (`PSEUDONYM_SALT_MISSING`, 500).
- **No existe tabla de mapeo** pseudonym_id → actor_id en la capa operacional.
- actor_id solo aparece en AccessAudit (acceso restringido).
- Los logs operacionales y la telemetria deben usar `pseudonym_id`; en `Wave 1` esto ya aplica al audit de negocio y queda pendiente endurecerlo sobre toda la salida estructurada del proceso.

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
