# UJ-VIN-003 — Journey de Revocación de vínculo por paciente

## Propósito

Este documento modela la tarea completa del slice `VIN-003`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIN-003.md`
- `../UXI/UXI-VIN-003.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-VIN-003.md`
- `../UXS/UXS-VIN-003.md`

## Goal del actor

suspender el acceso de un profesional específico sin ruido innecesario.

## Feeling global del journey

Este journey debe sentirse como:

- sereno
- firme
- claro
- bajo control

Y no como:

- romper algo sin entender impacto

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Entrada al flujo | identifica el vínculo que quiere revocar | claridad localizada | confundir el vínculo correcto |
| S02 | Confirmación | revisa el impacto y decide | firmeza serena | sentirse culpable o presionada |
| S03 | Estado resultante | ve que el acceso se cortó | certeza inmediata | quedarse dudando si ya no puede ver sus datos |

## Fricción aceptable

- una confirmación explícita
- explicar que el acceso cae de inmediato

## Fricción indebida

- copy culpabilizante
- igualar vínculo con consentimiento
- mostrar consecuencias vagas

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| vínculo ya revocado | muestra el estado actual sin dramatizar |
| ownership inválido | falla en cerrado sin exponer información |
| error de revocación | permite reintento con contexto |

## Momentos sensibles

- el paso de confirmación
- la evidencia de revocación exitosa

## Paso crítico que requiere UXS

- paso crítico: `S02`
- nombre: Confirmación de revocación del vínculo
- por qué baja a `UXS`: concentra claridad, impacto y tono del caso

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

**Estado:** `UJ` activo para `VIN-003`.
**Siguiente capa gobernada:** `../VOICE/VOICE-VIN-003.md` y `../UXS/UXS-VIN-003.md`.
