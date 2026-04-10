# RF-VIN-004: can_view_data default false en creacion

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-004 |
| Modulo | VIN |
| Actor | Sistema (invariante de datos) |
| Flujo fuente | FL-VIN-01, FL-VIN-02 |
| Prioridad | Security |

## Precondiciones detalladas
- Aplica a toda operacion `INSERT` de `CareLink`, sin excepcion.
- Incluye creacion por invitacion (RF-VIN-001) y por auto-vinculacion (RF-VIN-012).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| can_view_data | bool | Request body / sistema | Se ignora si se provee; siempre se fuerza `false` |

## Proceso (Happy Path)
1. En cualquier `INSERT` de `CareLink`, forzar `can_view_data=false` independientemente del input.
2. Mantener `DEFAULT false` en schema fisico como respaldo defensivo.
3. Permitir cambio posterior solo por el paciente owner mediante RF-VIN-023.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| can_view_data | bool | Siempre `false` en creacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| — | — | No hay error; el valor se fuerza silenciosamente | — |

## Casos especiales y variantes
- Si el profesional intenta enviar `can_view_data=true` en el body, el campo se ignora.
- El mismo invariante aplica al `CareLink` materializado desde `PendingInvite`.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | INSERT | can_view_data (siempre `false`) |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Creacion via invitacion siempre con can_view_data=false
  When POST /api/v1/care-links {patient_email: "p@test.com", can_view_data: true}
  Then CareLink creado con can_view_data=false

Scenario: Creacion via binding code siempre con can_view_data=false
  When POST /api/v1/care-links/bind {code: "BIT-12345"}
  Then CareLink creado con can_view_data=false
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-VIN-004-01 | Invitacion ignora can_view_data=true en input | Positivo |
| TP-VIN-004-02 | Auto-vinculacion crea con can_view_data=false | Positivo |
| TP-VIN-004-03 | Schema mantiene default false | Borde |

## Sin ambiguedades pendientes
Ninguna.
