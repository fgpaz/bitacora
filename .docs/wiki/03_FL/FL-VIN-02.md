# FL-VIN-02: Auto-vinculacion paciente a profesional

## Estado actual

`Parcialmente implementado — endpoint POST /api/v1/vinculos/accept existe; generacion de BindingCode por profesional queda diferida`.

El flujo de auto-vinculacion existe parcialmente via `POST /api/v1/vinculos/accept`. El paciente puede vincularse con un `BindingCode` valido que el profesional comparta. La generacion del codigo por el profesional (`POST /api/v1/professional/binding-codes`) queda diferida.

## Goal
Un paciente se vincula a un profesional usando un `BindingCode` efimero que el profesional comparte.

## Scope
**In:** Paciente ingresa codigo, se crea CareLink directo.
**Out:** Invitacion del profesional (→ FL-VIN-01), revocacion (→ FL-VIN-03).

## Actores y ownership
| Actor | Rol en el flujo |
|-------|----------------|
| Paciente | Ingresa codigo de vinculacion |
| Profesional | Emite el codigo desde su dashboard y elige su TTL por emision |
| Modulo Vinculos | Valida codigo, crea CareLink |
| Capa Seguridad | Audit |

## Precondiciones
- Paciente autenticado con ConsentGrant granted
- Profesional tiene un `BindingCode` activo

## Postcondiciones
- CareLink creado en estado `active`
- `can_view_data` default `false`
- `BindingCode` marcada como `used`
- AccessAudit registrado

## Secuencia principal

```mermaid
sequenceDiagram
    actor P as Paciente
    participant WEB as Next.js
    participant API as Bitacora.Api
    participant DB as bitacora_db

    P->>WEB: "Vincular con profesional" → ingresa codigo
    WEB->>API: POST /api/v1/care-links/bind {code: "BIT-ABC12"}
    API->>API: Validar codigo → resolver professional_id
    alt Codigo invalido o expirado
        API-->>WEB: 404 INVALID_CODE
        WEB-->>P: "Codigo invalido. Pedi un nuevo codigo a tu profesional."
    end
    API->>API: Verificar que no exista CareLink activo con este profesional
    API->>DB: INSERT CareLink (patient_id, professional_id, active, can_view_data: false)
    API->>DB: INSERT AccessAudit (carelink.created, trace_id)
    API-->>WEB: 201 {link_id, professional_name, can_view_data: false}
    WEB-->>P: "Vinculado con Dr. X. Activa el acceso a tus datos cuando quieras."
```

## Paths alternativos / errores

| Condicion | Resultado | HTTP |
|-----------|----------|------|
| Codigo invalido/expirado | Rechazo con mensaje | 404 |
| CareLink ya existe | Retornar existente | 409 |
| Profesional inactivo | Rechazo | 404 |

## Architecture slice
- **Modulos:** Auth → Vinculos → Seguridad
- **Patron:** `BindingCode` por emision con presets `15m / 3h / 24h / 72h`, default `15m`

## Data touchpoints
| Entidad | Operacion | Estado |
|---------|-----------|--------|
| BindingCode | INSERT → UPDATE | issued → used / expired |
| CareLink | INSERT | active (can_view_data: false) |
| AccessAudit | INSERT | append-only |

## RF candidatos
- RF-VIN-004: `can_view_data` default false (invariante compartida)
- RF-VIN-010: Generar BindingCode para auto-vinculacion
- RF-VIN-011: Validar BindingCode y resolver professional_id
- RF-VIN-012: Crear CareLink por auto-vinculacion

## Bottlenecks y mitigaciones
| Riesgo | Mitigacion |
|--------|-----------|
| Fuerza bruta de codigos | Formato `BIT-XXXXX` + rate limit + expiracion por emision |

## RF handoff checklist
- [x] Actores y ownership explicitos
- [x] Diagrama explica el flujo sin prosa
- [x] Bottlenecks y mitigaciones explicitos
- [x] Traducible a RF atomicos y testeables
- [x] Dentro del limite de 1 pagina
