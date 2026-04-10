# UXS-ONB-001 — Portada, consentimiento y puente del ONB-first

## Propósito

Este documento fija el contrato UX del slice `ONB-001` como paquete completo.

No describe todavía código ni layout final de implementación. Su función es volver operables la portada `ONB-first`, la variante invitada, el retorno `auth/bootstrap`, el consentimiento y la confirmación con puente al primer registro.

## Relación con el canon

Este documento depende de:

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `16_patrones_ui.md`
- `../UXR/UXR-ONB-001.md`
- `../UXI/UXI-ONB-001.md`
- `../UJ/UJ-ONB-001.md`
- `../VOICE/VOICE-ONB-001.md`
- `../../03_FL/FL-ONB-01.md`
- `../../03_FL/FL-CON-01.md`
- `../../04_RF/RF-ONB-001.md`
- `../../04_RF/RF-ONB-003.md`
- `../../04_RF/RF-CON-001.md`
- `../../04_RF/RF-CON-002.md`
- `../../04_RF/RF-CON-003.md`
- `../../06_pruebas/TP-ONB.md`
- `../../06_pruebas/TP-CON.md`

Y prepara directamente:

- `../PROTOTYPE/PROTOTYPE-ONB-001.md`
- `../UI-RFC/UI-RFC-ONB-001.md`
- `../HANDOFF-SPEC/HANDOFF-SPEC-ONB-001.md`

No debe contradecir:

- la secuencia real `bootstrap -> consent -> needs_first_entry`;
- la regla de una sola acción dominante por contexto;
- el tone set humano y cálido del slice;
- el hard gate de consentimiento.

## Slice dueño

### Nombre

`ONB-001` — entrada `ONB-first` del paciente hasta consentimiento y puente al primer registro.

### Alcance visible

Incluye:

- portada estándar;
- portada con hero adaptado por invitación;
- fallback genérico del hero adaptado;
- interstitial breve de retorno `auth/bootstrap`;
- consentimiento base;
- consentimiento con recordatorio ligero de contexto invitado;
- fricción principal por invitación/contexto;
- conflicto de versión del consentimiento;
- confirmación con CTA al primer registro.

Excluye:

- la implementación del primer `MoodEntry`;
- daily check-in;
- dashboards o pantallas profesionales;
- Telegram.

## Cobertura obligatoria

- desktop;
- mobile;
- estados clave;
- pack ampliado de fricciones con foco principal en confusión por invitación/contexto.

## Sensación del slice

La experiencia debe sentirse como:

- guía personal;
- cálida y seria;
- clara para empezar;
- explícita al hablar de control;
- breve al pasar por auth;
- serena al cerrar.

La anti-sensación principal es:

**“esto parece una herramienta del profesional o una admisión clínica”.**

## Contrato de interacción

### S01 — Portada estándar

#### Objetivo

Presentar Bitácora como espacio personal y abrir el onboarding con una sola acción dominante.

#### Jerarquía obligatoria

1. historia principal de guía personal;
2. CTA principal `Empezar ahora`;
3. soporte dominante de privacidad y resguardo;
4. cualquier camino de retorno queda fuera del hero principal o con prominencia muy baja.

#### Reglas

- no debe existir un CTA secundario fuerte;
- el soporte de privacidad no debe competir con la acción;
- el hero no debe parecer landing institucional.

### S01 — Hero adaptado por invitación

#### Objetivo

Resolver la fricción de contexto sin crear una home separada.

#### Reglas

- la misma portada adapta su hero;
- la señal dominante es `vínculo + propósito`;
- el propósito visible se expresa como `registro inicial con acompañamiento profesional`;
- el detalle visible es explícito: nombre + rol + frase breve de propósito cuando existan datos;
- si faltan datos, usar hero adaptado genérico y no volver al modo estándar.

#### Soporte

- privacidad y resguardo siguen visibles como capa secundaria;
- la invitación puede llegar hasta consentimiento con recordatorio ligero, pero desaparece en la confirmación final.

### S02 — Retorno `auth/bootstrap`

#### Objetivo

Mantener continuidad y evitar un salto técnico.

#### Reglas

- resolver como `interstitial breve`;
- una sola idea principal: se está preparando el espacio para seguir;
- no mostrar progreso técnico, claims ni estados internos;
- no agregar CTA paralelos.

### S03 — Consentimiento

#### Objetivo

Hacer explícito el control con `resguardo claro`.

#### Estructura mínima

1. encabezado breve;
2. resumen de control;
3. texto vigente del consentimiento;
4. confirmación explícita;
5. CTA principal `Aceptar y seguir`.

#### Reglas

