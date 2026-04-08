# 07 — Baseline Tecnica

## Servicio

| Campo | Valor |
|-------|-------|
| Nombre | Bitacora.Api |
| Tipo | Monolito modular |
| Runtime | .NET 10 |
| Template | ps.microservice.net10.fullskeleton.1.0.0 |
| Subagente | ps-dotnet10 |
| Puerto local | 5300 (API) |
| Health check | GET /health |

## Modulos internos

| Modulo | Responsabilidad | Entidades principales |
|--------|----------------|----------------------|
| Auth | Validar JWT Supabase, resolver identidad, inyectar contexto | User |
| Registro | Crear MoodEntry y DailyCheckin, cifrar, safe_projection | MoodEntry, DailyCheckin |
| Consent | Hard gate, otorgamiento/revocacion de consentimiento | ConsentGrant |
| Vinculos | Crear/revocar CareLink, validar acceso profesional | CareLink |
| Visualizacion | Queries sobre safe_projection, timeline, dashboard | (queries) |
| Telegram | Webhook/polling, keyboard inline, recordatorios | TelegramSession |
| Export | Generar CSV, descifrar payload bajo demanda | (generacion) |
| Seguridad | Cifrado AES, audit, pseudonimizacion, fail-closed | AccessAudit, EncryptionKeyVersion |

## Infraestructura

| Componente | Tecnologia | Ubicacion |
|-----------|-----------|-----------|
| Backend | .NET 10 (Bitacora.Api) | Dokploy VPS 54.37.157.93 |
| Frontend | Next.js 16 | Dokploy VPS 54.37.157.93 |
| Base de datos | PostgreSQL (bitacora_db dedicada) | Mismo VPS |
| Auth | Supabase Auth (GoTrue) | auth.tedi.nuestrascuentitas.com |
| Reverse proxy | Traefik (via Dokploy) | Mismo VPS |
| Dominio | bitacora.nuestrascuentitas.com | Cloudflare DNS |

## Observabilidad

| Aspecto | Implementacion |
|---------|---------------|
| Logs | Structured JSON (Serilog) |
| Tracing | OpenTelemetry con trace_id end-to-end (T3-13) |
| Pseudonimizacion | pseudonym_id en logs operacionales (T3-8). actor_id solo en AccessAudit. |
| Health | /health endpoint + Dokploy health check |

## Invariantes operacionales

1. **Fail-closed** (T3-10): toda falla de seguridad bloquea la operacion.
2. **trace_id obligatorio** (T3-13): se genera al ingreso si no existe.
3. **Cifrado app-layer** (T3-5): encrypted_payload + safe_projection. → `07_tech/TECH-CIFRADO.md`
4. **Telegram dual mode** (T3-3): webhook en prod, long-polling en dev. → `07_tech/TECH-TELEGRAM.md`
5. **Global Query Filters** (T3-6): EF Core filtra por patient_id automaticamente.

## Detail docs

| Doc | Tema |
|-----|------|
| `07_tech/TECH-CIFRADO.md` | Patron encrypted_payload + safe_projection + key rotation |
| `07_tech/TECH-TELEGRAM.md` | Modos webhook/polling, flujo conversacional, recordatorios |

## Sync gates

Cambios en 07 pueden forzar revision de:
- `04_RF/RF-SEC-*` si cambia fail-closed o pseudonimizacion
- `04_RF/RF-REG-*` si cambia cifrado o Global Query Filters
- `09_contratos_tecnicos.md` si cambia auth o endpoints

---

*Fuente: `.docs/raw/decisiones/02_decisiones_arquitectura.md`*
