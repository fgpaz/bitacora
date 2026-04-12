# CT-VINCULOS: Vinculacion de Profesionales y Pacientes

> Root: `09_contratos_tecnicos.md` — seccion API Surface / Endpoints de Vinculos (Wave 30).
> **Estado: PRODUCCION (Wave 30 + Phase 31).** Todos los endpoints enumerados abajo existen en runtime.
> Contrato canonico diferido documentado al final.

---

## Objetivo del contrato

Definir la superficie publica de la API para la vinculacion entre profesionales de salud y pacientes, incluyendo invitaciones profesionales, binding codes, y la gestion de visibilidad de datos clinicos.

---

## Paciente — Endpoints implementados

### GET /api/v1/vinculos

Lista todos los vinculos (en cualquier estado) del paciente autenticado.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient) |
| Consent gate | Requiere `ConsentGrant.status=granted` |
| Estado | **Implementado** |

**Response 200:**

```json
{
  "vinculos": [
    {
      "careLinkId": "uuid",
      "professionalId": "uuid",
      "professionalDisplayName": "string",
      "patientId": "uuid",
      "status": "Invited|Active|RevokedByPatient|RevokedByConsent|Rejected",
      "canViewData": false,
      "invitedAt": "timestamp",
      "acceptedAt": "timestamp|null",
      "revokedAt": "timestamp|null"
    }
  ]
}
```

---

### GET /api/v1/vinculos/active

Lista vinculos activos con `can_view_data=true`, es decir, aquellos donde el profesional tiene permiso de ver datos.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient) |
| Consent gate | Requiere `ConsentGrant.status=granted` |
| Estado | **Implementado** |

**Response 200:**

```json
{
  "vinculos": [
    {
      "careLinkId": "uuid",
      "professionalId": "uuid",
      "professionalDisplayName": "string",
      "patientId": "uuid",
      "status": "Active",
      "canViewData": true,
      "invitedAt": "timestamp",
      "acceptedAt": "timestamp",
      "revokedAt": null
    }
  ]
}
```

---

### POST /api/v1/vinculos/accept

Auto-vinculacion del paciente mediante un `BindingCode`. El codigo es consumido en la misma transaccion que crea el `CareLink` directamente en estado `active` con `can_view_data=false`.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient) |
| Consent gate | Requiere `ConsentGrant.status=granted` |
| Estado | **Implementado** |

**Request:**

```json
{
  "bindingCode": "string (formato BIT-XXXXX)"
}
```

**Response 201:**

```json
{
  "careLinkId": "uuid",
  "status": "active",
  "canViewData": false,
  "acceptedAtUtc": "timestamp",
  "isNewLink": true
}
```

**Errores tipados:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| INVALID_BODY | 400 | Cuerpo JSON ausente o invalido |
| BINDING_CODE_REQUIRED | 400 | `bindingCode` vacio o ausente |
| BINDING_CODE_INVALID_OR_EXPIRED | 410 | BindingCode no existe, usado, o vencido |
| CARE_LINK_ALREADY_EXISTS | 409 | Ya existe CareLink activo con el profesional |
| ACCEPT_CARE_LINK_FAILED | 500 | Fallo transaccional |

---

### DELETE /api/v1/vinculos/{id}

Revoca un vinculo existente. Solo el paciente owner puede invocar este endpoint.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient owner) |
| Consent gate | Requiere `ConsentGrant.status=granted` |
| Estado | **Implementado** |

**Request:**

```json
{
  "confirmed": true
}
```

**Response 200:**

```json
{
  "careLinkId": "uuid",
  "status": "revoked_by_patient",
  "revokedAtUtc": "timestamp"
}
```

**Errores tipados:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| INVALID_BODY | 400 | Cuerpo JSON ausente o invalido |
| CONFIRMED_FALSE | 422 | `confirmed != true` |
| CARE_LINK_ID_REQUIRED | 400 | `careLinkId` vacio |
| CARE_LINK_NOT_FOUND | 404 | CareLink inexistente |
| NOT_YOUR_CARE_LINK | 403 | Actor no es owner del vinculo |
| CARE_LINK_NOT_REVOKABLE | 422 | CareLink no esta en estado `invited` o `active` |
| REVOKE_CARE_LINK_FAILED | 500 | Fallo transaccional |

---

### PATCH /api/v1/vinculos/{id}/view-data

Actualiza `can_view_data` sobre un vinculo activo. Solo el paciente owner puede invocar este endpoint. Solo opera sobre `CareLink` en estado `active`.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient owner) |
| Consent gate | Requiere `ConsentGrant.status=granted` del paciente |
| Estado | **Implementado** |

**Request:**

```json
{
  "canViewData": true|false
}
```

**Response 200:**

```json
{
  "careLinkId": "uuid",
  "canViewData": true|false
}
```

**Errores tipados:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| INVALID_BODY | 400 | Cuerpo JSON ausente o invalido |
| CARE_LINK_NOT_FOUND | 404 | CareLink inexistente |
| NOT_YOUR_CARE_LINK | 403 | Actor no es owner del vinculo |
| CARE_LINK_NOT_REVOKABLE | 422 | CareLink no esta en estado `active` |

---

## Profesional — Endpoints implementados

