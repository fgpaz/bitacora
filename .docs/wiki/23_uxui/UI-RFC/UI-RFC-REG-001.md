# UI-RFC-REG-001 — Registro rápido de humor vía web

## Propósito

Este documento traduce `REG-001` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS-REG-001.md` ni autoriza extender el slice hacia factores diarios o Telegram. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete de registro rápido que T05 puede consumir sin reinterpretación fuerte.

## Estado del gate

- `slice`: `REG-001`
- `estado`: `abierto para UI-RFC + HANDOFF`
- `fundamento`: wave-prod gap map 2026-04-10, basado en seed docs existentes UXR/UXI/UJ/VOICE/UXS/PROTOTYPE
- `límite`: esta apertura no equivale a validación UX real — la evidencia se captura en la fase post-runtime del portfolio
- `deuda explícita`: la validación UX real sigue pendiente en `../UX-VALIDATION/UX-VALIDATION-REG-001.md`

## Referencias obligatorias

- `../../16_patrones_ui.md`
- `../../21_matriz_validacion_ux.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../07_baseline_tecnica.md`
- `../../09_contratos_tecnicos.md`
- `../../06_matriz_pruebas_RF.md`
- `../../06_pruebas/TP-REG.md`
- `../../03_FL/FL-REG-01.md`
- `../../04_RF/RF-REG-001.md`
- `../../04_RF/RF-REG-002.md`
- `../../04_RF/RF-REG-003.md`
- `../../04_RF/RF-REG-004.md`
- `../../04_RF/RF-REG-005.md`
- `../UXR/UXR-REG-001.md`
- `../UXI/UXI-REG-001.md`
- `../UJ/UJ-REG-001.md`
- `../VOICE/VOICE-REG-001.md`
- `../UXS/UXS-REG-001.md`
- `../PROTOTYPE/PROTOTYPE-REG-001.md`

## Slice cubierto

### In

- entrada contextualizada al registro rápido de humor;
- escala visible de -3 a +3;
- gesto directo de selección y guardado;
- confirmación breve y factual;
- reintento digno ante error recuperable;
- redirrección clara ante consentimiento faltante.

### Out

- formulario de factores diarios (REG-002);
- historial o visualización de registros (VIN-*, VIS-*);
- registro vía Telegram (TG-001, TG-002);
- exportación de datos (EXP-001).

## Resultado que la UI debe producir

La UI debe sentirse como `captura silenciosa`: un gesto breve que no interrumpe el día y deja certeza de guardado.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-ENTRY` | entrada contextualizada al registro | llegada desde home, timeline o CTA cercana | escala visible de inmediato |
| `S02-DEFAULT` | escala lista para interacción | estado base al entrar | permite tocar un valor |
| `S02-SUBMITTING` | feedback de guardado en curso | valor seleccionado | evita doble envío |
| `S02-SUCCESS` | confirmación factual breve | guardado exitoso | permite seguir sin pantalla extra |
| `S02-ERROR` | mensaje corto de error recuperable | error de red o de servidor | permite reintentar sin perder el valor elegido |
| `S02-CONSENT` | redirrección a consentimiento | `CONSENT_REQUIRED` del backend | vínculo directo al panel de consentimiento |
| `S02-SESSION` | reingreso digno tras sesión expirada | `401` del backend | pide reingreso sin perder el contexto de registro |

## Regiones de composición

### `S01-ENTRY`

- `PatientPageShell`
- `MoodEntryContextHeader` (mínimo)
- `QuickScaleBlock`
- `InlineFeedback`

### `S02-*`

- `PatientPageShell`
- `MoodScale` (escala -3..+3, 7 valores)
- `MoodEntrySubmitButton`
- `InlineFeedback`
- `ConsentRedirectLink` (solo en `S02-CONSENT`)
- `SessionRecoverPrompt` (solo en `S02-SESSION`)

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `PatientPageShell` | shell general del slice autenticado | loading, ready, error |
| `MoodScale` | escala de 7 valores -3..+3 | default, selected, disabled |
| `MoodEntrySubmitButton` | acción de registrar el valor | idle, loading, disabled |
| `InlineFeedback` | error localized o confirmación factual | info, error, confirm |
| `ConsentRedirectLink` | vínculo al panel de consentimiento | default |
| `SessionRecoverPrompt` | recuperación de sesión | default |

Los nombres pueden refinarse en código, pero la separación de responsabilidades no debe colapsarse.

