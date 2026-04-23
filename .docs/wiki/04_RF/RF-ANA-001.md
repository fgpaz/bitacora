# RF-ANA-001: Persistir evento de UX impact (no-PII)

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-ANA-001 |
| Modulo | ANA (Analytics) |
| Endpoint | POST /api/v1/analytics/events |
| Actor | Patient (API) |
| Prioridad | Privacy > Correctness > Performance |

## Precondiciones detalladas
- JWT Zitadel valido y no expirado.
- `User.role = 'patient'`.
- `event_name` dentro del whitelist (`time_to_cta_ready`, `ctr_rail_vs_checkin`, `logout_accidental_rate`, `decline_consent_rate`).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| JWT | string | Authorization header | Valido y no expirado |
| event | string | Body JSON | No nulo, no vacio, <=64 chars, whitelist |
| props | object? | Body JSON | JSON valido, <=2048 chars raw, **sin PII** (contrato del caller, no se valida contenido) |

## Proceso (Happy Path)
1. Resolver `patient_id` del JWT via `CurrentAuthenticatedPatientResolver`.
2. Validar `event_name` dentro del whitelist (rechazo 400 si no).
3. Validar longitud `props_json` <= 2048 chars (rechazo 400 si excede).
4. Crear `AnalyticsEvent` con `patient_id`, `event_name`, `props_json`, `trace_id`, `created_at_utc = UtcNow`.
5. Persistir via repository + SaveChanges.
6. Retornar HTTP 202 Accepted con `analytics_event_id` y `created_at_utc`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| analyticsEventId | uuid | ID interno del evento registrado |
| createdAtUtc | timestamp | ISO UTC del registro |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| EVENT_NAME_REQUIRED | 400 | `event_name` nulo o vacio | {error: "EVENT_NAME_REQUIRED"} |
| EVENT_NAME_UNKNOWN | 400 | `event_name` fuera del whitelist | {error: "EVENT_NAME_UNKNOWN"} |
| PROPS_TOO_LARGE | 400 | `props_json` excede 2048 chars | {error: "PROPS_TOO_LARGE"} |
| INVALID_BODY | 400 | Body ausente o malformado | {error: "INVALID_BODY"} |

## Casos especiales y variantes
- `props = null` o ausente: se persiste `props_json = NULL`, el endpoint sigue aceptando.
- Rate limiting: policy `write` comparte cota con otros endpoints de escritura; el stub frontend NO debe emitir de manera bulk (solo en respuesta a acciones del usuario).
- Fire-and-forget semantic: el front swallow de errores — si el endpoint falla, la UX no se rompe.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| AnalyticsEvent | INSERT | analytics_event_id, patient_id, event_name, props_json, trace_id, created_at_utc |

## Privacy invariantes
- `props_json` NO debe contener PII (email, nombre, contenido clinico, trace ids externos).
- El handler valida **longitud** pero NO contenido; es responsabilidad del caller respetar el contrato.
- `patient_id` queda asociado al evento para correlacion de UX flows individuales bajo autoridad del equipo; pseudonimizacion se aplica en consulta si la retention policy lo exige.
- Retention policy: sugerido 180 dias via task cron separada (follow-up pendiente).

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Registrar evento valido
  Given JWT patient valido
  When POST /api/v1/analytics/events con event="time_to_cta_ready" y props={source:"rail", delta_ms:1200}
  Then HTTP 202 Accepted
  And response incluye analyticsEventId UUID no vacio
  And el evento queda persistido en analytics_events

Scenario: Rechazar event fuera del whitelist
  Given JWT patient valido
  When POST /api/v1/analytics/events con event="something_random"
  Then HTTP 400
  And error code = "EVENT_NAME_UNKNOWN"

Scenario: Rechazar props excedentes
  Given JWT patient valido
  When POST /api/v1/analytics/events con event="time_to_cta_ready" y props > 2048 chars
  Then HTTP 400
  And error code = "PROPS_TOO_LARGE"
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-ANA-001-01 | Registrar evento time_to_cta_ready con props validas | Positivo |
| TP-ANA-001-02 | Registrar evento sin props (null) | Positivo |
| TP-ANA-001-03 | Rechazar event_name fuera del whitelist | Negativo |
| TP-ANA-001-04 | Rechazar props_json > 2048 chars | Negativo |

## Sin ambiguedades pendientes
Ninguna. Retention policy es follow-up operacional, no funcional.
