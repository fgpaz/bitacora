# UXR-ONB-001 — Onboarding Invitado del Paciente

## Propósito

Este documento define la capa de investigación UX del primer caso visible de Bitácora.

No describe pantallas, componentes ni contratos de interacción. Su función es documentar por qué existe este caso, qué problema humano intenta resolver, qué señales ya tenemos y qué debe aprender la cadena posterior antes de diseñar la journey, la voz específica o la spec de interacción.

En este proyecto, `UXR` documenta el problema antes que la pantalla.

## Relación con el canon

Este documento depende de:

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `14_metodo_prototipado_validacion_ux.md`
- `15_handoff_operacional_uxui.md`

Y prepara directamente:

- `../UXI/UXI-ONB-001.md`
- `../UJ/UJ-ONB-001.md`
- `23_uxui/VOICE/*`
- `../UXS/UXS-ONB-001.md`

No debe contradecir:

- la promesa “tu espacio seguro, vos decidís”;
- la prioridad de seguridad y privacidad por encima de velocidad;
- el consentimiento previo obligatorio antes del primer registro;
- la regla de acceso profesional revocable y `can_view_data=false` por defecto.

## Problem statement

Cuando una persona llega a Bitácora mediante una invitación de su profesional, necesita poder continuar el onboarding hasta su primer registro sin sentir que entra a una herramienta controlada por terceros.

El problema UX no es solo completar pasos. Es sostener continuidad en un flujo que combina:

- contexto clínico;
- vínculo profesional previo;
- consentimiento obligatorio;
- y primer acto de registro personal.

Si la experiencia se percibe demasiado ajena, demasiado clínica o demasiado orientada al profesional, la continuidad del flujo se rompe antes de que la persona llegue al valor real del producto.

## User / actor

La persona principal de este caso es:

- paciente nuevo;
- sin cuenta previa activa en Bitácora;
- que recibe una invitación válida de su profesional;
- y entra al onboarding desde ese contexto de vínculo.

No es el actor principal de este `UXR`:

- el profesional que emite la invitación;
- la persona que se auto-registra sin invitación;
- la persona que ya tenía cuenta y solo acepta un vínculo existente;
- la auto-vinculación por código.

## Usage context

La persona llega al flujo con un contexto distinto al onboarding “frío”.

Ya existe una relación previa con su profesional o, como mínimo, una expectativa de trabajo compartido. Eso hace que el primer contacto con Bitácora cargue preguntas silenciosas como:

- si este espacio es realmente suyo;
- si sus datos ya van a quedar visibles para otra persona;
- si está entrando a una herramienta clínica externa o a un registro propio;
- si el consentimiento es una formalidad o una decisión real.

El onboarding ocurre además en una situación donde la fricción legal es aceptada por diseño: el consentimiento informado es un hard gate previo al primer dato.

## Evidence or signal

Las señales ya presentes en el proyecto muestran que este caso no es accesorio sino estructural.

### Señales de producto y arquitectura

- El onboarding del MVP fue definido como `híbrido (profesional crea o paciente se auto-registra)`.
- El mecanismo principal del MVP es un vínculo persistente con consentimiento revocable y auditoría.
- El consentimiento informado fue definido como hard gate obligatorio antes del primer registro.

### Señales de flujo y prueba

- `FL-ONB-01` ya contempla reanudación automática del onboarding cuando la persona llega con `invite_token`.
- `FL-VIN-01` deja explícito que la invitación completada crea un `CareLink` activo con `can_view_data=false`.
- `TP-ONB` ya trata como criterio importante la continuidad desde bootstrap invitado hasta primer `MoodEntry`.

### Señales de investigación

- La investigación sobre sharing profesional indica que el modelo principal del MVP es acceso delegado con consentimiento.
- También deja explícito que el consentimiento revocable y el control del paciente son parte del núcleo del diseño, no un detalle posterior.

## Current friction

Aunque el caso funcional está documentado, la experiencia todavía tiene una fricción UX de base no resuelta explícitamente.

La persona puede interpretar el flujo de tres maneras problemáticas:

