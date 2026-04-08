# RF-VIS-010: Listar pacientes vinculados con can_view_data=true

## Execution Sheet
- Modulo: VIS
- Endpoint: GET /api/v1/professional/dashboard
- Actor: Professional (autenticado via JWT)
- Prioridad PDP: Security > Privacy > Correctness

## Precondiciones detalladas
- JWT valido con rol=professional y User.status=active
- Solo se exponen pacientes donde existe CareLink con:
  - `professional_id = actor_id`
  - `status = active`
  - `can_view_data = true`
- AccessAudit requerido por RF-VIS-014 (batch al final del request)

## Inputs
| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| cursor | string | No | Cursor de paginacion (ver RF-VIS-013) |
| page_size | int | No | Max 20, default 20 |

## Proceso (Happy Path)
1. Extraer professional_id del JWT
2. Query: `SELECT cl.patient_id FROM care_links cl WHERE cl.professional_id = @profId AND cl.status = 'active' AND cl.can_view_data = true`
3. Aplicar paginacion cursor-based (ver RF-VIS-013)
4. Para cada patient_id, retornar identificador pseudonimizado (no nombre real)
5. Triggear registro de AccessAudit batch (RF-VIS-014)

## Outputs
```json
{
  "patients": [
    { "pseudonym_id": "sha256-abc...", "patient_ref": "PAT-0042", "care_link_id": "uuid" }
  ],
  "count": 5,
  "next_cursor": null,
  "has_more": false
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| VIS_010_FORBIDDEN | 403 | JWT no corresponde a rol=professional |
| VIS_010_UNAUTHORIZED | 401 | JWT invalido |

## Casos especiales y variantes
- Profesional sin pacientes vinculados: retorna lista vacia (no 404)
- CareLink con `can_view_data=false`: excluido silenciosamente
- CareLink con `status=inactive`: excluido silenciosamente

## Impacto en modelo de datos
- Solo lectura sobre `care_links`
- Genera AccessAudit por cada patient_id expuesto (ver RF-VIS-014)

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Profesional lista sus pacientes activos
  Given un profesional con 3 CareLinks activos y can_view_data=true
  When GET /api/v1/professional/dashboard
  Then HTTP 200 con lista de 3 pacientes pseudonimizados

Scenario: CareLink inactivo no aparece
  Given un profesional con 1 CareLink status=inactive
  When GET /api/v1/professional/dashboard
  Then HTTP 200 con patients=[]

Scenario: Usuario sin rol professional es rechazado
  Given JWT con rol=patient
  When GET /api/v1/professional/dashboard
  Then HTTP 403
```

## Trazabilidad de tests
- UT: VIS010_FiltersCareLink_ActiveAndCanView
- IT: VIS010_ProfessionalRole_Required
- IT: VIS010_AuditTriggered_ForEachPatient

## Sin ambiguedades pendientes
- La respuesta nunca expone el nombre real del paciente, solo pseudonym_id y patient_ref
- `patient_ref` es un identificador opaco interno, no el supabase_user_id
