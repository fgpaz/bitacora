# 09 — Contratos Tecnicos

## Autenticacion

| Campo | Valor |
|-------|-------|
| Provider | Supabase Auth (GoTrue) |
| Instancia | auth.tedi.nuestrascuentitas.com |
| Metodos | Magic Link (primario), Google OAuth |
| Validacion JWT | Clave simetrica (`Supabase:JwtSecret` o env `Supabase__JwtSecret` / `SUPABASE_JWT_SECRET`) |
| Header | `Authorization: Bearer <access_token>` |
| Claims minimos | `sub`, `email`, `iat`, `exp` |
| Claim mapping | `MapInboundClaims=false`; fallback a `ClaimTypes.NameIdentifier` para `sub` |
| Resolucion | `JWT.sub -> User.supabase_user_id -> User.user_id + role` |

> Detalle: `09_contratos/CT-AUTH.md`

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

### Endpoints operacionales

| Metodo | Ruta | Uso | Descripcion |
|--------|------|-----|-------------|
| GET | /health | Liveness | Confirma proceso HTTP vivo |
| GET | /health/ready | Readiness | Valida config critica y conectividad PostgreSQL antes de abrir trafico |

### Superficie canonica diferida

| Metodo | Ruta | RF | Descripcion |
|--------|------|----|-------------|
| GET | /api/v1/mood-entries | RF-VIS-001 | Timeline del paciente |
| GET | /api/v1/daily-checkins | RF-VIS-002 | Consultar factores diarios |
| POST | /api/v1/care-links/bind | RF-VIN-012 | Auto-vincularse con BindingCode |
| POST | /api/v1/care-links/{id}/accept | RF-VIN-003 | Aceptar invitacion existente |
| DELETE | /api/v1/care-links/{id} | RF-VIN-020 | Revocar vinculo |
| PATCH | /api/v1/care-links/{id} | RF-VIN-023 | Habilitar o deshabilitar `can_view_data` |
| POST | /api/v1/telegram/pairing | RF-TG-001 | Generar codigo de vinculacion Telegram |
| GET | /api/v1/export/csv | RF-EXP-001 | Export CSV |

## Convenciones de contrato

- `patient_ref` es un identificador opaco de API; no es una columna persistida.
- `GET /api/v1/consent/current` requiere JWT de paciente.
- `POST /api/v1/auth/bootstrap` recibe `invite_token` por query string (`?invite_token=...`).
- Los errores usan envelope comun con `trace_id`.
- `GET /health/ready` responde `503` si falta `ConnectionStrings:BitacoraDb`, `SUPABASE_JWT_SECRET`, clave de cifrado valida, salt o conectividad DB.
- Los writes de consentimiento y registro fallan cerrado si la auditoria no puede persistirse.
- La revocacion de consentimiento hoy solo opera sobre `ConsentGrant`; las cascadas sobre vinculos y caches siguen diferidas.
- El bus de eventos permanece en `NoOp` mientras `EventBusSettings:HostAddress` siga vacio.

### Campos de respuesta hoy consumidos por frontend

- `POST /api/v1/auth/bootstrap` retorna `userId`, `status`, `needsConsent` y `resumePendingInvite`.
- `GET /api/v1/consent/current` retorna `version`, `text`, `sections` y `patientStatus`.
- `POST /api/v1/consent` retorna `consentGrantId`, `status`, `grantedAtUtc`, `needsFirstEntry` y `resumePendingInvite`.

## Patron de errores

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

| Doc | Tema |
|-----|------|
| `09_contratos/CT-AUTH.md` | Flujo completo Supabase Auth, JWT, session revocation |
| `09_contratos/CT-AUDIT.md` | Audit log, pseudonimizacion, trace_id, fail-closed |
| `09_contratos/CT-ERRORS.md` | Catalogo de errores tipados reutilizados |

## Sync gates

Cambios en 09 pueden forzar revision de:
- `04_RF/*` si cambian endpoints o error codes
- `07_baseline_tecnica.md` si cambia auth o deploy
- Frontend si cambian rutas o contratos de respuesta

---

*Fuente: `.docs/wiki/02_arquitectura.md`, `.docs/wiki/04_RF.md`*
