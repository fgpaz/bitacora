# EXP-001 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `EXP-001`.

No reemplaza `UX-VALIDATION-EXP-001.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `EXP-001`
- caso: exportación CSV del paciente
- ola operativa actual: `Cohorte E`, sesión compartida con `VIS-001`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-EXP-001.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-EXP-001.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-EXP-001.md`
  - `.docs/wiki/23_uxui/UXS/UXS-EXP-001.md`

## Hipótesis bajo test

1. La persona entiende qué datos salen en el CSV antes de descargar.
2. El flujo se siente como derecho simple de acceso y no como consola técnica.
3. El período elegido se entiende con una sola lectura.
4. El estado de preparación da certeza suficiente sin sobreexplicar.
5. El error recuperable permite reintento sin jerga ni ansiedad técnica.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco `mobile-first`
- duración objetivo: 12 a 18 minutos dentro de la cohorte compartida
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `VIS-001`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en lectura del alcance, confirmación del período y comprensión del handoff
  - capturas de pantalla si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas que podrían querer descargar una copia de sus registros;
- sin uso previo de este flujo;
- con comodidad básica en descargas móviles o web;
- preferentemente con contexto real o plausible de cuidado personal donde el acceso a una copia de los datos tenga sentido.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades que solo puedan “aprobar” la interfaz;
- personas ya expuestas al flujo o al vocabulario interno del producto.

### Fallback aceptable

Si el target real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad del flujo y no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Entrá a esta pantalla y contame qué entendés que va a salir del sistema.
2. Elegí un período y decime si te parece claro qué incluiría el archivo.
3. Dispará la descarga y contame qué esperás que pase.
4. Si algo fallara, explicame qué harías después.

### Preguntas de sondeo

- ¿Qué te hizo entender el alcance del archivo?
- ¿En algún momento esto se sintió técnico o administrativo?
- ¿El handoff de “preparando archivo” te alcanzó para no quedar esperando sin contexto?
- ¿Qué parte te dio más confianza y cuál menos?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` alcance | se entiende qué incluye el archivo |  |  | `VOICE-EXP-001` o `UXS-EXP-001` |  |
| `S01` período | el período elegido se entiende con rapidez |  |  | `UXS-EXP-001` |  |
| `S02` handoff | la preparación da certeza sin ruido extra |  |  | `VOICE-EXP-001` o `UXS-EXP-001` |  |
| `S03` resultado | la descarga iniciada se entiende como cierre factual |  |  | `VOICE-EXP-001` |  |
| `S03` error | el fallo permite reintento y mantiene calma |  |  | `VOICE-EXP-001` o `UXS-EXP-001` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide entender el alcance del archivo o deja ambigua la descarga;
- mayor: no bloquea a todas las personas, pero vuelve el flujo técnico o genera espera sin certeza;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-vis-001-exp-001/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-vis-001-exp-001/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-EXP-001.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-EXP-001.md`
- jerarquía, estados, secuencia o interacción -> `UXS-EXP-001.md`
- si el hallazgo cambia un patrón reusable de salida de datos -> revisar además `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md` o `16_patrones_ui.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-EXP-001.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `EXP-001`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-EXP-001.md` una vez exista evidencia observada.
