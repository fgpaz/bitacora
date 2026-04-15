# Task T5: GAP-03 — Crear usuario profesional smoke en GoTrue

## Shared Context
**Goal:** Resolver GAP-03: no existe usuario profesional activo en producción para probar VIN-P03 (binding code flow profesional).
**Stack:** GoTrue Admin API v2.177.0, curl, infra/.env
**Architecture:** GoTrue Admin API acepta POST /admin/users con header `Authorization: Bearer <service_role_key>`. El usuario debe tener `user_metadata.role = "professional"` para que T3-SEC-11 (frontend middleware) lo reconozca.

## Locked Decisions
- Usar GoTrue Admin API (no el flujo de signup normal) para pre-crear el usuario.
- `role: professional` va en `user_metadata` del payload de creación.
- Las credenciales se agregan a `infra/.env` como `SMOKE_PROF_EMAIL` y `SMOKE_PROF_PASSWORD`.
- El password debe cumplir los requisitos mínimos de GoTrue (min 6 chars recomendado mínimo 8 con mayús+número).
- Documentar en `infra/runbooks/professional-smoke-user.md`.

## Task Metadata
```yaml
id: T5
depends_on: []
agent_type: ps-worker
files:
  - modify: infra/.env
  - create: infra/runbooks/professional-smoke-user.md
complexity: medium
done_when: "curl GET /api/v1/professional/dashboard con JWT del smoke prof retorna 200 o 403 (no 401)"
```

## Reference
- GoTrue URL: `https://auth.bitacora.nuestrascuentitas.com`
- GOTRUE_SERVICE_ROLE_KEY: leer de `infra/.env` (variable GOTRUE_SERVICE_ROLE_KEY)
- API backend: `https://api.bitacora.nuestrascuentitas.com`
- Ejemplo bootstrap profesional: POST /api/v1/auth/bootstrap con JWT del profesional

## Prompt
**Paso 1: Verificar que el GOTRUE_SERVICE_ROLE_KEY funciona**

```bash
GOTRUE_SRK=$(grep GOTRUE_SERVICE_ROLE_KEY infra/.env | cut -d= -f2)
curl -s -o /dev/null -w "%{http_code}" \
  -H "Authorization: Bearer $GOTRUE_SRK" \
  https://auth.bitacora.nuestrascuentitas.com/admin/users
```
Esperado: `200`. Si retorna `401`, el service role key no está configurado — detener y reportar.

**Paso 2: Crear el usuario profesional**

```bash
GOTRUE_SRK=$(grep GOTRUE_SERVICE_ROLE_KEY infra/.env | cut -d= -f2)
curl -s -X POST \
  https://auth.bitacora.nuestrascuentitas.com/admin/users \
  -H "Authorization: Bearer $GOTRUE_SRK" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "smoke-prof@bitacora.test",
    "password": "SmokeProfTest2026!",
    "email_confirm": true,
    "user_metadata": {
      "role": "professional",
      "display_name": "Smoke Professional QA"
    }
  }'
```

Guardar el `id` del usuario retornado (será el `professional_user_id` para documentar).

**Paso 3: Verificar que se puede obtener un JWT real**

```bash
curl -s -X POST \
  https://auth.bitacora.nuestrascuentitas.com/token?grant_type=password \
  -H "Content-Type: application/json" \
  -d '{"email":"smoke-prof@bitacora.test","password":"SmokeProfTest2026!"}'
```

Guardar el `access_token` retornado para el paso siguiente.

**Paso 4: Bootstrap del profesional en Bitácora.Api**

```bash
PROF_TOKEN=<access_token del paso 3>
curl -s -X POST \
  https://api.bitacora.nuestrascuentitas.com/api/v1/auth/bootstrap \
  -H "Authorization: Bearer $PROF_TOKEN" \
  -H "Content-Type: application/json"
```

Esperado: `200` con el `userId` del profesional en Bitácora.

**Paso 5: Actualizar infra/.env**

Agregar al final de `infra/.env`:
```
# Smoke profesional credentials
SMOKE_PROF_EMAIL=smoke-prof@bitacora.test
SMOKE_PROF_PASSWORD=SmokeProfTest2026!
SMOKE_PROF_USER_ID=<id del paso 2>
```

**Paso 6: Crear runbook `infra/runbooks/professional-smoke-user.md`**

Documentar los pasos del proceso de creación, el propósito, y los pasos para re-crear si se pierde el usuario.

**Paso 7: Actualizar TP-VIN.md**

Actualizar `.docs/wiki/06_pruebas/TP-VIN.md` en la sección VIN-P03 para indicar:
- Estado: READY (usuario profesional disponible en infra/.env)
- Credenciales: SMOKE_PROF_EMAIL / SMOKE_PROF_PASSWORD

## Execution Procedure
1. Leer `infra/.env` para extraer GOTRUE_SERVICE_ROLE_KEY (no imprimir el valor en output).
2. Verificar el service role key con GET /admin/users.
3. Si falla: reportar el HTTP status y detenerse — no continuar.
4. Crear el usuario con POST /admin/users. Guardar el `id` del response.
5. Verificar login con POST /token?grant_type=password.
6. Verificar bootstrap con POST /api/v1/auth/bootstrap.
7. Editar `infra/.env` con Edit tool para agregar las 3 variables SMOKE_PROF_*.
8. Crear `infra/runbooks/professional-smoke-user.md` con Write tool.
9. Leer `.docs/wiki/06_pruebas/TP-VIN.md` y editar la fila de VIN-P03.
10. Reportar si el usuario ya existe (409 Conflict) — en ese caso, solo actualizar las variables de env.

## Skeleton
```bash
# Crear usuario profesional smoke — ejecutar desde la raíz del repo

GOTRUE_SRK=$(grep GOTRUE_SERVICE_ROLE_KEY infra/.env | cut -d= -f2)

# 1. Verificar acceso admin
curl -s -o /dev/null -w "%{http_code}" \
  -H "Authorization: Bearer $GOTRUE_SRK" \
  https://auth.bitacora.nuestrascuentitas.com/admin/users
# Esperado: 200

# 2. Crear usuario
curl -s -X POST \
  https://auth.bitacora.nuestrascuentitas.com/admin/users \
  -H "Authorization: Bearer $GOTRUE_SRK" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "smoke-prof@bitacora.test",
    "password": "SmokeProfTest2026!",
    "email_confirm": true,
    "user_metadata": {"role": "professional"}
  }'
```

## Verify
```bash
PROF_TOKEN=$(curl -s -X POST \
  https://auth.bitacora.nuestrascuentitas.com/token?grant_type=password \
  -H "Content-Type: application/json" \
  -d '{"email":"smoke-prof@bitacora.test","password":"SmokeProfTest2026!"}' \
  | jq -r .access_token)
curl -s -o /dev/null -w "%{http_code}" \
  https://api.bitacora.nuestrascuentitas.com/api/v1/auth/bootstrap \
  -H "Authorization: Bearer $PROF_TOKEN"
```
Esperado: `200`

## Commit
`infra(qa): add professional smoke user credentials and creation runbook (GAP-03)`
