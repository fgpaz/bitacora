# TG-001 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `TG-001`.

No reemplaza `UX-VALIDATION-TG-001.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `TG-001`
- caso: vinculación de cuenta Telegram
- ola operativa actual: `Cohorte G`, sesión compartida con `TG-002`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-TG-001.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-TG-001.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-TG-001.md`
  - `.docs/wiki/23_uxui/UXS/UXS-TG-001.md`

## Hipótesis bajo test

1. La persona entiende el puente web sin sentir una pantalla de setup confusa.
2. El código generado y su vencimiento se entienden rápido.
3. El siguiente paso hacia el bot es inequívoco.
4. El vencimiento no agrega ansiedad innecesaria.
5. La confirmación final deja claro que Telegram ya quedó vinculado.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco `mobile-first`
- duración objetivo: `12 a 15 minutos` dentro de la cohorte compartida
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `TG-002`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en generación de código, lectura del vencimiento y handoff al bot
  - capturas si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas usuarias de Telegram o con experiencia básica en el canal;
- sin uso previo de este flujo;
- comodidad básica con interacciones móviles breves;
- preferentemente con contexto real o plausible de recordatorios o seguimiento personal.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades que solo puedan “aprobar” la interfaz;
- personas ya expuestas al flujo o al vocabulario interno del producto.

### Fallback aceptable

Si el target real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad del puente a Telegram y no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Mirá esta pantalla y contame qué creés que tenés que hacer.
2. Generá el código y explicame qué entendés del vencimiento.
3. Mostrame cuál sería el siguiente paso para terminar el enlace.
4. Si el código venciera, contame qué harías.

### Preguntas de sondeo

- ¿La pantalla se sintió puente o setup largo?
- ¿Qué te hizo entender el siguiente paso?
- ¿El vencimiento del código te resultó claro o excesivo?
- ¿La confirmación final te dejó certeza suficiente?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` inicio | el flujo se entiende como puente corto |  |  | `VOICE-TG-001` o `UXS-TG-001` |  |
| `S02` código | el código y vencimiento se leen rápido |  |  | `UXS-TG-001` |  |
| `S02` handoff | el siguiente paso hacia Telegram es inequívoco |  |  | `VOICE-TG-001` o `UXS-TG-001` |  |
| `S02` vencimiento | la expiración no se vuelve alarmista |  |  | `VOICE-TG-001` |  |
| `S03` cierre | el vínculo final queda explícito |  |  | `VOICE-TG-001` o `UXS-TG-001` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide completar el vínculo o confunde el siguiente paso;
- mayor: no bloquea a todas las personas, pero vuelve ambiguo el handoff o el estado final;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-tg-001-tg-002/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-tg-001-tg-002/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-TG-001.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-TG-001.md`
- jerarquía, estados, secuencia o interacción -> `UXS-TG-001.md`
- si el hallazgo cambia un patrón reusable de puente multicanal -> revisar además `16_patrones_ui.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-TG-001.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `TG-001`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-TG-001.md` una vez exista evidencia observada.
