# 06 — Matriz de Pruebas RF

## Estado ejecutable actual

- Runtime actual: backend con superficies de Vinculos, Visualizacion, Export y Telegram implementadas en Phase 31+.
- Gate ejecutable vigente: `infra/smoke/zitadel-cutover-smoke.ps1` + E2E browser con sesion real de Zitadel.
- Rate limiting activo: politica `auth` (10 req/IP/min) + fail-closed 429.
- Cobertura ejecutable hoy (smoke gate):
  - `RF-ONB-001` baseline (`POST /api/v1/auth/bootstrap`)
  - `RF-CON-001`, `RF-CON-002`, `RF-CON-003` baseline
  - `RF-REG-001` baseline
  - `RF-REG-020` baseline
  - `RF-VIN-001..004` — coverage via `GET /vinculos`, `GET /vinculos/active`, `POST /vinculos/accept` (smoke)
  - `RF-VIS-001..003` — coverage via `GET /visualizacion/timeline`, `GET /visualizacion/summary` (smoke)
  - `RF-EXP-001` — coverage via `GET /export/patient-summary` y `GET /export/patient-summary/csv` (smoke)
  - `RF-TG-001`, `RF-TG-002` — coverage via `POST /telegram/pairing`, `GET /telegram/session` (smoke)
- Cobertura E2E produccion ejecutada (2026-04-14):
  - `RF-REG-001..005` — REG-P01 PASSED: `POST /api/v1/mood-entries` score=2, MoodEntry `97ac6459` en DB, canal=api
  - `RF-REG-020..025` — REG-P03 PASSED: `POST /api/v1/daily-checkins`, DailyCheckin `a3d87c3a` en DB, sleep=7, medication=true
  - `RF-TG-010..012` — TG-P02 PASSED produccion, TG-N02 PASSED CODE-VERIFIED + guardas en produccion
  - Evidencia: `artifacts/e2e/2026-04-14-e2e-agresivo/evidencia-resumen.md`
- Cobertura E2E produccion historica ejecutada (2026-04-15 con GoTrue legacy, previa a Zitadel):
  - `RF-REG-001..005` — REG-P01 PASSED: `POST /api/v1/mood-entries` score=1, MoodEntry `477cb6e4` en DB, canal=api, cifrado OK
  - `RF-REG-001` — REG-P01b PASSED (idempotencia): repeticion mismo score retorna HTTP 200, isDuplicate=true
  - `RF-REG-020..025` — REG-P03 PASSED: `POST /api/v1/daily-checkins`, DailyCheckin `b453c2d7` en DB, sleep=7, medication=true, safe_projection OK
  - `RF-REG-002, RF-REG-004` — REG-N01 PASSED: `POST /api/v1/mood-entries` score=99 rechazado con HTTP 422, INVALID_SCORE
  - `RF-REG-010..013` — REG-P02 PASSED: flujo Telegram completo via @tedi_responde (+2, 7h, 5 factores, med 09:30)
  - `RF-VIS-001..003` — VIS-P01, VIS-P03 PASSED: timeline y summary funcionan con datos reales
  - `RF-VIN-001, RF-VIN-002, RF-VIN-004` — VIN-P01, VIN-P02 PASSED: GET /vinculos retorna [] esperado para smoke user
  - `RF-EXP-001` — EXP-P01 PASSED: CSV export funciona con headers correctos
  - Evidencia: `artifacts/e2e/2026-04-15-e2e-full/evidencia-resumen.md`
- Superficie no cubierta por smoke pero con codigo implementado (requiere test unitario o E2E):
  - `RF-REG-010..015` (Telegram webhook real)
  - `RF-CON-011..013` (cascadas de revocacion)
  - `RF-VIN-010..023` (profesional + alertas)
  - `RF-SEC-*`
- **GAP legacy (2026-04-14) RESUELTO (2026-04-15):** Auth misconfiguration GoTrue fue resuelto antes de Wave B. Desde Wave B, el runtime canonico es Zitadel y la evidencia de cierre debe apuntar a `CT-AUTH-ZITADEL.md`.
- `src/Bitacora.Tests` sigue scaffold-only; la suite ampliada queda en T10.
- Las filas diferidas del canon se preservan, pero no deben leerse como cobertura ejecutable actual.

## Modulo REG

