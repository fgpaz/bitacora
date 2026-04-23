# 09 — Contratos Tecnicos

## Autenticacion

| Campo | Valor |
|-------|-------|
| Provider | Zitadel self-hosted v4.9.0 |
| Instancia | `https://id.nuestrascuentitas.com` (IdP compartido Teslita) |
| Metodos | OIDC Authorization Code + PKCE en frontend; Bearer JWT RS256 en backend |
| Validacion JWT | Metadata OIDC + JWKS (`ZITADEL_AUTHORITY`, `ZITADEL_AUDIENCE`) |
| Header | `Authorization: Bearer <access_token>` |
| Claims minimos | `sub`, `iat`, `exp`, `iss`, `aud`; `email` puede venir en JWT o resolverse via OIDC UserInfo |
| Claim mapping | `MapInboundClaims=false`; roles desde `urn:zitadel:iam:org:project:roles` a `patient` / `professional` |
| Resolucion | `JWT.sub -> User.auth_subject -> User.user_id + role`; si no existe, link-on-first-login por `email_hash` |

> Detalle activo: `09_contratos/CT-AUTH-ZITADEL.md`.

## API Surface

### Endpoints implementados en Wave 1

| Metodo | Ruta | RF | Descripcion |
|--------|------|----|-------------|
| POST | /api/v1/auth/bootstrap | RF-ONB-001 | Crear o reutilizar `User` desde JWT; acepta `invite_token` opcional por query string |
| GET | /api/v1/consent/current | RF-CON-001 | Devolver consentimiento activo configurado y estado del paciente |
| POST | /api/v1/consent | RF-CON-002 | Otorgar consentimiento vigente |
| DELETE | /api/v1/consent/current | RF-CON-010 | Revocar consentimiento vigente |
| POST | /api/v1/mood-entries | RF-REG-001 | Registrar humor |
| POST | /api/v1/daily-checkins | RF-REG-020 | Registrar o actualizar factores diarios del mismo dia |
| POST | /api/v1/analytics/events | RF-ANA-001 | Persistir evento de UX impact (no-PII); whitelist `time_to_cta_ready` / `ctr_rail_vs_checkin` / `logout_accidental_rate` / `decline_consent_rate` |

### Endpoints operacionales

| Metodo | Ruta | Uso | Descripcion |
|--------|------|-----|-------------|
| GET | /health | Liveness | Confirma proceso HTTP vivo |
| GET | /health/ready | Readiness | Valida config critica y conectividad PostgreSQL antes de abrir trafico |

### Endpoints de Vinculos (Wave 30)

| Metodo | Ruta | RF | Descripcion |
|--------|------|----|-------------|
| GET | /api/v1/vinculos | RF-VIN-010 | Lista de vinculos del paciente autenticado |
| GET | /api/v1/vinculos/active | RF-VIN-010 | Lista de vinculos activos con permiso de vista |
| POST | /api/v1/vinculos/accept | RF-VIN-003 | Aceptar vinculo mediante BindingCode |
| DELETE | /api/v1/vinculos/{id} | RF-VIN-020 | Revocar vinculo |
| PATCH | /api/v1/vinculos/{id}/view-data | RF-VIN-023 | Habilitar o deshabilitar `can_view_data` |

### Superficie canonica diferida

| Metodo | Ruta | RF | Descripcion |
|--------|------|----|-------------|
| GET | /api/v1/mood-entries | RF-VIS-001 | Timeline del paciente |
| GET | /api/v1/daily-checkins | RF-VIS-002 | Consultar factores diarios |
| POST | /api/v1/care-links/bind | RF-VIN-012 | Auto-vincularse con BindingCode (endpoint profesional emite codigo) |
| POST | /api/v1/care-links | RF-VIN-001 | Emitir invitacion de vinculo |

### Endpoints profesionales implementados

