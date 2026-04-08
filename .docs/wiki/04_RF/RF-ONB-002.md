# RF-ONB-002: Detectar usuario nuevo vs existente

## Execution Sheet
- Modulo: ONB
- Trigger: Invocado por RF-ONB-001 al recibir un bootstrap request
- Actor: Sistema
- Prioridad PDP: Correctness > Usability

## Precondiciones detalladas
- `supabase_user_id` extraido del JWT es el identificador canonico
- La deteccion determina el flujo subsiguiente del onboarding
- Los estados posibles de User son: registered, consent_granted, active

## Inputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| supabase_user_id | string | Extraido del JWT (campo sub) |

## Proceso (Happy Path)
1. Query: `SELECT user_id, status FROM users WHERE supabase_user_id = @sub LIMIT 1`
2. Si no existe fila: usuario nuevo → retornar `{ is_new: true }`
3. Si existe y `status = active`: usuario activo → retornar `{ is_new: false, status: "active", user_id }`
4. Si existe y `status = consent_granted`: consent dado pero no activo → retornar `{ is_new: false, status: "consent_granted", needs_first_entry: true }`
5. Si existe y `status = registered`: necesita consent → retornar `{ is_new: false, status: "registered", needs_consent: true }`

## Outputs
```json
// Usuario nuevo
{ "is_new": true }

// Usuario activo
{ "is_new": false, "status": "active", "user_id": "uuid" }

// Usuario con consent, sin primer entry
{ "is_new": false, "status": "consent_granted", "needs_first_entry": true }

// Usuario registrado sin consent
{ "is_new": false, "status": "registered", "needs_consent": true }
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| ONB_002_DB_ERROR | 500 | Error al consultar users |

## Casos especiales y variantes
- Estado `registered` sin ConsentGrant asociado: normal, el grant se crea en paso posterior
- Multiple rows para mismo supabase_user_id: no deberia ocurrir (UNIQUE constraint); si ocurre, loguear y retornar la mas reciente

## Impacto en modelo de datos
- Solo lectura sobre `users`

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Usuario nuevo detectado correctamente
  Given supabase_user_id no existe en DB
  When se ejecuta la deteccion
  Then retorna is_new=true

Scenario: Usuario activo detectado
  Given User con status=active para ese sub
  When se ejecuta la deteccion
  Then retorna is_new=false, status=active

Scenario: Usuario con consent_granted pero sin primer entry
  Given User con status=consent_granted
  When se ejecuta la deteccion
  Then retorna needs_first_entry=true

Scenario: Usuario con status=registered sin consent
  Given User con status=registered
  When se ejecuta la deteccion
  Then retorna needs_consent=true
```

## Trazabilidad de tests
- UT: ONB002_NewUser_IsNewTrue
- UT: ONB002_AllStatuses_MappedCorrectly
- IT: ONB002_UniqueConstraint_OnSupabaseUserId

## Sin ambiguedades pendientes
- Los 4 casos de deteccion son exhaustivos; no existen otros estados validos de User
- La deteccion es read-only; no modifica ningun estado