### POST /api/v1/professional/invites

Emite una invitacion de vinculo para un paciente identificado por su `emailHash` (SHA256 del email normalizado). Si el paciente existe, crea un `CareLink` en estado `invited`. Si no existe, crea un `PendingInvite` con TTL de 7 dias.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (professional) |
| Consent gate | Requiere `ConsentGrant.status=granted` del profesional |
| Estado | **Implementado** |

**Request:**

```json
{
  "emailHash": "string (sha256 hex del email normalizado)"
}
```

**Response 201:**

```json
{
  "resourceType": "care_link|pending_invite",
  "resourceId": "uuid",
  "status": "invited|issued",
  "expiresAt": "timestamp|null"
}
```

**Errores tipados:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| INVALID_BODY | 400 | Cuerpo JSON ausente o invalido |
| FORBIDDEN | 403 | Actor no es profesional |
| CARELINK_EXISTS | 409 | Ya existe CareLink activo o invitado con el paciente |
| PENDING_INVITE_EXISTS | 409 | Ya existe PendingInvite vigente para el mismo email hash |

---

### GET /api/v1/professional/patients

Lista todos los pacientes vinculados al profesional autenticado (con CareLink en cualquier estado).

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (professional) |
| Consent gate | Requiere `ConsentGrant.status=granted` del profesional |
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

**Nota:** no incluye pacientes que solo tengan `PendingInvite` pendiente sin CareLink creado.

---

## Invariantes de Autorizacion (implementadas)

1. Solo el **patient owner** puede listar sus vinculos (`GET /vinculos`, `GET /vinculos/active`).
2. Solo el **patient owner** puede aceptar un vinculo mediante binding code (`POST /vinculos/accept`).
3. Solo el **patient owner** puede revocar un vinculo (`DELETE /vinculos/{id}`).
4. Solo el **patient owner** puede modificar `can_view_data` (`PATCH /vinculos/{id}/view-data`).
5. Solo el **profesional owner** puede emitir invitaciones (`POST /professional/invites`) y listar sus pacientes (`GET /professional/patients`).
6. `can_view_data=false` es el valor por defecto y el unico valor aceptable en creacion (invariante T3-11).
7. Binding codes son de uso unico; se invalidan tras consumo exitoso en la misma transaccion.

---

## Invariantes de Compliance (implementadas)

1. **Consent como gate:** toda operacion de vinculo requiere consentimiento vigente del actor.
2. **No fuga de datos clinicos:** ninguna respuesta a un profesional con `can_view_data=false` puede contener `safe_projection` con datos clinicos del paciente.
3. **Audit:** toda operacion de vinculo genera `AccessAudit` con `action_type` correspondiente (`create`, `update`, `revoke`).
4. **Pseudonimizacion:** `actor_id` del profesional/paciente nunca aparece fuera de `AccessAudit`.

---

## Superficie Canonica Diferida

| Endpoint | Estado | Notas |
|----------|--------|-------|
| POST /api/v1/professional/invites | **Implementado** | Profesional emite invitacion por email (hash); crea PendingInvite |
| GET /api/v1/professional/patients | **Implementado** | Lista pacientes vinculados |
| POST /api/v1/care-links/{id}/accept (aceptacion via BindingCode del lado paciente) | **Flujo via binding code** | El paciente acepta via `POST /vinculos/accept` con BindingCode; la aceptacion directa de invitation pendiente aun no existe |
| Generar BindingCode (lado profesional) | Diferido | No existe endpoint para que el profesional emita un BindingCode |
| DELETE /api/v1/vinculos/{id} (profesional) | Diferido | Solo el paciente owner puede revocar |
| POST /api/v1/telegram/pairing | **Implementado** (Telegram) | Vinculacion Telegram |

---

## Delta entre contrato congelado y realizacion

| Aspecto | Contrato congelado | Implementado (Wave 30+) |
|---------|-------------------|------------------------|
| Ruta base paciente | `/api/v1/care-links` | `/api/v1/vinculos` |
| Accept por binding code | `POST /api/v1/care-links/bind` | `POST /api/v1/vinculos/accept` |
| Revocar vinculo (paciente) | `DELETE /api/v1/care-links/{id}` | `DELETE /api/v1/vinculos/{id}` (requiere body `{"Confirmed": true}`) |
| Gestionar visibilidad (paciente) | `PATCH /api/v1/care-links/{id}` | `PATCH /api/v1/vinculos/{id}/view-data` (requiere body `{"CanViewData": bool}`) |
| Emitir invitacion profesional | `POST /api/v1/care-links` | `POST /api/v1/professional/invites` (usa `EmailHash` no email plano) |
| Aceptar invitacion profesional (paciente) | `POST /api/v1/care-links/{id}/accept` | No implementado; flujo via BindingCode separado |
| Lista pacientes profesional | No definido | `GET /api/v1/professional/patients` |
| Generar BindingCode (profesional) | No expuesto en API publica | No implementado |

---

## Sincronizacion

Cambios en este contrato fuerzan revision de:
- `09_contratos_tecnicos.md` (seccion API Surface)
- `04_RF/RF-VIN-*`
- `03_FL/FL-VIN-*`
- `07_baseline_tecnica.md` si cambia auth o compliance