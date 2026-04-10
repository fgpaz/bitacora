# UXI-VIN-004 — Intención de Gestión de acceso profesional por paciente

## Propósito

Este documento fija la intención emocional y operativa del slice `VIN-004`.

No describe todavía la journey completa ni el detalle de una pantalla. Su función es decidir cómo debe sentirse el caso para que `UJ`, `VOICE` y `UXS` no improvisen tono, ritmo o confianza.

## Relación con el canon

Este documento depende de:

- `../../10_manifiesto_marca_experiencia.md`
- `../../11_identidad_visual.md`
- `../../12_lineamientos_interfaz_visual.md`
- `../../13_voz_tono.md`
- `../UXR/UXR-VIN-004.md`

Y prepara directamente:

- `../UJ/UJ-VIN-004.md`
- `../VOICE/VOICE-VIN-004.md`
- `../UXS/UXS-VIN-004.md`

## Sensación deseada

- granular
- legible
- reversible
- seguro

La intención de este caso es que la experiencia se sienta como `control fino`.

## Anti-sensación

La anti-sensación principal a evitar es:

**un switch técnico opaco.**

## Postura de confianza

la interfaz vuelve visible que el paciente decide sobre la visibilidad y no sobre la relación completa.

## Goal del actor

cambiar acceso sin tener que revocar el vínculo entero.

## Fricción aceptable

- mostrar un estado guardado visible
- pedir guardar si el patrón no es auto-save

## Fricción indebida

- tecnicismos como can_view_data
- efectos ambiguos
- igualar acceso con vínculo

## Defaults del caso

- hereda la base del sistema: casi nula fricción, seguridad implícita, simpleza radical, paciente primero cuando el actor principal es la persona usuaria, silencio útil fuera de los momentos sensibles, explicitud alta solo cuando cambian acceso, datos o consentimiento;
- la sensación dominante del slice es `granular`, pero sin sacrificar claridad sensible;
- la voz y la interfaz deben sostener una sola dirección dominante por paso.

## Criterio de validación rápida

Este `UXI` está bien calibrado si el caso se percibe como:

- granular
- legible
- reversible
- seguro

Y está mal calibrado si se percibe como:

- un switch técnico opaco
- una experiencia que pide más esfuerzo que el valor que devuelve.

---

**Estado:** `UXI` activo para `VIN-004`.
**Siguiente capa gobernada:** `../UJ/UJ-VIN-004.md`, `../VOICE/VOICE-VIN-004.md` y `../UXS/UXS-VIN-004.md`.
