# UX-VALIDATION — Índice de validaciones por slice

## Propósito

Este índice lista el estado de validación UX de cada slice visible.

No reemplaza `21_matriz_validacion_ux.md` ni los futuros `UX-VALIDATION-*`. Su función es centralizar el punto de entrada de la familia `UX-VALIDATION` dentro de `23_uxui`.

## Documentos activos — sesión 2026-04-10

Se crearon los primeros documentos `UX-VALIDATION-*` backed por evidencia de la sesión de validación 2026-04-10. Todos reflejan estado **`blocked`** — validación intentada pero no ejecutada por faltarle al entorno credentials y sesiones interactivas.

| Slice | Estado | Base disponible | Resultado de la sesión 2026-04-10 |
| --- | --- | --- | --- |
| `ONB-001` | blocked | prototipo + operativo listo | validación bloqueada por WEB-VAL-001 + WEB-VAL-002 |
| `REG-001` | blocked | prototipo + operativo listo | validación bloqueada por WEB-VAL-001 + WEB-VAL-002 |
| `REG-002` | blocked | prototipo + operativo listo | validación bloqueada por WEB-VAL-001 + WEB-VAL-002 |
| `VIN-002` | waiting evidence | prototipo + operativo listo | sin intento en esta sesión |
| `VIN-004` | waiting evidence | prototipo + operativo listo | sin intento en esta sesión |
| `VIN-003` | waiting evidence | prototipo + operativo listo | sin intento en esta sesión |
| `CON-002` | waiting evidence | prototipo + operativo listo | sin intento en esta sesión |
| `VIS-001` | blocked | prototipo + operativo listo | validación bloqueada por WEB-VAL-001 + WEB-VAL-002 |
| `EXP-001` | blocked | prototipo + operativo listo | validación bloqueada por WEB-VAL-001 + WEB-VAL-002 |
| `VIN-001` | blocked | prototipo + operativo listo | validación bloqueada por WEB-VAL-001 + WEB-VAL-002 |
| `VIS-002` | waiting evidence | prototipo + operativo listo | sin intento en esta sesión |
| `TG-001` | blocked | prototipo + operativo listo | validación bloqueada por TG-VAL-001 + TG-VAL-002 |
| `TG-002` | blocked | prototipo + operativo listo | validación bloqueada por TG-VAL-001 + TG-VAL-002 |

## Defectos críticos abiertos

| Defecto | Severidad | Slice afectada | Owner |
| --- | --- | --- | --- |
| `WEB-VAL-001` | Critical | ONB-001, REG-001, REG-002, VIN-001, VIS-001, EXP-001 | runtime/frontend config |
| `WEB-VAL-002` | Critical | ONB-001, REG-001, REG-002, VIN-001, VIS-001, EXP-001 | QA / validation harness |
| `TG-VAL-001` | Critical | TG-001, TG-002 | runtime/secret provisioning |
| `TG-VAL-002` | Critical | TG-001, TG-002 | QA / Telegram E2E setup |
| `WEB-VAL-003` | High | todas las rutas web | frontend runtime hardening |
| `TG-VAL-003` | High | TG-002 | backend runtime / QA |

## Waiver en vigor

La cobertura documental de profesional y Telegram se completó bajo waiver explícito el 2026-04-08. Ese waiver no promueve ningún slice a `validated`.

También existe un waiver de entrada a UI en `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`. Permite avanzar en documentación UI antes de la validación, pero no cambia la deuda de esta familia.

## Regla de creación

Un `UX-VALIDATION-*` solo puede pasar a `validated` cuando:

- existe prototipo enlazado y testeable;
- hubo evidencia moderada u observada suficiente;
- los hallazgos y severidades están explícitos;
- ningún crítico permanece abierto;
- el retorno a `VOICE` o `UXS` quedó resuelto o claramente marcado.

Los documentos creados en esta sesión respetan esa regla: todos reflejan estado `blocked` con defectos abiertos documentados honestamente.

---

**Estado:** familia `UX-VALIDATION` con documentos creados para 7 slices — todos `blocked`.
**Siguiente capa gobernada:** ejecución de cohortes reales para cerrar defectos y promover a `validated`.
