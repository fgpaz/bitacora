# UJ-EXP-001 — Journey de Exportación CSV del paciente

## Propósito

Este documento modela la tarea completa del slice `EXP-001`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-EXP-001.md`
- `../UXI/UXI-EXP-001.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-EXP-001.md`
- `../UXS/UXS-EXP-001.md`

## Goal del actor

descargar sus registros sin pelear con el sistema.

## Feeling global del journey

Este journey debe sentirse como:

- directo
- confiable
- controlado
- simple

Y no como:

- una descarga opaca o técnica

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Abrir exportación | ve qué formato y período puede descargar | claridad inmediata | no saber qué va a salir del sistema |
| S02 | Confirmar alcance | elige o confirma período y dispara la descarga | control simple | dudar del resultado o del tiempo |
| S03 | Reconocer inicio | la interfaz deja claro que la descarga empezó o falló | certeza tranquila | quedarse esperando sin feedback |

## Fricción aceptable

- elegir período base
- mostrar feedback mientras se prepara la respuesta

## Fricción indebida

- explicar cifrado y descifrado en UI normal
- forzar pasos accesorios
- usar jerga legal

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| sin registros | permite descargar headers o explica el resultado |
| error de generación | mensaje claro y reintento |
| sesión expirada | reautenticación con continuidad |

## Momentos sensibles

- la lectura del alcance del archivo
- el momento en que arranca la descarga

## Paso crítico que requiere UXS

- paso crítico: `S02`
- nombre: Confirmación del alcance y disparo de descarga
- por qué baja a `UXS`: es donde la exportación puede sentirse clara o técnica

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

**Estado:** `UJ` activo para `EXP-001`.
**Siguiente capa gobernada:** `../VOICE/VOICE-EXP-001.md` y `../UXS/UXS-EXP-001.md`.
