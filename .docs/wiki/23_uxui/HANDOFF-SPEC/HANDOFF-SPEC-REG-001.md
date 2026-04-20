# HANDOFF-SPEC-REG-001 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `REG-001` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-REG-001.md`
- `../UXS/UXS-REG-001.md`
- `../VOICE/VOICE-REG-001.md`
- `../PROTOTYPE/PROTOTYPE-REG-001.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-REG.md`

## Alcance implementable

### In

- entrada contextualizada al registro rápido de humor;
- escala visible -3..+3;
- gesto directo de selección y guardado con feedback inmediato;
- confirmación factual breve;
- manejo de error recuperable sin pérdida de dato;
- redirrección clara a consentimiento si corresponde.

### Out

- formulario de factores diarios (REG-002);
- historial o gráficos de humor (VIN-*, VIS-*);
- UI de Telegram;
- exportación de datos.

## Estados que deben existir

1. entrada al registro con escala visible;
2. escala con valor seleccionado;
3. feedback de guardado en curso;
4. confirmación factual breve;
5. error recuperable con opción de reintento;
6. redirrección a consentimiento;
7. reingreso de sesión preservando el valor elegido.

## Restricciones cerradas

- no abrir más de 7 valores en la escala;
- no agregar campos adicionales al MoodEntry más allá del score;
- no mostrar pantalla intermedia entre selección y confirmación;
- no usar copy interpretativo o terapéutico;
- no perder el valor ya seleccionado ante error de red o 500;
- no abrir REG-002 dentro de este slice.

## Contratos de transición

- `app/(patient)/registro/mood-entry/page.tsx` abre el slice;
- la selección de un valor envía `POST /api/v1/mood-entries` de forma inmediata;
- `201` o `200` deriva a confirmación factual y permite continuar;
- `403 CONSENT_REQUIRED` redirrige a `../onboarding/consent/page.tsx` (o al flujo de consentimiento);
- `422` deriva a error localized que no borra el valor ya elegido;
- la continuidad tras guardar es hacia la vista anterior o hacia el siguiente paso del paciente.

## Blockers explícitos ya resueltos

- el slice queda abierto por wave-prod gap map 2026-04-10;
- la validación UX real sigue pendiente — diferida a la fase post-runtime del portfolio;
- ni PROTOTYPE ni seed docs claiman validación ya ocurrida.

## Dependencias para implementación

- `T04`: shell paciente, sesión Zitadel, cliente API, routing base;
- `T05`: componentes del flujo de registro rápido de humor;
- el endpoint `POST /api/v1/mood-entries` debe estar implementado y deployado antes de que este slice pueda probarse end-to-end.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- qué valores muestra la escala;
- cómo se comporta el gesto de selección;
- qué mensaje de error se muestra en cada código;
- qué pasa tras la confirmación factual;
- cómo se maneja el caso de consentimiento faltante.

---

**Estado:** listo para consumo por `T05`.
**Siguiente artefacto:** `HANDOFF-ASSETS-REG-001.md`.
