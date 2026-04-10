# REG-002 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `REG-002`.

No reemplaza `UX-VALIDATION-REG-002.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `REG-002`
- caso: registro de factores diarios vía web
- ola operativa actual: `Cohorte B`, cohorte separada sin solapamiento con `ONB-001` ni `REG-001`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-REG-002.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-REG-002.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-REG-002.md`
  - `.docs/wiki/23_uxui/UXS/UXS-REG-002.md`

## Hipótesis bajo test

1. El check-in se percibe como formulario corto y no como encuesta clínica larga.
2. La persona entiende rápido qué información se le pide y por qué.
3. Los bloques agrupados sostienen una sola dirección dominante antes del guardado.
4. El bloque condicional de medicación aparece con claridad y sin cargar de más cuando corresponde.
5. El cierre de guardado es sobrio, claro y sin fricción innecesaria.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco mobile-first
- duración objetivo: 25 a 35 minutos
- tamaño mínimo: `5 personas`
- regla de cohorte: participantes nuevos, sin reutilización de la cohorte `ONB-001 + REG-001`
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en escaneo, bloque condicional y guardado
  - capturas de pantalla si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas que podrían usar Bitácora para completar un check-in diario breve;
- sin uso previo de este flujo;
- con comodidad básica en formularios móviles cortos;
- preferentemente con contexto real o plausible de seguimiento emocional o terapéutico donde tenga sentido registrar factores cotidianos.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades sin relación plausible con el caso;
- personas que ya conozcan la lógica del formulario por exposición previa al producto.

### Fallback aceptable

Si el target real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad del check-in, no evaluando respuestas personales;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Abrí el check-in y contame qué entendés que te pide en los primeros segundos.
2. Completalo como si lo estuvieras resolviendo hoy.
3. Si aparece el bloque de medicación, explicame qué entendés que te está preguntando.
4. Guardá el check-in y contame qué sentís que quedó registrado.
5. Si algo se siente de más o confuso, señalalo en el momento.

### Preguntas de sondeo

- ¿En algún momento esto se sintió como encuesta larga?
- ¿Qué bloque te resultó más claro y cuál más pesado?
- ¿La parte de medicación apareció cuando la esperabas o te sorprendió?
- ¿La acción final fue clara o tuviste que pensarla demasiado?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` entrada | el framing se entiende como check-in breve |  |  | `VOICE-REG-002` o `UXS-REG-002` |  |
| `S02` bloques | los grupos se escanean sin sensación de pared de campos |  |  | `UXS-REG-002` |  |
| `S02` medicación | el bloque condicional aparece con claridad y sin ruido |  |  | `VOICE-REG-002` o `UXS-REG-002` |  |
| `S03` guardado | la acción final se entiende como cierre natural |  |  | `UXS-REG-002` |  |
| `S03` confirmación o error | el cierre deja claridad sin dramatismo ni carga extra |  |  | `VOICE-REG-002` o `UXS-REG-002` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide completar el check-in o genera una lectura errónea de qué se registró;
- mayor: no bloquea a todas las personas, pero agrega fricción relevante, vuelve pesado el formulario o deja ambigua la lógica del guardado o del bloque condicional;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-reg-002/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-reg-002/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-REG-002.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-REG-002.md`
- jerarquía, estados, secuencia o interacción -> `UXS-REG-002.md`
- si el hallazgo cambia un patrón reusable de check-ins o formularios cortos -> revisar además `12_lineamientos_interfaz_visual.md` o `13_voz_tono.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-REG-002.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `REG-002`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-REG-002.md` una vez exista evidencia observada.
