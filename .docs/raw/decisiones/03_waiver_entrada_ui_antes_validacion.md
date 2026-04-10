# Decisión de Gobernanza — Entrada a UI antes de UX-Validation

**Fecha:** 2026-04-08  
**Contexto:** El canon UX visible del MVP ya quedó cubierto en `PROTOTYPE`, con `Stitch` como fuente visual primaria y cohortes `A-G` preparadas, pero sin evidencia real consolidada en `UX-VALIDATION-*`.

---

## Decisión

Se aprueba un **waiver explícito de entrada a UI** para Bitácora.

Desde este punto, el proyecto puede iniciar la capa UI antes de ejecutar `UX-VALIDATION-*`, con esta condición central:

- la validación UX real se difiere hasta que exista una versión funcional del producto;
- esa validación posterior podrá reabrir `UI-RFC`, system design, patrones o slices ya documentados si la evidencia observada contradice decisiones previas.

## Alcance del waiver

Este waiver habilita:

- entrada a UI global;
- endurecimiento de `16_patrones_ui.md`;
- creación del system design frontend;
- apertura de la familia `23_uxui/UI-RFC/*`;
- trabajo por slices en orden operativo.

Este waiver **no** declara que la UX esté validada.

Tampoco reemplaza:

- `14_metodo_prototipado_validacion_ux.md`;
- futuras `UX-VALIDATION-*`;
- QA técnico o visual sobre implementación.

## Motivo

La prioridad operativa del proyecto cambió:

- primero se quiere cerrar la capa UI documental y luego comenzar código;
- la validación real se hará sobre una experiencia funcional, no solo sobre prototipos;
- `Stitch` ya produjo un lenguaje visual suficientemente útil como para bajar contratos UI sin depender de visualidad inventada.

## Regla de precedencia

Durante esta fase:

1. `Stitch` y los `PROTOTYPE-*` vigentes son la fuente visual primaria.
2. `11_identidad_visual.md`, `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md` y `16_patrones_ui.md` siguen siendo autoridad global.
3. `awesome-design-md` puede usarse como inspiración secundaria de disciplina visual y documental, pero no reemplaza el canon Bitácora ni autoriza copiar estilos ajenos si contradicen los prototipos ya elegidos.

## Riesgo aceptado

El equipo acepta explícitamente que:

- puede haber retrabajo en `UI-RFC` y handoff;
- la validación posterior puede devolver cambios a `VOICE`, `UXS`, `16_patrones_ui.md` o system design;
- “listo para UI” no equivale a “validado con personas”.

## Boundaries

Durante la siguiente sesión de UI:

- el foco es documental, no implementación;
- deben hacerse todas las preguntas de rigor en la entrada global a UI;
- una vez iniciados los specs por slice, solo deben hacerse preguntas estrictamente necesarias;
- cada tarea debe usar varios subagentes;
- cualquier contradicción grave entre canon UX, prototipos Stitch y futura capa UI debe detener la ola y resolverse explícitamente.

## Orden operativo acordado

1. entrada a UI global;
2. slices núcleo paciente;
3. slices paciente restantes;
4. slices profesional;
5. slices Telegram;
6. código;
7. validación UX real sobre producto funcional.

---

**Estado:** waiver explícito vigente para entrada a UI.  
**No cambia:** la obligación futura de `UX-VALIDATION-*`.  
**Siguiente uso esperado:** bootstrap de nueva sesión de UI y apertura controlada de `UI-RFC`.
