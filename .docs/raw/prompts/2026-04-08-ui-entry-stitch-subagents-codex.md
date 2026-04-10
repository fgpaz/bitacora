<!-- target: codex | pressure: aggressive | generated: 2026-04-08 -->

# Entrada a UI de Bitácora — Stitch-first, docs-only, con subagentes en cada tarea

## Misión

Abrir la etapa UI de Bitácora desde cero en una sesión nueva de Codex, respetando el canon del repo y el waiver explícito que permite entrar a UI antes de la validación UX real.

Esta nueva sesión es `docs/UI first`. No debe escribir código de producto todavía. Su objetivo es:

1. endurecer la entrada global a UI;
2. dejar fijado el sistema frontend/documental;
3. preparar el descenso posterior por slices;
4. hacerlo con `Stitch` como fuente visual primaria y `awesome-design-md` como inspiración secundaria.

## Estado verificado del repo

- El repo actual es casi enteramente `docs-first`: no hay todavía una capa fuente de frontend/backend lista para implementar en el workspace.
- La cadena UX visible está completa en `PROTOTYPE` para `13` slices.
- La familia `UI-RFC` todavía no existe en `.docs/wiki/23_uxui/`.
- La entrada a UI quedó habilitada por waiver explícito en `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`.
- La validación UX real queda diferida hasta que exista código funcional.
- `Stitch` ya está conectado y dejó artefactos reales en `artifacts/stitch/`.
- Los HTML locales de prototipo deben seguir basándose en lo generado por Stitch.

## Decisiones cerradas que NO debes reabrir

- La validación UX formal va después de que exista código funcional; no ahora.
- `Stitch` es la fuente visual primaria.
- `awesome-design-md` inspira, pero no reemplaza el canon ni el gusto ya validado internamente con Stitch.
- La primera sesión de UI puede hacer todas las preguntas de rigor necesarias para fijar la capa global.
- Una vez abierta la fase de specs por slice, solo se permiten preguntas estrictamente necesarias.
- Esta sesión nueva es `docs only`; no implementación.
- Cada tarea no trivial debe usar varios subagentes.

## Referencia externa a usar

Usa `https://github.com/VoltAgent/awesome-design-md` como biblioteca de inspiración para:

- estructura de `DESIGN.md`;
- vocabulario de sistema visual;
- claridad editorial de artefactos;
- comparación de 2 o 3 familias de estilo.

No copies una estética ajena literalmente. Bitácora debe quedar más cerca de sus propios prototipos Stitch que de una clonación de Notion, Claude o Linear.

## Primer paso obligatorio

```text
$ps-contexto
```

Lee primero:

- `AGENTS.md`
- `CLAUDE.md`
- `.docs/wiki/02_arquitectura.md`
- `.docs/wiki/14_metodo_prototipado_validacion_ux.md`
- `.docs/wiki/15_handoff_operacional_uxui.md`
- `.docs/wiki/16_patrones_ui.md`
- `.docs/wiki/21_matriz_validacion_ux.md`
- `.docs/wiki/22_aprendizaje_ux_ui_spec_driven.md`
- `.docs/wiki/23_uxui/INDEX.md`
- `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-INDEX.md`
- `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-INDEX.md`
- `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`
- `.docs/stitch/README.md`

## Exploración obligatoria antes de planificar

Antes de cualquier decisión o plan, lanza `mínimo 5 subagentes` en paralelo en un solo mensaje. Si `ps-explorer` falla por límite de modelo, usa exploradores equivalentes y deja el fallback explícito.

Exploraciones mínimas obligatorias:

1. `ps-explorer`: relevar el estado real de `.docs/wiki/23_uxui/` y confirmar qué familias existen y cuáles faltan.
2. `ps-explorer`: relevar `artifacts/stitch/` y resumir qué slices y qué estados ya tienen HTML/PNG reales.
3. `ps-explorer`: revisar `PROTOTYPE-*.md` de los slices núcleo (`ONB-001`, `REG-001`, `REG-002`) para extraer patrones visuales y de interacción repetidos.
4. `ps-explorer`: inspeccionar si existe alguna capa de frontend/code real en el repo o si la sesión debe tratar el workspace como `docs-only`.
5. `ps-explorer`: revisar `16_patrones_ui.md`, `15_handoff_operacional_uxui.md` y el waiver de UI para detectar restricciones de avance.

