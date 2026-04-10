# UXI-VIN-001 — Intención de Emisión de invitación profesional a paciente

## Propósito

Este documento fija la intención emocional y operativa del slice `VIN-001`.

No describe todavía la journey completa ni el detalle de una pantalla. Su función es decidir cómo debe sentirse el caso para que `UJ`, `VOICE` y `UXS` no improvisen tono, ritmo o confianza.

## Relación con el canon

Este documento depende de:

- `../../10_manifiesto_marca_experiencia.md`
- `../../11_identidad_visual.md`
- `../../12_lineamientos_interfaz_visual.md`
- `../../13_voz_tono.md`
- `../UXR/UXR-VIN-001.md`

Y prepara directamente:

- `../UJ/UJ-VIN-001.md`
- `../VOICE/VOICE-VIN-001.md`
- `../UXS/UXS-VIN-001.md`

## Sensación deseada

- claro
- sobrio
- responsable
- no invasivo
- trazable

La intención de este caso es que la experiencia se sienta como `solicitud responsable`.

## Anti-sensación

La anti-sensación principal a evitar es:

**un alta de paciente estilo CRM o captación comercial.**

## Postura de confianza

aun en UI profesional, la propiedad del dato sigue del lado del paciente.

## Goal del actor

invitar sin prometer acceso ni perder trazabilidad del estado del vínculo.

## Fricción aceptable

- validar email y estados existentes
- mostrar pending invite o carelink invited con claridad

## Fricción indebida

- celebrar la emisión
- usar lenguaje de control clínico
- ocultar el estado resultante

## Defaults del caso

- hereda la base del sistema: casi nula fricción, seguridad implícita, simpleza radical, paciente primero cuando el actor principal es la persona usuaria, silencio útil fuera de los momentos sensibles, explicitud alta solo cuando cambian acceso, datos o consentimiento;
- la sensación dominante del slice es `claro`, pero sin sacrificar claridad sensible;
- la voz y la interfaz deben sostener una sola dirección dominante por paso.

## Criterio de validación rápida

Este `UXI` está bien calibrado si el caso se percibe como:

- claro
- sobrio
- responsable
- no invasivo
- trazable

Y está mal calibrado si se percibe como:

- un alta de paciente estilo CRM o captación comercial
- una experiencia que pide más esfuerzo que el valor que devuelve.

---

**Estado:** `UXI` activo para `VIN-001`.
**Siguiente capa gobernada:** `../UJ/UJ-VIN-001.md`, `../VOICE/VOICE-VIN-001.md` y `../UXS/UXS-VIN-001.md`.
