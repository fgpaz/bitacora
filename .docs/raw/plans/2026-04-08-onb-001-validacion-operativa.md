# ONB-001 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `ONB-001`.

No reemplaza `UX-VALIDATION-ONB-001.md` ni declara findings ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `ONB-001`
- caso: onboarding invitado del paciente hasta primer `MoodEntry`
- ola operativa actual: `Cohorte A`, sesión compartida con `REG-001`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-ONB-001.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-ONB-001.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-ONB-001.md`
  - `.docs/wiki/23_uxui/UXS/UXS-ONB-001.md`

## Hipótesis bajo test

1. La invitación se lee como contexto y no como dominio del profesional.
2. La persona entiende que Bitácora sigue siendo su espacio.
3. El consentimiento se vive como pausa breve y no como trámite legal pesado.
4. La persona entiende que aceptar no activa acceso automático del profesional.
5. La transición `S02 -> S03 -> S04` se siente inmediata.
6. La confirmación posterior al primer registro es serena y factual.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco mobile-first
- duración objetivo: 30 a 40 minutos
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `REG-001`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales por momento sensible
  - capturas de pantalla si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas que podrían llegar a Bitácora desde una invitación profesional;
- sin uso previo de Bitácora;
- con comodidad básica en flujos móviles de autenticación y formularios breves;
- preferentemente con contexto real o plausible de acompañamiento psicológico o psiquiátrico.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades sin relación plausible con el caso;
- personas que ya conozcan el flujo y solo “aprueben” la interfaz.

### Fallback aceptable

Si el target real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad del producto, no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Abrí la invitación y contame qué entendés que es este espacio.
2. Seguí como si quisieras empezar hoy.
3. Al llegar al consentimiento, explicame con tus palabras qué cambia y qué no cambia si seguís.
4. Completá el primer registro.
5. Al final, contame qué pensás que puede ver tu profesional y qué sentiste del recorrido.

### Preguntas de sondeo

- ¿En qué momento sentiste que el espacio era tuyo o ajeno?
- ¿Hubo algo que se sintiera trámite?
- ¿Qué te hizo pensar que seguías teniendo control?
- ¿Qué parte te resultó más clara y cuál menos?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` apertura | la invitación se entiende como contexto |  |  | `VOICE-ONB-001` o `UXS-ONB-001` |  |
| `S02` auth | continuidad sin sensación de trámite |  |  | `UXS-ONB-001` |  |
| `S03` consentimiento | control explícito sin carga pesada |  |  | `VOICE-ONB-001` o `UXS-ONB-001` |  |
| `S04` primer mood | valor inmediato |  |  | `UXS-ONB-001` |  |
| `S05` confirmación | cierre sereno y factual |  |  | `VOICE-ONB-001` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide completar la tarea o rompe comprensión de control, consentimiento o acceso;
- mayor: no bloquea a todas las personas, pero genera fricción relevante o lectura errónea repetida;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-onb-reg-001/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-onb-reg-001/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-ONB-001.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-ONB-001.md`
- jerarquía, estados, secuencia o interacción -> `UXS-ONB-001.md`
- si el hallazgo cambia un patrón reusable de onboarding y formularios -> revisar además `12_lineamientos_interfaz_visual.md` o `13_voz_tono.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-ONB-001.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `ONB-001`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-ONB-001.md` una vez exista evidencia observada.
