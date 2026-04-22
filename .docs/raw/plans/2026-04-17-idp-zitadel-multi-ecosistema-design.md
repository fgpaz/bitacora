# 2026-04-17 — IdP Zitadel multi-ecosistema (design lockeado)

> Brainstorming session: 2026-04-17. Decisiones aprobadas por el usuario. Reviar este doc antes de planificar implementacion.

## Decisiones lockeadas

1. **Tecnologia IdP:** Zitadel (Go + Postgres, Apache 2.0, self-hosted).
2. **Topologia:** dos instancias separadas del mismo stack — una por ecosistema.
   - `id.nuestrascuentitas.com` — ecosistema Teslita (nuestrascuentitas.com, bitacora, multi-tedi, **gastos**).
   - `id.buho.ar` (o subdominio equivalente) — ecosistema Buho (buho/salud, buho/digitalizacion).
3. **Aislamiento:** cada instancia tiene su propia Postgres dedicada. Sin cross-SSO entre ecosistemas.
3.a. **SSO dentro de Teslita:** UN solo user_id por persona para todos los productos Teslita. Si un usuario se loguea en `gastos`, queda logueado en `bitacora` y en `multi-tedi` (misma sesion Zitadel). Implementado via un unico Zitadel Organization que agrupa a todos, o via projects separados compartiendo user registry.
4. **Integracion en apps:** OIDC estandar. Cada app es un OAuth2 Client dentro de la Organization correspondiente.
5. **Fase 1 previa (Bitacora):** mantener fix H-002 + Google OAuth + password sobre Supabase actual. Pendiente de decision tactica: (a) terminar Fase 1 y migrar a Zitadel despues, o (b) saltar Fase 1 y arrancar Zitadel directo.
6. **SMTP provider Teslita:** Resend (lockeado 2026-04-17 via brainstorming). Free tier 3k/mes cubre 30x volumen Fase 1 estimado. DKIM con 1 TXT record, DPA + SOC2 Type 2 firmables. Escape hatch documentado a AWS SES region `sa-east-1` si el volumen supera 3k/mes o deliverability AR se degrada.

## Motivaciones

- **Compliance Ley 25.326 / 26.529 / 26.657** — buho/salud requiere audit trail granular, MFA obligatoria y data residency argentina.
- **Aislamiento de blast radius** — un incidente en Teslita no debe impactar a Buho/salud y viceversa.
- **Multi-producto** — cada ecosistema tiene varios productos; un IdP compartido dentro del ecosistema reduce ops × N.
- **Costo** — licencia $0; marginal VPS < $5/mes por instancia adicional.
- **Escape hatch** — si algun dia se requiere SaaS, Zitadel Cloud existe y ofrece migracion guiada.

## Arquitectura propuesta

```
ECOSISTEMA TESLITA                       ECOSISTEMA BUHO
id.nuestrascuentitas.com                 id.buho.ar
+----------------------------+           +----------------------------+
|  Zitadel #1 (self-hosted)  |           |  Zitadel #2 (self-hosted)  |
|  Postgres dedicada          |           |  Postgres dedicada          |
|                             |           |                             |
|  Organizations:             |           |  Organizations:             |
|    - nuestrascuentitas      |           |    - buho-salud  (H++)      |
|    - bitacora               |           |    - buho-digitalizacion    |
|    - multi-tedi             |           |                             |
|                             |           |  Policies refuerzo salud:   |
|  Policies baseline          |           |    - MFA forzada            |
|                             |           |    - session 15m            |
|                             |           |    - audit retention 5y     |
|                             |           |    - IP allowlist admin UI  |
|                             |           |    - passkeys habilitados   |
+----------------------------+           +----------------------------+
        |      |      |                           |             |
     [NCC] [BITA] [TEDI]                      [SALUD]       [DIGITAL]
```

## Politicas diferenciadas por Org

### Teslita (baseline)

- MFA opcional (recomendada para profesionales).
- Session lifetime 1h access / 30d refresh.
- Audit log retention 2 anos.
- Password complexity: 10 caracteres minimo.
- OAuth providers: Google + email/password + magic-link (cuando SMTP este OK).

### Buho/salud (hardened)

- MFA forzada (TOTP o passkey).
- Session lifetime 15 min access / 24h refresh.
- Audit log retention 5 anos, exportable a S3 append-only.
- Password complexity: 14 caracteres minimo + breach check (Zitadel soporta Have I Been Pwned).
- OAuth providers: Google + email/password + passkeys. Magic-link deshabilitado (no es apto para flujo clinico).
- IP allowlist para admin UI.
- Alert webhook a incident response si un usuario falla 3 logins consecutivos.

### Buho/digitalizacion (intermedio)

- MFA forzada para usuarios con rol admin/superadmin.
- Session lifetime 30 min access / 7d refresh.
- Audit log retention 2 anos.
- Resto igual a Teslita baseline.

## Integracion con cada app

| App | Rol | OIDC Client type | Claims obligatorios |
|-----|-----|------------------|---------------------|
| Bitacora frontend (Next.js 16) | SPA/App | PKCE public | sub, email, user_metadata.role |
| Bitacora.Api (.NET 10) | Resource server | validate JWT via OIDC discovery + JWKS cache | sub, email, role |
| multi-tedi (C:\repos\mios\multi-tedi) | TBD (ver repo) | PKCE public | sub, email, role |
| gastos (C:\repos\mios\gastos) | TBD (ver repo) | PKCE public | sub, email, role |
| nuestrascuentitas.com | TBD | PKCE public | sub, email |
| buho/salud | Resource server + SPA | PKCE public + M2M para workers | sub, email, role, tenant_id |
| buho/digitalizacion | TBD | TBD | TBD |

