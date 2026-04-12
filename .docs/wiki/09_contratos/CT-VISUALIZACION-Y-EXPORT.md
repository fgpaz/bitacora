# CT-VISUALIZACION-Y-EXPORT: Visualizacion y Exportacion de Datos

> Root: `09_contratos_tecnicos.md` — seccion Superficie canonica diferida.
> **Estado: IMPLEMENTADO EN PHASE 31+ (parcial).** Los endpoints `timeline`, `summary`, `alerts` del paciente y profesionales estan materializados. La exportacion CSV streaming sigue diferida. Export para profesionales es owner-only y no esta permitido.

---

## Objetivo del contrato

Definir la superficie publica de la API para la lectura del timeline del paciente, consulta de factores diarios, visualizacion profesional (resumen y alertas), y exportacion CSV.

---

## Visualizacion del Paciente — Implementacion Phase 31

### GET /api/v1/visualizacion/timeline

Timeline combinado (mood entries + daily check-ins) del paciente para un rango de fechas.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient) |
| Consent gate | Requiere `ConsentGrant.status=granted` |
| Filtro global | `patient_id` del contexto autenticado |
| Estado | **Implementado** |

**Query params:**

| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| from | date (ISO8601) | Si | Inicio del rango |
| to | date (ISO8601) | Si | Fin del rango |

**Response 200:**

```json
{
  "days": [
    {
      "date": "2026-04-01",
      "mood_entry": {
        "mood_entry_id": "uuid",
        "score": 2,
        "created_at": "2026-04-01T10:00:00Z"
      },
      "daily_checkin": {
        "daily_checkin_id": "uuid",
        "date": "2026-04-01",
        "sleep_hours": 7.5,
        "physical_activity": true,
        "social_activity": false,
        "anxiety": true,
        "irritability": false,
        "medication_taken": true
      }
    }
  ]
}
```

**Restricciones implementadas:**

- `from > to` -> `400 INVALID_DATE_RANGE` (BitacoraException).
- Rango sin datos -> `{ "days": [] }` (no 404).
- Proyeccion: solo `safe_projection` (sin datos clinicos sensibles).

**Errores tipados:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| INVALID_DATE_RANGE | 400 | `from > to` |
| UNAUTHORIZED | 401 | JWT invalido o expirado |

---

### GET /api/v1/visualizacion/summary

Resumen agregado de mood y check-ins para un rango de fechas.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient) |
| Consent gate | Requiere `ConsentGrant.status=granted` |
| Filtro global | `patient_id` del contexto autenticado |
| Estado | **Implementado** |

**Query params:**

| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| from | date (ISO8601) | Si | Inicio del rango |
| to | date (ISO8601) | Si | Fin del rango |

**Response 200:**

```json
{
  "patient_id": "uuid",
  "from": "2026-03-01",
  "to": "2026-04-01",
  "total_days": 32,
  "days_with_mood": 28,
  "days_with_checkin": 25,
  "avg_mood": 1.5,
  "avg_sleep_hours": 6.8,
  "anxiety_days": 12,
  "irritability_days": 8,
  "medication_taken_days": 20
}
```

**Restricciones implementadas:**

- `from > to` -> `400 INVALID_DATE_RANGE`.

---

## Visualizacion Profesional — Implementada

> **Invariante:** profesionales solo ven datos si existe `CareLink` con `professional_id` del contexto Y `can_view_data=true`. Esto se enforce via `ProfessionalDataAccessAuthorizer`.

### GET /api/v1/professional/patients/{patientId}/summary

Resumen clinico del paciente para el profesional autenticado.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (professional) |
| Autorizacion | `CareLink.can_view_data=true` para el patientId |
| Estado | **Implementado** |

**Query params:**

| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| from | date (ISO8601) | Si | Inicio del rango |
| to | date (ISO8601) | Si | Fin del rango |

**Response 200:** (misma estructura que `/visualizacion/summary`)

**Errores tipados:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| INVALID_DATE_RANGE | 400 | `from > to` |
| PROFESSIONAL_ACCESS_DENIED | 403 | No existe CareLink activo con `can_view_data=true` |
| UNAUTHORIZED | 401 | JWT invalido o expirado |

---

### GET /api/v1/professional/patients/{patientId}/timeline