Además, usa `mi-lsp` cuando haya que navegar semánticamente cualquier código bajo `src/`. Si no existe código útil, dilo explícitamente y no inventes paths.

## Secuencia de workflow obligatoria

Esta sesión debe seguir este flujo:

1. `$ps-contexto`
2. `$brainstorming`
3. `$ps-asistente-wiki`
4. si el trabajo global UI queda multi-step o riesgoso: `$writing-plans`
5. ejecución por batches con varios subagentes por tarea
6. `$ps-trazabilidad`
7. `$ps-auditar-trazabilidad` si abriste o cambiaste varias familias documentales

## Qué debe hacer la sesión nueva

### Fase 1 — Entrada global a UI

Objetivo: fijar la capa UI global antes de bajar a specs por slice.

Reglas:

- acá sí puedes hacer todas las preguntas de rigor;
- usa `awesome-design-md` para comparar estilos y traducirlos a reglas útiles;
- usa los prototipos Stitch de Bitácora como referencia principal de gusto y dirección visual;
- no conviertas inspiración externa en autoridad por encima del repo.

Después de `ps-asistente-wiki`, deja que el skill confirme cuál corresponde primero, pero la expectativa práctica es:

- revisar/reforzar `16_patrones_ui.md` si todavía está demasiado `seed`;
- crear o endurecer el frontend system design global;
- dejar explícita la forma en que los futuros `UI-RFC-*` van a usar Stitch, patrones, tokens, responsive y estados.

### Fase 2 — Descenso por slices

Una vez cerrada la entrada global a UI:

- bajar por slices en orden;
- hacer solo preguntas estrictamente necesarias;
- no reabrir filosofía global, identidad ni reglas base ya fijadas;
- preparar el terreno para que la sesión siguiente pueda arrancar código.

Orden congelado de slices:

1. `ONB-001`
2. `REG-001`
3. `REG-002`
4. `VIN-002`
5. `VIN-004`
6. `VIN-003`
7. `CON-002`
8. `VIS-001`
9. `EXP-001`
10. `VIN-001`
11. `VIS-002`
12. `TG-001`
13. `TG-002`

## Reglas visuales y de inspiración

- preferir calmado clínico, claridad y control;
- conservar la sobriedad y el gusto de las salidas Stitch ya aprobadas;
- usar `awesome-design-md` para lenguaje de sistema visual, no para copiar marcas;
- si comparas referencias, reducí a `2-3` familias como máximo y elegí una dirección recomendada;
- si una decisión visual contradice un prototipo Stitch fuerte ya existente, gana Stitch salvo que haya una razón explícita documentada.

## Boundaries

- `Docs only`.
- No escribir código de aplicación.
- No crear `UX-VALIDATION-*`.
- No fingir que los slices quedaron validados.
- No abrir handoff ni implementación.
- No tocar `07/08/09` salvo que aparezca una contradicción real disparada por la capa UI.

## Output esperado de la sesión

- artefactos UI globales creados o endurecidos;
- criterio explícito de uso de Stitch en la cadena UI;
- roadmap claro de descenso por slices;
- si corresponde, primeros `UI-RFC-*` del núcleo, pero solo después de cerrar la entrada global a UI y sin reabrir debates globales;
- todo sincronizado con índices y cierre por `ps-trazabilidad`.

## Regla de preguntas

- En la fase global UI: hacer todas las preguntas necesarias para cerrar bien el sistema.
- En la fase de specs por slice: preguntar solo si la respuesta cambia contrato, estados o arquitectura visual de forma material.
- Si una duda es cosmética y no cambia contrato, decidir y avanzar.

## Criterio de éxito

La sesión termina bien si:

- el repo sale con una entrada a UI coherente, trazable y sin doble autoridad;
- la inspiración externa quedó domesticada al lenguaje visual de Bitácora;
- la próxima sesión ya puede empezar código después de un descenso UI por slices suficientemente endurecido.

