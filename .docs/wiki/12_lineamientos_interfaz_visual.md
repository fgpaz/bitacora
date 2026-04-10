# 12 — Lineamientos de Interfaz Visual

## Propósito

Este documento define las reglas globales de interfaz de Bitácora.

No describe componentes concretos ni resuelve una pantalla específica. Su función es traducir el manifiesto y la identidad visual a decisiones reutilizables de superficie, jerarquía, densidad, composición, acción, responsive, accesibilidad y movimiento.

Desde este punto, ningún `UXS` debería improvisar cómo se siente o se organiza la interfaz base de Bitácora.

## Decisiones globales cerradas

- `primera impresión`: refugio personal
- `tono`: editorial íntimo
- `desempate`: claridad con calidez
- `historia líder`: registro personal
- `jerarquía`: contexto -> acción -> confianza
- `confianza jurídica`: solo contextual
- `interacción`: una acción dominante + secundaria silenciosa
- `cobertura deseada`: desktop + mobile + estados clave
- `responsive`: mobile-first paciente + expansión selectiva profesional
- `estados`: integrados con escalamiento
- `accesibilidad`: baseline explícito
- `traducción Stitch`: reglada con margen mínimo
- `continuidad`: visible pero discreta
- `densidad`: aire generoso
- `tipografía`: editorial medida
- `color`: atmosférico con acentos medidos
- `movimiento`: suave y funcional
- `contenedores`: suavidad contenida
- `superficie profesional`: misma familia con giro clínico
- `fondos`: textura suave editorial
- `iconografía`: funcional con calidez mínima
- `vacíos y confirmaciones`: sereno y humano
- `contenido visible de ejemplo`: cotidiano y concreto

## Inputs canónicos

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `20_UXS.md` cuando exista

### Referencias de inspiración no canónicas

Como referencia de lenguaje visual y disciplinamiento de interfaz, este documento toma inspiración estructural de sistemas editoriales silenciosos similares a los mostrados en `awesome-design-md`, especialmente en familias cercanas a Notion y Claude:

- profundidad baja;
- separación por aire y ritmo;
- contenedores suaves;
- jerarquía tranquila;
- acción visible sin agresividad.

Estas referencias inspiran la gramática visual, pero no sustituyen el canon de Bitácora.

## Principios de interfaz

- El contenido personal debe leerse antes que la mecánica del sistema.
- La interfaz debe acompañar con calma, pero explicar con claridad cuando una acción importa.
- La visualidad debe sentirse refugio personal serio, no dashboard ni consola operativa.
- La estructura debe respirar por defecto y comprimir solo donde haya una razón funcional concreta.
- La accesibilidad visual no es optativa ni decorativa; forma parte del tono de cuidado del producto.

## Modelo de superficie y profundidad

Bitácora adopta un modelo `editorial híbrido con contenedores contextuales`.

Eso significa:

- la página base debe sentirse continua, tranquila y mayormente plana;
- los contenedores no son el lenguaje dominante de toda la interfaz;
- los contenedores aparecen cuando aportan resguardo, agrupación o claridad real;
- la profundidad debe ser baja y silenciosa.

### Cuándo usar contenedores

Los contenedores suaves sí deben usarse en:

- bloques de consentimiento;
- secciones de acceso, compartición o revocación;
- formularios sensibles;
- agrupaciones de resumen;
- feedback relevante;
- overlays o capas temporales.

Los contenedores no deben usarse como hábito indiscriminado para:

- cada fragmento de contenido;
- cada estadística;
- cada acción menor;
- cada agrupación decorativa.

### Reglas de profundidad

- Las sombras deben ser mínimas y difusas.
- Los bordes deben separar con sutileza, no recortar con dureza.
- La sensación dominante debe ser papel ordenado, no material flotante.
- La profundidad no debe usarse para dramatizar ni jerarquizar en exceso.

## Jerarquía visual

La jerarquía global de Bitácora debe ser:

1. contenido;
2. guía;
3. acción;
4. estado;
5. soporte.

### Qué debe liderar

En la mayoría de las pantallas, el ojo debe encontrar primero:

- qué se está registrando;
- qué contenido propio existe;
- qué significado tiene la pantalla;
- qué decisión personal está en juego.

### Qué acompaña

La guía debe aparecer como segunda capa:

- explicaciones breves;
- ayudas contextuales;
- aclaraciones sobre privacidad o acceso;
- microcopy de orientación.

### Qué nunca debe competir

No deben competir con el contenido principal:

- banners permanentes;
- CTA con exceso de contraste o tamaño;
- métricas decorativas;
- alertas que no sean realmente críticas;
- capas de estado con apariencia de monitoreo.

