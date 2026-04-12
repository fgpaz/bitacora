# Decisión Operativa — Wave-prod canon gap map

**Fecha:** 2026-04-10

## Contexto

`wave-prod` se crea para normalizar el canon contra la verdad actual del repo antes de retomar trabajo pendiente del MVP.

La verificación repo-first confirma que el runtime vigente sigue siendo backend-only y que la documentación funcional/técnica continúa describiendo slices y entidades diferidas como alcance canónico, no como código ya materializado.

## Verdad implementada en el repo

- `Bitacora.Api` registra **7 módulos** en `src/Bitacora.Api/Program.cs:364-370`: `MapAuthEndpoints`, `MapConsentEndpoints`, `MapRegistroEndpoints`, `MapVinculosEndpoints`, `MapVisualizacionEndpoints`, `MapExportEndpoints`, `MapTelegramEndpoints`.
- La superficie implementada incluye (y excede) las 3 capas originales:
  - `POST /api/v1/auth/bootstrap`
  - `GET /api/v1/consent/current`
  - `POST /api/v1/consent`
  - `DELETE /api/v1/consent/current`
  - `POST /api/v1/mood-entries`
  - `POST /api/v1/daily-checkins`
  - `GET /api/v1/vinculos` + `/vinculos/active` + `/vinculos/accept` + `/vinculos/{id}` + `/vinculos/{id}/view-data`
  - `GET /api/v1/visualizacion/timeline` + `/visualizacion/summary`
  - `GET /api/v1/export/patient-summary` + `/export/patient-summary/csv`
  - `POST /api/v1/telegram/pairing` + `GET /api/v1/telegram/session` + `POST /api/v1/telegram/webhook`
  - `POST /api/v1/professional/invites` + `GET /api/v1/professional/patients`
  - `GET /api/v1/professional/patients/{patientId}/summary|timeline|alerts`
- Las entidades materializadas en `src/Bitacora.Domain/Entities/` y `AppDbContext.cs:30-34,143-219`:
  - `User`, `ConsentGrant`, `MoodEntry`, `DailyCheckin`, `PendingInvite`, `AccessAudit`, `EncryptionKeyVersion`
  - `BindingCode` (Wave 30 — implementado)
  - `CareLink` (Wave 30 — implementado)
  - `TelegramSession`, `TelegramPairingCode` (Phase 31 — implementados)
  - `ReminderConfig` (Phase 31 — implementado)
- `frontend/` existe con `package.json`, `middleware.ts`, `lib/api/professional.ts` y `.next/` buildado. El gap map anterior y `07_baseline_tecnica.md` son stale en este punto.
- Los 13 slices están abiertos a nivel `UI-RFC + HANDOFF` per `23_uxui/UI-RFC/UI-RFC-INDEX.md:47-61`. `ONB-001` no es el único slice abierto.
- La validación UX real sigue diferida a `Phase 60` con waiver vigente documented in `21_matriz_validacion_ux.md:31-38`.

## Alcance canónico pendiente

Las siguientes áreas siguen siendo parte del MVP objetivo y no están aún materializadas en runtime completo:

- Cascadas de revocación de consentimiento (`RF-CON-011..013`) — requieren `CareLink` y cache profesional operativos.
- Scheduling y envío real de recordatorios Telegram (`RF-TG-010..012`) — `ReminderWorker` registrado pero `SendTelegramMessageAsync` necesita integración completa con Bot API.
- Export streaming para datasets grandes (`RF-EXP-003`).
- Módulo `SEC` completo (`RF-SEC-*`) — auditoria de acceso profesional y fail-closed de lectura.
- Validación UX con evidencia real para los 13 slices — diferida a `Phase 60`.
- Infraestructura de staging y deploy completo de `frontend/` en Dokploy.

## Precedencia activa

- Portafolio activo para trabajo pendiente: `.docs/plans/wave-prod/`
- Portafolio histórico: `.docs/raw/plans/wave-1/`
- Si `wave-prod` y `wave-1` divergen para trabajo pendiente, prevalece `wave-prod`.
- El bootstrap backend-only de producción ya cerrado en `wave-1` se reutiliza como antecedente, pero no se reactiva como autoridad operativa.

## Hallazgos de herramienta

- `mi-lsp` debe tratarse como la primera herramienta de exploración.
- La sesión no debe asumir alias registrados de workspace solo por documentación.
- Antes de usar un alias, validar con:
  - `mi-lsp workspace list --format toon`
  - `mi-lsp workspace status <alias-o-path> --format toon`
- Si el alias no existe, usar el path del repo en `--workspace`.
- Si `mi-lsp` responde con `hint` o `next_hint`, seguir esa guía antes de hacer fallback.

## Targets de normalización de Phase 10

1. `03_FL` y `04_RF` deben distinguir con claridad runtime actual vs. trabajo pendiente, corrigiendo los estados de RF-VIN-001, RF-VIS-010..014 y RF-EXP-001 que dicen `Diferido` pero tienen endpoints implementados.
2. `05_modelo_datos.md` debe mantener estado de materialización actualizado: `BindingCode`, `CareLink`, `TelegramSession`, `TelegramPairingCode`, `ReminderConfig` como implementados en Wave 30 / Phase 31.
3. `07_baseline_tecnica.md` debe corregir que hay 7 módulos (no 3), que `frontend/` existe, que VIS/EXP/TG están materializados, y que `ReminderWorker` no es stub.
4. `08_modelo_fisico_datos.md` debe corregir que las tablas Telegram y `ReminderConfig` ya existen fisicamente.
5. `09_contratos_tecnicos.md` debe agregar los endpoints profesionales (`/professional/invites`, `/professional/patients`) como implementados y corregir referencias stale de `frontend/`.
6. `06_matriz_pruebas_RF.md`, `TP-*` e `infra/` deben evitar leer RF diferidos como cobertura ejecutable actual y corregir donde la cobertura ya existe.
7. `AGENTS.md` y `CLAUDE.md` deben quedar sincronizados para forzar exploración `mi-lsp`-first y validación explícita de workspace alias/path.

## No objetivos

- No reescribir `wave-prod` completo — solo normalizar los docs que contradicen el repo real.
- No mover ningún slice a `validated` — la validación UX real queda en `Phase 60`.
- No reabrir bootstrap de producción ya cerrado.
- No saltar a `Phase 11+` mientras `Phase 10` siga abierta.
- No eliminar slices deferidos de los indices RF — se marcan con `Implementado backend` o `Diferido` segun corresponda.

## Resultado esperado

Al cerrar `Phase 10`, el canon funcional, técnico, de pruebas y de políticas debe reflejar con precisión:

- qué existe hoy en código y base física;
- qué sigue siendo alcance pendiente del MVP;
- qué portafolio gobierna el trabajo pendiente;
- qué secuencia spec-driven continúa después.
