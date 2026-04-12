# UI-RFC-VIN-004 — Gestión de acceso profesional por paciente

## Propósito

Este documento traduce `VIN-004` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS`. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete `VIN-004`.

## Estado del gate

- `slice`: `VIN-004`
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
- `../../06_pruebas/TP-VIN.md`
- `../../03_FL/FL-VIN-04.md`
- `../../04_RF/RF-VIN-022.md`
- `../../04_RF/RF-VIN-023.md`
- `../UXR/UXR-VIN-004.md`
- `../UXI/UXI-VIN-004.md`
- `../UJ/UJ-VIN-004.md`
- `../VOICE/VOICE-VIN-004.md`
- `../UXS/UXS-VIN-004.md`
- `../PROTOTYPE/PROTOTYPE-VIN-004.md`
- `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md`

## Slice cubierto

### In

- lectura del estado actual de acceso;
- decisión y guardado del cambio;
- confirmación del nuevo estado.

### Out

- flujos de invitación profesional;
- revocación de vínculo;
- variante Telegram.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-DEFAULT` | estado actual legible | llega al gestión | permite evaluar |
| `S01-INACTIVE` | vínculo inactivo que no admite cambios | vínculo inactivo | muestra estado sin control |
| `S02-DIRTY` | nuevo estado seleccionado y efecto explícito | cambia toggle | pide confirmar si aplica |
| `S02-SAVING` | feedback corto durante guardado | click en CTA | evita inconsistencias |
| `S03-SAVED` | nuevo estado guardado con efecto visible | `200` | deja claro el efecto |
| `S03-ERROR` | error recuperable con reintento | `ERROR` | permite volver a intentar |

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `PatientPageShell` | shell general del slice | loading, ready, error |
| `AccessStateCard` | estado actual legible con profesional | default, inactive |
| `AccessToggle` | control de cambio de acceso | off, on, saving, error |
| `EffectReminderBlock` | explicación del efecto del cambio | default |
| `SaveActionBar` | acción de guardado | default, disabled, saving |
| `InlineFeedback` | error localizado | error, confirm |

## Contratos backend que la UI debe consumir

### 1. Actualización de acceso

- request:
  - `PATCH /api/v1/carelinks/<careLinkId>/access`
  - header: `Authorization: Bearer <access_token>`
  - body: `{ "canViewData": boolean }`
- respuesta esperada (contrato futuro):
  - `careLinkId`
  - `canViewData` (nuevo valor)
  - `updatedAtUtc`
- errores ожидаемые:
  - `LINK_INACTIVE`
  - `ERROR`

### 2. Lectura de vínculos

- request:
  - `GET /api/v1/carelinks`
- respuesta esperada (contrato futuro):
  - lista de vínculos con `canViewData` y `status`

**Nota:** CareLink endpoint es futuro. Contrato marcado como especulativo.

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| llega al gestión | `GET /api/v1/carelinks` | vínculos activos | muestra estado actual |
| cambio de toggle | actualización local | nuevo estado | habilita guardado |
| click `Guardar cambio` | `PATCH /api/v1/carelinks/<id>/access` | `200` | `S03-SAVED` |
| vínculo inactivo | `PATCH` | `LINK_INACTIVE` | muestra inactivo |
| error de guardado | `PATCH` | `ERROR` | `S03-ERROR` |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `LINK_INACTIVE` | estado inactivo visible | no mostrar toggle activo |
| `ERROR` | error localized con reintento | conserva estado previo visible |
| `NETWORK_ERROR` | feedback de error de conexión | propone reintentar |

## Reglas de jerarquía y copy

- el estado actual se entiende antes de tocar cualquier control;
- la consecuencia del cambio queda explícita antes del guardado;
- la interfaz no usa lenguaje técnico interno como `can_view_data`;
- el guardado puede ser explícito o auto-save según el patrón que defina T04.

## Responsive y accesibilidad

- mobile-first;
- toggle accesible por teclado con label claro;
- `prefers-reduced-motion` respetado.

## Criterio de implementación

1. el estado actual se entiende antes de interactuar;
2. la consecuencia del cambio se lee antes del guardado;
3. la interfaz no usa lenguaje técnico interno;
4. mobile y desktop conservan la misma lógica de control explícito.

## Dependencias abiertas

- CareLink endpoint de acceso es futuro — contrato especulativo;
- `T04` debe definir el patrón de guardado (explícito o auto-save);
- `T04` debe prover shell paciente, sesión, cliente API.

---

**Estado:** `UI-RFC` activo para `VIN-004`.
**Siguiente capa gobernada:** `HANDOFF-SPEC-VIN-004.md`.
