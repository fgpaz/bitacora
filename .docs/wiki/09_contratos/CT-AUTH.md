# CT-AUTH: Autenticacion y Autorizacion

> Root: `09_contratos_tecnicos.md` — seccion Autenticacion y API Surface.
> Este contrato freeze la superficie activa y la superficie diferida.

---

## Superficie activa (runtime actual)

### POST /api/v1/auth/bootstrap

| Campo | Detalle |
|-------|----------|
| Autenticacion | JWT Bearer via Supabase Auth |
| Parametro de query | `invite_token` opcional (string) |
| Handler | `AuthEndpoints.cs` + `BootstrapPatientCommand` |
| Consent gate | No requiere consentimiento previo |

**Request:**

```
POST /api/v1/auth/bootstrap?invite_token=<opcional>
Authorization: Bearer <jwt>
```

**Response 200:**

```json
{
  "userId": "uuid",
  "status": "registered|active",
  "needsConsent": true|false",
  "resumePendingInvite": true|false"
}
```

**Errores tipados:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| ONB_001_JWT_INVALID | 401 | JWT sin claims minimos (`sub`, `email`) |
| ONB_001_ENCRYPT_FAILED | 500 | Fallo al crear usuario local de forma segura |
| PATIENT_NOT_FOUND | 404 | JWT resolve a user local inexistente |

**Invariantes:**

- JWT se valida por clave simetrica (`SUPABASE_JWT_SECRET` / `Supabase__JwtSecret`).
- `sub` -> `User.supabase_user_id` -> `User.user_id` + `role`.
- Si `User.sessions_revoked_at` existe y `jwt.iat` < `sessions_revoked_at` -> rechazo.
- Clock skew tolerado: 30 segundos.
- El `invite_token` se procesa en bootstrap; si es valido y pertenece a un `PendingInvite`, se materializa el `CareLink` resultante.

---

## Superficie profesional activa

### POST /api/v1/professional/invites

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (professional) |
| Consent gate | Requiere consentimiento vigente del profesional |
| Handler | `VinculosEndpoints.cs` + `CreatePendingInviteCommand` |
| Estado | **Implementado** |

**Request:**

```json
{
  "emailHash": "sha256_hex"
}
```

**Response 201:**

```json
{
  "pendingInviteId": "uuid",
  "status": "issued",
  "expiresAt": "timestamp"
}
```

**Errores tipados:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| INVALID_BODY | 400 | Cuerpo JSON ausente o invalido |
| FORBIDDEN | 403 | Actor no es profesional |
| CARELINK_EXISTS | 409 | Ya existe vinculo activo o invitado con el paciente |

---

### GET /api/v1/professional/patients

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (professional) |
| Consent gate | Requiere consentimiento vigente del profesional |
| Handler | `VinculosEndpoints.cs` + `GetProfessionalPatientsQuery` |
| Estado | **Implementado** |

**Response 200:**

```json
{
  "patients": [
    {
      "patientId": "uuid",
      "displayName": "string",
      "careLinkId": "uuid",
      "status": "Active|Invited",
      "canViewData": false,
      "linkedAt": "timestamp|null"
    }
  ]
}
```

**Nota:** solo retorna pacientes con `CareLink` activos o invitados para ese profesional. No incluye pacientes que solo tengan `PendingInvite` pendiente.

---

## Superficie diferida (no existe en runtime actual)

### POST /api/v1/care-links

| Campo | Detalle |
|-------|----------|
| Autenticacion | JWT Bearer (professional) |
| Consent gate | Requiere consentimiento vigente |
| Estado | **Diferido** — modulo VIN no existe en runtime |

**Request (futuro):**

```json
{
  "patient_email": "string"
}
```

**Response 201 (futuro):**

```json
{
  "resource_type": "care_link|pending_invite",
  "resource_id": "uuid",
  "status": "invited|issued",
  "expires_at": "timestamp"
}
```

**Errores tipados diferidos:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| CARELINK_EXISTS | 409 | Ya existe vinculo activo o invitado |
| PENDING_INVITE_EXISTS | 409 | Ya existe PendingInvite vigente |
| FORBIDDEN | 403 | Actor no es professional |

