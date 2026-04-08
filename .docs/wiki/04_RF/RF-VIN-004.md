# RF-VIN-004: can_view_data default false en creacion

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-VIN-004 |
| Modulo | VIN |
| Actor | Sistema (invariante de datos) |
| Flujo fuente | FL-VIN-01 |
| Prioridad | Security |

## Precondiciones detalladas
- Aplica a toda operacion INSERT de CareLink, sin excepcion.
- Incluye creacion por invitacion (RF-VIN-001) y por auto-vinculacion (RF-VIN-012).

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| can_view_data | bool | Request body / sistema | Se ignora si se provee; siempre se fuerza false |

## Proceso (Happy Path)
1. En cualquier INSERT de CareLink, forzar can_view_data=false independientemente del input.
2. Verificacion de constraint: CHECK (can_view_data = false) en INSERT (o DEFAULT false en schema).
3. Solo UPDATE posterior por el patient owner (RF-VIN-023) puede cambiar a true.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| can_view_data | bool | Siempre false en creacion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| — | — | No hay error; el valor se fuerza silenciosamente | — |

## Casos especiales y variantes
- Si el professional intenta enviar can_view_data=true en el body: campo ignorado, siempre false.
- Schema DB debe tener DEFAULT false y solo el patient puede hacer UPDATE.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| CareLink | INSERT | can_view_data (siempre false) |

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
| TP-VIN-004-03 | DB constraint rechaza INSERT con can_view_data=true | Borde |

## Sin ambiguedades pendientes
Ninguna.
