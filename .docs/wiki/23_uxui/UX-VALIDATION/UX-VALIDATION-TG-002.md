# UX-VALIDATION-TG-002 — Recordatorio y registro conversacional por Telegram

## Estado

`validated_with_qa_dev_evidence`

## Slice

- `TG-002` — Recordatorio y registro conversacional por Telegram.
- Cadena: `UXR-TG-002 -> UXI-TG-002 -> UJ-TG-002 -> VOICE-TG-002 -> UXS-TG-002 -> PROTOTYPE-TG-002 -> UI-RFC-TG-002 -> HANDOFF-*`.

## Evidencia disponible

| Evidencia | Estado | Nota |
| --- | --- | --- |
| E2E productivo histórico 2026-04-20 `qa-dev` | PASS | pairing, registro Telegram y privacidad del bot sin eco de valores clínicos |
| Regresión `#21` local 2026-04-20 | CODE-VERIFIED | `22:00` Buenos Aires se convierte a `{ hourUtc: 1, minuteUtc: 0 }`; backend valida sesión, hora, minuto y timezone |
| E2E productivo post-fix RF-TG-006 2026-04-20 `qa-dev` | PASS | `/configuracion/telegram` guardó 22:00 Buenos Aires sin 500; Telegram permaneció vinculado; logout fail-closed. Evidencia: `artifacts/e2e/2026-04-20-bitacora-reminder-ui-qa-dev/` |
| UI mobile post-hardening | PASS local | screenshots 320/375/tablet/desktop bajo `artifacts/e2e/2026-04-20-bitacora-reminder-ui-fix/` |
| Persistencia de horario al recargar 2026-04-20 | PASS local | Playwright con sesión sintética y API mockeada confirmó `Actual: 22:00, hora de Buenos Aires` sin overflow en 320/375/desktop. Evidencia: `artifacts/e2e/2026-04-20-telegram-guided-setup-persisted-reminder/` |

## Hallazgos abiertos

| ID | Severidad | Estado | Acción |
| --- | --- | --- | --- |
| TG-VAL-003 | High | resuelto | Validado con evidencia productiva `qa-dev` y seguimiento local sintético para persistencia UI |

## Criterios de cierre

- Guardar recordatorio `22:00` desde producción no devuelve `UNEXPECTED_ERROR`.
- El request observado o inferido por contrato conserva la conversión Buenos Aires local -> UTC.
- Telegram permanece vinculado.
- Logout y protección fail-closed siguen funcionando.
- Evidencia no contiene `chat_id`, phone, username personal, cookies, JWT, refresh tokens, patient IDs ni payloads clínicos.

## Decisión

Cerrar como `validated_with_qa_dev_evidence` para el alcance `TG-002` y la regresión `#21`.

La cuenta Telegram productiva del equipo no se usa para QA. La evidencia vigente usa `qa-dev` para validación productiva y sesiones sintéticas locales solo para layout/persistencia UI. No se hizo cleanup ni escritura directa en base de datos.

---

**Estado:** validado con evidencia QA `qa-dev`.
**Siguiente paso:** repetir el smoke productivo con `qa-dev` si se redeploya el flujo de Telegram o cambia el fixture de autenticación.
