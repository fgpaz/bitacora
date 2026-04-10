# TP-SEC — Plan de Pruebas del Modulo SEC

## Alcance

- RF cubiertos: RF-SEC-001..003
- Flujo origen: FL-SEC-01

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| SEC-P01 | RF-SEC-001, RF-SEC-002 | Positivo | Interceptor profesional audita lectura permitida con pseudonym_id correcto |
| SEC-N01 | RF-SEC-001 | Negativo | Interceptor bloquea lectura sin CareLink visible |
| SEC-N02 | RF-SEC-002 | Negativo | Falla si falta salt de pseudonimizacion |
| SEC-N03 | RF-SEC-003 | Negativo | No retorna datos cuando la auditoria falla |

## Gherkin expandido

```gherkin
Scenario: Lectura profesional permitida queda auditada
  Given professional autenticado con CareLink activo y can_view_data=true
  And existe salt de pseudonimizacion configurado
  When consulta summary o alerts de un paciente visible
  Then se genera AccessAudit con action_type="read"
  And los logs operacionales solo usan pseudonym_id

Scenario: Interceptor bloquea acceso sin permiso
  Given professional autenticado sin CareLink visible para el paciente solicitado
  When consulta summary o alerts
  Then se retorna 403
  And no se exponen datos clinicos

Scenario: Auditoria fallida activa fail-closed
  Given professional autenticado con permiso valido
  And la insercion de AccessAudit falla
  When consulta datos clinicos del paciente
  Then se retorna error de auditoria
  And no se retornan datos del paciente
```

## Criterios de salida

- Cobertura positiva y negativa de los 3 RF del modulo.
- Evidencia de pseudonimizacion y fail-closed ante falla de audit.
