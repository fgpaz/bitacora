# HANDOFF-SPEC-CON-002 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `CON-002` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-CON-002.md`
- `../UXS/UXS-CON-002.md`
- `../VOICE/VOICE-CON-002.md`
- `../PROTOTYPE/PROTOTYPE-CON-002.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-CON.md`

## Alcance implementable

### In

- entrada al consentimiento activo;
- revisión del impacto antes de revocar;
- confirmación de revocación con cascada clara.

### Out

- flujo de nuevo consentimiento;
- vinculación;
- variante Telegram.

## Estados que deben existir

1. consentimiento vigente antes de revisar impacto;
2. impacto visible y CTA disponible;
3. feedback corto durante revocación;
4. consentimiento revocado con cascada entendible;
5. consentimiento ya revocado sin duplicar acción;
6. error recuperable con reintento claro.

## Restricciones cerradas

- no convertir el paso en una pared de advertencias;
- no usar tono legalista ni de castigo;
- no ocultar que la revocación afecta tanto registro como acceso;
- no introducir más de una acción principal;
- no usar copy que suene a bloqueo definitivo.

## Contratos de transición

- `page.tsx` abre el slice desde ajustes de consentimiento;
- `DELETE /api/v1/consent` exitoso deriva a estado `revoked` con cascada visible;
- error `ALREADY_REVOKED` muestra estado actual sin repetir pantalla;
- `SESSION_EXPIRED` deriva a reautenticación con continuidad.

## Blockers explícitos

- endpoint de revocación de consentimiento es futuro — contrato marcado como especulativo;
- la cascada a vínculos depende de que existan los endpoints de CareLink;
- no existe runtime de consentimiento todavía;
- la validación UX real sigue diferida a `Phase 60`.

## Dependencias para implementación

- `T04`: shell paciente, sesión, cliente API, routing base;
- endpoint de consentimiento (`DELETE /api/v1/consent`) pendiente de definición;
- detalle de la cascada a vínculos requiere coordinación con T05 cuando existan endpoints CareLink;
- cuando exista runtime, los contratos backend se actualizan en `UI-RFC-CON-002.md`.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- cómo se enumera el impacto en 2-3 ideas concretas;
- cómo se presenta la asimetría entre acción primaria y secundaria;
- qué copy se usa para confirmar la suspensión de registro y accesos;
- cómo se presenta el error recuperable.

---

**Estado:** listo para consumo por `T04/T05`.
**Siguiente artefacto:** `HANDOFF-MAPPING-CON-002.md` si se requiere mapeo explícito de diseño a componente.
