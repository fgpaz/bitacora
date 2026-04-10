# 16 — Patrones UI

## Propósito

Este documento consolida los patrones reutilizables de interfaz de Bitácora.

No reemplaza `12_lineamientos_interfaz_visual.md`, no resuelve casos concretos ni sustituye futuros `UI-RFC-*`. Su función es fijar patrones de pantalla, bloque, estado y acción que ya aparecen de forma repetida en el canon UX y que más adelante deberán traducirse a sistema frontend y contratos técnicos.

Sus decisiones operativas asumen como base:

- una acción dominante por pantalla y una secundaria opcional silenciosa;
- señales de confianza y control solo contextuales;
- continuidad visible pero discreta;
- estados integrados con escalamiento (de silencioso a bloqueante según impacto);
- aire generoso por defecto;
- mobile-first paciente con expansión profesional selectiva;
- familia visual compartida paciente/profesional con giro clínico en la lectura operativa.

Durante la entrada a UI bajo dispensa, este documento también gobierna el criterio de elegibilidad de patrones para `UI-RFC` temprana sin confundir esa etapa con validación UX real.

## Relación con el canon

Este documento depende de:

- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `14_metodo_prototipado_validacion_ux.md`
- `21_matriz_validacion_ux.md`
- `23_uxui/INDEX.md`
- `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`
- futuros `VOICE-*`, `UXS-*`, `PROTOTYPE-*` y `UX-VALIDATION-*`

No debe contradecir:

- la promesa “tu espacio seguro, vos decidís”;
- la regla de baja fricción en onboarding y formularios;
- la necesidad de explicitud en consentimiento, acceso y revocación;
- la prioridad de contenido personal antes que mecánica del sistema;
- que la dispensa de entrada a UI no equivale a `UX-VALIDATION`.

## Estado de los patrones

Bitácora usa tres estados para sus patrones:

- `seed`: patrón extraído de lineamientos y specs estables, todavía no consolidado por validación repetida;
- `validated`: patrón observado con evidencia suficiente en uno o más slices;
- `hardened`: patrón ya listo para bajar a system design frontend y `UI-RFC`.

Hoy este documento funciona como `baseline seed`.

La entrada temprana a UI bajo dispensa no modifica estos estados. Un patrón puede ser utilizable en documentación técnica temprana sin dejar de ser `seed`.

## Vía operativa de entrada a UI bajo dispensa

Bitácora abre una `vía operativa` excepcional para documentación UI antes de la validación real.

Esta vía:

- existe solo por la dispensa documentada en `.docs/raw/decisiones/03_decision_entrada_ui_con_validacion_diferida.md`;
- habilita consumo técnico-condicional en diseño de sistema frontend y `UI-RFC`;
- no promueve patrones a `validated`;
- no autoriza llamarlos `hardened`;
- no reemplaza el retorno posterior a `VOICE`, `UXS` o `UX-VALIDATION`.

### Condiciones de entrada a la vía

Un patrón puede usarse en esta vía solo si:

- el slice está cubierto por la dispensa vigente de entrada a UI;
- la cadena `VOICE -> UXS -> PROTOTYPE` del slice está estable y trazable;
- la referencia visual primaria del slice es Stitch por estado obligatorio;
- el uso del patrón no contradice `11`, `12`, `13`, `14`, `21` ni `23_uxui/INDEX.md`.

### Condiciones de salida del lane

El patrón sale de esta vía cuando:

- existe `UX-VALIDATION-*` real para el slice o slices que lo usan;
- los hallazgos críticos ya volvieron al documento dueño;
- recién entonces puede iniciar la promoción normal `seed -> validated -> hardened`.

## Patrones base

### 1. Shell editorial de una columna

- estado: `seed`
- uso: pantallas personales del paciente y recorridos de lectura o carga principal;
- rasgos:
  - eje vertical claro;
  - ancho moderado;
  - aire generoso y silencioso para sostener el tono editorial;
  - una sola dirección dominante en mobile.

### 2. Contenedor sensible

- estado: `seed`
- uso: consentimiento, acceso, revocación, exportación o acciones con efecto sobre privacidad;
- rasgos:
  - borde o superficie suave;
  - copy explícito;
  - separación suficiente del resto del contenido;
  - CTA primaria clara sin dramatismo;
  - señales de confianza desplegadas solo en el contexto inmediato de la acción sensible;
  - continuidad visual del bloque con el resto del flujo sin elevar el peso visual.

### 3. Gesto rápido de valor

- estado: `seed`
- uso: registros instantáneos como `REG-001` o primeros actos de input muy breve;
- rasgos:
  - un solo gesto principal;
  - acción secundaria silenciosa opcional para edición o salida;
  - feedback inmediato;
  - confirmación factual, sin gratificación adicional;
  - cero interpretación emocional agregada.

