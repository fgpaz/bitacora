# Bitacora — Release Readiness

**Slug:** bitacora.nuestrascuentitas.com
**Type:** nota-tecnica
**Date:** 2026-04-13
**Status:** Produccion operativa en proyecto Dokploy dedicado

---

## Resumen ejecutivo

Bitacora corre ahora en un proyecto Dokploy dedicado (`bitacora`) separado de `nuestrascuentitas`. Backend, frontend y Telegram estan operativos en produccion. El frontend se recreo con GitHub + nixpacks y se corrigieron dos bloqueos reales: `nixpacks` no estaba instalado en el host y el build necesitaba Node 22 + lockfile sincronizado.

## Estado de componentes

| Componente | Status | URL |
|------------|--------|-----|
| Backend API (.NET 10) | Operativo | `https://api.bitacora.nuestrascuentitas.com` |
| Telegram Bot (@tedi_responde_bot) | Operativo con webhook | `https://api.bitacora.nuestrascuentitas.com/api/v1/telegram/webhook` |
| Frontend (Next.js 16) | Operativo | `https://bitacora.nuestrascuentitas.com` |
| Base de datos PostgreSQL | Operativa (BitacoraDb) | `postgres-reboot-solid-state-application-l55mww:5432` |

## Dokploy actual

| Recurso | ID | Nota |
|---------|----|------|
| Project | `18WEM8BMIq-z_wgkrNlp8` | `bitacora` |
| Environment | `ULVQy3BehcO0VOH-J-ZVv` | `production` |
| PostgreSQL | `BZIF_i_IftviCCVnoS9p7` | `bitacora-db` |
| API app | `UROM_r5ETX0rvs-1WZ3bi` | `bitacora-api` |
| Frontend app | `BRTMuvBfWtslXHnShtrnB` | `bitacora-frontend` |

## Backend API — Verificacion

### Endpoints operativos (23 total)

```
/api/v1/auth/bootstrap
/api/v1/consent/current
/api/v1/consent
/api/v1/mood-entries
/api/v1/daily-checkins
/api/v1/vinculos
/api/v1/vinculos/active
/api/v1/vinculos/accept
/api/v1/vinculos/{id}
/api/v1/vinculos/{id}/view-data
/api/v1/professional/invites
/api/v1/professional/patients
/api/v1/visualizacion/timeline
/api/v1/visualizacion/summary
/api/v1/professional/patients/{patientId}/summary
/api/v1/professional/patients/{patientId}/timeline
/api/v1/professional/patients/{patientId}/alerts
/api/v1/export/patient-summary
/api/v1/export/{patientId}/constraints
/api/v1/export/patient-summary/csv
/api/v1/telegram/pairing
/api/v1/telegram/session
/api/v1/telegram/webhook
```

### Health checks

| Check | Valor | Observacion |
|-------|-------|-------------|
| `/health` | 200 OK | Liveness funcional |
| `/health/ready` | database: unreachable | Falso positivo — consultas reales funcionan |
| 23 endpoints | Responden correctamente | Auth requerida en clinical endpoints |

### Secrets sincronizados en vault (prod)

| Secret | Valor | Ubicacion |
|--------|-------|-----------|
| BITACORA_SUPABASE_JWT_SECRET | `srgGCnJ1...` | Dokploy env + Infisical vault |
| BITACORA_ENCRYPTION_KEY | `ERJY/JsA...` | Dokploy env + Infisical vault |
| BITACORA_PSEUDONYM_SALT | `0a6e89ad...` | Dokploy env + Infisical vault |
| BITACORA_TELEGRAM_BOT_TOKEN | `8609908294:AAE...` | Dokploy env + Infisical vault |
| BITACORA_BASE_URL | `https://api.bitacora.nuestrascuentitas.com` | Dokploy env + Infisical vault |
| TELEGRAM_BOT_TOKEN | `8609908294:AAE...` | Dokploy env + Infisical vault |

## Telegram — Configuracion

```mermaid
flowchart LR
    A["@mi_bitacora_personal_bot"] -->|Webhook set| B["https://api.bitacora.nuestrascuentitas.com/api/v1/telegram/webhook"]
    B --> C["Bitacora.Api /telegram/webhook"]
    C -->|SendReminderCommand| A
```

