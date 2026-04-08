# CT-AUDIT: Auditoria, Pseudonimizacion y Fail-Closed

> Root: `09_contratos_tecnicos.md` errores globales, `07_baseline_tecnica.md` invariantes.

## Audit log (AccessAudit)

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
| Otorgar consent | grant | consent_grant |
| Revocar consent | revoke | consent_grant |
| Crear CareLink | create | care_link |
| Revocar CareLink | revoke | care_link |
| Profesional lee dashboard | read | safe_projection |
| Export CSV | export | mood_entry |
| Vincular Telegram | create | telegram_session |

## Pseudonimizacion (T3-8)

```text
pseudonym_id = SHA256(actor_id + env_salt)
```

- `env_salt` se carga desde variable de entorno `BITACORA_PSEUDONYM_SALT`.
- Si `BITACORA_PSEUDONYM_SALT` no existe → fail-closed (`PSEUDONYM_SALT_MISSING`, 500).
- **No existe tabla de mapeo** pseudonym_id → actor_id en la capa operacional.
- actor_id solo aparece en AccessAudit (acceso restringido).
- Todos los logs (Serilog), telemetria (OpenTelemetry), y metricas usan pseudonym_id.

## Interceptor de audit (RF-SEC-001)

Implementado como middleware/filter de ASP.NET que:
1. Detecta si el request es de un profesional accediendo a datos de paciente.
2. Verifica CareLink.can_view_data = true.
3. Si acceso denegado: retorna 403 sin revelar si el recurso existe.
4. Si acceso permitido: ejecuta la operacion + INSERT AccessAudit en la misma transaccion.
5. Si audit INSERT falla: rollback de todo (fail-closed).

## Sync gates

Cambios en audit fuerzan revision de:
- RF-SEC-001..003
- RF-VIS-014 (audit en dashboard)
- 07_baseline_tecnica.md si cambia pseudonimizacion
