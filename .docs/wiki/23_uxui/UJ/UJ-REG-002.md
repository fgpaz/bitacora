# UJ-REG-002 — Journey de Registro de factores diarios vía web

## Propósito

Este documento modela la tarea completa del slice `REG-002`.

No reemplaza la voz ni la spec crítica. Su función es traducir `UXR` y `UXI` a un recorrido vivido con ritmo, puntos sensibles, errores relevantes y un paso crítico dueño de `UXS`.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-REG-002.md`
- `../UXI/UXI-REG-002.md`
- `../../13_voz_tono.md`

Y prepara directamente:

- `../VOICE/VOICE-REG-002.md`
- `../UXS/UXS-REG-002.md`

## Goal del actor

registrar factores del día sin sentir que completa un examen.

## Feeling global del journey

Este journey debe sentirse como:

- compacto
- ordenado
- comprensible
- sin juicio
- útil

Y no como:

- una encuesta clínica larga y moralizante

## Main path

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| S01 | Entrada al check-in | entiende rápido qué va a registrar hoy | claridad ligera | sentir una pared de campos |
| S02 | Completar bloques | recorre factores agrupados con lógica simple | avance sostenido | perderse entre preguntas heterogéneas |
| S03 | Guardar y cerrar | envía el check-in y recibe confirmación breve | cierre ordenado | dudar si valió la pena completar el formulario |

## Fricción aceptable

- mostrar un bloque condicional solo cuando corresponde
- feedback breve al guardar

## Fricción indebida

- mostrar todos los campos de una vez
- agregar texto clínico explicativo
- pedir precisión innecesaria

## Variant / error path

| Condición | Respuesta esperada |
| --- | --- |
| valor fuera de rango | explica el problema en lenguaje simple y mantiene el resto del formulario |
| sesión expirada | redirige sin culpar y preserva la dignidad del flujo |
| fallo de guardado | permite reintento sin reiniciar toda la carga |

## Momentos sensibles

- el primer escaneo del formulario
- la aparición de medicación como bloque condicional
- el submit final

## Paso crítico que requiere UXS

- paso crítico: `S03`
- nombre: Revisión final y envío del check-in
- por qué baja a `UXS`: es el punto donde el slice puede volverse pesado o seguir sintiéndose breve

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

**Estado:** `UJ` activo para `REG-002`.
**Siguiente capa gobernada:** `../VOICE/VOICE-REG-002.md` y `../UXS/UXS-REG-002.md`.
