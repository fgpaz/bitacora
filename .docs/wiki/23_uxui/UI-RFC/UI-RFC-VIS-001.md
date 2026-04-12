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

### 1. Lectura de timeline

- request:
  - `GET /api/v1/mood-entries/timeline?period=<week|month|quarter>`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada (contrato futuro — no existe runtime):
  - `entries[]` con `date`, `mood`, `factors`
  - `period`
  - `hasMore`
- errores ожидаемые:
  - `NO_ENTRIES` — sin datos en el período
  - `SERVICE_UNAVAILABLE`

### 2. Cambio de período

- mismo endpoint con query param diferente

**Nota:** El endpoint de timeline es futuro. Contrato marcado como especulativo.

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| abre timeline | `GET /timeline?period=month` | con datos | `S01-READY` |
| abre timeline | `GET /timeline?period=month` | sin datos | `S03-EMPTY` |
| cambio de período | `GET /timeline?period=week` | con datos | `S02-PERIOD` (chart actualizado) |
| fallo de carga | `GET /timeline` | error | `S03-ERROR` |

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

- Endpoint de timeline es futuro — contrato especulativo;
- La librería del chart puede variar según decisión técnica de T04;
- `T04` debe prover shell paciente, sesión, cliente API.

---

**Estado:** `UI-RFC` activo para `VIS-001`.
**Siguiente capa gobernada:** `HANDOFF-SPEC-VIS-001.md`.