## Densidad y ritmo

La interfaz de Bitácora debe trabajar con `aire generoso por defecto` y `compresión localizada por necesidad`.

### Aire por defecto

La pantalla debe respirar en:

- márgenes exteriores;
- separación entre bloques;
- agrupación de formularios;
- encabezados y subtítulos;
- estados de lectura reflexiva.

### Compresión permitida

La compresión solo debe aparecer cuando mejora la tarea sin endurecer la experiencia:

- metadata;
- fechas;
- tablas breves;
- listados secundarios;
- indicadores de estado de apoyo.

### Reglas de ritmo

- El espacio debe marcar jerarquía antes que decoración.
- La separación debe sentirse constante y predecible.
- La interfaz no debe apilar demasiados elementos sin pausa visual.
- La densidad nunca debe acercar una pantalla al lenguaje de dashboard.

## Composición y layout

La filosofía de composición de Bitácora es `centrado sereno`.

### Qué significa

- los bloques principales viven dentro de un ancho de lectura cuidado;
- la página debe sentirse contenida, no extendida de borde a borde;
- la mirada se mantiene enfocada en un eje claro;
- la composición privilegia continuidad y calma sobre simultaneidad extrema.

### Reglas de layout

- El ancho de lectura principal debe ser moderado.
- La alineación debe sentirse estable y reposada.
- Las pantallas personales deben preferir una narrativa vertical clara.
- Las columnas secundarias, si existen, deben acompañar y no competir con el contenido principal.
- La composición no debe parecer plantilla SaaS de dos o tres paneles por defecto.

### Excepciones permitidas

Se permite romper el centrado sereno solo cuando:

- una vista profesional en desktop requiere comparación clínica o seguimiento multi-caso imposible de resolver en una sola columna;
- la excepción mantiene un máximo de dos zonas funcionales visibles, con una primaria y una secundaria de apoyo;
- en esa excepción no se introduce más de un bloque comparativo denso por pantalla sin pausa visual clara;
- un resumen secundario mejora la comprensión;
- una acción sensible necesita aislamiento visual adicional.

Aun en esos casos, la lógica de refugio personal serio debe seguir siendo reconocible.
En mobile, incluso esas excepciones deben volver a una columna dominante.

## Jerarquía de CTA

La acción primaria de Bitácora debe tener `presencia calmada pero inequívoca`.

### CTA primario

Debe:

- reconocerse rápido;
- sostenerse con color, peso y contraste adecuados;
- aparecer como una decisión clara;
- no secuestrar emocionalmente la pantalla.

No debe:

- gritar;
- ocupar demasiado ancho sin razón;
- competir con el contenido principal;
- convertir la interfaz en un flujo imperativo.

### CTA secundarios y terciarios

- Los secundarios deben ser claramente visibles, pero más silenciosos.
- Los terciarios deben vivir como texto, link o acción contenida.
- Las acciones destructivas deben ser explícitas, nunca camufladas.
- Las acciones sensibles deben priorizar claridad sobre elegancia extrema.

### Orden de decisión

Cuando hay varias acciones:

1. una acción primaria máxima;
2. una acción secundaria clara;
3. acciones terciarias discretas;
4. destructivas separadas y con fricción semántica suficiente.

## Formularios, feedback y navegación

Bitácora usa una interacción `suave pero explícita`.

### Formularios

Los formularios deben sentirse:

- legibles;
- respirados;
- guiados sin paternalismo;
- claros en su estructura.

Reglas:

- etiquetas por encima del campo;
- ayudas breves y contextuales;
- agrupación por sentido, no por rigidez técnica;
- validación cercana al lugar del error;
- mensajes claros sobre efecto y consecuencia cuando aplique.

### Feedback

El feedback debe ser:

- sereno en tono;
- explícito en significado;
- breve en texto;
- visible sin ser alarmista.

Reglas:

- confirmaciones simples y directas;
- errores concretos, no vagos;
- estados vacíos con orientación;
- banners o notices solo cuando suman comprensión real;
- acciones sensibles con confirmación clara y lenguaje preciso.

### Navegación

La navegación debe sentirse:

- estable;
- simple;
- predecible;
- sin ruido.

Reglas:

- pocas decisiones visibles a la vez;
- jerarquía clara entre sección actual y secciones vecinas;
- breadcrumbs o equivalentes solo donde realmente mejoren ubicación;
- navegación móvil sin saturación de opciones simultáneas.

## Filosofía responsive

Bitácora adopta una filosofía `mobile-first de una sola columna por defecto`.

