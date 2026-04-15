# Runbook: Telegram E2E Cleanup

## Propósito
Desvincular el perfil QA @tedi_responde (telegram_chat_id=8645234680) antes de cada ciclo E2E que incluya TG-P01b (vinculación fresca).

**Advertencia:** Este runbook modifica datos en PRODUCCIÓN. Ejecutar solo antes de un ciclo E2E planificado.

## Prerequisitos
- Acceso SSH al servidor turismo
- La sesión E2E anterior debe estar completamente cerrada
- Las migraciones de Telegram deben estar aplicadas en producción (`20260412000001_AddTelegramSessionsAndPairingCodes` y posteriores)

## Pasos

### 1. Obtener container name de PostgreSQL
```bash
ssh turismo "docker ps --format '{{.Names}}' | grep postgres"
```

Nota: Puede haber múltiples containers. Identificar el correcto buscando el que contiene la DB `bitacora_db` (típicamente el nombre incluye "reboot" o "solid-state").

### 2. DELETE de la sesión Telegram anterior
```bash
CONTAINER=<nombre del container del paso 1>
ssh turismo "docker exec -i $CONTAINER psql -U bitacora -d bitacora_db -c \"DELETE FROM telegram_sessions WHERE chat_id = '8645234680';\""
```
Resultado esperado: `DELETE 1` (o `DELETE 0` si ya estaba limpio)

### 3. Verificar cleanup
```bash
ssh turismo "docker exec -i $CONTAINER psql -U bitacora -d bitacora_db -c \"SELECT count(*) FROM telegram_sessions WHERE chat_id = '8645234680';\""
```
Resultado esperado: `count = 0`

### 4. Proceder con el E2E
- Generar nuevo pairing code: `POST /api/v1/telegram/pairing-code` con JWT del smoke user
- Enviar `/start <CÓDIGO>` via @tedi_responde al bot @mi_bitacora_personal_bot
- Verificar vinculación: `SELECT * FROM telegram_sessions WHERE chat_id = '8645234680';`

## Notas
- @tedi_responde es el perfil QA dedicado al smoke user actual (1e9df465-e464-48a7-b2f2-dc482ecbc7ce)
- Ejecutar este cleanup ANTES de cada E2E que incluya el módulo TG
- Si la tabla `telegram_sessions` no existe, las migraciones aún no fueron desplegadas. Contactar DevOps.

## Troubleshooting

### "Did not find any relation named 'telegram_sessions'"
Las migraciones de Telegram no se han aplicado en la DB de producción. Verificar que se han ejecutado:
```bash
ssh turismo "docker exec -i $CONTAINER psql -U bitacora -d bitacora_db -c \"SELECT * FROM \\\"__EFMigrationsHistory\\\" ORDER BY \\\"MigrationId\\\" DESC LIMIT 5;\""
```
Debe aparecer al menos `20260412000001_AddTelegramSessionsAndPairingCodes`.
