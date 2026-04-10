# UJ-ONB-001 — Journey del Onboarding Invitado del Paciente

## Propósito

Este documento modela la tarea completa que vive la persona cuando llega a Bitácora mediante una invitación profesional y avanza hasta su primer registro de humor.

No describe pantallas aisladas ni resuelve todavía el contrato preciso de una interacción crítica. Su función es traducir `UXR-ONB-001.md` y `UXI-ONB-001.md` a un recorrido completo, con ritmo, pasos estables, fricciones aceptables, errores relevantes y momentos sensibles.

En este proyecto, `UJ` describe qué vive la persona de principio a fin, no cómo se implementa una pantalla.

## Relación con el canon

Este documento depende de:

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `../UXR/UXR-ONB-001.md`
- `../UXI/UXI-ONB-001.md`

Y prepara directamente:

- `23_uxui/VOICE/*`
- `../UXS/UXS-ONB-001.md`

No debe contradecir:

- la promesa “tu espacio seguro, vos decidís”;
- la prioridad de baja fricción en onboarding y formularios;
- la necesidad de seguridad implícita desde el inicio;
- el consentimiento como momento donde el control se vuelve explícito;
- la regla de acceso profesional revocable y `can_view_data=false` por defecto.

## Goal del usuario

La persona quiere empezar rápido, entender solo lo necesario y llegar a registrar cómo se siente sin perder control sobre sus datos.

No llega al flujo para “configurar una cuenta” ni para “gestionar un vínculo”. Llega para empezar a usar una herramienta que le fue recomendada o compartida por su profesional, pero que debe sentirse propia desde el primer momento.

## Feeling global del journey

El journey completo debe sentirse:

- ligero;
- continuo;
- simple;
- seguro de forma implícita;
- claro sin sobreexplicación;
- centrado en la persona, no en el profesional.

La anti-sensación dominante a evitar es:

**un trámite ajeno que frena demasiado antes de dejar empezar.**

## Ritmo del journey

Este recorrido debe tener un ritmo casi uniforme, liviano y poco intrusivo.

### Tramos que deben sentirse más ligeros

- apertura de la invitación;
- autenticación y bootstrap;
- paso al primer registro;
- confirmación posterior al primer mood.

### Única pausa deliberada

La única pausa consciente del journey ocurre en el consentimiento.

Esa pausa debe:

- ser breve;
- hacer explícito el control del paciente;
- explicar lo justo sobre compartición y acceso;
- no volverse una ceremonia legal pesada.

Fuera de ese punto, el journey no debe frenar.

## Main path

### S01 — Apertura de invitación y primer encuadre

La persona abre el link de invitación y entiende rápidamente que:

- fue invitada por su profesional;
- está entrando a Bitácora;
- el espacio sigue siendo suyo;
- puede avanzar sin sentirse observada.

Este paso debe instalar contexto sin instalar vigilancia.

### S02 — Autenticación y bootstrap sin pérdida de contexto

La persona crea o confirma su acceso sin perder el hilo del journey.

Debe percibir que:

- el flujo sigue siendo uno solo;
- no fue “sacada” de la invitación;
- no necesita entender arquitectura, vínculo ni estados internos;
- el sistema conserva contexto y la acerca al primer uso real.

### S03 — Consentimiento como explicitación de control

La persona pasa por consentimiento como única pausa deliberada del recorrido.

Este momento debe dejar claro que:

- el registro no empieza antes del consentimiento;
- compartir datos no es automático;
- el control sigue del lado del paciente;
- aceptar no significa quedar expuesta sin reversibilidad.

La tarea en este paso no es solo “aceptar”, sino aceptar con comprensión suficiente y sin caída brusca del ritmo general.

### S04 — Primer registro de humor como llegada al valor

Apenas termina el consentimiento, la persona llega al primer registro.

Ese pasaje debe sentirse inmediato.

La experiencia aquí debe transmitir:

- “ya empecé”;
- “me pidieron solo lo necesario”;
- “esto realmente sirve para registrar cómo estoy”.

### S05 — Confirmación serena y continuidad futura

Después del primer mood, la persona recibe una confirmación breve y clara.

La salida correcta del journey no es una celebración. Es una confirmación serena de que:

- el inicio ya ocurrió;
- el registro fue simple;
- el sistema quedó listo para continuidad futura.

## Stable steps `S01..Sn`

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| `S01` | Apertura invitada | abre invitación y entiende dónde está entrando | claridad rápida + espacio propio | interpretar Bitácora como herramienta del profesional |
| `S02` | Auth / bootstrap | crea acceso sin perder contexto | continuidad fluida | sentir corte, desorientación o trámite |
| `S03` | Consentimiento | entiende y acepta el control explícito antes del primer dato | pausa breve + control legible | sentir carga legal, vigilancia o presión |
| `S04` | Primer mood | registra su humor por primera vez | llegada inmediata al valor | sentir demasiados pasos antes de empezar |
| `S05` | Confirmación y continuidad | cierra el inicio y entiende que puede seguir | alivio sereno + simpleza | sensación de cierre ambiguo o esfuerzo excesivo |

