# VIN-001 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `VIN-001`.

No reemplaza `UX-VALIDATION-VIN-001.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `VIN-001`
- caso: emisión de invitación profesional a paciente
- ola operativa actual: `Cohorte F`, sesión compartida con `VIS-002`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-001.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-001.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-VIN-001.md`
  - `.docs/wiki/23_uxui/UXS/UXS-VIN-001.md`

## Hipótesis bajo test

1. La invitación se entiende como vínculo pendiente y no como alta clínica definitiva.
2. Queda claro que el acceso a datos sigue bajo control del paciente.
3. La acción principal se entiende rápido y sin sobrelectura administrativa.
4. El estado de éxito deja claro qué pasó.
5. El conflicto evita acciones duplicadas innecesarias.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco `desktop-first`
- duración objetivo: `15 a 18 minutos` dentro de la cohorte compartida
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `VIS-002`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en comprensión del alcance, envío y lectura del conflicto
  - capturas si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- profesionales adultos que puedan invitar a una persona a un espacio de seguimiento;
- sin uso previo de este flujo;
- comodidad básica con interfaces web de escritorio;
- idealmente con experiencia real o plausible de acompañamiento clínico o terapéutico.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades que solo puedan “aprobar” la interfaz;
- personas ya expuestas al flujo o al vocabulario interno del producto.

### Fallback aceptable

Si el target profesional real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad del flujo y no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Mirá esta pantalla y contame qué creés que crea esta acción.
2. Cargá un email y explicame qué esperás que pase después de enviar la invitación.
3. Si ya existiera una invitación o vínculo previo, contame qué entenderías.
4. Mostrame si en algún momento sentís que se habilita acceso clínico automático.

### Preguntas de sondeo

- ¿Qué te hizo entender que el acceso sigue controlado por el paciente?
- ¿La pantalla te sonó demasiado administrativa?
- ¿El resultado final te dejó claro si la invitación salió o no?
- ¿El conflicto te impidió repetir una acción innecesaria?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` entrada | la invitación se entiende como vínculo pendiente |  |  | `VOICE-VIN-001` o `UXS-VIN-001` |  |
| `S01` alcance | el control del paciente queda explícito |  |  | `VOICE-VIN-001` |  |
| `S02` acción | la emisión se siente breve y responsable |  |  | `UXS-VIN-001` |  |
| `S02` éxito | el estado resultante se entiende sin explicación oral |  |  | `VOICE-VIN-001` o `UXS-VIN-001` |  |
| `S03` conflicto | el caso existente evita duplicidad |  |  | `VOICE-VIN-001` o `UXS-VIN-001` |  |

## Severidad y criterio de salida

### Severidad

- crítico: induce a pensar que la invitación ya habilita acceso clínico;
- mayor: no bloquea a todas las personas, pero vuelve ambigua la naturaleza del vínculo o del resultado;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-vin-001-vis-002/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-vin-001-vis-002/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-001.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-VIN-001.md`
- jerarquía, estados, secuencia o interacción -> `UXS-VIN-001.md`
- si el hallazgo cambia un patrón reusable de invitación -> revisar además `16_patrones_ui.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-VIN-001.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `VIN-001`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-001.md` una vez exista evidencia observada.
