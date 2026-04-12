# HANDOFF-VISUAL-QA-VIS-001 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `VIS-001`.

## Por qué el gate está activo

El gate visual está activo porque `VIS-001` combina:

- gráfico que debe sentirse legible y no analítico;
- estado vacío que no debe confundirse con error;
- filtro de período que no debe competir con el chart.

Una deriva visual aquí puede alterar la percepción del registro propio como herramienta de auto-conocimiento.

## Checkpoints obligatorios

### 1. Chart principal

- el gráfico es lo primero visible sin scroll;
- no se Entierro detrás de controles;
- no parece panel clínico ni analytics.

### 2. Período y filtro

- el selector de período no compite con la lectura del chart;
- al cambiar período, el chart se actualiza sin recargar la página;
- no aparece analytics ni framing clínico.

### 3. Estado vacío

- copy de vacío es útil y no parece error;
- no se Entierro bajo controles;
- la orientación ayuda sin dramatizar.

### 4. Carga y error

- el skeleton o placeholder no es agresivo;
- el error de carga es breve y propone reintento;
- no hay spinners ocupando todo el viewport.

## Drift inaceptable

- copy como `Tu evolución`, `Patrón clínico`, `Mejora sostenida`;
- chart que parece panel de analytics frío;
- estado vacío confundible con error;
- animación excesiva del chart.

## Tolerancias aceptables

- pequeñas variaciones de espaciado que no cambien jerarquía del chart;
- ajuste fino de densidad entre desktop y mobile;
- split interno de componentes sin cambiar la lectura del slice;
- librería del chart puede variar según decisión técnica.

## Evidencia mínima de cierre

- captura desktop de `S01-READY`, `S02-PERIOD`, `S03-EMPTY` y `S03-ERROR`;
- captura mobile de `S01-READY` y `S03-EMPTY`;
- evidencia de que el chart es legible en mobile y que el filtro no compite con la lectura.

## Validación real diferida

La evidencia mínima de cierre arriba es el gate documental. La validación UX real con personas reales queda diferida a `Phase 60`.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
