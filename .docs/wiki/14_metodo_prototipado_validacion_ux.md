# 14 — Método de Prototipado y Validación UX

## Propósito

Este documento define el método global de prototipado y validación UX de Bitácora.

No almacena evidencia de casos concretos ni reemplaza futuros `PROTOTIPO-*`, `UX-VALIDATION-*`, `VOICE-*`, `20_UXS.md` o procesos de QA. Su función es fijar:

- qué cuenta como prototipo validable;
- con qué fidelidad mínima debe trabajarse;
- qué evidencia realmente vale;
- cuándo la validación es obligatoria;
- quiénes pueden participar;
- cuándo una ronda puede darse por cerrada;
- cómo vuelven los hallazgos al documento dueño.

En Bitácora, validar no es “mostrar pantallas” ni “pedir opinión”. Es comprobar, con personas adecuadas y evidencia suficiente, si la experiencia se entiende, se usa, transmite seguridad sin juicio y sostiene claridad en contextos sensibles.

## Relación con el canon

Este documento depende de:

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`

No debe contradecir:

- la promesa “tu espacio seguro, vos decidís”;
- la regla de privacidad antes que monitoreo;
- la necesidad de claridad explícita en consentimiento, acceso profesional y revocación;
- la combinación de calma clínica, interfaz serena y voz contenida.

### Referencias de inspiración no canónicas

Como referencia de calidad de artefacto, esta guía toma inspiración de familias documentales y de prototipado cercanas a las mostradas en `awesome-design-md`, especialmente en líneas similares a Notion y Claude:

- artefactos claros y editables;
- estados suficientemente expresivos;
- jerarquía legible;
- copy y comportamiento visibles dentro del prototipo;
- baja ambigüedad sobre qué se está testeando.

Estas referencias inspiran disciplina de entrega y claridad del artefacto. No sustituyen el criterio metodológico de Bitácora.

## Qué cuenta como prototipo

En Bitácora, un prototipo es un artefacto lo bastante real como para observar:

- si una persona entiende qué está pasando;
- si puede completar la tarea;
- si percibe control, privacidad y claridad;
- si el tono verbal y visual sostienen la confianza esperada;
- si los estados y consecuencias sensibles se leen sin ambigüedad.

Un prototipo validable no es solo una maqueta visual prolija. Debe hacer visibles:

- jerarquía;
- contenido;
- acciones;
- estados;
- feedback;
- copy real o casi final;
- cambios de pantalla, capa o foco relevantes;
- consecuencias sensibles cuando existan.

No cuenta como prototipo suficiente para validación:

- un moodboard;
- una pantalla suelta sin contexto;
- un happy path aislado;
- wireframes exploratorios sin copy ni estados;
- una demo interna que obliga a explicar verbalmente lo que el producto todavía no comunica.

## Niveles de fidelidad

Bitácora adopta `alta fidelidad` como baseline para validar UX.

### Baja fidelidad

Sirve para pensar alternativas, ordenar ideas o alinear internamente.

No sirve como evidencia principal de validación UX en Bitácora.

### Fidelidad intermedia

Puede ayudar a revisar estructura o flujo temprano cuando todavía se está abriendo el problema.

Tampoco alcanza por sí sola para validar pantallas nuevas o decisiones sensibles.

### Alta fidelidad

Es el nivel requerido para validación UX formal en Bitácora.

Debe permitir validar:

- comportamiento;
- copy;
- jerarquía visual;
- estados vacíos, errores y confirmaciones;
- lectura de consentimiento, acceso y revocación;
- sensación de calma, claridad y control.

`Alta fidelidad` no significa pixel-perfect final ni implementación cerrada. Significa “suficientemente real” para que la persona interactúe con algo cercano a la experiencia que luego va a existir.

## Completitud mínima requerida

Bitácora exige `cobertura casi total`, no solo happy path.

Antes de validar, un prototipo debe cubrir como mínimo:

- flujo principal completo de la pantalla o caso;
- estados vacíos relevantes;
- errores previsibles o validaciones cercanas a la tarea;
- confirmaciones o estados de continuidad;
- bifurcaciones sensibles;
- consecuencias visibles de acciones que cambian acceso, privacidad o control;
- puntos donde la confianza puede subir o romperse.

### Qué significa cobertura casi total

La persona no debería tener que imaginar:

- qué pasa si algo falla;
- qué pasa después de aceptar;
- qué cambia si comparte acceso;
- cómo se revoca una decisión;
- cómo se ve un estado vacío o una confirmación;
- qué texto acompaña una acción sensible.

Si una parte importante del caso depende de explicación oral del moderador, el prototipo todavía no está listo.

## Evidencia aceptada

La evidencia principal aceptada en Bitácora es `sesión moderada u observada`.

### Evidencia principal válida

Cuenta como evidencia principal:

- sesión moderada remota o presencial;
- observación directa de resolución de tareas;
- registro de bloqueos, dudas, tiempos y lecturas erróneas;
- notas estructuradas con marcas temporales;
- grabación o evidencia equivalente cuando el contexto lo permita;
- síntesis posterior con hallazgos, severidad e impacto.

### Evidencia complementaria

Puede sumarse como complemento:

- feedback asincrónico;
- comentarios escritos posteriores;
- revisión experta interna;
- señales cuantitativas auxiliares si existieran.

La evidencia asincrónica nunca reemplaza la observación principal.

### Evidencia que no alcanza por sí sola

No alcanza por sí sola:

- opinión del equipo;
- preferencia estética sin conducta observada;
- aprobación de stakeholders;
- comentarios aislados en chat;
- revisión interna de diseño;
- “nos pareció claro” sin contacto con personas válidas.

## Casos donde la validación es obligatoria

En Bitácora, la validación UX es obligatoria para `toda pantalla nueva`.

También es obligatoria cuando un cambio modifica de forma sustantiva:

- consentimiento;
- acceso profesional;
- revocación;
- privacidad o compartición;
- onboarding;
- formularios sensibles;
- estados vacíos o errores de alto impacto;
- jerarquía principal de una pantalla existente;
- lenguaje clave en momentos delicados.

En un producto con sensibilidad clínica y datos personales delicados, “parece claro” no alcanza como criterio de salida.

## Participantes válidos

La regla base es `target real por defecto`.

### Regla base

Se prioriza:

- personas del público real al que apunta la pantalla;
- perfiles que efectivamente usarían la funcionalidad;
- segmentos más expuestos al riesgo de confusión, desconfianza o carga emocional.

### Uso de proxies

Los proxies solo se aceptan con justificación explícita.

Ejemplos de uso excepcional:

- etapa exploratoria previa, claramente marcada como insuficiente;
- imposibilidad transitoria de acceso al target real;
- revisión complementaria con profesionales cuando el flujo tiene componente clínico.

Un proxy no reemplaza el cierre con target real.

### Lo que no cuenta

No reemplazan participantes válidos:

- equipo interno;
- diseño;
- producto;
- desarrollo;
- stakeholders;
- amistades sin relación real con el caso de uso.

## Tamaño mínimo por ronda

Cada ronda de validación debe incluir `5 personas` como mínimo.

### Regla práctica

- si la pantalla responde a un único perfil claramente definido, la ronda mínima es de 5;
- si existen perfiles materialmente distintos, la muestra debe hacer visible esa diferencia o la ronda debe declararse insuficiente;
- si un segmento es el de mayor riesgo, no puede quedar fuera por comodidad operativa.

Cinco no garantizan verdad total, pero sí un piso mínimo para detectar fricciones, patrones y quiebres de comprensión antes de avanzar.

## Regla de iteración hacia specs

Todo hallazgo debe volver al `documento dueño`.

Editar solo el prototipo no alcanza como cierre si el problema revela una decisión de fondo mal especificada.

### Regla de retorno

- si el hallazgo afecta wording, framing o intensidad verbal, vuelve a `VOICE-*` o a `13_voz_tono.md` si el problema es global;
- si el hallazgo afecta flujo, jerarquía, paso, estado, secuencia o comportamiento, vuelve a `20_UXS.md` o a `12_lineamientos_interfaz_visual.md` si expone una regla base insuficiente;
- si el hallazgo afecta identidad o percepción visual estructural, puede escalar a `11_identidad_visual.md`;
- si el hallazgo afecta más de una capa, debe sincronizarse en todas antes de considerarse resuelto.

### Regla de trazabilidad

Cada hallazgo relevante debe dejar explícito:

- qué se observó;
- por qué importa;
- qué documento es dueño de la corrección;
- qué cambio se decidió;
- qué queda pendiente de revalidar.

Sin ese retorno, la iteración queda cosmética y el drift vuelve a aparecer más adelante.

## Regla de salida de una ronda

Una ronda puede cerrarse solo con:

- `cero críticos`;
- `como máximo un mayor no sistémico`.

### Qué es crítico

Se considera crítico un hallazgo que:

- impide completar la tarea;
- rompe comprensión de una consecuencia sensible;
- debilita seriamente la confianza;
- induce una lectura errónea sobre acceso, privacidad, consentimiento o control;
- expone una ambigüedad inaceptable para el contexto clínico.

### Qué es mayor

Se considera mayor un hallazgo que:

- no bloquea a todas las personas;
- pero genera fricción relevante;
- se repite con suficiente frecuencia;
- o amenaza consistencia entre pantallas si no se corrige.

Si hay más de un mayor sistémico, la ronda no está lista para cierre aunque no existan críticos.

## Diferencia entre validación UX y QA

Validación UX y QA no son intercambiables.

### Validación UX

La validación UX busca comprobar:

- si la experiencia se entiende;
- si se puede completar;
- si el tono genera confianza;
- si la carga cognitiva es razonable;
- si la persona percibe control, claridad y resguardo.

### QA

QA busca comprobar:

- si la implementación respeta la spec;
- si los estados y reglas funcionan;
- si no hay regresiones;
- si se cumplen criterios funcionales, visuales y técnicos;
- si el producto implementado se comporta como debe.

### Regla operativa

- validación UX ocurre antes o durante el endurecimiento del caso;
- QA ocurre sobre implementación o artefactos técnicos verificables;
- pasar QA no demuestra comprensión humana;
- pasar validación UX no demuestra corrección técnica.

Las dos capas son obligatorias cuando el caso llega a implementación.

## Criterio de validación rápida

El método de Bitácora está bien aplicado si:

- el prototipo es de alta fidelidad;
- la cobertura es casi total;
- hubo observación moderada u observada como evidencia principal;
- participaron al menos 5 personas válidas;
- los hallazgos volvieron al documento dueño;
- la ronda cerró sin críticos y con, como máximo, un mayor no sistémico.

El método está mal aplicado si:

- se validó con pantallas bonitas pero incompletas;
- se usó solo feedback asincrónico;
- se testeó con equipo interno como reemplazo del target real;
- los hallazgos se corrigieron en Figma pero no en specs;
- se cerró la ronda con ambigüedades sensibles todavía abiertas.

---

**Estado:** baseline global de prototipado y validación UX.
**Precedencia:** este documento depende de `10`, `11`, `12` y `13`.
**Siguiente capa gobernada:** futuros `PROTOTIPO-*`, `UX-VALIDATION-*`, `VOICE-*`, `20_UXS.md` y `15_handoff_operacional_uxui.md`.
