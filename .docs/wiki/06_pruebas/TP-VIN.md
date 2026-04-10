# TP-VIN — Plan de Pruebas del Modulo VIN

## Alcance

- RF cubiertos: RF-VIN-001..004, RF-VIN-010..012, RF-VIN-020..023
- Flujos origen: FL-VIN-01, FL-VIN-02, FL-VIN-03, FL-VIN-04

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| VIN-P01 | RF-VIN-001, RF-VIN-002, RF-VIN-004 | Positivo | Invitacion del profesional crea CareLink invitado para paciente existente |
| VIN-P02 | RF-VIN-001 | Positivo | Invitacion del profesional crea PendingInvite para paciente no registrado |
| VIN-N01 | RF-VIN-001 | Negativo | Rechaza CareLink o PendingInvite duplicados |
| VIN-P03 | RF-VIN-003, RF-VIN-022 | Positivo | Paciente owner acepta invitacion y activa CareLink |
| VIN-N02 | RF-VIN-003, RF-VIN-022 | Negativo | No-owner no puede aceptar ni operar sobre el CareLink |
| VIN-P04 | RF-VIN-010, RF-VIN-011, RF-VIN-012 | Positivo | Auto-vinculacion con BindingCode valido y TTL elegido |
| VIN-N03 | RF-VIN-010, RF-VIN-011, RF-VIN-012 | Negativo | Rechaza ttl_preset invalido, codigo expirado o codigo usado |
| VIN-P05 | RF-VIN-020, RF-VIN-021 | Positivo | Revocacion del vinculo por paciente invalida acceso y cache |
| VIN-P06 | RF-VIN-023 | Positivo | Paciente habilita y deshabilita can_view_data |
| VIN-N04 | RF-VIN-023 | Negativo | Professional no puede cambiar can_view_data |

## Gherkin expandido

```gherkin
Scenario: Profesional invita a paciente ya registrado
  Given professional autenticado
  And existe un paciente con email "p@test.com"
  When POST /api/v1/care-links con {patient_email: "p@test.com"}
  Then se crea un CareLink con status="invited"
  And can_view_data=false

Scenario: Profesional invita a paciente aun no registrado
  Given professional autenticado
  And no existe paciente con email "nuevo@test.com"
  When POST /api/v1/care-links con {patient_email: "nuevo@test.com"}
  Then se crea un PendingInvite con status="issued"
  And expires_at corresponde a 7 dias

Scenario: Paciente se auto-vincula con BindingCode de 72 horas
  Given professional autenticado genera un BindingCode con ttl_preset="72h"
  And paciente autenticado tiene consentimiento vigente
  When POST /api/v1/care-links/bind con el codigo emitido
  Then se crea un CareLink activo
  And can_view_data=false
  And el BindingCode queda marcado como usado

Scenario: Paciente owner gestiona visibilidad del profesional
  Given existe un CareLink activo perteneciente al paciente
  When PATCH /api/v1/care-links/{id} con {can_view_data: true}
  Then can_view_data=true
  When PATCH /api/v1/care-links/{id} con {can_view_data: false}
  Then can_view_data=false

Scenario: Professional no puede cambiar la visibilidad
  Given existe un CareLink activo entre paciente y professional
  And el request usa JWT del professional
  When PATCH /api/v1/care-links/{id} con {can_view_data: true}
  Then se retorna 403 FORBIDDEN
```

## Criterios de salida

- Cobertura positiva y negativa de los 11 RF del modulo.
- Evidencia de separacion entre `PendingInvite` y `BindingCode`.
- Evidencia de TTL por emision y default `15m` para BindingCode.
