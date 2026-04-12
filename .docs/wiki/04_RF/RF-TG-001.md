# RF-TG-001: Generar pairing code con TTL

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-TG-001 |
| Modulo | TG |
| Endpoint | POST /api/v1/telegram/pairing |
| Actor | Patient (API) |
| Prioridad | Security |
| Estado | **Implementado** via `GeneratePairingCodeCommand` + `POST /api/v1/telegram/pairing` |

## Precondiciones detalladas
- JWT valido con `User.status=active` y `ConsentGrant.status=granted`.
- Solo puede haber un `TelegramPairingCode` activo por `patient_id`; el anterior se invalida.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| patient_id | uuid | JWT | Existente |

## Proceso (Happy Path)
1. Extraer `patient_id` del JWT.
2. Invalidar cualquier `TelegramPairingCode` activo existente para ese paciente.
3. Generar codigo con formato `BIT-XXXXX`.
4. Calcular `expires_at = now() + 15 minutos`.
5. INSERT `TelegramPairingCode(patient_id, code, expires_at, used=false)`.
6. Retornar `{ code, expires_in: 900, expires_at }`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| code | string | Codigo generado con formato `BIT-XXXXX` |
| expires_in | int | Segundos restantes del TTL (`900`) |
| expires_at | timestamp | UTC de expiracion |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| TG_001_UNAUTHORIZED | 401 | JWT invalido | {error: "TG_001_UNAUTHORIZED"} |
| TG_001_CONSENT_REQUIRED | 403 | Consentimiento no vigente | {error: "TG_001_CONSENT_REQUIRED"} |

## Casos especiales y variantes
- Paciente ya vinculado: puede generar nuevo codigo para relink.
- Si existe codigo activo previo, se invalida antes de insertar el nuevo.
- Colision de codigo: regenerar hasta obtener uno unico.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| TelegramPairingCode | UPDATE (condicional) | used=true en codigo activo previo |
| TelegramPairingCode | INSERT | patient_id, code, expires_at, used |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente genera codigo de pairing
  Given paciente autenticado con consentimiento vigente
  When POST /api/v1/telegram/pairing
  Then HTTP 200 con code="BIT-XXXXX"
  And expires_in=900

Scenario: Paciente sin consentimiento no puede generar codigo
  Given paciente con ConsentGrant.status=revoked
  When POST /api/v1/telegram/pairing
  Then HTTP 403 con TG_001_CONSENT_REQUIRED
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-TG-001-01 | Genera codigo con formato BIT-XXXXX | Positivo |
| TP-TG-001-02 | Invalida codigo previo al generar uno nuevo | Positivo |
| TP-TG-001-03 | Rechaza generacion sin consentimiento | Negativo |

## Sin ambiguedades pendientes
Ninguna.
