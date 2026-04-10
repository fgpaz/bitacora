# RF-CON-003: Hard gate — bloquear registro sin consent granted

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-CON-003 |
| Modulo | CON |
| Actor | Sistema (Middleware) |
| Flujo fuente | FL-CON-01 |
| Prioridad | Security |

## Precondiciones detalladas
- Request entrante apunta a POST /api/v1/mood-entries o POST /api/v1/daily-checkins.
- JWT presente y patient_id extraido.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | JWT | Extraido antes del middleware |
| endpoint | string | Request path | Debe estar en lista protegida |

## Proceso (Happy Path)
1. Interceptar request antes del handler.
2. Consultar ConsentGrant WHERE patient_id=? AND status='granted'.
3. Si existe: dejar pasar la request al handler.
4. Si no existe: cortar la cadena y retornar 403 CONSENT_REQUIRED.
5. INSERT AccessAudit con `action_type='read'`, `resource_type='consent_grant'`, `outcome='denied'` solo si el request queda bloqueado.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| — | — | Pasa al handler si consent OK |
| error | string | "CONSENT_REQUIRED" si bloqueado |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CONSENT_REQUIRED | 403 | Sin grant activo | {error: "CONSENT_REQUIRED"} |
| UNAUTHORIZED | 401 | Sin JWT | {error: "UNAUTHORIZED"} |

## Casos especiales y variantes
- Grant revocado: tratado igual que ausente; retorna 403.
- Endpoints GET no requieren este gate; solo aplica a POST de escritura de datos clinicos.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| AccessAudit | INSERT (condicional) | trace_id, patient_id, action_type, resource_type, outcome, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente con consent activo puede registrar
  Given ConsentGrant.status="granted" para patient_id
  When POST /api/v1/mood-entries con JWT valido
  Then el handler procesa la request

Scenario: Paciente sin consent bloqueado
  Given no existe ConsentGrant activo para patient_id
  When POST /api/v1/mood-entries
  Then se retorna 403 con error CONSENT_REQUIRED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-CON-003-01 | Request pasa con consent activo | Positivo |
| TP-CON-003-02 | 403 sin consent grant | Negativo |
| TP-CON-003-03 | 403 con consent revocado | Negativo |

## Sin ambiguedades pendientes
Ninguna.
