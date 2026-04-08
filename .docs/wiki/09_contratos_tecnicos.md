# 09 — Contratos Tecnicos

## Autenticacion

| Campo | Valor |
|-------|-------|
| Provider | Supabase Auth (GoTrue) |
| Instancia | auth.tedi.nuestrascuentitas.com (compartida con multi-tedi) |
| Metodos | Magic Link (primario), Google OAuth |
| Validacion JWT | Clave simetrica (Supabase__JwtSecret) |
| Header | `Authorization: Bearer <access_token>` |
| Resolucion | JWT.sub → User.supabase_user_id → User.user_id + role |

> Detalle: `09_contratos/CT-AUTH.md`

## API Surface

### Endpoints publicos (paciente autenticado)

| Metodo | Ruta | RF | Descripcion |
|--------|------|------|-------------|
| POST | /api/v1/auth/bootstrap | RF-ONB-001 | Crear/resolver User desde JWT |
| GET | /api/v1/consent/current | RF-CON-001 | Texto de consentimiento vigente |
| POST | /api/v1/consent | RF-CON-002 | Aceptar consentimiento |
| DELETE | /api/v1/consent/current | RF-CON-010 | Revocar consentimiento |
| POST | /api/v1/mood-entries | RF-REG-001 | Registrar humor |
| GET | /api/v1/mood-entries | RF-VIS-001 | Timeline (safe_projection) |
| POST | /api/v1/daily-checkins | RF-REG-020 | Registrar factores diarios |
| GET | /api/v1/daily-checkins | RF-VIS-002 | Consultar factores |
| POST | /api/v1/care-links/bind | RF-VIN-012 | Auto-vincular con profesional |
| POST | /api/v1/care-links/{id}/accept | RF-VIN-003 | Aceptar invitacion |
| DELETE | /api/v1/care-links/{id} | RF-VIN-020 | Revocar vinculo |
| PATCH | /api/v1/care-links/{id} | RF-VIN-023 | Activar can_view_data |
| POST | /api/v1/telegram/pairing | RF-TG-001 | Generar codigo de vinculacion TG |
| GET | /api/v1/export/csv | RF-EXP-001 | Export CSV |

### Endpoints profesional (autenticado, role=professional)

| Metodo | Ruta | RF | Descripcion |
|--------|------|------|-------------|
| POST | /api/v1/care-links | RF-VIN-001 | Invitar paciente |
| POST | /api/v1/professional/binding-codes | RF-VIN-010 | Generar codigo de vinculacion |
| GET | /api/v1/professional/dashboard | RF-VIS-010 | Dashboard multi-paciente |

### Endpoints sistema (webhook)

| Metodo | Ruta | RF | Descripcion |
|--------|------|------|-------------|
| POST | /api/v1/telegram/webhook | RF-REG-010 | Webhook de Telegram |

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

### Codigos de error globales

| Codigo | HTTP | Trigger |
|--------|------|---------|
| CONSENT_REQUIRED | 403 | Registro sin ConsentGrant granted |
| HEALTH_PROFILE_ACCESS_DENIED | 403 | Profesional sin can_view_data o CareLink inactivo |
| INVALID_SCORE | 422 | mood_score fuera de -3..+3 |
| INVALID_CODE | 404 | Codigo de vinculacion invalido o expirado |
| ENCRYPTION_KEY_MISSING | 500 | Clave de cifrado no disponible (fail-closed) |
| AUDIT_WRITE_FAILED | 500 | Fallo al escribir audit (fail-closed, no retorna datos) |
| PSEUDONYM_SALT_MISSING | 500 | Salt de pseudonimizacion no disponible |
| SESSION_NOT_LINKED | 404 | TelegramSession no vinculada |
| CARELINK_EXISTS | 409 | CareLink ya existe entre profesional y paciente |
| CONSENT_VERSION_MISMATCH | 409 | Version de consentimiento obsoleta |

## Versionado

- API version: `v1` en URL path
- Consent version: string semantica (ej: "1.0")
- Encryption key_version: int monotonically increasing

## Detail docs

| Doc | Tema |
|-----|------|
| `09_contratos/CT-AUTH.md` | Flujo completo Supabase Auth, JWT, session revocation |
| `09_contratos/CT-AUDIT.md` | Audit log, pseudonimizacion, trace_id, fail-closed |

## Sync gates

Cambios en 09 pueden forzar revision de:
- `04_RF/*` si cambian endpoints o error codes
- `07_baseline_tecnica.md` si cambia auth o deploy
- Frontend (Next.js) si cambian rutas o contratos de respuesta

---

*Fuente: `.docs/wiki/02_arquitectura.md`, `.docs/wiki/04_RF.md`*
