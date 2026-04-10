# Ola paciente Stitch-first

## Propósito

Este documento define la próxima ola paciente de `PROTOTYPE` con producción `Stitch-first`.

No reemplaza `PROTOTYPE-*` por slice ni adelanta validaciones. Su función es fijar orden, slices, comandos y fallback para que la producción visual avance con ritmo sin romper la trazabilidad del canon UX.

## Regla principal

- Stitch produce primero;
- `PROTOTYPE-*.md` sigue siendo el documento dueño del slice;
- si Stitch no deja un resultado navegable o suficientemente expresivo en un intento breve, se completa con artefacto local complementario y la ola sigue.

## Orden de la ola

### Subola 1 — vínculo y acceso

- `VIN-002`
- `VIN-004`

### Subola 2 — corte y revocación

- `VIN-003`
- `CON-002`

### Subola 3 — lectura y salida de datos

- `VIS-001`
- `EXP-001`

## Setup de generación

- config Stitch: `.docs/stitch/patient-wave-a.prompts.json`
- comando para listar slices disponibles:
  - `npm run list-slices:patientwave`
- criterio operativo:
  - generar `core` primero;
  - pasar a `full` solo cuando el core ya sirva como base del `PROTOTYPE-*`;
  - correr de a un slice por vez por estabilidad del servicio.

## Comandos por slice

### Subola 1

- `npm run generate:vin002`
- `npm run generate:vin004`

### Subola 2

- `npm run generate:vin003`
- `npm run generate:con002`

### Subola 3

- `npm run generate:vis001`
- `npm run generate:exp001`

## Salidas esperadas

Cada corrida deja material en:

- `artifacts/stitch/<slice>/<timestamp>/`

Cada slice queda listo para pasar a `PROTOTYPE-*` cuando:

- existe al menos una corrida `core` útil;
- la jerarquía, copy y estados sensibles ya son legibles;
- el artefacto puede citarse desde el `PROTOTYPE-*.md`;
- no depende de explicación oral para entender el paso crítico.

## Estado actual de la ola

- `VIN-002`: Stitch se intentó en modo `core`, pero la corrida no devolvió una salida recuperable por timeouts; se activó fallback local canónico en `23_uxui/PROTOTYPE/PROTOTYPE-VIN-002.md` + `23_uxui/PROTOTYPE/PROTOTYPE-VIN-002.html`.
- `VIN-003`: corrida `core` parcial; Stitch devolvió el estado `success` y dejaron timeout los demás frames, así que se consolidó fallback local canónico en `23_uxui/PROTOTYPE/PROTOTYPE-VIN-003.md` + `23_uxui/PROTOTYPE/PROTOTYPE-VIN-003.html`, con soporte descargado en `artifacts/stitch/vin-003/2026-04-08T23-07-28-368Z/`.
- `VIN-004`: corrida `core` completada y consolidada en `23_uxui/PROTOTYPE/PROTOTYPE-VIN-004.md` + `23_uxui/PROTOTYPE/PROTOTYPE-VIN-004.html`, con soporte Stitch descargado en `artifacts/stitch/vin-004/2026-04-08T22-47-11-393Z/`.
- `CON-002`: corrida `core` parcial; Stitch devolvió `success` y `error`, pero la entrada quedó en timeout, así que se consolidó fallback local canónico en `23_uxui/PROTOTYPE/PROTOTYPE-CON-002.md` + `23_uxui/PROTOTYPE/PROTOTYPE-CON-002.html`, con soporte descargado en `artifacts/stitch/con-002/2026-04-08T23-10-37-785Z/`.
- `VIS-001`: Stitch completó el pack visual principal pantalla por pantalla y ahora es la referencia primaria del slice, con descargas locales en `artifacts/stitch/vis-001/2026-04-08T23-48-12-302Z/`, `2026-04-08T23-49-24-752Z/`, `2026-04-08T23-50-25-259Z/`, `2026-04-08T23-51-39-073Z/` y `2026-04-08T23-52-43-358Z/`; el HTML local queda como respaldo consolidado.
- `EXP-001`: Stitch completó `s01-period`, `s02-generating` y `s03-error`, descargados en `artifacts/stitch/exp-001/2026-04-08T23-55-19-433Z/`, `2026-04-08T23-56-20-483Z/` y `2026-04-08T23-58-35-022Z/`; `s01-default` y `s02-success` siguen en fallback local por timeout repetido del servicio.
- `VIN-002` + `VIN-004`: briefs operativos de validación listos para `Cohorte C`.
- `VIN-003` + `CON-002`: briefs operativos de validación listos para `Cohorte D`.
- `VIS-001` + `EXP-001`: briefs operativos de validación listos para `Cohorte E`.
- estado global: la ola paciente completa quedó prototipada; resta correr evidencia real por cohortes `C`, `D` y `E`.

## Siguiente paso después de Stitch

Cuando un slice de esta ola ya tiene salida suficiente:

1. crear su `PROTOTYPE-*.md`;
2. enlazar el resultado Stitch y cualquier complemento local necesario;
3. definir su preparación operativa de validación;
4. pasar a `UX-VALIDATION-*` una vez exista evidencia real.

---

**Estado:** ola paciente `Stitch-first` definida.
**Siguiente capa gobernada:** `23_uxui/PROTOTYPE/*` para `VIN-002`, `VIN-004`, `VIN-003`, `CON-002`, `VIS-001` y `EXP-001`.