| RF | TP | Escenario positivo | Escenario negativo |
|----|----|--------------------|--------------------|
| RF-REG-001 | TP-REG | Crea MoodEntry con score valido | Rechaza score invalido o falta de consentimiento |
| RF-REG-002 | TP-REG | Acepta score entre -3 y +3 | Rechaza score fuera de rango |
| RF-REG-003 | TP-REG | Cifra payload y construye safe_projection de MoodEntry | Falla cerrado si falta la clave |
| RF-REG-004 | TP-REG | Permite registrar con ConsentGrant vigente | Bloquea escritura clinica sin grant activo |
| RF-REG-005 | TP-REG | Reusa el registro existente dentro de la ventana de idempotencia | Evita duplicado fuera de la ventana o con score distinto |
| RF-REG-010 | TP-REG | Acepta webhook Telegram con firma valida | Rechaza firma invalida o body malformado |
| RF-REG-011 | TP-REG | Resuelve TelegramSession linked por chat_id | Falla si la sesion no existe o esta unlinked |
| RF-REG-012 | TP-REG | Crea MoodEntry desde callback Telegram | Rechaza callback malformado o sin consentimiento |
| RF-REG-013 | TP-REG | Avanza la secuencia de factores por Telegram | Corta la secuencia ante timeout o respuesta invalida |
| RF-REG-014 | TP-REG | Informa guidance si el chat no esta vinculado | No crea datos y audita el rechazo si falta sesion |
| RF-REG-015 | TP-REG | Redirige a la web si falta consentimiento via Telegram | No crea datos y audita el bloqueo |
| RF-REG-020 | TP-REG | Crea o actualiza DailyCheckin con payload valido | Rechaza checkin sin consentimiento o datos invalidos |
| RF-REG-021 | TP-REG | Valida factores diarios y normaliza hora aproximada | Rechaza sleep_hours u horario de medicacion invalidos |
| RF-REG-022 | TP-REG | Inserta el primer DailyCheckin del dia | Actualiza el existente y reporta error si falla DB |
| RF-REG-023 | TP-REG | Cifra DailyCheckin y excluye medication_time de safe_projection | Falla cerrado si no hay clave disponible |
| RF-REG-024 | TP-REG | Registra audit de alta o actualizacion de DailyCheckin | Falla cerrado si no puede persistir AccessAudit |
| RF-REG-025 | TP-REG | Persiste medicacion con horario aproximado normalizado | Rechaza falta de horario cuando medication_taken=true |

## Modulo CON

| RF | TP | Escenario positivo | Escenario negativo |
|----|----|--------------------|--------------------|
| RF-CON-001 | TP-CON | Devuelve consentimiento activo autenticado y patient_status | Falla si no existe configuracion activa |
| RF-CON-002 | TP-CON | Registra ConsentGrant con version vigente | Rechaza version desactualizada o grant duplicado |
| RF-CON-003 | TP-CON | Permite escritura clinica con grant activo | Bloquea POST clinicos sin consentimiento |
| RF-CON-010 | TP-CON | Revoca consentimiento con confirmacion explicita | Rechaza revocacion sin confirmacion o sin grant activo |
| RF-CON-011 | TP-CON | Revoca CareLinks asociados al revocar consentimiento | Hace no-op si no hay links activos o falla el cascade |
| RF-CON-012 | TP-CON | Invalida caches de safe_projection tras revocacion | Deja warning si cache no responde sin romper la revocacion |
| RF-CON-013 | TP-CON | Ejecuta revocacion atomica de consentimiento y links | Hace rollback total si falla una etapa ACID |

## Modulo VIN

| RF | TP | Escenario positivo | Escenario negativo |
|----|----|--------------------|--------------------|
| RF-VIN-001 | TP-VIN | Crea CareLink invitado o PendingInvite segun exista el paciente | Rechaza invitacion duplicada o email invalido |
| RF-VIN-002 | TP-VIN | Resuelve patient_id por email hash normalizado | Retorna null si el email no existe |
| RF-VIN-003 | TP-VIN | Activa CareLink invitado del paciente owner | Rechaza aceptacion por no-owner o status invalido |
| RF-VIN-004 | TP-VIN | Fuerza can_view_data=false en toda creacion de CareLink | Ignora cualquier intento de crearlo en true |
| RF-VIN-010 | TP-VIN | Genera BindingCode con preset valido y TTL esperado | Rechaza ttl_preset fuera del catalogo |
| RF-VIN-011 | TP-VIN | Valida BindingCode y resuelve professional_id | Rechaza codigo inexistente, expirado o usado |
| RF-VIN-012 | TP-VIN | Auto-vincula al paciente con BindingCode valido | Rechaza codigo invalido, expirado o link duplicado |
| RF-VIN-020 | TP-VIN | Revoca CareLink por decision del paciente | Rechaza revocacion por no-owner o status invalido |
| RF-VIN-021 | TP-VIN | Invalida cache al revocar un vinculo | Deja warning sin revertir la revocacion si falla cache |
| RF-VIN-022 | TP-VIN | Verifica ownership correcto del CareLink | Rechaza accesso sin ownership o link inexistente |
| RF-VIN-023 | TP-VIN | Habilita o deshabilita can_view_data por el paciente owner | Rechaza cambio por professional o link no activo |

