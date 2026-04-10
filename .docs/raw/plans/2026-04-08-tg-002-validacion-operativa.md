# TG-002 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `TG-002`.

No reemplaza `UX-VALIDATION-TG-002.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `TG-002`
- caso: recordatorio y registro conversacional por Telegram
- ola operativa actual: `Cohorte G`, sesión compartida con `TG-001`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-TG-002.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-TG-002.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-TG-002.md`
  - `.docs/wiki/23_uxui/UXS/UXS-TG-002.md`

## Hipótesis bajo test

1. El recordatorio se siente opcional y breve.
2. El teclado inline se entiende sin fricción.
3. La confirmación no interrumpe el canal ni insiste de más.
4. El error se percibe como recuperable sin presión.
5. La conversación conserva tono humano y liviano.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco `mobile-first`
- duración objetivo: `12 a 15 minutos` dentro de la cohorte compartida
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `TG-001`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en lectura del recordatorio, elección del valor y resultado
  - capturas si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas usuarias de Telegram o con experiencia básica en el canal;
- sin uso previo de este flujo;
- comodidad básica con respuestas rápidas en móvil;
- preferentemente con contexto real o plausible de recordatorios personales.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades que solo puedan “aprobar” la interfaz;
- personas ya expuestas al flujo o al vocabulario interno del producto.

### Fallback aceptable

Si el target real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad del intercambio y no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Leé este recordatorio y contame qué te transmite.
2. Elegí un valor y explicame si el teclado te resultó claro.
3. Decime qué entenderías si no quisieras responder ahora.
4. Si algo fallara, contame qué esperarías hacer después.

### Preguntas de sondeo

- ¿El recordatorio te sonó opcional o insistente?
- ¿La respuesta se sintió rápida de verdad?
- ¿La confirmación te pareció suficiente o demasiado presente?
- ¿El error mantuvo un tono amable y recuperable?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` recordatorio | el mensaje se siente opcional |  |  | `VOICE-TG-002` o `UXS-TG-002` |  |
| `S01` keyboard | la elección del valor se entiende rápido |  |  | `UXS-TG-002` |  |
| `S01` salida | la posibilidad de no responder queda visible |  |  | `UXS-TG-002` |  |
| `S02` confirmación | la respuesta registrada no interrumpe el canal |  |  | `VOICE-TG-002` |  |
| `S03` error | el fallo deja reintento sin presión |  |  | `VOICE-TG-002` o `UXS-TG-002` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide responder o convierte el recordatorio en presión no deseada;
- mayor: no bloquea a todas las personas, pero vuelve ambigua la respuesta o su resultado;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-tg-001-tg-002/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-tg-001-tg-002/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-TG-002.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-TG-002.md`
- jerarquía, estados, secuencia o interacción -> `UXS-TG-002.md`
- si el hallazgo cambia un patrón reusable de respuesta conversacional -> revisar además `16_patrones_ui.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-TG-002.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `TG-002`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-TG-002.md` una vez exista evidencia observada.
