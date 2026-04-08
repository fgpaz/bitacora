# RF-ONB-001: Crear User desde JWT de Supabase

## Execution Sheet
- Modulo: ONB
- Endpoint: POST /api/v1/auth/bootstrap
- Actor: Usuario recien registrado (JWT de Supabase valido)
- Prioridad PDP: Security > Privacy > Correctness

## Precondiciones detalladas
- JWT emitido por Supabase, firmado y no expirado
- El JWT contiene `sub` (supabase_user_id) y `email`
- Si el User ya existe en la DB local, no duplicar (ver RF-ONB-002)
- El email debe cifrarse antes de persistir

## Inputs
| Campo | Origen | Descripcion |
|-------|--------|-------------|
| JWT | Authorization header (Bearer) | Token de Supabase post-registro |

## Proceso (Happy Path)
1. Validar y decodificar JWT de Supabase
2. Extraer `supabase_user_id` (campo `sub`) y `email`
3. Verificar si ya existe `User WHERE supabase_user_id = @sub`
4. Si existe: delegar a RF-ONB-002 (retornar contexto existente)
5. Si no existe (nuevo usuario):
   a. Cifrar email con clave de aplicacion (AES-256-GCM, key_version actual)
   b. Crear `User(supabase_user_id, role=patient, status=registered, encrypted_email)`
   c. INSERT en `users`
6. Retornar `{ user_id, status: "registered", needs_consent: true }`

## Outputs
```json
{
  "user_id": "internal-uuid",
  "status": "registered",
  "needs_consent": true
}
```

## Errores tipados
| Codigo | HTTP | Descripcion |
|--------|------|-------------|
| ONB_001_JWT_INVALID | 401 | JWT malformado o firma invalida |
| ONB_001_JWT_EXPIRED | 401 | JWT expirado |
| ONB_001_ENCRYPT_FAILED | 500 | Error al cifrar email; no persistir en claro |

## Casos especiales y variantes
- Usuario ya existente: delegar a RF-ONB-002 sin crear duplicado
- Email ausente en JWT (proveedor OAuth sin email): persistir como null cifrado
- Rol asignado siempre es `patient` en el bootstrap; no puede cambiarse aqui

## Impacto en modelo de datos
- INSERT en `users` con: supabase_user_id, role=patient, status=registered, encrypted_email, key_version

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Nuevo usuario crea registro con status=registered
  Given JWT valido con sub nuevo no existente en DB
  When POST /api/v1/auth/bootstrap
  Then HTTP 200 con status=registered y needs_consent=true
  And usuario creado en DB con role=patient y email cifrado

Scenario: Usuario existente no genera duplicado
  Given JWT con sub ya existente en DB
  When POST /api/v1/auth/bootstrap
  Then HTTP 200 con contexto del usuario existente
  And no se crea nuevo registro en users

Scenario: JWT expirado es rechazado
  When POST con JWT expirado
  Then HTTP 401 con ONB_001_JWT_EXPIRED
```

## Trazabilidad de tests
- UT: ONB001_NewUser_CreatedWithRegisteredStatus
- UT: ONB001_EmailEncrypted_BeforePersist
- IT: ONB001_ExistingUser_NoDuplicate
- IT: ONB001_ExpiredJWT_Returns401

## Sin ambiguedades pendientes
- El email nunca se persiste en texto claro, ni en logs
- `role=patient` es el unico rol asignable en bootstrap
