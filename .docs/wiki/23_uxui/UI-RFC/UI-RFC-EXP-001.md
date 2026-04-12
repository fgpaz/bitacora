# UI-RFC-EXP-001 — Exportación CSV del paciente

## Propósito

Este documento traduce `EXP-001` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS`. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete `EXP-001`.

## Estado del gate

- `slice`: `EXP-001`
- `estado`: `abierto para UI-RFC + HANDOFF`
- `fundamento`: autoridad del plan T3 — `wave-prod 11-docs-uxui-canon`
- `límite`: este contrato no equivale a validación UX real
- `deuda explícita`: la validación real sigue diferida a `Phase 60`

## Referencias obligatorias

- `../../16_patrones_ui.md`
- `../../21_matriz_validacion_ux.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../07_baseline_tecnica.md`
- `../../09_contratos_tecnicos.md`
- `../../06_matriz_pruebas_RF.md`
- `../../06_pruebas/TP-EXP.md`
- `../../03_FL/FL-EXP-01.md`
- `../../04_RF/RF-EXP-001.md`
- `../../04_RF/RF-EXP-002.md`
- `../../04_RF/RF-EXP-003.md`
- `../UXR/UXR-EXP-001.md`
- `../UXI/UXI-EXP-001.md`
- `../UJ/UJ-EXP-001.md`
- `../VOICE/VOICE-EXP-001.md`
- `../UXS/UXS-EXP-001.md`
- `../PROTOTYPE/PROTOTYPE-EXP-001.md`
- `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md`

## Slice cubierto

### In

- entrada con alcance visible;
- selección o confirmación de período;
- preparación y disparo de descarga.

### Out

- timeline;
- detalle de registro;
- variante Telegram.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-DEFAULT` | alcance visible y CTA disponible | llega a exportación | permite decidir |
| `S01-PERIOD` | período seleccionado y alcance actualizado | cambio de filtro | permite confirmar |
| `S02-GENERATING` | preparación breve del archivo | click en CTA | sostiene certeza |
| `S02-SUCCESS` | descarga iniciada o lista para continuar | `200` + file download | cierra sin pantalla extra |
| `S03-ERROR` | error recuperable con reintento | error de generación | permite reintentar |

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `PatientPageShell` | shell general del slice | loading, ready, error |
| `ExportScopeBlock` | resumen del alcance del archivo | default |
| `PeriodSelector` | selector simple de período | default, selected |
| `ExportActionBar` | CTA de descarga | default, disabled, loading |
| `GeneratingFeedback` | feedback durante preparación | default |
| `InlineFeedback` | error localized | error, retry |

## Contratos backend que la UI debe consumir

### 1. Exportación JSON estructurada

- request:
  - `GET /api/v1/export/patient-summary?from=<DateOnly>&to=<DateOnly>`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada:
  - `PatientExportDto` con `patientId`, `dateRange`, `exportedAtUtc`, `summary`, `entries[]`
- errores esperados:
  - `INVALID_DATE_RANGE` — `to < from` (400 Bad Request)
  - `SERVICE_UNAVAILABLE` — error recuperable

### 2. Exportación CSV directa

- request:
  - `GET /api/v1/export/patient-summary/csv?from=<DateOnly>&to=<DateOnly>`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada:
  - `Content-Type: text/csv`
  - `Content-Disposition: attachment; filename="bitacora-export-<yyyyMMdd>-<yyyyMMdd>.csv"`
  - cuerpo: archivo CSV con headers (date, mood_score, mood_time, sleep_hours, physical, social, anxiety, irritability, medication, med_time)
- errores esperados:
  - `INVALID_DATE_RANGE` — `to < from` (400 Bad Request)
  - `SERVICE_UNAVAILABLE` — error recuperable

**Nota:** Endpoints verificados via mi-lsp (2026-04-12). Corresponden a `ExportPatientSummaryQuery`. El endpoint CSV usa GET con query params, no POST con body.

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| llega a exportación | muestra alcance | default | permite decidir |
| selecciona período | actualiza alcance | período elegido | `S01-PERIOD` |
| click `Descargar CSV` | `GET /export/patient-summary/csv?from=&to=` | `200` | `S02-GENERATING` luego descarga directa |
| sin registros | `GET /export/patient-summary/csv?from=&to=` | `200` con entries vacías | CSV con headers y filas vacías |
| error de generación | `GET /export/patient-summary/csv` | error | `S03-ERROR` |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `NO_ENTRIES` | genera CSV con headers o copy explícito | no confunde ni dramatiza |
| `GENERATION_IN_PROGRESS` | feedback de preparación | no mostrar error técnico |
| `SERVICE_UNAVAILABLE` | error recuperable con reintento | no introducir jerga de backend |
| `NETWORK_ERROR` | feedback de error de conexión | proponer reintentar |

## Reglas de jerarquía y copy

- queda claro qué datos incluye el archivo antes de disparar;
- el inicio de la descarga tiene feedback suficiente;
- la interfaz no se llena de jerga técnica;
- la exportación se presenta como derecho simple de acceso.

## Responsive y accesibilidad

- mobile-first;
- el selector de período es funcional en mobile;
- `prefers-reduced-motion` respetado.

## Criterio de implementación

1. el alcance del archivo se entiende antes de descargar;
2. el selector de período no agrega fricción innecesaria;
3. el estado de generación sostiene certeza tranquila;
4. la exportación se percibe como salida simple y confiable.

## Dependencias abiertas
 - Endpoints `GET /api/v1/export/patient-summary` y `GET /api/v1/export/patient-summary/csv` existen en runtime — verificados via mi-lsp (2026-04-12);
 - El formato CSV usa GET con query params — no requiere body ni procesamiento async;
- El formato exacto del CSV debe definirse en coordinación con T05;
- `T04` debe prover shell paciente, sesión, cliente API.

---

**Estado:** `UI-RFC` activo para `EXP-001`.
**Siguiente capa gobernada:** `HANDOFF-SPEC-EXP-001.md`.
