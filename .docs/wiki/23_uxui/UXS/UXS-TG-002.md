# UXS-TG-002 — Respuesta al recordatorio

## Propósito

Este documento fija el contrato UX del paso crítico del slice `TG-002`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-TG-002.md`
- `../UXI/UXI-TG-002.md`
- `../UJ/UJ-TG-002.md`
- `../VOICE/VOICE-TG-002.md`
- `../../03_FL/FL-TG-02.md`
- `../../03_FL/FL-REG-02.md`
- `../../06_pruebas/TP-TG.md`
- `../../06_pruebas/TP-REG.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-TG-002.md`
- `../UX-VALIDATION/UX-VALIDATION-TG-002.md`

## Slice y paso dueño

- slice: `TG-002`
- paso crítico: `S02` — Respuesta al recordatorio
- entrada: la persona recibe un mensaje del sistema en Telegram
- salida correcta: responde con un valor o decide no hacerlo sin fricción adicional

## Sensación del paso

- sensación objetivo: un toque breve y opcional
- anti-sensación: un empuje insistente

## Tarea del usuario

1. leer la pregunta
2. decidir si responde
3. entender el resultado inmediato

## Contrato de interacción

### Estructura mínima

- mensaje breve
- keyboard inline con valores
- salida fácil
- confirmación conversacional corta

### Acción primaria

- `Elegir un valor`

### Acción secundaria

- `Ahora no`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| reminder_sent | pregunta y keyboard visibles | permite responder o salir |
| reply_submitting | feedback breve en el chat | evita doble tap |
| reply_success | confirmación corta y continuación opcional | deja el canal fluido |
| reply_error | mensaje corto de fallo | permite reintentar sin presión |

## Contrato de copy

- titular aprobado: `¿Cómo te sentís?`
- texto de apoyo aprobado: `Respondé si te sirve ahora.`
- acción primaria aprobada: `Elegir valor`
- error recuperable aprobado: `No pudimos registrar esa respuesta. Probá de nuevo si querés.`

## Aceptación

1. el recordatorio se siente opcional
2. la salida fácil está visible
3. la confirmación no insiste ni felicita

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

---

## Nota de estado runtime y validación

- `TelegramSession` y `ReminderConfig` existen en runtime (Phase 30+): entities, tablas, seam webhook y `ReminderWorker` activos segun `TECH-TELEGRAM.md`
- la validacion UX real queda diferida a `Phase 60`
- `UI-RFC-TG-002.md` y `HANDOFF-SPEC-TG-002.md` actuan como contrato conversacional para implementacion backend/telegram
- los estados declarados en este documento (reminder_sent, reply_submitting, reply_success, reply_error) fueron heredados en el arbol de estados del UI-RFC

## Deltas 2026-04-22 — impeccable-hardening

> 2026-04-22 — sync impeccable-hardening: deltas aplicados sobre implementación en rama `feature/impeccable-hardening-2026-04-22` (W5). Fuente de verdad: `.docs/raw/reports/2026-04-22-impeccable-hardening-closure.md`.

### TelegramPairingCard — split presentacional (compartido con TG-001)

- El split presentacional de `TelegramPairingCard` en 3 subcomponentes (`PairingCodeDisplay`, `PairingInstructions`, `PairingReminderSection`) afecta también este slice en lo que respecta al schedule picker y la sección de recordatorio conversacional (W5).
- El padre mantiene el estado compartido; los subcomponentes son puramente presentacionales y no introducen lógica nueva.
- Ver detalle completo del split y mapping en `../HANDOFF-MAPPING/HANDOFF-MAPPING-TG-001.md` y `../HANDOFF-MAPPING/HANDOFF-MAPPING-TG-002.md`.

### Copy del mensaje guardado

- `"Recordatorio guardado para las {time}, hora de Buenos Aires."` — sin cambios en esta wave; copy aprobado conservado.

### Notas de implementación

- Todos los cambios son `ui-only, no-schema, no-contract, no-auth`.

**Estado:** `UXS` activo para `TG-002`.
**Siguiente capa gobernada:** `../PROTOTYPE/PROTOTYPE-TG-002.md` y `../UX-VALIDATION/UX-VALIDATION-TG-002.md`.
