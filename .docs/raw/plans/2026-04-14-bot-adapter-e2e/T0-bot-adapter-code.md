# Task T0: Python Bot Adapter — Código y Dockerfile

## Shared Context
**Goal:** Crear el microservicio Python que transforma el webhook nativo de Telegram al DTO interno del API Bitácora.
**Stack:** Python 3.12 + FastAPI + httpx + Uvicorn; Dockerfile multi-stage; deploy en Dokploy.
**Architecture:** Proxy stateless: Telegram → adapter (transform) → `POST /api/v1/telegram/webhook` con header `X-Telegram-Webhook-Secret`.
**Ubicación en repo:** `src/TelegramBotAdapter/` (directorio nuevo dentro del monorepo existente).

## Task Metadata
```yaml
id: T0
depends_on: []
agent_type: ps-python
files:
  - create: src/TelegramBotAdapter/app.py
  - create: src/TelegramBotAdapter/requirements.txt
  - create: src/TelegramBotAdapter/Dockerfile
  - read: infra/.env
complexity: low
done_when: "src/TelegramBotAdapter/ tiene los 3 archivos creados y commiteados; `docker build -t tg-adapter src/TelegramBotAdapter/` exits 0 localmente"
```

## Reference
No hay servicios Python previos en este repo. Seguir el patrón FastAPI minimal (single-file app, sin capa de dominio compleja).

## Prompt
Crear el directorio `src/TelegramBotAdapter/` en el monorepo `C:\repos\mios\humor` y escribir 3 archivos:

### 1. `src/TelegramBotAdapter/requirements.txt`

Contenido exacto:
```
fastapi==0.115.0
uvicorn[standard]==0.30.6
httpx==0.27.2
python-dotenv==1.0.1
```

### 2. `src/TelegramBotAdapter/app.py`

El archivo debe implementar exactamente este comportamiento:

**Variables de entorno requeridas:**
- `BITACORA_API_URL` — URL base del API (ej: `https://api.bitacora.nuestrascuentitas.com`)
- `TELEGRAM_WEBHOOK_SECRET` — secreto que el adapter añade como header `X-Telegram-Webhook-Secret` al reenviar
- `ADAPTER_SECRET_TOKEN` — token que Telegram enviará en el header `X-Telegram-Bot-Api-Secret-Token` (para validar que el request viene de Telegram)

**Endpoint `POST /webhook`:**
1. Leer el header `X-Telegram-Bot-Api-Secret-Token` del request
2. Si `ADAPTER_SECRET_TOKEN` está configurado (no vacío), validarlo contra el header. Si no coincide → retornar `{"ok": True}` inmediatamente (siempre 200, Telegram no debe reintentar)
3. Leer el body JSON del update nativo de Telegram
4. Transformar según el tipo de update:
   - Si hay `body["message"]`:
     - `update = body["message"].get("text", "")`
     - `chat_id = str(body["message"]["chat"]["id"])`
     - `callback_query_id = None`
   - Si hay `body["callback_query"]`:
     - `update = body["callback_query"].get("data", "")`
     - `chat_id = str(body["callback_query"]["message"]["chat"]["id"])`
     - `callback_query_id = body["callback_query"]["id"]`
   - Cualquier otro tipo de update: retornar `{"ok": True}` sin reenviar
5. Generar `trace_id = str(uuid.uuid4())`
6. Construir payload:
   ```json
   {
     "Update": "<update>",
     "ChatId": "<chat_id>",
     "TraceId": "<trace_id>",
     "CallbackQueryId": "<callback_query_id_or_null>"
   }
   ```
7. Hacer `POST` a `{BITACORA_API_URL}/api/v1/telegram/webhook` con:
   - Header `X-Telegram-Webhook-Secret: {TELEGRAM_WEBHOOK_SECRET}`
   - Header `Content-Type: application/json`
   - Body: el payload construido
   - Timeout: 15 segundos
8. Log del resultado (info: status code + trace_id; error si falla la conexión)
9. Siempre retornar `{"ok": True}` a Telegram (incluso si el API falla)

**Endpoint `GET /health`:**
- Retorna `{"status": "ok", "service": "tg-adapter"}`

**Error handling:**
- Cualquier excepción en el reenvío debe ser capturada, logueada, y NO propagada — siempre retornar `{"ok": True}` a Telegram.
- Si el body del request de Telegram está malformado → log warning, retornar `{"ok": True}`.

**Main entry point:**
```python
if __name__ == "__main__":
    import uvicorn
    uvicorn.run("app:app", host="0.0.0.0", port=8080, log_level="info")
```

### 3. `src/TelegramBotAdapter/Dockerfile`

```dockerfile
FROM python:3.12-slim AS base

WORKDIR /app

# Install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy app
COPY app.py .

# Non-root user for security
RUN useradd -m -u 1001 appuser && chown -R appuser:appuser /app
USER appuser

EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD python -c "import urllib.request; urllib.request.urlopen('http://localhost:8080/health')"

CMD ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "8080", "--log-level", "info"]
```

### Notas importantes:
- NO crear `__init__.py` ni estructura de paquetes. Es un single-file app.
- NO añadir dependencias innecesarias (no SQLAlchemy, no Pydantic models, no pytest aún).
- El `CallbackQueryId` en el payload debe ser `null` (Python `None`) cuando no hay callback_query, no un string vacío. httpx/json lo serializará correctamente a `null`.
- El campo `TraceId` debe ser un string UUID v4 válido (el API espera `Guid` parseado desde string).

## Skeleton

```python
import os
import uuid
import logging
from fastapi import FastAPI, Request
import httpx

logger = logging.getLogger("tg-adapter")
logging.basicConfig(level=logging.INFO)

app = FastAPI(title="Telegram Bot Adapter", version="1.0.0")

API_URL = os.environ.get("BITACORA_API_URL", "")
WEBHOOK_SECRET = os.environ.get("TELEGRAM_WEBHOOK_SECRET", "")
ADAPTER_SECRET = os.environ.get("ADAPTER_SECRET_TOKEN", "")


@app.post("/webhook")
async def webhook(request: Request):
    # 1. Validate Telegram secret
    # 2. Parse native Telegram update
    # 3. Transform to internal DTO
    # 4. Forward to API
    # 5. Always return {"ok": True}
    ...


@app.get("/health")
async def health():
    return {"status": "ok", "service": "tg-adapter"}


if __name__ == "__main__":
    import uvicorn
    uvicorn.run("app:app", host="0.0.0.0", port=8080, log_level="info")
```

## Verify
```
cd C:\repos\mios\humor
docker build -t tg-adapter-test src/TelegramBotAdapter/ && echo "BUILD OK"
```
Output esperado: `BUILD OK` (sin errores de sintaxis Python ni de copia de archivos).

Si Docker no está disponible localmente: validar con `python -m py_compile src/TelegramBotAdapter/app.py && echo SYNTAX OK`.

## Commit
```
git add src/TelegramBotAdapter/
git commit -m "feat(tg-adapter): add Python bot adapter service for Telegram webhook transformation"
```
