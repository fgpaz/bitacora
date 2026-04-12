# UI-RFC-VIN-003 — Revocación de vínculo por paciente

## Propósito

Este documento traduce `VIN-003` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS`. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete `VIN-003`.

## Estado del gate

- `slice`: `VIN-003`
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
- `../../03_FL/FL-VIN-03.md`
- `../../04_RF/RF-VIN-020.md`
- `../../04_RF/RF-VIN-021.md`
- `../../04_RF/RF-VIN-022.md`
- `../UXR/UXR-VIN-003.md`
- `../UXI/UXI-VIN-003.md`
- `../UJ/UJ-VIN-003.md`
- `../VOICE/VOICE-VIN-003.md`
- `../UXS/UXS-VIN-003.md`
- `../PROTOTYPE/PROTOTYPE-VIN-003.md`
- `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md`

## Slice cubierto

### In

- entrada con contexto del vínculo actual;
- confirmación de revocación con impacto visible;
- resultado con acceso cortado.

### Out

- flujos de invitación profesional;
- gestión de acceso;
- revocación de consentimiento.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-ENTRY` | lectura breve del vínculo actual | llega al flujo | permite identificar vínculo |
| `S02-DEFAULT` | impacto visible y CTA disponible | entra a confirmación | permite decidir |
| `S02-SUBMITTING` | feedback corto durante revocación | click en CTA | evita doble submit |
| `S03-SUCCESS` | vínculo revocado y acceso cortado | `200` | cierra sin dramatismo |
| `S03-ALREADY` | vínculo ya revocado | `ALREADY_REVOKED` | muestra estado sin repetir |
| `S03-ERROR` | error recuperable con reintento | `ERROR` | permite reintentar |

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `PatientPageShell` | shell general del slice | loading, ready, error |
| `LinkContextCard` | contexto del vínculo actual | default |
| `ImpactBriefBlock` | explicación del impacto en 2-3 ideas | default |
| `RevokeActionBar` | acción primaria de revocación | default, submitting |
| `SuccessConfirmationBlock` | confirmación de acceso cortado | default |
| `InlineFeedback` | error o conflicto | error, confirm |

## Contratos backend que la UI debe consumir

### 1. Revocación de vínculo

- request:
  - `DELETE /api/v1/vinculos/{careLinkId:guid}`
  - header: `Authorization: Bearer <access_token>`
  - body: `{ "confirmed": true }`
- respuesta esperada:
  - `outcome` (`"revoked_by_patient"`)
  - `revokedAtUtc`
- errores esperados:
  - `INVALID_BODY` — confirmed no es true
  - `403 Forbidden` — el paciente no es dueño del vínculo
  - `404 Not Found` — vínculo no existe
  - `422 Unprocessable Entity` — el vínculo no puede ser revocado (estado no permite)
  - `409 Conflict` — el vínculo ya fue revocado

### 2. Lectura de vínculos del paciente

- request:
  - `GET /api/v1/vinculos`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada:
  - `links[]` con `careLinkId`, `professionalName`, `status`, `canViewData`, `createdAtUtc`

**Nota:** Endpoints verificados via mi-lsp (2026-04-12). Corresponden a `RevokeCareLinkCommand` y `GetCareLinksByPatientQuery`.

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| llega al flujo | `GET /api/v1/vinculos` | vínculos existentes | muestra vínculos |
| click `Revocar vínculo` | `DELETE /api/v1/vinculos/{id}` | `200` | `S03-SUCCESS` |
| vínculo ya revocado | `DELETE /api/v1/vinculos/{id}` | `409 Conflict` | `S03-ALREADY` |
| vínculo no editable | `DELETE /api/v1/vinculos/{id}` | `422 Unprocessable Entity` | `S03-ALREADY` |
| error de revocación | `DELETE /api/v1/vinculos/{id}` | otro error | `S03-ERROR` |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `ALREADY_REVOKED` | muestra estado actual sin drama | no repetir la acción |
| `LINK_INACTIVE` |说明 vínculo no admite cambios | no tecnificar |
| `ERROR` | error recuperable con reintento | no dejar sin salida |

## Reglas de jerarquía y copy

- la acción secundaria `Conservar vínculo` no compite visualmente con la primaria;
- el impacto se enumera en 2-3 ideas concretas;
- el resultado confirma acceso cortado sin dramatizar;
- copy no culpabiliza ni usa lenguaje legalista.

## Responsive y accesibilidad

- mobile-first;
- la asimetría entre acciones debe ser clara en mobile;
- `prefers-reduced-motion` respetado.

## Criterio de implementación

1. el impacto se entiende antes de confirmar;
2. la acción secundaria no compite visualmente con la primaria;
3. el resultado confirma acceso cortado sin volver el paso dramático;
4. mobile y desktop conservan la misma lógica.
## Dependencias abiertas

- Endpoints `DELETE /api/v1/vinculos/{id}` y `GET /api/v1/vinculos` existen en runtime — verificados via mi-lsp (2026-04-12);
- `T04` debe prover shell paciente, sesion, cliente API.

---

**Estado:** `UI-RFC` activo para `VIN-003`.
**Siguiente capa gobernada:** `HANDOFF-SPEC-VIN-003.md`.
