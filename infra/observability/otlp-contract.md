# OTLP and Runtime Observability Contract

## Default posture

- `Telemetry__Enabled=true`
- `Telemetry__Otlp__Enabled=false` unless a production collector endpoint is available
- `Telemetry__Otlp__Endpoint` stays empty until the collector is real

## Minimum production signals

- structured application logs from `Console`
- `trace_id` on error envelopes and request flow
- `pseudonym_id` for operational correlation where applicable
- liveness via `GET /health`
- readiness via `GET /health/ready`
- smoke evidence via `infra/smoke/zitadel-cutover-smoke.ps1`

## Required readiness checks

`GET /health/ready` must fail when any of these is broken:

- connection string missing
- `ZITADEL_AUTHORITY`, `ZITADEL_AUDIENCE`, or OIDC metadata missing
- encryption key missing or invalid
- pseudonym salt missing
- PostgreSQL unreachable

## Incident triggers

Treat any of these as a production incident:

- `GET /health` or `GET /health/ready` fails
- smoke gate fails after deploy
- migrations fail or leave readiness red
- audit writes fail for consent or registro
- encryption or pseudonymization config is invalid

## T01 note

Observability in this session is intentionally minimal and backend-only. The exporter may stay disabled, but the posture, failure thresholds, and smoke ownership must still be explicit.
