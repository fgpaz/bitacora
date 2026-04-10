# UJ-CON-002 — Journey de Revocación de consentimiento

## Propósito

Este documento modela la tarea completa del slice `CON-002`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-CON-002.md`
- `../UXI/UXI-CON-002.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-CON-002.md`
- `../UXS/UXS-CON-002.md`

## Goal del actor

revocar consentimiento entendiendo el efecto sobre registro y accesos.

## Feeling global del journey

Este journey debe sentirse como:

- serio
- sereno
- claro
- no punitivo

Y no como:

- una amenaza o castigo disfrazado de configuración

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Llegar al flujo | abre el ajuste de consentimiento | serenidad contenida | enfrentarse a una pantalla alarmista |
| S02 | Revisar impacto | entiende qué se suspende y qué accesos caen | claridad sensible | mezclar consecuencias sin jerarquía |
| S03 | Confirmar revocación | ejecuta la decisión y ve el nuevo estado | control explícito | quedarse sin certeza sobre el resultado |

## Fricción aceptable

- una pausa deliberada de mayor intensidad que otros formularios
- confirmación explícita de impacto

## Fricción indebida

- copy amenazante
- legalese sin traducción humana
- ocultar la cascada a vínculos

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| consentimiento ya revocado | muestra el estado actual sin repetir drama |
| error en cascade transaccional | falla en cerrado y evita estados parciales |
| sesión expirada | reautentica con claridad |

## Momentos sensibles

- la lectura del impacto
- la confirmación final de revocación

## Paso crítico que requiere UXS

- paso crítico: `S02`
- nombre: Revisión del impacto antes de revocar
- por qué baja a `UXS`: es el momento donde la seriedad debe seguir siendo legible y no punitiva

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

**Estado:** `UJ` activo para `CON-002`.
**Siguiente capa gobernada:** `../VOICE/VOICE-CON-002.md` y `../UXS/UXS-CON-002.md`.
