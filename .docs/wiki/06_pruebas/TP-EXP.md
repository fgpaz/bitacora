# TP-EXP — Plan de Pruebas del Modulo EXP

## Alcance

- RF cubiertos: RF-EXP-001..003
- Flujo origen: FL-EXP-01

## Estado de ejecucion actual

- `Parcialmente implementado` en el runtime actual (Phase 31+).
- `GET /api/v1/export/patient-summary` (JSON) implementada.
- `GET /api/v1/export/patient-summary/csv` implementada (CSV owner-only).
- El CSV se genera en memoria (no streaming); los datos se extraen de `safe_projection` exclusivamente; no hay descifrado de `encrypted_payload`.
- **Restriccion critica: export es owner-only.** Profesionales NO pueden exportar datos de pacientes aunque tengan `can_view_data=true`. Esta restriccion es enforceada a nivel de API y reflejada explicitamente en `ExportGate` del frontend.

## Gap conocido para Phase 41

- `GET /api/v1/export/{patientId}/constraints` — **NO existe en backend** (404).
  - Consumido por `frontend/lib/api/professional.ts:115–118` (`getExportConstraints`).
  - El frontend espera que retorne `ExportConstraint` con `allowed:false` para profesionales.
  - **Impacto:** profesionales ven un estado de export sin podervalidar constraints real desde el backend.
  - **Target:** implementar en Phase 41 como parte de FL-EXP-01.

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| EXP-P01 | RF-EXP-001, RF-EXP-002, RF-EXP-003 | Positivo | Export CSV de rango valido con streaming y descifrado correcto |
| EXP-N01 | RF-EXP-001 | Negativo | Rechaza rango invalido o parametros ausentes |
| EXP-N02 | RF-EXP-002 | Negativo | Falla export completo si falta la clave requerida |
| EXP-N03 | RF-EXP-003 | Negativo | Reporta error si el stream se interrumpe |

## Gherkin expandido

```gherkin
Scenario: Paciente exporta su historial en CSV
  Given patient autenticado con MoodEntry y DailyCheckin en el rango
  When GET /api/v1/export/patient-summary/csv?from=2026-04-01&to=2026-04-07
  Then HTTP 200 con Content-Type text/csv
  And Content-Disposition: attachment; filename="bitacora-export-YYYYMMDD-YYYYMMDD.csv"
  And la primera fila contiene headers: fecha,mood_score,sleep_hours,physical_activity,social_activity,anxiety,irritability,medication_taken
  And cada fila posterior contiene los datos del dia correspondiente

Scenario: Export JSON completo de un rango
  Given patient autenticado con MoodEntry y DailyCheckin en el rango
  When GET /api/v1/export/patient-summary?from=2026-04-01&to=2026-04-07
  Then HTTP 200 con DTO JSON que incluye entries, summary y metadata

Scenario: Export con rango invalido es rechazado
  Given patient autenticado
  When GET /api/v1/export/patient-summary/csv?from=2026-04-07&to=2026-04-01
  Then se retorna 400 INVALID_DATE_RANGE

Scenario: Profesional no puede exportar datos de paciente
  Given professional autenticado con CareLink activo y can_view_data=true
  When accede a cualquier endpoint de export para ese patientId
  Then se retorna 403 FORBIDDEN
  And ExportGate muestra estado "Exportacion no disponible"
```

## Pendiente para validacion final

- La experiencia de export CSV requiere validacion UX del flujo de descarga en desktop y mobile.
- El paso de confirmacion antes de descarga (si aplica) necesita validacion.
- UX validation: NO completada.

## Criterios de salida

- Cobertura positiva y negativa de los RF del modulo export.
- Evidencia de que CSV es owner-only y profesionales no pueden acceder.
- Evidencia de generacion en memoria (no streaming) para datasets grandes.
