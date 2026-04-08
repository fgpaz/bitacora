# RF-REG-013: Flujo secuencial de factores via Telegram

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-REG-013 |
| Modulo | REG |
| Actor | Patient via Telegram Bot |
| Flujo fuente | FL-REG-02 |
| Prioridad | Usability |

## Precondiciones detalladas
- MoodEntry creado exitosamente por RF-REG-012.
- patient_id resuelto y con consent activo.
- Bot tiene permisos de envio de mensajes con InlineKeyboard.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | Contexto de sesion | Verificado |
| mood_entry_id | uuid | RF-REG-012 | Referencia al registro recien creado |
| respuestas_secuenciales | callbacks | Telegram InlineKeyboard | Una por pregunta |

## Proceso (Happy Path)
1. Tras confirmar MoodEntry, bot envia pregunta 1: horas de sueno (opciones: <5, 5-7, 7-9, >9).
2. Recibir callback, almacenar sleep_hours temporalmente en sesion.
3. Preguntar actividad fisica (si/no).
4. Preguntar actividad social (si/no).
5. Preguntar ansiedad presente (si/no).
6. Preguntar irritabilidad presente (si/no).
7. Preguntar toma de medicacion (si/no). Si si, preguntar hora.
8. Con todos los campos, invocar RF-REG-022 (UPSERT DailyCheckin).
9. Confirmar registro al usuario.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| daily_checkin_id | uuid | ID del DailyCheckin creado/actualizado |
| confirmacion_bot | string | Mensaje de confirmacion al usuario |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| SESSION_TIMEOUT | 408 | Usuario no responde en 10 min | Bot cancela flujo y notifica |
| CHECKIN_FAILED | 500 | Fallo en RF-REG-022 | Bot notifica error, datos parciales descartados |

## Casos especiales y variantes
- Usuario abandona el flujo a mitad: no crear DailyCheckin parcial. Descartar estado de sesion.
- Usuario responde fuera de orden (mensaje libre): ignorar, reenviar pregunta actual.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| DailyCheckin | INSERT/UPDATE via RF-REG-022 | Todos los campos |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Flujo completo crea DailyCheckin
  Given MoodEntry creado para el patient
  When el usuario responde las 6 preguntas secuenciales
  Then se crea DailyCheckin con todos los campos
  And el bot confirma el registro

Scenario: Flujo abandonado no crea registro parcial
  Given el flujo secuencial inicio
  When el usuario no responde en 10 minutos
  Then no se crea DailyCheckin
  And el bot cancela el flujo
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-REG-013-01 | Flujo completo genera DailyCheckin | Positivo |
| TP-REG-013-02 | Timeout cancela flujo sin registro parcial | Negativo |
| TP-REG-013-03 | Respuesta libre fuera de flujo es ignorada | Negativo |

## Sin ambiguedades pendientes
Ninguna.
