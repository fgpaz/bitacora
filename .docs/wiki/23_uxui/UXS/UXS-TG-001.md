# UXS-TG-001 — Código de vinculación a Telegram

## Propósito

Este documento fija el contrato UX del paso crítico del slice `TG-001`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-TG-001.md`
- `../UXI/UXI-TG-001.md`
- `../UJ/UJ-TG-001.md`
- `../VOICE/VOICE-TG-001.md`
- `../../03_FL/FL-TG-01.md`
- `../../06_pruebas/TP-TG.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-TG-001.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-TG-001.md`

## Slice y paso dueño

- slice: `TG-001`
- paso crítico: `S01` — Generación y handoff del código
- entrada: la persona decide habilitar Telegram desde la web
- salida correcta: sale con un código útil o con el vínculo ya confirmado

## Sensación del paso

- sensación objetivo: un puente corto y guiado
- anti-sensación: una pantalla de setup confusa

## Tarea del usuario

1. generar el código
2. entender el siguiente paso
3. reaccionar si el código vence

## Contrato de interacción

### Estructura mínima

- encabezado breve
- acción para generar código
- bloque central con código y vencimiento
- instrucción única hacia el bot

### Acción primaria

- `Generar código`

### Acción secundaria

- `Hacerlo después`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| idle | explica el beneficio y muestra la CTA principal | permite iniciar el flujo |
| code_generated | código visible con vencimiento y siguiente paso | guía hacia Telegram |
| expired | estado claro de vencimiento | permite regenerar |
| linked | confirmación de vínculo exitoso | cierra el flujo |
| error_retryable | error breve | permite reintentar |

## Contrato de copy

- titular aprobado: `Vincular Telegram`
- texto de apoyo aprobado: `Generá un código y envialo al bot para terminar el enlace.`
- acción primaria aprobada: `Generar código`
- error recuperable aprobado: `No pudimos generar este código. Probá de nuevo.`

## Aceptación

1. cada paso dice una sola acción siguiente
2. el vencimiento del código es visible pero no alarmista
3. la confirmación final deja claro que el vínculo ya quedó activo

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

---

**Estado:** `UXS` activo para `TG-001`.
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-TG-001.md` y `../UX-VALIDATION/UX-VALIDATION-TG-001.md`.