---

## Modelo de Autorizacion

### Roles activos

| Role | Descripcion | Resolucion |
|------|-------------|-----------|
| patient | Paciente registrado | `User.role = patient` |
| professional | Profesional de salud | `User.role = professional` |

### Resolucion de contexto

```
JWT.sub (supabase_user_id)
  -> User.supabase_user_id
  -> User.user_id + role
  -> patient_id | professional_id
```

### Middleware activos

| Middleware | Gate | Ubicacion |
|-----------|------|-----------|
| `ConsentRequiredMiddleware` | `POST /mood-entries`, `POST /daily-checkins` sin consentimiento -> 403 | `src/.../Middleware/` |
| `CurrentAuthenticatedPatientResolver` | Resuelve paciente autenticado desde JWT | `src/.../Security/` |
| `ApiExceptionMiddleware` | Maneja `BitacoraException` y errores inesperados | `src/.../Middleware/` |

---

### Rate limiting policy

| Contexto | Policy | Limite | Aplicado a |
|----------|--------|--------|------------|
| Auth bootstrap | `auth` | 10 req/IP/min | `POST /api/v1/auth/bootstrap` |
| Clinical writes | `write` | 30 req/IP/min | `POST /mood-entries`, `POST /daily-checkins` |
| Health/ready | `auth` | 10 req/IP/min | `GET /health`, `GET /health/ready` |

El rate limiter esta registrado antes del health check endpoint en Program.cs, por lo que `/health/ready` queda sujeto a la politica `auth`. Exceso de limite retorna 429 + `Retry-After` header.

---

## Consentimiento como gate de autorizacion

- `POST /api/v1/auth/bootstrap` **no** requiere consentimiento; crea o reactiva el usuario.
- `GET /api/v1/consent/current` — cualquier JWT valido puede leer el texto.
- `POST /api/v1/consent` — cualquier JWT valido puede otorgar.
- `POST /api/v1/mood-entries`, `POST /api/v1/daily-checkins` — requieren `ConsentGrant.status=granted`.
- `POST /api/v1/telegram/pairing` — requiere `ConsentGrant.status=granted`.
- Los endpoints de vinculo, visualizacion profesional y export son **future contracts** que heredan este gate.

---

## Rutas paciente implementadas (frontend)

| Ruta | Componente | Descripcion |
|-----|------------|-------------|
| `/onboarding` | `OnboardingFlow` | flujo bootstrap -> consent -> bridge |
| `/consent` | `ConsentGatePanel` | lectura y otorgamiento de consentimiento |

## Auth instance de Bitácora (target architecture)

**Instancia dedicada:** `auth.bitacora.nuestrascuentitas.com` (GoTrue `supabase/gotrue:v2.177.0`)
**Estado:** pendiente de deploy — ver plan en `.docs/raw/investigacion/2026-04-14-auth-misconfiguration.md`

**Invariante objetivo:**
- Frontend: `NEXT_PUBLIC_SUPABASE_URL=https://auth.bitacora.nuestrascuentitas.com`
- Backend: `SUPABASE_JWT_SECRET=<secret de auth.bitacora>` (mismo secreto que la instancia GoTrue)
- Los JWT emitidos por `auth.bitacora.nuestrascuentitas.com` son validados correctamente por el backend.

**GAP actual (2026-04-14 — pendiente de cierre):**
El frontend usa `auth.tedi.nuestrascuentitas.com` (multi-tedi, JWT secret diferente al del backend).
Bloquea flujo web completo para usuarios reales. Fix: deploy de `auth.bitacora` + actualizacion de
env vars en frontend y backend + redeploy.

Ver investigacion completa: `.docs/raw/investigacion/2026-04-14-auth-misconfiguration.md`

---

## Sincronizacion

Cambios en este contrato fuerzan revision de:
- `09_contratos_tecnicos.md` (seccion Autenticacion y API Surface)
- `04_RF/RF-ONB-001`, `RF-ONB-002`
- `07_baseline_tecnica.md` si cambia provider o metodo de validacion
- Frontend si cambian claims, metodos de auth o flujo de bootstrap
- `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` (rutas paciente y componentes)
