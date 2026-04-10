# Waiver — Entrada a UI sin `UX-VALIDATION` previa

## Fecha

`2026-04-08`

## Propósito

Este waiver deja explícita una excepción operativa para Bitácora:

- la validación UX observada con personas reales se difiere hasta que exista código funcional;
- mientras tanto, el proyecto puede abrir la capa UI documental y técnica;
- esta excepción no convierte ningún slice en `validated` ni reemplaza `UX-VALIDATION-*`.

## Contexto

Al momento de emitir este waiver, el repo ya tiene:

- `13` slices visibles del MVP en `PROTOTYPE`;
- `Stitch` como fuente visual primaria de los prototipos;
- cohortes `A-G` preparadas para validación futura;
- ausencia de `UX-VALIDATION-*` con evidencia real;
- ausencia de `UI-RFC` y de frontend system design canónico.

El equipo decidió priorizar el avance hacia UI y luego validar con el producto funcional, documentando esta deuda de evidencia en vez de fingir que la validación ya ocurrió.

## Relación con el canon

Este waiver:

- no reemplaza [14_metodo_prototipado_validacion_ux.md](C:/repos/mios/humor/.docs/wiki/14_metodo_prototipado_validacion_ux.md);
- no reemplaza [15_handoff_operacional_uxui.md](C:/repos/mios/humor/.docs/wiki/15_handoff_operacional_uxui.md);
- no modifica el significado de [21_matriz_validacion_ux.md](C:/repos/mios/humor/.docs/wiki/21_matriz_validacion_ux.md);
- habilita una excepción puntual para abrir la entrada a UI antes de `UX-VALIDATION-*`.

## Alcance permitido ahora

Queda permitido:

- endurecer [16_patrones_ui.md](C:/repos/mios/humor/.docs/wiki/16_patrones_ui.md) como base de entrada a UI;
- crear `.docs/wiki/07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`;
- iniciar la familia `.docs/wiki/23_uxui/UI-RFC/`;
- producir `UI-RFC-*` por slice aun sin evidencia real, siempre que el waiver quede visible.

## Alcance todavía bloqueado

Este waiver no habilita por sí solo:

- marcar slices como `validated`;
- crear `HANDOFF-*` como si la experiencia ya hubiera sobrevivido validación real;
- presentar la UX como cerrada metodológicamente;
- borrar la obligación posterior de correr `UX-VALIDATION-*`.

## Reglas operativas

### 1. `Stitch` sigue siendo la fuente visual primaria

Los nuevos artefactos UI deben apoyarse primero en los prototipos ya aceptados de `Stitch`.

Los HTML locales y wrappers existen para navegación y consulta, no para reemplazar el lenguaje visual ya aprobado.

### 2. `awesome-design-md` inspira, no gobierna

La referencia externa `https://github.com/VoltAgent/awesome-design-md` puede usarse para:

- enriquecer lenguaje visual;
- mejorar claridad de patrones;
- afinar consistencia editorial y de design-system.

No puede reemplazar:

- la identidad de Bitácora;
- el tono ya fijado;
- ni la autoridad visual concreta de los prototipos aceptados.

### 3. Preguntas amplias solo al inicio de UI

En los primeros documentos globales de UI:

- se deben hacer las preguntas de rigor;
- se debe cerrar la arquitectura visual y técnica;
- se debe evitar llegar a `UI-RFC` con ambigüedades estructurales.

Una vez cerrada la entrada a UI:

- las specs por slice deben hacer solo preguntas estrictamente necesarias;
- cualquier duda menor debe resolverse por precedencia documental, no por reabrir todo el lenguaje visual.

### 4. Subagentes obligatorios por tarea no trivial

Cada tarea no trivial de la nueva sesión UI debe usar varios subagentes.

Regla mínima:

- `>= 3` subagentes para tareas medianas;
- `>= 5` subagentes para tareas grandes o multi-módulo;
- siempre lanzados en paralelo y con objetivos atómicos.

### 5. Riesgo explícito de skills con canon viejo

Algunos skills de UI todavía mencionan rutas del canon anterior.

Para Bitácora, la precedencia documental correcta sigue siendo la del repo actual:

- `16_patrones_ui.md`
- `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `23_uxui/UI-RFC/*`

Si un skill contradice esas rutas, debe adaptarse al canon endurecido del repo en vez de seguirse literalmente.

## Condición de salida de esta excepción

La excepción se considera cerrada solo cuando:

1. exista código funcional para los casos implementados;
2. se ejecuten las rondas reales de validación UX;
3. se creen los `UX-VALIDATION-*` correspondientes;
4. cualquier hallazgo relevante vuelva a `VOICE`, `UXS`, `16` o `UI-RFC` según corresponda.

## Efecto práctico para la próxima sesión

La próxima sesión puede empezar por la entrada a UI y luego bajar por slices.

Orden recomendado:

1. `16_patrones_ui.md`
2. `TECH-FRONTEND-SYSTEM-DESIGN.md`
3. `UI-RFC-INDEX`
4. `UI-RFC` de slices prioritarios

## Estado

`Activo`

## Siguiente uso esperado

Bootstrap de nueva sesión Codex para abrir la capa UI bajo excepción documentada.
