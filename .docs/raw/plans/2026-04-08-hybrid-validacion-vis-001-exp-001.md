# Ola híbrida de validación UX — `VIS-001` y `EXP-001`

## Propósito

Este documento coordina la ejecución de la ola híbrida de validación UX sobre los slices de lectura y salida de datos ya prototipados.

No reemplaza los briefs operativos por slice ni declara hallazgos observados. Su función es fijar cómo se agrupan las sesiones, qué cohorte participa, cómo se reparte la evidencia y qué artefactos legítimos salen después.

## Alcance de la ola

La ola cubre:

- `VIS-001` — timeline longitudinal del paciente
- `EXP-001` — exportación CSV del paciente

## Diseño de cohorte

### Cohorte E

- tamaño: `5 participantes`
- modalidad: remota, `mobile-first`
- sesión compartida: `VIS-001 -> EXP-001`
- duración objetivo: `35 a 45 minutos`
- propósito:
  - validar si el timeline se siente legible, calmo y propio;
  - observar si la exportación se entiende como salida simple y no técnica;
  - aislar, dentro de la misma sesión, qué hallazgos pertenecen a `VIS-001` y cuáles a `EXP-001`.

## Reglas operativas

- no reutilizar participantes de las cohortes `A`, `B`, `C` o `D`;
- no escribir `UX-VALIDATION-*` hasta contar con evidencia observada suficiente;
- etiquetar cada hallazgo por slice:
  - `VIS-001`
  - `EXP-001`
- registrar hallazgos transversales por separado solo si afectan más de un slice y luego devolverlos al documento dueño correcto.

## Artefactos y carpetas de trabajo

### Briefs operativos dueños

- `.docs/raw/plans/2026-04-08-vis-001-validacion-operativa.md`
- `.docs/raw/plans/2026-04-08-exp-001-validacion-operativa.md`

### Estructura de evidencia

- `artifacts/ux-validation/2026-04-08-hybrid-vis-001-exp-001/`
  - `README.md`
  - `notes/session-template.md`
  - `notes/`
  - `evidence/`

## Salida legítima después de las sesiones

Una vez completada la cohorte y consolidada la evidencia, los siguientes artefactos legítimos son:

- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIS-001.md`
- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-EXP-001.md`

## Regla de retorno a docs dueños

- wording, framing o intensidad verbal -> `VOICE-*`
- jerarquía, estados, secuencia o comportamiento -> `UXS-*`
- patrón reusable de lectura longitudinal o salida de datos -> `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md` o `16_patrones_ui.md`

---

**Estado:** ola híbrida definida y lista para ejecución.
**Siguiente paso operativo:** correr `Cohorte E` con evidencia real.
