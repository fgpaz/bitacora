# UXS-CON-002 — Revocación del consentimiento

## Propósito

Este documento fija el contrato UX del paso crítico del slice `CON-002`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-CON-002.md`
- `../UXI/UXI-CON-002.md`
- `../UJ/UJ-CON-002.md`
- `../VOICE/VOICE-CON-002.md`
- `../../03_FL/FL-CON-02.md`
- `../../06_pruebas/TP-CON.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-CON-002.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-CON-002.md`

## Slice y paso dueño

- slice: `CON-002`
- paso crítico: `S02` — Revisión del impacto antes de revocar
- entrada: la persona llega con intención de cortar el consentimiento vigente
- salida correcta: el consentimiento queda revocado y la suspensión de registro/accesos se entiende

## Sensación del paso

- sensación objetivo: una pausa seria pero serena
- anti-sensación: una pared de advertencias

## Tarea del usuario

1. entender el impacto
2. confirmar la decisión
3. salir con claridad sobre el nuevo estado

## Contrato de interacción

### Estructura mínima

- encabezado breve
- resumen de impacto en dos o tres ideas
- acción principal de revocación y secundaria de conservación

### Acción primaria

- `Revocar consentimiento`

### Acción secundaria

- `Conservar consentimiento`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| default | impacto visible con CTA disponible | permite revisar y decidir |
| submitting | feedback breve de proceso | evita doble submit |
| success | estado revocado visible | confirma suspensión de registro y accesos |
| error_retryable | error claro de revocación | permite reintentar sin ambigüedad |

## Contrato de copy

- titular aprobado: `Revocar consentimiento`
- texto de apoyo aprobado: `Si seguís, se suspende el registro y los profesionales pierden acceso a tus datos.`
- acción primaria aprobada: `Revocar consentimiento`
- error recuperable aprobado: `No pudimos revocar este consentimiento. Probá de nuevo.`

## Aceptación

1. la cascada a vínculos queda clara antes de confirmar
2. la pantalla no suena punitiva ni legalista
3. el estado final se entiende sin explicación adicional

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

## Deltas 2026-04-23 — CON-002 UI materializada

> 2026-04-23 — sync slice CON-002: materialización frontend en rama `feature/login-flow-followups-2026-04-23` (W7). Fuente: `.docs/raw/reports/2026-04-23-login-flow-followups-v2-closure.md`. Backend preexistente (`DELETE /api/v1/consent/current` + `RevokeConsentCommand`).

### ConsentRevocationPanel — nuevo componente

- `frontend/components/patient/consent/ConsentRevocationPanel.tsx` nuevo + `.module.css`.
- Estados implementados: `loading | default | submitting | success | error` (alineados al modelo de estados del UXS).
- Estructura visible: header con título `"Revocar consentimiento"` + versión activa + sección `"Qué pasa si seguís"` con 4 bullets de impacto + `decisionBar` con 2 CTAs (conservar + revocar).
- Integración con backend existente: `revokeConsent(confirmed: true)` en `lib/api/client.ts` llama a `DELETE /api/v1/consent/current` con body `{confirmed: true}`.
- Ley 26.529 Art. 10 revocabilidad + Art. 2 inc. e) autonomía.

### Ruta nueva `/configuracion/consent`

- `frontend/app/(patient)/configuracion/consent/page.tsx` nuevo. Usa `PatientPageShell` como envoltura, igual patrón que `/configuracion/vinculos` y `/configuracion/telegram`.

### ShellMenu — item `Consentimiento`

- Agregado entre `Vínculos` y el separator del logout en `PatientPageShell.tsx`. Cumple la promesa UI de la revocationNote del ConsentGatePanel (`"Podés revocarlo cuando quieras desde Mi cuenta."`).

### Copy aprobado materializado

- Título: `"Revocar consentimiento"` (UXS aprobado).
- Sección impacto: `"Qué pasa si seguís"` (variación UX de la regla base "impacto visible").
- 4 bullets de impacto:
  - `"Se suspende el registro de nuevos datos en Bitácora."`
  - `"Los profesionales vinculados pierden acceso a tus datos."`
  - `"Tu historial anterior queda guardado; no se borra."` (nuevo — aclaración sobre persistencia del histórico).
  - `"Podés volver a otorgar el consentimiento cuando quieras."` (nuevo — reversibilidad explícita).
- CTA primario: `"Revocar consentimiento"` (UXS aprobado).
- CTA secundario: `"Conservar consentimiento"` (UXS aprobado).
- Success: `"Tu consentimiento quedó revocado."` + nota de impacto + CTA `"Volver al dashboard"`.
- Error canon 13 via `formatUserFacingError()`.

### Tests e2e nuevos

- `frontend/e2e/consent-revocation.spec.ts` con 3 tests: panel visible con impact list, DELETE dispara y muestra confirmación, ShellMenu contiene item `"Consentimiento"` que navega.

### Notas de implementación

- Cambios `ui-only + client.ts wire, no-schema, no-contract nuevo, no-auth-edit`.
- Copy congelado preservado: los CTAs primario/secundario del UXS quedan literales.
- Stable step `S02` del UJ cerrado con UI materializada.

---

**Estado:** `UXS` activo para `CON-002` con implementación frontend materializada 2026-04-23 (backend preexistente).
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-CON-002.md` y `../UX-VALIDATION/UX-VALIDATION-CON-002.md` con evidencia real de pacientes usando la UI.
