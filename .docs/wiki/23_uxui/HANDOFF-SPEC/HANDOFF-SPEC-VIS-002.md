# HANDOFF-SPEC-VIS-002 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `VIS-002` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-VIS-002.md`
- `../UXS/UXS-VIS-002.md`
- `../VOICE/VOICE-VIS-002.md`
- `../PROTOTYPE/PROTOTYPE-VIS-002.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-VIS.md`
- `../../06_pruebas/TP-SEC.md`

## Alcance implementable

### In

- carga inicial del dashboard;
- lista paginada de pacientes visibles;
- resumen y alertas básicas por tarjeta;
- cambio de página.

### Out

- detalle de paciente individual;
- flujos de vinculación;
- variante Telegram.

## Estados que deben existir

1. skeleton o placeholder sobrio;
2. lista de pacientes visible y priorizable;
3. cambio de página sin ruido;
4. ausencia de pacientes visibles;
5. error recuperable de carga.

## Restricciones cerradas

- no usar estética de EHR ni de pared de alertas;
- no dramatizar alertas básicas;
- no perder el límite de acceso visible;
- no mostrar pacientes sin acceso habilitado;
- no convertir la lista en tabla administrativa.

## Contratos de transición

- `page.tsx` abre el slice desde la home profesional;
- paginación carga la siguiente página sin recargar todo;
- selección de paciente deriva a detalle;
- vacío se presenta con copy claro y no como error.

## Blockers explícitos

- CareLink endpoints de pacientes son futuros — contratos marcados como especulativos;
- no existe runtime CareLink activo;
- la validación UX real sigue diferida a `Phase 60`.

## Dependencias para implementación

- `T04`: shell profesional, sesión, cliente API, routing base;
- endpoints de CareLink (`GET /api/v1/carelinks/patients`) pendientes de definición;
- cuando exista runtime, los contratos backend se actualizan en `UI-RFC-VIS-002.md`.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- cómo se presenta la lista de pacientes;
- cómo se diferencian las alertas básicas de las dramáticas;
- cómo se presenta el vacío;
- cómo se implementa la paginación.

---

**Estado:** listo para consumo por `T04/T05`.
**Siguiente artefacto:** `HANDOFF-MAPPING-VIS-002.md` si se requiere mapeo explícito de diseño a componente.
