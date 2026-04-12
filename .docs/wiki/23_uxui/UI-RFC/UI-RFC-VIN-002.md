# UI-RFC-VIN-002 — Auto-vinculación paciente a profesional por código

## Propósito

Este documento traduce `VIN-002` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS` ni autoriza extender el slice hacia flujos de revocación. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete `VIN-002`.

## Estado del gate

- `slice`: `VIN-002`
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
- `../../03_FL/FL-VIN-02.md`
- `../../04_RF/RF-VIN-004.md`
- `../../04_RF/RF-VIN-010.md`
- `../../04_RF/RF-VIN-011.md`
- `../../04_RF/RF-VIN-012.md`
- `../UXR/UXR-VIN-002.md`
- `../UXI/UXI-VIN-002.md`
- `../UJ/UJ-VIN-002.md`
- `../VOICE/VOICE-VIN-002.md`
- `../UXS/UXS-VIN-002.md`
- `../PROTOTYPE/PROTOTYPE-VIN-002.md`
- `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md`

## Slice cubierto

### In

- entrada al flujo con contexto breve;
- ingreso o pegado del código;
- validación y creación del vínculo;
- confirmación con acceso todavía bajo control del paciente.

### Out

- flujos de invitación profesional;
- gestión de acceso profesional;
- variante Telegram.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-DEFAULT` | entrada con contexto breve y código pendiente | llega al flujo | permite leer contexto |
| `S02-DEFAULT` | campo vacío con efecto del vínculo visible | entra a ingreso de código | permite ingresar código |
| `S02-READY` | código listo para enviar | código pasa validación de formato | habilita `Vincular` |
| `S02-SUBMITTING` | feedback breve durante validación | click en CTA | evita doble envío |
| `S02-INVALID` | código inválido con recuperación digna | `INVALID_CODE` | permite corregir o abandonar |
| `S02-EXPIRED` | código expirado con salida clara | `CODE_EXPIRED` | invita a pedir nuevo |
| `S03-SUCCESS` | vínculo activo con acceso aún desactivado | `201` | muestra vínculo activo |
| `S03-EXISTING` | vínculo ya existente sin duplicación | `ALREADY_LINKED` | explica estado actual |

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `PatientPageShell` | shell general del slice paciente | loading, ready, error |
| `CodeInput` | campo para código de vinculación | empty, valid, invalid |
| `ContextBriefBlock` | contexto breve antes del código | default |
| `LinkEffectReminder` | recordatorio de que vínculo no es acceso | default |
| `InlineFeedback` | error localized or confirmation | info, error, confirm |
| `ActiveLinkCard` | confirmación de vínculo activo | default |

## Contratos backend que la UI debe consumir

### 1. Aceptación de vínculo por código (paciente)

- request:
  - `POST /api/v1/vinculos/accept`
  - header: `Authorization: Bearer <access_token>`
  - body: `{ "bindingCode": string }`
- respuesta esperada:
  - `careLinkId`
  - `status` (`active`)
  - `professionalName`
  - `canViewData` (`false` — acceso desactivado por defecto)
- errores esperados:
  - `INVALID_BODY` — bindingCode faltante
  - `INVALID_CODE` — código no válido o no encontrado
  - `CODE_EXPIRED` — código expirado (410 Gone)
  - `ALREADY_LINKED` — vínculo ya existente (409 Conflict)
  - `NOT_FOUND` — vínculo no existe (404 Not Found)

### 2. Lectura de vínculos del paciente

- request:
  - `GET /api/v1/vinculos`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada:
  - `links[]` con `careLinkId`, `professionalName`, `status`, `canViewData`, `createdAtUtc`

### 3. Vínculos activos con permiso de vista

- request:
  - `GET /api/v1/vinculos/active`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada:
  - `links[]` con `careLinkId`, `professionalName`, `canViewData`

**Nota:** Endpoints verificados via mi-lsp (2026-04-12). El endpoint `POST /api/v1/vinculos/accept` corresponde a `AcceptCareLinkCommand`.

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| ingreso de código | validación local | formato ok | habilita CTA |
| click `Vincular` | `POST /api/v1/vinculos/accept` | `201` | `S03-SUCCESS` (vínculo activo, acceso desactivado) |
| código inválido | `POST /api/v1/vinculos/accept` | `INVALID_CODE` / `404` | `S02-INVALID` |
| código expirado | `POST /api/v1/vinculos/accept` | `CODE_EXPIRED` / `410` | `S02-EXPIRED` |
| vínculo ya existente | `POST /api/v1/vinculos/accept` | `ALREADY_LINKED` / `409` | `S03-EXISTING` |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `INVALID_CODE` | inline error breve | no tecnificar — decir "no está disponible" |
| `CODE_EXPIRED` | mensaje con sugerencia de pedir nuevo | no culpar ni dramatizar |
| `ALREADY_LINKED` | explica el estado actual | no duplicar la acción de vincular |
| `SERVICE_UNAVAILABLE` | error recuperable con reintento | no dejar sin salida |

## Reglas de jerarquía y copy

- una sola acción dominante;
- contexto breve antes del campo de código;
- después del vínculo exitoso, queda claro que acceso sigue desactivado;
- errores distinguen código inválido de expirado;
- copy no tecnifica el proceso.

## Responsive y accesibilidad

- mobile-first;
- campo de código con teclado apropiado;
- `prefers-reduced-motion` respetado.

## Criterio de implementación

1. el campo de código domina sin ruido accesorio;
2. el límite de acceso se entiende antes y después del envío;
3. errores son breves y dignos;
4. mobile y desktop conservan la misma lógica.

## Dependencias abiertas

 - Endpoints `POST /api/v1/vinculos/accept`, `GET /api/v1/vinculos`, `GET /api/v1/vinculos/active` existen en runtime — verificados via mi-lsp (2026-04-12);
 - `T04` debe prover shell paciente, sesion, cliente API y routing.

---

**Estado:** `UI-RFC` activo para `VIN-002`.
**Siguiente capa gobernada:** `HANDOFF-SPEC-VIN-002.md`.
