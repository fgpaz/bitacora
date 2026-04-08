# Decisiones de Arquitectura — Brainstorming T3

**Fecha:** 2026-04-08
**Contexto:** Decisiones tecnicas para el MVP de Bitacora, resueltas via brainstorming interactivo con protocolo completo.

---

## T3-1: Project Decision Priority

**Decision:** Security > Privacy > Correctness > Usability > Maintainability > Performance > Cost > Time-to-market

**Justificacion:** El proyecto maneja datos de salud mental — la categoria mas sensible bajo la ley argentina. Security lidera como paraguas tecnico. Privacy en segundo lugar porque el core del producto es "el paciente controla sus datos". Correctness antes de Usability porque un dato clinico incorrecto es peor que una UX imperfecta. Usability en 4to porque sin adopcion de pacientes no hay producto.

---

## T3-2: Arquitectura de servicios

**Decision:** Monolito modular puro (.NET 10).

**Template:** `ps.microservice.net10.fullskeleton.1.0.0.nupkg`
**Subagente:** `ps-dotnet10`

**Justificacion:** MVP con un solo desarrollador. La complejidad de microservicios no se justifica para el scope actual. Modulos internos bien separados (Registro, Vinculos, Consent, Visualizacion, Telegram, Export) permiten extraccion futura sin rewrite. El webhook de Telegram es un endpoint mas dentro del monolito.

**Estructura de modulos:**
- `Bitacora.Api` — monolito principal
  - Modulo Registro (humor + factores diarios)
  - Modulo Vinculos (profesional-paciente)
  - Modulo Consent (consentimiento informado)
  - Modulo Visualizacion (timeline + dashboard)
  - Modulo Telegram (webhook + bot logic)
  - Modulo Export (CSV)

---

## T3-3: Modo del bot de Telegram

**Decision:** Webhook en produccion + long-polling en desarrollo local.

**Justificacion:** Dokploy VPS tiene HTTPS terminado por Traefik, soporta webhook nativamente. Long-polling en dev local evita la necesidad de tunnel (ngrok). La libreria Telegram.Bot de .NET soporta ambos modos con configuracion por entorno.

**Configuracion:**
- Produccion: `POST /api/v1/telegram/webhook` (webhook registrado con Telegram API)
- Desarrollo: `GetUpdatesReceiver` (long-polling, sin HTTPS requerido)

---

## T3-4: Aislamiento de base de datos

**Decision:** DB dedicada en el mismo server PostgreSQL del VPS.

**Justificacion:** Datos de salud mental requieren aislamiento total (PDP: Security > Privacy). DB dedicada garantiza: credenciales separadas, backup/restore independiente, y que un breach en multi-tedi u otro servicio no exponga datos de Bitacora. El overhead de una DB extra en el mismo server PostgreSQL es minimo.

**Configuracion:**
- Server: PostgreSQL en VPS 54.37.157.93
- DB: `bitacora_db` (dedicada, credenciales propias)
- Esquema: `public` (unico esquema dentro de la DB dedicada)

---

## T3-5: Encriptacion de datos en reposo (REVISADA post-BuhoSalud)

**Decision:** Patron `encrypted_payload` + `safe_projection` a nivel aplicacion (adoptado de BuhoSalud).

**Decision original:** Disk-level + PII cifrado. **Revisada** al incorporar las decisiones de seguridad de `C:\repos\buho\salud\.docs\wiki\`.

**Justificacion:** BuhoSalud rechaza explicitamente disk-level como insuficiente para datos de salud y cifra todo el health payload a nivel aplicacion. Para Bitacora, adoptamos el mismo patron:
- `encrypted_payload`: dato clinico completo cifrado (AES, app-layer) con `key_version` para rotacion de claves.
- `safe_projection`: vista sanitizada con datos operacionales minimos para queries (mood_score, timestamps, flags booleanos). No contiene PII ni texto libre.
- PII (nombre, email, DNI, telefono): cifrado a nivel aplicacion, nunca en `safe_projection`.

**Patron:**
```
MoodEntry:
  encrypted_payload  → {mood_score, notes, medication_detail, ...} cifrado AES
  safe_projection    → {mood_score: -2, sleep_hours: 7, has_anxiety: true, ...} en claro
  key_version        → 1 (version de clave para rotacion)
  encrypted_at       → timestamp
```

**Queries de visualizacion** usan `safe_projection` (en claro). El `encrypted_payload` solo se descifra cuando el paciente o profesional autorizado accede al detalle completo.

**Reglas de fail-closed:**
- Si la clave de cifrado no esta disponible → fallo cerrado (HTTP 500), ningun dato se escribe.
- Key rotation manual y auditada. Records historicos descifran con su `key_version` original.

---

## T3-6: Row-Level Security

**Decision:** App-level con EF Core Global Query Filters. Migracion a RLS de PostgreSQL en Roadmap.

**Justificacion:** EF Core Global Query Filters es el equivalente funcional de RLS pero a nivel ORM: se configura una vez y aplica automaticamente a toda query sobre entidades de paciente. Mas simple de debuggear y testear que RLS de PostgreSQL, especialmente para el modelo de acceso dual (paciente ve sus datos + profesional ve datos de pacientes vinculados). Migracion a RLS posible sin cambios en la logica de negocio.

**Implementacion:**
```csharp
// En DbContext.OnModelCreating
modelBuilder.Entity<MoodEntry>()
    .HasQueryFilter(e => e.PatientId == _currentPatientId);
