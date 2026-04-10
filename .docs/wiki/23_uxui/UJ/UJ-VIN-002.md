# UJ-VIN-002 — Journey de Auto-vinculación paciente a profesional por código

## Propósito

Este documento modela la tarea completa del slice `VIN-002`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIN-002.md`
- `../UXI/UXI-VIN-002.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-VIN-002.md`
- `../UXS/UXS-VIN-002.md`

## Goal del actor

crear el vínculo con precisión sin ceder control sobre los datos.

## Feeling global del journey

Este journey debe sentirse como:

- preciso
- breve
- controlado
- claro
- seguro

Y no como:

- pegar un código sin saber qué activa

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Entrada al flujo | abre la acción de vincularse por código | claridad inmediata | sentir un proceso técnico opaco |
| S02 | Ingreso del código | pega o escribe el código y avanza | precisión simple | dudar si el código es correcto o qué hará |
| S03 | Confirmación del vínculo | ve el vínculo activo con control de acceso todavía en sus manos | cierre seguro | creer que ya comparte datos |

## Fricción aceptable

- mostrar un error corto si el código no sirve
- explicar que el vínculo no equivale a acceso

## Fricción indebida

- pedir información extra además del código
- usar texto técnico sobre binding
- confirmar sin explicar el resultado

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| código inválido | invita a pedir uno nuevo sin culpar |
| código expirado | explica que debe solicitar otro |
| vínculo ya existente | explica el estado actual y evita duplicación |

## Momentos sensibles

- la revisión antes de enviar el código
- la confirmación posterior al vínculo

## Paso crítico que requiere UXS

- paso crítico: `S02`
- nombre: Ingreso y validación del código
- por qué baja a `UXS`: ahí se concentra la confianza operacional del slice

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

**Estado:** `UJ` activo para `VIN-002`.
**Siguiente capa gobernada:** `../VOICE/VOICE-VIN-002.md` y `../UXS/UXS-VIN-002.md`.
