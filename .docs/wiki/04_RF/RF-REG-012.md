# RF-REG-012: Crear MoodEntry desde callback de Telegram

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-012 |
| Modulo | REG |
| Actor | Patient via Telegram Bot |
| Flujo fuente | FL-REG-02 |
| Prioridad | Correctness |

## Precondiciones detalladas
- Webhook validado por RF-REG-010.
- patient_id resuelto por RF-REG-011.
- callback_data contiene score en formato definido (ej: "mood:2").
- ConsentGrant.status = 'granted' para el patient_id.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| callback_data | string | Telegram InlineKeyboard | Formato "mood:{score}", score extraido como int |
| patient_id | uuid | RF-REG-011 | Resuelto y verificado |
| chat_id | int | Update de Telegram | Trazabilidad |

## Proceso (Happy Path)
1. Parsear callback_data para extraer score.
2. Ejecutar RF-REG-004 (verificar consent).
3. Ejecutar RF-REG-002 (validar score).
4. Ejecutar RF-REG-005 (verificar idempotencia).
5. Ejecutar RF-REG-003 (cifrar payload con channel='telegram').
6. INSERT MoodEntry con channel='telegram'.
7. INSERT AccessAudit con `action_type='create'`, `resource_type='mood_entry'`, `resource_id=mood_entry_id`.
8. Responder al bot para confirmar registro al usuario.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| mood_entry_id | uuid | ID del registro creado |
| safe_projection | jsonb | {mood_score, channel='telegram', created_at} |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| CONSENT_REQUIRED | 403 | Sin consent activo | Bot notifica al usuario |
| INVALID_SCORE | 422 | Score invalido en callback_data | Bot notifica error |
| ENCRYPTION_FAILURE | 500 | Fallo de KMS | Bot notifica error generico |

## Casos especiales y variantes
- callback_data malformado (no sigue "mood:{score}"): rechazar con MALFORMED_CALLBACK.
- Idempotencia: mismo comportamiento que RF-REG-001.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| MoodEntry | INSERT | mood_entry_id, patient_id, encrypted_payload, safe_projection (channel='telegram'), key_version, encrypted_at, created_at_utc |
| AccessAudit | INSERT | trace_id, actor_id, patient_id, action_type, resource_type, resource_id, created_at_utc |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Callback de Telegram crea MoodEntry con canal correcto
  Given patient con consent activo
  And callback_data = "mood:1"
  When se procesa el callback
  Then MoodEntry.safe_projection.channel = 'telegram'
  And se retorna 201

Scenario: callback_data malformado es rechazado
  Given callback_data = "invalid_format"
  When se procesa el callback
  Then se retorna error MALFORMED_CALLBACK
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-012-01 | Callback valido crea MoodEntry canal telegram | Positivo |
| TP-REG-012-02 | callback_data malformado retorna error | Negativo |
| TP-REG-012-03 | Sin consent retorna 403 | Negativo |

## Sin ambiguedades pendientes
Ninguna.
