# HANDOFF-SPEC-EXP-001 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `EXP-001` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-EXP-001.md`
- `../UXS/UXS-EXP-001.md`
- `../VOICE/VOICE-EXP-001.md`
- `../PROTOTYPE/PROTOTYPE-EXP-001.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-EXP.md`

## Alcance implementable

### In

- entrada con alcance visible;
- selección o confirmación de período;
- preparación y disparo de descarga.

### Out

- timeline;
- detalle de registro;
- variante Telegram.

## Estados que deben existir

1. alcance visible y CTA disponible;
2. período seleccionado y alcance actualizado;
3. preparación breve del archivo;
4. descarga iniciada o lista para continuar;
5. error recuperable con reintento.

## Restricciones cerradas

- no explicar cifrado, payloads ni detalle técnico de stream;
- no convertir la exportación en consola o tabla administrativa;
- no competir con una segunda acción principal;
- no romper continuidad visual con `VIS-001`;
- no usar jerga legal.

## Contratos de transición

- `page.tsx` abre el slice desde el timeline o como ruta independiente;
- selector de período actualiza el alcance sin cambiar de pantalla;
- `POST /api/v1/mood-entries/export` devuelve `202` y la descarga arranca;
- éxito cierra sin pantalla extra.

## Blockers explícitos

- ninguno — los endpoints `GET /api/v1/export/patient-summary` y `GET /api/v1/export/patient-summary/csv` existen en runtime (verificado mi-lsp 2026-04-12);
- la validación UX real sigue diferida a `Phase 60`.

## Dependencias para implementación

- `T04`: shell paciente, sesion, cliente API, routing base;
- cuando la evidencia de runtime contradiga el contrato, se actualiza en `UI-RFC-EXP-001.md`.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- qué copy se usa para explicar el alcance del archivo;
- cómo se presenta el selector de período;
- qué feedback se muestra mientras se prepara la descarga;
- cómo se presenta el éxito.

---

**Estado:** listo para consumo por `T04/T05`.
**Siguiente artefacto:** `HANDOFF-MAPPING-EXP-001.md` si se requiere mapeo explícito de diseño a componente.
