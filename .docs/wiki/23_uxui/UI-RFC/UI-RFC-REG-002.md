# UI-RFC-REG-002 — Registro de factores diarios vía web

## Propósito

Este documento traduce `REG-002` a contrato técnico UI implementable.

No declara `UX-VALIDATION`, no reemplaza `UXS-REG-002.md` ni autoriza extender el slice hacia registro de humor o Telegram. Su función es congelar jerarquía, estados, componentes, contratos backend y reglas responsive del paquete de check-in diario que T05 puede consumir sin reinterpretación fuerte.

## Estado del gate

- `slice`: `REG-002`
- `estado`: `abierto para UI-RFC + HANDOFF`
- `fundamento`: wave-prod gap map 2026-04-10, basado en seed docs existentes UXR/UXI/UJ/VOICE/UXS/PROTOTYPE
- `límite`: esta apertura no equivale a validación UX real — la evidencia se captura en la fase post-runtime del portfolio
- `deuda explícita`: la validación UX real sigue pendiente en `../UX-VALIDATION/UX-VALIDATION-REG-002.md`

## Referencias obligatorias

- `../../16_patrones_ui.md`
- `../../21_matriz_validacion_ux.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../07_baseline_tecnica.md`
- `../../09_contratos_tecnicos.md`
- `../../06_matriz_pruebas_RF.md`
- `../../06_pruebas/TP-REG.md`
- `../../03_FL/FL-REG-03.md`
- `../../04_RF/RF-REG-020.md`
- `../../04_RF/RF-REG-021.md`
- `../../04_RF/RF-REG-022.md`
- `../../04_RF/RF-REG-023.md`
- `../../04_RF/RF-REG-024.md`
- `../../04_RF/RF-REG-025.md`
- `../UXR/UXR-REG-002.md`
- `../UXI/UXI-REG-002.md`
- `../UJ/UJ-REG-002.md`
- `../VOICE/VOICE-REG-002.md`
- `../UXS/UXS-REG-002.md`
- `../PROTOTYPE/PROTOTYPE-REG-002.md`

## Slice cubierto

### In

- entrada al check-in diario con bloques agrupados;
- campos: sleep_hours, physical_activity, social_activity, anxiety, irritability, medication_taken (+ medication_time si aplica);
- bloque de medicación condicional (solo si medication_taken = true);
- revisión final antes del guardado;
- guardado con feedback breve;
- uno por día (UPSERT — se actualiza si ya existe registro del día).

### Out

- registro de humor (REG-001) — es un flujo separado;
- historial o visualización de registros (VIN-*, VIS-*);
- registro vía Telegram (TG-001, TG-002);
- exportación de datos (EXP-001).

## Resultado que la UI debe producir

La UI debe sentirse como `check-in compacto`: la persona reconoce rápido qué va a registrar, recorre bloques con lógica simple y cierra sin sentir que rindió un examen.

## Taxonomía de estados obligatorios

| Estado | Objetivo | Trigger principal | Acción dominante |
| --- | --- | --- | --- |
| `S01-ENTRY` | entrada al check-in con framing ligero | llegada desde home o CTA | bloques visibles de inmediato |
| `S02-PARTIAL` | formulario con algunos bloques ya respondidos | avanzar sin mostrar todos los campos a la vez | permite recorrer bloques uno a uno |
| `S02-MEDICATION` | bloque condicional de medicación visible | `medication_taken = true` seleccionado | solicita horario aproximado |
| `S03-READY` | formulario listo para envío | todos los campos obligatorios completados | `Guardar check-in` habilitado |
| `S03-SUBMITTING` | feedback breve de guardado | CTA activada y pulsada | evita doble envío |
| `S03-SUCCESS` | confirmación factual del guardado | `200 OK` o `201 Created` | permite seguir sin pantalla extra |
| `S03-ERROR` | error recuperable cerca del área final | `422` o `500` del backend | permite corregir o reintentar |
| `S03-CONSENT` | redirrección a consentimiento | `CONSENT_REQUIRED` del backend | vínculo directo al panel de consentimiento |
| `S03-SESSION` | reingreso digno tras sesión expirada | `401` del backend | pide reingreso preservando los datos ya cargados |

## Regiones de composición

### `S01-ENTRY` / `S02-*`

- `PatientPageShell`
- `DailyCheckinHeader`
- `FactorBlock` (sueño)
- `FactorBlock` (actividad física)
- `FactorBlock` (actividad social)
- `FactorBlock` (ansiedad)
- `FactorBlock` (irritabilidad)
- `MedicationBlock` (condicional — solo si `medication_taken = true`)
- `DailyCheckinSubmitBar`
- `InlineFeedback`

## Gramática de componentes para implementación

| Primitive / componente | Rol | Estados mínimos |
| --- | --- | --- |
| `PatientPageShell` | shell general del slice autenticado | loading, ready, error |
| `FactorBlock` | bloque de factor individual (bool o float) | unchecked, checked, disabled |
| `MedicationBlock` | bloque condicional de medicación | collapsed, expanded, filled |
| `SleepHoursInput` | input de horas de sueño (float 0-24) | default, filled, error |
| `DailyCheckinSubmitBar` | barra final con CTA única | disabled, enabled, loading |
| `InlineFeedback` | error localized o confirmación factual | info, error, confirm |

Los nombres pueden refinarse en código, pero la separación de responsabilidades no debe colapsarse.

## Contratos backend que la UI debe consumir

