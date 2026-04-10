# UJ-REG-001 — Journey de Registro rápido de humor vía web

## Propósito

Este documento modela la tarea completa del slice `REG-001`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-REG-001.md`
- `../UXI/UXI-REG-001.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-REG-001.md`
- `../UXS/UXS-REG-001.md`

## Goal del actor

registrar cómo se siente ahora sin romper el ritmo del día.

## Feeling global del journey

Este journey debe sentirse como:

- instantáneo
- liviano
- propio
- seguro
- sin juicio

Y no como:

- un mini formulario emocional que demora más de lo que ayuda

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Entrada al registro | abre el registro desde un punto ya contextualizado | continuidad inmediata | sentir que entró a un flujo más largo de lo esperado |
| S02 | Selección del valor | elige un valor de la escala y dispara el guardado | respuesta instantánea | dudar si el gesto realmente guardó el dato |
| S03 | Confirmación breve | ve que el registro quedó guardado y puede seguir | cierre factual | quedarse sin certeza o con ruido extra |

## Fricción aceptable

- una confirmación corta de guardado
- reintento digno si la sesión expiró

## Fricción indebida

- copy explicativo antes de la escala
- pedir campos extra sin valor inmediato
- tono terapéutico o motivacional

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| sesión expirada | pide reingresar sin culpar ni resetear el contexto más de lo necesario |
| consentimiento faltante o revocado | redirige con claridad al paso correspondiente |
| error de envío | permite reintento breve sin borrar la intención de registrar |

## Momentos sensibles

- la primera lectura de la escala
- la confirmación de que el tap sí se guardó

## Paso crítico que requiere UXS

- paso crítico: `S02`
- nombre: Selección y confirmación inmediata del humor
- por qué baja a `UXS`: concentra la promesa de baja fricción del slice

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

**Estado:** `UJ` activo para `REG-001`.
**Siguiente capa gobernada:** `../VOICE/VOICE-REG-001.md` y `../UXS/UXS-REG-001.md`.
