# VOICE-ONB-001 — Voz del onboarding invitado del paciente

## Propósito

Este documento define la voz específica del onboarding del paciente que llega mediante una invitación válida y avanza hasta su primer `MoodEntry`.

No reemplaza `UXS-ONB-001.md` ni fija todavía microcopy final de pantalla. Su función es volver explícita la regla verbal del caso para que la futura spec de consentimiento, el prototipo y la validación no tengan que improvisar framing, intensidad ni wording sensible.

## Relación con el canon

Este documento depende de:

- `../../13_voz_tono.md`
- `../UXR/UXR-ONB-001.md`
- `../UXI/UXI-ONB-001.md`
- `../UJ/UJ-ONB-001.md`
- `../../03_FL/FL-ONB-01.md`
- `../../03_FL/FL-CON-01.md`
- `../../03_FL/FL-VIN-01.md`
- `../../03_FL/FL-REG-01.md`

Y prepara directamente:

- `../UXS/UXS-ONB-001.md`
- futuros `PROTOTYPE-*`
- futuros `UX-VALIDATION-*`

No debe contradecir:

- la promesa “tu espacio seguro, vos decidís”;
- la prioridad de casi nula fricción en onboarding y formularios;
- la necesidad de seguridad implícita desde el inicio;
- el consentimiento como momento donde el control se vuelve explícito;
- la regla de que el vínculo profesional no equivale a acceso automático a los datos.

## Caso objetivo y audiencia

### Slice

`ONB-001`: onboarding del paciente nuevo que abre una invitación profesional válida, se autentica, atraviesa consentimiento y llega a su primer registro de humor.

### Audiencia principal

- persona nueva;
- sin cuenta previa activa;
- que llega a Bitácora desde una invitación;
- y necesita sentir que puede empezar rápido sin entrar a un sistema ajeno.

### Fuera de alcance

- aceptación de vínculo por paciente ya registrado;
- Telegram;
- UI profesional;
- pasos posteriores al primer `MoodEntry`.

## Regla verbal central

La regla de voz de este caso es:

**silencio útil antes, control explícito en consentimiento, silencio útil después.**

Eso significa:

- la voz no acompaña de más;
- la voz no “demuestra cuidado” agregando texto;
- la seguridad se siente primero por orden, calma y baja carga;
- las palabras solo se expanden cuando hace falta volver legible una decisión sensible;
- terminado ese momento, la voz vuelve a comprimirse.

## Framing emocional heredado

Este onboarding debe sonar:

- ligero;
- claro;
- propio;
- sobrio;
- cuidado;
- poco demandante.

No debe sonar:

- burocrático;
- clínicamente pesado;
- vigilante;
- demasiado legalista;
- controlado por el profesional;
- sobreexplicado.

La voz correcta no “contiene” con discurso. Despeja el camino.

## Traducción verbal de las prioridades del sistema

### Casi nula fricción

En voz, casi nula fricción significa:

- títulos cortos;
- una idea principal por bloque;
- helpers solo si destraban una duda real;
- CTA con verbos directos;
- nada de párrafos para justificar pasos normales.

### Seguridad implícita

En voz, seguridad implícita significa:

- no repetir promesas abstractas sobre seguridad;
- no instalar miedo para después calmarlo;
- no usar frases de vigilancia encubierta;
- hacer visible el control exactamente cuando cambia algo sensible.

### Simpleza radical

En voz, simpleza radical significa:

- pedir y explicar solo lo indispensable;
- evitar dobles mensajes;
- no combinar orientación, tranquilidad y contexto en el mismo bloque;
- dejar que la interfaz haga parte del trabajo sin cargarlo todo al copy.

## Mapa verbal por tramo del journey

### `S01` — Apertura de invitación y primer encuadre

### Objetivo verbal

Explicar rápido por qué llegó la persona y, al mismo tiempo, dejar claro que Bitácora sigue siendo su espacio.

### Dirección aprobada

- mencionar la invitación como contexto, no como mando;
- ubicar a Bitácora en primer plano, no al profesional;
- hablar de empezar, no de cumplir un proceso;
- instalar propiedad y continuidad con pocas palabras.

### Debe sentirse como

“Entré por una invitación, pero esto sigue siendo mío.”

### Evitar

- abrir con el profesional como sujeto dominante;
- sonar a derivación clínica o admisión;
- explicar demasiado temprano qué pasará en todo el flujo;
- prometer seguridad de forma enfática sin necesidad.

### `S02` — Auth / bootstrap sin pérdida de contexto

### Objetivo verbal

Mantener continuidad y reducir sensación de trámite.

### Dirección aprobada

- copy funcional y corto;
- una sola instrucción por paso;
- recordar contexto solo si evita desorientación;
- evitar lenguaje técnico o de cuenta/sesión si no agrega claridad.

### Debe sentirse como

“Estoy entrando para seguir, no empezando un trámite aparte.”

### Evitar

- beneficios marketineros;
- explicaciones sobre arquitectura o autenticación;
- repetir que el espacio es seguro en cada pantalla;
- tono de formulario administrativo.

### `S03` — Consentimiento como explicitación de control

### Objetivo verbal

Hacer legible el control sin romper el ritmo general.

### Dirección aprobada

- explicar primero qué habilita este paso;
- nombrar con claridad quién puede acceder y qué no pasa automáticamente;
- hacer visible la reversibilidad cuando corresponda;
- sostener un tono sereno, no punitivo ni protector.

### Orden verbal recomendado

1. Qué paso se está dando y por qué existe.
2. Qué conserva bajo control la persona.
3. Qué puede ver el profesional y bajo qué condición.
4. Qué acción sigue después.

