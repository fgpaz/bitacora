# UJ-VIS-001 — Journey de Timeline longitudinal del paciente

## Propósito

Este documento modela la tarea completa del slice `VIS-001`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIS-001.md`
- `../UXI/UXI-VIS-001.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-VIS-001.md`
- `../UXS/UXS-VIS-001.md`

## Goal del actor

leer el patrón de sus registros sin necesitar interpretación externa.

## Feeling global del journey

Este journey debe sentirse como:

- legible
- calmo
- útil
- propio

Y no como:

- un panel analítico frío

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Abrir timeline | ve sus registros en un período base | valor visible rápido | encontrar primero filtros y después el dato |
| S02 | Leer y ajustar período | usa filtros simples para cambiar la lectura | control tranquilo | sentir que hay demasiada configuración |
| S03 | Entender estado vacío o con datos | sale sabiendo qué significa lo que ve | claridad estable | confundir vacío con error |

## Fricción aceptable

- filtros base de período
- estado vacío explícito

## Fricción indebida

- jerga analítica
- interpretaciones clínicas automáticas
- sobrecarga de controles

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| sin datos en el período | muestra un vacío útil y orientado |
| período demasiado largo | aplica paginación o recorte claro |
| fallo de carga | permite reintento sin perder el encuadre |

## Momentos sensibles

- la primera lectura del gráfico
- el ajuste del período
- el estado vacío

## Paso crítico que requiere UXS

- paso crítico: `S02`
- nombre: Lectura inicial y ajuste de período
- por qué baja a `UXS`: ahí se define si el timeline se siente legible o técnico

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

**Estado:** `UJ` activo para `VIS-001`.
**Siguiente capa gobernada:** `../VOICE/VOICE-VIS-001.md` y `../UXS/UXS-VIS-001.md`.
