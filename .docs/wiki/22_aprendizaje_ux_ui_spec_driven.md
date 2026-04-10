# 22 — Aprendizaje UX/UI Spec-Driven

## Propósito

Este documento consolida aprendizajes transversales del canon UX/UI de Bitácora.

No reemplaza `VOICE-*`, `UXS-*`, `UX-VALIDATION-*` ni futuros `UI-RFC-*`. Su función es capturar lo que deja de ser una decisión aislada de un slice y pasa a formar parte del aprendizaje reusable del sistema.

## Relación con el canon

Este documento depende de:

- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `14_metodo_prototipado_validacion_ux.md`
- `16_patrones_ui.md`
- futuros `UX-VALIDATION-*`

## Tipos de aprendizaje

Bitácora distingue entre:

- `hipótesis operativas`: reglas útiles extraídas del canon actual, todavía sin respaldo fuerte de validación real;
- `aprendizajes validados`: reglas confirmadas por evidencia observada y ya absorbidas en documentos dueños;
- `aprendizajes endurecidos`: reglas que además ya orientan patrones o decisiones de implementación.

## Estado actual

Hoy este documento queda en estado `bootstrap`.

Todavía no hay aprendizajes validados consolidados porque la primera ola de `UX-VALIDATION` sigue pendiente de evidencia real. Sí existen hipótesis operativas suficientemente fuertes como para ordenar el trabajo restante.

## Hipótesis operativas vigentes

### 1. Voz antes que step spec

Cuando el wording cambia comprensión, control o confianza, el slice necesita `VOICE-*` antes de `UXS`.

### 2. Prototipo no es decoración

Un `PROTOTYPE-*` solo cuenta si es enlazado, testeable y muestra estados sensibles, no solo el happy path.

### 3. Seguridad se comunica por control, no por promesas

En Bitácora, la sensación de resguardo sube cuando la persona entiende qué cambia, quién accede y qué puede revertir.

### 4. Una sola dirección dominante reduce fricción

Onboarding y formularios funcionan mejor cuando sostienen una sola acción principal, aire suficiente y texto comprimido.

### 5. El retorno a documentos dueños evita drift

Corregir solo el prototipo o solo la interfaz visible no alcanza; el hallazgo tiene que volver a `VOICE`, `UXS` o al documento global correspondiente.

### 6. HTML local y Stitch pueden coexistir

El artefacto canónico debe seguir siendo trazable y estable aunque Stitch se use para acelerar producción visual.

## Regla de promoción de aprendizaje

Una hipótesis operativa pasa a `aprendizaje validado` cuando:

- aparece en evidencia de `UX-VALIDATION-*`;
- el hallazgo o confirmación se repite con suficiente consistencia;
- ya fue absorbido por el documento dueño correspondiente.

Un aprendizaje validado pasa a `endurecido` cuando:

- impacta más de un slice;
- ya ordena patrones o reglas de implementación;
- deja de ser una excepción local.

## Qué debe registrarse acá y qué no

### Sí registrar

- reglas que cambian el canon global;
- aprendizajes que se repiten en más de un slice;
- decisiones que afectan cómo diseñamos, prototipamos o validamos.

### No registrar

- bugs o detalles aislados de una pantalla;
- hallazgos todavía no absorbidos;
- opiniones internas sin evidencia suficiente.

## Criterio de mantenimiento

Este documento debe actualizarse:

- al cerrar cada ola de `UX-VALIDATION`;
- al promover una hipótesis a aprendizaje validado;
- al detectar una regla transversal que ya cambió `12`, `13`, `16` o el orden de trabajo.

---

**Estado:** registro bootstrap de aprendizaje spec-driven.
**Siguiente capa gobernada:** endurecimiento del canon global y extracción de reglas reutilizables para UI.
