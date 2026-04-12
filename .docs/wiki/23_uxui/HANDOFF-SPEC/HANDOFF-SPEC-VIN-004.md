# HANDOFF-SPEC-VIN-004 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `VIN-004` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-VIN-004.md`
- `../UXS/UXS-VIN-004.md`
- `../VOICE/VOICE-VIN-004.md`
- `../PROTOTYPE/PROTOTYPE-VIN-004.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-VIN.md`

## Alcance implementable

### In

- lectura del estado actual de acceso;
- decisión y guardado del cambio;
- confirmación del nuevo estado.

### Out

- flujos de invitación profesional;
- revocación de vínculo;
- variante Telegram.

## Estados que deben existir

1. estado actual legible antes del cambio;
2. vínculo inactivo que no admite cambios;
3. nuevo estado seleccionado y efecto explícito;
4. feedback corto durante guardado;
5. nuevo estado guardado con efecto visible;
6. error recuperable con reintento claro.

## Restricciones cerradas

- no usar lenguaje interno como `can_view_data`;
- no igualar acceso con vínculo;
- no convertir el control en un switch opaco sin explicación;
- no introducir una segunda acción primaria;
- no usar copy dramatizante sobre el cambio.

## Contratos de transición

- `page.tsx` abre el slice desde gestión de vínculos;
- `PATCH /api/v1/carelinks/<id>/access` exitoso deriva a nuevo estado guardado;
- error `LINK_INACTIVE` muestra inactivo sin toggle activo.

## Blockers explícitos

- ninguno — los endpoints `PATCH /api/v1/vinculos/{id}/view-data`, `GET /api/v1/vinculos`, `GET /api/v1/vinculos/active` existen en runtime (verificado mi-lsp 2026-04-12);
- la validación UX real sigue diferida a `Phase 60`.

## Dependencias para implementación

- `T04`: shell paciente, sesion, cliente API, routing base;
- patrón de guardado (explícito o auto-save) pendiente de definición en T04;
- cuando la evidencia de runtime contradiga el contrato, se actualiza en `UI-RFC-VIN-004.md`.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- cómo se presenta el estado actual antes de interactuar;
- cómo se explica el efecto del cambio;
- cómo se presenta el resultado del guardado;
- cómo se maneja el error recuperable.

---

**Estado:** listo para consumo por `T04/T05`.
**Siguiente artefacto:** `HANDOFF-MAPPING-VIN-004.md` si se requiere mapeo explícito de diseño a componente.