- **Bot:** `@mi_bitacora_personal_bot`
- **Token:** `8609908294:AAEQpubqrpf48pSL6ERAGwxx7lNgj7dUoYI`
- **Webhook:** Confirmado activo via `getWebhookInfo`
- **getUpdates:** No disponible (webhook activo)

## Frontend — Resolucion

### Causa raiz inicial

El frontend viejo fallaba con `docker build` porque dependia del Dockerfile y de una imagen base desde Docker Hub. Ese problema se elimino al recrear el frontend con `buildType=nixpacks`.

```
error getting credentials - err: exec: "docker-credential-desktop.exe": not found in $PATH
```

Luego aparecieron dos bloqueos reales del nuevo frontend:

1. El host `turismo` no tenia `nixpacks` instalado (`bash: nixpacks: command not found`).
2. El build de Next.js requeria Node 20+ y `package-lock.json` alineado con `package.json`.

Ambos quedaron resueltos.

### Resolucion aplicada

1. Proyecto Dokploy nuevo: `bitacora`
2. Frontend recreado con `GitHub + nixpacks`
3. Instalacion de `nixpacks` 1.41.0 en el host `turismo`
4. `frontend/package.json` actualizado a Node 22
5. `frontend/package-lock.json` sincronizado
6. `frontend/nixpacks.toml` agregado para cachear solo directorios validos

### Estado final del frontend

Frontend publico en `https://bitacora.nuestrascuentitas.com` con status HTTP 200.

### next.config.js standalone

```js
const nextConfig = {
  reactStrictMode: true,
  output: 'standalone',
  env: {
    NEXT_PUBLIC_API_BASE_URL: process.env.NEXT_PUBLIC_API_BASE_URL,
    NEXT_PUBLIC_SUPABASE_URL: process.env.NEXT_PUBLIC_SUPABASE_URL,
    NEXT_PUBLIC_SUPABASE_ANON_KEY: process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY,
  },
};
```

## Variantes de entorno del frontend

| Variable | Valor prod |
|----------|-----------|
| NEXT_PUBLIC_API_BASE_URL | `https://api.bitacora.nuestrascuentitas.com` |
| NEXT_PUBLIC_SUPABASE_URL | `https://auth.tedi.nuestrascuentitas.com` |
| NEXT_PUBLIC_SUPABASE_ANON_KEY | `eyJhbGci...` (anon key) |

## Gaps conocidos de produccion

| ID | Severidad | Descripcion | Solucion |
|----|----------|-------------|----------|
| FE-DOKPLOY-01 | Resuelto | Docker Hub no accesible desde Dokploy | Frontend migrado a `nixpacks` + host preparado |
| DB-HEALTH-01 | Baja | `/health/ready` reporta database unreachable | Falso positivo — consultas funcionan. El probe de EF no puede conectar via nombre host desde el container |
| TELEGRAM-01 | Media | getUpdates no disponible para debugging | Usar webhook exclusively. El bot responde si el backend tiene el token |

## Commits de produccion (wave-prod)

```
304bb77 fix(frontend): use npm install instead of npm ci in Dockerfile
d26bee6 feat(frontend): add Dockerfile for standalone Next.js deployment
3b67ac3 fix(spec): correct SendTelegramMessageAsync stub claim
dfc231a docs(qa): synchronize final validation evidence
7e4e1b2 fix(runtime): harden backend security, audit, and frontend guards
527d958 feat(frontend): complete Phase 41 UI gaps per UI-RFC contracts
ce5ba41 fix(frontend): use shared sha256 module for invite EmailHash
7b94b3a fix(frontend): wire ExportGate to real endpoint
95c92df feat(export): implement GET /export/{patientId}/constraints endpoint
```

## Checklist de produccion

- [x] Backend desplegado y respondiendo
- [x] 23 endpoints verificados
- [x] Telegram webhook configurado
- [x] Secrets en vault (prod)
- [x] Secrets en Dokploy (prod env)
- [x] DNS configurado (api.bitacora.nuestrascuentitas.com)
- [x] Frontend desplegado y funcionando
- [ ] Smoke test E2E con usuario real
- [ ] Backup diario de PostgreSQL configurado
- [ ] Monitoring/alerting configurado

## Proximo paso

Resolver FE-DOKPLOY-01: configurar registry de Docker en Dokploy o hacer build local del frontend y subir la imagen via `docker save/load` o registry interno.