### 4. Formulario corto agrupado

- estado: `seed`
- uso: check-ins breves o carga de factores cotidianos;
- rasgos:
  - agrupación por sentido;
  - helpers mínimos;
  - bloque condicional visible solo cuando aplica;
  - una sola acción principal al final;
  - layout mobile-first con columna base y expansión profesional selectiva solo cuando el contexto del flujo lo requiere;
  - estados integrados (normal, validación suave, conflicto) sin ruptura de layout.

### 5. Rail final de guardado

- estado: `seed`
- uso: cierre de formularios o decisiones sensibles;
- rasgos:
  - CTA primaria inequívoca;
  - secundaria silenciosa;
  - error localizado;
  - confirmación sin pantalla extra innecesaria cuando no hace falta;
  - continuidad visible pero discreta hacia el siguiente paso recomendado.

### 6. Confirmación factual breve

- estado: `seed`
- uso: guardado correcto, actualización de acceso, consentimiento registrado;
- rasgos:
  - verbo claro;
  - sin elogio;
  - sin celebración;
  - continuidad visible pero discreta si existe siguiente paso;
  - tono sereno, humano y centrado en la acción ejecutada.

### 7. Error recuperable localizado

- estado: `seed`
- uso: fallas de guardado, sesión expirada, conflictos recuperables;
- rasgos:
  - aparece cerca del punto afectado;
  - explica qué pasó;
  - permite reintento;
  - evita dramatizar;
  - escala su alcance solo cuando el impacto cruza a bloqueo de sesión o de consentimiento.

### 8. Control explícito de acceso

- estado: `seed`
- uso: vinculación, gestión de acceso, revocación, consentimiento y exportación;
- rasgos:
  - nombrar quién puede ver qué;
  - explicar si el cambio es inmediato o reversible;
  - mantener tono sereno;
  - no deslizarse a lenguaje de vigilancia;
  - mantener lazos clínicos en el mismo sistema visual, pero en superficie funcional sobria.

### 9. Estado integrado y escalable

- estado: `seed`
- uso: patrones que deben reaccionar en secuencia según sensibilidad de la acción;
- rasgos:
  - transición natural de `default` a `sensible` y luego a `bloqueante`;
  - feedback progresivo en función del riesgo; cuando no hay riesgo, se mantiene en modo discreto;
  - continuidad de contexto para no perder el hilo del check-in.

### 10. Hero contextual adaptativo

- estado: `seed`
- uso: onboarding o entradas públicas que deben absorber contexto sin bifurcar la experiencia;
- rasgos:
  - misma estructura base para camino estándar y contextual;
  - cambio fuerte de copy y señal de propósito, no de producto;
  - una sola acción dominante;
  - soporte de confianza subordinado a la historia principal.

### 11. Interstitial de continuidad breve

- estado: `seed`
- uso: retornos de auth, bootstrap o checks previos a una acción sensible;
- rasgos:
  - duración corta;
  - lenguaje humano, no técnico;
  - confirma continuidad sin abrir una pantalla nueva con peso propio;
  - evita loaders agresivos o spinners teatrales.

### 12. Puente de siguiente acción

- estado: `seed`
- uso: cierres donde hace falta orientar con claridad el paso inmediato posterior;
- rasgos:
  - confirmación factual breve;
  - CTA siguiente inequívoca;
  - sin tono celebratorio;
  - cierra el slice actual y abre el próximo sin confusión.

## Patrones prioritarios para la primera ola UI

La primera ola UI de Bitácora prioriza esta jerarquía:

1. `Shell editorial de una columna`
2. `Hero contextual adaptativo`
3. `Interstitial de continuidad breve`
4. `Contenedor sensible`
5. `Puente de siguiente acción`
6. `Gesto rápido de valor`
7. `Formulario corto agrupado`
8. `Rail final de guardado`
9. `Confirmación factual breve`
10. `Error recuperable localizado`
11. `Control explícito de acceso`

El orden importa porque los tres primeros slices objetivo (`ONB-001`, `REG-001`, `REG-002`) dependen sobre todo de esta gramática.

## Elegibilidad operativa bajo dispensa

