# Bot Adapter Telegram + E2E Agresivo — Implementation Plan

**Goal:** Construir y deployar el bot adapter Python que transforma el webhook nativo de Telegram al DTO interno del API; luego ejecutar E2E completo web y Telegram real para cerrar TG-P02/TG-N02 y todos los gaps residuales post-brainstorming.

**Architecture:** Nuevo microservicio Python/FastAPI `src/TelegramBotAdapter/` que actúa como proxy de transformación: recibe el JSON nativo de Telegram → normaliza a `{Update,ChatId,TraceId,CallbackQueryId}` → reenvía al API con header `X-Telegram-Webhook-Secret`. El webhook del bot de Telegram se reconfigura de `api.bitacora…/webhook` a `tg-adapter.bitacora…/webhook`.

**Tech Stack:** Python 3.12 + FastAPI + httpx | Dokploy (Dockerfile build) | Traefik + Let's Encrypt | sshr + dkp para deploy | Playwright MCP para E2E web | psql via sshr para verificación DB

**Context Source:**
- API: `https://api.bitacora.nuestrascuentitas.com` (Dokploy app `UROM_r5ETX0rvs-1WZ3bi`)
- Web: `https://bitacora.nuestrascuentitas.com` (Next.js 16)
- VPS: turismo `54.37.157.93`, Dokploy en `:3000`
- Bot token: `8609908294:AAEQpubqrpf48pSL6ERAGwxx7lNgj7dUoYI`
- Webhook secret (API valida este header): `Hhl43GhDDyL0jDuoJknq8HD0UB3ukQ2HjqDDDVygZM57GHm`
- DB: `postgresql://bitacora:c3fd62bcf1bd6dba57682a06fbcabf93@postgres-reboot-solid-state-application-l55mww:5432/bitacora_db`
- Dokploy API key: `zHdqJHChVWMCUwthJhaUepBDvJOEjMweBOkQLSklbyqmJmxSDvlHqCjBdUsgpYaI`
- Proyecto Bitácora ID: `18WEM8BMIq-z_wgkrNlp8`
- Chat_id falso actual en DB: `99887766` (creado por simulación, no usuario real)
- ReminderConfig existente: `988cf8ef-7144-40db-b41c-74d09b27ca82` (next_fire mañana)

**Runtime:** CC

**Available Agents:**
- `ps-python` — Python FastAPI microservices and LangGraph orchestration
- `ps-worker` — general-purpose file/git/config/docs/shell
- `ps-explorer` — read-only code exploration
- `ps-dotnet10` — .NET 10 microservices (endpoint, CQRS, MassTransit)
- `ps-next-vercel` — Next.js specialist
- `ps-docs` — Wiki, specs, READMEs, changelogs
- `ps-code-reviewer` — Code review for bugs, security, quality
- `ps-qa-orchestrator` — QA audit
- `ps-gap-auditor` — Gap detection spec-vs-code
- `ps-sdd-sync-gen` — Auto-generate specs from code

**Initial Assumptions:**
1. El dominio `tg-adapter.bitacora.nuestrascuentitas.com` puede ser configurado via Dokploy + Let's Encrypt en el mismo VPS.
2. La sesión de pairing existente con `chat_id=99887766` puede coexistir hasta que el usuario real haga `/start`; el unique constraint es por `patient_id`, no por `chat_id` global (confirmar antes de ejecutar T3).
3. El usuario de test para E2E web tiene su email en la tabla `patients` y puede recibir magic link vía Supabase.

---

## Risks & Assumptions

**Assumptions needing validation:**
- `tg-adapter.bitacora.nuestrascuentitas.com` no existe aún — crear registro DNS o usar IP directa temporalmente mientras Let's Encrypt provisiona. Si el dominio tarda, usar `--insecure` es inaceptable para webhook de Telegram (require HTTPS válido). Alternativa: reusar el subdominio existente del API con un path distinto agregando un endpoint proxy al API. Si el DNS falla → fallback: agregar un endpoint `/api/v1/telegram/native-webhook` al API .NET que haga la transformación internamente.
- La sesión fake con `chat_id=99887766` en `telegram_sessions` puede bloquear un nuevo pairing si la constraint es `UNIQUE(patient_id)`. Verificar con `SELECT * FROM telegram_sessions` antes de T3.

**Known risks:**
- DNS propagation delay: si `tg-adapter.bitacora.nuestrascuentitas.com` tarda en propagar, Telegram no puede validar el certificado y rechazará `setWebhook`. Mitigation: usar `https://api.bitacora.nuestrascuentitas.com/api/v1/telegram/native-webhook` como fallback en T1 si el dominio nuevo no está listo.
- E2E Telegram T3 requiere acceso físico al teléfono/app Telegram del usuario real para enviar `/start`. No automatizable por código. El executor debe ejecutar manualmente en Telegram y observar la respuesta del bot.

