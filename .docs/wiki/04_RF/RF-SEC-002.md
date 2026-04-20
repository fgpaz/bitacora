# RF-SEC-002: Generar pseudonym_id

## Execution Sheet
- Modulo: SEC
- Trigger: Invocado por RF-SEC-001 al construir cada AccessAudit
- Actor: Sistema
- Prioridad PDP: Security > Privacy (deterministico, sin tabla de mapeo)

## Precondiciones detalladas
- `env_salt` disponible como variable de entorno (SECRET_AUDIT_SALT o equivalente)
- Si `env_salt` no esta disponible, la aplicacion NO debe iniciar (fail en startup)
- No existe tabla de mapeo pseudonym_id → actor_id en ningun lugar del sistema

## Inputs
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| actor_id | string | auth_subject del profesional |
| env_salt | string | Salt del entorno (secreto, no en codigo) |

## Proceso (Happy Path)
1. Leer `env_salt` desde configuracion/vault
2. Si `env_salt` es null o vacio → lanzar excepcion en startup (no en runtime)
3. Calcular: `pseudonym_id = SHA256(actor_id + env_salt)` como hex string lowercase
4. Retornar `pseudonym_id`

## Outputs
```
pseudonym_id = "a3f2e1..." (64 caracteres hex, SHA256)
```

## Errores tipados
| Codigo | Descripcion |
|--------|-------------|
| SEC_002_SALT_MISSING | env_salt no configurado; fallo en startup de aplicacion |
| SEC_002_ACTOR_NULL | actor_id es null; excepcion, no generar pseudonym |

## Casos especiales y variantes
- Mismo actor siempre genera el mismo pseudonym_id (deterministico con salt fijo)
- Si el salt cambia: todos los pseudonym_id anteriores son incompatibles (rompe correlacion historica)
- No hay forma de revertir pseudonym_id → actor_id sin el salt (privacidad por diseno)
- El pseudonym_id se usa en logs y audits pero nunca se expone en APIs publicas

## Impacto en modelo de datos
- Solo escritura en `access_audits.pseudonym_id`
- No crea ninguna tabla de mapeo

## Criterios de aceptacion (Gherkin)
```gherkin
Scenario: Pseudonym generado correctamente
  Given actor_id="user-123" y env_salt="secreto"
  When se calcula pseudonym_id
  Then resultado es SHA256("user-123secreto") como hex lowercase
  And la misma llamada con los mismos inputs siempre da el mismo resultado

Scenario: env_salt ausente impide iniciar la aplicacion
  Given SECRET_AUDIT_SALT no esta en variables de entorno
  When la aplicacion intenta iniciar
  Then la aplicacion falla en startup con mensaje claro

Scenario: actor_id null lanza excepcion
  Given actor_id=null
  When se invoca la funcion
  Then excepcion SEC_002_ACTOR_NULL, no se genera pseudonym
```

## Trazabilidad de tests
- UT: SEC002_SHA256_Deterministic
- UT: SEC002_NullActorId_Throws
- IT: SEC002_SaltMissing_AppFailsOnStartup

## Sin ambiguedades pendientes
- Algoritmo: SHA256 estandar (no HMAC, no bcrypt)
- Concatenacion: `actor_id + env_salt` (sin separador)
- Output: hex string lowercase de 64 caracteres
