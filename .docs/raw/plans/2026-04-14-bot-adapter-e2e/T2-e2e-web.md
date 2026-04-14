# Task T2: E2E Web Completo — Login → Consentimiento → Mood Entry → Verificar DB

## Shared Context
**Goal:** Ejecutar el flujo E2E completo desde el navegador web: login con magic link → consentimiento → entrada de humor → verificar DailyCheckin en la DB de producción.
**Stack:** Playwright MCP (navegador) + sshr para consulta DB; web en `https://bitacora.nuestrascuentitas.com`.
**Architecture:** Flujo UI que ejercita FL-REG-01 completo. El magic link se obtiene vía Supabase Admin API o recuperando el email del usuario de test de la DB.
**Independiente de:** T1 (el E2E web no necesita el adapter de Telegram).

## Task Metadata
```yaml
id: T2
depends_on: []
agent_type: ps-worker
files:
  - read: infra/.env
complexity: medium
done_when: "SELECT * FROM daily_checkins ORDER BY created_at DESC LIMIT 1 retorna una fila creada en los últimos 10 minutos; evidencia guardada en artifacts/e2e/2026-04-14-e2e-web/"
```

## Reference
- Web app: `https://bitacora.nuestrascuentitas.com`
- API: `https://api.bitacora.nuestrascuentitas.com`
- DB connection: `postgresql://bitacora:c3fd62bcf1bd6dba57682a06fbcabf93@postgres-reboot-solid-state-application-l55mww:5432/bitacora_db`
- sshr: `C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1`

## Prompt

### Paso 0: Encontrar usuario de test en la DB

```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker exec \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) \
   psql 'postgresql://bitacora:c3fd62bcf1bd6dba57682a06fbcabf93@postgres-reboot-solid-state-application-l55mww:5432/bitacora_db' \
   -c 'SELECT id, user_id FROM patients LIMIT 5;'"
```

**Alternativa si el container name cambia:**
```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker ps --format '{{.Names}}' | grep -i 'bitacora\|app'"
```

Buscar el email del usuario en Supabase o en una tabla de users. El user_id en patients es el sub de Supabase JWT.

### Paso 1: Obtener magic link via Supabase Admin API

Primero verificar la URL de Supabase desde el contenedor del API:
```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker exec \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) \
   printenv | grep -i 'SUPABASE\|NEXT_PUBLIC_SUPABASE'"
```

Si se encuentra `SUPABASE_SERVICE_ROLE_KEY` y `SUPABASE_URL`, generar magic link:
```bash
SUPABASE_URL="<url>"
SERVICE_KEY="<service_role_key>"
TEST_EMAIL="<email del usuario de test>"

curl -sS -X POST \
  -H "apikey: $SERVICE_KEY" \
  -H "Authorization: Bearer $SERVICE_KEY" \
  -H "Content-Type: application/json" \
  -d "{\"email\": \"$TEST_EMAIL\"}" \
  "$SUPABASE_URL/auth/v1/admin/users" # o /auth/v1/otp
```

**Alternativa:** Si hay acceso al email, simplemente usar la UI de login: ir a `https://bitacora.nuestrascuentitas.com`, ingresar el email del usuario de test, y el magic link llegará al email. Si es un email de test con acceso, abrir el link directamente.

### Paso 2: Navegar con Playwright

Usar el MCP de Playwright para ejecutar el flujo:

**2a. Abrir la app web:**
- Navegar a `https://bitacora.nuestrascuentitas.com`
- Tomar screenshot inicial
- Verificar que la página de login carga correctamente

**2b. Login con magic link:**
- Si se obtuvo el magic link en el paso 1, navegar directamente a él
- Si no, completar el formulario de login con el email del usuario de test y esperar el email

**2c. Completar consentimiento (si es la primera vez del usuario):**
- Si aparece la pantalla de consentimiento, completar el flujo
- Si ya tiene consentimiento previo, este paso se saltea automáticamente

**2d. Registrar una entrada de humor:**
- Navegar a la sección de registro de humor
- Seleccionar un valor de humor (ej: `+2`)
- Completar los factores adicionales si aparecen (sueño, físico, social, ansiedad, irritabilidad, medicación)
- Enviar el formulario
- Verificar el mensaje de confirmación

**2e. Tomar screenshots de evidencia:**
Guardar en `artifacts/e2e/2026-04-14-e2e-web/`:
- `01-login.png` — pantalla de login
- `02-consent.png` — pantalla de consentimiento (si aplica)
- `03-mood-entry.png` — formulario de humor completado
- `04-confirmation.png` — confirmación de registro exitoso

### Paso 3: Verificar en DB via sshr

```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host bitacora --cmd \
  "docker exec \$(docker ps --filter name=app-copy --format '{{.Names}}' | head -1) \
   psql 'postgresql://bitacora:c3fd62bcf1bd6dba57682a06fbcabf93@postgres-reboot-solid-state-application-l55mww:5432/bitacora_db' \
   -c 'SELECT id, patient_id, mood_score, checkin_date, created_at FROM daily_checkins ORDER BY created_at DESC LIMIT 3;'"
```

Verificar que:
1. Hay una fila con `created_at` en los últimos 10 minutos
2. `mood_score` coincide con lo que se ingresó en el UI
3. El `patient_id` corresponde al usuario de test

### Paso 4: Crear directorio de evidencias y guardar

```bash
mkdir -p artifacts/e2e/2026-04-14-e2e-web/
```

Guardar la salida del query de DB como `artifacts/e2e/2026-04-14-e2e-web/db-verification.txt`.

### Notas de troubleshooting:

**Si el login con magic link no funciona:**
- Verificar que Supabase esté accesible en `https://auth.tedi.nuestrascuentitas.com`
- Revisar logs del API: `sshr exec --host bitacora --cmd "docker logs <container> --tail 50"`

**Si la página no carga:**
- Verificar que el contenedor del frontend esté vivo: `sshr exec --host bitacora --cmd "docker ps | grep -i next\|front\|web"`
- Puede que el frontend sea un dominio diferente — verificar con `sshr exec --host bitacora --cmd "docker ps --format '{{.Names}} {{.Ports}}'"`

**Si daily_checkins no tiene la fila nueva:**
- Revisar logs del API para errores de validación
- Verificar que el usuario tiene consentimiento activo en `consent_grants`
- Revisar `access_audits` para ver si el request llegó y qué resultado tuvo

## Verify
```bash
# Via sshr - debe retornar al menos 1 fila con created_at reciente
sshr exec --host bitacora --cmd "docker exec <container> psql '<CONN_STR>' -c 'SELECT mood_score, created_at FROM daily_checkins ORDER BY created_at DESC LIMIT 1;'"
```
Output esperado: una fila con `mood_score` entre -3 y 3, `created_at` en los últimos 10 minutos.

## Commit
```
git add artifacts/e2e/2026-04-14-e2e-web/
git commit -m "test(e2e): web E2E evidence — login→consent→mood entry→db verification 2026-04-14"
```