## Modulo VIS

| RF | TP | Escenario positivo | Escenario negativo |
|----|----|--------------------|--------------------|
| RF-VIS-001 | TP-VIS | Devuelve timeline de MoodEntry en rango valido | Rechaza rango invalido o sin autenticacion |
| RF-VIS-002 | TP-VIS | Devuelve DailyCheckin en rango valido | Rechaza rango invalido o sin autenticacion |
| RF-VIS-003 | TP-VIS | Pagina periodos largos con cursor valido | Rechaza cursor o page_size invalidos |
| RF-VIS-010 | TP-VIS | Lista solo pacientes visibles del profesional | Oculta pacientes con can_view_data=false o rol invalido |
| RF-VIS-011 | TP-VIS | Calcula resumen de 7 dias para paciente visible | Falla cerrado si no hay CareLink visible o auditoria |
| RF-VIS-012 | TP-VIS | Calcula alertas LOW_MOOD_STREAK cuando corresponde | No genera alertas con dias no consecutivos o falla audit |
| RF-VIS-013 | TP-VIS | Pagina lista de pacientes del dashboard | Rechaza cursores inconsistentes o page_size excesivo |
| RF-VIS-014 | TP-VIS | Genera audit por cada paciente expuesto al profesional | Falla cerrado si no puede persistir el batch de audit |

## Modulo EXP

| RF | TP | Escenario positivo | Escenario negativo |
|----|----|--------------------|--------------------|
| RF-EXP-001 | TP-EXP | Genera CSV con headers estandarizados y filas por dia | Rechaza rango invalido o falla de descifrado |
| RF-EXP-002 | TP-EXP | Descifra payloads por key_version correcta | Falla si falta la clave requerida |
| RF-EXP-003 | TP-EXP | Hace streaming de datasets grandes sin buffer completo | Reporta error si el stream se interrumpe |

## Modulo TG

| RF | TP | Escenario positivo | Escenario negativo |
|----|----|--------------------|--------------------|
| RF-TG-001 | TP-TG | Genera pairing code BIT-XXXXX con TTL de 15 minutos | Rechaza solicitud sin consentimiento o JWT valido |
| RF-TG-002 | TP-TG | Vincula chat_id con /start y codigo activo | Responde guidance si el codigo es invalido, expirado o duplicado |
| RF-TG-003 | TP-TG | Garantiza unicidad de chat_id por paciente | Rechaza chat_id ya vinculado a otra cuenta |
| RF-TG-005 | TP-TG | Desvincula sesion Telegram desde UI web con confirmacion | Retorna 404 si no hay sesion activa (TG-P05, TG-N04) |
| RF-TG-006 | TP-TG | Configura y consulta horario de recordatorio con timezone desde UI; convierte hora local BA a UTC y reconstruye hora local al recargar | Retorna 403 sin sesion activa en PUT; 400 con hora/minuto/timezone invalido; GET sin config retorna `configured=false` (TG-P06, TG-P06b, TG-N05, TG-N06) |
| RF-TG-010 | TP-TG | Ejecuta scheduler y detecta recordatorios vencidos | Tolera error de DB o planificacion inconsistente |
| RF-TG-011 | TP-TG | Envia mensaje con keyboard inline correcto | Reporta fallo si falta token o no existe chat |
| RF-TG-012 | TP-TG | Omite envio cuando hay consent revocado o session unlinked | Rechaza ejecucion con datos de sesion invalidos |

## Modulo SEC

| RF | TP | Escenario positivo | Escenario negativo |
|----|----|--------------------|--------------------|
| RF-SEC-001 | TP-SEC | Intercepta acceso profesional y audita lectura permitida | Bloquea acceso si no existe permiso o falla audit |
| RF-SEC-002 | TP-SEC | Genera pseudonym_id estable a partir de actor_id y salt | Falla si no existe salt configurado |
| RF-SEC-003 | TP-SEC | No retorna datos cuando la auditoria falla | Falla cerrado ante cualquier error de audit |

## Modulo ONB

| RF | TP | Escenario positivo | Escenario negativo |
|----|----|--------------------|--------------------|
| RF-ONB-001 | TP-ONB | Crea usuario local y reanuda PendingInvite valida | Rechaza JWT invalido o evita duplicar usuario existente |
| RF-ONB-002 | TP-ONB | Detecta correctamente usuario nuevo vs existente | Reporta error si falla la consulta local |
| RF-ONB-003 | TP-ONB | Fuerza consentimiento y consume PendingInvite vigente | Bloquea acceso a datos si el usuario sigue registered |
| RF-ONB-004 | TP-ONB | Registra el primer MoodEntry tras consentimiento | Rechaza primer registro con score invalido o sin consent |
| RF-ONB-005 | TP-ONB | Transiciona a active tras el primer MoodEntry | Hace no-op si ya estaba active o loguea estado inesperado |