| Patrón | Estado base actual | Elegible para entrada a UI | Evidencia Stitch mínima |
| --- | --- | --- | --- |
| Shell editorial de una columna | `seed` | sí, bajo dispensa | al menos un estado principal y un estado sensible del slice |
| Contenedor sensible | `seed` | sí, bajo dispensa | estado `default` y estado de conflicto o error |
| Gesto rápido de valor | `seed` | sí, bajo dispensa | entrada, acción principal y confirmación factual |
| Formulario corto agrupado | `seed` | sí, bajo dispensa | recorrido parcial, condicional y cierre |
| Rail final de guardado | `seed` | sí, bajo dispensa | listo para guardar, enviando, error y confirmación |
| Confirmación factual breve | `seed` | sí, bajo dispensa | confirmación visible sin toast celebratorio |
| Error recuperable localizado | `seed` | sí, bajo dispensa | error visible cerca del punto afectado |
| Control explícito de acceso | `seed` | sí, bajo dispensa | estado base y cambio sensible o reversible |
| Hero contextual adaptativo | `seed` | sí, bajo dispensa | hero estándar + hero contextual + fallback del mismo slice |
| Interstitial de continuidad breve | `seed` | sí, bajo dispensa | estado intermedio visible y no técnico |
| Puente de siguiente acción | `seed` | sí, bajo dispensa | confirmación + CTA al próximo paso |

Esta tabla no abre slices por sí sola. El gating sigue siendo por slice, no por patrón aislado.

## Regla de traducción y fuente Stitch para UI-RFC sin validación real

Durante esta etapa docs-first:

- Stitch es la autoridad visual primaria para traducir patrones a `UI-RFC`;
- `PROTOTYPE-*.md` sigue siendo el dueño del alcance, los frames obligatorios y la trazabilidad del slice;
- el HTML local consolidado puede servir como wrapper navegable del prototipo;
- el HTML local no reemplaza cobertura Stitch faltante cuando el gating del slice es `strict Stitch only`;
- toda divergencia entre Stitch, `VOICE`, `UXS` y `PROTOTYPE` bloquea la apertura de `UI-RFC`.

### Regla anti-deriva

Si un patrón se intenta usar en `UI-RFC` con texto, jerarquía o estados distintos al prototipo Stitch del slice:

- el uso queda bloqueado;
- la corrección vuelve a `VOICE` o `UXS`;
- el patrón no puede presentarse como maduro ni como validado.

## Regla de promoción de patrones

Un patrón pasa de `seed` a `validated` solo cuando:

- aparece en al menos un slice con `UX-VALIDATION-*`;
- no deja hallazgos críticos abiertos sobre su lógica principal;
- el retorno a `VOICE` o `UXS` ya fue absorbido.

Un patrón pasa de `validated` a `hardened` cuando:

- se repite en más de un slice;
- mantiene consistencia entre actores o contextos;
- ya no depende de interpretación artesanal al implementarlo.

La vía operativa bajo dispensa no altera esta promoción. La transición a `validated` o `hardened` queda bloqueada hasta cumplir validación UX real.

## Gating por slice y trazabilidad de entrada a UI-RFC

Un slice solo puede abrir `UI-RFC` en esta etapa si:

- está cubierto por la dispensa explícita de entrada a UI;
- sigue en deuda de validación real y eso está visible en `21_matriz_validacion_ux.md`;
- mantiene cadena consistente en `23_uxui/INDEX.md`;
- todos los estados obligatorios definidos en su `PROTOTYPE-*` tienen cobertura Stitch suficiente para el gating vigente;
- la decisión de autoridad visual del slice no entra en conflicto con `11`, `12` o `13`.

Si la cobertura Stitch es incompleta:

- el slice queda `bloqueado`;
- no se crea `UI-RFC-*` de slice;
- el bloqueo debe quedar nombrado en `23_uxui/UI-RFC/UI-RFC-INDEX.md`;
- el fallback local no puede promoverse silenciosamente a autoridad visual.

## Qué todavía no debe salir de este documento

Este documento todavía no define:

- nombres finales de componentes de implementación;
- props o interfaces TypeScript;
- contratos técnicos completos por slice;
- variantes cerradas por breakpoint a nivel de código;
- cierre de UX validada;
- autorización para omitir evidencia Stitch cuando el gate del slice sea estricto.

Eso pertenece a frontend system design y futuros `UI-RFC-*`.

## Criterio de validación rápida

Este documento está bien usado si:

- evita reinventar bloques repetidos en cada `UXS`;
- deja claro qué patrones son `seed` y cuáles ya están `validated`;
- habilita entrada a UI sin fingir que la validación ya ocurrió;
- distingue con claridad entre `estado del patrón` y `uso operativo bajo dispensa`.

Está mal usado si:

- se toma como contrato técnico final;
- promueve patrones no validados como definitivos;
- confunde la vía operativa con evidencia real;
- reemplaza decisiones que todavía deben seguir viviendo en `VOICE-*` o `UXS-*`.

---

**Estado:** baseline de patrones UI en estado `seed`, con vía operativa explícita para entrada a UI bajo dispensa.
**Precedencia:** este documento depende principalmente de `12`, `13`, `14`, `21`, `23_uxui` y la decisión de dispensa.
**Siguiente capa gobernada:** `07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md` y `23_uxui/UI-RFC/*`.
