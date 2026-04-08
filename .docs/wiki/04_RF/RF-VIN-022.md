# RF-VIN-022: Verificar ownership del CareLink

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-022 |
| Modulo | VIN |
| Actor | Sistema (guard interno) |
| Flujo fuente | FL-VIN-01, FL-VIN-02, FL-VIN-03 |
| Prioridad | Security |

## Precondiciones detalladas
- JWT presente y patient_id extraido.
- care_link_id provisto como parametro de la operacion.
- Llamado antes de cualquier operacion sobre el CareLink por parte del paciente.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| care_link_id | uuid | Path param | Existente en BD |
| patient_id | uuid | JWT | Existente |

## Proceso (Happy Path)
1. SELECT patient_id FROM care_links WHERE care_link_id=?.
2. Comparar patient_id del registro con patient_id del JWT.
3. Si coinciden: retornar OK al llamador.
4. Si no coinciden o no existe el link: retornar 403 FORBIDDEN.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| authorized | bool | true si owner, false si no |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| FORBIDDEN | 403 | patient_id no coincide | {error: "FORBIDDEN"} |
| LINK_NOT_FOUND | 404 | care_link_id no existe | {error: "LINK_NOT_FOUND"} |

## Casos especiales y variantes
- Professional intentando operar sobre CareLink del paciente: rechazado (403).
- No se revela si el link existe cuando el requestor no es el owner (404 vs 403 a criterio de impl.).

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | SELECT | patient_id (solo lectura) |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Owner puede operar sobre su CareLink
  Given CareLink.patient_id coincide con JWT patient_id
  When se verifica ownership
  Then se retorna authorized=true

Scenario: No-owner bloqueado
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
