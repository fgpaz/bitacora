# TECH-ROLLOUT-Y-OPERABILIDAD: Orden de Rollout, Gates Fail-Closed y Reglas Operacionales

> Root: `07_baseline_tecnica.md` invariantes T3-10, T3-11, T3-12, T3-14, T3-RL-01..03; `infra/runbooks/production-bootstrap.md`.

## Orden de rollout: Phase 30 → 31 → 40 → 41 → 50 → 60

Todo rollout de Bitacora.Api sigue esta secuencia obligatoria. No se puede abrir trafico ni marcar fase completa si un paso falla.

### Phase 30 — Backend basico

Precondition: Secrets en Dokploy + migraciones aplicadas + GATE-SMOKE-001..006 passing

Superficies: Auth, Consent, Registro, Vinculos, Visualizacion, Export

1. Secretos en Dokploy: `ZITADEL_AUTHORITY`, `ZITADEL_AUDIENCE`, `BITACORA_ENCRYPTION_KEY`, `BITACORA_PSEUDONYM_SALT`, `ConnectionStrings__BitacoraDb`.
2. `bitacora-db` deployado y reachable.
3. Migraciones aplicadas manualmente (`infra/runbooks/manual-migrations.md`).
4. `GET /health/ready` retorna 200.
5. `bitacora-api` desplegado sobre Dokploy.
6. Smoke gate pasa: `infra/smoke/zitadel-cutover-smoke.ps1` sale 0 (GATE-SMOKE-001..006).

### Phase 31 — Telegram webhook + recordatorios

Precondition: Phase 30 smoke passing + GATE-SMOKE-013..015 + Telegram smoke

Superficies: Telegram webhook entrypoint, ReminderWorker, pairing, session

7. `TELEGRAM_BOT_TOKEN` y `BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN` configurados en Dokploy.
8. Webhook registrado en Telegram Bot API.
9. Smoke Telegram pasa: GATE-SMOKE-013 (pairing), GATE-SMOKE-014 (session), GATE-SMOKE-015 (webhook).
10. ReminderWorker verificado: 1 recordatorio/paciente/dia max (T3-RL-02), skip si consent revocado (T3-RL-03).

### Phase 40 — Frontend web Next.js 16

Precondition: Phase 31 smoke + profesional endpoints (GATE-SMOKE-VIN-PROF-*, GATE-SMOKE-VIS-PROF-*) + **UX validation evidencia**

Superficies: bitacora.nuestrascuentitas.com

11. Secrets Next.js configurados: `ZITADEL_WEB_CLIENT_ID`, `ZITADEL_WEB_REDIRECT_URI`, `ZITADEL_WEB_POST_LOGOUT_REDIRECT_URI`.
12. Deployment Next.js a Vercel o Dokploy.
13. Smoke endpoints web: GATE-SMOKE-007..012.
14. **UX validation con evidencia en wiki antes de marcar completa.**

### Phase 41 — Profesional dashboard

Precondition: Phase 40 UX validation + profesional endpoints validados + **UX validation evidencia**

Superficies: profesionales.nuestrascuentitas.com

15. Smoke profesional: GATE-SMOKE-VIN-PROF-001..002, GATE-SMOKE-VIS-PROF-001.
16. **UX validation con evidencia en wiki antes de marcar completa.**

### Phase 50 — Alertas y notificaciones push

Precondition: Phase 41 + consent actualizado para canales push

17. Canal push configurado y consentido.
18. Smoke de notificaciones.

### Phase 60 — UX validation terminal

Precondition: Todas las superficies tienen evidencia de UX

19. Recopilacion de evidencia de todas las phases.
20. Cierre formal con evidencia en wiki.

### Regla de terminalidad

**Validacion de UX es actividad terminal (deferida a Phase 60).** No se marca ninguna fase como completa hasta que la validacion UX tenga evidencia. El flujo es:

```
Codigo existe → Smoke pasa → UX validado → Fase se marca completa
```

- Phase 30 y 31: smoke passing es suficiente para abrir trafico.
- Phase 40 y 41: smoke + UX validation evidencia obligatoria.
- Phase 60: cierre formal con evidencia de todas las superficies.

Nunca: `Codigo existe → Smoke pasa → Fase se marca completa`

---

## Fail-closed runtime rules

