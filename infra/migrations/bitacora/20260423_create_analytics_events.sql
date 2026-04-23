-- Analytics events — tabla para medir UX impact del producto.
-- Follow-up #3 del closure login-flow-redesign 2026-04-23.
-- Aplica manual en producción (runbook: infra/runbooks/manual-migrations.md).
--
-- PII policy: props_json NO debe contener PII. El patient_id queda registrado
-- como responsable del evento; el equipo puede pseudonimizar en consulta si la
-- retention policy lo exige.
--
-- Retention policy: TODO a definir con el equipo (sugerido: 180 días).

BEGIN;

CREATE TABLE IF NOT EXISTS analytics_events (
    analytics_event_id UUID PRIMARY KEY,
    patient_id UUID NOT NULL,
    event_name VARCHAR(64) NOT NULL,
    props_json JSONB NULL,
    trace_id UUID NOT NULL,
    created_at_utc TIMESTAMP WITHOUT TIME ZONE NOT NULL
);

CREATE INDEX IF NOT EXISTS "IX_analytics_events_event_name_created_at_utc"
    ON analytics_events (event_name, created_at_utc);

CREATE INDEX IF NOT EXISTS "IX_analytics_events_patient_id_created_at_utc"
    ON analytics_events (patient_id, created_at_utc);

COMMIT;
