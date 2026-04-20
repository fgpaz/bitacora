# Task T1: Deploy Adapter en Dokploy + Reconfigurar Webhook

## Shared Context
**Goal:** Poner el bot adapter en producción en el VPS turismo via Dokploy y apuntar el webhook del bot de Telegram al adapter.
**Stack:** Dokploy REST API + sshr; VPS turismo 54.37.157.93; dominio `tg-adapter.bitacora.nuestrascuentitas.com`.
**Architecture:** Nueva app Dokploy en el proyecto Bitácora existente, build desde el Dockerfile en `src/TelegramBotAdapter/`; Traefik expone con Let's Encrypt; `setWebhook` reconfigura Telegram.
**Depende de:** T0 (código del adapter commiteado y pusheado).

## Task Metadata
```yaml
id: T1
depends_on: [T0]
agent_type: ps-worker
files:
  - read: infra/.env
complexity: medium
done_when: "`curl https://tg-adapter.bitacora.nuestrascuentitas.com/health` retorna `{\"status\":\"ok\"}` Y `curl https://api.telegram.org/bot<redacted-telegram-bot-token>/getWebhookInfo` muestra `\"url\":\"https://tg-adapter.bitacora.nuestrascuentitas.com/webhook\"`"
```

## Reference
- infra/.env contiene todas las credenciales: `DOKPLOY_API_KEY`, `DOKPLOY_URL`, `BITACORA_PROJECT_ID`.
- dkp wrapper: `C:\Users\fgpaz\.agents\skills\dokploy-cli\scripts\dkp.ps1` (Windows) o `~/.agents/skills/dokploy-cli/scripts/dkp.sh` (Git Bash).
- sshr wrapper: `C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1`.

## Prompt

### Paso 0: Push del código

Asegurarse de que `src/TelegramBotAdapter/` esté en la rama `main` del remote antes de continuar:
```bash
git push origin main
```

### Paso 1: Crear nueva app en Dokploy

Usar la API de Dokploy para crear una nueva aplicación en el proyecto Bitácora:

```bash
DOKPLOY_URL="http://54.37.157.93:3000"
DOKPLOY_API_KEY="zHdqJHChVWMCUwthJhaUepBDvJOEjMweBOkQLSklbyqmJmxSDvlHqCjBdUsgpYaI"
PROJECT_ID="18WEM8BMIq-z_wgkrNlp8"

curl -sS -X POST \
  -H "x-api-key: $DOKPLOY_API_KEY" \
  -H "Content-Type: application/json" \
  -d "{
    \"name\": \"tg-adapter\",
    \"appName\": \"tg-adapter\",
    \"projectId\": \"$PROJECT_ID\",
    \"buildType\": \"dockerfile\",
    \"dockerfile\": \"./src/TelegramBotAdapter/Dockerfile\"
  }" \
  "$DOKPLOY_URL/api/application.create"
```

**Guardar el `applicationId` del response.** Llamarlo `TG_ADAPTER_APP_ID`.

### Paso 2: Configurar git source

```bash
TG_ADAPTER_APP_ID="<ID del paso 1>"

curl -sS -X POST \
  -H "x-api-key: $DOKPLOY_API_KEY" \
  -H "Content-Type: application/json" \
  -d "{
    \"applicationId\": \"$TG_ADAPTER_APP_ID\",
    \"repository\": \"https://github.com/<owner>/humor\",
    \"branch\": \"main\",
    \"buildPath\": \"/\"
  }" \
  "$DOKPLOY_URL/api/application.saveGitProvider"
```

**Nota:** Si el repo es privado, usar el mismo proveedor git ya configurado en la app Bitácora API (`UROM_r5ETX0rvs-1WZ3bi`). Verificar con `GET application.one?applicationId=UROM_r5ETX0rvs-1WZ3bi` para ver la configuración de git actual y replicarla.

### Paso 3: Configurar variables de entorno

```bash
TMPFILE=$(mktemp)
cat > "$TMPFILE" << 'EOF'
{
  "applicationId": "TG_ADAPTER_APP_ID_PLACEHOLDER",
  "env": "BITACORA_API_URL=https://api.bitacora.nuestrascuentitas.com\nTELEGRAM_WEBHOOK_SECRET=<redacted-webhook-secret>\nADAPTER_SECRET_TOKEN=<redacted-webhook-secret>"
}
EOF
# Reemplazar el placeholder con el ID real
sed -i "s/TG_ADAPTER_APP_ID_PLACEHOLDER/$TG_ADAPTER_APP_ID/" "$TMPFILE"

curl -sS -X POST \
  -H "x-api-key: $DOKPLOY_API_KEY" \
  -H "Content-Type: application/json" \
  -d @"$TMPFILE" \
  "$DOKPLOY_URL/api/application.saveEnvironment"