| Metodo | Ruta | RF | Descripcion |
|--------|------|----|-------------|
| POST | /api/v1/professional/invites | RF-VIN-001 | Emitir invitacion de vinculo por email (hash) |
| GET | /api/v1/professional/patients | RF-VIN-010 | Lista pacientes vinculados al profesional |
| GET | /api/v1/professional/patients/{patientId}/summary | RF-VIS-010 | Resumen clinico del paciente para profesional |
| GET | /api/v1/professional/patients/{patientId}/timeline | RF-VIS-011 | Timeline del paciente para profesional |
| GET | /api/v1/professional/patients/{patientId}/alerts | RF-VIS-012 | Alertas del paciente para profesional |

### Endpoints Telegram

| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| DELETE | /api/v1/telegram/session | Bearer JWT (paciente) | Desvincula (soft delete) la sesion Telegram del paciente autenticado |
| GET | /api/v1/telegram/reminder-schedule | Bearer JWT (paciente) | Consulta el horario de recordatorio guardado; no expone `chat_id` |
| PUT | /api/v1/telegram/reminder-schedule | Bearer JWT (paciente) | Configura el horario de recordatorio del bot; recibe hora/minuto UTC convertidos desde la UI local |

**DELETE /api/v1/telegram/session**

- **Auth**: Bearer JWT (patient)
- **Rate limit**: write
- **Response 200**: `{ patientId: uuid, unlinkedAtUtc: ISO }`
- **Response 404**: `{ code: "TG_SESSION_NOT_FOUND" }`
- **Descripcion**: Desvincula (soft delete) la sesion Telegram del paciente autenticado. No elimina historico de pairing, solo marca como inactivo.

**PUT /api/v1/telegram/reminder-schedule**

- **Auth**: Bearer JWT (patient)
- **Rate limit**: write
- **Body**: `{ hourUtc: int(0-23), minuteUtc: int(0|30), timezone: string(IANA|Windows|null) }`
- **Contrato horario**: la UI paciente muestra hora local Buenos Aires; `frontend/lib/api/client.ts` convierte esa hora local a `hourUtc`/`minuteUtc` antes de enviar. Ejemplo: `22:00` Buenos Aires => `{ hourUtc: 1, minuteUtc: 0 }`.
- **Response 200**: `{ reminderConfigId: uuid, hourUtc: int, minuteUtc: int, reminderTimezone: string, enabled: bool, nextFireAtUtc: ISO }`
- **Response 400**: `{ code: "TG_006_INVALID_HOUR" }`, `{ code: "TG_006_INVALID_MINUTE" }`, `{ code: "TG_006_INVALID_TIMEZONE" }`
- **Response 403**: `{ code: "TG_006_NO_ACTIVE_SESSION" }`
- **Descripcion**: Configura el horario de recordatorio del bot Telegram. Persiste horario UTC y timezone de contexto local. Devuelve proximo disparo del recordatorio en UTC.

**GET /api/v1/telegram/reminder-schedule**

- **Auth**: Bearer JWT (patient)
- **Rate limit**: auth/default
- **Response 200 sin configuracion**: `{ configured: false, reminderConfigId: null, hourUtc: null, minuteUtc: null, reminderTimezone: null, enabled: null, nextFireAtUtc: null }`
- **Response 200 con configuracion**: `{ configured: true, reminderConfigId: uuid, hourUtc: int, minuteUtc: int, reminderTimezone: string, enabled: bool, nextFireAtUtc: ISO|null }`
- **Descripcion**: Consulta owner-only del recordatorio guardado para que la UI recargada pueda reconstruir la hora local Buenos Aires. No expone `chat_id`, `patient_id` ni payloads clinicos.

## Convenciones de contrato

