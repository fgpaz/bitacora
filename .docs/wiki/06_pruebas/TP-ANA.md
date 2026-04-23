# TP-ANA — Test Plan modulo Analytics

## Scope

Validar el endpoint `POST /api/v1/analytics/events` (RF-ANA-001) y la persistencia de `analytics_events` con no-PII enforcement y whitelist de eventos.

## Prerrequisitos

- DB con tabla `analytics_events` creada (via SQL `infra/migrations/bitacora/20260423_create_analytics_events.sql`).
- JWT Zitadel valido para patient autenticado.
- Backend Api corriendo con endpoint registrado.

## Casos

### TP-ANA-001-01 — Registrar evento time_to_cta_ready con props validas

**Tipo:** Positivo

**Precondiciones:**
- Patient autenticado con JWT valido.

**Pasos:**
1. POST `/api/v1/analytics/events` con body:
   ```json
   {"event": "time_to_cta_ready", "props": {"source": "rail", "delta_ms": 1200}}
   ```

**Aceptacion:**
- HTTP 202 Accepted.
- Response JSON incluye `analyticsEventId` (UUID no-empty) y `createdAtUtc` (ISO UTC).
- DB: nuevo row en `analytics_events` con `event_name='time_to_cta_ready'`, `props_json` con los campos enviados, `patient_id` igual al del JWT, `created_at_utc` cercano al UtcNow.

### TP-ANA-001-02 — Registrar evento sin props (null)

**Tipo:** Positivo

**Precondiciones:**
- Patient autenticado con JWT valido.

**Pasos:**
1. POST `/api/v1/analytics/events` con body:
   ```json
   {"event": "ctr_rail_vs_checkin"}
   ```

**Aceptacion:**
- HTTP 202 Accepted.
- DB: `props_json` = NULL.

### TP-ANA-001-03 — Rechazar event_name fuera del whitelist

**Tipo:** Negativo

**Pasos:**
1. POST `/api/v1/analytics/events` con body:
   ```json
   {"event": "random_marketing_event"}
   ```

**Aceptacion:**
- HTTP 400.
- Error envelope con `code = "EVENT_NAME_UNKNOWN"`.
- DB: sin registros nuevos.

### TP-ANA-001-04 — Rechazar props_json > 2048 chars

**Tipo:** Negativo

**Pasos:**
1. POST `/api/v1/analytics/events` con body cuyo `props` serializado excede 2048 chars.

**Aceptacion:**
- HTTP 400.
- Error envelope con `code = "PROPS_TOO_LARGE"`.

### TP-ANA-001-05 — Rechazar body ausente

**Tipo:** Negativo

**Pasos:**
1. POST `/api/v1/analytics/events` sin body.

**Aceptacion:**
- HTTP 400.
- Error envelope con `code = "INVALID_BODY"` o `EVENT_NAME_REQUIRED` segun deserializacion.

### TP-ANA-001-06 — Rechazar sin JWT

**Tipo:** Negativo

**Pasos:**
1. POST `/api/v1/analytics/events` sin `Authorization: Bearer`.

**Aceptacion:**
- HTTP 401 (rate limiting policy `write` + auth middleware).

## Cobertura

- Endpoint accept/reject: todos.
- Validaciones de negocio: whitelist, longitud props.
- Auth: sin JWT / JWT invalido (cobertura por middleware).
- PII enforcement: **NO se valida en runtime** (trust del caller); auditoría manual por code review.

## Notas

- Retention policy (180d sugerido) es follow-up operacional via cron task; fuera de scope funcional.
- La persistencia es append-only; UPDATE/DELETE no estan expuestos por diseno.
