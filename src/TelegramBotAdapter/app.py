import logging
import os
import uuid

import httpx
from dotenv import load_dotenv
from fastapi import FastAPI, Request

load_dotenv()

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("tg-adapter")

BITACORA_API_URL: str = os.environ.get("BITACORA_API_URL", "")
TELEGRAM_WEBHOOK_SECRET: str = os.environ.get("TELEGRAM_WEBHOOK_SECRET", "")
ADAPTER_SECRET_TOKEN: str = os.environ.get("ADAPTER_SECRET_TOKEN", "")

app = FastAPI(title="TelegramBotAdapter", version="1.0.0")


@app.post("/webhook")
async def webhook(request: Request) -> dict:
    incoming_secret = request.headers.get("X-Telegram-Bot-Api-Secret-Token", "")
    if ADAPTER_SECRET_TOKEN and incoming_secret != ADAPTER_SECRET_TOKEN:
        logger.warning("webhook: invalid secret token, ignoring update")
        return {"ok": True}

    body: dict = await request.json()

    update: str
    chat_id: str
    callback_query_id: str | None

    if "message" in body:
        update = body["message"].get("text", "")
        chat_id = str(body["message"]["chat"]["id"])
        callback_query_id = None
    elif "callback_query" in body:
        update = body["callback_query"].get("data", "")
        chat_id = str(body["callback_query"]["message"]["chat"]["id"])
        callback_query_id = body["callback_query"]["id"]
    else:
        logger.info("webhook: unsupported update type, skipping")
        return {"ok": True}

    trace_id = str(uuid.uuid4())
    payload = {
        "Update": update,
        "ChatId": chat_id,
        "TraceId": trace_id,
        "CallbackQueryId": callback_query_id,
    }

    try:
        async with httpx.AsyncClient(timeout=15.0) as client:
            response = await client.post(
                f"{BITACORA_API_URL}/api/v1/telegram/webhook",
                json=payload,
                headers={
                    "X-Telegram-Webhook-Secret": TELEGRAM_WEBHOOK_SECRET,
                    "Content-Type": "application/json",
                },
            )
        logger.info("webhook forwarded: status=%s trace_id=%s", response.status_code, trace_id)
    except Exception as exc:
        logger.error("webhook forward failed: trace_id=%s error=%s", trace_id, exc)

    return {"ok": True}


@app.get("/health")
async def health() -> dict:
    return {"status": "ok", "service": "tg-adapter"}


if __name__ == "__main__":
    import uvicorn

    uvicorn.run("app:app", host="0.0.0.0", port=8080, log_level="info")