- `patient_ref` es un identificador opaco de API; no es una columna persistida.
- `GET /api/v1/consent/current` requiere JWT de paciente.
- `POST /api/v1/auth/bootstrap` recibe `invite_token` por query string (`?invite_token=...`).
- Los errores usan envelope comun con `trace_id`.
- `GET /health/ready` responde `503` si falta `ConnectionStrings:BitacoraDb`, autoridad/audiencia/metadata Zitadel, clave de cifrado valida, salt o conectividad DB.
- Los writes de consentimiento y registro fallan cerrado si la auditoria no puede persistirse.
- La revocacion de consentimiento hoy solo opera sobre `ConsentGrant`; las cascadas sobre vinculos y caches siguen diferidas.
- El bus de eventos permanece en `NoOp` mientras `EventBusSettings:HostAddress` siga vacio.

### Invariantes de compliance transversales

1. **ConsentRequiredMiddleware** es el hard gate para POST /mood-entries y POST /daily-checkins; cualquier intento sin consentimiento activo genera 403 + AccessAudit outcome=Denied.
2. **AccessAudit.append-only**: la unica operacion valida sobre access_audits es INSERT. UPDATE y DELETE estan prohibidas a nivel de aplicacion y esquema.
3. **PseudonymizationService fail-closed**: si BITACORA_PSEUDONYM_SALT falta o no resuelve, toda operacion que dependa del servicio de pseudonimizacion retorna 500.
4. **Sin fuga a Telegram**: ninguna respuesta HTTP dirigida al bot Telegram ni ningun mensaje del bot puede contener encrypted_payload, safe_projection con datos clinicos, o cualquier campo derivable de registros clinicos del paciente.
5. **Export CSV es owner-only**: GET /api/v1/export/csv solo acepta JWT del paciente owner; no se serve a contextos profesionales aunque CareLink.can_view_data=true.
6. **Retencion resiliente**: crisis (mood_score=-3) >= 5 anos; AccessAudit >= 2 anos; ConsentGrant permanente.

### Campos de respuesta hoy consumidos por frontend

Los contratos de respuesta listados en esta seccion son consumidos por `frontend/` (existente bajo `frontend/`).

- `POST /api/v1/auth/bootstrap` retorna `userId`, `status`, `needsConsent` y `resumePendingInvite`.
- `GET /api/v1/consent/current` retorna `version`, `text`, `sections` y `patientStatus`.
- `POST /api/v1/consent` retorna `consentGrantId`, `status`, `grantedAtUtc`, `needsFirstEntry` y `resumePendingInvite`.

## Patron de errores

Los codigos activos para VIN, VIS, EXP y TG ya estan materializados en runtime. Ver `CT-ERRORS.md` para el catalogo completo por modulo.

Todas las respuestas de error siguen este envelope:

```json
{
  "error": {
    "code": "CONSENT_REQUIRED",
    "message": "Debes aceptar el consentimiento informado antes de registrar datos.",
    "trace_id": "550e8400-e29b-41d4-a716-446655440000"
  }
}
```

### Codigos transversales mas usados