### 1. Creación / actualización de DailyCheckin

- precondición: sesión JWT válida y consentimiento activo
- request:
  - `POST /api/v1/daily-checkins`
  - header: `Authorization: Bearer <access_token>`
  - body:
    ```json
    {
      "sleep_hours": float,
      "physical_activity": boolean,
      "social_activity": boolean,
      "anxiety": boolean,
      "irritability": boolean,
      "medication_taken": boolean,
      "medication_time": "HH:MM" // solo si medication_taken = true
    }
    ```
- respuesta esperada:
  - `201 Created` (primer registro del día) con `{ "daily_checkin_id": uuid, "safe_projection": { ... }, "checkin_date": date }`
  - `200 OK` (actualización del registro del día) con mismo body

### 2. Errores tipados del backend

| Código | HTTP | Significado | UI esperada |
| --- | --- | --- | --- |
| `CONSENT_REQUIRED` | 403 | Sin consentimiento activo | redirrección a consentimiento |
| `INVALID_SLEEP_HOURS` | 422 | sleep_hours fuera de 0-24 | error localizado en bloque de sueño |
| `INVALID_BOOLEAN` | 422 | Campo booleano nulo o tipo incorrecto | error localizado en el bloque afectado |
| `MISSING_MEDICATION_TIME` | 422 | medicación = true sin horario | error en bloque de medicación |
| `INVALID_TIME_FORMAT` | 422 | medication_time con formato inválido | error en bloque de medicación |
| `ENCRYPTION_FAILURE` | 500 | Fallo de KMS | error recuperable sin perder datos cargados |

## Mapeo UX -> contrato

| Momento UI | Backend | Condición | Resultado visible |
| --- | --- | --- | --- |
| entrar al check-in | UI local | — | `S01-ENTRY` con primer bloque visible |
| tocar `medication_taken = true` | UI local | interacción inmediata | `S02-MEDICATION` se expande |
| tocar `Guardar check-in` | `POST /api/v1/daily-checkins` | `201` | `S03-SUCCESS` con confirmación factual |
| tocar `Guardar check-in` | `POST /api/v1/daily-checkins` | `200` | `S03-SUCCESS` (actualización del día) |
| tocar `Guardar check-in` | `POST /api/v1/daily-checkins` | `403 CONSENT_REQUIRED` | `S03-CONSENT` |
| tocar `Guardar check-in` | `POST /api/v1/daily-checkins` | `422 VALIDATION_ERROR` | `S03-ERROR` localizada en bloque |
| tocar `Guardar check-in` | `POST /api/v1/daily-checkins` | `401` | `S03-SESSION` con datos preservados |

## Manejo de errores obligatorio

| Código | UI esperada | Regla |
| --- | --- | --- |
| `CONSENT_REQUIRED` | redirrección a consentimiento sin borrar datos cargados | preservarlos en estado local |
| `INVALID_SLEEP_HOURS` | error en el bloque de sueño, los demás bloques siguen | no invalidar todo el formulario |
| `MISSING_MEDICATION_TIME` | error en el bloque de medicación | mantener los demás bloques intactos |
| `ENCRYPTION_FAILURE` | error breve en el área de feedback y opción de reintento | no perder los datos ya cargados |
| `401` | prompt de reingreso que preserve todos los datos del formulario | no exigir recomenzar |

## Reglas de jerarquía y copy

- una sola acción dominante visible por estado (`Guardar check-in`);
- no mostrar todos los bloques a la vez — recorrido progresivo por bloques;
- el bloque de medicación solo aparece cuando `medication_taken = true`;
- la confirmación es factual y no decorativa;
- copy de apoyo mínimo por bloque;
- `Completá tu check-in de hoy` es el titular aprobado en `UXS-REG-002.md`.

## Responsive y accesibilidad

- `mobile-first` real para todo el slice;
- cada bloque factor es tocable sin zoom;
- labels claros para los booleanos (`Sí` / `No`);
- input de `sleep_hours` con keyboard numérico;
- `prefers-reduced-motion` respetado;
- contraste mínimo en todos los controles;
- foco visible en el bloque activo y en el CTA final.

## Validación diferida

Este contrato UI-RFC no sustituye la validación UX real. Antes de marcar el slice como `validated`:

- [ ] capturar evidencia de `S01` y `S02` con personas reales;
- [ ] verificar que el formulario no se siente como una pared de campos;
- [ ] verificar que el bloque de medicación aparece con claridad cuando corresponde;
- [ ] verificar que el guardado no se siente pesado ni ceremonioso.

Estas evidencias se documentarán en `../UX-VALIDATION/UX-VALIDATION-REG-002.md` cuando existan runtime web funcional y evidencia de usuarios reales.

## Dependencias abiertas

- `REG-001` (registro de humor) comparte el shell pero no debe contaminar este contrato;
- `T05` implementa el flujo paciente y consume este contrato sin reinterpretar estados;
- cuando exista implementación funcional, `UX-VALIDATION-REG-002.md` podrá reabrir este contrato si la evidencia contradice el slice.

---

**Estado:** `UI-RFC` activo para `REG-002` bajo wave-prod gap map 2026-04-10.
**Siguiente capa gobernada:** `HANDOFF-SPEC-REG-002.md`, `HANDOFF-ASSETS-REG-002.md`, `HANDOFF-MAPPING-REG-002.md`, `HANDOFF-VISUAL-QA-REG-002.md`.
