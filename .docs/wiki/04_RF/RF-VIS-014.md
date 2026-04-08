# RF-VIS-014: Audit por cada paciente accedido

## Execution Sheet
- Modulo: VIS
- Trigger: Cualquier endpoint profesional que exponga datos de pacientes
- Actor: Sistema (automatico post-verificacion de CareLink)
- Prioridad PDP: Security (obligatorio, fail-closed)

## Precondiciones detalladas
- professional_id resuelto del JWT
- Lista de patient_ids accedidos en el request actual
- env_salt disponible para generacion de pseudonym_id (RF-SEC-002)
- AccessAudit es append-only; nunca se actualiza ni elimina

## Inputs
| Campo | Origen | Descripcion |
|-------|--------|-------------|
| actor_id | JWT | professional supabase_user_id |
| patient_ids | Query result | Lista de pacientes expuestos en este request |
| trace_id | HTTP Header X-Trace-Id | Correlacion del request |
| action_type | Constante por endpoint | Ej: PROFESSIONAL_DASHBOARD_LIST |
| resource_type | Constante | CARE_LINK o MOOD_ENTRY |

## Proceso (Happy Path)
1. Recolectar todos los patient_ids accedidos durante el request
2. Para cada patient_id, calcular pseudonym_id = SHA256(actor_id + env_salt) (RF-SEC-002)
3. Construir batch de registros AccessAudit:
   - audit_id: nuevo UUID
   - trace_id: del header
   - actor_id: del JWT
   - pseudonym_id: calculado
   - action_type: segun endpoint
   - resource_type / resource_id: segun contexto
   - patient_id: el real (campo protegido, no expuesto en respuesta)
   - outcome: SUCCESS
   - created_at_utc: now()
4. INSERT batch en `access_audits`
5. Si INSERT falla → abortar request, retornar 500 (RF-SEC-003)

## Outputs
- No retorna datos al cliente
- Efecto: registros insertados en `access_audits`

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| VIS_014_AUDIT_BATCH_FAILED | 500 | Fallo en INSERT batch; request abortado |
| VIS_014_SALT_MISSING | 500 | env_salt no disponible (RF-SEC-002) |

## Casos especiales y variantes
- Request que expone 0 pacientes (lista vacia): no genera audits
- Un fallo parcial en el batch falla todo el batch (transaccional)
- trace_id ausente en header: usar UUID generado en servidor como fallback

## Impacto en modelo de datos
- INSERT en `access_audits` (append-only, nunca UPDATE/DELETE)
- Campos: audit_id, trace_id, actor_id, pseudonym_id, action_type, resource_type, resource_id, patient_id, outcome, created_at_utc

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Dashboard lista 5 pacientes, se insertan 5 audits
  Given profesional accede a 5 pacientes en un request
  When el request completa exitosamente
  Then 5 registros en access_audits con outcome=SUCCESS

Scenario: Fallo en INSERT audit aborta el request
  Given el INSERT en access_audits lanza excepcion
  When el profesional intenta acceder al dashboard
  Then HTTP 500 y ningun dato de paciente es retornado

Scenario: Lista vacia no genera audits
  Given profesional sin pacientes vinculados
  When GET /api/v1/professional/dashboard
  Then HTTP 200 con lista vacia y 0 audits insertados
```

## Trazabilidad de tests
- UT: VIS014_BatchBuild_CorrectFields
- IT: VIS014_AuditInserted_ForEachPatient
- IT: VIS014_AuditFail_Aborts_Response

## Sin ambiguedades pendientes
- El batch es una sola transaccion DB; parcial no es aceptable
- `pseudonym_id` es identico para el mismo actor en todos los audits (deterministico)
