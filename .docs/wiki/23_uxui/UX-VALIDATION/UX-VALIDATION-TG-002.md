# UX-VALIDATION-TG-002 — Recordatorio y registro conversacional por Telegram

## Estado

`blocked_auth_credentials`

## Slice

- `TG-002` — Recordatorio y registro conversacional por Telegram.
- Cadena: `UXR-TG-002 -> UXI-TG-002 -> UJ-TG-002 -> VOICE-TG-002 -> UXS-TG-002 -> PROTOTYPE-TG-002 -> UI-RFC-TG-002 -> HANDOFF-*`.

## Evidencia disponible

| Evidencia | Estado | Nota |
| --- | --- | --- |
| E2E productivo histórico 2026-04-20 `qa-dev` | PASS | pairing, registro Telegram y privacidad del bot sin eco de valores clínicos |
| Regresión `#21` local 2026-04-20 | CODE-VERIFIED | `22:00` Buenos Aires se convierte a `{ hourUtc: 1, minuteUtc: 0 }`; backend valida sesión, hora, minuto y timezone |
| UI mobile post-hardening | PASS local | screenshots 320/375/tablet/desktop bajo `artifacts/e2e/2026-04-20-bitacora-reminder-ui-fix/` |
| E2E productivo post-deploy RF-TG-006 | BLOCKED | `artifacts/e2e/2026-04-20-bitacora-reminder-ui-prod-e2e/`; no hay credencial QA web reutilizable para completar login Zitadel |

## Hallazgos abiertos

| ID | Severidad | Estado | Acción |
| --- | --- | --- | --- |
| TG-VAL-003 | High | blocked_auth_credentials | Revalidar en producción guardar recordatorio `22:00` desde `/configuracion/telegram` con credencial QA web dedicada y cuenta Telegram QA serial |

## Criterios de cierre

- Guardar recordatorio `22:00` desde producción no devuelve `UNEXPECTED_ERROR`.
- El request observado o inferido por contrato conserva la conversión Buenos Aires local -> UTC.
- Telegram permanece vinculado.
- Logout y protección fail-closed siguen funcionando.
- Evidencia no contiene `chat_id`, phone, username personal, cookies, JWT, refresh tokens, patient IDs ni payloads clínicos.

## Decisión

No cerrar como `validated` hasta completar el E2E productivo post-deploy de la regresión `#21`.

El intento post-deploy de `2026-04-20` quedó bloqueado antes de entrar al producto: los secretos disponibles permiten operar infraestructura, pero no contienen una credencial QA web reutilizable para el login de paciente en Zitadel. No se hizo cleanup ni escritura directa en base de datos.

---

**Estado:** bloqueado por credencial QA web.
**Siguiente paso:** completar fixture QA de autenticación/pairing y reejecutar E2E productivo post-deploy.
