# 15 — Handoff Operacional UX/UI

## Propósito

Este documento define el modelo global de handoff UX/UI de Bitácora.

No almacena assets de casos concretos ni reemplaza futuros `HANDOFF-SPEC-*`, `HANDOFF-ASSETS-*`, `HANDOFF-MAPPING-*` o `HANDOFF-VISUAL-QA-*`. Su función es fijar cómo una experiencia ya validada se transforma en un paquete operativo consumible por implementación sin huecos, sin drift y sin decisiones implícitas.

En Bitácora, el handoff no es “pasar Figma” ni “dejar algo bastante claro”. Es una compuerta operativa estricta entre UX validada e implementación.

## Relación con el canon

Este documento depende de:

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `14_metodo_prototipado_validacion_ux.md`

Y gobierna la forma en que más adelante deberán usarse, como mínimo:

- `VOICE-*`
- `20_UXS.md`
- `UI-RFC-*`
- `HANDOFF-SPEC-*`
- `HANDOFF-ASSETS-*`
- `HANDOFF-MAPPING-*`
- `HANDOFF-VISUAL-QA-*`

No debe contradecir:

- la promesa “tu espacio seguro, vos decidís”;
- la prioridad de claridad en momentos sensibles;
- la necesidad de evitar deriva entre intención validada e implementación;
- la lógica de trazabilidad explícita entre decisión, artefacto y código.

### Referencias de inspiración no canónicas

Como referencia de disciplina editorial y operativa, esta guía toma inspiración de bibliotecas documentales como las reunidas en `awesome-design-md`, especialmente en lo que resuelven bien:

- artefactos fáciles de consumir;
- poca ambigüedad sobre ownership;
- claridad sobre readiness;
- separación entre documentos dueños;
- lenguaje operativo en vez de decorativo.

Estas referencias inspiran orden y legibilidad. No reemplazan el criterio de handoff de Bitácora.

## Principios de handoff

### Gate estricto

El handoff de Bitácora funciona como `gate estricto`.

Eso significa:

- no se implementa una slice visible sin el pack requerido;
- no se reemplazan artefactos por conversaciones informales;
- no se asume que frontend va a completar huecos de diseño por contexto;
- no se habilitan fast-tracks implícitos.

### Unidad operativa

La unidad de handoff es `un pack por caso UX validado`.

No se organiza por:

- bundle amplio de feature;
- release genérica;
- pantalla aislada sin continuidad de caso.

Esto permite que cada paquete tenga:

- una historia operativa única;
- un owner claro;
- trazabilidad con validación;
- un alcance implementable sin reconstruir contexto desde cero.

### Especialización obligatoria

El handoff no se colapsa en una checklist genérica.

Cada tipo de ambigüedad debe vivir en su artefacto dueño:

- alcance y estados en `HANDOFF-SPEC-*`;
- inventario de recursos en `HANDOFF-ASSETS-*`;
- correspondencia diseño-código en `HANDOFF-MAPPING-*`;
- control visual final en `HANDOFF-VISUAL-QA-*` cuando aplique.

## Artefactos obligatorios

Todo pack de handoff de Bitácora se construye sobre artefactos especializados.

### `HANDOFF-SPEC-*`

Es obligatorio en todos los casos.

Debe fijar como mínimo:

- alcance implementable;
- referencias fuente;
- estados visibles;
- restricciones;
- blockers;
- expectativas de comportamiento ya resueltas.

### `HANDOFF-ASSETS-*`

Es obligatorio en todos los casos.

Debe inventariar:

- assets exportables;
- íconos, ilustraciones o imágenes;
- fuentes o recursos externos;
- variantes requeridas;
- dependencias de diseño que desarrollo necesita consumir.

Si un caso no requiere assets especiales, el documento igual debe existir y declarar explícitamente `sin assets especiales`.

### `HANDOFF-MAPPING-*`

Es obligatorio en todos los casos.

Debe hacer explícita la correspondencia entre:

- objetos o bloques de diseño;
- componentes o módulos de implementación;
- estados o variantes;
- tokens o decisiones visibles que no deben reinterpretarse.

### `HANDOFF-VISUAL-QA-*`

Es obligatorio solo cuando se activa su gate.

No es un “plus estético”. Es el artefacto que deja explícitos:

- qué se revisa visualmente;
- qué drift es inaceptable;
- qué checkpoints de fidelidad importan;
- qué condiciones deben verificarse antes de cerrar la implementación.

## Owners y consumidores

### Owner principal

El owner principal del pack es `Diseño/UX`.

Su responsabilidad es:

- armar el pack completo;
- asegurar consistencia entre artefactos;
- declarar blockers o faltantes reales;
- no empujar a implementación con vacíos documentales;
- reabrir el pack cuando aparezca drift visible o conductual.

### Consumidor principal

El consumidor principal es `Frontend`.

Su responsabilidad es:

- implementar a partir del pack vigente;
- señalar ambigüedades antes de inventar comportamiento;
- devolver drift detectado al artefacto dueño;
- no tratar mockups o comentarios sueltos como fuente de verdad alternativa.

### Consumidores secundarios

