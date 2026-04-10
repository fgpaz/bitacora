# UXS-ONB-001 — Consentimiento del Onboarding Invitado

## Propósito

Este documento fija el contrato UX del paso crítico `S03` del slice `ONB-001`.

No vuelve a contar el onboarding completo ni reemplaza la voz del caso. Su función es volver operable el momento de consentimiento para que prototipo, validación y contrato técnico trabajen sobre la misma interacción visible.

En este proyecto, `UXS` es el punto donde la intención deja de ser abstracta y se vuelve decisión de pantalla, estado, copy y aceptación.

## Relación con el canon

Este documento depende de:

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `../UXR/UXR-ONB-001.md`
- `../UXI/UXI-ONB-001.md`
- `../UJ/UJ-ONB-001.md`
- `../VOICE/VOICE-ONB-001.md`
- `03_FL/FL-ONB-01.md`
- `03_FL/FL-CON-01.md`
- `04_RF/RF-ONB-003.md`
- `04_RF/RF-CON-001.md`
- `04_RF/RF-CON-002.md`
- `04_RF/RF-CON-003.md`
- `06_pruebas/TP-ONB.md`
- `06_pruebas/TP-CON.md`

Y prepara directamente:

- `../PROTOTYPE/PROTOTYPE-ONB-001.md`
- `../UX-VALIDATION/UX-VALIDATION-ONB-001.md`
- futuros `UI-RFC-*`

No debe contradecir:

- la promesa “tu espacio seguro, vos decidís”;
- la regla de una sola pausa deliberada en el journey;
- la voz `silencio útil antes, control explícito en consentimiento, silencio útil después`;
- el hard gate de consentimiento previo;
- la regla `can_view_data=false` por defecto.

## Slice y paso dueño

### Slice

`ONB-001`: onboarding del paciente nuevo que llega por invitación válida y debe llegar a su primer `MoodEntry`.

### Paso

`S03 — Consentimiento como explicitación de control`

### Entrada

La persona ya atravesó `S01` y `S02`, está autenticada, conserva contexto del onboarding y necesita consentir antes de registrar cualquier dato clínico.

### Salida correcta

El consentimiento queda registrado y la transición a `S04` ocurre sin festejo, sin pantalla intermedia innecesaria y sin reabrir dudas sobre control.

## Qué parte del journey debe preservar

Este paso debe preservar:

- continuidad del onboarding;
- sensación de espacio propio;
- baja fricción general;
- claridad suficiente sin exceso de lectura;
- percepción de lugar seguro y cuidado.

No debe preservar la ligereza borrando información sensible. Debe preservar la ligereza resolviendo lo sensible con orden y compresión.

## Sensación del paso

### Sensación objetivo

Este paso debe sentirse como una `pausa breve, clara y serena`.

No debe sentirse como:

- pantalla legal pesada;
- advertencia clínica;
- trámite administrativo;
- momento de presión;
- examen de comprensión.

### Anti-sensación

La anti-sensación principal es:

**“me frenaron con un bloque legal largo y confuso justo cuando estaba por empezar”.**

### Postura de confianza

La seguridad ya debe sentirse desde antes. En este paso se vuelve explícita sin cambiar de clima.

La postura correcta es:

- explicar qué habilita el consentimiento;
- mostrar qué control conserva la persona;
- aclarar que compartir datos no es automático;
- mantener serenidad y baja agresividad visual/verbal.

### Lectura visceral, conductual y reflexiva

Visceral:

- orden;
- calma;
- densidad contenida;
- ningún gesto de alarma.

Conductual:

- una sola decisión principal;
- texto resumido antes del contenido completo;
- CTA inequívoco;
- salida inmediata al siguiente paso.

Reflexiva:

- “entendí lo importante”;
- “no me escondieron nada”;
- “sigo teniendo control”;
- “ya puedo seguir”.

## Tarea del usuario

La tarea de la persona en este paso es:

