# Runbook: Professional Smoke User

## Propósito

El usuario profesional smoke permite ejecutar los tests del módulo VIN que requieren un profesional activo, específicamente VIN-P03 (binding code flow).

## Credenciales

Las credenciales están almacenadas en `infra/.env`:

```
SMOKE_PROF_EMAIL=smoke-prof@bitacora.test
SMOKE_PROF_PASSWORD=SmokeProfTest2026!
SMOKE_PROF_USER_ID=ccb0f939-ffce-43c2-8276-af3642f51db4
```

## Cómo recrear si se pierde

Si la cuenta se elimina o queda inoperable en GoTrue, ejecutar:

```bash
GOTRUE_SRK=$(grep "^GOTRUE_SERVICE_ROLE_KEY=" infra/.env | cut -d= -f2)
RESPONSE=$(curl -s -X POST \
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
  }')
echo "$RESPONSE" | grep -o '"id":"[^"]*"' | head -1
```

Copiar el `id` del response y actualizar `SMOKE_PROF_USER_ID` en `infra/.env`.

## Verificación

Para verificar que el usuario funciona:

```bash
# 1. Login
PROF_TOKEN=$(curl -s -X POST \
  "https://auth.bitacora.nuestrascuentitas.com/token?grant_type=password" \
  -H "Content-Type: application/json" \
  -d '{"email":"smoke-prof@bitacora.test","password":"SmokeProfTest2026!"}' \
  | grep -o '"access_token":"[^"]*"' | cut -d'"' -f4)

# 2. Bootstrap en Bitácora.Api
curl -s -o /dev/null -w "%{http_code}\n" \
  -X POST \
  https://api.bitacora.nuestrascuentitas.com/api/v1/auth/bootstrap \
  -H "Authorization: Bearer $PROF_TOKEN" \
  -H "Content-Type: application/json"
```

Esperado: HTTP 200

## Historial

- 2026-04-15: Usuario creado para habilitar GAP-03 (VIN-P03 test case)
