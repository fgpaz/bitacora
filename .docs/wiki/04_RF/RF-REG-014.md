# RF-REG-014: Manejar TelegramSession no vinculada

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-014 |
| Modulo | REG |
| Actor | Sistema (handler Telegram) |
| Flujo fuente | FL-REG-02 |
| Prioridad | Usability |

## Precondiciones detalladas
- RF-REG-010 ya valido el webhook.
- RF-REG-011 no pudo resolver una `TelegramSession` activa para el `chat_id`.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| chat_id | bigint | Telegram Update | Requerido |
| trace_id | uuid | Contexto del request | Requerido |

## Proceso (Happy Path)
1. Detectar que no existe `TelegramSession` en estado `linked` para el `chat_id`.
2. No intentar crear `MoodEntry` ni continuar el flujo conversacional.
3. Responder al usuario: `Tu cuenta no esta vinculada. Visita bitacora.nuestrascuentitas.com para vincularla.`
4. INSERT `AccessAudit` con `action_type='read'`, `resource_type='telegram_session'`, `outcome='denied'`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| handled | bool | `true` si el caso fue manejado |
| bot_message | string | Mensaje de guidance enviado al usuario |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| SESSION_NOT_LINKED | 200 | `chat_id` sin `TelegramSession` activa | Mensaje Telegram con link a la web |
| AUDIT_WRITE_FAILED | 500 | Fallo al persistir auditoria del rechazo | {error: "AUDIT_WRITE_FAILED"} |

## Casos especiales y variantes
- Si la sesion existe pero esta `unlinked`, se trata igual que inexistente.
- El webhook sigue respondiendo `200` a Telegram cuando el caso de negocio fue manejado correctamente.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| TelegramSession | SELECT | chat_id, status |
| AccessAudit | INSERT | trace_id, action_type, resource_type, outcome, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Callback de chat no vinculado muestra guidance
  Given no existe TelegramSession linked para el chat_id
  When llega un callback de humor por webhook
  Then no se crea MoodEntry
  And el bot responde con instrucciones de vinculacion

Scenario: Falla de auditoria bloquea la respuesta final
  Given no existe TelegramSession linked
  And la escritura de AccessAudit falla
  When llega un callback de humor por webhook
  Then se registra AUDIT_WRITE_FAILED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-014-01 | Chat no vinculado recibe guidance y no crea datos | Positivo |
| TP-REG-014-02 | Sesion unlinked se trata como no vinculada | Positivo |
| TP-REG-014-03 | Falla la auditoria del rechazo | Negativo |

## Sin ambiguedades pendientes
Ninguna.
