# UI-RFC-VIS-002 — Dashboard multi-paciente del profesional

## Propósito

Este documento traduce `VIS-002` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS`. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete `VIS-002`.

## Estado del gate

- `slice`: `VIS-002`
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
- `../../06_pruebas/TP-SEC.md`
- `../../03_FL/FL-VIS-02.md`
- `../../04_RF/RF-VIS-010.md`
- `../../04_RF/RF-VIS-011.md`
- `../../04_RF/RF-VIS-012.md`
- `../../04_RF/RF-VIS-013.md`
- `../../04_RF/RF-VIS-014.md`
- `../UXR/UXR-VIS-002.md`
- `../UXI/UXI-VIS-002.md`
- `../UJ/UJ-VIS-002.md`
- `../VOICE/VOICE-VIS-002.md`
- `../UXS/UXS-VIS-002.md`
- `../PROTOTYPE/PROTOTYPE-VIS-002.md`
- `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md`

## Slice cubierto

### In

- carga inicial del dashboard;
- lista paginada de pacientes visibles;
- resumen y alertas básicas por tarjeta;
- cambio de página.

### Out

- detalle de paciente individual;
- flujos de vinculación;
- variante Telegram.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-LOADING` | skeleton o placeholder sobrio | primera carga | anticipa contenido sin ruido |
| `S01-READY` | lista de pacientes visible y priorizable | datos cargados | permite priorizar lectura |
| `S01-PAGINATION` | cambio de página sin ruido | interacción de paginación | mantiene jerarquía |
| `S02-EMPTY` | ausencia de pacientes visibles | sin pacientes con acceso | vacío claro |
| `S03-ERROR` | error recuperable de carga | error de fetch | permite reintentar |

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `ProfessionalPageShell` | shell general del slice profesional | loading, ready, error |
| `PatientList` | lista paginada de pacientes | loading, ready, empty, error |
| `PatientCard` | tarjeta de resumen por paciente | default, alert |
| `PaginationControls` | controles de paginación | default, disabled |
| `DashboardSkeleton` | placeholder editorial durante carga | default |
| `InlineFeedback` | error localized | error, retry |

## Contratos backend que la UI debe consumir

### 1. Lectura de pacientes visibles

- request:
  - `GET /api/v1/carelinks/patients?page=<n>&limit=<n>`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada (contrato futuro — no existe runtime CareLink):
  - `patients[]` con `patientRef`, `displayName`, `lastEntryAt`, `hasRecentAlert`
  - `total`
  - `page`
  - `hasMore`
- errores ожидаемые:
  - `NO_PATIENTS` — sin pacientes visibles
  - `SERVICE_UNAVAILABLE`

### 2. Detalle de paciente

- request:
  - `GET /api/v1/carelinks/patients/<patientRef>/summary`
- respuesta esperada (contrato futuro):
  - resumen con último registro y alertas

**Nota:** CareLink endpoints son futuros. Contratos marcados como especulativos.

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| abre dashboard | `GET /patients` | con datos | `S01-READY` |
| abre dashboard | `GET /patients` | sin pacientes | `S02-EMPTY` |
| cambio de página | `GET /patients?page=2` | con datos | `S01-PAGINATION` |
| fallo de carga | `GET /patients` | error | `S03-ERROR` |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `NO_PATIENTS` | vacío claro y no error | no alarmar |
| `SERVICE_UNAVAILABLE` | error breve con reintento | no dramatizar |
| `NETWORK_ERROR` | feedback de error | proponer reintentar |

## Reglas de jerarquía y copy

- solo se muestran pacientes con acceso activo;
- las alertas no dramatizan ni reemplazan la lectura clínica;
- la lista mantiene jerarquía aun con varias tarjetas;
- la paginación es contenida y no rompe la jerarquía.

## Responsive y accesibilidad

- desktop-first para este slice;
- la lista se adapta a mobile con scroll vertical;
- `prefers-reduced-motion` respetado;
- contraste y labels accesibles.

## Criterio de implementación

1. el dashboard se mantiene legible aun con varias tarjetas;
2. el límite de visibilidad se lee sin explicación oral;
3. la paginación no rompe la jerarquía de lectura;
4. el vacío no se confunde con error.

## Dependencias abiertas

- CareLink endpoints de pacientes son futuros — contratos especulativos;
- `T04` debe prover shell profesional, sesión, cliente API y routing.

---

**Estado:** `UI-RFC` activo para `VIS-002`.
**Siguiente capa gobernada:** `HANDOFF-SPEC-VIS-002.md`.
