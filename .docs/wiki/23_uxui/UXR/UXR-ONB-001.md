# UXR-ONB-001 — ONB-first del paciente con contexto de invitación opcional

## Propósito

Este documento define la capa de investigación UX del primer slice web de Bitácora.

No describe todavía layout ni contratos de componente. Su función es fijar el problema humano que debe resolver la entrada `ONB-first` antes de bajar a intención, journey, voz, spec, `UI-RFC` y handoff.

## Relación con el canon

Este documento depende de:

- `10_manifiesto_marca_experiencia.md`
- `11_identidad_visual.md`
- `12_lineamientos_interfaz_visual.md`
- `13_voz_tono.md`
- `15_handoff_operacional_uxui.md`
- `03_FL/FL-ONB-01.md`
- `04_RF/RF-ONB-001.md`
- `04_RF/RF-ONB-003.md`

Y prepara directamente:

- `../UXI/UXI-ONB-001.md`
- `../UJ/UJ-ONB-001.md`
- `../VOICE/VOICE-ONB-001.md`
- `../UXS/UXS-ONB-001.md`

No debe contradecir:

- la promesa “tu espacio seguro, vos decidís”;
- el hard gate de consentimiento previo al primer dato clínico;
- la semántica de `invite_token` opcional y `resume_pending_invite=true|false`;
- que el acceso profesional no es automático por aceptar consentimiento.

## Problem statement

La primera experiencia web de Bitácora tiene que resolver dos lecturas a la vez sin partirse en dos productos:

- entrada estándar para una persona que recién llega;
- entrada contextual para alguien que llegó por una invitación profesional.

El problema UX no es sólo “mostrar una home”. Es evitar que la persona interprete Bitácora como una herramienta del profesional, que pierda contexto al autenticarse, o que llegue al consentimiento sin entender por qué sigue teniendo control.

## Actor principal

La persona principal de este caso es:

- paciente nuevo o sin uso web previo;
- que llega a la primera entrada pública del producto;
- con o sin contexto de invitación válido.

No es el actor principal:

- el profesional;
- Telegram;
- el paciente ya activo dentro de registro diario;
- la implementación del primer `MoodEntry`, que pertenece al slice siguiente.

## Contexto de uso

La app web todavía no existe como runtime implementado en el repo, pero el backend productivo sí existe y ya obliga una secuencia real:

1. autenticación;
2. `POST /api/v1/auth/bootstrap`;
3. consentimiento;
4. primer registro.

Eso vuelve crítica la primera capa visible: la UI tiene que preparar bien esa secuencia antes de que exista el frontend real, porque después `T04/T05` no deberían inventar la historia ni la jerarquía del caso.

## Señales y evidencia ya presentes

### Señales de flujo y runtime

- `FL-ONB-01` ya fija bootstrap con `invite_token` opcional, consentimiento obligatorio y continuidad hacia el primer registro.
- `RF-ONB-001` deja explícito que `invite_token` inválido o expirado no bloquea bootstrap: sólo cae `resume_pending_invite=false`.
- `RF-ONB-003` fija consentimiento como hard gate y devuelve `needs_first_entry=true`.
- El backend actual ya expone `POST /api/v1/auth/bootstrap`, `GET /api/v1/consent/current` y `POST /api/v1/consent`.

### Señales de drift visual detectadas

- el material previo cargaba demasiado peso sobre el consentimiento;
- la variante invitada no resolvía con suficiente claridad el contexto humano del vínculo;
- el cierre posterior al consentimiento tendía a volverse más solemne o lírico de lo que permite el canon;
- la autoridad visual seguía bloqueada por el gate Stitch viejo aunque el proyecto ya necesitaba abrir implementación web cuanto antes.

## Hipótesis

Si la primera entrada se construye como una guía personal del paciente, con una sola portada que pueda adaptarse a contexto de invitación, un retorno breve de auth, un consentimiento de resguardo claro y una confirmación con puente directo al primer registro, entonces:

- baja la confusión sobre a quién pertenece el espacio;
- la invitación se entiende como contexto y no como vigilancia;
- el consentimiento se percibe como control explícito, no como pared legal;
- y `T04/T05` pueden implementar sin rediseñar la experiencia.

## Riesgo UX si no se trabaja

Si este problema queda sin cerrar explícitamente:

- la variante invitada puede sentirse como una home separada o como una pantalla del profesional;
- auth/bootstrap puede introducir un corte técnico y frío;
- el consentimiento puede volver a cargar más peso del necesario;
- y la implementación web puede nacer con decisiones visuales contradictorias entre docs.

## Señales de éxito

La señal principal de éxito para este slice es:

- la persona llega a la confirmación de consentimiento con intención clara de hacer su primer registro.

Señales observables secundarias:

- puede explicar que Bitácora sigue siendo su espacio;
- entiende por qué llegó si existe una invitación;
- no interpreta la invitación como acceso automático del profesional;
- reconoce que el siguiente paso natural es su primer registro, no otra configuración.

## Boundaries and defaults

Este `UXR` adopta como defaults explícitos:

- el slice visible termina en `confirmación + puente` al primer registro;
- el primer `MoodEntry` queda fuera del paquete visual de esta tarea;
- la invitación es una variante contextual de la misma entrada, no una experiencia paralela;
- el tono buscado es humano y cálido, con resguardo claro en momentos sensibles;
- la fricción principal a resolver es confusión por invitación/contexto.

## Criterio de validación rápida

Este `UXR` está bien planteado si:

- deja claro que el problema central es propiedad percibida + continuidad;
- explica por qué la invitación no puede dominar la entrada;
- nombra explícitamente el límite del slice;
- y prepara `UXI/UJ/VOICE/UXS` sin obligarlos a reabrir la historia principal.

Este `UXR` está mal planteado si:

- reduce el caso a consentimiento aislado;
- sigue tratando la entrada invitada como único camino;
- o confunde el slice visual con la implementación del primer registro.

---

**Estado:** slice UXR activo para `ONB-001`.
**Precedencia:** depende del canon global y de `FL/RF` del onboarding real.
**Siguiente capa gobernada:** `../UXI/UXI-ONB-001.md`, `../UJ/UJ-ONB-001.md`, `../VOICE/VOICE-ONB-001.md` y `../UXS/UXS-ONB-001.md`.
