# UXI-ONB-001 — Intención del ONB-first del paciente

## Propósito

Este documento define cómo debe sentirse la entrada web `ONB-first` del paciente.

No describe todavía componentes ni detalle de implementación. Su función es fijar la sensación, la historia líder y la postura de confianza para que journey, voz, spec, `UI-RFC` y handoff no inventen el carácter del slice.

## Relación con el canon

Este documento depende de:

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `../UXR/UXR-ONB-001.md`

Y prepara directamente:

- `../UJ/UJ-ONB-001.md`
- `../VOICE/VOICE-ONB-001.md`
- `../UXS/UXS-ONB-001.md`

No debe contradecir:

- la promesa “tu espacio seguro, vos decidís”;
- el orden `contexto -> acción -> confianza`;
- una sola acción dominante por contexto;
- el consentimiento como explicitación de control;
- la presencia profesional sólo contextual y explícita.

## Sensación deseada

La entrada `ONB-first` debe sentirse:

- humana;
- cálida;
- clara;
- propia;
- poco intimidante;
- cuidada;
- rápida de empezar.

La sensación dominante no es solemnidad clínica. Es `guía personal con resguardo claro`.

## Historia líder

La historia principal del slice es:

**Bitácora te recibe como un espacio personal para empezar a registrar cómo te sentís.**

Todo lo demás acompaña:

- privacidad y resguardo como soporte visible;
- invitación profesional como contexto cuando existe;
- consentimiento como pausa breve para hacer explícito el control;
- confirmación como puente inmediato al primer registro.

## Arquetipo dominante

El arquetipo del caso es `guía serena que abre el camino`.

Eso implica:

- explicar lo justo;
- no usar tono institucional;
- no tratar la invitación como eje dramático;
- sostener continuidad visible sin tutorializar;
- empujar al siguiente paso con calma, no con presión.

## Postura de confianza

La confianza del slice se construye en dos capas.

### En la entrada

- el soporte dominante es privacidad y resguardo;
- la UI debe verse personal antes que clínica;
- el CTA principal debe ser directo: `Ingresar` (label canónico desde 2026-04-22; antes `Empezar ahora`. Va directo a `/ingresar` OIDC+PKCE);
- no debe haber un camino secundario fuerte que compita con la acción principal.

### En la variante invitada

- la portada sigue siendo la misma, pero con `hero adaptado`;
- la señal dominante pasa a ser `vínculo + propósito`;
- el propósito visible debe presentarse como `registro inicial con acompañamiento profesional`;
- si faltan datos del vínculo, el hero adaptado se mantiene de forma genérica y no vuelve al modo estándar puro.

### En el consentimiento

- la confianza se vuelve explícita como `resguardo claro`;
- si la persona llegó invitada, el contexto se recuerda de forma ligera, no dominante;
- el consentimiento no debe sentirse como un muro legal ni como un discurso tranquilizador.

## Anti-sensación

La anti-sensación principal a evitar es:

**“entré a algo ajeno que me explica demasiado y decide por mí”.**

También deben evitarse:

- onboarding más del profesional que de la persona;
- portada institucional o clínica dura;
- confirmaciones solemnes o celebratorias;
- continuidad rota por un retorno de auth técnico y frío.

## Fricción aceptable y fricción indebida

### Fricción aceptable

- una pausa breve para consentimiento;
- una aclaración visible cuando la persona no entiende el contexto de invitación;
- un interstitial corto al volver de auth/bootstrap.

### Fricción indebida

- múltiples caminos con el mismo peso en la portada;
- explicaciones largas antes de que exista una acción clara;
- repetición decorativa de promesas de seguridad;
- cualquier paso extra entre consentimiento y el puente al primer registro.

## Implicancias para la cadena posterior

### Hacia `UJ`

El journey debe salir de esta intención con:

- portada estándar y portada invitada dentro del mismo sistema;
- retorno `auth/bootstrap` como transición breve;
- consentimiento como pausa de resguardo claro;
- > **Deprecado 2026-04-22**: confirmación con CTA `Hacer mi primer registro` — S04-BRIDGE eliminado. El post-consent redirige directo a `/dashboard` y el equivalente operativo es el CTA `Registrar humor` del empty state. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

### Hacia `VOICE`

La voz del slice deberá:

- ser cálida, no blanda;
- usar claridad breve;
- nombrar el vínculo profesional sin dejarlo dominar la pantalla;
- evitar cualquier wording que sobredimensione “compartido” o coordinación clínica.

### Hacia `UXS`

La spec debe volver operables:

- la jerarquía exacta del hero;
- la variante contextual por invitación;
- el fallback genérico de invitación;
- el interstitial de auth;
- el puente final al primer registro.

## Defaults del caso

Este `UXI` adopta como defaults explícitos:

- historia líder: guía personal del paciente;
- tono: humano y cálido;
- CTA principal: `Ingresar` (label canónico desde 2026-04-22);
- sin camino secundario fuerte en hero;
- cobertura obligatoria: desktop + mobile + estados clave;
- profundidad de entrega: casi-`UI-RFC` completo.

## Criterio de validación rápida

Este `UXI` está bien calibrado si el slice se percibe como:

- entrada propia y clara;
- invitación contextual, no invasiva;
- consentimiento breve pero serio;
- continuidad natural hacia el primer registro.

Este `UXI` está mal calibrado si se percibe como:

- landing institucional;
- experiencia del profesional;
- paso legal pesado;
- o serie de pantallas sueltas sin hilo narrativo.

---

**Estado:** intención UX activa para `ONB-001`.
**Precedencia:** depende de `../UXR/UXR-ONB-001.md` y del canon global.
**Siguiente capa gobernada:** `../UJ/UJ-ONB-001.md`, `../VOICE/VOICE-ONB-001.md` y `../UXS/UXS-ONB-001.md`.
