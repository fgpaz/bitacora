# RF-REG-015: Manejar consentimiento no otorgado via Telegram

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-015 |
| Modulo | REG |
| Actor | Sistema (handler Telegram) |
| Flujo fuente | FL-REG-02 |
| Prioridad | Privacy |

## Precondiciones detalladas
- Existe `TelegramSession` activa para el `chat_id`.
- La verificacion de consentimiento detecta que el paciente no tiene `ConsentGrant.status='granted'`.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | TelegramSession | Existente |
| chat_id | bigint | Telegram Update | Requerido |
| trace_id | uuid | Contexto del request | Requerido |

## Proceso (Happy Path)
1. Detectar que el paciente no tiene consentimiento vigente.
2. No crear `MoodEntry` ni continuar la secuencia de factores.
3. Responder al usuario: `Debes completar tu consentimiento en la web antes de registrar datos.`
4. INSERT `AccessAudit` con `action_type='read'`, `resource_type='consent_grant'`, `outcome='denied'`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| handled | bool | `true` si el bloqueo fue aplicado |
| bot_message | string | Mensaje enviado al usuario |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CONSENT_REQUIRED | 200 | No existe `ConsentGrant` vigente | Mensaje Telegram con link al consentimiento |
| AUDIT_WRITE_FAILED | 500 | Fallo al persistir auditoria del bloqueo | {error: "AUDIT_WRITE_FAILED"} |

## Casos especiales y variantes
- `ConsentGrant` revocado se trata igual que ausente.
- El webhook consume el caso con `200` y mensaje al usuario; no se delega reintento a Telegram.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| ConsentGrant | SELECT | patient_id, status, consent_version |
| AccessAudit | INSERT | trace_id, patient_id, action_type, resource_type, outcome, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente sin consentimiento es redirigido a la web
  Given TelegramSession linked para el chat
  And el paciente no tiene ConsentGrant vigente
  When llega un callback de humor por webhook
  Then no se crea MoodEntry
  And el bot responde con instrucciones para completar el consentimiento

Scenario: Falla de auditoria bloquea la respuesta final
  Given no existe ConsentGrant vigente
  And la escritura de AccessAudit falla
  When llega un callback de humor por webhook
  Then se registra AUDIT_WRITE_FAILED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-015-01 | Bloquea registro via Telegram sin consentimiento | Positivo |
| TP-REG-015-02 | Trata consentimiento revocado como no vigente | Positivo |
| TP-REG-015-03 | Falla si no puede auditar el bloqueo | Negativo |

## Sin ambiguedades pendientes
Ninguna.
