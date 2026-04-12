# HANDOFF-SPEC-VIN-001 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `VIN-001` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-VIN-001.md`
- `../UXS/UXS-VIN-001.md`
- `../VOICE/VOICE-VIN-001.md`
- `../PROTOTYPE/PROTOTYPE-VIN-001.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-VIN.md`

## Alcance implementable

### In

- formulario de invitación profesional con email;
- revisión del alcance antes de emitir;
- emisión de vínculo pendiente;
- conflicto por vínculo o invitación existente.

### Out

- flujos de vinculación por código del paciente;
- gestión de acceso profesional;
- páginas profesionales;
- variante Telegram.

## Estados que deben existir

1. entrada con campo y alcance visible;
2. listo para enviar (CTA habilitada);
3. envío en curso (feedback breve);
4. éxito (estado pendiente visible);
5. conflicto (vínculo o invitación existente).

## Restricciones cerradas

- no usar lenguaje de alta administrativa de paciente;
- no prometer acceso clínico automático;
- no mostrar copy celebratoria;
- no introducir segunda acción dominante;
- no usar jerga técnica de backend.

## Contratos de transición

- `page.tsx` abre el slice desde la UI profesional;
- email válido habilita la CTA;
- `POST /api/v1/carelinks/invite` exitoso deriva a estado pendiente;
- conflicto existente muestra carta sin repetir la acción.

## Blockers explícitos

- ninguno — el endpoint `POST /api/v1/professional/invites` existe en runtime (verificado mi-lsp 2026-04-12);
- la validación UX real sigue diferida a `Phase 60`.

## Dependencias para implementación

- `T04`: shell profesional, sesion, cliente API, routing base;
- cuando la evidencia de runtime contradiga el contrato, se actualiza en `UI-RFC-VIN-001.md`.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- qué historia cuenta el formulario;
- cómo se expresa el alcance del vínculo pendiente;
- qué pasa cuando ya existe un vínculo o invitación;
- cuál es el resultado visible después del envío.

---

**Estado:** listo para consumo por `T04/T05`.
**Siguiente artefacto:** `HANDOFF-MAPPING-VIN-001.md` si se requiere mapeo explícito de diseño a componente.
