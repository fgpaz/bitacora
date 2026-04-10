# PROTOTYPE-ONB-001 — Prototipo del onboarding invitado del paciente

## Propósito

Este documento define el prototipo del slice `ONB-001`.

No declara que la validación ya ocurrió ni reemplaza `UXS-ONB-001.md`. Su función es dejar explícito qué se prototipa, con qué fidelidad, qué flujo y estados cubre, qué referencias debe usar y qué supuestos siguen abiertos antes de observar personas reales.

## Relación con el canon

Este documento depende de:

- `../../14_metodo_prototipado_validacion_ux.md`
- `../UXR/UXR-ONB-001.md`
- `../UXI/UXI-ONB-001.md`
- `../UJ/UJ-ONB-001.md`
- `../VOICE/VOICE-ONB-001.md`
- `../UXS/UXS-ONB-001.md`
- `../../03_FL/FL-ONB-01.md`
- `../../03_FL/FL-CON-01.md`
- `../../06_pruebas/TP-ONB.md`
- `../../06_pruebas/TP-CON.md`

Y prepara directamente:

- `../UX-VALIDATION/UX-VALIDATION-ONB-001.md`

No debe contradecir:

- la prioridad de `casi nula fricción`;
- la postura de `seguridad implícita`;
- la regla de `simpleza radical`;
- `paciente primero`;
- la única pausa deliberada en `S03`;
- `silencio útil antes, control explícito en consentimiento, silencio útil después`.

## Slice cubierto

### Caso

`ONB-001`: onboarding del paciente nuevo que llega por invitación válida y debe llegar a su primer `MoodEntry`.

### Cobertura del prototipo

Este prototipo cubre el flujo completo:

- `S01` apertura de invitación y primer encuadre;
- `S02` auth / bootstrap sin pérdida de contexto;
- `S03` consentimiento como explicitación de control;
- `S04` primer registro de humor;
- `S05` confirmación serena y continuidad.

### Foco principal de observación

Aunque el flujo sea completo, el foco principal de observación es:

- continuidad real entre `S02 -> S03 -> S04`;
- lectura de control en `S03`;
- llegada inmediata al primer valor en `S04`.

## Nivel de fidelidad

El prototipo debe ser de `alta fidelidad`.

Eso implica:

- copy real o casi final;
- jerarquía visual equivalente a la experiencia esperada;
- estados sensibles visibles;
- comportamiento y transición suficientemente reales para observar comprensión;
- mobile y desktop representados con la misma lógica de una sola columna serena.

No alcanza:

- una pantalla suelta;
- wireframe exploratorio;
- happy path sin errores;
- maqueta que requiera explicación verbal para completar el caso.

## Referencias del prototipo

| Recurso | Estado actual | Regla |
| --- | --- | --- |
| archivo o canvas fuente | `./PROTOTYPE-ONB-001.html` | corresponde solo a `ONB-001` y funciona como referencia navegable mínima del slice |
| URL o ruta canónica del prototipo | `./PROTOTYPE-ONB-001.html` | debe usarse como fuente base hasta que exista una referencia externa equivalente |
| pack de frames | definido en este documento | debe mantenerse alineado con `VOICE` y `UXS` |

### Referencia canónica actual

- [Abrir prototipo local](./PROTOTYPE-ONB-001.html)

## Inventario mínimo de frames

| Frame ID | Tramo | Qué muestra | Obligatorio |
| --- | --- | --- | --- |
| `ONB-001-S01-ENTRY` | `S01` | entrada por invitación con framing paciente-primero | sí |
| `ONB-001-S01-EXPIRED` | `S01` | invitación expirada con salida clara | sí |
| `ONB-001-S02-AUTH` | `S02` | auth / bootstrap manteniendo continuidad | sí |
| `ONB-001-S02-AUTH-ERROR` | `S02` | falla de auth sin pérdida de dignidad | sí |
| `ONB-001-S03-DEFAULT` | `S03` | consentimiento default con resumen de control + texto vigente | sí |
| `ONB-001-S03-READY` | `S03` | consentimiento listo para enviar | sí |
| `ONB-001-S03-SUBMITTING` | `S03` | feedback breve durante envío | sí |
| `ONB-001-S03-ERROR` | `S03` | error recuperable de envío | sí |
| `ONB-001-S03-VERSION-CONFLICT` | `S03` | conflicto de versión vigente | sí |
| `ONB-001-S03-RESUME` | `S03` | reanudación tras abandono previo | sí |
| `ONB-001-S04-MOOD` | `S04` | primer registro de humor disponible de inmediato | sí |
| `ONB-001-S05-CONFIRM` | `S05` | confirmación serena posterior al primer mood | sí |

