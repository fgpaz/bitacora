# RF-TG-001: Generar pairing code con TTL

## Execution Sheet
- Modulo: TG
- Endpoint: POST /api/v1/telegram/pairing
- Actor: Patient (autenticado via JWT)
- Prioridad PDP: Security > Usability

## Precondiciones detalladas
- JWT valido con User.status=active y ConsentGrant.status=granted
- El paciente no debe tener ya una TelegramSession con status=linked (o puede tener una nueva si quiere re-vincular)
- Solo puede haber un codigo de pairing activo por patient_id (el anterior se invalida)

## Inputs
- Sin body. patient_id extraido del JWT.

## Proceso (Happy Path)
1. Extraer patient_id del JWT
2. Invalidar cualquier TelegramPairingCode activo existente para este patient_id
3. Generar codigo: formato `BIT-XXXXX` donde XXXXX es 5 caracteres alfanumericos uppercase (A-Z0-9)
4. Calcular `expires_at = now() + 15 minutos`
5. Persistir `TelegramPairingCode(patient_id, code, expires_at)`
6. Retornar `{ code, expires_in: 900 }`

## Outputs
```json
{
  "code": "BIT-A3K9Z",
  "expires_in": 900
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| TG_001_UNAUTHORIZED | 401 | JWT invalido |
| TG_001_CONSENT_REQUIRED | 403 | ConsentGrant no vigente |

## Casos especiales y variantes
- Paciente ya vinculado: se puede generar nuevo codigo (permite re-vincular desde otro chat_id)
- Codigo generado anteriormente aun activo: se invalida y se genera uno nuevo
- Colision de codigo: regenerar hasta obtener uno unico (probabilidad negligible)

## Impacto en modelo de datos
- DELETE o UPDATE `telegram_pairing_codes` (invalida el anterior)
- INSERT `telegram_pairing_codes` con nuevo codigo y TTL

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Paciente genera codigo de pairing
  Given paciente autenticado con consent vigente
  When POST /api/v1/telegram/pairing
  Then HTTP 200 con code="BIT-XXXXX" y expires_in=900

Scenario: Codigo anterior es invalidado al generar uno nuevo
  Given paciente con codigo activo BIT-AAA11
  When POST /api/v1/telegram/pairing
  Then BIT-AAA11 queda invalido en DB
  And nuevo codigo retornado

Scenario: Paciente sin consent no puede generar codigo
  Given paciente con ConsentGrant.status=revoked
  When POST /api/v1/telegram/pairing
  Then HTTP 403 con TG_001_CONSENT_REQUIRED
```

## Trazabilidad de tests
- UT: TG001_CodeFormat_BIT_XXXXX
- UT: TG001_PreviousCode_Invalidated
- IT: TG001_ConsentRevoked_Returns403
- IT: TG001_ExpiresAt_15Minutes

## Sin ambiguedades pendientes
- El formato es exactamente `BIT-` seguido de 5 caracteres; no 4, no 6
- `expires_in` siempre retorna 900 (segundos), independiente de cuando se creo el codigo
