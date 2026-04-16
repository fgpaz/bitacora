# TP-CON — Plan de Pruebas del Modulo CON

## Alcance

- RF cubiertos: RF-CON-001..003, RF-CON-010..013
- Flujos origen: FL-CON-01, FL-CON-02

## Estado de ejecucion actual

- `Wave 1` implementa y deja listos para prueba efectiva: RF-CON-001, RF-CON-002, RF-CON-003 y RF-CON-010 baseline.
- RF-CON-011, RF-CON-012 y RF-CON-013 siguen planificados, pero dependen de `CareLink` y cache profesional, todavia no materializados en runtime.
- T01 agrega un smoke backend minimo que ejecuta lectura, otorgamiento y gate negativo de consentimiento mediante `infra/smoke/backend-smoke.ps1`.

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| CON-P01 | RF-CON-001, RF-CON-002 | Positivo | Lectura autenticada del consentimiento activo y otorgamiento exitoso |
| CON-N01 | RF-CON-001 | Negativo | Falla si no existe consentimiento activo configurado |
| CON-N02 | RF-CON-002 | Negativo | Rechaza version desactualizada o grant duplicado |
| CON-P02 | RF-CON-003 | Positivo | Gate de consentimiento permite escrituras clinicas con grant activo |
| CON-N03 | RF-CON-003 | Negativo | Gate bloquea POST clinicos sin consentimiento |
| CON-P03 | RF-CON-010, RF-CON-011, RF-CON-012, RF-CON-013 | Positivo | Revocacion con cascade, invalidacion de cache y commit atomico |
| CON-N04 | RF-CON-010, RF-CON-013 | Negativo | Rollback total ante fallo transaccional |

## Gherkin expandido

```gherkin
Scenario: Paciente lee y acepta consentimiento vigente
  Given existe consentimiento activo "v1.2" en configuracion
  And paciente autenticado sin grant vigente
  When GET /api/v1/consent/current
  Then se retorna version="v1.2" y patient_status="none"
  When POST /api/v1/consent con {version: "v1.2", accepted: true}
  Then se crea ConsentGrant con status="granted"

Scenario: Gate bloquea escrituras clinicas sin consentimiento
  Given paciente autenticado sin ConsentGrant vigente
  When POST /api/v1/mood-entries con {score: 1}
  Then se retorna 403 CONSENT_REQUIRED
  And se registra AccessAudit con outcome="denied"

Scenario: Revocacion completa con cascade y cache invalidada
  Given paciente con ConsentGrant.status="granted"
  And existen CareLinks activos para ese paciente
  And existen claves de cache de safe_projection para profesionales afectados
  When DELETE /api/v1/consent/current con {confirmed: true}
  Then ConsentGrant queda en status="revoked"
  And los CareLinks quedan en status="revoked_by_consent"
  And las claves de cache se invalidan
  And la transaccion termina en un solo commit

  # Nota de fase:
  # Este escenario sigue diferido hasta que existan CareLinks y cache profesional en runtime.

Scenario: Fallo del cascade revierte toda la revocacion
  Given paciente con ConsentGrant.status="granted"
  And el UPDATE de CareLinks falla durante la transaccion
  When DELETE /api/v1/consent/current con {confirmed: true}
  Then se retorna 500 REVOCATION_FAILED
  And ConsentGrant permanece en status="granted"
```

## Resultados de ejecucion (E2E 2026-04-15)

| TC ID | Estado | Ambiente | Fecha | Evidencia |
|-------|--------|----------|-------|-----------|
| CON-P01 | PASSED | produccion (JWT real GoTrue) | 2026-04-15 | GET /consent/current: HTTP 200, version=2026-04-09.v1, patientStatus=none. Evidencia: F2-01-consent-current.json |
| CON-P02 | PASSED | produccion | 2026-04-15 | POST /consent: HTTP 201, consentGrantId=32ba81be, status=consent_granted. DB verificado: consent_grants.status=Granted. Evidencia: F2-02-consent-grant.json, F2-04-db-consent.txt |
| CON-N01 | PASSED | produccion | 2026-04-15 | POST /mood-entries sin consent: HTTP 403, code=CONSENT_REQUIRED. Gate verificado antes de otorgar consent. Evidencia: F2-3 |

## Criterios de salida

- Cobertura positiva y negativa de los 7 RF del modulo.
- Evidencia de texto activo leido desde configuracion y no desde DB.
- Evidencia de atomicidad entre revocacion de consentimiento y revocacion de vinculos.
- Para el runtime backend-only actual, el smoke minimo debe cubrir `GET /api/v1/consent/current`, rechazo pre-consent y `POST /api/v1/consent`.
