<!-- target: codex | pressure: aggressive | generated: 2026-04-08 -->

# Entrada a UI de Bitácora bajo waiver explícito

## Misión

Iniciar una nueva sesión Codex para abrir la capa UI de Bitácora bajo waiver explícito de validación UX diferida.

Esta sesión debe:

- empezar por la entrada global a UI;
- usar `Stitch` como fuente visual primaria;
- usar `awesome-design-md` solo como inspiración estructurada;
- trabajar con varios subagentes en cada tarea no trivial;
- hacer preguntas rigurosas en los primeros documentos UI;
- y, una vez cerrada la base global, bajar por slices haciendo solo las preguntas estrictamente necesarias.

La meta es dejar el proyecto listo para que la sesión siguiente pueda empezar código con el menor margen posible de ambigüedad.

## Skill stack obligatorio

Usá:

- `$ps-contexto` — primer paso obligatorio
- `$mi-lsp` — navegación semántica si aparecen raíces reales de código bajo `src/`
- `$ps-asistente-wiki` — para confirmar fase documental y no saltar capas sin nombrarlo
- `$brainstorming` — obligatorio antes de planes o specs no triviales
- `$writing-plans` — obligatorio porque esta sesión es grande y multi-step
- `$ps-trazabilidad` — cierre obligatorio
- `$ps-auditar-trazabilidad` — obligatorio al final por tratarse de trabajo multi-módulo

Además:

- usá `spawn_agent` para exploración paralela;
- para cada tarea no trivial, lanzá varios subagentes;
- para cualquier tarea grande o transversal, usá `>= 5` subagentes en un solo mensaje.

## Verificá el repo antes de confiar en este prompt

Estado verificado del workspace al `2026-04-08`:

- el repo está dominado por docs y artefactos; no hay base de aplicación frontend implementada todavía en raíces normales de producto;
- `23_uxui` ya cubre los `13` slices visibles en `PROTOTYPE`;
- `UX-VALIDATION` existe solo como familia/índice, sin evidencia real consolidada;
- `UI-RFC` todavía no existe en el repo;
- `TECH-FRONTEND-SYSTEM-DESIGN.md` todavía no existe;
- [16_patrones_ui.md](C:/repos/mios/humor/.docs/wiki/16_patrones_ui.md) existe solo como baseline `seed`;
- `Stitch` ya produjo la referencia visual primaria de los prototipos aceptados;
- `VIS-002` sigue con una pequeña deuda visual: el estado `ready` no quedó completo en Stitch y mantiene fallback parcial;
- existe waiver explícito para entrar a UI sin `UX-VALIDATION` previa: [2026-04-08-waiver-entrada-ui-sin-ux-validation.md](C:/repos/mios/humor/.docs/raw/decisiones/2026-04-08-waiver-entrada-ui-sin-ux-validation.md).

## Exploración obligatoria antes de planificar o escribir

Hacé esto antes de cualquier plan o documento nuevo:

1. Corré `$ps-contexto`.
2. Leé `AGENTS.md`.
3. Usá `$mi-lsp` si descubrís raíces reales de código bajo `src/`; si no existen, decilo explícitamente y no simules exploración semántica donde no hay código.
4. Lanzá mínimo `5` subagentes de exploración en un solo mensaje, con objetivos atómicos:
   1. verificar dónde vive hoy el código real del frontend o confirmar que todavía no existe;
   2. verificar que `UI-RFC` y frontend system design siguen ausentes;
   3. mapear qué prototipos Stitch existen por slice y qué limitaciones visuales ya quedaron documentadas;
   4. listar contratos, RF y docs técnicos que van a condicionar `UI-RFC`;
   5. buscar convenciones de diseño, componentes, tokens o arquitectura frontend ya presentes en el repo o confirmar su ausencia.
5. Si el modelo de un subagente falla o entra en límite, redispatch inmediato con otro explorador equivalente. No te excuses con la primera falla.
6. Cruzá resultados. Si dos exploradores se contradicen, lanzá un sexto subagente desempate.
7. Leé el README del repo de referencia externa: `https://github.com/VoltAgent/awesome-design-md`.

## Workflow SDD obligatorio para esta sesión

1. `$ps-contexto`
2. bloque de exploración obligatoria (`$mi-lsp` si aplica + 5 subagentes)
3. `$ps-asistente-wiki`
4. `$brainstorming`
5. `$writing-plans`
6. ejecución por waves con subagentes
7. `$ps-trazabilidad`
8. `$ps-auditar-trazabilidad`

## Fuentes primarias a leer primero

- [AGENTS.md](C:/repos/mios/humor/AGENTS.md)
- [02_arquitectura.md](C:/repos/mios/humor/.docs/wiki/02_arquitectura.md)
- [12_lineamientos_interfaz_visual.md](C:/repos/mios/humor/.docs/wiki/12_lineamientos_interfaz_visual.md)
- [13_voz_tono.md](C:/repos/mios/humor/.docs/wiki/13_voz_tono.md)
- [14_metodo_prototipado_validacion_ux.md](C:/repos/mios/humor/.docs/wiki/14_metodo_prototipado_validacion_ux.md)
- [15_handoff_operacional_uxui.md](C:/repos/mios/humor/.docs/wiki/15_handoff_operacional_uxui.md)
- [16_patrones_ui.md](C:/repos/mios/humor/.docs/wiki/16_patrones_ui.md)
- [21_matriz_validacion_ux.md](C:/repos/mios/humor/.docs/wiki/21_matriz_validacion_ux.md)
- [23_uxui/INDEX.md](C:/repos/mios/humor/.docs/wiki/23_uxui/INDEX.md)
- [PROTOTYPE-INDEX.md](C:/repos/mios/humor/.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-INDEX.md)
- [2026-04-08-waiver-entrada-ui-sin-ux-validation.md](C:/repos/mios/humor/.docs/raw/decisiones/2026-04-08-waiver-entrada-ui-sin-ux-validation.md)
- [.docs/stitch/README.md](C:/repos/mios/humor/.docs/stitch/README.md)

