#!/usr/bin/env bash
#
# apply-analytics-migration.sh
#
# Helper para aplicar la SQL migration analytics_events en dev o prod.
# Consume el connection string del entorno actual ($ConnectionStrings__BitacoraDb)
# y ejecuta `infra/migrations/bitacora/20260423_create_analytics_events.sql`
# con ON_ERROR_STOP.
#
# Uso:
#   # Dev local (connection string cargada via mkey pull o export manual):
#   bash scripts/apply-analytics-migration.sh
#
#   # Prod (runbook infra/runbooks/manual-migrations.md):
#   BITACORA_ENV=prod bash scripts/apply-analytics-migration.sh
#
# Verifica post-apply con:
#   psql "$ConnectionStrings__BitacoraDb" -c "SELECT COUNT(*) FROM analytics_events;"
#
# Rollback (si el cron todavia no corrio y queres deshacer):
#   psql "$ConnectionStrings__BitacoraDb" -c "DROP TABLE IF EXISTS analytics_events;"
#

set -euo pipefail

ENV_LABEL="${BITACORA_ENV:-dev}"
MIGRATION_FILE="infra/migrations/bitacora/20260423_create_analytics_events.sql"

if [[ -z "${ConnectionStrings__BitacoraDb:-}" ]]; then
  echo "ERROR: ConnectionStrings__BitacoraDb no esta definida en env."
  echo "  Dev: bash ~/.claude/skills/mi-key-cli/scripts/mkey.sh pull bitacora prod"
  echo "       (exporta las vars en tu shell)."
  echo "  Prod: seguir infra/runbooks/manual-migrations.md"
  exit 1
fi

if [[ ! -f "$MIGRATION_FILE" ]]; then
  echo "ERROR: migration file no encontrada: $MIGRATION_FILE"
  exit 1
fi

echo ">> Aplicando migration ($ENV_LABEL): $MIGRATION_FILE"

# Usar psql de Program Files si no esta en PATH (Windows + Git Bash).
PSQL_BIN="psql"
if ! command -v "$PSQL_BIN" >/dev/null 2>&1; then
  if [[ -x "/c/Program Files/PostgreSQL/16/bin/psql.exe" ]]; then
    PSQL_BIN="/c/Program Files/PostgreSQL/16/bin/psql.exe"
  else
    echo "ERROR: psql no disponible en PATH. Instalar PostgreSQL client."
    exit 1
  fi
fi

"$PSQL_BIN" "$ConnectionStrings__BitacoraDb" \
  -v ON_ERROR_STOP=1 \
  -f "$MIGRATION_FILE"

echo ">> Verificando tabla creada..."
"$PSQL_BIN" "$ConnectionStrings__BitacoraDb" \
  -v ON_ERROR_STOP=1 \
  -c "SELECT 'analytics_events' AS tabla, COUNT(*) AS rows FROM analytics_events;"

echo ">> Migration aplicada OK en $ENV_LABEL."

if [[ "$ENV_LABEL" == "prod" ]]; then
  echo ""
  echo "Proximos pasos recomendados:"
  echo "  1. Rebuild backend (bitacora-api) en Dokploy para cargar el nuevo endpoint."
  echo "  2. Verificar health/ready tras el redeploy."
  echo "  3. Agendar cron task de cleanup segun decision retention policy"
  echo "     (ver .docs/raw/decisiones/2026-04-23-analytics-retention-policy.md)."
fi
