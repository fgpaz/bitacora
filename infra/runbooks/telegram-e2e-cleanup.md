# Runbook: Telegram E2E Cleanup

## Propósito
Desvincular el perfil QA `qa-dev` antes de cada ciclo E2E que incluya TG-P01b (vinculación fresca).

**Advertencia:** Este runbook modifica datos en PRODUCCIÓN. Ejecutar solo antes de un ciclo E2E planificado.
**Cuenta permitida:** usar solo `qa-dev`. No usar ni limpiar `qa-alt`.

## Prerequisitos
- Acceso SSH al servidor turismo
- La sesión E2E anterior debe estar completamente cerrada
- Las migraciones de Telegram deben estar aplicadas en producción (`20260412000001_AddTelegramSessionsAndPairingCodes`, `20260420190000_AllowTelegramRelinkAfterUnlink` y posteriores)
- El `chat_id` de Telegram es PII. No registrar valores reales en este runbook, issues, logs compartidos ni evidencia persistente.

## Pasos

### 1. Obtener container name de PostgreSQL
```bash
ssh turismo "docker ps --format '{{.Names}}' | grep postgres"
```

Nota: Puede haber múltiples containers. Identificar el correcto buscando el que contiene la DB `bitacora_db` (típicamente el nombre incluye "reboot" o "solid-state").

### 2. Soft-unlink de la sesión Telegram anterior
```bash
CONTAINER=<nombre del container del paso 1>
QA_DEV_TELEGRAM_CHAT_ID=<obtenido de forma efímera con mi-telegram-cli qa-dev>
ssh turismo "docker exec -i $CONTAINER psql -U bitacora -d bitacora_db -v chat_id=\"$QA_DEV_TELEGRAM_CHAT_ID\" -c \"UPDATE telegram_sessions SET status='Unlinked', unlinked_at_utc=now(), updated_at_utc=now(), conversation_state='Idle', pending_mood_score=NULL, pending_factors_json=NULL WHERE chat_id = :'chat_id' AND status='Linked';\""
```
Resultado esperado: `UPDATE 1` (o `UPDATE 0` si ya estaba limpio)

### 3. Verificar cleanup
```bash
ssh turismo "docker exec -i $CONTAINER psql -U bitacora -d bitacora_db -v chat_id=\"$QA_DEV_TELEGRAM_CHAT_ID\" -c \"SELECT count(*) FROM telegram_sessions WHERE chat_id = :'chat_id' AND status='Linked';\""
```
Resultado esperado: `count = 0` para `status='Linked'`. Pueden existir filas históricas `Unlinked`.

### 4. Proceder con el E2E
- Generar nuevo pairing code: `POST /api/v1/telegram/pairing` con JWT del smoke user
- Enviar `/start <CÓDIGO>` con `mi-telegram-cli --profile qa-dev` al bot @mi_bitacora_personal_bot
- Verificar vinculación con un conteo filtrado por `$QA_DEV_TELEGRAM_CHAT_ID`; no exportar filas completas ni `chat_id` real a evidencia.

## Notas
- `qa-dev` es el perfil QA dedicado para smokes Telegram con `mi-telegram-cli`.
- Mantener `QA_DEV_TELEGRAM_CHAT_ID` solo como variable local de la shell y descartarla al terminar la prueba.
- Ejecutar este cleanup ANTES de cada E2E que incluya el módulo TG
- No ejecutar DELETE físico sobre `telegram_sessions`: la traza histórica se preserva y la unicidad activa se resuelve con índice parcial `UNIQUE(chat_id) WHERE status='Linked'`.
- Si la tabla `telegram_sessions` no existe, las migraciones aún no fueron desplegadas. Contactar DevOps.

## Troubleshooting

### "Did not find any relation named 'telegram_sessions'"
Las migraciones de Telegram no se han aplicado en la DB de producción. Verificar que se han ejecutado:
```bash
ssh turismo "docker exec -i $CONTAINER psql -U bitacora -d bitacora_db -c \"SELECT * FROM \\\"__EFMigrationsHistory\\\" ORDER BY \\\"MigrationId\\\" DESC LIMIT 5;\""
```
Debe aparecer al menos `20260412000001_AddTelegramSessionsAndPairingCodes`.
