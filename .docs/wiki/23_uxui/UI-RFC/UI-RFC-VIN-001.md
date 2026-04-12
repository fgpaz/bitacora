# UI-RFC-VIN-001 — Emisión de invitación profesional a paciente

## Propósito

Este documento traduce `VIN-001` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS` ni autoriza extender el slice hacia flujos de acceso. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete `VIN-001` que T04/T05 pueden consumir sin reinterpretación.

## Estado del gate

- `slice`: `VIN-001`
- `estado`: `abierto para UI-RFC + HANDOFF`
- `fundamento`: autoridad del plan T3 — `wave-prod 11-docs-uxui-canon`
- `límite`: este contrato no equivale a validación UX real; la auditoría visual sigue pendiente
- `deuda explícita`: la validación real sigue diferida a `Phase 60`

## Referencias obligatorias

- `../../16_patrones_ui.md`
- `../../21_matriz_validacion_ux.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../07_baseline_tecnica.md`
- `../../09_contratos_tecnicos.md`
- `../../06_matriz_pruebas_RF.md`
- `../../06_pruebas/TP-VIN.md`
- `../../03_FL/FL-VIN-01.md`
- `../../04_RF/RF-VIN-001.md`
- `../../04_RF/RF-VIN-002.md`
- `../../04_RF/RF-VIN-003.md`
- `../../04_RF/RF-VIN-004.md`
- `../UXR/UXR-VIN-001.md`
- `../UXI/UXI-VIN-001.md`
- `../UJ/UJ-VIN-001.md`
- `../VOICE/VOICE-VIN-001.md`
- `../UXS/UXS-VIN-001.md`
- `../PROTOTYPE/PROTOTYPE-VIN-001.md`
- `.docs/raw/decisiones/2026-04-10-wave-prod-uxui-gap-map.md`

## Slice cubierto

### In

- entrada al formulario de invitación profesional;
- carga y validación de email;
- revisión del alcance antes de emitir;
- emisión de vínculo pendiente;
- conflicto cuando ya existe invitación o vínculo.

### Out

- flujo de vinculación por código del paciente;
- gestión de acceso profesional;
- páginas profesionales;
- variante Telegram.

## Resultado que la UI debe producir

La UI debe sentirse como una acción breve y responsable que deja claro que la invitación crea un vínculo pendiente, no acceso automático a datos clínicos.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-DEFAULT` | entrada con campo de email y alcance visible | llega al formulario | permite cargar email |
| `S01-READY` | CTA habilitada cuando email es válido | email pasa validación | `Enviar invitación` |
| `S01-SUBMITTING` | feedback corto durante envío | click en CTA | evita doble envío |
| `S02-SUCCESS` | invitación pendiente emitida | `201` del backend | muestra estado pendiente |
| `S03-CONFLICT` | vínculo o invitación existente | `ALREADY_EXISTS` | explica el conflicto |

## Regiones de composición

### `S01-*`

- `ProfessionalPageShell`
- `InviteFormBlock`
- `EmailInput`
- `ScopeReminderInline`
- `PrimaryActionStack`
- `InlineFeedback`

### `S02-SUCCESS`

- `ProfessionalPageShell`
- `PendingStatusCard`
- `NextActionHint`

### `S03-CONFLICT`

- `ProfessionalPageShell`
- `ConflictResolutionCard`
- `SecondaryUtilityRow`

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `ProfessionalPageShell` | shell general del slice profesional | loading, ready, error |
| `InviteFormBlock` | formulario de invitación con email y alcance | default, ready, submitting |
| `EmailInput` | campo de email con validación | default, valid, invalid |
| `ScopeReminderInline` | recordatorio breve del alcance | default |
| `PendingStatusCard` | confirmación de estado pendiente | default |
| `ConflictResolutionCard` | explica vínculo o invitación existente | default |
| `InlineFeedback` | error localizado o confirmación factual | info, error, confirm |

## Contratos backend que la UI debe consumir

### 1. Emisión de invitación (profesional)

- request:
  - `POST /api/v1/professional/invites`
  - header: `Authorization: Bearer <access_token>`
  - body: `{ "emailHash": string }`
- respuesta esperada:
  - `pendingInviteId`
  - `expiresAtUtc`
- errores esperados:
  - `INVALID_BODY` — emailHash faltante o mal formado
  - `ALREADY_EXISTS` — vínculo o invitación existente
  - `SERVICE_UNAVAILABLE` — error recuperable

### 2. Lectura de vínculos del paciente

- request:
  - `GET /api/v1/vinculos`
  - header: `Authorization: Bearer <access_token>`
- respuesta esperada:
  - `links[]` con `careLinkId`, `professionalName`, `status`, `canViewData`

**Nota:** Los endpoints reales de CareLink existen en runtime (confirmado via mi-lsp). El endpoint `POST /api/v1/professional/invites` corresponde a `CreatePendingInviteCommand`. El endpoint `GET /api/v1/vinculos` corresponde a `GetCareLinksByPatientQuery`.

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| carga email válida | validación local | formato ok | habilita CTA |
| click `Enviar invitación` | `POST /api/v1/professional/invites` | `201` | `S02-SUCCESS` (invitación pendiente) |
| email ya vinculado | `POST /api/v1/professional/invites` | `ALREADY_EXISTS` | `S03-CONFLICT` |
| email inválido | validación local | formato incorrecto | inline error |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `INVALID_EMAIL` | inline error bajo el campo | no dramatizar ni mostrar jerga de backend |
| `ALREADY_EXISTS` | carta de conflicto con estado actual | no presentar como error genérico |
| `SERVICE_UNAVAILABLE` | mensaje de error recuperable con reintento | no dejar al usuario sin salida |
| `NETWORK_ERROR` | feedback de error de conexión | proponer reintentar |

## Reglas de jerarquía y copy

- una sola acción dominante visible por estado;
- el alcance del vínculo pendiente se explica en una línea;
- privacidad y control del paciente se mantienen presentes pero sin peso institucional;
- la confirmación de éxito no es celebratoria;
- el conflicto evita duplicar acciones innecesarias.

## Responsive y accesibilidad

- `mobile-first` real para todo el slice;
- el campo de email debe funcionar en mobile con teclado apropiado;
- `prefers-reduced-motion` respetado;
- contraste y labels accesibles para el campo de email.

## Criterio de implementación

La implementación de T04/T05 cumple este contrato si:

1. el formulario de invitación es accesible y funcional en mobile y desktop;
2. el estado pendiente queda claro después del envío;
3. el conflicto se resuelve sin volver a enviar;
4. ningún texto suena a alta administrativa de paciente;
5. no se asume runtime CareLink existente.

## Dependencias abiertas

- `T04` debe prover shell profesional, sesión, cliente API y routing base;
- endpoint `POST /api/v1/professional/invites` existe en runtime — verificado via mi-lsp (2026-04-12);
- cuando la evidencia de runtime contradiga el contrato, `UX-VALIDATION-VIN-001.md` podrá reabrir este documento.

---

**Estado:** `UI-RFC` activo para `VIN-001` bajo autoridad T3.
**Siguiente capa gobernada:** `HANDOFF-SPEC-VIN-001.md`.
