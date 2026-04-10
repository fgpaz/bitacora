# 03 — Decisión de entrada a UI con validación diferida

## Fecha

`2026-04-08`

## Contexto

Bitácora ya cerró la cobertura UX visible del MVP en `PROTOTYPE` para los `13` slices visibles:

- `ONB-001`
- `REG-001`
- `REG-002`
- `VIN-001`
- `VIN-002`
- `VIN-003`
- `VIN-004`
- `CON-002`
- `VIS-001`
- `VIS-002`
- `EXP-001`
- `TG-001`
- `TG-002`

También ya existen cohortes operativas `A` a `G` preparadas para validación UX, pero todavía no hay `UX-VALIDATION-*` con evidencia real.

En paralelo, el workspace actual sigue estando en modo `docs-first`: hay canon UX/UI, salidas de Stitch y prototipos navegables, pero todavía no existe capa `UI-RFC` ni implementación frontend/backend viva dentro del repo.

## Decisión

Se aprueba un `waiver explícito de entrada a UI` para abrir la capa UI antes de ejecutar validación UX con personas reales.

La validación UX formal queda diferida hasta que exista código funcional suficiente para observar el producto implementado.

## Alcance del waiver

Este waiver habilita:

- documentación global de entrada a UI;
- refinamiento de patrones y system design frontend;
- apertura posterior de `UI-RFC-*` por slice;
- uso de `Stitch` como fuente visual primaria de referencia;
- uso de `awesome-design-md` como inspiración editorial y visual secundaria, no canónica.

Este waiver no habilita:

- marcar ningún slice como `validated`;
- crear `UX-VALIDATION-*` sin evidencia real;
- declarar implementación lista para cierre final sin validación posterior;
- tratar referencias externas como autoridad por encima del canon propio de Bitácora.

## Regla de precedencia

La jerarquía de referencia para la nueva etapa UI queda así:

1. canon propio de Bitácora (`10` a `23_uxui`);
2. salidas Stitch ya generadas para Bitácora;
3. `awesome-design-md` como biblioteca de inspiración para lenguaje visual y estructura de `DESIGN.md`;
4. preferencias nuevas solo si no contradicen los puntos anteriores.

## Regla de trabajo para las próximas sesiones

- primera sesión UI:
  - foco en `entrada a UI`, no en código;
  - se permiten todas las preguntas de rigor necesarias para endurecer patrones, sistema y criterio global;
  - `Stitch` y los prototipos ya generados deben guiar la dirección visual.
- sesiones posteriores de specs por slice:
  - hacer solo las preguntas estrictamente necesarias;
  - evitar reabrir decisiones globales ya cerradas;
  - preparar el terreno para comenzar código inmediatamente después.

## Orden objetivo de slices para la etapa UI

El orden base para bajar `UI-RFC` por slice será:

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

## Consecuencias

- `21_matriz_validacion_ux.md` mantiene todos los slices en `prepared_waiting_evidence`;
- `23_uxui/UX-VALIDATION/*` sigue siendo deuda explícita, no cancelada;
- la siguiente sesión puede entrar a UI sin fingir validación, siempre que cite este waiver;
- cuando exista código funcional, deberá ejecutarse validación UX y documentarse antes del cierre real.

## Estado

`Aprobado`