## Fricciones

### Fricción aceptable

Se acepta fricción cuando:

- la exige el consentimiento previo;
- mejora claridad sobre control y compartición;
- evita ambigüedad sobre quién puede ver datos y cuándo.

### Fricción indebida

No se acepta fricción que provenga de:

- pérdida de contexto entre invitación y auth;
- pasos que repiten información sin agregar comprensión;
- formularios más largos de lo indispensable;
- explicaciones extensas sobre seguridad antes de que la persona experimente valor;
- demora innecesaria entre consentimiento y primer registro.

## Variant / error path

| Condición | Qué vive la persona | Respuesta esperada del journey |
| --- | --- | --- |
| Invitación expirada | llega con intención pero no puede continuar por ese link | el flujo corta con claridad, sin dramatizar, y orienta a pedir una nueva invitación |
| Falla de auth | intenta entrar pero el acceso no se completa | el sistema falla en cerrado, explica breve y permite reintento sin perder dignidad |
| Rechazo o abandono en consentimiento | no acepta o deja el flujo en la pausa deliberada | no avanza a registro, pero tampoco se la presiona ni se vuelve punitivo |
| Abandono antes del primer mood | ya dio consentimiento pero no termina el primer registro | al volver, el flujo retoma cerca del valor, no desde un circuito completo |
| Pérdida de sensación de control | interpreta que el profesional ya ve sus datos | el journey debe recentrar control explícito antes de dejarla continuar |

## Momentos sensibles

Los momentos más sensibles del journey son:

- el primer encuadre de la invitación;
- el consentimiento;
- la transición inmediata al primer registro.

Son sensibles por razones distintas:

- `S01` define si el sistema se percibe propio o ajeno;
- `S03` define si la confianza se vuelve explícita sin romper ritmo;
- `S04` define si el producto entrega valor real enseguida o si sigue pidiendo esfuerzo.

## Critical steps que requieren `UXS`

### Paso crítico principal

`S03 — Consentimiento como explicitación de control`

Debe ser el primer candidato a `UXS-ONB-001.md` porque concentra:

- claridad sensible;
- postura de confianza;
- posible quiebre de continuidad;
- y el único freno deliberado del journey.

### Paso sensible secundario

`S01 — Apertura de invitación y primer encuadre`

Debe bajar a `VOICE` de forma explícita y podría requerir spec posterior si el framing inicial concentra demasiada ambigüedad o riesgo de lectura vigilante.

## Implicancias para pasos siguientes

### Hacia `23_uxui/VOICE/*`

La voz del caso deberá resolver, como mínimo:

- cómo se presenta la invitación sin que el profesional domine la escena;
- cómo se explica consentimiento sin tono legalista pesado;
- cómo se confirma el primer registro sin celebración ni juicio.

### Hacia `UXS-ONB-001.md`

La primera spec UX de este caso deberá tomar `S03` y resolver:

- jerarquía de información;
- copy;
- estados y errores;
- claridad de control;
- continuidad hacia `S04`.

## Defaults transferibles

Este journey adopta como defaults reutilizables para onboarding y formularios del sistema:

- casi nula fricción;
- seguridad implícita;
- simpleza radical;
- una sola pausa deliberada cuando el caso exige control explícito;
- valor visible lo antes posible.

Estos defaults no reemplazan el canon global, pero sí deben tratarse como baseline de producto para experiencias equivalentes salvo excepción explícita.

## Criterio de validación rápida

Este `UJ` está bien modelado si:

- el recorrido puede contarse de principio a fin sin saltos arbitrarios;
- la única pausa fuerte es consentimiento;
- el profesional queda contextualizado y no dominante;
- el primer `MoodEntry` aparece como llegada real al valor;
- y los puntos que necesitan `VOICE` y `UXS` quedan explícitos.

Este `UJ` está mal modelado si:

- parece una secuencia de pantallas en vez de una tarea vivida;
- distribuye fricción pareja en todo el flujo;
- demora demasiado el primer valor;
- trata el consentimiento como trámite aislado;
- o deja ambiguo dónde se rompe la confianza.

---

**Estado:** journey UX del onboarding invitado del paciente.
**Precedencia:** este documento depende de `../UXR/UXR-ONB-001.md` y `../UXI/UXI-ONB-001.md`.
**Siguiente capa gobernada:** `../VOICE/VOICE-ONB-001.md` y `../UXS/UXS-ONB-001.md`, comenzando por `S03`.