```

---

## Decisiones de Seguridad Heredadas de BuhoSalud

Fuente: `C:\repos\buho\salud\.docs\wiki\` — revisadas 2026-04-08.

### T3-7: Separacion identidad/salud

**Decision:** `User` y datos clinicos (MoodEntry, DailyCheckin) son aggregates separados. Ningun campo de salud puede vivir en `User`. Ningun campo PII puede vivir en las tablas clinicas sin cifrar.

**Fuente BuhoSalud:** RF-SEC-001, RF-SEC-002, RF-SEC-003, FL-SEC-01.

### T3-8: Pseudonimizacion en logs

**Decision:** Todos los logs operacionales y telemetria usan `pseudonym_id = HASH(actor_id + tenant_id + env_salt)`, nunca el `actor_id` real. El `actor_id` solo aparece en el audit log (append-only, acceso restringido). No existe tabla de mapeo pseudonym→actor en la capa operacional.

**Fuente BuhoSalud:** RF-SEC-007, RF-SEC-008.

### T3-9: Audit log append-only

**Decision:** Tabla `AccessAudit` append-only (sin UPDATE ni DELETE). Registra: `audit_id`, `trace_id`, `actor_id`, `pseudonym_id`, `action_type`, `resource_type`, `resource_id`, `outcome`, `created_at_utc`. Cada operacion transaccional genera al menos un registro con el mismo `trace_id`.

**Fuente BuhoSalud:** AuditLogEntry (3.49).

### T3-10: Fail-closed como invariante arquitectonico

**Decision:** Todo gate de seguridad falla cerrado, nunca permite. Especificamente:
- Clave de cifrado ausente → HTTP 500, nada se escribe.
- Auth check fallido → sin acceso.
- Internal service secret ausente → rechazo.
- Consent no otorgado → sin acceso a datos del paciente.

**Fuente BuhoSalud:** Regla transversal en CT-COMPLIANCE, RF-SEC-004, RF-SEC-005.

### T3-11: Consent defaults false

**Decision:** Acceso profesional a datos del paciente default `false`. Solo el paciente puede activarlo explicitamente. Revocacion inmediata con cascade (invalida caches, corta sesiones activas).

**Fuente BuhoSalud:** PatientProfessionalLink, RF-ACQ-011.

### T3-12: Retencion de datos

**Decision:**
- Eventos de crisis / alertas de riesgo: retencion minima 5 anos (compliance salud mental, Ley 26.657).
- Registros de humor regulares: sujeto a politica de retencion definida con el paciente en el consentimiento informado.
- Audit log: retencion minima 2 anos.
- Derecho de supresion: anonimizacion de PII + cifrado con clave destruida del payload de salud. Audit trail anonimizado se conserva.

**Fuente BuhoSalud:** CrisisEvent (3.22), User deletion pattern.

### T3-13: Trace ID end-to-end

**Decision:** `trace_id` obligatorio punta a punta. Toda accion de seguridad (audit, consent, cifrado, acceso profesional) lleva el mismo `trace_id` que inicio el request. Si un servicio recibe un request sin `X-Trace-Id`, genera uno antes de continuar.

**Fuente BuhoSalud:** Regla transversal.

---

## Resumen de Decisiones de Arquitectura

| ID | Decision | Valor |
|----|----------|-------|
| T3-1 | PDP | Security > Privacy > Correctness > Usability > Maintainability > Performance > Cost > TTM |
| T3-2 | Servicios | Monolito modular (.NET 10, template fullskeleton, ps-dotnet10) |
| T3-3 | Telegram | Webhook prod + long-polling dev |
| T3-4 | DB | Dedicada en mismo server PostgreSQL |
| T3-5 | Cifrado | encrypted_payload + safe_projection (patron BuhoSalud) + PII cifrado |
| T3-6 | RLS | EF Core Global Query Filters (app-level) |
| T3-7 | Separacion identidad/salud | User y datos clinicos son aggregates separados |
| T3-8 | Pseudonimizacion | pseudonym_id en logs, actor_id solo en audit |
| T3-9 | Audit log | Append-only, trace_id obligatorio |
| T3-10 | Fail-closed | Todo gate de seguridad falla cerrado |
| T3-11 | Consent defaults | Acceso profesional default false, paciente activa |
| T3-12 | Retencion | Crisis 5 anos, audit 2 anos, supresion por anonimizacion |
| T3-13 | Trace ID | Obligatorio end-to-end en toda operacion |

---

## Decisiones Heredadas (no debatidas en T3)

| Decision | Valor | Fuente |
|----------|-------|--------|
| Auth | Supabase Auth en auth.tedi.nuestrascuentitas.com | Cerrada en plan inicial |
| Deploy | Dokploy VPS 54.37.157.93 | Cerrada en plan inicial |
| Frontend | Next.js 16 | Cerrada en plan inicial |
| Dominio | bitacora.nuestrascuentitas.com | Cerrada en plan inicial |