### Principio general

En móvil, la interfaz no debe intentar preservar simultaneidad de escritorio en versión reducida.

Debe preservar:

- foco;
- continuidad;
- claridad;
- ritmo de lectura;
- esfuerzo cognitivo bajo.

### Reglas para mobile

- una columna principal por defecto;
- apilado vertical claro;
- acciones cercanas a su contexto;
- bloques secundarios detrás de expansión, resumen o secuenciación si hace falta;
- nada de mini-dashboard comprimido.

### Reglas para desktop

- mantener el eje de lectura sereno;
- usar ancho adicional para respiración y apoyo, no para ruido;
- introducir columnas secundarias solo cuando la comprensión mejora claramente.

## Reglas de movimiento

La regla de motion de Bitácora distingue entre `clima de marca` e `interfaz funcional`.

### Clima de marca

El clima general sigue siendo casi quieto, en coherencia con `11_identidad_visual.md`.

### Interfaz funcional

La interfaz puede usar motion `suave-visible` solo cuando ayuda a:

- orientar transición;
- marcar cambio de foco;
- abrir o cerrar overlays;
- confirmar continuidad entre estados;
- reducir sensación de corte brusco.

### Motion permitido

- fades suaves;
- desplazamientos cortos;
- cambios de estado discretos;
- énfasis de foco no agresivo;
- transiciones que mejoran comprensión espacial.

### Motion no permitido

- rebotes;
- celebraciones;
- pulsos constantes;
- delays teatrales;
- animación ornamental sin función;
- movimiento que compita con contenido, lectura o estados sensibles.

### Movimiento reducido

- Debe existir alternativa de movimiento reducido.
- La orientación no puede depender exclusivamente de animación.
- El sistema debe seguir siendo entendible en modo casi estático.

## Baseline de accesibilidad visual

La base de accesibilidad visual de Bitácora es `AA estricto con foco legible y estados redundantes`.

### Reglas no negociables

- contraste de texto y elementos interactivos conforme a AA;
- foco visible y estable;
- zonas de toque cómodas para interacción táctil;
- estados no comunicados solo por color;
- jerarquía legible en mobile y desktop;
- mensajes y estados compatibles con cansancio visual o atención reducida.

### Foco

El foco debe:

- verse con claridad;
- sentirse integrado a la marca;
- no depender de brillo agresivo;
- ser visible sobre todas las superficies relevantes.

### Estados

Los estados deben distinguirse por combinación de:

- color;
- texto;
- icono cuando haga falta;
- posición o estructura;
- contraste suficiente.

### Zonas de toque y lectura

- Las zonas de toque deben ser táctilmente cómodas.
- La tipografía no debe apoyarse en tamaños frágiles.
- El espaciado debe sostener lectura incluso en pantallas pequeñas.

## Reglas de traducción

### Regla de propiedad documental

- `12_lineamientos_interfaz_visual.md` es el dueño explícito de la dirección visual global.
- `11_identidad_visual.md` absorbe implicancias de identidad.
- `13_voz_tono.md` absorbe implicancias de tono y texto.
- `16_patrones_ui.md` absorbe implicancias de patrones.
- `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` traduce hacia tokens, layout y componentes.
- `23_uxui/UI-RFC` define únicamente contratos por slice.

### Hacia `20_UXS.md`, `16_patrones_ui.md` y `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`

- Mantener la dirección global de este documento como fuente única.
- No reabrir decisiones de primer orden.
- Trasladar las implicancias en cada capa sin cambiar las decisiones.
- Mantener la dirección estable en contextos de onboarding, registro, vínculos y continuidad personal.

## Criterio de validación rápida

Una pantalla de Bitácora respeta este documento si:

- se siente contenido personal antes que operación;
- la acción principal es clara sin ser agresiva;
- el aire visual baja ansiedad en vez de subirla;
- los estados sensibles son explícitos;
- mobile mantiene foco y continuidad;
- la accesibilidad está resuelta sin endurecer la marca.

Una pantalla no respeta este documento si:

- parece un dashboard;
- apila tarjetas sin criterio;
- depende de color para explicar estados;
- comprime contenido sensible sin necesidad;
- convierte las acciones en el elemento más ruidoso;
- usa motion llamativo o innecesario.

---

**Estado:** baseline global de interfaz visual.
**Precedencia:** este documento depende de `10_manifiesto_marca_experiencia.md` y `11_identidad_visual.md`.
**Siguiente capa gobernada:** `13_voz_tono.md`, `16_patrones_ui.md` y futuros `20_UXS.md`.
