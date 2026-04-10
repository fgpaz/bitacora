# UXI-VIN-002 — Intención de Auto-vinculación paciente a profesional por código

## Propósito

Este documento fija la intención emocional y operativa del slice `VIN-002`.

No describe todavía la journey completa ni el detalle de una pantalla. Su función es decidir cómo debe sentirse el caso para que `UJ`, `VOICE` y `UXS` no improvisen tono, ritmo o confianza.

## Relación con el canon

Este documento depende de:

- `../../10_manifiesto_marca_experiencia.md`
- `../../11_identidad_visual.md`
- `../../12_lineamientos_interfaz_visual.md`
- `../../13_voz_tono.md`
- `../UXR/UXR-VIN-002.md`

Y prepara directamente:

- `../UJ/UJ-VIN-002.md`
- `../VOICE/VOICE-VIN-002.md`
- `../UXS/UXS-VIN-002.md`

## Sensación deseada

- preciso
- breve
- controlado
- claro
- seguro

La intención de este caso es que la experiencia se sienta como `puente breve y seguro`.

## Anti-sensación

La anti-sensación principal a evitar es:

**pegar un código sin saber qué activa.**

## Postura de confianza

el control explícito aparece justo después de resolver el código.

## Goal del actor

crear el vínculo con precisión sin ceder control sobre los datos.

## Fricción aceptable

- mostrar un error corto si el código no sirve
- explicar que el vínculo no equivale a acceso

## Fricción indebida

- pedir información extra además del código
- usar texto técnico sobre binding
- confirmar sin explicar el resultado

## Defaults del caso

- hereda la base del sistema: casi nula fricción, seguridad implícita, simpleza radical, paciente primero cuando el actor principal es la persona usuaria, silencio útil fuera de los momentos sensibles, explicitud alta solo cuando cambian acceso, datos o consentimiento;
- la sensación dominante del slice es `preciso`, pero sin sacrificar claridad sensible;
- la voz y la interfaz deben sostener una sola dirección dominante por paso.

## Criterio de validación rápida

Este `UXI` está bien calibrado si el caso se percibe como:

- preciso
- breve
- controlado
- claro
- seguro

Y está mal calibrado si se percibe como:

- pegar un código sin saber qué activa
- una experiencia que pide más esfuerzo que el valor que devuelve.

---

**Estado:** `UXI` activo para `VIN-002`.
**Siguiente capa gobernada:** `../UJ/UJ-VIN-002.md`, `../VOICE/VOICE-VIN-002.md` y `../UXS/UXS-VIN-002.md`.
