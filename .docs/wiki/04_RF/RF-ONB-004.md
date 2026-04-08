# RF-ONB-004: Registrar primer MoodEntry post-consent

## Execution Sheet
- Modulo: ONB
- Endpoint: POST /api/v1/mood-entries (primer uso post-consent)
- Actor: Patient con User.status=consent_granted
- Prioridad PDP: Correctness > Usability

## Precondiciones detalladas
- User.status = consent_granted (consent otorgado, nunca ha creado un MoodEntry)
- ConsentGrant.status = granted
- El flujo de onboarding dirige al usuario a esta pantalla tras el consent (RF-ONB-003)
- Este primer MoodEntry es funcionalmente identico a cualquier otro; no tiene campo especial

## Inputs
| Campo | Tipo | Requerido | Descripcion |
|-------|------|-----------|-------------|
| mood_score | int [-3..3] | Si | Puntuacion de animo |
| channel | string | No | Origen del entry. Default: "web" |
| noted_at | datetime (ISO8601) | No | Default: now() |

## Proceso (Happy Path)
1. Validar JWT: User.status=consent_granted o active (ambos pueden crear entries)
2. Validar `mood_score` en rango [-3, 3]
3. Construir MoodEntry:
   - patient_id del JWT
   - mood_score (persistir en safe_projection)
   - channel (default "web")
   - created_at = now() UTC
   - Cifrar payload completo con clave actual (key_version)
4. INSERT en `mood_entries`
5. Si es el primer entry del paciente (count=0 antes del insert), emitir evento interno `FirstMoodEntryCreated`
6. El evento triggerea RF-ONB-005 para transicionar User.status a active
7. Retornar el entry creado (solo safe_projection)

## Outputs
```json
{
  "entry_id": "uuid",
  "mood_score": 2,
  "channel": "web",
  "created_at": "2026-04-07T10:00:00Z"
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| ONB_004_MOOD_SCORE_INVALID | 400 | mood_score fuera de rango [-3, 3] |
| ONB_004_CONSENT_REQUIRED | 403 | ConsentGrant no vigente |
| ONB_004_UNAUTHORIZED | 401 | JWT invalido |

## Casos especiales y variantes
- Usuario ya activo (status=active) creando un entry: mismo flujo, sin evento FirstMoodEntryCreated
- Evento FirstMoodEntryCreated es idemponente: si ya existen entries, no se emite
- Error en transicion de estado (RF-ONB-005) no debe revertir la creacion del entry

## Impacto en modelo de datos
- INSERT en `mood_entries`
- Puede triggear UPDATE en `users` (via RF-ONB-005)

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Primer MoodEntry creado exitosamente
  Given User.status=consent_granted y sin entries previos
  When POST /api/v1/mood-entries con mood_score=2
  Then HTTP 201 con safe_projection del entry
  And evento FirstMoodEntryCreated emitido

Scenario: mood_score fuera de rango es rechazado
  When POST con mood_score=5
  Then HTTP 400 con ONB_004_MOOD_SCORE_INVALID

Scenario: Usuario activo crea entry sin triggear evento
  Given User.status=active con entries previos
  When POST /api/v1/mood-entries
  Then HTTP 201 sin evento FirstMoodEntryCreated
```

## Trazabilidad de tests
- UT: ONB004_MoodScoreValidation_Range
- UT: ONB004_FirstEntry_EmitsEvent
- UT: ONB004_SubsequentEntry_NoEvent
- IT: ONB004_PayloadCiphered_BeforePersist

## Sin ambiguedades pendientes
- "Primer entry" se determina por COUNT de mood_entries del paciente antes del INSERT
- El cifrado del payload ocurre antes del INSERT, nunca despues
