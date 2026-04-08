# FL-ONB-01: Onboarding completo del paciente

## Goal
Un nuevo paciente se registra, acepta el consentimiento informado y realiza su primer registro de humor.

## Scope
**In:** Registro de cuenta (Supabase Auth), consentimiento (hard gate), primer mood entry.
**Out:** Vinculacion con profesional (→ FL-VIN-02), configuracion Telegram (→ FL-TG-01).

## Actores y ownership
| Actor | Rol en el flujo |
|-------|----------------|
| Paciente | Se registra, acepta consent, registra primer humor |
| Supabase Auth | Crea cuenta (magic link / Google OAuth) |
| Modulo Auth | Resuelve User desde Supabase |
| Modulo Consent | Presenta y registra consentimiento |
| Modulo Registro | Crea primer MoodEntry |

## Precondiciones
- El paciente accede a bitacora.nuestrascuentitas.com por primera vez
- No tiene cuenta previa

## Postcondiciones
- User creado en bitacora_db
- ConsentGrant en estado `granted`
- Al menos un MoodEntry creado
- Paciente en estado `active`

## Secuencia principal

```mermaid
sequenceDiagram
    actor P as Paciente
    participant WEB as Next.js
    participant SUPA as Supabase Auth
    participant API as Bitacora.Api
    participant DB as bitacora_db

    P->>WEB: Accede a bitacora.nuestrascuentitas.com
    P->>WEB: Click "Registrarme"
    WEB->>SUPA: Iniciar auth (magic link / Google)
    SUPA-->>P: Email con magic link / Google OAuth
    P->>SUPA: Confirma auth
    SUPA-->>WEB: JWT access_token
    WEB->>API: POST /api/v1/auth/bootstrap
    API->>API: Validar JWT → supabase_user_id
    API->>DB: INSERT User (supabase_user_id, email_cifrado)
    API-->>WEB: {user_id, needs_consent: true}

    Note over WEB,P: Pantalla de consentimiento informado (hard gate)
    WEB-->>P: Muestra consentimiento informado completo
    P->>WEB: "Acepto"
    WEB->>API: POST /api/v1/consent {type: informed_consent, accepted: true}
    API->>DB: INSERT ConsentGrant (granted)
    API->>DB: INSERT AccessAudit (consent.granted)
    API-->>WEB: {consent_granted: true}

    Note over WEB,P: Primer registro de humor
    WEB-->>P: "¿Como te sentis hoy?" [keyboard -3..+3]
    P->>WEB: Selecciona humor (+0)
    WEB->>API: POST /api/v1/mood-entries {score: 0}
    API->>DB: INSERT MoodEntry (cifrado)
    API-->>WEB: 201 Created
    WEB-->>P: "Bienvenido a Bitacora. Tu primer registro esta hecho."
```

## Paths alternativos / errores

| Condicion | Resultado |
|-----------|----------|
| Email ya registrado | "Ya tenes cuenta. Inicia sesion." |
| Paciente rechaza consent | No puede registrar datos. Queda en estado `registered` sin `consent_granted`. |
| Paciente cierra la ventana antes del primer mood | Queda con consent pero sin datos. Proximo login → directo al registro. |
| Auth falla (Supabase down) | Fail-closed, pagina de error. |

## Architecture slice
- **Modulos:** Auth (Supabase) → Consent → Registro → Seguridad
- **Flujo compuesto:** integra FL-CON-01 y FL-REG-01

## Data touchpoints
| Entidad | Operacion | Estado resultante |
|---------|-----------|------------------|
| User | INSERT | registered → active |
| ConsentGrant | INSERT | granted |
| MoodEntry | INSERT | created |
| AccessAudit | INSERT x3 | append-only |

## RF candidatos
- RF-ONB-001: Crear User desde JWT de Supabase (bootstrap)
- RF-ONB-002: Detectar usuario nuevo vs existente
- RF-ONB-003: Forzar pantalla de consent antes de cualquier registro
- RF-ONB-004: Registrar primer MoodEntry post-consent
- RF-ONB-005: Transicionar User a estado `active` tras primer mood

## Bottlenecks y mitigaciones
| Riesgo | Mitigacion |
|--------|-----------|
| Magic link lento (email delivery) | UX: mostrar "Revisa tu email" + opcion Google OAuth |
| Abandono en pantalla de consent | El consent es obligatorio; sin el no se puede avanzar |

## RF handoff checklist
- [x] Actores y ownership explicitos
- [x] Diagrama explica el flujo sin prosa
- [x] Bottlenecks y mitigaciones explicitos
- [x] Traducible a RF atomicos y testeables
- [x] Dentro del limite de 2 paginas (flujo compuesto)
