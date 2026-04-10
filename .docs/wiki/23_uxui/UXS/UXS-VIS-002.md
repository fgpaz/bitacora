# UXS-VIS-002 — Primera lectura del dashboard

## Propósito

Este documento fija el contrato UX del paso crítico del slice `VIS-002`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIS-002.md`
- `../UXI/UXI-VIS-002.md`
- `../UJ/UJ-VIS-002.md`
- `../VOICE/VOICE-VIS-002.md`
- `../../03_FL/FL-VIS-02.md`
- `../../06_pruebas/TP-VIS.md`
- `../../06_pruebas/TP-SEC.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-VIS-002.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-VIS-002.md`

## Slice y paso dueño

- slice: `VIS-002`
- paso crítico: `S02` — Primera lectura del dashboard
- entrada: el profesional abre el tablero y necesita entender la jerarquía de la información
- salida correcta: puede priorizar una lectura sin perder de vista límites de acceso

## Sensación del paso

- sensación objetivo: un tablero sobrio y acotado
- anti-sensación: una pared de tarjetas o alertas dramáticas

## Tarea del usuario

1. reconocer quién aparece y por qué
2. leer resúmenes básicos
3. elegir a quién abrir en detalle

## Contrato de interacción

### Estructura mínima

- encabezado breve
- lista paginada de pacientes visibles
- resumen y alertas básicas por tarjeta
- acción clara hacia detalle

### Acción primaria

- `Ver resumen`

### Acción secundaria

- `Cambiar página`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| loading | skeleton o placeholder sobrio | anticipa contenido sin ruido |
| ready | lista y resúmenes visibles | permite priorizar lectura |
| empty | vacío claro para ausencia de pacientes visibles | evita confusión con error |
| error_retryable | error breve de carga | permite reintentar |

## Contrato de copy

- titular aprobado: `Dashboard de pacientes`
- texto de apoyo aprobado: `Solo aparecen personas con acceso activo a sus datos.`
- acción primaria aprobada: `Ver resumen`
- error recuperable aprobado: `No pudimos cargar este dashboard. Probá de nuevo.`

## Aceptación

1. queda explícito que solo se muestran pacientes con acceso habilitado
2. las alertas no dramatizan ni reemplazan la lectura clínica
3. la lista mantiene jerarquía aun con varios pacientes

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

---

**Estado:** `UXS` activo para `VIS-002`.
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-VIS-002.md` y `../UX-VALIDATION/UX-VALIDATION-VIS-002.md`.