| ID | Regla | Consecuencia si se viola |
|----|-------|--------------------------|
| T3-10 | Toda falla de seguridad bloquea la operacion | 401/403 immediate; sin fallback |
| T3-11 | ConsentRequiredMiddleware es el unico gate de escritura clinica para `/mood-entries` y `/daily-checkins` | 403 + AccessAudit outcome=Denied |
| T3-12 | PseudonymizationService fail-closed: sin salt valido se lanza excepcion | 500 en toda operacion que dependa de pseudonym |
| T3-14 | Encryption key fail-closed: sin clave de 32 bytes, readiness queda not_ready | `/health/ready` retorna 503; no hay escritura clinica |
| T3-15 | Falta de configuracion Zitadel o JWKS inaccesible deja readiness not_ready | no se abre trafico |
| T3-16 | `DataAccess:ApplyMigrationsOnStartup=false` se preserva en produccion | migraciones solo via runbook explícito |
| T3-17 | Rate limiting fail-closed: politica `auth` (10 req/IP/min) retorna 429 si se excede | cualquier otro status fuera del limite |

---

## Middleware pipeline (orden fail-closed confirmado)

```
UseRateLimiter()             → fail-closed: 429 si se excede el limite (politica "auth": 10 req/IP/min)
TraceIdMiddleware           → genera trace_id al ingreso si no existe (nunca se salta)
ApiExceptionMiddleware       → envelope de error con trace_id, sin fuga de datos internos
Correlate                    → propaga X-Correlation-ID
UseAuthentication            → JWT Zitadel RS256 valido via JWKS; 401 si falla
UseAuthorization             → claims verificados; 403 si no tiene acceso
ConsentRequiredMiddleware    → hard gate para POST clinicos; 403 sin ConsentGrant activo + audit
```

### Ruta de denegacion clinica (ConsentRequiredMiddleware)

```
POST /api/v1/mood-entries  →  Sin ConsentGrant activo
POST /api/v1/daily-checkins  →  Sin ConsentGrant activo
          ↓
AccessAudit(Outcome=Denied, Action=Read)
          ↓
throw BitacoraException("CONSENT_REQUIRED", 403)
```

---

## Observabilidad minima

Todo ambiente de produccion de Bitacora.Api requiere como minimo:

| Seal | Requisito |
|------|-----------|
| Liveness | `GET /health` retorna 200 |
| Readiness | `GET /health/ready` retorna 200; valida connection string, metadata/JWKS Zitadel, encryption key, pseudonym salt, PostgreSQL connectivity |
| Tracing | `trace_id` propagado end-to-end; presente en todo log y envelope de error |
| Pseudonimizacion | `pseudonym_id` en logs operativos; `actor_id` solo en `AccessAudit` |
| Logs | `Console` provider; estructura legible con trace_id y pseudonym_id |
| Smoke | `infra/smoke/zitadel-cutover-smoke.ps1` pasa completo con exit 0; incluye OIDC discovery/JWKS, readiness, login redirect, session publica y endpoints protegidos sin sesion |

### Datos de salud: PROHIBIDO en logs, traces y telemetry

Bajo ninguna circunstancia se puede incluir en logs operativos, traces o telemetry:

- `encrypted_payload` o cualquier campo cifrado
- `safe_projection` con datos clinicos
- `patient_id` directo (solo `pseudonym_id`)
- `chat_id` vinculado a un paciente especifico
- `email`, `phone_number` o cualquier identificador directo
- Contenido de mensajes del bot Telegram relacionados con estado de animo

### Incident triggers (tratar como produccion)

- `GET /health` o `GET /health/ready` retorna != 200
- Smoke gate falla post-deploy
- Migraciones fallan o dejan readiness en rojo
- Writes de audit fallan para consentimiento o registro
- Config de cifrado o pseudonimizacion invalida

---

## Secrets y config expectations

### Variables requeridas en produccion

| Variable | Validacion | Fail-closed |
|----------|------------|-------------|
| `ZITADEL_AUTHORITY` | URL issuer canonica (`https://id.nuestrascuentitas.com`) | Readiness not_ready |
| `ZITADEL_AUDIENCE` | Project audience Bitacora (`369306332534145382`) | JWT rechazado |
| `ZITADEL_WEB_CLIENT_ID` | Client web Bitacora (`369306336963330406`) | OIDC frontend no inicia login |
| `BITACORA_ENCRYPTION_KEY` | 32 bytes exacto (Base64) | Readiness not_ready; escritura bloqueada |
| `BITACORA_PSEUDONYM_SALT` | No vacia | Throw en startup de cualquier operacion que necesite pseudonym |
| `ConnectionStrings__BitacoraDb` | Connection string valido | Readiness not_ready |
| `Telemetry__Enabled` | `true` default | - |
| `Telemetry__Otlp__Enabled` | `false` hasta que collector exista | - |
| `DataAccess:ApplyMigrationsOnStartup` | `false` en produccion | - |