- como una herramienta “del profesional” más que un espacio propio;
- como un trámite legal antes de empezar, sin comprender el valor personal del registro;
- como un onboarding compuesto por piezas separadas en lugar de una continuidad clara hasta el primer registro.

El riesgo crece porque la entrada invitada mezcla en pocos pasos:

- vínculo profesional;
- creación de cuenta;
- consentimiento;
- y primer registro de humor.

Sin una hipótesis UX explícita, es fácil que la futura journey optimice pasos pero no la lectura emocional del flujo.

## Hypothesis

Si el onboarding invitado comunica desde el inicio que Bitácora sigue siendo un espacio personal y controlado por la propia persona, aun cuando exista contexto profesional, entonces la continuidad hasta el primer registro mejora.

Esa mejora debería ocurrir porque la persona:

- entiende por qué fue invitada;
- no asume acceso automático de su profesional a sus datos;
- percibe el consentimiento como una decisión real y no como una trampa de entrada;
- y llega al primer registro sin sentir que perdió autonomía.

## UX risk if unchanged

Si este problema no se trabaja explícitamente, Bitácora corre varios riesgos de experiencia:

- abandono antes del primer registro;
- percepción de vigilancia o exposición indebida;
- lectura equivocada del vínculo profesional;
- debilitamiento de la promesa “tu espacio seguro, vos decidís”;
- journeys futuras demasiado centradas en eficiencia y no en resguardo.

El riesgo no es solo de conversión. También es de confianza clínica y coherencia con el manifiesto.

## Success metric or observable signal

La señal principal de éxito para este caso es:

- la persona llega a crear su primer `MoodEntry`.

Señales observables secundarias:

- no aparecen dudas visibles sobre si el espacio sigue siendo suyo;
- no interpreta la invitación como acceso automático del profesional a sus datos;
- atraviesa consentimiento y primer registro como partes de una misma continuidad, no como cortes arbitrarios;
- puede explicar con sus palabras que el acceso profesional sigue bajo su control.

## Linked `FL-*`

- `FL-ONB-01` — onboarding completo del paciente
- `FL-VIN-01` — invitación profesional a paciente
- `FL-CON-01` — otorgamiento de consentimiento informado
- `FL-REG-01` — registro de humor vía web

## Candidate `UJ-*`

Journey candidata:

- onboarding invitado del paciente desde apertura de invitación hasta primer registro confirmado.

Esa `UJ` futura deberá observar especialmente:

- el primer encuadre del vínculo profesional;
- el paso por consentimiento;
- la transición al primer registro;
- y el momento en que la persona confirma que sigue teniendo control sobre sus datos.

## Boundaries and defaults

Este `UXR` adopta como defaults explícitos:

- la cohorte principal es paciente nuevo con invitación válida;
- el foco principal es continuidad del flujo;
- la lectura dominante buscada es autonomía con contexto profesional;
- la fricción legal del consentimiento no se elimina, se trabaja desde claridad y continuidad;
- el caso no incluye Telegram, auto-vinculación por código ni aceptación de vínculo por paciente ya registrado.

## Criterio de validación rápida

Este `UXR` está bien planteado si:

- describe un problema humano y no una pantalla;
- deja claro que el caso es onboarding invitado y no onboarding genérico;
- puede explicar por qué la continuidad depende de autonomía percibida;
- usa señales ya existentes del repositorio;
- y deja preparado un `UXI-ONB-001.md` sin obligar a inventar el conflicto central del caso.

Este `UXR` está mal planteado si:

- se vuelve un resumen funcional de `FL-ONB-01`;
- reduce el caso a consentimiento legal aislado;
- habla solo de conversión sin confianza;
- o deja ambiguo si la experiencia es para la persona o para el profesional.

---

**Estado:** primer caso UXR activo del canon nuevo.
**Precedencia:** este documento depende de `10-15`.
**Siguiente capa gobernada:** `../UXI/UXI-ONB-001.md`, `../UJ/UJ-ONB-001.md`, `../VOICE/VOICE-ONB-001.md` y `../UXS/UXS-ONB-001.md`.