## Contratos backend que la UI debe consumir

### 1. Creación de MoodEntry

- precondición: sesión JWT válida y consentimiento activo
- request:
  - `POST /api/v1/mood-entries`
  - header: `Authorization: Bearer <access_token>`
  - body: `{ "score": integer }`
- respuesta esperada:
  - `201 Created` con `{ "mood_entry_id": uuid, "safe_projection": { "mood_score": integer, "channel": "api", "created_at": timestamp } }`
  - `200 OK` si ya existe entrada idempotente (misma ventana de 1 min, mismo score)

### 2. Errores tipados del backend

| Código | HTTP | Significado | UI esperada |
| --- | --- | --- | --- |
| `CONSENT_REQUIRED` | 403 | Sin consentimiento activo | redirrección a panel de consentimiento |
| `INVALID_SCORE` | 422 | Score fuera de rango -3..+3 | mensaje breve de error,scale sigue disponible |
| `ENCRYPTION_FAILURE` | 500 | Fallo de KMS | error recuperable, no perder el dato |
| `DUPLICATE_ENTRY` | 200 | Idempotencia activada | tratar como success |

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| tocar un valor de la escala | UI local (sin request) | interacción inmediata | valor seleccionado visualmente |
| confirmar selección | `POST /api/v1/mood-entries` | `201` | `S02-SUCCESS` con confirmación factual |
| confirmar selección | `POST /api/v1/mood-entries` | `200` | `S02-SUCCESS` (idempotencia — es success igualmente) |
| confirmar selección | `POST /api/v1/mood-entries` | `403 CONSENT_REQUIRED` | `S02-CONSENT` |
| confirmar selección | `POST /api/v1/mood-entries` | `422 INVALID_SCORE` | `S02-ERROR` |
| confirmar selección | `POST /api/v1/mood-entries` | `401` | `S02-SESSION` |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `CONSENT_REQUIRED` | redirrección a consentimiento sin cargar nada | no mostrar pantalla de error de registro |
| `INVALID_SCORE` | mensaje localizado en el área de feedback | no borrar el valor ya seleccionado |
| `ENCRYPTION_FAILURE` | error breve y opción de reintento | no perder el valor ya seleccionado |
| `401` | prompt de reingreso que preserve el valor ya elegido | no exigir que la persona reingrese el dato |

## Reglas de jerarquía y copy

- una sola acción dominante visible por estado;
- la escala ocupa el centro del espacio sin columnas;
- la confirmación es factual y no celebratoria;
- copy de apoyo mínimo: solo lo que destraba duda real;
- `¿Cómo te sentís ahora?` es el titular aprobado en `UXS-REG-001.md` — no agregar interpretación.

## Responsive y accesibilidad

- `mobile-first` real para todo el slice;
- la escala debe ser tocable con un dedo sin zoom;
- los valores de la escala deben tener labels accesibles (`-3`, `-2`, `-1`, `0`, `+1`, `+2`, `+3`);
- `prefers-reduced-motion` respetado — sin animaciones que retrasen la sensación de gesto instantáneo;
- contraste mínimo en la escala y en el CTA de guardado;
- foco visible en el valor seleccionado y en el botón de guardado.

## Validación diferida

Este contrato UI-RFC no sustituye la validación UX real. Antes de marcar el slice como `validated`:

- [ ] capturar evidencia de `S02` con personas reales;
- [ ] verificar que la escala no se interpreta como evaluación;
- [ ] verificar que la confirmación no compite con la acción siguiente;
- [ ] verificar que el gesto se siente instantáneo en mobile y desktop.

Estas evidencias se documentarán en `../UX-VALIDATION/UX-VALIDATION-REG-001.md` cuando existan runtime web funcional y evidencia de usuarios reales.

## Dependencias abiertas

- `REG-002` (factores diarios) comparte el shell pero no debe contaminar este contrato;
- `T05` implementa el flujo paciente y consume este contrato sin reinterpretar estados;
- cuando exista implementación funcional, `UX-VALIDATION-REG-001.md` podrá reabrir este contrato si la evidencia contradice el slice.

---

**Estado:** `UI-RFC` activo para `REG-001` bajo wave-prod gap map 2026-04-10.
**Siguiente capa gobernada:** `HANDOFF-SPEC-REG-001.md`, `HANDOFF-ASSETS-REG-001.md`, `HANDOFF-MAPPING-REG-001.md`, `HANDOFF-VISUAL-QA-REG-001.md`.
