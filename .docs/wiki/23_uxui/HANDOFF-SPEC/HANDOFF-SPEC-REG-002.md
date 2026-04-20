# HANDOFF-SPEC-REG-002 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `REG-002` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-REG-002.md`
- `../UXS/UXS-REG-002.md`
- `../VOICE/VOICE-REG-002.md`
- `../PROTOTYPE/PROTOTYPE-REG-002.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-REG.md`

## Alcance implementable

### In

- entrada al check-in diario con bloques agrupados;
- factores: sueño, actividad física, actividad social, ansiedad, irritabilidad;
- bloque de medicación condicional (horario aproximado);
- revisión final con una sola acción dominante;
- guardado con feedback breve (uno por día — UPSERT);
- manejo de error recuperable sin pérdida de datos cargados.

### Out

- registro de humor (REG-001) — es un flujo separado;
- historial o gráficos de factores (VIN-*, VIS-*);
- UI de Telegram;
- exportación de datos.

## Estados que deben existir

1. entrada al check-in con framing ligero;
2. formulario con bloques parcialmente completados;
3. bloque de medicación expandido (si `medication_taken = true`);
4. formulario listo para envío (`Guardar check-in` habilitado);
5. feedback de guardado en curso;
6. confirmación factual breve;
7. error recuperable con localized feedback;
8. redirrección a consentimiento;
9. reingreso de sesión preservando todos los datos cargados.

## Restricciones cerradas

- no mostrar todos los bloques a la vez — recorrido progresivo;
- no pedir más campos que los definidos en `RF-REG-021`;
- no usar tono clínico, moralizante o evaluativo;
- no mostrar el bloque de medicación si `medication_taken = false`;
- no perder los datos cargados ante error recuperable;
- no usar pantalla extra tras el guardado — confirmación inline;
- no abrir REG-001 dentro de este slice.

## Contratos de transición

- `app/(patient)/registro/daily-checkin/page.tsx` abre el slice;
- `POST /api/v1/daily-checkins` con body válido deriva a `S03-SUCCESS`;
- `201` = primer check-in del día, `200` = actualización del día;
- `403 CONSENT_REQUIRED` redirrige a consentimiento con datos preservados;
- `422 VALIDATION_ERROR` muestra error localized sin borrar el formulario;
- la continuidad tras guardar es hacia la vista anterior o hacia el siguiente paso del paciente.

## Blockers explícitos ya resueltos

- el slice queda abierto por wave-prod gap map 2026-04-10;
- la validación UX real sigue pendiente — diferida a la fase post-runtime del portfolio;
- ni PROTOTYPE ni seed docs claiman validación ya ocurrida.

## Dependencias para implementación

- `T04`: shell paciente, sesión Zitadel, cliente API, routing base;
- `T05`: componentes del flujo de check-in diario;
- el endpoint `POST /api/v1/daily-checkins` debe estar implementado y deployado antes de que este slice pueda probarse end-to-end;
- la validación de `medication_time` como formato `HH:MM` normalizado a bloques de 15 minutos es responsabilidad del backend — frontend solo transmite y muestra el error si llega.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- qué bloques muestra y en qué orden;
- cuándo aparece el bloque de medicación;
- qué pasa cuando el usuario ya tiene un check-in del día;
- qué mensaje de error se muestra en cada código;
- qué pasa tras la confirmación factual;
- cómo se preservan los datos ante error o reingreso de sesión.

---

**Estado:** listo para consumo por `T05`.
**Siguiente artefacto:** `HANDOFF-ASSETS-REG-002.md`.