rm -f "$TMPFILE"
```

**Nota sobre ADAPTER_SECRET_TOKEN:** El valor `<redacted-webhook-secret>` es el mismo que el webhook secret del API — el adapter lo usará tanto para validar requests de Telegram como para incluirlo en el reenvío al API. Esto simplifica la configuración.

### Paso 4: Crear dominio con HTTPS

```bash
curl -sS -X POST \
  -H "x-api-key: $DOKPLOY_API_KEY" \
  -H "Content-Type: application/json" \
  -d "{
    \"applicationId\": \"$TG_ADAPTER_APP_ID\",
    \"host\": \"tg-adapter.bitacora.nuestrascuentitas.com\",
    \"port\": 8080,
    \"https\": true,
    \"certificateType\": \"letsencrypt\"
  }" \
  "$DOKPLOY_URL/api/domain.create"
```

**Si Let's Encrypt falla** (dominio no resuelve todavía o rate limit): usar `"https": false` temporalmente para verificar el deploy, luego habilitar HTTPS una vez que el DNS propague. Telegram requiere HTTPS para webhooks — si no hay certificado válido, el `setWebhook` fallará. En ese caso, esperar propagación DNS o usar el endpoint de la API como fallback (ver abajo).

### Paso 5: Trigger deploy

```bash
curl -sS -X POST \
  -H "x-api-key: $DOKPLOY_API_KEY" \
  -H "Content-Type: application/json" \
  -d "{\"applicationId\": \"$TG_ADAPTER_APP_ID\"}" \
  "$DOKPLOY_URL/api/application.deploy"
```

Esperar ~2 minutos y verificar con:
```bash
curl -sS \
  -H "x-api-key: $DOKPLOY_API_KEY" \
  "$DOKPLOY_URL/api/deployment.all?applicationId=$TG_ADAPTER_APP_ID" | python3 -c "import sys,json; d=json.load(sys.stdin); print(d[0]['status'] if d else 'no deployments')"
```

Verificar health:
```bash
curl -sS https://tg-adapter.bitacora.nuestrascuentitas.com/health
# Esperado: {"status":"ok","service":"tg-adapter"}
```

### Paso 6: Reconfigurar webhook de Telegram

**Verificar webhook actual:**
```bash
curl -sS "https://api.telegram.org/bot<redacted-telegram-bot-token>/getWebhookInfo" | python3 -m json.tool
```

**Reconfigurarlo al adapter:**
```bash
BOT_TOKEN="<redacted-telegram-bot-token>"
ADAPTER_SECRET="<redacted-webhook-secret>"
NEW_WEBHOOK="https://tg-adapter.bitacora.nuestrascuentitas.com/webhook"

curl -sS -X POST \
  "https://api.telegram.org/bot${BOT_TOKEN}/setWebhook" \
  -H "Content-Type: application/json" \
  -d "{
    \"url\": \"${NEW_WEBHOOK}\",
    \"secret_token\": \"${ADAPTER_SECRET}\",
    \"allowed_updates\": [\"message\", \"callback_query\"]
  }"
```

Esperado: `{"ok":true,"result":true,"description":"Webhook was set"}`

**Verificar post-configuración:**
```bash
curl -sS "https://api.telegram.org/bot${BOT_TOKEN}/getWebhookInfo" | python3 -m json.tool
```
Debe mostrar `"url": "https://tg-adapter.bitacora.nuestrascuentitas.com/webhook"` y `"has_custom_certificate": false`.

### Plan de fallback si el dominio/HTTPS falla:

Si `tg-adapter.bitacora.nuestrascuentitas.com` no está disponible con HTTPS válido, usar como alternativa el path `/api/v1/telegram/native-webhook` en el API existente — **pero esto requiere una tarea adicional en .NET**. Reportar el problema al usuario antes de implementar el fallback.

### Paso 7: Verificación de extremo a extremo del adapter

Enviar un update simulado de Telegram directamente al adapter para verificar que transforma y reenvía correctamente:

```bash
curl -sS -X POST \
  -H "Content-Type: application/json" \
  -H "X-Telegram-Bot-Api-Secret-Token: Hhl43GhDDyL0jDuoJknq8HD0UB3ukQ2HjqDDDVygZM57GHm" \
  -d '{
    "update_id": 999001,
    "message": {
      "text": "/health_test",
      "chat": {"id": 12345678, "type": "private"},
      "from": {"id": 12345678, "is_bot": false, "first_name": "Test"},
      "message_id": 1,
      "date": 1700000000
    }
  }' \
  https://tg-adapter.bitacora.nuestrascuentitas.com/webhook
```
Esperado: `{"ok": true}`

Verificar logs del adapter con sshr:
```bash
& "C:\Users\fgpaz\.agents\skills\ssh-remote\scripts\sshr.ps1" exec --host turismo --cmd "docker logs tg-adapter --tail 20 2>&1"
```

## Verify
```bash
curl -sS https://tg-adapter.bitacora.nuestrascuentitas.com/health
# → {"status":"ok","service":"tg-adapter"}

curl -sS "https://api.telegram.org/bot<redacted-telegram-bot-token>/getWebhookInfo" | grep '"url"'
# → "url": "https://tg-adapter.bitacora.nuestrascuentitas.com/webhook"
```

## Commit
```
git add infra/
git commit -m "infra(tg-adapter): deploy bot adapter to Dokploy and reconfigure Telegram webhook"
```
(Si no hay cambios en infra, no commitear — solo documentar el ID del app en el plan.)