1. entender por qué este consentimiento aparece ahora;
2. comprender qué habilita y qué no habilita;
3. revisar la versión vigente;
4. aceptarla con control suficiente;
5. seguir directamente al primer registro.

No llega a esta pantalla para aprender el producto ni para revisar detalles técnicos del vínculo.

## Contrato de interacción

### Estructura mínima

La pantalla debe resolver el paso con cuatro bloques y no más:

1. encabezado breve;
2. resumen de control;
3. texto completo del consentimiento;
4. confirmación explícita + acción principal.

### Jerarquía

El orden de lectura debe ser:

1. qué es este paso;
2. qué seguís controlando;
3. dónde está el texto completo vigente;
4. qué acción te permite seguir.

No debe aparecer antes de esto:

- información secundaria del vínculo;
- explicaciones sobre arquitectura;
- mensajes de tranquilidad vacíos;
- CTA duplicados.

### Composición

- una sola columna principal;
- ancho de lectura moderado;
- superficie calma y contenida;
- aire suficiente entre bloques;
- action area clara al final del paso.

### Resumen de control

Antes del texto completo debe aparecer un resumen breve, no decorativo, con máximo tres ideas:

- necesitás este consentimiento para registrar datos;
- tus registros siguen bajo tu control;
- el profesional no obtiene acceso automático por aceptar esta pantalla.

Este resumen existe para bajar carga cognitiva, no para reemplazar el texto completo.

### Texto completo

- el consentimiento completo debe mostrarse como contenido legible y versionado;
- la versión vigente debe verse, pero no dominar la pantalla;
- el texto debe poder recorrerse con scroll en el mismo paso;
- la lectura completa se considera parte del contrato del paso.

### Confirmación explícita

- debe existir una sola casilla de confirmación vinculada a la versión vigente;
- el sistema no debe enviar aceptación implícita;
- la acción principal permanece deshabilitada hasta que se cumplan las condiciones del paso.

### Acción primaria

La pantalla debe tener una sola acción primaria:

- `Aceptar y seguir`

### Acción secundaria

La salida no principal debe ser de baja prominencia y no competir con la primaria.

Dirección aprobada:

- `Salir por ahora`

No corresponde una dupla simétrica tipo `Aceptar / Rechazar` con el mismo peso visual.

## Reglas del flujo del paso

### Condiciones para habilitar la acción primaria

La acción primaria se habilita solo cuando:

- la persona recorrió el texto completo vigente;
- la casilla de aceptación quedó marcada;
- no hay request en curso.

### Transición de éxito

- al registrar correctamente el consentimiento no debe aparecer una pantalla de celebración;
- la persona debe pasar de inmediato a `S04`;
- si hay feedback visible, debe ser breve y no interrumpir la continuidad.

### Reanudación

Si la persona abandona después de autenticarse pero antes de aceptar:

- el onboarding debe retomar en este mismo paso;
- no debe obligarla a repetir pasos anteriores;
- el contexto de invitación debe seguir preservado.

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| `default` | encabezado, resumen de control, texto vigente, checkbox, CTA deshabilitada | puede leer y prepararse para aceptar |
| `ready_to_submit` | CTA habilitada | puede confirmar y seguir |
| `submitting` | CTA bloqueada + feedback breve de proceso | evita doble envío y conserva contexto |
| `success_transition` | sin toast dominante; transición directa a `S04` | continuidad inmediata |
| `error_retryable` | error breve y específico cerca del área de acción | permite reintento sin reiniciar lectura |
| `version_conflict` | notice claro de cambio de versión | recarga foco y exige revisar la versión vigente |
| `expired_session` | error orientado a reautenticación | no culpa a la persona |

## Contrato de copy

### Titular

Debe orientar el paso sin dramatizar.

Dirección aprobada:

- `Revisá el consentimiento`

### Texto de apoyo

Debe explicar por qué aparece este paso y qué habilita.

Dirección aprobada:

- `Para registrar cómo te sentís, primero necesitás revisar y aceptar esta versión.`

### Resumen de control

Direcciones aprobadas:

