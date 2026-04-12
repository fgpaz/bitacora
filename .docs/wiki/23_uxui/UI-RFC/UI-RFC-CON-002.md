# UI-RFC-CON-002 — Revocación de consentimiento

## Propósito

Este documento traduce `CON-002` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS`. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete `CON-002`.

## Estado del gate

- `slice`: `CON-002`
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
- `../../06_pruebas/TP-CON.md`
- `../../03_FL/FL-CON-02.md`
- `../../04_RF/RF-CON-010.md`
- `../../04_RF/RF-CON-011.md`
- `../../04_RF/RF-CON-012.md`
- `../../04_RF/RF-CON-013.md`
- `../UXR/UXR-CON-002.md`
- `../UXI/UXI-CON-002.md`
- `../UJ/UJ-CON-002.md`
- `../VOICE/VOICE-CON-002.md`
- `../UXS/UXS-CON-002.md`
- `../PROTOTYPE/PROTOTYPE-CON-002.md`
- `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md`

## Slice cubierto

### In

- entrada al consentimiento vigente;
- revisión del impacto antes de revocar;
- confirmación de revocación con cascada clara.

### Out

- flujo de nuevo consentimiento;
- vinculación;
- variante Telegram.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-ENTRY` | consentimiento vigente antes de revisar impacto | llega al ajuste | permite identificar estado |
| `S02-DEFAULT` | impacto visible y CTA disponible | entra a revisión | permite decidir |
| `S02-SUBMITTING` | feedback corto durante revocación | click en CTA | evita doble submit |
| `S03-SUCCESS` | consentimiento revocado con cascada entendible | `200` | cierra sin dramatismo |
| `S03-ALREADY` | consentimiento ya revocado | `ALREADY_REVOKED` | muestra estado sin repetir |
| `S03-ERROR` | error recuperable con reintento | `ERROR` | permite reintentar |

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `PatientPageShell` | shell general del slice | loading, ready, error |
| `ConsentCurrentBlock` | estado actual del consentimiento | default |
| `ImpactCascadeBlock` | enumeración del impacto en 2-3 ideas | default |
| `RevokeConsentBar` | acción primaria de revocación | default, submitting |
| `SecondaryActionRow` | acción de conservar | default |
| `SuccessCascadeBlock` | confirmación de suspensión de registro y accesos | default |
| `InlineFeedback` | error localized | error, confirm |

## Contratos backend que la UI debe consumir

### 1. Revocación de consentimiento

- request:
  - `DELETE /api/v1/consent/current`
  - header: `Authorization: Bearer <access_token>`
  - body: `{ "confirmed": true }`
- respuesta esperada:
  - `outcome` (`"revoked_by_patient"`)
  - `revokedAtUtc`
  - `affectedCareLinks[]` — CareLinks afectados por la revocación
- errores esperados:
  - `INVALID_BODY` — confirmed no es true
  - `NO_ACTIVE_CONSENT` — no hay consentimiento activo para revocar
  - `CASCADE_ERROR` — error en la transacción en cascada con vínculos
  - `ERROR` — error recuperable

### 2. Lectura de estado de consentimiento

- request:
  - `GET /api/v1/consent/current`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada:
  - `consentGrantId`
  - `version`
  - `text` (full consent text)
  - `sections[]` (consent sections)
  - `status` (`none`, `active`, `revoked`)
  - `grantedAtUtc`

**Nota:** Endpoints verificados via mi-lsp (2026-04-12). Corresponden a `RevokeConsentCommand` y `GetCurrentConsentQuery`.

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| llega al ajuste | `GET /consent/current` | consentimiento vigente | muestra estado actual |
| click `Revocar consentimiento` | `DELETE /consent/current` | `200` | `S03-SUCCESS` (suspension de registro y accesos) |
| no hay consentimiento activo | `DELETE /consent/current` | `NO_ACTIVE_CONSENT` | `S03-ALREADY` |
| error en cascada | `DELETE /consent/current` | `CASCADE_ERROR` | `S03-ERROR` |
| error recuperable | `DELETE /consent/current` | `ERROR` | `S03-ERROR` |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `ALREADY_REVOKED` | muestra estado actual sin repetir drama | no duplicar pantalla |
| `CASCADE_ERROR` | error en cerrado — no exponer estados parciales | rollback visible |
| `ERROR` | error recuperable con reintento | no dejar al usuario sin salida |
| `SESSION_EXPIRED` | reautenticación con continuidad | no perder contexto |

## Reglas de jerarquía y copy

- copy no es amenazante ni legalista;
- la cascada se enumera en 2-3 ideas concretas: suspensión de registro y pérdida de accesos profesionales;
- la acción secundaria `Conservar consentimiento` no compite visualmente con la primaria;
- el resultado final es factual y no celebratorio.

## Responsive y accesibilidad

- mobile-first;
- la asimetría entre acciones debe ser clara en mobile;
- la lista de impacto es escaneeable;
- `prefers-reduced-motion` respetado.

## Criterio de implementación

1. la cascada se entiende antes de confirmar y no recién después;
2. la acción secundaria conserva el consentimiento sin competir visualmente con la primaria;
3. el resultado final deja claro que se suspenden registro y accesos;
 - Endpoints de consentimiento existen en runtime — verificados via mi-lsp (2026-04-12);
 - El detalle de la cascada a vinculos debe coordinarse con T05;
## Dependencias abiertas

 - Ninguna — los endpoints existen en runtime (verificado mi-lsp 2026-04-12);
- El detalle de la cascada a vínculos debe coordinarse con T05 cuando existan los endpoints;
- `T04` debe prover shell paciente, sesión, cliente API.

---

**Estado:** `UI-RFC` activo para `CON-002`.
**Siguiente capa gobernada:** `HANDOFF-SPEC-CON-002.md`.
