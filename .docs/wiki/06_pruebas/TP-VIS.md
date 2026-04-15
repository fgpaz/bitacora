# TP-VIS — Plan de Pruebas del Modulo VIS

## Alcance

- RF cubiertos: RF-VIS-001..003, RF-VIS-010..014
- Flujos origen: FL-VIS-01, FL-VIS-02

## Estado de ejecucion actual

- `Parcialmente implementado` en el runtime actual (Phase 31+).
- Endpoints profesionales implementados: `GET /api/v1/professional/patients`, `GET /api/v1/professional/patients/{patientId}/summary`, `GET /api/v1/professional/patients/{patientId}/timeline`, `GET /api/v1/professional/patients/{patientId}/alerts`.
- Todos los endpoints profesionales requieren `CareLink` con `can_view_data=true` (authorization via `ProfessionalDataAccessAuthorizer`).
- La lista de pacientes profesionales se serve via `/api/v1/professional/patients`; no existe un endpoint `/professional/dashboard` dedicado.
- La superficie de exportacion para profesionales NO esta permitida (owner-only); `ExportGate` en frontend hace esta restriccion explicita.

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| VIS-P01 | RF-VIS-001, RF-VIS-002, RF-VIS-003 | Positivo | Timeline paciente paginado con rangos validos |
| VIS-N01 | RF-VIS-001, RF-VIS-002, RF-VIS-003 | Negativo | Rechaza rango o cursor invalido |
| VIS-P02 | RF-VIS-010, RF-VIS-013, RF-VIS-014 | Positivo | Dashboard profesional lista solo pacientes visibles y audita cada exposicion |
| VIS-N02 | RF-VIS-010 | Negativo | Oculta pacientes con can_view_data=false y rechaza rol incorrecto |
| VIS-P03 | RF-VIS-011 | Positivo | Summary de 7 dias calcula promedio, min, max y trend |
| VIS-P04 | RF-VIS-012 | Positivo | Alerts detecta rachas de bajo humor consecutivas |
| VIS-N03 | RF-VIS-011, RF-VIS-012, RF-VIS-014 | Negativo | Fail-closed si no existe CareLink visible o falla la auditoria |

## Gherkin expandido

```gherkin
Scenario: Paciente consulta timeline con paginacion valida
  Given patient autenticado con datos clinicos en el rango solicitado
  When GET /api/v1/visualizacion/timeline?from=2026-04-01&to=2026-04-07
  Then se retorna {days: [...]} con mood_entry y daily_checkin por dia
  And se respetan los limites de paginacion

Scenario: Profesional lista sus pacientes vinculados
  Given professional autenticado
  And tiene CareLinks activos y pendientes
  When GET /api/v1/professional/patients
  Then se retornan pacientes vinculados con status, display_name y email
  And se ocultan pacientes con CareLink en estado que no permite acceso

Scenario: Profesional obtiene summary de un paciente visible
  Given professional autenticado con CareLink activo y can_view_data=true
  And el paciente tiene entradas clinicas en el rango
  When GET /api/v1/professional/patients/{patientId}/summary?from=2026-03-01&to=2026-04-01
  Then se retorna avg_mood, min_mood, max_mood, total_days, days_with_mood
  And se retorna 403 si no existe CareLink con can_view_data=true

Scenario: Profesional obtiene timeline de un paciente visible
  Given professional autenticado con CareLink activo y can_view_data=true
  When GET /api/v1/professional/patients/{patientId}/timeline?from=2026-03-01&to=2026-04-01&page=1&page_size=20
  Then se retorna lista de entries paginada con entry_type, data y created_at
  And se retorna 403 si no existe CareLink con can_view_data=true

Scenario: Profesional obtiene alertas de un paciente visible
  Given professional autenticado con CareLink activo y can_view_data=true
  When GET /api/v1/professional/patients/{patientId}/alerts?from=2026-03-01&to=2026-04-01
  Then se retorna lista de alertas con severity, type y message
  And se retorna 403 si no existe CareLink con can_view_data=true

Scenario: Profesional no puede exportar datos de paciente
  Given professional autenticado
  When accede a la pestana de exportacion en el detalle del paciente
  Then se muestra estado "Exportacion no disponible"
  And el motivo indica que la exportacion es solo para el paciente propietario
```

### E2E 2026-04-15

| TC ID | Estado | Ambiente | Fecha | Evidencia |
|-------|--------|----------|-------|-----------|
| VIS-P01 | PASSED | produccion | 2026-04-15 | GET /visualizacion/timeline?from=2026-04-15&to=2026-04-15: HTTP 200, 1 día con MoodEntry 477cb6e4 + DailyCheckin b453c2d7. Evidencia: F6-01-timeline.json |
| VIS-P03 | PASSED | produccion | 2026-04-15 | GET /visualizacion/summary?from=2026-04-09&to=2026-04-15: HTTP 200, avgMood=1, avgSleep=7, daysWithEntry=1. Evidencia: F6-03-summary.json |
| VIS-OBS | RESUELTO | produccion | 2026-04-15 | GAP-05: key mismatch en GetPatientTimelineQuery y GetPatientSummaryQuery (buscaban physical_activity/medication_taken en lugar de has_physical/has_medication). Fix: 2026-04-15, commit b826356. Verificación post-deploy: medicationTaken=true, physicalActivity=true correctamente proyectados. |

## Pendiente para validacion final

- La experiencia completa de navegacion profesional (lista -> detalle -> tabs) requiere validacion UX con usuarios reales.
- El componente ExportGate refleja la restriccion owner-only a nivel de API.
- VIS-OBS (2026-04-15): RESUELTO — key mismatch corregido en GetPatientTimelineQuery y GetPatientSummaryQuery (GAP-05, commit b826356).
- UX validation: NO completada.

## Criterios de salida

- Cobertura positiva y negativa de los RF del modulo visualizacion.
- Evidencia de que summary, timeline y alerts son tabs separados en PatientDetail.
- Evidencia de que export para profesionales esta explicitamente bloqueado en la UI.
