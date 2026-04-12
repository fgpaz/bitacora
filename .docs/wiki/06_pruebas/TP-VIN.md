# TP-VIN — Plan de Pruebas del Modulo VIN

## Alcance

- RF cubiertos: RF-VIN-001..004, RF-VIN-010..012, RF-VIN-020..023
- Flujos origen: FL-VIN-01, FL-VIN-02, FL-VIN-03, FL-VIN-04

## Estado de ejecucion actual

- `Parcialmente implementado` en el runtime actual (Wave 30+).
- La superficie de paciente (`vinculos`) esta completa: `GET /api/v1/vinculos`, `GET /api/v1/vinculos/active`, `POST /api/v1/vinculos/accept`, `DELETE /api/v1/vinculos/{id}`, `PATCH /api/v1/vinculos/{id}/view-data`.
- La superficie profesional (`professional/invites`, `professional/patients`) esta implementada en backend.
- Los endpoints de invitacion profesional emiten `PendingInvite` y NO `CareLink` directo; la aceptacion del paciente completa el vinculo.
- El flujo completo profesional->paciente->aceptacion requiere `POST /api/v1/professional/invites` (profesional) + aceptacion del paciente via codigo (futuro).

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
Scenario: Profesional invita a paciente por email
  Given professional autenticado
  When POST /api/v1/professional/invites con {email_hash: "hash_del_email"}
  Then se crea un PendingInvite con status="pending"
  And se registra AccessAudit con accion "create"

Scenario: Profesional lista sus pacientes vinculados
  Given professional autenticado
  And tiene CareLinks en diversos estados
  When GET /api/v1/professional/patients
  Then se retornan solo los pacientes con CareLink activo o pendiente
  And cada paciente incluye display_name, email y status

Scenario: Paciente acepta vinculo mediante BindingCode
  Given existe un PendingInvite pendiente para el paciente
  And paciente autenticado tiene consentimiento vigente
  When POST /api/v1/vinculos/accept con {binding_code: "BIT-XXXXX"}
  Then se crea un CareLink activo
  And can_view_data=false
  And el BindingCode queda invalidado

Scenario: Paciente owner gestiona visibilidad del profesional
  Given existe un CareLink activo perteneciente al paciente
  When PATCH /api/v1/vinculos/{id}/view-data con {can_view_data: true}
  Then can_view_data=true
  When PATCH /api/v1/vinculos/{id}/view-data con {can_view_data: false}
  Then can_view_data=false

Scenario: Profesional no puede cambiar la visibilidad
  Given existe un CareLink activo entre paciente y profesional
  And el request usa JWT del profesional
  When PATCH /api/v1/vinculos/{id}/view-data con {can_view_data: true}
  Then se retorna 403 FORBIDDEN
```

## Pendiente para validacion final

- El flujo completo de invitacion profesional (crear PendingInvite + email al paciente + aceptacion con BindingCode) requiere validacion UX del puente de email y la experiencia de aceptacion del paciente.
- La pantalla profesional de detalle de paciente aun no tiene flujo de generacion de BindingCode visible para el profesional.
- UX validation: NO completada.

## Criterios de salida

- Cobertura positiva y negativa de los RF de vinculos del modulo.
- Evidencia de separacion entre `PendingInvite` (profesional emite) y `BindingCode` (paciente consume).
- Los RF-VIN-001..004, VIN-010..012, VIN-020..023 requieren actualizacion de rutas a `/vinculos` para la versions paciente y `/professional/*` para la versions profesional.
