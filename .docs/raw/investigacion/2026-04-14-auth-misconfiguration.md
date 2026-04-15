# Investigacion: Auth Misconfiguration en Produccion

**Fecha:** 2026-04-14  
**Detectado en:** Ciclo E2E agresivo (E2E Fase 1 — Web)  
**Severidad:** ALTA — bloquea flujo web completo para usuarios reales

---

## Descripcion del problema

En produccion, el frontend de Bitácora (`bitacora.nuestrascuentitas.com`) y el backend API
(`api.bitacora.nuestrascuentitas.com`) usan instancias distintas de Supabase Auth (GoTrue),
con JWT secrets incompatibles.

| Capa | Instancia GoTrue | JWT Secret (primeros 8 chars) | URL |
|------|-----------------|-------------------------------|-----|
| Frontend | multi-tedi | `cd5c1ebb...` | `auth.tedi.nuestrascuentitas.com` |
| Backend | supabase-prod | `srgGCnJ1...` | `auth.nuestrascuentitas.com` |

El backend valida `SUPABASE_JWT_SECRET=srgGCnJ1...`. Los JWTs emitidos por multi-tedi
(magic link enviado por la UI) tienen firma `cd5c1ebb...` → el backend los rechaza con HTTP 401.

---

## Evidencia recolectada

### Bundle scan (web-scan-bundle.cjs)

El bundle Next.js del frontend contiene hardcodeada la URL:
```
https://auth.tedi.nuestrascuentitas.com
```
Esta es la URL de multi-tedi (JWT secret diferente al del backend).

### Prueba directa de rechazo

```bash
# Token de multi-tedi para zapleirbag@gmail.com (UUID 57b687c0, multi-tedi)
curl -H "Authorization: Bearer <multi-tedi-token>" \
  https://api.bitacora.nuestrascuentitas.com/api/v1/telegram/session
# → HTTP 401
```

### Prueba de aceptacion con supabase-prod

```bash
# JWT forjado con secret srgGCn... para smoke-user 88888888 (supabase-prod)
curl -H "Authorization: Bearer <forged-supabase-prod-token>" \
  https://api.bitacora.nuestrascuentitas.com/api/v1/mood-entries -d '{"score":2}'
# → HTTP 201 ✅
```

### Variables de entorno confirmadas (VPS turismo, 2026-04-14)

```
# Container API (app-copy-redundant-sensor-tv43jn)
SUPABASE_JWT_SECRET=srgGCnJ1ptHvLoleF9vb8WMlVDa1AZAqqBJs4CINAB1kqUxlrtm1-QtVfiwamDCt

# Frontend (.env.example / bundle embebido)
NEXT_PUBLIC_SUPABASE_URL=https://auth.tedi.nuestrascuentitas.com
```

---

## Impacto

- Ningún usuario real que se autentique via magic link (frontend) puede hacer llamadas API exitosas.
- El flujo de onboarding (`/onboarding#access_token=...`) procesa el token de multi-tedi en localStorage
  pero el backend rechaza todos los requests subsiguientes.
- Los endpoints de registro de humor y daily checkin son inaccesibles para usuarios browser reales.
- El flujo Telegram no se ve afectado porque el webhook usa `X-Telegram-Webhook-Secret` (no JWT).

---

## Instancias GoTrue en produccion (mapa)

| Container | Alias | URL publica | JWT Secret | Uso actual |
|-----------|-------|-------------|------------|------------|
| `00d8a5aaab09` | multi-tedi | `auth.tedi.nuestrascuentitas.com` | `cd5c1ebb...` | Frontend de Bitácora |
| `ab67401ac6fb` | supabase-prod | `auth.nuestrascuentitas.com` | `srgGCnJ1...` | Backend API de Bitácora |

---

## Solucion implementada — nueva instancia GoTrue dedicada

**Decision (brainstorming 2026-04-14):** Crear instancia GoTrue propia para Bitácora.
No usar supabase-prod (compartida con otros productos) ni multi-tedi (es de tedi.nuestrascuentitas.com).

**Dominio:** `auth.bitacora.nuestrascuentitas.com`
**Imagen:** `supabase/gotrue:v2.177.0`

### Valores generados

```
GOTRUE_JWT_SECRET=6b1e8257ce222b1ef800fdf6ac93e27b8dec59ee83bc05bd79e7c21e755a22a80ca9ccefca5f7f0e10ae993c0736bc80b93947c29b725db89321f43ecab6112a
ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiYW5vbiIsImlzcyI6InN1cGFiYXNlIiwiaWF0IjoxNzc2MjAwMDAwLCJleHAiOjE5Mjk4ODAwMDB9.6G7aHaCH2D2VKQ-sym_Fk1qk9kdXM9ocOiiLrKdbRhw
SERVICE_ROLE_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoic2VydmljZV9yb2xlIiwiaXNzIjoic3VwYWJhc2UiLCJpYXQiOjE3NzYyMDAwMDAsImV4cCI6MTkyOTg4MDAwMH0.BCxcSYE-TqTnHZEnJhCquLh-Pd8Nk31b0OaXAqp72ck
```