**Unknowns:**
- Supabase anon key y URL para el frontend: verificar en los contenedores del frontend web con `docker exec ... printenv | grep SUPABASE`.
- Email del paciente de test: verificar con `SELECT * FROM patients LIMIT 5` en la DB.

---

## Wave Dispatch Map

```mermaid
graph TD
    subgraph Wave0["Wave 0: Bot Adapter Code"]
        T0["T0: Python bot adapter code<br/>ps-python"]
    end
    subgraph Wave1["Wave 1: Parallel — Deploy + E2E Web"]
        T1["T1: Deploy adapter + setWebhook<br/>ps-worker"]
        T2["T2: E2E web completo<br/>ps-worker"]
    end
    subgraph Wave2["Wave 2: E2E Telegram Real"]
        T3["T3: E2E Telegram real user<br/>ps-worker"]
    end
    subgraph Wave3["Wave 3: Documentation Closure"]
        T4["T4: Cerrar TP-TG TG-P02/TG-N02<br/>ps-docs"]
    end
    subgraph WaveFinal["Final Wave: Traceability"]
        T5["T5: ps-trazabilidad"]
        T6["T6: ps-auditar-trazabilidad"]
        T5 --> T6
    end
    T0 --> T1
    T0 -.->|independent| T2
    T1 --> T3
    T3 --> T4
    T4 --> T5
```

**Notas de dependencias:**
- T2 (E2E web) es **independiente** de T1 (deploy adapter) — puede lanzarse en paralelo con T1.
- T3 (E2E Telegram) **requiere** T1 completo (adapter en producción y webhook reconfigurado).
- T4 requiere T3 (evidencia real de TG-P02/TG-N02).

---

## Task Index

| Task | Wave | Agent | Subdoc | Done When |
|------|------|-------|--------|-----------|
| T0 | 0 | ps-python | `./2026-04-14-bot-adapter-e2e/T0-bot-adapter-code.md` | `src/TelegramBotAdapter/` con `app.py`, `requirements.txt`, `Dockerfile` commiteados |
| T1 | 1 | ps-worker | `./2026-04-14-bot-adapter-e2e/T1-deploy-adapter.md` | Adapter vivo en producción + `getWebhookInfo` retorna nueva URL |
| T2 | 1 | ps-worker | `./2026-04-14-bot-adapter-e2e/T2-e2e-web.md` | `daily_checkins` tiene nueva fila verificada por `/db-cli` |
| T3 | 2 | ps-worker | `./2026-04-14-bot-adapter-e2e/T3-e2e-telegram.md` | `daily_checkins` con mood + sleep vía bot real confirmado en DB |
| T4 | 3 | ps-docs | `./2026-04-14-bot-adapter-e2e/T4-close-tp-tg.md` | `TP-TG.md` con TG-P02/TG-N02 marcados PASSED con evidencia real |
| T5 | F | — | inline | `ps-trazabilidad` closure completo, 0 gaps |
| T6 | F | — | inline | `ps-auditar-trazabilidad` limpio |

---

## Final Wave: Mandatory Traceability Closure

### Task T5: ps-trazabilidad

Clasificar cambio como:
- Módulo nuevo: `TelegramBotAdapter` (Python microservice)
- Cierre de test: TG-P02, TG-N02 (RF-TG-010, RF-TG-011, RF-TG-012)
- Contrato técnico: nuevo CT para el adapter (bot adapter → API boundary)

Checklist mínimo:
- [ ] `FL-TG-02.md` actualizado: refleja que el adapter está entre Telegram y el API
- [ ] `RF-TG-010..012` — mapeo a test plans TG-P02/TG-N02 cerrado con evidencia
- [ ] `09_contratos_tecnicos.md` — agregar CT-TG-003 para el adapter boundary
- [ ] `TP-TG.md` — TG-P02/TG-N02 en PASSED
- [ ] `06_matriz_pruebas_RF.md` — actualizar estado de TG
- [ ] No hay cambios en data model, 08 no requiere update

### Task T6: ps-auditar-trazabilidad

Audit read-only cross-document:
- Verificar coherencia RF-TG-002 ↔ FL-TG-01 ↔ TP-TG (TG-P01/TG-N01 ya PASSED, TG-P02/TG-N02 ahora PASSED)
- Verificar que `CT-TG-003` (adapter) no contradiga ningún RF existente
- Confirmar que `07_baseline_tecnica.md` refleje el nuevo servicio de adapter
- Si audit encuentra gaps → volver a implementación o wiki antes de cerrar
