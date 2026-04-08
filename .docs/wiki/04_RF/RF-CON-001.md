# RF-CON-001: Presentar texto de consentimiento vigente

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-CON-001 |
| Modulo | CON |
| Actor | Patient (API) |
| Flujo fuente | FL-CON-01 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Existe al menos una version de consentimiento publicada en la BD.
- El endpoint es publico para lectura del texto; patient_id es opcional para verificar estado.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | JWT (opcional) | Existente si se provee |

## Proceso (Happy Path)
1. Leer version vigente de la tabla consent_versions (latest published).
2. Retornar version_id, texto completo y secciones.
3. Si patient_id presente, incluir ConsentGrant.status del paciente para esa version.
4. INSERT AccessAudit con trace_id, operacion='CONSENT_READ'.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| version | string | Identificador de version vigente |
| text | string | Texto legal completo |
| sections | jsonb | Secciones numeradas del texto |
| patient_status | string | Estado del grant del paciente (si aplica) |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| NO_CONSENT_VERSION | 503 | No hay version publicada | {error: "NO_CONSENT_VERSION"} |
| PATIENT_NOT_FOUND | 404 | patient_id invalido | {error: "PATIENT_NOT_FOUND"} |

## Casos especiales y variantes
- Sin JWT: retorna solo texto y version, sin patient_status.
- Version anterior: no se expone; siempre se sirve la vigente.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| AccessAudit | INSERT | trace_id, patient_id, operacion, created_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Leer texto sin autenticacion
  Given existe version vigente "v1.2"
  When GET /api/v1/consent/current sin JWT
  Then se retorna 200 con version="v1.2" y text no vacio

Scenario: Leer texto con paciente autenticado
  Given paciente tiene ConsentGrant.status="granted"
  When GET /api/v1/consent/current con JWT valido
  Then se retorna patient_status="granted"
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-CON-001-01 | Retorna texto vigente sin JWT | Positivo |
| TP-CON-001-02 | Incluye patient_status con JWT | Positivo |
| TP-CON-001-03 | 503 si no hay version publicada | Negativo |

## Sin ambiguedades pendientes
Ninguna.
