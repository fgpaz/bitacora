# UJ-VIN-001 — Journey de Emisión de invitación profesional a paciente

## Propósito

Este documento modela la tarea completa del slice `VIN-001`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIN-001.md`
- `../UXI/UXI-VIN-001.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-VIN-001.md`
- `../UXS/UXS-VIN-001.md`

## Goal del actor

invitar sin prometer acceso ni perder trazabilidad del estado del vínculo.

## Feeling global del journey

Este journey debe sentirse como:

- claro
- sobrio
- responsable
- no invasivo
- trazable

Y no como:

- un alta de paciente estilo CRM o captación comercial

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Abrir el flujo | ingresa al formulario de invitación | foco simple | entrar a un flujo administrativo demasiado pesado |
| S02 | Revisar y emitir | carga email y entiende el alcance real de la invitación | claridad responsable | creer que la acción ya habilita acceso |
| S03 | Ver estado pendiente | recibe confirmación de envío o creación del estado pendiente | cierre trazable | quedarse sin entender qué sigue |

## Fricción aceptable

- validar email y estados existentes
- mostrar pending invite o carelink invited con claridad

## Fricción indebida

- celebrar la emisión
- usar lenguaje de control clínico
- ocultar el estado resultante

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| email inválido | se corrige sin dramatizar |
| vínculo ya activo | explica que no hace falta reinvitar |
| invitación expirada previa | orienta a emitir una nueva |

## Momentos sensibles

- la revisión antes de enviar
- la confirmación del estado pendiente

## Paso crítico que requiere UXS

- paso crítico: `S02`
- nombre: Revisión y emisión de invitación
- por qué baja a `UXS`: allí se define si la UI suena a solicitud responsable o a acceso automático

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

**Estado:** `UJ` activo para `VIN-001`.
**Siguiente capa gobernada:** `../VOICE/VOICE-VIN-001.md` y `../UXS/UXS-VIN-001.md`.
