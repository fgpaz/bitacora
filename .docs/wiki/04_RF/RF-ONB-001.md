# RF-ONB-001: Crear User desde JWT de Supabase (bootstrap)

## Execution Sheet
| Campo | Valor |
|-------|-------|
| ID | RF-ONB-001 |
| Modulo | ONB |
| Endpoint | POST /api/v1/auth/bootstrap |
| Actor | Patient (API) |
| Prioridad | Security |

## Precondiciones detalladas
- JWT emitido por Supabase, firmado y no expirado.
- El JWT contiene `sub` y `email` si el proveedor lo provee.
- Si el `User` ya existe en DB local, no debe duplicarse.
- El email se cifra antes de persistirse y registra `key_version`.

## Inputs
| Campo | Tipo | Origen | Validacion |
|-------|------|--------|-----------|
| JWT | string | Authorization header | Valido y no expirado |
| invite_token | string | Query string / contexto de onboarding (opcional) | Token opaco, si se provee |

## Proceso (Happy Path)
1. Validar y decodificar el JWT de Supabase.
2. Extraer `supabase_user_id` y `email`.
3. Verificar si ya existe `User WHERE supabase_user_id=@sub`.
4. Si no existe:
   a. Cifrar email con la clave de aplicacion vigente.
   b. INSERT `User(supabase_user_id, role='patient', status='registered', encrypted_email, email_hash, key_version)`.
5. Si `invite_token` esta presente, resolver si existe `PendingInvite` vigente compatible con el email del usuario.
6. Retornar el contexto de bootstrap con `needs_consent=true` y `resume_pending_invite=true/false`.

## Outputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| user_id | uuid | ID interno del usuario |
| status | string | `registered` o estado existente |
| needs_consent | bool | Siempre `true` si aun no otorgo consentimiento |
| resume_pending_invite | bool | Indica si existe una invitacion pendiente reanudable |

## Errores tipados
| Codigo | HTTP | Trigger | Respuesta |
|--------|------|---------|----------|
| ONB_001_JWT_INVALID | 401 | JWT malformado o firma invalida | {error: "ONB_001_JWT_INVALID"} |
| ONB_001_JWT_EXPIRED | 401 | JWT expirado | {error: "ONB_001_JWT_EXPIRED"} |
| ONB_001_ENCRYPT_FAILED | 500 | Error al cifrar email | {error: "ONB_001_ENCRYPT_FAILED"} |

## Casos especiales y variantes
- `invite_token` invalido o expirado no bloquea el bootstrap; se retorna `resume_pending_invite=false`.
- Si el usuario ya existia, se reutiliza el registro local y no se crea duplicado.

## Impacto en modelo de datos
| Entidad | Operacion | Campos afectados |
|---------|-----------|-----------------|
| User | INSERT (condicional) | supabase_user_id, role, status, encrypted_email, email_hash, key_version |
| PendingInvite | SELECT (condicional) | invite_token, invitee_email_hash, status, expires_at |

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Nuevo usuario crea registro y reanuda invitacion pendiente
  Given JWT valido con sub nuevo no existente en DB
  And existe un PendingInvite vigente para el email del JWT
  When POST /api/v1/auth/bootstrap
  Then HTTP 200 con status="registered"
  And needs_consent=true
  And resume_pending_invite=true

Scenario: Usuario existente no genera duplicado
  Given JWT con sub ya existente en DB
  When POST /api/v1/auth/bootstrap
  Then HTTP 200 con contexto del usuario existente
  And no se crea un nuevo registro
```

## Trazabilidad de tests
| TP ID | Escenario | Tipo |
|-------|-----------|------|
| TP-ONB-001-01 | Crea usuario nuevo con email cifrado y key_version | Positivo |
| TP-ONB-001-02 | Reanuda invitacion pendiente valida | Positivo |
| TP-ONB-001-03 | No duplica usuario existente | Negativo |

## Sin ambiguedades pendientes
Ninguna.
