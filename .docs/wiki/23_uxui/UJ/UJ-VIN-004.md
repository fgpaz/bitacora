# UJ-VIN-004 — Journey de Gestión de acceso profesional por paciente

## Propósito

Este documento modela la tarea completa del slice `VIN-004`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIN-004.md`
- `../UXI/UXI-VIN-004.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-VIN-004.md`
- `../UXS/UXS-VIN-004.md`

## Goal del actor

cambiar acceso sin tener que revocar el vínculo entero.

## Feeling global del journey

Este journey debe sentirse como:

- granular
- legible
- reversible
- seguro

Y no como:

- un switch técnico opaco

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Leer el estado actual | ve si el acceso está activo o no | legibilidad inmediata | no entender el estado de partida |
| S02 | Decidir el cambio | activa o desactiva acceso con comprensión del efecto | control explícito | mover un switch sin saber qué hizo |
| S03 | Confirmación de efecto | ve el nuevo estado guardado | certeza tranquila | dudar si el profesional ya cambió de visibilidad |

## Fricción aceptable

- mostrar un estado guardado visible
- pedir guardar si el patrón no es auto-save

## Fricción indebida

- tecnicismos como can_view_data
- efectos ambiguos
- igualar acceso con vínculo

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| vínculo inactivo | explica que ese vínculo ya no admite cambios |
| ownership inválido | falla en cerrado |
| error al guardar | permite reintento y conserva el estado previo visible |

## Momentos sensibles

- la lectura del estado actual
- el momento del cambio
- la confirmación del nuevo estado

## Paso crítico que requiere UXS

- paso crítico: `S02`
- nombre: Cambio del estado de acceso
- por qué baja a `UXS`: allí se define si el control se entiende o se vuelve técnico

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

**Estado:** `UJ` activo para `VIN-004`.
**Siguiente capa gobernada:** `../VOICE/VOICE-VIN-004.md` y `../UXS/UXS-VIN-004.md`.
