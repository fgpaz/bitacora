# VOICE-ONB-001 — Voz del ONB-first del paciente

## Propósito

Este documento define la voz específica de la entrada `ONB-first` del paciente.

No reemplaza `UXS` ni fija todo el microcopy final. Su función es dejar cerrada la regla verbal del slice para que `UXS`, `UI-RFC`, handoff e implementación no improvisen framing, intensidad ni wording sensible.

## Relación con el canon

Este documento depende de:

- `../../13_voz_tono.md`
- `../UXR/UXR-ONB-001.md`
- `../UXI/UXI-ONB-001.md`
- `../UJ/UJ-ONB-001.md`
- `../../03_FL/FL-ONB-01.md`
- `../../03_FL/FL-CON-01.md`
- `../../04_RF/RF-ONB-001.md`
- `../../04_RF/RF-ONB-003.md`

Y prepara directamente:

- `../UXS/UXS-ONB-001.md`
- `../PROTOTYPE/PROTOTYPE-ONB-001.md`
- futuros `UI-RFC-*`

No debe contradecir:

- la promesa “tu espacio seguro, vos decidís”;
- la entrada cálida y humana del slice;
- el consentimiento como `resguardo claro`;
- el límite de no sobreprometer coordinación clínica.

## Regla verbal central

La regla de voz del caso es:

**guía cálida al entrar, resguardo claro al decidir, impulso sereno al salir.**

Eso significa:

- pocas palabras, pero no secas;
- contexto explícito sólo cuando evita confusión real;
- profesional nombrado como contexto, no como dueño de la escena;
- ninguna frase debe sonar más coordinada o “compartida” de lo que el producto hoy sostiene.

## Mapa verbal por tramo

### S01 — Portada estándar

Objetivo verbal:

- presentar Bitácora como un espacio personal;
- dejar una sola acción clara;
- usar privacidad y resguardo como soporte, no como muro explicativo.

Dirección aprobada:

- CTA principal: `Ingresar` (label canónico desde 2026-04-22; va directo a `/ingresar` OIDC+PKCE y evita el falso magic link);
- sin camino secundario fuerte dentro del hero;
- titulares breves y humanos;
- tono de guía personal, no de landing institucional.

### S01 — Hero adaptado por invitación

Objetivo verbal:

- aclarar por qué llegó la persona;
- bajar la fricción de contexto;
- sostener que el espacio sigue siendo suyo.

Dirección aprobada:

- el hero debe nombrar vínculo + propósito;
- el propósito visible se formula como `registro inicial con acompañamiento profesional`;
- con datos completos: mostrar nombre y rol profesional más una frase breve de contexto;
- si faltan datos: mantener hero adaptado genérico, sin volver al hero estándar puro.

Evitar:

- `monitoreo`;
- `seguimiento de tu evolución`;
- `registro compartido` como coordinación fuerte;
- cualquier wording que sugiera acceso automático del profesional.

### S02 — Retorno auth/bootstrap

Objetivo verbal:

- evitar un corte técnico;
- confirmar continuidad;
- preparar el paso siguiente.

Dirección aprobada:

- una frase corta tipo `Estamos preparando tu espacio.`;
- lenguaje no técnico;
- cero explicaciones de sesión, token o bootstrap.

### S03 — Consentimiento

Objetivo verbal:

- volver legible el control;
- explicar lo indispensable;
- sostener el tono humano sin suavizar el hecho sensible.

Dirección aprobada:

- resumir primero qué habilita este paso;
- nombrar explícitamente que aceptar no activa acceso automático del profesional;
- si la persona llegó invitada, recordar el contexto con una línea breve y silenciosa;
- CTA principal: `Aceptar y seguir`.

### S04 — Confirmación + puente

> **Deprecado 2026-04-22**: El estado S04-BRIDGE fue eliminado del flujo. No existe ya una pantalla de puente post-consent. El post-consent va directo a `/dashboard` sin pantalla intermedia. El CTA "Hacer mi primer registro" ya no existe como pantalla; fue reemplazado por el CTA "Registrar humor" del empty state del dashboard. La voz de confirmacion post-consent sigue siendo valida como microcopy para el momento inmediatamente anterior al redirect (si se incluye una confirmacion inline breve en el futuro). Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

Historia del contrato verbal (referencia archivada):

Objetivo verbal:

- cerrar el consentimiento;
- evitar celebracion;
- empujar al siguiente valor.

Direccion aprobada (historica):

- confirmacion factual;
- CTA principal: `Hacer mi primer registro`;
- sin frases de logro, progreso o felicitacion.

## Direcciones aprobadas de microcopy

### Portada estándar

- Titular: enfoque personal y claro.
- Soporte: resguardo y privacidad en una o dos líneas.
- CTA: `Ingresar` (label canónico desde 2026-04-22).

### Portada invitada

- Titular: foco personal, no institucional.
- Bloque contextual: nombre + rol profesional cuando existan; si no, contexto genérico.
- Propósito: `registro inicial con acompañamiento profesional`.

