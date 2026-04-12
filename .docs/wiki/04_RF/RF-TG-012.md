# RF-TG-012: Skip si consent revocado o session unlinked

## Execution Sheet
- Modulo: TG
- Trigger: Invocado por `SendReminderCommand` antes de cada envio de recordatorio
- Actor: Sistema
- Prioridad PDP: Privacy > Security (skip silencioso protege la privacidad)
- Estado: **Implementado** dentro de `SendReminderCommand`

## Precondiciones detalladas
- patient_id conocido desde ReminderConfig
- Se verifica ConsentGrant y TelegramSession antes de cualquier accion de envio
- El skip es silencioso: no se notifica al paciente, no se genera error en logs de negocio

## Inputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| patient_id | uuid | Paciente del recordatorio |

## Proceso (Happy Path)
1. Query: `SELECT status FROM consent_grants WHERE patient_id = @id ORDER BY created_at DESC LIMIT 1`
2. Si `status != 'granted'` → skip silencioso; return
3. Query: `SELECT status, chat_id FROM telegram_sessions WHERE patient_id = @id AND status = 'linked' LIMIT 1`
4. Si no existe sesion linked → skip silencioso; return
5. Retornar `chat_id` al caller (RF-TG-010) para continuar con RF-TG-011

## Outputs
- Si pasa: retorna chat_id para envio
- Si skip: retorna indicador de skip (null/None), sin excepcion

## Errores tipados
| Codigo | Descripcion |
|--------|-------------|
| TG_012_DB_ERROR | Error al consultar consent o sesion; loguear, tratar como skip |

## Casos especiales y variantes
- ConsentGrant nunca creado (paciente en registro): skip
- Consent revocado y luego re-otorgado: el mas reciente determina el estado
- Multiples sesiones para el mismo paciente (no deberia ocurrir): usar la mas reciente linked
- ReminderConfig activo pero sesion unlinked: skip; no desactivar ReminderConfig automaticamente

## Impacto en modelo de datos
- Solo lectura sobre `consent_grants` y `telegram_sessions`

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Consent vigente y sesion linked -> recordatorio habilitado
  Given ConsentGrant.status=granted y TelegramSession.status=linked
  When se verifica el skip check
  Then retorna chat_id para envio

Scenario: Consent revocado -> skip silencioso
  Given ConsentGrant.status=revoked
  When se verifica el skip check
  Then retorna skip, no se envia mensaje

Scenario: Sesion unlinked -> skip silencioso
  Given ConsentGrant.status=granted pero TelegramSession.status=unlinked
  When se verifica el skip check
  Then retorna skip, no se envia mensaje

Scenario: Error de DB -> skip por precaucion
  Given error al consultar consent_grants
  When se verifica el skip check
  Then retorna skip, se loguea el error
```

## Trazabilidad de tests
- UT: TG012_GrantedAndLinked_ReturnsChatId
- UT: TG012_RevokedConsent_Skip
- UT: TG012_UnlinkedSession_Skip
- IT: TG012_DBError_TreatedAsSkip

## Sin ambiguedades pendientes
- "Silencioso" significa: no excepcion, no log de warning de negocio, solo log de debug si se desea
- El skip no desactiva el ReminderConfig; el recordatorio puede dispararse en el futuro si el consent es restaurado
