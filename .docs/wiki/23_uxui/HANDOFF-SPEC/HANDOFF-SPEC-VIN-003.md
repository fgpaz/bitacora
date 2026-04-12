# HANDOFF-SPEC-VIN-003 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `VIN-003` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-VIN-003.md`
- `../UXS/UXS-VIN-003.md`
- `../VOICE/VOICE-VIN-003.md`
- `../PROTOTYPE/PROTOTYPE-VIN-003.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-VIN.md`

## Alcance implementable

### In

- entrada con contexto del vínculo actual;
- confirmación de revocación con impacto visible;
- resultado con acceso cortado.

### Out

- flujos de invitación profesional;
- gestión de acceso;
- revocación de consentimiento.

## Estados que deben existir

1. lectura breve del vínculo actual antes de decidir;
2. impacto visible y CTA principal disponible;
3. feedback corto durante revocación;
4. vínculo revocado y acceso cortado;
5. vínculo ya revocado sin duplicar la acción;
6. error recuperable con reintento claro.

## Restricciones cerradas

- no usar framing culpabilizante ni de pérdida emocional;
- no pedir más datos para revocar;
- no introducir una tercera acción principal;
- no presentar la revocación como castigo o ruptura terapéutica;
- no igualar vínculo con consentimiento.

## Contratos de transición

- `page.tsx` abre el slice desde gestión de vínculos;
- `DELETE /api/v1/carelinks/<id>` exitoso deriva a estado revocado;
- error `ALREADY_REVOKED` muestra estado actual sin repetir acción.

## Blockers explícitos

- ninguno — los endpoints `DELETE /api/v1/vinculos/{id}` y `GET /api/v1/vinculos` existen en runtime (verificado mi-lsp 2026-04-12);
- la validación UX real sigue diferida a `Phase 60`.

## Dependencias para implementación

- `T04`: shell paciente, sesion, cliente API, routing base;
- cuando la evidencia de runtime contradiga el contrato, se actualiza en `UI-RFC-VIN-003.md`.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- cómo se presenta el impacto antes de confirmar;
- cómo se distingue la acción secundaria de la primaria;
- qué copy se usa para confirmar el acceso cortado;
- cómo se presenta el error recuperable.

---

**Estado:** listo para consumo por `T04/T05`.
**Siguiente artefacto:** `HANDOFF-MAPPING-VIN-003.md` si se requiere mapeo explícito de diseño a componente.