- `Tus registros siguen bajo tu control.`
- `Aceptar este paso no activa acceso automático para tu profesional.`
- `Podés revisar la versión vigente antes de seguir.`

### Confirmación

Dirección aprobada:

- `Leí el consentimiento y acepto esta versión.`

### Feedback de proceso

Dirección aprobada:

- `Registrando consentimiento...`

### Error recuperable

Dirección aprobada:

- `No pudimos registrar tu consentimiento. Probá de nuevo.`

### Conflicto de versión

Dirección aprobada:

- `El consentimiento cambió. Revisá la versión actual antes de seguir.`

### Copy prohibido en este paso

- `Esto nos permite cuidarte mejor`;
- `Tu profesional podrá acompañarte de cerca`;
- `Debés aceptar para continuar` sin aclarar alcance;
- `Tus datos están seguros` como frase vacía;
- cualquier felicitación o tono celebratorio al aceptar.

## Accesibilidad

- foco inicial en el encabezado del paso;
- región del texto completo identificable como contenido de consentimiento;
- checkbox accesible por teclado y lector de pantalla;
- CTA deshabilitada con razón comprensible por contexto, no solo por color;
- errores anunciados cerca del área de acción y con foco recuperable;
- scroll y lectura posibles en mobile sin bloquear zoom ni navegación estándar;
- contraste AA estricto en texto, controles y foco.

## Telemetría mínima

| Evento | Momento | Propósito |
| --- | --- | --- |
| `onb_consent_viewed` | el paso se renderiza | medir inicio del paso |
| `onb_consent_scrolled_end` | la persona llega al final del texto | detectar lectura completa |
| `onb_consent_checked` | marca la casilla | medir intención de aceptación |
| `onb_consent_submit_started` | pulsa acción primaria | medir intento |
| `onb_consent_submit_succeeded` | backend registra consentimiento | medir conversión del paso |
| `onb_consent_submit_failed` | request falla | medir fricción técnica |
| `onb_consent_version_conflict` | backend responde conflicto de versión | detectar drift de contenido |
| `onb_consent_abandoned` | sale del paso sin aceptar | medir abandono en pausa crítica |

## Aceptación

Este `UXS` se considera listo si cumple todo lo siguiente:

1. El paso se entiende como parte del onboarding y no como circuito separado.
2. La persona ve primero un resumen de control antes del texto completo.
3. El profesional aparece como actor contextual, no dominante.
4. El consentimiento completo vigente es legible y versionado.
5. La acción primaria solo se habilita cuando se cumple lectura + confirmación explícita.
6. El éxito lleva directo a `S04` sin celebración ni pausa extra.
7. Los errores son específicos y permiten reintento digno.
8. El conflicto de versión obliga a revisar la versión actual sin ambigüedad.
9. En mobile la lectura sigue siendo clara, en una sola columna y sin mini-dashboard.
10. El paso mantiene la sensación `pausa breve, clara y serena`.

## Defaults transferibles para onboarding y formularios

Este `UXS` fija defaults reutilizables para formularios y pasos sensibles del sistema:

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud verbal y visual solo cuando cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión;
- no repetir promesas abstractas de seguridad;
- no convertir un requisito legal en una ceremonia pesada.

## Criterio de validación rápida

El paso está bien calibrado si la persona percibe:

- claridad rápida;
- control legible;
- baja fricción;
- ausencia de vigilancia;
- continuidad directa hacia el primer valor.

El paso está mal calibrado si la persona percibe:

- pared de texto;
- trámite legal;
- obligación opaca;
- sistema del profesional;
- freno innecesario justo antes de empezar.

---

**Estado:** `UXS` activo para `ONB-001 / S03`.
**Precedencia:** este documento depende de `../UXI/UXI-ONB-001.md`, `../UJ/UJ-ONB-001.md` y `../VOICE/VOICE-ONB-001.md`.
**Siguiente capa gobernada:** `../PROTOTYPE/PROTOTYPE-ONB-001.md` y `../UX-VALIDATION/UX-VALIDATION-ONB-001.md`.