Pueden intervenir como revisores o stakeholders:

- `Producto`, para coordinación, prioridad y bloqueos;
- `QA`, para preparar criterios de verificación posteriores;
- otros roles, solo cuando el caso tenga una dependencia explícita.

Ninguno de ellos reemplaza el ownership principal de Diseño/UX sobre el paquete.

## Regla de readiness

Un caso solo puede pasar a handoff o considerarse listo para implementación cuando cumple todas las condiciones mínimas del gate.

### Condiciones de entrada al handoff

Antes de iniciar el pack, el caso debe tener:

- caso UX ya definido y estable;
- validación UX existente o waiver explícito fuera del flujo normal;
- findings relevantes ya devueltos al documento dueño;
- intención, voz y comportamiento sensibles ya resueltos aguas arriba.

### Condiciones de salida del handoff

Antes de habilitar implementación, el pack debe tener:

- `HANDOFF-SPEC-*` completo;
- `HANDOFF-ASSETS-*` completo, incluso si declara que no hay assets especiales;
- `HANDOFF-MAPPING-*` completo;
- `HANDOFF-VISUAL-QA-*` completo cuando su gate esté activo;
- referencias consistentes con `VOICE-*`, `20_UXS.md` y `UI-RFC-*`;
- blockers y dependencias visibles, no implícitos.

### Regla operativa

Si falta cualquiera de esos elementos, el caso no está listo para implementación.

`Bastante claro`, `lo vemos sobre la marcha` o `después lo ajustamos en código` no cuentan como readiness válida en Bitácora.

## Regla de drift

La regla de drift de Bitácora es `severa y explícita`.

Cualquier cambio visible o conductual acordado reabre el handoff afectado.

### Cambios que reabren el pack

Reabren el handoff, entre otros:

- cambios de copy visible;
- cambios de jerarquía o layout;
- cambios de estados, validaciones o feedback;
- cambios de interacción, foco o navegación;
- cambios en assets, iconografía o motion;
- cambios de responsive que alteren lectura o prioridad;
- cambios de tokens visibles que afecten percepción de la pantalla.

### Regla de resincronización

Cuando hay drift:

- no se corrige solo en diseño;
- no se corrige solo en código;
- no se deja la discrepancia para “más adelante”.

Debe actualizarse el artefacto dueño y, si el cambio toca más de una capa, deben resincronizarse todos los documentos impactados antes de volver a considerar el pack como vigente.

### Regla de precedencia

Si hay contradicción entre artefactos, el pack deja de estar listo.

No existe handoff válido cuando:

- el diseño visible dice una cosa;
- el mapping asume otra;
- o el comportamiento acordado ya cambió pero la documentación sigue vieja.

## Regla de visual QA

El `Visual QA` de Bitácora es `selectivo con trigger híbrido`.

No aplica a todos los casos por defecto, pero tampoco queda librado al criterio informal del equipo.

### Cuándo se activa

El gate de `HANDOFF-VISUAL-QA-*` se activa cuando hay:

- complejidad visual relevante;
- composición no estándar;
- responsive exigente;
- motion o comportamiento visual delicado;
- assets especiales con riesgo de drift;
- o superficies críticas explícitas aunque la visualidad sea sobria.

### Superficies críticas explícitas

Se consideran críticas, como mínimo:

- consentimiento;
- acceso profesional;
- privacidad o compartición;
- revocación;
- onboarding;
- otras pantallas donde una variación visible cambie percepción de control, claridad o resguardo.

### Qué debe verificar

Cuando aplica, `HANDOFF-VISUAL-QA-*` debe dejar explícito:

- qué aspectos visuales no pueden degradarse;
- qué diferencias son tolerables y cuáles no;
- qué checkpoints deben revisarse;
- qué estados o breakpoints requieren atención especial;
- qué señal de confianza se perdería si hay drift.

### Regla de cierre

Si el gate de Visual QA está activo, la implementación no debe darse por cerrada hasta que esa verificación exista y quede resuelta.

## Criterio de validación rápida

El handoff operativo de Bitácora está bien aplicado si:

- existe un pack por caso UX validado;
- Diseño/UX aparece como owner principal;
- Frontend consume un paquete completo, no una mezcla de referencias sueltas;
- los tres artefactos base están siempre presentes;
- `HANDOFF-VISUAL-QA-*` aparece cuando su trigger aplica;
- cualquier drift visible o conductual reabre el paquete.

El handoff está mal aplicado si:

- un caso llega a implementación con huecos documentales;
- assets o mapping quedan “implícitos”;
- el equipo discute cuál es la fuente de verdad vigente;
- se cambia el comportamiento visible sin reabrir docs;
- Visual QA se omite en casos donde complejidad o criticidad lo exigían.

---

**Estado:** baseline global de handoff operacional UX/UI.
**Precedencia:** este documento depende de `10`, `11`, `12`, `13` y `14`.
**Siguiente capa gobernada:** futuros `HANDOFF-SPEC-*`, `HANDOFF-ASSETS-*`, `HANDOFF-MAPPING-*`, `HANDOFF-VISUAL-QA-*` y el paso a implementación.