### Interstitial

- `Estamos preparando tu espacio.`
- variante opcional breve: `Enseguida seguís con el consentimiento.`

### Consentimiento

- Titular orientado a revisar, no a obedecer.
- Resumen de control breve.
- Confirmación explícita vinculada a la versión vigente.

### Confirmacion + puente (historico)

> **Deprecado 2026-04-22**: Esta seccion describe el microcopy del estado S04-BRIDGE que fue eliminado. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

- Confirmacion factual breve.
- CTA: `Hacer mi primer registro` (reemplazado por "Registrar humor" en dashboard empty state).

## Formulaciones prohibidas o riesgosas

### Prohibido

- `tu profesional va a ver tus registros` sin contexto de control;
- `primer registro compartido` si suena a coordinación ya activa;
- `seguimiento de tu evolución`;
- `estamos listos para acompañarte de cerca`;
- `perfecto`, `muy bien`, `seguí así`.

### Riesgoso

- copy demasiado lírico en confirmación;
- copy clínico rígido en hero o interstitial;
- frases de seguridad vacías sin traducir a control.

## Defaults del slice

Este slice fija como defaults:

- voseo sobrio;
- títulos de 2 a 6 palabras cuando sea posible;
- una idea principal por bloque;
- privacidad como soporte fuerte en entrada;
- vínculo + propósito como señal dominante en variante invitada;
- bridge final accionable y no celebratorio.

## Criterio de validación rápida

La voz del slice está bien calibrada si se percibe como:

- cálida pero seria;
- clara sin explicar de más;
- contextual en la invitación;
- serena en consentimiento;
- factual al cerrar.

La voz está mal calibrada si se percibe como:

- marketing de bienestar;
- onboarding del profesional;
- copy demasiado técnico;
- o promesa de acompañamiento más grande que el producto actual.

## Cambios recientes

- 2026-04-22: S04 y el microcopy de "Confirmacion + puente" deprecados junto con la Bridge Card. La voz de S01, S02 y S03 sigue activa sin cambios. Ver decision doc `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

## Deltas 2026-04-23 — login flow redesign

> 2026-04-23 — sync login flow redesign: deltas verbales aplicados sobre implementación en rama `feature/login-flow-redesign-2026-04-23` (W1–W4), merged a `main` en commit `5d91158`. Fuente de verdad: `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md`.

### S01 — Hero variant returning (paciente recurrente con cookie viva)

- Titular: `"Volviste."` (una palabra, serena, reconoce continuidad sin dramatizar).
- Sub: `"Seguí donde dejaste."` (invita sin exigir).
- CTA principal: `"Seguir registrando"` → `/dashboard` (verbo directo, alineado con patrón de S01 estándar "Ingresar").
- Privacidad como soporte constante: `"Solo vos ves lo que registrás. Tus datos son privados."` (copy congelado 2026-04-22).

### S01 post-decline — mensaje sereno en landing

- Mensaje accesible vía `role=status aria-live=polite`: `"Podés aceptar cuando quieras. Tu sesión sigue activa."`.
- Tono factual, no dramatiza, no regaña. Reafirma reversibilidad y persistencia de la sesión.
- Se muestra solo cuando el query param `?declined=1` está presente tras clickear el CTA `"Ahora no"` de S03.

### S03 — CTA secundario y revocabilidad

- CTA secundario: `"Ahora no"` (label canónico desde 2026-04-23). Ofrece salida respetuosa sin expresión de causa. Ley 26.529 Art. 2 inc. e.
- Nota de revocabilidad: `"Podés revocarlo cuando quieras desde Mi cuenta."`. Ley 26.529 Art. 10.

### Formulaciones nuevas aprobadas

- `"Volviste."` (S01 returning h1)
- `"Seguí donde dejaste."` (S01 returning sub)
- `"Seguir registrando"` (S01 returning CTA)
- `"Ahora no"` (S03 CTA secundario)
- `"Podés revocarlo cuando quieras desde Mi cuenta."` (S03 revocationNote)
- `"Podés aceptar cuando quieras. Tu sesión sigue activa."` (landing post-decline message)

### Notas verbales

- Todas las formulaciones usan voseo sobrio (regla canon 13).
- Todas usan tildes obligatorias (regla 9.1 CLAUDE.md).
- Sin celebración, sin juicio, sin escalas emocionales. Canon 13 §Errores/Estados sensibles.

---

**Estado:** spec de voz activa para `ONB-001`. S04-BRIDGE deprecado 2026-04-22; S01 (incluyendo variant returning), S02 y S03 (con CTA secundario y revocabilidad) vigentes con deltas 2026-04-23.
**Precedencia:** depende de `../../13_voz_tono.md`, `../UXI/UXI-ONB-001.md` y `../UJ/UJ-ONB-001.md`.
**Siguiente capa gobernada:** `../UXS/UXS-ONB-001.md`, `../PROTOTYPE/PROTOTYPE-ONB-001.md` y `../UI-RFC/UI-RFC-ONB-001.md`.
