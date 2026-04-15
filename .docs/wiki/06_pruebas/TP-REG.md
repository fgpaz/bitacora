# TP-REG — Plan de Pruebas del Modulo REG

## Alcance

- RF cubiertos: RF-REG-001..005, RF-REG-010..015, RF-REG-020..025
- Flujos origen: FL-REG-01, FL-REG-02, FL-REG-03
- Rutas implementadas: `/registro/mood-entry`, `/registro/daily-checkin` (patient shell)

## Estado de ejecucion actual

- `Wave 1` implementa los flujos web: `MoodEntryForm` (REG-001) y `DailyCheckinForm` (REG-002) con maquina de estados en cliente.
- `MoodEntryForm` estados: `idle | submitting | success | error | consent | session`.
- `DailyCheckinForm` estados: `idle | submitting | success | error | consent | session`.
- `PatientPageShell` usado como shell unico para todas las pantallas paciente.
- `InlineFeedback` usado para errores con `trace_id` visible.
- RF-REG-010..015 siguen como plan de prueba canonico para la futura capa Telegram.
- T01 agrega un smoke backend minimo que ejecuta `POST /api/v1/mood-entries` y `POST /api/v1/daily-checkins` sobre la superficie real.

### Resultados de ejecucion (E2E 2026-04-14)

| TC ID | Estado | Ambiente | Fecha | Evidencia |
|-------|--------|----------|-------|-----------|
| REG-P01 | PASSED | produccion (API-level) | 2026-04-14 | MoodEntry `97ac6459` creado, score=2, channel=api, DB confirmado |
| REG-P03 | PASSED | produccion (API-level) | 2026-04-14 | DailyCheckin `a3d87c3a` creado, sleep=7, physical=true, medication=true, DB confirmado |

**Metodo:** JWT HS256 forjado con `SUPABASE_JWT_SECRET` de supabase-prod para usuario smoke-test `88888888-8888-8888-8888-888888888888` (status=Active, consent activo). Evidencia completa en `artifacts/e2e/2026-04-14-e2e-agresivo/evidencia-resumen.md`.

**Hallazgo bloqueante (auth misconfiguration):** La autenticacion browser real (magic link via `auth.tedi.nuestrascuentitas.com`) no puede completar llamadas API porque el backend valida con secret de instancia distinta. Ver gap en `CT-AUTH.md` y `.docs/raw/investigacion/2026-04-14-auth-misconfiguration.md`.

## Cobertura RF

| TC ID | RF | Tipo | Escenario |
|------|----|------|-----------|
| REG-P01 | RF-REG-001, RF-REG-002, RF-REG-003, RF-REG-004, RF-REG-005 | Positivo | MoodEntry web valido con consentimiento, cifrado y safe_projection |
| REG-N01 | RF-REG-002, RF-REG-004 | Negativo | Registro web bloqueado por score invalido o falta de consentimiento |
| REG-P02 | RF-REG-010, RF-REG-011, RF-REG-012, RF-REG-013 | Positivo | Registro via Telegram con webhook valido y secuencia de factores |
| REG-N02 | RF-REG-010 | Negativo | Webhook Telegram con firma invalida o update malformado |
| REG-N03 | RF-REG-014 | Negativo | Callback Telegram con chat no vinculado muestra guidance y no crea datos |
| REG-N04 | RF-REG-015 | Negativo | Callback Telegram sin consentimiento vigente muestra guidance y no crea datos |
| REG-P03 | RF-REG-020, RF-REG-021, RF-REG-022, RF-REG-023, RF-REG-024, RF-REG-025 | Positivo | DailyCheckin web con UPSERT, audit y medicacion aproximada |
| REG-N05 | RF-REG-021, RF-REG-025 | Negativo | DailyCheckin rechazado por sleep_hours invalido o falta de medication_time |
| REG-N06 | RF-REG-023, RF-REG-024 | Negativo | Fail-closed si falla cifrado o auditoria de DailyCheckin |

## Gherkin expandido

```gherkin
Scenario: Registro web de humor con score valido
  Given patient autenticado con ConsentGrant.status="granted"
  And no existe un MoodEntry duplicado en la ventana de idempotencia
  When POST /api/v1/mood-entries con {score: 2}
  Then se crea MoodEntry con safe_projection.mood_score=2
  And el payload queda cifrado con key_version vigente
  And se registra AccessAudit de create

Scenario: Registro Telegram exitoso con flujo secuencial
  Given webhook Telegram con firma valida
  And existe TelegramSession linked para el chat
  And el paciente tiene consentimiento vigente
  When llega callback_data "mood:1"
  Then se crea MoodEntry con channel="telegram"
  And el bot responde la primera pregunta de factores diarios

Scenario: Telegram sin vinculacion no crea datos
  Given webhook Telegram con firma valida
  And no existe TelegramSession linked para el chat
  When llega callback_data "mood:1"
  Then no se crea MoodEntry
  And el bot responde con instrucciones de vinculacion web

Scenario: DailyCheckin del dia se actualiza con medicacion aproximada
  Given patient autenticado con consentimiento vigente
  And ya existe DailyCheckin para la fecha actual
  When POST /api/v1/daily-checkins con medication_taken=true y medication_time="08:07"
  Then se actualiza el DailyCheckin existente
  And medication_time se normaliza al bloque aproximado correspondiente
  And safe_projection mantiene solo has_medication
  And se registra AccessAudit de update

Scenario: DailyCheckin falla cerrado si la auditoria no persiste
  Given patient autenticado con payload diario valido
  And la escritura de AccessAudit falla
  When POST /api/v1/daily-checkins
  Then se retorna 500 AUDIT_WRITE_FAILED
  And no se confirma la respuesta exitosa al cliente
```

## Estados de interfaz segun implementacion

| Estado UI | Significado | Comportamiento |
|-----------|-------------|----------------|
| `idle` | formulario listo para interaccion | campos habilitados, CTA visible |
| `submitting` | peticion en vuelo | CTA deshabilitada, aria-busy=true |
| `success` | registro guardado | feedback factual + enlaces de continuidad |
| `error` | fallo recuperable | mensaje humanizado + `InlineFeedback` con `trace_id` |
| `consent` | falta consentimiento | redireccion a `/consent` via `PatientPageShell` |
| `session` | sesion expirada | redireccion a `/ingresar` via `PatientPageShell` |

## Dependencias de validacion final

- Validacion UX de `MoodScale` ( gestos, accesibilidad, estados de foco ).
- Validacion UX de `DailyCheckinForm` ( agrupacion de bloques, condicional medicacion ).
- Validacion UX de flujos de error humanizado ( no mostrar codigos de error raw ).
- Validacion UX de continuidad post-registro ( enlaces a siguiente tarea ).
- **Sin marcar validacion UX como completa.**

## Criterios de salida

- Cobertura positiva y negativa de los 17 RF del modulo.
- Evidencia de fail-closed en cifrado y auditoria.
- Evidencia de guidance correcto en Telegram para sesiones no vinculadas y consentimiento ausente.
- Para la productivizacion backend-only, el smoke minimo debe demostrar rechazo pre-consent y exito post-consent para MoodEntry y DailyCheckin.
- **Sin marcar validacion UX como completa** — los slices de registro esperan validacion.
