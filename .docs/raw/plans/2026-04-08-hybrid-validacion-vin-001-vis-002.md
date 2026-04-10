# Ola híbrida de validación UX — `VIN-001` y `VIS-002`

## Propósito

Este documento coordina la ejecución futura de la ola híbrida de validación UX sobre los slices profesionales ya prototipados.

No reemplaza los briefs operativos por slice ni declara hallazgos observados. Su función es fijar cómo se agrupan las sesiones, qué cohorte participa, cómo se reparte la evidencia y qué artefactos legítimos salen después.

## Alcance de la ola

La ola cubre:

- `VIN-001` — emisión de invitación profesional a paciente
- `VIS-002` — dashboard multi-paciente del profesional

## Diseño de cohorte

### Cohorte F

- tamaño: `5 participantes`
- modalidad preferida: remota, `desktop-first`
- sesión compartida: `VIN-001 -> VIS-002`
- duración objetivo: `35 a 45 minutos`
- propósito:
  - validar si la invitación se entiende como acto breve y responsable;
  - validar si el dashboard se lee como tablero sobrio y acotado;
  - aislar qué hallazgos pertenecen a la emisión de invitación y cuáles a la primera lectura del dashboard.

## Reglas operativas

- no reutilizar participantes de cohortes `A` a `E`;
- no escribir `UX-VALIDATION-*` hasta contar con evidencia observada suficiente;
- etiquetar cada hallazgo por slice:
  - `VIN-001`
  - `VIS-002`
- registrar hallazgos transversales por separado solo si afectan más de un slice y luego devolverlos al documento dueño correcto.

## Artefactos y carpetas de trabajo

### Briefs operativos dueños

- `.docs/raw/plans/2026-04-08-vin-001-validacion-operativa.md`
- `.docs/raw/plans/2026-04-08-vis-002-validacion-operativa.md`

### Estructura de evidencia

- `artifacts/ux-validation/2026-04-08-hybrid-vin-001-vis-002/`
  - `README.md`
  - `notes/session-template.md`
  - `notes/`
  - `evidence/`

## Salida legítima después de las sesiones

Una vez completada la cohorte y consolidada la evidencia, los siguientes artefactos legítimos son:

- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-001.md`
- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIS-002.md`

## Regla de retorno a docs dueños

- wording, framing o intensidad verbal -> `VOICE-*`
- jerarquía, estados, secuencia o comportamiento -> `UXS-*`
- patrón reusable de invitación o lectura profesional -> `16_patrones_ui.md`

---

**Estado:** ola híbrida definida y lista para ejecución futura.
**Siguiente paso operativo legítimo:** correr `Cohorte F` con evidencia real.