Timeline del paciente para el profesional autenticado.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (professional) |
| Autorizacion | `CareLink.can_view_data=true` para el patientId |
| Estado | **Implementado** |

**Query params:**

| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| from | date (ISO8601) | Si | Inicio del rango |
| to | date (ISO8601) | Si | Fin del rango |

**Response 200:** (misma estructura que `/visualizacion/timeline`)

**Errores tipados:**

| Codigo | HTTP | Trigger |
|--------|------|---------|
| INVALID_DATE_RANGE | 400 | `from > to` |
| PROFESSIONAL_ACCESS_DENIED | 403 | No existe CareLink activo con `can_view_data=true` |
| UNAUTHORIZED | 401 | JWT invalido o expirado |

---

## Exportacion — Implementacion Phase 31

### GET /api/v1/export/patient-summary

Exportacion estructurada en formato JSON.

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient owner unicamente) |
| Consent gate | Requiere `ConsentGrant.status=granted` |
| Estado | **Implementado** |

**Response 200:** DTO JSON con summary y entries (estructura definida en RF-EXP-001).

---

### GET /api/v1/export/patient-summary/csv

Exportacion en formato CSV (RF-EXP-002).

| Campo | Detalle |
|-------|---------|
| Autenticacion | JWT Bearer (patient owner unicamente) |
| Consent gate | Requiere `ConsentGrant.status=granted` |
| Estado | **Implementado** |

**Headers de respuesta:**

```
Content-Type: text/csv
Content-Disposition: attachment; filename="bitacora-export-YYYYMMDD-YYYYMMDD.csv"
```

**Cabecera del CSV:**

```csv
fecha,mood_score,sleep_hours,physical_activity,social_activity,anxiety,irritability,medication_taken
```

**Nota:** Los datos se extraen de `safe_projection` exclusivamente. No hay descifrado de `encrypted_payload` (RF-EXP-002 diferido). `medication_time` no se incluye en el CSV.

---

## Superficies deferidas

| Endpoint | Estado | Notas |
|----------|--------|-------|
| GET /api/v1/visualizacion/timeline | **Implementado** | |
| GET /api/v1/visualizacion/summary | **Implementado** | |
| GET /api/v1/professional/patients/{patientId}/summary | **Implementado** | Requiere `CareLink.can_view_data=true` |
| GET /api/v1/professional/patients/{patientId}/timeline | **Implementado** | Requiere `CareLink.can_view_data=true` |
| GET /api/v1/professional/patients/{patientId}/alerts | **Implementado** | Requiere `CareLink.can_view_data=true` |
| GET /api/v1/export/patient-summary | **Implementado** | DTO JSON; owner-only |
| GET /api/v1/export/patient-summary/csv | **Implementado** | CSV con safe_projection; owner-only |
| Descifrado de payloads (RF-EXP-002) | **Diferido** | No requerido; solo `safe_projection` |
| Streaming CSV (RF-EXP-003) | **Diferido** | CSV sin streaming; dataset en memoria |
| Export para profesionales | **No permitido** | Export es owner-only; profesionales ven 403; `ExportGate` lo explicita en UI |

---

## Invariantes de Autorizacion

1. **Owner-only para export:** `GET /api/v1/export/patient-summary` solo acepta JWT del paciente owner; nunca se sirve a contexto profesional aunque `CareLink.can_view_data=true`.
2. **Filtro global siempre activo:** el Global Query Filter filtra por `patient_id` del contexto en toda lectura de datos del paciente.
3. **Proyeccion segura:** todos los endpoints usan `safe_projection` exclusivamente; no hay descifrado de `encrypted_payload`.

---

## Invariantes de Compliance

1. **Sin fuga a profesionales:** los endpoints `visualizacion` son owner-only; no hay endpoints profesionales en esta fase.
2. **Safe projection only:** los queries no acceden a `encrypted_payload`; operan exclusivamente sobre `safe_projection`.
3. **Pseudonimizacion en logs:** ninguna respuesta incluye `actor_id` real; se usa `pseudonym_id`.

---

## Sincronizacion

Cambios en este contrato fuerzan revision de:
- `09_contratos_tecnicos.md` (seccion Superficie canonica diferida)
- `04_RF/RF-VIS-*`, `RF-EXP-*`
- `07_baseline_tecnica.md` si cambia cifrado o compliance