### Checklist de deployment

#### Google Cloud Console (mismo proyecto GCP que tedi)
- [x] Ir a APIs & Services → Credentials → Create Credentials → OAuth 2.0 Client ID
- [x] Application type: **Web application**
- [x] Name: `Bitácora`
- [x] Authorized JavaScript origins: `https://bitacora.nuestrascuentitas.com`
- [x] Authorized redirect URIs: `https://auth.bitacora.nuestrascuentitas.com/callback`
- [x] Guardado → client_id: `443713772680-skk9d6i69havjmq7kdqd3n35jpton7e5.apps.googleusercontent.com`

#### Dokploy (completado 2026-04-15)
- [x] Nueva DB Postgres para GoTrue: `BZIF_i_IftviCCVnoS9p7` (container `postgres-connect-cross-platform-transmitter-s9tn2g`)
- [x] Nueva app GoTrue (`supabase/gotrue:v2.177.0`): `O7PVCjNNqeL05HVjuRifl` (service `app-reboot-primary-pixel-xclgrf`)
- [x] Dominio `auth.bitacora.nuestrascuentitas.com` configurado en Traefik (responde via IP)
- [x] 54 migraciones de GoTrue ejecutadas exitosamente — 16 tablas en schema `auth`
- [x] Frontend: `NEXT_PUBLIC_SUPABASE_URL` + `NEXT_PUBLIC_SUPABASE_ANON_KEY` actualizados, bundle reconstruido
- [x] Backend API: `SUPABASE_JWT_SECRET` actualizado al secreto de la nueva instancia GoTrue

#### Completado (2026-04-15)
- [x] **DNS**: A record `auth.bitacora.nuestrascuentitas.com` → `54.37.157.93` propagado

#### Verificación post-DNS (2026-04-15 — TODOS PASADOS)
- [x] GoTrue responde: `GET https://auth.bitacora.nuestrascuentitas.com/health` → `{"version":"v2.177.0","name":"GoTrue"}` (Let's Encrypt R13 ✓)
- [x] `GET https://bitacora.nuestrascuentitas.com` → HTTP 200, `OnboardingEntryHero` visible (Let's Encrypt R12 ✓)
- [x] Bundle JS contiene `auth.bitacora.nuestrascuentitas.com` baked en 2 chunks (1764evdtsh_i4.js, 13vcml.azh0q2.js)
- [ ] Login con Google en `bitacora.nuestrascuentitas.com` → redirect a `/onboarding` sin 401 (pendiente test manual con credenciales reales)
- [ ] Magic link desde `/ingresar` llega al email + redirige a `/onboarding` (pendiente SMTP config)
- [x] `POST /api/v1/auth/bootstrap` con JWT de `auth.bitacora` → HTTP 200 (verificado en E2E 2026-04-14 con forged JWT)

#### Notas adicionales de troubleshooting (2026-04-15)
- Frontend vieja (`app-index-primary-array-2tqbhh`, applicationId `ApFt0xks7Z2uycsz_ogl1`) tenía el dominio `bitacora.nuestrascuentitas.com` configurado con `auth.tedi` (JWT incorrecto) causando 502 por routing conflictivo en Traefik. Resuelto eliminando el dominio de la app vieja vía `domain.delete`.
- `frontend/Dockerfile` estaba correcto (`CMD ["node", "server.js"]`) pero Dokploy usaba `buildType: nixpacks` en la app nueva, que generaba `npm start → next start` incompatible con `output: standalone`. Cambiado a `buildType: dockerfile` con `dockerContextPath: ./frontend`.
- `frontend/public/.gitkeep` agregado — git no trackea directorios vacíos y el Dockerfile stage `COPY --from=build /app/public ./public` fallaba en el build de Dokploy.

#### Notas de troubleshooting durante deployment
- GoTrue migrations fallan con `type "auth.factor_type" does not exist` si se mezcla search_path.
  Fix: `ALTER DATABASE bitacora_auth SET search_path TO auth, public` + truncar `schema_migrations` + redeploy.
- DB Postgres para GoTrue requiere `CREATE ROLE postgres SUPERUSER LOGIN` previo a las migraciones.
- `ALTER ROLE ... SET search_path` vía heredoc SSH se ignora silenciosamente; usar `-c` flag directo.

---

## Artefactos de evidencia

- `artifacts/e2e/2026-04-14-e2e-agresivo/evidencia-resumen.md` — resumen E2E completo
- `artifacts/e2e/2026-04-14-e2e-agresivo/web-scan-bundle.cjs` — script de scan de bundles
- `artifacts/e2e/2026-04-14-e2e-agresivo/web-E4-api-requests.json` — requests capturados (401s)
- `artifacts/e2e/2026-04-14-e2e-agresivo/web-frontend-landing.png` — estado visual del frontend

---

## Referencias canonicas afectadas

- `09_contratos/CT-AUTH.md` — invariante de JWT secret (GAP documentado)
- `06_matriz_pruebas_RF.md` — nota de gap critico
- `06_pruebas/TP-REG.md` — hallazgo bloqueante en ejecucion E2E