## Flujos y estados testeados por el prototipo

### Flujo principal

- la persona abre la invitación;
- entiende que entra a Bitácora y no a una herramienta del profesional;
- se autentica sin perder contexto;
- atraviesa consentimiento;
- llega enseguida al primer registro;
- recibe confirmación breve y serena.

### Estados sensibles obligatorios

- invitación expirada;
- auth fallida;
- consentimiento default;
- consentimiento listo para enviar;
- consentimiento en envío;
- consentimiento con error recuperable;
- consentimiento con conflicto de versión;
- reanudación del consentimiento tras abandono;
- confirmación posterior al primer mood.

### Regla de cobertura

La persona no debe tener que imaginar:

- qué pasa si la invitación ya no sirve;
- qué pasa si falla auth;
- qué pasa si falla el submit del consentimiento;
- qué pasa si cambió la versión vigente;
- qué pasa después de aceptar;
- qué pasa al llegar al primer registro.

## Hipótesis que este prototipo debe permitir observar

1. La invitación se lee como contexto, no como dominio del profesional.
2. El consentimiento se vive como pausa breve y no como trámite legal pesado.
3. La persona entiende que aceptar no activa acceso automático.
4. La continuidad hacia el primer `MoodEntry` se siente inmediata.
5. El cierre confirma el inicio sin elogios ni dramatización.

## Reglas de construcción del prototipo

- mantener copy alineado con `VOICE-ONB-001`;
- mantener jerarquía, bloques y estados alineados con `UXS-ONB-001.md`;
- no introducir mensajes de tranquilidad vacíos;
- no usar placeholders ambiguos;
- no sumar pantallas accesorias que rompan la sensación de continuidad;
- no convertir `S03` en una pared de texto sin resumen de control;
- no celebrar en `S05`.

## Criterio de readiness antes de validar

El prototipo está listo para validación solo si:

1. la referencia canónica del prototipo ya está adjunta en este documento;
2. el flujo completo `S01 -> S05` es navegable;
3. los estados obligatorios están visibles y consistentes;
4. el copy ya no depende de explicación oral del moderador;
5. mobile y desktop conservan la misma lógica de claridad, foco y baja fricción;
6. la continuidad `S02 -> S03 -> S04` puede observarse sin saltos artificiales.

## Supuestos abiertos antes de validación

- el detalle fino de motion puede seguir siendo discreto mientras no altere comprensión;
- la versión exacta del texto legal puede ajustarse si cambia la publicación vigente, pero debe mantener longitud, estructura y densidad equivalentes;
- si aparece un hallazgo de wording, vuelve primero a `VOICE`;
- si aparece un hallazgo de jerarquía, estados o secuencia, vuelve primero a `UXS-ONB-001.md`.

## Criterio de validación rápida

Este prototipo está bien armado si:

- se percibe suficientemente real para observar conducta;
- cubre el flujo completo y no solo el happy path;
- hace visibles los estados que podrían romper confianza o continuidad;
- permite validar el caso sin necesidad de explicación compensatoria.

Este prototipo está mal armado si:

- parece una suma de pantallas sin ritmo;
- deja fuera errores o bifurcaciones sensibles;
- reduce el consentimiento a una sola vista bonita;
- o necesita que alguien “cuente” lo que el producto todavía no muestra.

---

**Estado:** prototipo enlazado, navegable y testeable para `ONB-001`.
**Precedencia:** este documento depende de `14_metodo_prototipado_validacion_ux.md`, `VOICE-ONB-001.md` y `UXS-ONB-001.md`.
**Siguiente capa gobernada:** `23_uxui/UX-VALIDATION/UX-VALIDATION-ONB-001.md`.
