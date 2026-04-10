# Ola híbrida de validación UX — `ONB-001`, `REG-001` y `REG-002`

## Propósito

Este documento coordina la ejecución de la primera ola híbrida de validación UX sobre los slices ya prototipados.

No reemplaza los briefs operativos por slice ni declara hallazgos observados. Su función es fijar cómo se agrupan las sesiones, qué cohortes participan, cómo se reparte la evidencia y qué artefactos legítimos salen después.

## Alcance de la ola

La ola cubre:

- `ONB-001` — onboarding invitado del paciente hasta primer `MoodEntry`
- `REG-001` — registro rápido de humor vía web
- `REG-002` — registro de factores diarios vía web

## Diseño de cohortes

### Cohorte A

- tamaño: `5 participantes`
- modalidad: remota, `mobile-first`
- sesión compartida: `ONB-001 -> REG-001`
- duración objetivo: `35 a 45 minutos`
- propósito:
  - validar continuidad natural entre onboarding y primer gesto de registro;
  - observar si el paso de consentimiento al primer registro conserva baja fricción;
  - aislar, dentro de la misma sesión, qué hallazgos pertenecen a `ONB-001` y cuáles a `REG-001`.

### Cohorte B

- tamaño: `5 participantes nuevos`
- modalidad: remota, `mobile-first`
- sesión dedicada: `REG-002`
- duración objetivo: `25 a 35 minutos`
- propósito:
  - observar carga cognitiva del check-in diario sin arrastre de onboarding;
  - medir claridad del bloque condicional de medicación;
  - validar que el guardado siga siendo liviano aunque el slice tenga más densidad que `REG-001`.

## Reglas operativas

- no reutilizar participantes entre `Cohorte A` y `Cohorte B`;
- no escribir `UX-VALIDATION-*` hasta contar con evidencia observada suficiente;
- en `Cohorte A`, etiquetar cada hallazgo por slice:
  - `ONB-001`
  - `REG-001`
- registrar hallazgos transversales por separado solo si afectan más de un slice y luego devolverlos al documento dueño correcto.

## Artefactos y carpetas de trabajo

### Briefs operativos dueños

- `.docs/raw/plans/2026-04-08-onb-001-validacion-operativa.md`
- `.docs/raw/plans/2026-04-08-reg-001-validacion-operativa.md`
- `.docs/raw/plans/2026-04-08-reg-002-validacion-operativa.md`

### Estructura de evidencia

- `artifacts/ux-validation/2026-04-08-hybrid-onb-reg-001/`
  - `README.md`
  - `notes/session-template.md`
  - `notes/`
  - `evidence/`
- `artifacts/ux-validation/2026-04-08-reg-002/`
  - `README.md`
  - `notes/session-template.md`
  - `notes/`
  - `evidence/`

## Salida legítima después de las sesiones

Una vez completadas las dos cohortes y consolidada la evidencia, los siguientes artefactos legítimos son:

- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-ONB-001.md`
- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-REG-001.md`
- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-REG-002.md`

## Regla de retorno a docs dueños

- wording, framing o intensidad verbal -> `VOICE-*`
- jerarquía, estados, secuencia o comportamiento -> `UXS-*`
- patrón reusable de onboarding o formularios livianos -> `12_lineamientos_interfaz_visual.md` o `13_voz_tono.md`

---

**Estado:** ola híbrida definida y lista para ejecución.
**Siguiente paso operativo:** correr `Cohorte A` y `Cohorte B` con evidencia real.
