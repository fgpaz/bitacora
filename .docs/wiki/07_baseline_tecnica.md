# 07 — Baseline Tecnica

## Servicio

| Campo | Valor |
|-------|-------|
| Nombre | Bitacora.Api |
| Tipo | Monolito modular |
| Runtime | .NET 10 |
| Template | `sandinas-ms10` (local full skeleton) |
| Subagente | ps-dotnet10 |
| Puerto local | `5254` HTTP / `7281` HTTPS via `launchSettings.json` |
| Health check | GET /health |

## Estado actual de implementación

- `Wave 1` implementada y runnable localmente.
- Alcance materializado hoy: `Auth`, `Consent`, `Registro` y `Seguridad`.
- `Vinculos`, `Visualizacion`, `Telegram` y `Export` siguen como modulos del canon, pero no tienen runtime efectivo aun.

## Modulos internos

| Modulo | Responsabilidad | Entidades principales | Estado |
|--------|----------------|----------------------|--------|
| Auth | Validar JWT Supabase, resolver identidad, bootstrap de paciente | User | Implementado |
| Registro | Crear MoodEntry y DailyCheckin, cifrar, `safe_projection` | MoodEntry, DailyCheckin | Implementado |
| Consent | Hard gate, otorgamiento y revocacion de consentimiento | ConsentGrant | Implementado |
| Vinculos | Detectar `PendingInvite` reanudable desde onboarding | PendingInvite | Parcial |
| Visualizacion | Queries sobre `safe_projection` y lectura profesional via `patient_ref` opaco | (queries) | Diferido |
| Telegram | Webhook/polling, pairing, sesiones y recordatorios | TelegramSession, TelegramPairingCode | Diferido |
| Export | Generar CSV, descifrar payload bajo demanda | (generacion) | Diferido |
| Seguridad | Cifrado AES-GCM, audit, pseudonimizacion, fail-closed | AccessAudit, EncryptionKeyVersion | Implementado |

## Infraestructura

| Componente | Tecnologia | Ubicacion |
|-----------|-----------|-----------|
| Backend | .NET 10 (Bitacora.Api) | Local runnable hoy; Dokploy planeado en VPS 54.37.157.93 |
| Frontend | Next.js 16 | Planeado, sin runtime en esta ola |
| Base de datos | PostgreSQL (`bitacora_db` dedicada) | Local/dev y luego mismo VPS |
| Auth | Supabase Auth (GoTrue) | auth.tedi.nuestrascuentitas.com |
| Reverse proxy | Traefik (via Dokploy) | Planeado |
| Dominio | bitacora.nuestrascuentitas.com | Planeado |

## Observabilidad

| Aspecto | Implementacion |
|---------|---------------|
| Logs | `Console` + `Debug` providers del host .NET |
| Tracing | OpenTelemetry con `trace_id` end-to-end (T3-13) |
| Pseudonimizacion | `pseudonym_id` en logs operacionales; `actor_id` solo en `AccessAudit` |
| Health | `/health` endpoint; smoke local verificado |

## Invariantes operacionales

1. Fail-closed (T3-10): toda falla de seguridad bloquea la operacion.
2. `trace_id` obligatorio (T3-13): se genera al ingreso si no existe.
3. Cifrado app-layer (T3-5): `encrypted_payload + safe_projection`. Ver `07_tech/TECH-CIFRADO.md`.
4. Telegram dual mode (T3-3): documentado, pero diferido de runtime. Ver `07_tech/TECH-TELEGRAM.md`.
5. Global Query Filters (T3-6): EF Core filtra por `patient_id` automaticamente en `MoodEntry` y `DailyCheckin`.
6. Consentimiento activo en config: el texto vigente no se persiste en DB; en DB solo queda la evidencia aceptada (`ConsentGrant`).
7. `patient_ref` es una proyeccion opaca de API, no un campo persistido del modelo.

## Detail docs

| Doc | Tema |
|-----|------|
| `07_tech/TECH-CIFRADO.md` | Patron encrypted_payload + safe_projection + key rotation |
| `07_tech/TECH-TELEGRAM.md` | Modos webhook/polling, flujo conversacional, pairing y recordatorios |
| `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` | Gramática técnica de la futura UI Next.js 16, estados, accesibilidad y trazabilidad Stitch |

## Sync gates

Cambios en 07 pueden forzar revision de:

- `04_RF/RF-SEC-*` si cambia fail-closed o pseudonimizacion
- `04_RF/RF-REG-*` si cambia cifrado o Global Query Filters
- `09_contratos_tecnicos.md` si cambia auth o endpoints
- `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` si cambian error envelopes, auth, estados sensibles o reglas frontend de acceso

---

*Fuente: `.docs/raw/decisiones/02_decisiones_arquitectura.md`*
