# Ola híbrida de validación UX — `VIN-003` y `CON-002`

## Propósito

Este documento coordina la ejecución de la ola híbrida de validación UX sobre los slices de revocación ya prototipados.

No reemplaza los briefs operativos por slice ni declara hallazgos observados. Su función es fijar cómo se agrupan las sesiones, qué cohorte participa, cómo se reparte la evidencia y qué artefactos legítimos salen después.

## Alcance de la ola

La ola cubre:

- `VIN-003` — revocación de vínculo por paciente
- `CON-002` — revocación de consentimiento

## Diseño de cohorte

### Cohorte D

- tamaño: `5 participantes`
- modalidad: remota, `mobile-first`
- sesión compartida: `VIN-003 -> CON-002`
- duración objetivo: `35 a 45 minutos`
- propósito:
  - validar si la revocación de vínculo se entiende como decisión firme sin dramatización;
  - observar si la revocación de consentimiento muestra una cascada clara sin tono punitivo;
  - aislar, dentro de la misma sesión, qué hallazgos pertenecen a `VIN-003` y cuáles a `CON-002`.

## Reglas operativas

- no reutilizar participantes de las cohortes `A`, `B` o `C`;
- no escribir `UX-VALIDATION-*` hasta contar con evidencia observada suficiente;
- etiquetar cada hallazgo por slice:
  - `VIN-003`
  - `CON-002`
- registrar hallazgos transversales por separado solo si afectan más de un slice y luego devolverlos al documento dueño correcto.

## Artefactos y carpetas de trabajo

### Briefs operativos dueños

- `.docs/raw/plans/2026-04-08-vin-003-validacion-operativa.md`
- `.docs/raw/plans/2026-04-08-con-002-validacion-operativa.md`

### Estructura de evidencia

- `artifacts/ux-validation/2026-04-08-hybrid-vin-003-con-002/`
  - `README.md`
  - `notes/session-template.md`
  - `notes/`
  - `evidence/`

## Salida legítima después de las sesiones

Una vez completada la cohorte y consolidada la evidencia, los siguientes artefactos legítimos son:

- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-003.md`
- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-CON-002.md`

## Regla de retorno a docs dueños

- wording, framing o intensidad verbal -> `VOICE-*`
- jerarquía, estados, secuencia o comportamiento -> `UXS-*`
- patrón reusable de revocación, control o consentimiento -> `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md` o `16_patrones_ui.md`

---

**Estado:** ola híbrida definida y lista para ejecución.
**Siguiente paso operativo:** correr `Cohorte D` con evidencia real.