### Debe sentirse como

“Acá entiendo lo importante sin sentir que me están empujando ni asustando.”

### Evitar

- legalese pesado;
- `debés aceptar` sin explicar alcance;
- `tu profesional podrá monitorear tu evolución`;
- `esto nos permite cuidarte mejor`;
- cualquier frase que haga parecer que aceptar implica visibilidad automática total.

### `S04` — Primer registro de humor

### Objetivo verbal

Llegar al valor lo antes posible sin ceremonia.

### Dirección aprobada

- pregunta directa;
- contexto mínimo;
- cero interpretación emocional;
- cero explicación adicional si la acción ya es evidente.

### Debe sentirse como

“Ya estoy usando Bitácora.”

### Evitar

- introducciones largas;
- tono terapéutico;
- lenguaje de progreso o desempeño;
- copy que vuelva a explicar consentimiento o vínculo en este punto.

### `S05` — Confirmación y continuidad

### Objetivo verbal

Cerrar el inicio con una confirmación serena, no celebratoria.

### Dirección aprobada

- confirmar el hecho;
- si hace falta, marcar el siguiente paso con una línea breve;
- sostener estabilidad emocional baja.

### Debe sentirse como

“Ya empecé y no me hicieron un mundo de esto.”

### Evitar

- elogios;
- felicitaciones;
- promesas emocionales;
- cierre solemne o excesivamente cálido.

## Direcciones aprobadas de microcopy

### Titulares

Los titulares del slice deben:

- orientar la acción inmediata;
- usar entre 2 y 6 palabras cuando sea posible;
- evitar abstracciones como `bienestar`, `progreso` o `acompañamiento`.

### Texto de apoyo

El texto de apoyo debe:

- resolver solo la duda principal del paso;
- caber idealmente en una o dos frases cortas;
- subir un poco en `S03` y volver a comprimirse después.

### CTA

Los CTA deben:

- usar verbos simples y directos;
- describir la acción, no la emoción;
- evitar fórmulas blandas o ambiguas.

Correcto:

- `Continuar`
- `Aceptar y seguir`
- `Registrar`

Incorrecto:

- `Quiero empezar mi camino`
- `Sí, acepto continuar cuidándome`
- `Seguir avanzando`

### Helpers

Los helpers solo aparecen si:

- evitan una duda que podría frenar la continuidad;
- aclaran control en un punto sensible;
- o explican un error con salida concreta.

No deben usarse para:

- repetir el titular;
- aportar calidez decorativa;
- tranquilizar sin contenido accionable.

## Mensajes sensibles y phrasing prohibido

### Sensibles del caso

Los mensajes sensibles principales de este slice son:

- encuadre de la invitación;
- consentimiento;
- acceso profesional;
- invitación expirada;
- falla de autenticación;
- continuidad tras consentimiento.

### Prohibido

- `monitorear`, `seguir tu evolución`, `acompañarte de cerca`;
- `perfecto`, `muy bien`, `seguí así`;
- `debés completar este proceso` sin contexto;
- `tus datos están seguros` como muletilla vacía;
- `tu profesional podrá ver tus registros` si todavía no se aclaró el control real;
- cualquier wording que presente a Bitácora como herramienta del profesional antes que de la persona.

### Riesgoso

- sobreexplicar lo legal en la primera lectura;
- sonar protector en vez de claro;
- usar “paciente” cuando la segunda persona o `persona` alcanza;
- introducir términos internos como `bootstrap`, `token`, `sesión`, `vinculación` en UX normal.

## Terminología del caso

### Preferir

- `vos` con voseo sobrio;
- `Bitácora` para orientar el espacio;
- `registro` para la acción principal;
- `datos` o `registros` para la información;
- `profesional` para el actor clínico contextual;
- `acceso` y `compartir` cuando importe visibilidad.

### Usar con criterio

- `paciente` cuando el momento sea clínico, legal o relacional;
- `consentimiento` cuando el paso realmente lo requiera;
- `revocar` cuando importe la reversibilidad formal.

### Evitar como wording base

- `usuario`;
- `monitoreo`;
- `progreso`;
- `evolución`;
- `habilitar visibilidad`;
- `input`, `evento`, `score`.

## Defaults transferibles para onboarding y formularios del sistema

Este slice fija defaults reutilizables para experiencias equivalentes:

- la voz debe sacar fricción, no agregar compañía;
- la confianza debe sentirse implícita antes de explicarse;
- la explicitud verbal sube solo cuando cambian acceso, datos o consentimiento;
- después de un momento sensible, la voz vuelve a comprimirse;
- en formularios, una instrucción clara vale más que tres capas de explicación.

Estos defaults se sostienen en todo el sistema salvo excepción explícita del caso.

## Criterio de validación rápida

La voz de este slice está bien calibrada si se percibe como:

- breve;
- segura sin insistencia;
- clara en consentimiento;
- propia de la persona, no del profesional;
- ligera incluso en pasos sensibles.

La voz de este slice está mal calibrada si se percibe como:

- trámite;
- consentimiento pesado;
- app vigilante;
- copy demasiado explicativo;
- onboarding del profesional más que del paciente.

---

**Estado:** spec de voz activa para `ONB-001`.
**Precedencia:** este documento depende de `../../13_voz_tono.md`, `../UXI/UXI-ONB-001.md` y `../UJ/UJ-ONB-001.md`.
**Siguiente capa gobernada:** `../UXS/UXS-ONB-001.md`, `../PROTOTYPE/PROTOTYPE-ONB-001.md` y `../UX-VALIDATION/UX-VALIDATION-ONB-001.md`.
