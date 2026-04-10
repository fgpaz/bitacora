# UXS-EXP-001 — Solicitud de exportación CSV

## Propósito

Este documento fija el contrato UX del paso crítico del slice `EXP-001`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-EXP-001.md`
- `../UXI/UXI-EXP-001.md`
- `../UJ/UJ-EXP-001.md`
- `../VOICE/VOICE-EXP-001.md`
- `../../03_FL/FL-EXP-01.md`
- `../../06_pruebas/TP-EXP.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-EXP-001.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-EXP-001.md`

## Slice y paso dueño

- slice: `EXP-001`
- paso crítico: `S02` — Confirmación del alcance y disparo de descarga
- entrada: la persona ya decidió que quiere sacar una copia de sus registros
- salida correcta: la descarga empieza con feedback suficiente o el error queda claro

## Sensación del paso

- sensación objetivo: una salida simple y confiable
- anti-sensación: no saber si la descarga arrancó o qué incluye

## Tarea del usuario

1. revisar qué se descarga
2. confirmar el período
3. disparar la descarga

## Contrato de interacción

### Estructura mínima

- encabezado breve
- resumen de alcance
- selector simple de período si aplica
- CTA única de descarga

### Acción primaria

- `Descargar CSV`

### Acción secundaria

- `Cancelar`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| default | alcance visible y CTA disponible | permite decidir |
| generating | feedback corto de preparación | sostiene certeza mientras arranca |
| success_handoff | descarga iniciada o confirmada | cierra sin pantalla extra |
| error_retryable | error breve | permite reintentar |

## Contrato de copy

- titular aprobado: `Exportar tus registros`
- texto de apoyo aprobado: `Podés descargar un CSV con tus datos del período elegido.`
- acción primaria aprobada: `Descargar CSV`
- error recuperable aprobado: `No pudimos preparar este archivo. Probá de nuevo.`

## Aceptación

1. queda claro qué datos incluye el archivo
2. el inicio de la descarga tiene feedback suficiente
3. la interfaz no se llena de jerga técnica

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

---

**Estado:** `UXS` activo para `EXP-001`.
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-EXP-001.md` y `../UX-VALIDATION/UX-VALIDATION-EXP-001.md`.
