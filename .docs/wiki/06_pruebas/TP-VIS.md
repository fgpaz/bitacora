# TP-VIS — Plan de Pruebas del Modulo VIS

## Alcance

- RF cubiertos: RF-VIS-001..003, RF-VIS-010..014
- Flujos origen: FL-VIS-01, FL-VIS-02

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
  When GET /api/v1/mood-entries y GET /api/v1/daily-checkins con cursor valido
  Then se retorna solo informacion del paciente autenticado
  And se respetan los limites de paginacion

Scenario: Dashboard profesional lista solo pacientes visibles
  Given professional autenticado
  And tiene dos CareLinks activos con can_view_data=true
  And tiene un CareLink activo con can_view_data=false
  When GET /api/v1/professional/dashboard
  Then solo aparecen los dos pacientes visibles
  And se registra AccessAudit por cada paciente expuesto

Scenario: Profesional obtiene summary y alerts de un paciente visible
  Given professional autenticado con CareLink activo y can_view_data=true
  And el paciente tiene entradas clinicas en los ultimos 30 dias
  When GET /api/v1/professional/patients/PAT-0042/summary
  Then se calcula avg_mood, min_mood, max_mood y trend
  When GET /api/v1/professional/patients/PAT-0042/alerts
  Then se retorna LOW_MOOD_STREAK si hay tres dias consecutivos con mood <= -2

Scenario: Auditoria fallida bloquea lectura profesional
  Given professional autenticado con CareLink visible
  And la escritura de AccessAudit falla
  When GET /api/v1/professional/patients/PAT-0042/summary
  Then se retorna 500 VIS_011_AUDIT_FAILED
  And no se retornan datos del paciente
```

## Criterios de salida

- Cobertura positiva y negativa de los 8 RF del modulo.
- Evidencia de separacion entre dashboard, summary y alerts.
- Evidencia de ocultamiento silencioso cuando `can_view_data=false`.
