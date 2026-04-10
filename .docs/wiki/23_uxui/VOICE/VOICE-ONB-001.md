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

- CTA principal: `Empezar ahora`;
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

Objetivo verbal:

- cerrar el consentimiento;
- evitar celebración;
- empujar al siguiente valor.

Dirección aprobada:

- confirmación factual;
- CTA principal: `Hacer mi primer registro`;
- sin frases de logro, progreso o felicitación.

## Direcciones aprobadas de microcopy

### Portada estándar

- Titular: enfoque personal y claro.
- Soporte: resguardo y privacidad en una o dos líneas.
- CTA: `Empezar ahora`.

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

### Confirmación + puente

- Confirmación factual breve.
- CTA: `Hacer mi primer registro`.

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

---

**Estado:** spec de voz activa para `ONB-001`.
**Precedencia:** depende de `../../13_voz_tono.md`, `../UXI/UXI-ONB-001.md` y `../UJ/UJ-ONB-001.md`.
**Siguiente capa gobernada:** `../UXS/UXS-ONB-001.md`, `../PROTOTYPE/PROTOTYPE-ONB-001.md` y `../UI-RFC/UI-RFC-ONB-001.md`.