## Impacto sobre Bitacora.Api y frontend

- `Bitacora.Api` JWT validation: hoy usa clave simetrica `Supabase:JwtSecret`. Zitadel usa **RS256 asimetrico** con JWKS expuesto en `{ISSUER}/oauth/v2/keys`. Refactor:
  - reemplazar `AddJwtBearer` config simetrica por `Authority = "https://id.nuestrascuentitas.com/bitacora"` + validacion JWKS automatica.
  - mapeo de claim `role` desde `urn:zitadel:iam:org:project:role:{roleKey}` (Zitadel convention).
  - actualizar `09_contratos/CT-AUTH.md` con el nuevo contrato.
- Frontend: `createBrowserClient` de `@supabase/ssr` se reemplaza por un OIDC client (por ejemplo `oidc-client-ts` o implementacion PKCE custom). Middleware sigue leyendo una cookie `sb-access-token` (o se renombra a `bitacora-access-token`).
- El fix H-002 hecho hoy (sb-access-token cookie sync) queda obsoleto bajo Zitadel porque el OIDC client manejara storage por si mismo.

## Pre-requisitos infra

| Pre-req | Status | Responsable |
|--------|--------|-------------|
| DNS `id.nuestrascuentitas.com` apuntando al VPS turismo | Pendiente | Usuario |
| DNS `id.buho.ar` o subdominio equivalente | Pendiente | Usuario |
| Certificados TLS (Dokploy/Traefik autoemitidos via Lets Encrypt) | Automatico | Dokploy |
| VPS con al menos 2 vCPU + 4GB RAM (comparte con apps existentes) | OK (turismo) | - |
| SMTP saliente (Resend recomendado, $0 free tier 3k mails/mes) | Pendiente (H-014 relacionado) | Usuario |
| Postgres 15+ para cada instancia | OK (Dokploy provisiona) | - |

## Plan de migracion propuesto (Fase 2)

### Wave A — Stand-up del IdP Teslita

1. Deploy Zitadel #1 en Dokploy con Postgres dedicada.
2. Configurar dominio `id.nuestrascuentitas.com` con TLS.
3. Crear Organizations: `nuestrascuentitas`, `bitacora`, `multi-tedi`.
4. Configurar SMTP Resend con domain DKIM/SPF.
5. Crear primer Application/Client para `bitacora` con PKCE.
6. Smoke test: crear usuario admin, login, validar JWKS.

### Wave B — Migracion Bitacora (primer producto)

1. Refactor `Bitacora.Api` auth: Bearer JWT con JWKS de Zitadel.
2. Refactor frontend: reemplazar `@supabase/ssr` por `oidc-client-ts`.
3. Migrar usuarios existentes (script: Supabase users -> Zitadel import).
4. Actualizar wiki (`09_contratos/CT-AUTH.md`, `07_baseline_tecnica.md`).
5. QA E2E completo: login Google + login password + logout.
6. Decomisionar Supabase Auth self-hosted de Bitacora.

### Wave C — Migracion nuestrascuentitas.com + multi-tedi

Patron identico a Bitacora, validado por Wave B.

### Wave D — Stand-up del IdP Buho

1. Deploy Zitadel #2 en el mismo VPS (subdirectorio Dokploy nuevo).
2. Configurar dominio `id.buho.ar` con TLS.
3. Crear Organizations + policies hardened de buho/salud.
4. SMTP separado (idealmente otro dominio / subdominio sender).

### Wave E — Migracion buho/salud (bajo DPIA)

1. Redactar DPIA (Data Protection Impact Assessment) referenciando Ley 26.529/657.
2. Configurar audit log export a S3 append-only.
3. Refactor backend/frontend igual que Bitacora pero con compliance checklist.
4. Pentesting externo recomendado antes de go-live.

### Wave F — Migracion buho/digitalizacion

Patron intermedio, tras validacion de Wave E.

## Esfuerzo estimado

| Wave | Scope | Estimate |
|------|-------|----------|
| A | Stand-up Teslita IdP | 1-2 dias |
| B | Migracion Bitacora | 4-6 dias |
| C | Migracion NCC + multi-tedi | 3-5 dias por producto |
| D | Stand-up Buho IdP | 1-2 dias |
| E | Migracion buho/salud (con DPIA) | 5-8 dias (incluyendo pentest) |
| F | Migracion buho/digitalizacion | 3-5 dias |

Total aproximado: **20-35 dias/persona** distribuidos en 4-6 semanas calendario.

## Decisiones aun pendientes

- **Fase 1 Bitacora:** aprovechamos el fix H-002 en curso (con Google + password) para no dejar Bitacora caido, o saltamos directo a Wave B de Zitadel?
- **DNS Buho:** dominio exacto a usar para `id.buho.*`.
- **Users de multi-tedi y nuestrascuentitas.com:** donde viven hoy? hay migracion de usuarios o se empieza fresh?
- **Cuando migrar buho:** antes de proyectos nuevos en salud, o despues.

## Referencias

- Zitadel docs: https://zitadel.com/docs
- Zitadel GitHub: https://github.com/zitadel/zitadel
- OIDC spec: https://openid.net/specs/openid-connect-core-1_0.html
- Pricing comparison: `.docs/raw/prompts/2026-04-17-revalidar-deploy-y-h002-cookie-sync.md` (brainstorming archivado)
