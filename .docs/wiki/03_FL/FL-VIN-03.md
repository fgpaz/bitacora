# FL-VIN-03: Revocacion de vinculo por paciente

## Estado actual

`Implementado — endpoint DELETE /api/v1/vinculos/{id}`.

La revocacion por paciente existe en runtime bajo `/api/v1/vinculos/{id}`. Requiere cuerpo `{"Confirmed": true}` en el request. El estado resultante es `revoked_by_patient`. La invalidacion de caches sigue diferida.

## Scope
**In:** Revocacion de CareLink, cascade de cache, audit.
**Out:** Revocacion de consentimiento completa (→ FL-CON-02).

## Actores y ownership
| Actor | Rol en el flujo |
|-------|----------------|
| Paciente | Solicita revocacion del vinculo |
| Modulo Vinculos | Cambia estado del CareLink |
| Capa Seguridad | Invalida caches, registra audit |

## Precondiciones
- Paciente autenticado
- CareLink en estado `active`

## Postcondiciones
- CareLink en estado `revoked_by_patient`
- Profesional pierde acceso inmediato
- Caches de safe_projection invalidadas
- AccessAudit registrado

## Secuencia principal

```mermaid
sequenceDiagram
    actor P as Paciente
    participant WEB as Next.js
    participant API as Bitacora.Api
    participant DB as bitacora_db

    P->>WEB: "Revocar vinculo con Dr. X"
    WEB-->>P: Confirmacion: "Dr. X perdera acceso a tus datos"
    P->>WEB: Confirma
    WEB->>API: DELETE /api/v1/vinculos/{link_id} {"Confirmed": true}
    API->>DB: UPDATE CareLink SET status = revoked_by_patient, revoked_at = now()
    API->>DB: INSERT AccessAudit (carelink.revoke, trace_id)
    API-->>WEB: 200 {revoked: true}
    WEB-->>P: "Vinculo revocado. Dr. X ya no puede ver tus datos."
```

## Paths alternativos / errores

| Condicion | Resultado | HTTP |
|-----------|----------|------|
| CareLink ya revocado | Retornar estado actual | 200 |
| CareLink no pertenece al paciente | Rechazo | 403 |

## Architecture slice
- **Modulos:** Auth → Vinculos → Seguridad
- **Invariante:** Revocacion inmediata + invalidacion de cache

## Data touchpoints
| Entidad | Operacion | Estado |
|---------|-----------|--------|
| CareLink | UPDATE | revoked_by_patient |
| AccessAudit | INSERT | append-only |

## RF candidatos
- RF-VIN-020: Revocar CareLink por paciente
- RF-VIN-021: Invalidar caches de safe_projection del profesional
- RF-VIN-022: Verificar ownership del CareLink antes de revocar

## Bottlenecks y mitigaciones
| Riesgo | Mitigacion |
|--------|-----------|
| Profesional accede durante revocacion | Invalidacion de cache sincrona en la tx |

## RF handoff checklist
- [x] Actores y ownership explicitos
- [x] Diagrama explica el flujo sin prosa
- [x] Bottlenecks y mitigaciones explicitos
- [x] Traducible a RF atomicos y testeables
- [x] Dentro del limite de 1 pagina
