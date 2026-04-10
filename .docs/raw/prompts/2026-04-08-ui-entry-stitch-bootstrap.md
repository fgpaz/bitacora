<!-- target: codex | pressure: max-pressure | generated: 2026-04-08 -->

# Bitácora — Bootstrap agresivo para nueva sesión de entrada a UI

## Misión

Entrá a la capa UI de Bitácora **sin inventar contexto y sin reabrir decisiones ya cerradas**.

Objetivo de esta sesión:

1. documentar y absorber correctamente la entrada excepcional a UI antes de `UX-VALIDATION`;
2. cerrar la **entrada global a UI**;
3. avanzar luego **por slices**;
4. usar `Stitch` como fuente visual primaria;
5. usar varios subagentes en **cada tarea**;
6. dejar el proyecto listo para que la sesión siguiente ya pueda empezar código.

## Estado verificado del repo

- El repo visible hoy es **canon + prototipos + tooling Stitch**. No hay capa de app/frontend implementada todavía en paths visibles del workspace.
- Todos los `13` slices visibles del MVP ya tienen `PROTOTYPE`.
- La familia `23_uxui/UI-RFC` **todavía no existe**.
- No se detectó un doc vigente de frontend system design.
- `Stitch` ya está configurado en `.docs/stitch/` y tiene corridas reales en `artifacts/stitch/`.
- Los HTML de prototipo ya deben tratarse como wrappers basados en `Stitch`, no como reinterpretaciones manuales.
- El único hueco visual conocido es `VIS-002 ready`, que sigue con cobertura parcial de Stitch y fallback local.
- El worktree está **sucio**. No reviertas cambios ajenos.

## Decisiones bloqueadas que NO se reabren

- La validación UX real se difiere hasta que exista código funcional. Fuente: `.docs/raw/decisiones/03_waiver_entrada_ui_antes_validacion.md`
- `Stitch` es la fuente visual primaria.
- `awesome-design-md` se usa como inspiración secundaria, no como nueva autoridad.
- En los **primeros documentos globales de UI** se hacen todas las preguntas de rigor.
- Cuando empiecen los specs por slice, hacé **solo las preguntas estrictamente necesarias**.
- La secuencia es: **entrada a UI primero, slices después**.
- La prioridad del proyecto sigue siendo: Security > Privacy > Correctness > Usability > Maintainability > Performance > Cost > Time-to-market.

## Primer paso obligatorio

```text
$ps-contexto
```

Leé primero:

