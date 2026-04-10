# Waiver de evidencia — cierre de cobertura UX restante

## Propósito

Este documento explicita el waiver operativo que permite completar la cobertura UX visible restante sin fingir validación real.

No reemplaza `21_matriz_validacion_ux.md`, no crea `UX-VALIDATION-*` y no habilita `UI-RFC`. Su función es dejar trazable por qué la ola profesional y la ola Telegram pueden avanzar a `PROTOTYPE` antes de contar con sesiones observadas.

## Decisión activa

- fecha de activación: `2026-04-08`
- alcance: `VIN-001`, `VIS-002`, `TG-001`, `TG-002`
- regla: se permite cerrar `PROTOTYPE` y preparación operativa de validación antes de correr evidencia real
- límite: ningún slice puede avanzar a `UI-RFC` sin `UX-VALIDATION-*` con evidencia observada

## Motivo del waiver

- el proyecto necesita terminar primero la cobertura UX visible del MVP;
- hoy no existe disponibilidad real de participantes para ejecutar cohortes `A` a `G`;
- la cobertura paciente ya quedó adelantada a `PROTOTYPE`, por lo que profesional y Telegram eran los huecos restantes del mapa UX;
- el waiver se usa para terminar la cobertura, no para simular cierre metodológico.

## Orden operativo cubierto por este waiver

1. ola profesional:
   - `VIN-001`
   - `VIS-002`
2. ola Telegram:
   - `TG-001`
   - `TG-002`
3. vuelta al gate estricto:
   - ejecutar cohortes `A` a `G`
   - redactar `UX-VALIDATION-*`
   - recién entonces abrir `UI-RFC`

## Reglas de implementación

- `Stitch` es la fuente visual primaria para los cuatro slices;
- cada slice igual debe quedar con `PROTOTYPE-*.md` como documento dueño;
- si un estado puntual falla en Stitch, se reintenta por pantalla individual antes de aceptar fallback local;
- el fallback local solo existe como respaldo navegable, nunca como excusa para marcar el slice como validado;
- todos los índices y la matriz deben reflejar que el resultado final del waiver es `prepared_waiting_evidence`, no `validated`.

## Salidas esperadas

- `PROTOTYPE-VIN-001.md/html`
- `PROTOTYPE-VIS-002.md/html`
- `PROTOTYPE-TG-001.md/html`
- `PROTOTYPE-TG-002.md/html`
- briefs operativos y carpetas de evidencia para cohortes `F` y `G`
- sincronización de:
  - `21_matriz_validacion_ux.md`
  - `23_uxui/INDEX.md`
  - `23_uxui/PROTOTYPE/PROTOTYPE-INDEX.md`
  - `23_uxui/UX-VALIDATION/UX-VALIDATION-INDEX.md`

## Criterio de cierre del waiver

Este waiver queda correctamente ejecutado cuando:

1. los trece slices visibles del MVP tienen `PROTOTYPE`;
2. todos los slices visibles quedan en estado `prepared_waiting_evidence` o equivalente explícito de espera de evidencia real;
3. sigue sin existir ningún `UX-VALIDATION-*` inventado;
4. la siguiente decisión canónica vuelve a ser ejecutar sesiones reales.

---

**Estado:** waiver activo y ejecutable para cierre de cobertura UX restante.
**Siguiente capa gobernada:** retorno al gate estricto mediante `UX-VALIDATION-*` con evidencia real.
