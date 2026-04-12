# HANDOFF-SPEC-VIS-001 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `VIS-001` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-VIS-001.md`
- `../UXS/UXS-VIS-001.md`
- `../VOICE/VOICE-VIS-001.md`
- `../PROTOTYPE/PROTOTYPE-VIS-001.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-VIS.md`

## Alcance implementable

### In

- apertura del timeline con período base;
- lectura inicial del gráfico;
- ajuste de período.

### Out

- exportación CSV;
- detalle de registro individual;
- variante Telegram.

## Estados que deben existir

1. skeleton o placeholder editorial del timeline;
2. timeline base con período actual visible;
3. ajuste de período con chart aún dominante;
4. vacío útil y distinguible de error;
5. error breve de carga con reintento.

## Restricciones cerradas

- no usar jerga analítica ni framing clínico;
- no enterrar el chart detrás de controles;
- no convertir `EXP-001` en CTA principal del slice;
- no confundir ausencia de datos con falla del sistema;
- no introducir animaciones excesivas.

## Contratos de transición

- `page.tsx` abre el slice desde la home del paciente;
- período por defecto `month`;
- cambio de período actualiza el chart sin recargar la página;
- estado vacío se distingue del error con copy orientado.

## Blockers explícitos

- ninguno — los endpoints `GET /api/v1/visualizacion/timeline` y `GET /api/v1/visualizacion/summary` existen en runtime (verificado mi-lsp 2026-04-12);
- la validación UX real sigue diferida a `Phase 60`.

## Dependencias para implementación

- `T04`: shell paciente, sesion, cliente API, routing base;
- librería del chart pendiente de definición técnica;
- cuando la evidencia de runtime contradiga el contrato, se actualiza en `UI-RFC-VIS-001.md`.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- cuál es el chart principal y cómo se presenta;
- cómo se comportan los filtros de período;
- qué copy se usa cuando no hay datos;
- cómo se presenta el error de carga.

---

**Estado:** listo para consumo por `T04/T05`.
**Siguiente artefacto:** `HANDOFF-MAPPING-VIS-001.md` si se requiere mapeo explícito de diseño a componente.
