# RF-VIN-022: Verificar ownership del CareLink

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-022 |
| Modulo | VIN |
| Actor | Sistema (guard interno) |
| Flujo fuente | FL-VIN-01, FL-VIN-03, FL-VIN-04 |
| Prioridad | Security |

## Precondiciones detalladas
- JWT presente y `patient_id` extraido.
- `care_link_id` provisto como parametro de la operacion.
- El guard se ejecuta antes de aceptar invitaciones, revocar vinculos o cambiar `can_view_data`.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| care_link_id | uuid | Path param | Existente en BD |
| patient_id | uuid | JWT | Existente |

## Proceso (Happy Path)
1. SELECT `patient_id` FROM `care_links WHERE care_link_id=?`.
2. Comparar `patient_id` del registro con `patient_id` del JWT.
3. Si coinciden, retornar `authorized=true`.
4. Si no coinciden, retornar `403 FORBIDDEN`.
5. Si no existe el link, retornar `404 LINK_NOT_FOUND`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| authorized | bool | `true` si es owner, `false` si no |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| FORBIDDEN | 403 | `patient_id` no coincide | {error: "FORBIDDEN"} |
| LINK_NOT_FOUND | 404 | `care_link_id` no existe | {error: "LINK_NOT_FOUND"} |

## Casos especiales y variantes
- Un profesional nunca pasa este guard; solo aplica a operaciones del paciente owner.
- La ausencia del link y la falta de ownership se distinguen explicitamente para mantener contratos tipados consistentes.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | SELECT | patient_id |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Owner puede operar sobre su CareLink
  Given CareLink.patient_id coincide con JWT patient_id
  When se verifica ownership
  Then se retorna authorized=true

Scenario: No-owner es bloqueado
  Given CareLink.patient_id != JWT patient_id
  When se verifica ownership
  Then se retorna 403 FORBIDDEN
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-022-01 | Owner verificado correctamente | Positivo |
| TP-VIN-022-02 | 403 si patient_id no coincide | Negativo |
| TP-VIN-022-03 | 404 si care_link_id no existe | Negativo |

## Sin ambiguedades pendientes
Ninguna.
