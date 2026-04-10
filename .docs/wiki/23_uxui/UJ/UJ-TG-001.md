# UJ-TG-001 — Journey de Vinculación de cuenta Telegram

## Propósito

Este documento modela la tarea completa del slice `TG-001`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-TG-001.md`
- `../UXI/UXI-TG-001.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-TG-001.md`
- `../UXS/UXS-TG-001.md`

## Goal del actor

conectar la cuenta web con Telegram sin perderse entre canales.

## Feeling global del journey

Este journey debe sentirse como:

- rápido
- guiado
- simple
- seguro

Y no como:

- una configuración técnica

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Generar código | obtiene un código temporal con una instrucción concreta | claridad paso a paso | quedarse sin saber cuál es el siguiente movimiento |
| S02 | Ir al bot | lleva el código a Telegram | continuidad entre canales | sentir corte entre web y bot |
| S03 | Confirmar vínculo | recibe confirmación de enlace exitoso | cierre nítido | dudar si la cuenta ya quedó vinculada |

## Fricción aceptable

- mostrar vencimiento del código
- ofrecer regenerar si expiró

## Fricción indebida

- explicar internals del bot
- dar muchas instrucciones a la vez
- usar jerga de pairing

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| código expirado | ofrece generar uno nuevo |
| código inválido | explica el problema sin tecnicismos |
| chat ya vinculado | muestra una salida clara sin ambigüedad |

## Momentos sensibles

- la generación del código
- la transición al bot
- la confirmación final

## Paso crítico que requiere UXS

- paso crítico: `S01`
- nombre: Generación y handoff del código
- por qué baja a `UXS`: es el punto que define si el puente entre web y Telegram se entiende

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

**Estado:** `UJ` activo para `TG-001`.
**Siguiente capa gobernada:** `../VOICE/VOICE-TG-001.md` y `../UXS/UXS-TG-001.md`.
