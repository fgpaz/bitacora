# FL-TG-01: Vinculacion de cuenta Telegram

## Goal
El paciente vincula su cuenta de Telegram con su cuenta de Bitacora para habilitar el registro via bot.

## Scope
**In:** Generacion de codigo de vinculacion, envio al bot, confirmacion.
**Out:** Registro via Telegram (→ FL-REG-02), recordatorios (→ FL-TG-02).

## Actores y ownership
| Actor | Rol en el flujo |
|-------|----------------|
| Paciente | Genera codigo en web, lo envia al bot |
| Modulo Auth | Valida JWT |
| Modulo Telegram | Genera codigo, recibe /start, vincula chat_id |

## Precondiciones
- Paciente autenticado en web
- Bot de Telegram activo (@BitacoraBot)

## Postcondiciones
- TelegramSession creada en estado `linked` con chat_id
- Paciente puede registrar humor via Telegram

## Secuencia principal

```mermaid
sequenceDiagram
    actor P as Paciente
    participant WEB as Next.js
    participant API as Bitacora.Api
    participant DB as bitacora_db
    participant TG as Telegram

    P->>WEB: "Vincular Telegram"
    WEB->>API: POST /api/v1/telegram/pairing
    API->>DB: INSERT TelegramPairingCode (patient_id, code, expires_at: +15min)
    API-->>WEB: {code: "BIT-A7X3K2", expires_in: 900}
    WEB-->>P: "Envia este mensaje al bot: /start BIT-A7X3K2"

    P->>TG: /start BIT-A7X3K2
    TG->>API: POST /api/v1/telegram/webhook {message: "/start BIT-A7X3K2"}
    API->>DB: SELECT TelegramPairingCode WHERE code = "BIT-A7X3K2" AND NOT expired
    API->>DB: INSERT TelegramSession (patient_id, chat_id, linked)
    API->>DB: DELETE TelegramPairingCode
    API->>DB: INSERT AccessAudit (telegram.linked, trace_id)
    API-->>TG: "Cuenta vinculada. Ya podes registrar tu humor desde aca."
    TG-->>P: Confirmacion + keyboard inline de humor
```

## Paths alternativos / errores

| Condicion | Resultado |
|-----------|----------|
| Codigo expirado | "Codigo expirado. Genera uno nuevo en la web." |
| Codigo invalido | "Codigo no valido." |
| chat_id ya vinculado a otro paciente | "Este Telegram ya esta vinculado a otra cuenta." |

## Architecture slice
- **Modulos:** Auth → Telegram
- **Patron:** Pairing code con TTL (15 min)

## Data touchpoints
| Entidad | Operacion | Estado |
|---------|-----------|--------|
| TelegramPairingCode | INSERT → DELETE | temporal |
| TelegramSession | INSERT | linked |
| AccessAudit | INSERT | append-only |

## RF candidatos
- RF-TG-001: Generar pairing code con TTL
- RF-TG-002: Vincular chat_id via /start + code
- RF-TG-003: Validar unicidad de chat_id por paciente

## Bottlenecks y mitigaciones
| Riesgo | Mitigacion |
|--------|-----------|
| Fuerza bruta de codigos | Formato BIT-XXXXX (alfanumerico 5 chars = 60M combinaciones) + TTL 15min + rate limit |

## RF handoff checklist
- [x] Actores y ownership explicitos
- [x] Diagrama explica el flujo sin prosa
- [x] Bottlenecks y mitigaciones explicitos
- [x] Traducible a RF atomicos y testeables
- [x] Dentro del limite de 1 pagina
