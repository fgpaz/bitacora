# HANDOFF-VISUAL-QA-VIS-002 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `VIS-002`.

## Por qué el gate está activo

El gate visual está activo porque `VIS-002` combina:

- lista de pacientes que no debe parecer EHR;
- alertas básicas que no deben dramatizarse;
- paginación que no debe sentirse como tabla administrativa.

Una deriva visual aquí puede alterar la percepción profesional de responsabilidad sobre el dato.

## Checkpoints obligatorios

### 1. Lista de pacientes

- la lista se prioriza visualmente sin parecerse a un monitor;
- las tarjetas son sobrias y no alarmistas;
- no se muestran pacientes sin acceso habilitado.

### 2. Alertas y resúmenes

- las alertas básicas se distinguen de las dramáticas;
- no se usa copy tipo `Pacientes críticos` o `Vigilancia activa`;
- el tono es responsable y sobrio.

### 3. Paginación

- el cambio de página es fluido;
- no se recarga todo el dashboard;
- la navegación es clara.

### 4. Vacío

- copy claro cuando no hay pacientes visibles;
- no se presenta como error ni como wall.

## Drift inaceptable

- copy como `Mis pacientes monitorizados`, `Vigilancia activa`, `Cartera`;
- estética de EHR o panel de administración;
- dramatización de alertas básicas;
- pacientes listados sin filtro de acceso.

## Tolerancias aceptables

- pequeñas variaciones de espaciado que no cambien jerarquía;
- ajuste fino de densidad entre desktop y mobile;
- split interno de componentes sin cambiar la lectura del slice.

## Evidencia mínima de cierre

- captura desktop de `S01-LOADING`, `S02-READY`, `S04-EMPTY` y `S03-ERROR`;
- captura mobile de `S02-READY` y `S04-EMPTY`;
- evidencia de que la paginación es fluida.

## Validación real diferida

La evidencia mínima de cierre arriba es el gate documental. La validación UX real con personas reales queda diferida a `Phase 60`.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
