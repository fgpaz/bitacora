# UXS-VIN-003 — Confirmación de revocación del vínculo

## Propósito

Este documento fija el contrato UX del paso crítico del slice `VIN-003`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIN-003.md`
- `../UXI/UXI-VIN-003.md`
- `../UJ/UJ-VIN-003.md`
- `../VOICE/VOICE-VIN-003.md`
- `../../03_FL/FL-VIN-03.md`
- `../../06_pruebas/TP-VIN.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-VIN-003.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-VIN-003.md`

## Slice y paso dueño

- slice: `VIN-003`
- paso crítico: `S02` — Confirmación de revocación del vínculo
- entrada: la persona ya eligió el vínculo correcto y necesita tomar una decisión consciente
- salida correcta: el acceso del profesional queda cortado y la interfaz lo confirma

## Sensación del paso

- sensación objetivo: una decisión firme y serena
- anti-sensación: una pantalla que mete miedo o culpa

## Tarea del usuario

1. entender el impacto
2. confirmar la revocación
3. salir con certeza sobre el resultado

## Contrato de interacción

### Estructura mínima

- encabezado breve
- explicación concreta del impacto
- acciones primaria y secundaria claramente asimétricas

### Acción primaria

- `Revocar vínculo`

### Acción secundaria

- `Conservar vínculo`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| default | impacto visible y acción principal disponible | permite decidir |
| submitting | feedback corto de proceso | evita doble submit |
| success | confirmación clara de acceso cortado | cierra sin tono celebratorio |
| error_retryable | mensaje corto de fallo | permite reintentar |

## Contrato de copy

- titular aprobado: `Revocar vínculo`
- texto de apoyo aprobado: `Si seguís, este profesional pierde acceso inmediato a tus datos.`
- acción primaria aprobada: `Revocar vínculo`
- error recuperable aprobado: `No pudimos revocar este vínculo. Probá de nuevo.`

## Aceptación

1. el impacto queda claro antes de confirmar
2. la acción secundaria no compite visualmente con la primaria
3. el resultado confirma acceso cortado sin dramatizar

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

---

**Estado:** `UXS` activo para `VIN-003`.
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-VIN-003.md` y `../UX-VALIDATION/UX-VALIDATION-VIN-003.md`.
