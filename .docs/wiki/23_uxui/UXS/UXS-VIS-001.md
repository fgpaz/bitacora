# UXS-VIS-001 — Lectura inicial del timeline

## Propósito

Este documento fija el contrato UX del paso crítico del slice `VIS-001`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIS-001.md`
- `../UXI/UXI-VIS-001.md`
- `../UJ/UJ-VIS-001.md`
- `../VOICE/VOICE-VIS-001.md`
- `../../03_FL/FL-VIS-01.md`
- `../../06_pruebas/TP-VIS.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-VIS-001.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-VIS-001.md`

## Slice y paso dueño

- slice: `VIS-001`
- paso crítico: `S02` — Lectura inicial y ajuste de período
- entrada: la persona abre su historial y necesita entender rápido qué está viendo
- salida correcta: puede leer el período actual o cambiarlo sin perderse

## Sensación del paso

- sensación objetivo: una lectura calma y utilizable
- anti-sensación: un gráfico frío que exige demasiada interpretación

## Tarea del usuario

1. ubicar el período actual
2. leer el gráfico base
3. cambiar período si hace falta

## Contrato de interacción

### Estructura mínima

- encabezado corto
- gráfico principal
- controles de período simples
- estado vacío claro cuando corresponda

### Acción primaria

- `Aplicar período`

### Acción secundaria

- `Restablecer`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| loading | skeleton o placeholder calmo | anticipa el gráfico sin ruido |
| ready | gráfico y filtros visibles | permite lectura y ajuste |
| empty | mensaje claro de ausencia de datos | evita confusión con error |
| error_retryable | error breve de carga | permite reintentar |

## Contrato de copy

- titular aprobado: `Tu timeline`
- texto de apoyo aprobado: `Revisá tus registros por período sin perder el hilo.`
- acción primaria aprobada: `Aplicar período`
- error recuperable aprobado: `No pudimos cargar este período. Probá de nuevo.`

## Aceptación

1. el gráfico principal aparece antes que controles accesorios
2. los filtros no interrumpen la lectura inicial
3. el estado vacío se distingue del error

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

---

**Estado:** `UXS` activo para `VIS-001`.
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-VIS-001.md` y `../UX-VALIDATION/UX-VALIDATION-VIS-001.md`.