- el resumen de control aparece antes del texto completo;
- debe nombrar que aceptar no activa acceso automático del profesional;
- si hubo invitación, mostrar un recordatorio contextual ligero, no dominante;
- el consentimiento sigue siendo una sola columna, claro en desktop y mobile;
- la acción primaria se habilita sólo con lectura + confirmación explícita.

### S03 — Fricción principal por contexto

#### Qué debe resolver

Cuando la persona duda qué significa la invitación o qué implica “acompañamiento profesional”, la UI debe aclarar:

- por qué llegó;
- qué parte del vínculo es contextual;
- y qué sigue bajo control del paciente.

#### Regla

Esta aclaración vive dentro del mismo slice, sin modal paralelo ni desvío largo.

### S03 — Conflicto de versión

- debe mostrarse como estado explícito;
- recentra foco sobre la versión vigente;
- evita reintentos ambiguos;
- mantiene el tono sereno.

### S04 — Confirmación + puente

#### Objetivo

Cerrar el consentimiento y empujar al siguiente valor.

#### Reglas

- confirmación factual breve;
- CTA principal exacto: `Hacer mi primer registro`;
- sin celebraciones;
- sin recordar otra vez la invitación;
- sin presentar el primer formulario dentro de este slice.

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| `hero_standard` | portada `ONB-first` estándar | CTA principal lista para iniciar |
| `hero_invite_explicit` | hero adaptado con vínculo + propósito visibles | baja confusión contextual |
| `hero_invite_fallback` | hero adaptado genérico | conserva contexto aunque falten datos |
| `invite_expired` | contexto no válido o vencido | salida clara y digna |
| `auth_interstitial` | transición breve post-auth | continuidad sin explicación técnica |
| `consent_default` | consentimiento base con resumen de control | prepara aceptación |
| `consent_invite_reminder` | consentimiento con recordatorio ligero del contexto invitado | continuidad contextual sin dominancia |
| `consent_context_clarification` | aclaración por confusión de invitación/contexto | resuelve la fricción principal |
| `consent_version_conflict` | cambio de versión vigente | exige revisar la versión actual |
| `consent_error_retryable` | error recuperable cerca del rail de acción | reintento digno |
| `bridge_ready` | confirmación + CTA al primer registro | cierra el slice y empuja al siguiente |

## Responsive

- mobile-first de una columna;
- desktop conserva una columna dominante con soporte discreto;
- el hero adaptado no debe fragmentarse en varias columnas que rompan la lectura;
- el rail de acción de consentimiento debe seguir siendo inequívoco en ambos tamaños.

## Accesibilidad

- foco visible y estable en todos los estados;
- CTA y checkbox accesibles por teclado;
- estados no comunicados sólo por color;
- el recordatorio contextual de invitación debe ser legible sin depender de iconografía;
- el interstitial debe seguir siendo entendible con movimiento reducido.

## Acceptance criteria

1. La portada estándar deja una sola acción dominante: `Empezar ahora`.
2. No existe un camino secundario fuerte dentro del hero.
3. La variante invitada adapta el mismo hero y muestra vínculo + propósito explícitos.
4. Si faltan datos del vínculo, existe hero adaptado genérico.
5. El retorno `auth/bootstrap` se resuelve con interstitial breve y no técnico.
6. El consentimiento se siente como `resguardo claro` y no como pared legal.
7. La fricción principal por invitación/contexto tiene una resolución visible dentro del slice.
8. La invitación persiste hasta consentimiento con recordatorio ligero y desaparece en confirmación.
9. La confirmación final usa `Hacer mi primer registro` como CTA inequívoco.
10. Desktop y mobile cubren el mismo state pack sin reinterpretación.

## Defaults transferibles

Este `UXS` fija como defaults para implementación posterior:

- hero único con variante contextual;
- secondary paths silenciosos;
- interstitials breves de continuidad;
- consentimiento con resumen de control;
- bridges de siguiente acción sin celebración.

## Criterio de validación rápida

El slice está bien calibrado si:

- guía, contexto y resguardo se leen en ese orden;
- la invitación aclara sin colonizar toda la experiencia;
- el consentimiento no rompe el ritmo;
- el cierre deja una acción siguiente clara.

El slice está mal calibrado si:

- el hero parece institucional;
- la invitación se vuelve una mini-home separada;
- auth o consentimiento se vuelven técnicos;
- o la confirmación pretende cerrar más de lo que el slice muestra.

---

**Estado:** `UXS` activo para `ONB-001`.
**Precedencia:** depende de `UXR`, `UXI`, `UJ`, `VOICE` y `FL/RF` del onboarding real.
**Siguiente capa gobernada:** `../PROTOTYPE/PROTOTYPE-ONB-001.md`, `../UI-RFC/UI-RFC-ONB-001.md` y la cadena `HANDOFF-*`.
