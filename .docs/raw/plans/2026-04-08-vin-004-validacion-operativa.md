# VIN-004 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `VIN-004`.

No reemplaza `UX-VALIDATION-VIN-004.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `VIN-004`
- caso: gestión de acceso profesional por paciente
- ola operativa actual: `Cohorte C`, sesión compartida con `VIN-002`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-004.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-004.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-VIN-004.md`
  - `.docs/wiki/23_uxui/UXS/UXS-VIN-004.md`

## Hipótesis bajo test

1. El estado actual del acceso se entiende antes de interactuar.
2. La persona comprende el efecto del cambio antes de guardar.
3. Activar o restringir acceso se percibe como ajuste reversible y no como revocación del vínculo.
4. La confirmación posterior al guardado deja certeza sin dramatizar.
5. El error recuperable mantiene visible el estado previo y no vuelve técnico el problema.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco `mobile-first`
- duración objetivo: 15 a 20 minutos dentro de la cohorte compartida
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `VIN-002`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en lectura del estado actual, cambio, guardado y error
  - capturas de pantalla si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas que podrían decidir si un profesional puede ver sus registros;
- sin uso previo de este flujo;
- con comodidad básica en configuraciones móviles simples;
- preferentemente con contexto real o plausible de acompañamiento terapéutico donde el control del acceso tenga sentido.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades que solo puedan “aprobar” la interfaz;
- personas ya expuestas al flujo o a la terminología interna del producto.

### Fallback aceptable

Si el target real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad del control y no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Mirá el estado actual y contame qué entendés que puede o no puede hacer este profesional.
2. Cambiá ese estado como si fuera tu decisión hoy.
3. Antes de guardar, explicame qué creés que va a pasar si seguís.
4. Guardá el cambio y contame qué entendés que quedó distinto.
5. Si algo fallara, contame qué esperarías que se mantuviera igual.

### Preguntas de sondeo

- ¿El estado actual te resultó claro de entrada?
- ¿En algún momento esto se sintió como un switch técnico?
- ¿Qué te ayudó a entender la diferencia entre vínculo y acceso?
- ¿La confirmación final te alcanzó para saber qué cambió?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` estado actual | el punto de partida se entiende de inmediato |  |  | `VOICE-VIN-004` o `UXS-VIN-004` |  |
| `S02` cambio | la decisión se entiende como ajuste reversible |  |  | `UXS-VIN-004` |  |
| `S02` efecto | la consecuencia del cambio se entiende antes del guardado |  |  | `VOICE-VIN-004` o `UXS-VIN-004` |  |
| `S03` confirmación | el nuevo estado queda claro sin dramatismo |  |  | `VOICE-VIN-004` o `UXS-VIN-004` |  |
| `S03` error | el fallo permite reintento y conserva contexto |  |  | `VOICE-VIN-004` o `UXS-VIN-004` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide completar el cambio o deja una lectura errónea sobre quién puede ver los datos;
- mayor: no bloquea a todas las personas, pero agrega fricción relevante o vuelve ambigua la consecuencia del cambio;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-vin-002-vin-004/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-vin-002-vin-004/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-004.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-VIN-004.md`
- jerarquía, estados, secuencia o interacción -> `UXS-VIN-004.md`
- si el hallazgo cambia un patrón reusable de control de acceso o reversibilidad -> revisar además `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md` o `16_patrones_ui.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-VIN-004.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `VIN-004`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-004.md` una vez exista evidencia observada.
