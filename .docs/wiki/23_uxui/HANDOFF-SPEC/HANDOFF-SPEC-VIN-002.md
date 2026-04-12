# HANDOFF-SPEC-VIN-002 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `VIN-002` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-VIN-002.md`
- `../UXS/UXS-VIN-002.md`
- `../VOICE/VOICE-VIN-002.md`
- `../PROTOTYPE/PROTOTYPE-VIN-002.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-VIN.md`

## Alcance implementable

### In

- entrada con contexto breve;
- ingreso o pegado del código;
- validación y creación del vínculo;
- confirmación con acceso todavía bajo control del paciente.

### Out

- flujos de invitación profesional;
- gestión de acceso profesional;
- variante Telegram.

## Estados que deben existir

1. entrada con contexto breve y código pendiente;
2. campo vacío con efecto del vínculo visible;
3. código listo para enviar;
4. envío en curso (feedback breve);
5. código inválido con recuperación digna;
6. código expirado con salida clara;
7. vínculo activo con acceso aún desactivado;
8. vínculo ya existente sin duplicación.

## Restricciones cerradas

- no pedir información extra además del código;
- no usar tecnicismos sobre binding, token o habilitación;
- no confirmar el vínculo sin explicar el límite de acceso;
- no agregar más de una acción primaria por paso;
- no introducir copy celebratoria.

## Contratos de transición

- `page.tsx` abre el slice paciente;
- código válido habilita la CTA;
- `POST /api/v1/carelinks/bind` exitoso deriva a vínculo activo con `canViewData=false`;
- conflicto existente muestra estado sin repetir la acción de vincular.

## Blockers explícitos

- ninguno — los endpoints `POST /api/v1/vinculos/accept`, `GET /api/v1/vinculos`, `GET /api/v1/vinculos/active` existen en runtime (verificado mi-lsp 2026-04-12);
- la validación UX real sigue diferida a `Phase 60`.

## Dependencias para implementación

- `T04`: shell paciente, sesion, cliente API, routing base;
- cuando la evidencia de runtime contradiga el contrato, se actualiza en `UI-RFC-VIN-002.md`.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- qué contexto se muestra antes del código;
- cómo se explica que vínculo no es acceso automático;
- qué mensaje se muestra cuando el código no sirve;
- cómo se cierra la confirmación del vínculo.

---

**Estado:** listo para consumo por `T04/T05`.
**Siguiente artefacto:** `HANDOFF-MAPPING-VIN-002.md` si se requiere mapeo explícito de diseño a componente.
