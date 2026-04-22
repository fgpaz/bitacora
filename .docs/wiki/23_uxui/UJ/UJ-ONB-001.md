# UJ-ONB-001 — Journey del ONB-first del paciente

## Propósito

Este documento modela la tarea completa que vive la persona desde la entrada pública de Bitácora hasta la confirmación que la empuja a su primer registro.

No resuelve todavía el contrato de componente. Su función es traducir `UXR` y `UXI` a una secuencia visible, estable y trazable para `VOICE`, `UXS`, `UI-RFC` y handoff.

## Relación con el canon

Este documento depende de:

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `../UXR/UXR-ONB-001.md`
- `../UXI/UXI-ONB-001.md`

Y prepara directamente:

- `../VOICE/VOICE-ONB-001.md`
- `../UXS/UXS-ONB-001.md`

No debe contradecir:

- la prioridad de paciente primero;
- una sola acción dominante por contexto;
- el gate de consentimiento previo;
- la continuidad real del backend `bootstrap -> consent -> first entry`.

## Goal del usuario

La persona quiere empezar rápido, entender por qué está acá y no perder control sobre sus datos mientras se acerca a su primer registro.

No llega para aprender el sistema completo ni para gestionar todavía el vínculo profesional. Llega para dar el primer paso correcto.

## Feeling global del journey

El journey debe sentirse:

- cálido;
- simple;
- seguro sin ruido;
- continuo;
- centrado en la persona;
- serio sin volverse pesado.

La anti-sensación dominante a evitar es:

**una mezcla de landing, trámite y admisión clínica.**

## Main path

### S01 — Entrada pública y primer encuadre

La persona entra a Bitácora y encuentra una portada con una sola dirección dominante.

- si llega sin contexto especial, ve una entrada estándar guiada por `Ingresar` (label canónico desde 2026-04-22; antes `Empezar ahora`);
- si llega por invitación, ve la misma portada con `hero adaptado` y contexto explícito de vínculo + propósito;
- si faltan datos del vínculo, la variante sigue visible de forma genérica y no desaparece.

### S02 — Retorno de auth/bootstrap

Después de autenticarse, el sistema no corta la experiencia con un estado técnico.

La persona vive un interstitial breve que confirma continuidad:

- ya entró;
- el sistema prepara su espacio;
- el siguiente paso es revisar consentimiento.

### S03 — Consentimiento con resguardo claro

La persona llega al consentimiento como única pausa deliberada.

Este paso debe dejar claro:

- por qué aparece ahora;
- qué control conserva;
- que aceptar no activa acceso automático del profesional;
- y que, si llegó invitada, ese contexto sigue existiendo pero no domina la decisión.

### S04 — Confirmación + puente

> **Deprecado 2026-04-22**: S04 fue eliminado del journey real. Tras aceptar consentimiento, el frontend hace `window.location.assign('/dashboard')` sin pantalla intermedia. El paciente cae directo en el empty state del dashboard (`S05-DASHBOARD-EMPTY`) y el CTA de entrada al primer registro es `Registrar humor`, que abre un modal inline (`MoodEntryDialog`) sin sacar al paciente del historial. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

Tras aceptar consentimiento, la experiencia no celebra ni agrega ceremonia.

La persona recibe una confirmación serena y un puente inequívoco:

- `Hacer mi primer registro`

Ese CTA cierra el slice visible y deja preparado el siguiente tramo funcional.

## Stable steps

| Step | Nombre | Qué vive la persona | Sensación objetivo | Riesgo si falla |
| --- | --- | --- | --- | --- |
| `S01` | Entrada estándar o contextual | entiende rápido qué es Bitácora y por qué llegó | guía personal + claridad | sentir que entra a algo ajeno |
| `S02` | Auth/bootstrap return | percibe continuidad sin corte técnico | continuidad cálida | trámite o confusión post-auth |
| `S03` | Consentimiento | hace explícito su control y acepta la versión vigente | resguardo claro | muro legal, vigilancia o pérdida de contexto |
| `S04` | Confirmación + puente | entiende que ya puede hacer su primer registro | impulso sereno | cierre ambiguo o solemne |

## Variant / error paths

| Condición | Qué vive la persona | Respuesta esperada del journey |
| --- | --- | --- |
| Invitación con datos completos | la portada adapta hero y propósito | baja la confusión sin convertirse en experiencia paralela |
| Invitación sin datos suficientes | hero adaptado genérico | conserva contexto y evita fallback abrupto |
| Invitación expirada o inválida | no puede continuar por ese contexto | explica claro y vuelve a una entrada comprensible sin dramatizar |
| Falla de auth/bootstrap | continuidad rota por una causa técnica | se informa con dignidad y permite reintento sin cambiar el tono |
| Confusión por contexto de invitación | la persona duda quién acompaña o qué significa “compartido” | el sistema aclara vínculo + propósito antes de pedir la acción sensible |
| Conflicto de versión de consentimiento | la versión vigente cambió | recentra foco y exige revisar la versión actual |

## Momentos sensibles

Los momentos más sensibles del journey son:

- el hero adaptado por invitación;
- el interstitial breve de retorno;
- el consentimiento;
- el puente final.

Lo sensible no viene sólo por legalidad. Viene por percepción de propiedad, continuidad y resguardo.

## Critical steps que requieren `UXS`

Este caso no puede bajar a `UI-RFC` sólo con un paso crítico aislado.

La `UXS` debe cubrir el state pack completo del slice:

- `S01` entrada estándar;
- `S01` entrada invitada explícita;
- fallback genérico de invitación;
- `S02` interstitial;
- `S03` consentimiento base y con recordatorio contextual;
- `S03` fricción principal por invitación/contexto;
- `S04` confirmación + puente.

## Implicancias para capas siguientes

### Hacia `VOICE`

La voz tiene que fijar:

- `Ingresar` como CTA principal (label canónico desde 2026-04-22);
- ausencia de camino secundario fuerte;
- wording de vínculo + propósito en hero invitado;
- interstitial breve no técnico;
- confirmación final sin celebración.

### Hacia `UXS`

La spec debe volver operables:

- jerarquía de bloques;
- estados clave desktop/mobile;
- fallback contextual;
- límites exactos del slice frente al futuro primer registro.

## Criterio de validación rápida

Este `UJ` está bien modelado si:

- puede contarse como una sola continuidad;
- la invitación vive como variante del mismo camino;
- el consentimiento no monopoliza el relato;
- el slice termina con un puente claro al siguiente valor.

Este `UJ` está mal modelado si:

- vuelve a contar un onboarding hasta primer mood completo;
- deja al hero invitado como apéndice menor;
- o corta la experiencia con auth o consentimiento demasiado técnicos.

---

**Estado:** journey UX activo para `ONB-001`.
**Precedencia:** depende de `../UXR/UXR-ONB-001.md` y `../UXI/UXI-ONB-001.md`.
**Siguiente capa gobernada:** `../VOICE/VOICE-ONB-001.md` y `../UXS/UXS-ONB-001.md`.
