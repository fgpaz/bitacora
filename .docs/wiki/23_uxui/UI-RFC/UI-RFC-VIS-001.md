# UI-RFC-VIS-001 — Timeline longitudinal del paciente

## Propósito

Este documento traduce `VIS-001` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS`. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete `VIS-001`.

## Estado del gate

- `slice`: `VIS-001`
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
- `../../06_pruebas/TP-VIS.md`
- `../../03_FL/FL-VIS-01.md`
- `../../04_RF/RF-VIS-001.md`
- `../../04_RF/RF-VIS-002.md`
- `../../04_RF/RF-VIS-003.md`
- `../UXR/UXR-VIS-001.md`
- `../UXI/UXI-VIS-001.md`
- `../UJ/UJ-VIS-001.md`
- `../VOICE/VOICE-VIS-001.md`
- `../UXS/UXS-VIS-001.md`
- `../PROTOTYPE/PROTOTYPE-VIS-001.md`
- `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md`

## Slice cubierto

### In

- apertura del timeline con período base;
- lectura inicial del gráfico;
- ajuste de período.

### Out

- exportación CSV;
- detalle de registro individual;
- variante Telegram.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-LOADING` | skeleton o placeholder editorial | primera carga | anticipa el gráfico sin ruido |
| `S01-READY` | timeline base con período actual visible | datos cargados | permite leer |
| `S02-PERIOD` | ajuste de período con chart aún dominante | cambio de filtro | permite cambiar período |
| `S03-EMPTY` | vacío útil y distinguible de error | sin datos en período | orienta sin confundir |
| `S03-ERROR` | error breve de carga con reintento | error de fetch | permite reintentar |

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `PatientPageShell` | shell general del slice | loading, ready, error |
| `TimelineChart` | gráfico longitudinal suavizado | loading, ready, empty, error |
| `PeriodFilter` | selector de período simple | default, active |
| `ChartSkeleton` | placeholder editorial durante carga | default |
| `EmptyStateBlock` | orientación cuando no hay datos | default |
| `InlineFeedback` | error localized | error, retry |

## Contratos backend que la UI debe consumir

### 1. Lectura de timeline del paciente

- request:
  - `GET /api/v1/visualizacion/timeline?from=<DateOnly>&to=<DateOnly>`
  - header: `Authorization: Bearer <access_token>`
  - query params: `from` (DateOnly, inclusive), `to` (DateOnly, inclusive)
- respuesta esperada:
  - `entries[]` con `entryId`, `recordDate`, `moodLevel`, `factorFlags`, `createdAtUtc`
  - `hasMore` (bool)
- errores esperados:
  - `INVALID_DATE_RANGE` — `to < from` (400 Bad Request)
  - `SERVICE_UNAVAILABLE` — error recuperable

### 2. Resumen del período

- request:
  - `GET /api/v1/visualizacion/summary?from=<DateOnly>&to=<DateOnly>`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada:
  - `averageMood`, `entryCount`, `dominantFactors[]`, `streakDays`

### 3. Cambio de período

- mismo endpoint con query params diferentes

**Nota:** Endpoints verificados via mi-lsp (2026-04-12). Corresponden a `GetPatientTimelineQuery` y `GetPatientSummaryQuery`.

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| abre timeline | `GET /visualizacion/timeline?from=&to=` | con datos | `S01-READY` |
| abre timeline | `GET /visualizacion/timeline?from=&to=` | sin datos | `S03-EMPTY` |
| cambio de período | `GET /visualizacion/timeline?from=&to=` | con datos | `S02-PERIOD` (chart actualizado) |
| fallo de carga | `GET /visualizacion/timeline` | error | `S03-ERROR` |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `NO_ENTRIES` | estado vacío útil | no confundir con error |
| `SERVICE_UNAVAILABLE` | error breve con reintento | no dramatizar |
| `NETWORK_ERROR` | feedback de error de conexión | proponer reintentar |

## Reglas de jerarquía y copy

- el chart principal aparece antes que controles accesorios;
- el filtro de período no compite con la lectura inicial;
- el estado vacío se distingue del error sin explicación adicional;
- la exportación aparece disponible pero no como CTA dominante.

## Responsive y accesibilidad

- mobile-first;
- el chart es legible en pantallas pequeñas;
- `prefers-reduced-motion` respetado — alternativa sin animación si el navegador lo pide;
- contraste accesible en labels del chart.

## Criterio de implementación

1. el chart principal aparece antes que controles accesorios;
2. el filtro de período mantiene una sola dirección dominante;
3. el estado vacío se distingue del error con claridad;
4. mobile y desktop conservan la misma lógica de lectura.

## Dependencias abiertas

 - Endpoints `GET /api/v1/visualizacion/timeline` y `GET /api/v1/visualizacion/summary` existen en runtime — verificados via mi-lsp (2026-04-12);
 - La libreria del chart puede variar segun decision tecnica de T04;
 - `T04` debe prover shell paciente, sesion, cliente API.

---

**Estado:** `UI-RFC` activo para `VIS-001`.
**Siguiente capa gobernada:** `HANDOFF-SPEC-VIS-001.md`.