| Codigo | HTTP | Trigger |
|--------|------|---------|
| UNAUTHORIZED | 401 | JWT ausente o claims obligatorios faltantes |
| FORBIDDEN | 403 | Actor autenticado fuera del rol esperado |
| PATIENT_NOT_FOUND | 404 | El JWT resuelve un usuario local inexistente |
| INVALID_BODY | 400 | Cuerpo JSON ausente o invalido para el endpoint |
| CONSENT_REQUIRED | 403 | Registro o vinculacion sin consentimiento activo |
| CONSENT_VERSION_MISMATCH | 409 | Version de consentimiento obsoleta |
| CONSENT_ALREADY_GRANTED | 409 | Consentimiento vigente ya otorgado |
| ACCEPTED_FALSE | 422 | Se intento otorgar consentimiento con `accepted=false` |
| CONFIRMED_FALSE | 422 | Se intento revocar consentimiento con `confirmed=false` |
| NO_ACTIVE_CONSENT | 404 | No existe consentimiento activo para revocar |
| CARELINK_EXISTS | 409 | Ya existe vinculo activo o invitado |
| BINDING_CODE_NOT_FOUND | 404 | BindingCode inexistente |
| BINDING_CODE_EXPIRED | 410 | BindingCode expirado |
| BINDING_CODE_ALREADY_USED | 409 | BindingCode ya consumido |
| SESSION_NOT_LINKED | 200 | Webhook Telegram sin sesion vinculada |
| TG_SESSION_NOT_FOUND | 404 | Sesion Telegram del paciente no existe |
| TG_006_NO_ACTIVE_SESSION | 403 | Paciente sin sesion Telegram activa para programar recordatorio |
| TG_006_INVALID_HOUR | 400 | `hourUtc` fuera de 0..23 |
| TG_006_INVALID_MINUTE | 400 | `minuteUtc` distinto de 0 o 30 |
| TG_006_INVALID_TIMEZONE | 400 | Zona horaria invalida o no soportada |
| AUDIT_WRITE_FAILED | 500 | Fallo al persistir `AccessAudit` |
| ENCRYPTION_FAILURE | 500 | Fallo de cifrado o clave no disponible |
| NO_CONSENT_CONFIG | 503 | No hay consentimiento activo configurado |
| INVALID_SCORE | 422 | `mood_score` fuera de rango |
| VALIDATION_ERROR | 422 | Error de validacion de `daily-checkins` |

> Catalogo ampliado y detalle por modulo: `09_contratos/CT-ERRORS.md`

## Versionado

- API version: `v1` en URL path
- Consent version: string semantica (ej: `1.0`)
- Encryption `key_version`: entero monotonicamente creciente
- BindingCode TTL preset: `15m` / `3h` / `24h` / `72h`
- Runtime target de T01: `api.bitacora.nuestrascuentitas.com`

## Detail docs

| Doc | Tema | Estado |
|-----|------|--------|
| `09_contratos/CT-AUTH-ZITADEL.md` | Flujo activo Zitadel OIDC PKCE, JWT RS256/JWKS, roles y cutover aceptado | Activo |
| `09_contratos/CT-AUDIT.md` | Audit log, pseudonimizacion, trace_id, fail-closed | Activo |
| `09_contratos/CT-ERRORS.md` | Catalogo de errores tipados con sections activas (VIN, VIS, EXP, TG) | Activo |
| `09_contratos/CT-VINCULOS.md` | Contrato de vinculos paciente y profesional, binding codes, estado production | Activo |
| `09_contratos/CT-VISUALIZACION-Y-EXPORT.md` | Contrato de visualizacion paciente y profesional, export CSV owner-only | Activo |
| `09_contratos/CT-TELEGRAM-RUNTIME.md` | Runtime Telegram: pairing, webhook /start, secreto, invariante no-fuga | Activo |

## Decision register

| Fecha | Decision | Doc |
|-------|----------|-----|
| 2026-04-10 | **Hardening exceptions register + go/no-go** | `.docs/raw/decisiones/2026-04-10-wave-prod-hardening-exceptions-register.md` |

## Sync gates

Cambios en 09 pueden forzar revision de:
- `04_RF/*` si cambian endpoints o error codes
- `07_baseline_tecnica.md` si cambia auth o deploy
- Frontend si cambian rutas o contratos de respuesta

## Hardening exceptions register

Ver `.docs/raw/decisiones/2026-04-10-wave-prod-hardening-exceptions-register.md` para el registro unico de excepciones aceptadas, atajos prohibidos y gates obligatorios antes de cada phase de codigo.

Este documento es el unico checkpoint que las fases `30`, `31`, `40`, `41`, `50` y `60` deben satisfacer antes de avanzar.

---

*Fuente: `.docs/wiki/02_arquitectura.md`, `.docs/wiki/04_RF.md`*

> Detalle IdP activo (Wave B 2026-04-19): `09_contratos/CT-AUTH-ZITADEL.md`.
