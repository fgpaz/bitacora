# TECH-CIFRADO: Patron encrypted_payload + safe_projection

> Root: `07_baseline_tecnica.md` invariante #3, `08_modelo_fisico_datos.md` seccion cifrado.

## Patron

Cada entidad clinica (MoodEntry, DailyCheckin) tiene dos representaciones:

```text
┌─────────────────────────────────────────────┐
│ Entidad clinica                             │
├─────────────────────────────────────────────┤
│ encrypted_payload  (bytea)  ← AES-256-GCM  │
│ safe_projection    (jsonb)  ← en claro      │
│ key_version        (int)    ← para rotacion │
│ encrypted_at       (timestamp)              │
└─────────────────────────────────────────────┘
```

### encrypted_payload
- Contiene el dato clinico completo (score, notas, detalles de medicacion, etc.)
- Cifrado con AES-256-GCM antes de llegar a PostgreSQL
- Solo se descifra en memoria de la aplicacion
- Nunca se expone en logs, telemetria, ni AI

### safe_projection
- Vista sanitizada con datos operacionales minimos para queries
- **MoodEntry:** `{mood_score: int, channel: string, created_at: timestamp}`
- **DailyCheckin:** `{sleep_hours: decimal, has_physical: bool, has_social: bool, has_anxiety: bool, has_irritability: bool, has_medication: bool}`
- No contiene PII ni texto libre
- Usada por timeline, dashboard, alertas y correlaciones

### Cuando se descifra encrypted_payload
1. Export CSV (RF-EXP-001) — descifra row-by-row en stream
2. Vista de detalle de un registro individual (no MVP, futuro)
3. Nunca para queries de visualizacion (usan safe_projection)

## Gestion de claves

| Campo | Valor |
|-------|-------|
| Algoritmo | AES-256-GCM |
| Key storage | Variable de entorno (`BITACORA_ENCRYPTION_KEY`) o vault |
| Key rotation | Manual. Nueva key_version, records historicos descifran con su version. |
| Key material en DB | NO. Solo key_version (int) para saber que clave usar. |

### Fail-closed (T3-10)
- Si `BITACORA_ENCRYPTION_KEY` no esta disponible o no resuelve a 32 bytes → `GET /health/ready` queda en `not_ready`.
- Si se intenta escribir o cifrar sin clave valida → la operacion falla cerrada.
- Si la clave para un key_version historico no esta → operacion falla con `ENCRYPTION_KEY_MISSING` (500).
- Ningun dato clinico se escribe sin cifrar. Nunca.

## Flujo de escritura

```text
1. API recibe {score: 1, notes: "..."}
2. Construir payload completo: {score, notes, channel, patient_id, timestamp}
3. Obtener active key_version y key material
4. AES-256-GCM encrypt → encrypted_payload (bytea)
5. Extraer campos operacionales → safe_projection (jsonb)
6. INSERT (encrypted_payload, safe_projection, key_version, encrypted_at)
```

## Flujo de lectura (visualizacion)

```text
1. API recibe GET /api/v1/mood-entries?from=&to=
2. SELECT safe_projection FROM mood_entries WHERE ... (Global Query Filter)
3. Retornar safe_projection directamente (sin descifrado)
```

## PII en tabla users

Los campos PII (first_name, last_name, email, dni, phone) se cifran con el mismo patron AES pero sin safe_projection — se descifran solo cuando se necesita mostrar el nombre del paciente.

`email_hash = SHA256(email)` se almacena para lookup sin descifrar.

## Sync gates

Cambios en cifrado fuerzan revision de:
- RF-REG-003, RF-REG-023 (cifrado de MoodEntry/DailyCheckin)
- RF-EXP-002 (descifrado para export)
- RF-SEC-001 (safe_projection nunca contiene PII)
