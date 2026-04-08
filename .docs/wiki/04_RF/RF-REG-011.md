# RF-REG-011: Resolver TelegramSession por chat_id

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-011 |
| Modulo | REG |
| Actor | Sistema (resolucion de identidad) |
| Flujo fuente | FL-REG-02 |
| Prioridad | Security |

## Precondiciones detalladas
- chat_id extraido y validado por RF-REG-010.
- Tabla TelegramSession accesible en lectura.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| chat_id | int | Update de Telegram parseado | Requerido, entero positivo |

## Proceso (Happy Path)
1. Consultar TelegramSession WHERE chat_id = :chat_id AND status = 'linked'.
2. Si existe, retornar patient_id asociado.
3. Si no existe sesion activa, retornar error SESSION_NOT_FOUND.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| patient_id | uuid | ID del paciente vinculado al chat_id |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| SESSION_NOT_FOUND | 404 | No existe TelegramSession activa para chat_id | {error: "SESSION_NOT_FOUND"} |
| SESSION_UNLINKED | 403 | TelegramSession existe pero status != 'linked' | {error: "SESSION_UNLINKED"} |

## Casos especiales y variantes
- Multiples sesiones historicas para mismo chat_id: usar la mas reciente con status='linked'.
- chat_id no vinculado nunca: SESSION_NOT_FOUND, bot debe guiar al usuario al proceso de vinculacion.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| TelegramSession | SELECT | chat_id, patient_id, status |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: chat_id vinculado retorna patient_id
  Given existe TelegramSession con chat_id=12345 y status='linked'
  When se resuelve la sesion para chat_id=12345
  Then se retorna el patient_id correspondiente

Scenario: chat_id desvinculado retorna error
  Given no existe TelegramSession activa para chat_id=99999
  When se resuelve la sesion para chat_id=99999
  Then se retorna SESSION_NOT_FOUND
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-011-01 | chat_id vinculado resuelve patient_id | Positivo |
| TP-REG-011-02 | chat_id sin sesion retorna 404 | Negativo |
| TP-REG-011-03 | Sesion con status distinto a linked retorna 403 | Negativo |

## Sin ambiguedades pendientes
Ninguna.
