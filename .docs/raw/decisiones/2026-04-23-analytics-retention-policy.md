# Retention policy — analytics_events

**Fecha:** 2026-04-23
**Tipo:** decisión provisional, pendiente ratificación clínico-legal
**Scope:** política de retención para tabla `analytics_events` introducida en el closure followups v2 2026-04-23 (W8).
**Decisión provisional:** **180 días de retención rolling window con cleanup via cron task**.

---

## 1. Motivación

`analytics_events` registra eventos de UX impact (no-PII) con `patient_id` como responsable. El mandato original del follow-up #3 decía "endpoint con storage Postgres", pero no definía retention. Necesitamos política antes de que la tabla se vuelva crónica (post-merge + deploy prod).

Decisión provisional tomada por el equipo técnico (Claude + humano `fgpaz`) pendiente de ratificación clínico-legal. La política se puede revisar sin refactor de código si se ajusta solo el período.

---

## 2. Factores evaluados

### Privacy — Ley 25.326 Art. 4 inc. 7 (minimización)

> Los datos objeto de tratamiento no pueden ser utilizados para finalidades distintas o incompatibles con aquellas que motivaron su obtención.

- Los eventos de analytics tienen finalidad explícita: **medir impact del rediseño UX** para informar decisiones de producto.
- Retenerlos indefinidamente excede la finalidad original.
- 180 días cubren holgadamente ciclos de análisis trimestrales + retrospectivas.

### Utility — señales del negocio

- Análisis primarios (validar si `time_to_cta_ready` mejora post-rediseño): requieren ~90 días de baseline + ~90 días post-rediseño = 180 días.
- Análisis de cohorte (CTR rail vs checkin por período): mismo orden.
- Datos más antiguos que 6 meses pierden relevancia (el producto evoluciona).

### Storage cost

- Volumen estimado: ~1000 eventos/paciente/día × N pacientes activos × 180 días.
- Row size ~200 bytes (UUID + string 64 + jsonb 200 + UUID + timestamp).
- Con 1000 pacientes activos: ~1000 × 1000 × 180 = 180M rows ≈ 36 GB. **Orden de magnitud aceptable para PostgreSQL con índice BRIN sobre created_at_utc si crece.**
- Con pacientes iniciales < 100: ~18M rows ≈ 3.6 GB. Trivial.

### Legal residual

- `patient_id` asociado al evento podría considerarse dato personal bajo Ley 25.326 (identificable indirectamente).
- Retención prolongada amplifica el riesgo en caso de breach.
- 180 días es conservador frente a analytics típicos (12-24 meses industria) pero sano para salud mental.

---

## 3. Política definida

### Retention window

**180 días** desde `created_at_utc`. Rows con `created_at_utc < NOW() - 180 days` se eliminan via cron task dedicado.

### Mecanismo de cleanup

**Cron task server-side** (no scheduled worker .NET para mantener simpleza):

```sql
DELETE FROM analytics_events
WHERE created_at_utc < NOW() - INTERVAL '180 days';
```

Frecuencia sugerida: **diaria a las 03:00 UTC** (hora de bajo tráfico).

### Alternativas consideradas

1. **Partitioning por mes + DROP PARTITION** — mejor performance en volúmenes grandes, pero agrega complejidad operacional hoy innecesaria. Diferir hasta que el volumen lo justifique.
2. **TTL via trigger** — PostgreSQL no tiene TTL nativo. El trigger after-insert agregaría overhead.
3. **Retention por `patient_id`** (ej: eliminar al cerrar cuenta) — válido pero ortogonal: lo resuelve el flujo de eliminación de cuenta, no esta política.

### Safeguards

- **Append-only enforcement en aplicación** preservado: DELETE solo ocurre via cron task con credenciales DB dedicadas, NO desde el endpoint.
- **Audit del cleanup** opcional (P3): registrar conteo eliminado en otro log operacional si el equipo lo requiere.
- **Backup**: snapshots diarios cubren 30 días (según `infra/runbooks/backup-and-restore.md`); si se necesita recuperar eventos eliminados, restore desde snapshot del día previo a la eliminación.

---

## 4. Implementación operacional (P2 follow-up)

### Task cron propuesto

Ubicación: `infra/cron/analytics-cleanup.sh` (a crear cuando se decida ejecutar).

```sh
#!/usr/bin/env bash
set -euo pipefail

PGPASSWORD="$BITACORA_DB_PASSWORD" psql \
  -h "$BITACORA_DB_HOST" \
  -p "$BITACORA_DB_PORT" \
  -U "$BITACORA_DB_USER" \
  -d "$BITACORA_DB_NAME" \
  -c "DELETE FROM analytics_events WHERE created_at_utc < NOW() - INTERVAL '180 days';" \
  -v ON_ERROR_STOP=1
```

Registro en Dokploy como scheduled task diaria a 03:00 UTC (CRON: `0 3 * * *`).

### Runbook operacional

Post-ratificación clínico-legal, agregar sección a `infra/runbooks/` con:
- Verificación del count pre/post cleanup.
- Procedimiento de rollback si se detecta error.
- Ventana de mantenimiento si el volumen crece.

### Monitoreo

- Métrica de conteo diario: `SELECT COUNT(*) FROM analytics_events WHERE created_at_utc > NOW() - INTERVAL '1 day'`.
- Alerta si conteo cae a 0 durante 24h (puede indicar que el endpoint está caído).
- Alerta si conteo crece > 10× la media móvil de 7 días (posible abuse o bug en el cliente).

---

## 5. Trazabilidad

- Origen: closure `2026-04-23-login-flow-followups-v2-closure.md` §9 follow-up P2 #3.
- Entity: `src/Bitacora.Domain/Entities/AnalyticsEvent.cs`.
- Schema: `infra/migrations/bitacora/20260423_create_analytics_events.sql`.
- Canon técnico: `05_modelo_datos.md` + `08_modelo_fisico_datos.md` (ahora con retention policy).
- Normativa: Ley 25.326 Art. 4 inc. 7.

---

## 6. Acciones siguientes

| Acción | Owner | Estado |
|--------|-------|--------|
| Ratificar 180d con equipo clínico-legal | humano `fgpaz` + abogado/a | **pendiente** |
| Crear `infra/cron/analytics-cleanup.sh` | humano | **pendiente** |
| Registrar scheduled task en Dokploy (prod) | humano | **pendiente** |
| Actualizar canon 05/08 con la regla (este commit) | Claude | **hecho** |
| Docs-only update de `manual-migrations.md` con nota de retention | Claude | **hecho** |

---

**Estado:** decisión provisional documentada. Código no requiere cambio (la política es operacional). Merge pre-producción NO bloqueante.