- `AGENTS.md`
- `CLAUDE.md`
- `.docs/wiki/02_arquitectura.md`
- `.docs/wiki/11_identidad_visual.md`
- `.docs/wiki/12_lineamientos_interfaz_visual.md`
- `.docs/wiki/14_metodo_prototipado_validacion_ux.md`
- `.docs/wiki/15_handoff_operacional_uxui.md`
- `.docs/wiki/16_patrones_ui.md`
- `.docs/wiki/21_matriz_validacion_ux.md`
- `.docs/wiki/23_uxui/INDEX.md`
- `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-INDEX.md`
- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-INDEX.md`
- `.docs/raw/decisiones/03_waiver_entrada_ui_antes_validacion.md`

## Exploración obligatoria antes de planificar

Antes de proponer cambios, lanzá **mínimo 5 subagentes de exploración en paralelo** en un solo mensaje. Si `ps-explorer` choca con límite de modelo, usá exploradores equivalentes en otro modelo, pero **no bajes de 5 probes paralelos**.

Objetivos mínimos:

1. verificar qué familias UI existen y cuáles faltan (`16`, system design, `UI-RFC`, handoff);
2. mapear `PROTOTYPE-*` + `artifacts/stitch/*` por ola y por slice;
3. localizar el tooling Stitch real en `.docs/stitch/` y cómo se ejecuta;
4. verificar si existe código frontend visible o si la sesión entra a UI desde cero;
5. relevar qué docs técnicos (`07/08/09`, RF, FL) condicionan componentes, estados y contratos UI.

Regla adicional:

- Si aparece `src/` o código real, usá `mi-lsp` para navegación semántica.
- Si `src/` no existe, **no inventes** estructura de app; trabajá explícitamente desde docs + prototipos + Stitch.

## Skill routing obligatorio

Usá este flujo, sin atajos:

1. `$ps-contexto`
2. `$brainstorming`
3. `$ps-asistente-wiki`
4. `$writing-plans`
5. ejecución por waves con varios subagentes en cada tarea
6. `$ps-trazabilidad` por batch
7. `$ps-trazabilidad` final
8. `$ps-auditar-trazabilidad` al cierre

## Regla de preguntas

### Para la entrada global a UI

Hacé las preguntas de rigor necesarias para cerrar:

- qué comunica primero la UI;
- qué patrón visual lidera;
- qué decisiones globales se endurecen en `16`;
- cómo se traducen los prototipos Stitch a sistema implementable;
- qué límites se imponen para no caer en estética SaaS genérica ni copiar estilos externos.

### Para los specs por slice

No abras rondas largas de descubrimiento.

Preguntá solo si:

- falta una decisión que no puede inferirse del canon;
- hay contradicción entre prototipo Stitch y docs dueños;
- una omisión impediría escribir un contrato UI claro.

## Uso de Stitch y de awesome-design-md

Trabajá así:

- `Stitch` manda en lo visual.
- `awesome-design-md` sirve para inspirar disciplina de sistema, secciones útiles, responsive, componentes y lenguaje documental.
- Si `awesome-design-md` contradice `Bitácora` o los prototipos Stitch ya elegidos, gana `Bitácora`.
- No copies un look ajeno completo. Extraé gramática útil, no identidad prestada.

Referencia externa a usar como inspiración:

- `https://github.com/VoltAgent/awesome-design-md`

Dato útil ya verificado:

- su README presenta `DESIGN.md` como documento de sistema visual consumible por agentes y Stitch, con foco en tema visual, tipografía, componentes, layout, responsive y guía para agentes.

## Resultado esperado de la sesión

### Wave 0 — absorción del waiver

- confirmar fase real con `$ps-asistente-wiki`;
- tomar el waiver como decisión activa;
- documentar cualquier sync mínimo necesario para que la entrada a UI no quede en doble autoridad.

### Wave 1 — entrada global a UI

Cerrar, como mínimo:

- endurecimiento de `16_patrones_ui.md`;
- creación del frontend system design;
- definición explícita de cómo `Stitch` alimenta la capa UI;
- criterio de componentización, estados, tokens, motion, responsive y accesibilidad;
- criterio de autoridad entre canon global, Stitch y specs de slice.

### Wave 2+ — specs por slice

Abrí los slices en este orden:

1. núcleo paciente:
   - `ONB-001`
   - `REG-001`
   - `REG-002`
2. vínculo y control paciente:
   - `VIN-002`
   - `VIN-004`
   - `VIN-003`
   - `CON-002`
3. lectura y exportación:
   - `VIS-001`
   - `EXP-001`
4. profesional:
   - `VIN-001`
   - `VIS-002`
5. Telegram:
   - `TG-001`
   - `TG-002`

Para cada slice:

- leer `VOICE-*`, `UXS-*`, `PROTOTYPE-*` y Stitch artifacts;
- usar subagentes;
- minimizar preguntas;
- producir el contrato UI sin inventar estados fuera del prototipo.

## Boundaries

- Esta sesión es **UI docs only**. No empieces implementación.
- No inventes `src/`, `app/`, `components/` ni estructura de código si todavía no existen.
- No reviertas cambios del worktree.
- No uses `UX-VALIDATION-*` como si ya existieran.
- No conviertas el waiver en “validación implícita”.

## Criterios de calidad

- Cero doble autoridad entre canon global, sistema UI y slices.
- Cero deriva entre `Stitch` y los futuros contratos UI sin explicación explícita.
- Cada tarea debe usar varios subagentes.
- Cada batch debe cerrar con trazabilidad.
- La sesión debe dejar el terreno listo para que la próxima arranque código sin reabrir descubrimiento global.

## Cierre obligatorio

Antes de terminar:

- corré `$ps-trazabilidad`;
- corré `$ps-auditar-trazabilidad`;
- dejá explícito qué quedó listo para código y qué quedó pendiente.