Después absorbé una muestra representativa de prototipos Stitch ya aceptados:

- [PROTOTYPE-ONB-001.md](C:/repos/mios/humor/.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-ONB-001.md)
- [PROTOTYPE-REG-001.md](C:/repos/mios/humor/.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-REG-001.md)
- [PROTOTYPE-VIS-001.md](C:/repos/mios/humor/.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIS-001.md)
- [PROTOTYPE-VIN-001.md](C:/repos/mios/humor/.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-001.md)
- [PROTOTYPE-TG-001.md](C:/repos/mios/humor/.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-TG-001.md)

## Decisiones cerradas que NO debés reabrir

- la validación UX real se difiere hasta que exista código funcional;
- no se deben inventar `UX-VALIDATION-*` ahora;
- `Stitch` es la fuente visual primaria;
- los HTML locales deben seguir basados en Stitch, no reinterpretarlo;
- `awesome-design-md` inspira, pero no manda;
- en los primeros documentos UI hay que hacer las preguntas de rigor;
- en las specs por slice solo se hacen preguntas estrictamente necesarias;
- esta sesión debe dejar la entrada a UI cerrada y, si alcanza el tiempo, abrir los primeros slices prioritarios;
- si un skill de UI apunta al canon viejo (`10_*`, `10_uxui/*`), adaptalo al canon real del repo y nombrá la contradicción.

## Regla de preguntas para esta sesión

Fase 1 — entrada global a UI:

- usá el protocolo completo de `$brainstorming`;
- no avances si siguen abiertas decisiones de patrones, tokens, theming, arquitectura de componentes o precedencia visual.

Fase 2 — specs por slice:

- preguntá solo si cambia de verdad:
  - el boundary de componentes;
  - la jerarquía visual principal;
  - el contrato con backend;
  - la accesibilidad o el estado;
  - o la dependencia entre slices.

Todo lo demás debe resolverse por precedencia documental y por los prototipos Stitch ya aceptados.

## Boundaries

- esta sesión es `docs + contratos UI`, no implementación;
- podés crear o actualizar:
  - [16_patrones_ui.md](C:/repos/mios/humor/.docs/wiki/16_patrones_ui.md)
  - `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
  - `.docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md`
  - `.docs/wiki/23_uxui/UI-RFC/UI-RFC-*.md`
- no crees `HANDOFF-*`;
- no generes código frontend todavía, salvo micro-scaffolds internos si fueran estrictamente necesarios para pensar una spec y el usuario lo autoriza;
- no marques ningún slice como `validated`.

## Orden recomendado dentro de la sesión

### Wave 1 — entrada a UI

1. confirmar con `$ps-asistente-wiki` que la entrada excepcional a UI queda explícitamente soportada por el waiver;
2. endurecer [16_patrones_ui.md](C:/repos/mios/humor/.docs/wiki/16_patrones_ui.md) usando:
   - los prototipos Stitch aceptados;
   - la identidad y lineamientos del repo;
   - y `awesome-design-md` solo como referencia de claridad/patrones;
3. crear `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`;
4. iniciar `.docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md`.

### Wave 2 — bajar por slices prioritarios

Si la Wave 1 queda estable, abrir `UI-RFC` en este orden:

1. `ONB-001`
2. `REG-001`
3. `REG-002`

Si todavía hay tiempo y claridad:

4. `VIN-002`
5. `VIN-004`

No abras Telegram ni profesional antes del núcleo paciente salvo que el repo o el usuario te obliguen.

## Riesgos que debés tratar como reales

- si el repo contradice este prompt, confiá en el repo y explicitá la contradicción;
- si el skill de UI trae rutas del canon viejo, tratá eso como drift y adaptalo;
- si un patrón global sigue demasiado blando para pasar a `UI-RFC`, frená y endurecé antes de seguir;
- si una spec por slice depende de backend inexistente o ambiguo, marcá la dependencia y no la tapes con copy;
- si una decisión visual importante no puede justificarse desde Stitch, docs canónicos o inspiración declarada, no la inventes.

## Resultado esperado al cerrar

La sesión debe terminar con:

- entrada a UI abierta y documentada sin ambigüedad;
- [16_patrones_ui.md](C:/repos/mios/humor/.docs/wiki/16_patrones_ui.md) endurecido más allá de `seed`;
- `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` creado;
- `.docs/wiki/23_uxui/UI-RFC/UI-RFC-INDEX.md` creado;
- y, si la claridad alcanza, `UI-RFC` iniciales para `ONB-001`, `REG-001` y `REG-002`.

No cierres la sesión sin:

- `$ps-trazabilidad`
- `$ps-auditar-trazabilidad`

## Última regla

Trabajá como si la sesión siguiente fuera a empezar código de verdad.

Eso significa:

- nada de docs decorativos;
- nada de taxonomías lindas pero vacías;
- nada de patrones que no bajen a componentes, tokens, estados y contratos;
- nada de reabrir todo el universo en cada slice.
