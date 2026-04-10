# Ola híbrida de validación UX — `VIN-002` y `VIN-004`

## Propósito

Este documento coordina la ejecución de la ola híbrida de validación UX sobre los slices de vínculo y acceso ya prototipados.

No reemplaza los briefs operativos por slice ni declara hallazgos observados. Su función es fijar cómo se agrupan las sesiones, qué cohorte participa, cómo se reparte la evidencia y qué artefactos legítimos salen después.

## Alcance de la ola

La ola cubre:

- `VIN-002` — auto-vinculación paciente a profesional por código
- `VIN-004` — gestión de acceso profesional por paciente

## Diseño de cohorte

### Cohorte C

- tamaño: `5 participantes`
- modalidad: remota, `mobile-first`
- sesión compartida: `VIN-002 -> VIN-004`
- duración objetivo: `35 a 45 minutos`
- propósito:
  - validar si el vínculo por código se entiende como gesto preciso y no técnico;
  - observar si la transición desde vínculo activo hasta gestión de acceso conserva control explícito;
  - aislar, dentro de la misma sesión, qué hallazgos pertenecen a `VIN-002` y cuáles a `VIN-004`.

## Reglas operativas

- no reutilizar participantes de la ola `ONB-001 / REG-001 / REG-002`;
- no escribir `UX-VALIDATION-*` hasta contar con evidencia observada suficiente;
- etiquetar cada hallazgo por slice:
  - `VIN-002`
  - `VIN-004`
- registrar hallazgos transversales por separado solo si afectan más de un slice y luego devolverlos al documento dueño correcto.

## Artefactos y carpetas de trabajo

### Briefs operativos dueños

- `.docs/raw/plans/2026-04-08-vin-002-validacion-operativa.md`
- `.docs/raw/plans/2026-04-08-vin-004-validacion-operativa.md`

### Estructura de evidencia

- `artifacts/ux-validation/2026-04-08-hybrid-vin-002-vin-004/`
  - `README.md`
  - `notes/session-template.md`
  - `notes/`
  - `evidence/`

## Salida legítima después de las sesiones

Una vez completada la cohorte y consolidada la evidencia, los siguientes artefactos legítimos son:

- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-002.md`
- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-004.md`

## Regla de retorno a docs dueños

- wording, framing o intensidad verbal -> `VOICE-*`
- jerarquía, estados, secuencia o comportamiento -> `UXS-*`
- patrón reusable de control de acceso o de vínculo paciente-profesional -> `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md` o `16_patrones_ui.md`

---

**Estado:** ola híbrida definida y lista para ejecución.
**Siguiente paso operativo:** correr `Cohorte C` con evidencia real.
