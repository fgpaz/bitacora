# TP-EXP — Plan de Pruebas del Modulo EXP

## Alcance

- RF cubiertos: RF-EXP-001..003
- Flujo origen: FL-EXP-01

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| EXP-P01 | RF-EXP-001, RF-EXP-002, RF-EXP-003 | Positivo | Export CSV de rango valido con streaming y descifrado correcto |
| EXP-N01 | RF-EXP-001 | Negativo | Rechaza rango invalido o parametros ausentes |
| EXP-N02 | RF-EXP-002 | Negativo | Falla export completo si falta la clave requerida |
| EXP-N03 | RF-EXP-003 | Negativo | Reporta error si el stream se interrumpe |

## Gherkin expandido

```gherkin
Scenario: Export CSV completo de un rango con datos
  Given patient autenticado con MoodEntry y DailyCheckin en el rango
  When GET /api/v1/export/csv?from=2026-04-01&to=2026-04-07
  Then HTTP 200 con Content-Type text/csv
  And la primera fila contiene headers estandarizados
  And el cuerpo se envia por streaming

Scenario: Export con rango invalido es rechazado
  Given patient autenticado
  When GET /api/v1/export/csv?from=2026-04-07&to=2026-04-01
  Then se retorna 400 EXP_001_RANGE_INVALID

Scenario: Falta una key_version historica durante export
  Given existe un registro cuyo key_version no esta disponible
  When GET /api/v1/export/csv para un rango que lo incluye
  Then se retorna 500 EXP_001_DECRYPT_FAILED
  And no se entrega un CSV parcial
```

## Criterios de salida

- Cobertura positiva y negativa de los 3 RF del modulo.
- Evidencia de streaming real y fail-closed ante descifrado fallido.
