# Task T4: GAP-01/02 — Fixture de cleanup Telegram para E2E

## Shared Context
**Goal:** Resolver GAP-01/02: el perfil QA `@tedi_responde` ya está vinculado al paciente `<redacted-patient-id-prefix>` de la sesión 2026-04-14, impidiendo re-ejecutar TG-P01b (vinculación fresca) para el smoke user actual.
**Stack:** PostgreSQL (SSH `turismo` + docker exec), mi-telegram-cli
**Architecture:** La tabla `telegram_sessions` vincula `telegram_chat_id` con `patient_id`. Para desvincular, DELETE la fila de `@tedi_responde` (chat_id=<redacted-telegram-chat-id>) en producción DB.

## Locked Decisions
- El DELETE apunta al `patient_id = <redacted-patient-id-prefix>-...` (smoke user de 2026-04-14), NO al smoke user actual (<redacted-smoke-user-id-prefix>).
- El @tedi_responde se reasigna al smoke user actual (<redacted-smoke-user-id-prefix>) como perfil QA dedicado.
- Documentar el runbook de cleanup en `infra/runbooks/telegram-e2e-cleanup.md`.
- NO hay un tercer perfil QA — @tedi_responde es el perfil dedicado post-cleanup.

## Task Metadata
```yaml
id: T4
depends_on: []
agent_type: ps-worker
files:
  - create: infra/runbooks/telegram-e2e-cleanup.md
complexity: medium
done_when: "telegram_sessions no tiene fila con telegram_chat_id=<redacted-telegram-chat-id>; runbook creado en infra/runbooks/"
```

## Reference
- DB container: `postgres-reboot-solid-state-application-l55mww.1.2c4bbjlda84c5vuw0boph36n7`
- SSH alias: `turismo`
- Pairing code del smoke user actual: generado en cada E2E session con POST /api/v1/telegram/pairing-code
- @tedi_responde chat_id: `<redacted-telegram-chat-id>`
- Smoke user 2026-04-14 patient ID (a desvincular): `<redacted-patient-id-prefix>` (buscar en DB para UUID completo)
- Credenciales DB: en `infra/.env` (BITACORA_DB_USER=bitacora, BITACORA_DB_PASSWORD=..., BITACORA_DB_NAME=bitacora_db)

## Prompt
El perfil QA `@tedi_responde` tiene una sesión activa vinculada al paciente de la sesión 2026-04-14. Para poder re-ejecutar el test TG-P01b en futuros E2E, necesitás:

**Paso 1: Desvincular @tedi_responde de la sesión anterior**

Conectarse a la DB de producción y eliminar la sesión de Telegram para ese chat:

```bash
ssh turismo "docker exec -i <container-name> psql -U bitacora -d bitacora_db -c \"DELETE FROM telegram_sessions WHERE telegram_chat_id = '<redacted-telegram-chat-id>';\""
```

Para obtener el container name exacto:
```bash
ssh turismo "docker ps --format '{{.Names}}' | grep postgres"
```

Verificar que el DELETE eliminó exactamente 1 fila.

**Paso 2: Verificar que la tabla quedó sin esa fila**
```bash
ssh turismo "docker exec -i <container-name> psql -U bitacora -d bitacora_db -c \"SELECT * FROM telegram_sessions WHERE telegram_chat_id = '<redacted-telegram-chat-id>';\""
```
Debe retornar 0 filas.

**Paso 3: Crear runbook en `infra/runbooks/telegram-e2e-cleanup.md`**

El runbook debe documentar exactamente estos pasos, el propósito (fixture para E2E), y la advertencia de que este DELETE es sobre producción (no staging).

**Paso 4: Documentar en TP-TG.md**

Actualizar `.docs/wiki/06_pruebas/TP-TG.md` en la sección de TG-P01b para indicar:
- Estado: FIXTURE REQUERIDO (cleanup pre-E2E)
- Procedimiento: runbook `infra/runbooks/telegram-e2e-cleanup.md`

## Execution Procedure
1. Correr `ssh turismo "docker ps --format '{{.Names}}' | grep postgres"` para obtener el container name.
2. Correr DELETE con el container name obtenido.
3. Verificar con SELECT que retorna 0 filas.
4. Crear `infra/runbooks/telegram-e2e-cleanup.md` con el Write tool — incluir los comandos exactos.
5. Actualizar `.docs/wiki/06_pruebas/TP-TG.md`: leer primero, luego editar la fila TG-P01b.
6. Reportar si la tabla `telegram_sessions` tiene columnas diferentes a las esperadas (`telegram_chat_id`, `patient_id`, `telegram_session_id`).

## Skeleton
```markdown
# Runbook: Telegram E2E Cleanup

## Propósito
Desvincular el perfil QA @tedi_responde (chat_id=<redacted-telegram-chat-id>) antes de cada ciclo E2E que incluya TG-P01b.
Este cleanup es necesario porque @tedi_responde es el perfil dedicado al smoke user actual.

## Advertencia
Este runbook modifica datos en PRODUCCIÓN. Ejecutar solo antes de un ciclo E2E planificado.

## Pasos

### 1. Obtener container name
```bash
ssh turismo "docker ps --format '{{.Names}}' | grep postgres"
```

### 2. DELETE telegram_sessions
```bash
ssh turismo "docker exec -i <CONTAINER> psql -U bitacora -d bitacora_db -c \"DELETE FROM telegram_sessions WHERE telegram_chat_id = '<redacted-telegram-chat-id>';\""
```
Resultado esperado: `DELETE 1`

### 3. Verificar
```bash
ssh turismo "docker exec -i <CONTAINER> psql -U bitacora -d bitacora_db -c \"SELECT count(*) FROM telegram_sessions WHERE telegram_chat_id = '<redacted-telegram-chat-id>';\""
```
Resultado esperado: `count = 0`

### 4. Nueva vinculación (en el E2E)
Ejecutar POST /api/v1/telegram/pairing-code con el JWT del smoke user para obtener el nuevo código.
Enviar `/start <CÓDIGO>` via @tedi_responde al bot @mi_bitacora_personal_bot.
```

## Verify
`ssh turismo "docker exec -i <container> psql -U bitacora -d bitacora_db -c \"SELECT count(*) FROM telegram_sessions WHERE telegram_chat_id = '<redacted-telegram-chat-id>';\""` → `count = 0`

## Commit
`docs(qa): add telegram E2E cleanup runbook; fix TG-P01b fixture state (GAP-01/02)`