### Fuente de secretos

- Control-plane y secrets compartidos: `C:\repos\mios\multi-tedi` via `mi-key-cli`
- Bridge local: `infra/.env` (no committing)
- Solo copiar en `infra/.env` los valores listados en `infra/.env.template`

---

## Limites de seguridad en runtime de Telegram (Phase 31+)

**Estado:** implementado. Phase 31 habilita el runtime Telegram completo.

| Limite | Valor | Regla | Justificacion |
|--------|-------|-------|---------------|
| Recordatorios por paciente/dia | Max 1 | T3-RL-02: throttle en ReminderWorker aunque ReminderConfig tenga mayor frecuencia | No saturar al paciente |
| Rate limit envio | 30 msg/segundo (Telegram Bot API) | ReminderWorker + SendTelegramMessageAsync con semaforo | Evitar bloqueo de API |
| Retry en recordatorios | Max 3 intentos con backoff exponencial | ReminderWorker | Evitar spam en outage |
| TTL estado conversacional | 10 minutos de inactividad | ConversationStateDictionary | No persiste estado efimero en DB |
| Pairing code TTL | 15 minutos | ReminderWorker genera codigo | Limitar ventana de ataque |
| chat_id unico | Un `chat_id` por paciente | TelegramSession unique constraint | Rechazar vinculacion multiple |
| Consent check por recordatorio | ConsentGrant activo + TelegramSession.linked | T3-RL-03: ReminderWorker checkea antes de cada envio | Revocacion inmediata de recordatorios |
| Session unlink check | TelegramSession.unlinked = false | ReminderWorker: skip si unlinked | Cortar recordatorios si paciente desvincula |

### Fail-closed reminder logic

```
ReminderWorker.Fire(reminder):
  1. check ConsentGrant activo → skip si revoked
  2. check TelegramSession.linked → skip si unlinked
  3. check throttle: ya se envio recordatorio hoy? → skip si ya se envio
  4. retry loop (max 3 intentos, backoff exponencial)
  5. AccessAudit solo si envio exitoso
  6. Nunca registrar encrypted_payload en logs de recordatorio
```

### Webhook security rules (Telegram)

1. Toda peticion a `POST /api/v1/telegram/webhook` debe tener header `X-Telegram-Bot-Api-Secret-Token` con valor igual a `BITACORA_TELEGRAM_WEBHOOK_SECRET_TOKEN`.
2. Sin header o valor incorrecto → 403 Forbidden (sin parsing del body).
3. El signature check ocurre antes de cualquier parsing del body.
4. Logs del webhook no contienen el body decrypted ni session data del paciente.

### Invariantes de no-fuga Telegram

1. **encrypted_payload no sale del proceso .NET** bajo ninguna circunstancia.
2. **safe_projection no se incluye en respuestas del bot.** El bot solo confirma receipt y pide siguiente input.
3. **Descifrado para export ocurre en memoria y se transmite como attachment**; no se almacena descifrado en disco ni caches.
4. **Logs del modulo Telegram no contienen safe_projection ni encrypted_payload.** Solo session ID y trace_id.
5. **Consent revocation o session unlink corta inmediatamente el recordatorio** sin enviar datos clinicos.

---

## Timing de validacion terminal

| Actividad | Cuando | Criterio de cierre |
|----------|---------|--------------------|
| Smoke gate | Post-deploy, antes de abrir trafico | `infra/smoke/backend-smoke.ps1` exit 0 |
| Validacion UX | Despues de smoke pasando y code feature completo | Evidencia de prototype validation en wiki |
| Readiness probe | Post-deploy y post-migracion | `GET /health/ready` retorna 200 con todos los checks verdes |
| Migraciones | Antes de abrir trafico, nunca en startup | `dotnet ef database update` exitoso + backup manual previo |
| Rollback si falla smoke | Inmediato, sin apertura de trafico | Restore from backup + redeploy |

---

## Sync gates

Cambios en este documento fuerzan revision de:

- `07_baseline_tecnica.md` ( invariantes fail-closed, pipeline middleware)
- `04_RF/RF-SEC-*` ( si cambian reglas de audit o fail-closed)
- `04_RF/RF-REG-*` ( si cambia middleware o consentimiento)
- `06_matriz_pruebas_RF.md` ( si cambian gates o smoke)
- `infra/runbooks/production-bootstrap.md` ( si cambia orden de bootstrap)
- `infra/README.md` (si cambian expectativas de secretos o config)
