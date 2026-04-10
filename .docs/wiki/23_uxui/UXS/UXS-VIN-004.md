# UXS-VIN-004 — Cambio de acceso del profesional

## Propósito

Este documento fija el contrato UX del paso crítico del slice `VIN-004`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIN-004.md`
- `../UXI/UXI-VIN-004.md`
- `../UJ/UJ-VIN-004.md`
- `../VOICE/VOICE-VIN-004.md`
- `../../03_FL/FL-VIN-04.md`
- `../../06_pruebas/TP-VIN.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-VIN-004.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-VIN-004.md`

## Slice y paso dueño

- slice: `VIN-004`
- paso crítico: `S02` — Cambio del estado de acceso
- entrada: la persona ya está frente a un vínculo activo y necesita ajustar visibilidad
- salida correcta: el nuevo estado queda guardado y el efecto se entiende

## Sensación del paso

- sensación objetivo: control fino con efecto visible
- anti-sensación: mover un switch sin comprender consecuencias

## Tarea del usuario

1. leer el estado actual
2. decidir el cambio
3. guardar y confirmar el efecto

## Contrato de interacción

### Estructura mínima

- estado actual legible
- control principal de cambio
- área final de guardado o confirmación del auto-save

### Acción primaria

- `Guardar cambio`

### Acción secundaria

- `Dejar como está`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| default | estado actual visible | permite evaluar |
| dirty | nuevo estado seleccionado | pide confirmar si aplica |
| saving | feedback corto de guardado | evita inconsistencias |
| saved | nuevo estado visible con lenguaje simple | deja claro el efecto |
| error_retryable | error localizado | permite volver a intentar |

## Contrato de copy

- titular aprobado: `Gestionar acceso`
- texto de apoyo aprobado: `Podés decidir si este profesional puede ver tus registros.`
- acción primaria aprobada: `Guardar cambio`
- error recuperable aprobado: `No pudimos guardar este cambio. Probá de nuevo.`

## Aceptación

1. el estado actual se entiende antes de interactuar
2. la consecuencia del cambio queda explícita
3. la interfaz no usa lenguaje técnico interno

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

---

**Estado:** `UXS` activo para `VIN-004`.
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-VIN-004.md` y `../UX-VALIDATION/UX-VALIDATION-VIN-004.md`.
