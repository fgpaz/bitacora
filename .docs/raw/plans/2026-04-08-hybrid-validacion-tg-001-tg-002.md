# Ola híbrida de validación UX — `TG-001` y `TG-002`

## Propósito

Este documento coordina la ejecución futura de la ola híbrida de validación UX sobre los slices de Telegram ya prototipados.

No reemplaza los briefs operativos por slice ni declara hallazgos observados. Su función es fijar cómo se agrupan las sesiones, qué cohorte participa, cómo se reparte la evidencia y qué artefactos legítimos salen después.

## Alcance de la ola

La ola cubre:

- `TG-001` — vinculación de cuenta Telegram
- `TG-002` — recordatorio y registro conversacional por Telegram

## Diseño de cohorte

### Cohorte G

- tamaño: `5 participantes`
- modalidad preferida: remota, `mobile-first`
- sesión compartida: `TG-001 -> TG-002`
- duración objetivo: `30 a 40 minutos`
- propósito:
  - validar si el puente web hacia Telegram se entiende sin fricción de setup;
  - validar si el recordatorio conversacional se siente opcional y breve;
  - aislar qué hallazgos pertenecen al pairing y cuáles a la respuesta en chat.

## Reglas operativas

- no reutilizar participantes de cohortes `A` a `F`;
- no escribir `UX-VALIDATION-*` hasta contar con evidencia observada suficiente;
- etiquetar cada hallazgo por slice:
  - `TG-001`
  - `TG-002`
- registrar hallazgos transversales por separado solo si afectan más de un slice y luego devolverlos al documento dueño correcto.

## Artefactos y carpetas de trabajo

### Briefs operativos dueños

- `.docs/raw/plans/2026-04-08-tg-001-validacion-operativa.md`
- `.docs/raw/plans/2026-04-08-tg-002-validacion-operativa.md`

### Estructura de evidencia

- `artifacts/ux-validation/2026-04-08-hybrid-tg-001-tg-002/`
  - `README.md`
  - `notes/session-template.md`
  - `notes/`
  - `evidence/`

## Salida legítima después de las sesiones

Una vez completada la cohorte y consolidada la evidencia, los siguientes artefactos legítimos son:

- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-TG-001.md`
- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-TG-002.md`

## Regla de retorno a docs dueños

- wording, framing o intensidad verbal -> `VOICE-*`
- jerarquía, estados, secuencia o comportamiento -> `UXS-*`
- patrón reusable de puente multicanal o respuesta conversacional -> `16_patrones_ui.md`

---

**Estado:** ola híbrida definida y lista para ejecución futura.
**Siguiente paso operativo legítimo:** correr `Cohorte G` con evidencia real.