## Gates operacionales (smoke + fail-closed)

Estos gates no corresponden a un modulo RF unico pero son ejecutados en el smoke y en los gates de rollout:

| Gate | Endpoint / Accion | Criterio pass | Criterio fail |
|------|-------------------|---------------|---------------|
| GATE-SMOKE-001 | `GET /health` | 200 | != 200 |
| GATE-SMOKE-002 | `GET /health/ready` | 200 | != 200 |
| GATE-SMOKE-003 | `POST /api/v1/mood-entries` sin consent | 403 + CONSENT_REQUIRED | cualquier otro status |
| GATE-SMOKE-004 | `POST /api/v1/consent` | 201 o 409 | otro status |
| GATE-SMOKE-005 | `POST /api/v1/mood-entries` con consent | 200 o 201 | no 2xx |
| GATE-SMOKE-006 | `POST /api/v1/daily-checkins` con consent | 200 o 201 | no 2xx |
| GATE-FAIL-001 | `GET /health/ready` sin metadata/JWKS de Zitadel alcanzable | 503 o throw en startup | startup ok |
| GATE-FAIL-002 | `GET /health/ready` sin BITACORA_ENCRYPTION_KEY valida | 503 | 200 |
| GATE-FAIL-003 | `GET /health/ready` sin BITACORA_PSEUDONYM_SALT | 500 o throw | 200 |
| GATE-SMOKE-007 | `GET /api/v1/vinculos` con JWT valido | 200 | no 2xx |
| GATE-SMOKE-008 | `GET /api/v1/vinculos/active` con JWT valido | 200 | no 2xx |
| GATE-SMOKE-009 | `GET /api/v1/visualizacion/timeline?from=&to=` con JWT valido | 200 | no 2xx |
| GATE-SMOKE-010 | `GET /api/v1/visualizacion/summary?from=&to=` con JWT valido | 200 | no 2xx |
| GATE-SMOKE-011 | `GET /api/v1/export/patient-summary?from=&to=` con JWT valido | 200 | no 2xx |
| GATE-SMOKE-012 | `GET /api/v1/export/patient-summary/csv?from=&to=` con JWT valido | 200 | no 2xx |
| GATE-SMOKE-016 | `GET /api/v1/export/{patientId}/constraints` con JWT valido (patient owner y professional) | 200 + `allowed` true/false segun rol | no 2xx |
| GATE-SMOKE-013 | `POST /api/v1/telegram/pairing` con JWT valido y consentimiento | 200 | no 2xx |
| GATE-SMOKE-014 | `GET /api/v1/telegram/session` con JWT valido | 200 | no 2xx |
| GATE-SMOKE-015 | `POST /api/v1/telegram/webhook` con secret token valido | 200 | no 200 |
| GATE-SMOKE-VIN-PROF-001 | `POST /api/v1/professional/invites` con JWT professional valido | 201 | no 2xx |
| GATE-SMOKE-VIN-PROF-002 | `GET /api/v1/professional/patients` con JWT professional valido | 200 | no 2xx |
| GATE-SMOKE-VIS-PROF-001 | `GET /api/v1/professional/patients/{patientId}/summary` con JWT professional + CareLink valido | 200 | no 2xx |
| GATE-SMOKE-VIS-PROF-002 | `GET /api/v1/professional/patients/{patientId}/timeline?from=&to=` con JWT professional + CareLink valido | 200 | no 2xx |
| GATE-SMOKE-VIS-PROF-003 | `GET /api/v1/professional/patients/{patientId}/alerts?from=&to=` con JWT professional + CareLink valido | 200 | no 2xx |
| GATE-FAIL-004 | `GET /health/ready` sin ConnectionStrings__BitacoraDb reachable | 503 | 200 |
| GATE-RL-001 | `POST /api/v1/auth/bootstrap` 11 veces en 1 min desde misma IP | 429 + Retry-After header | otro status |
| GATE-SMOKE-TG-001 | ReminderWorker: 2 recordatorios configurados para mismo paciente/dia, solo 1 se envia | Log: "reminder skipped by throttle", AccessAudit no generado | mas de 1 envio |
| GATE-SMOKE-TG-002 | ReminderWorker: ConsentGrant revocado antes de envio | Log: "reminder skipped: consent revoked", AccessAudit no generado | se genera intento de envio |
| GATE-SMOKE-TG-003 | ReminderWorker: TelegramSession unlinked antes de envio | Log: "reminder skipped: session unlinked", AccessAudit no generado | se genera intento de envio |

---

*Fuente: `.docs/wiki/04_RF.md` y `.docs/wiki/04_RF/RF-*.md`, `07_tech/TECH-ROLLOUT-Y-OPERABILIDAD.md`*
