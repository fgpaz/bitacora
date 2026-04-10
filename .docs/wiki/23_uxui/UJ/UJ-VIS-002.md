# UJ-VIS-002 — Journey de Dashboard multi-paciente del profesional

## Propósito

Este documento modela la tarea completa del slice `VIS-002`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIS-002.md`
- `../UXI/UXI-VIS-002.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-VIS-002.md`
- `../UXS/UXS-VIS-002.md`

## Goal del actor

leer un resumen útil de pacientes visibles sin ruido ni sensación de dominio total.

## Feeling global del journey

Este journey debe sentirse como:

- sobrio
- acotado
- priorizado
- responsable

Y no como:

- un panel de vigilancia

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Abrir dashboard | ve solo pacientes con acceso habilitado | límite claro | asumir que faltan datos sin explicación |
| S02 | Leer prioridades | revisa resumen y alertas básicas | sobriedad útil | encontrar dramatización o sobrecarga |
| S03 | Entrar a detalle | elige un paciente desde un resumen entendible | continuidad responsable | abrir datos sin saber por qué priorizar ese caso |

## Fricción aceptable

- paginación clara
- alertas básicas en tono factual
- ocultar silenciosamente lo no habilitado

## Fricción indebida

- lenguaje de monitorización continua
- códigos crípticos
- resúmenes sin jerarquía

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| sin pacientes visibles | muestra vacío claro y no error |
| patient_ref inválido en detalle | falla en cerrado sin exponer datos |
| error de carga | permite reintentar |

## Momentos sensibles

- la primera vista de la lista
- la lectura de alertas básicas
- el paso al detalle

## Paso crítico que requiere UXS

- paso crítico: `S02`
- nombre: Primera lectura del dashboard
- por qué baja a `UXS`: define si el tablero se percibe responsable o vigilante

## Defaults transferibles

- el journey debe conservar una sola dirección dominante por tramo;
- la explicitud sube solo donde cambia acceso, datos o consentimiento;
- la confirmación posterior al gesto principal debe ser breve y factual.

## Criterio de validación rápida

Este `UJ` está bien modelado si:

- puede contarse de principio a fin sin saltos arbitrarios;
- el paso crítico queda explícito;
- los errores relevantes no se confunden con caminos principales;
- la sensación global sigue siendo consistente con `UXI`.

---

**Estado:** `UJ` activo para `VIS-002`.
**Siguiente capa gobernada:** `../VOICE/VOICE-VIS-002.md` y `../UXS/UXS-VIS-002.md`.
