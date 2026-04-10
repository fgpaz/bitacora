# CON-002 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `CON-002`.

No reemplaza `UX-VALIDATION-CON-002.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `CON-002`
- caso: revocación de consentimiento
- ola operativa actual: `Cohorte D`, sesión compartida con `VIN-003`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-CON-002.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-CON-002.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-CON-002.md`
  - `.docs/wiki/23_uxui/UXS/UXS-CON-002.md`

## Hipótesis bajo test

1. La cascada de impacto se entiende antes de confirmar.
2. La revocación se percibe como pausa seria pero no punitiva.
3. La pantalla deja claro que se suspenden registro y accesos.
4. La confirmación final se entiende sin explicación adicional.
5. El error recuperable mantiene claridad sin volver legalista el paso.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco `mobile-first`
- duración objetivo: 15 a 20 minutos dentro de la cohorte compartida
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `VIN-003`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en lectura de la cascada, confirmación y resultado
  - capturas de pantalla si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas que podrían necesitar pausar el consentimiento de uso de sus datos;
- sin uso previo de este flujo;
- con comodidad básica en interacciones móviles breves;
- preferentemente con contexto real o plausible de acompañamiento terapéutico donde consentimiento, registro y acceso profesional sean conceptos relevantes.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades que solo puedan “aprobar” la interfaz;
- personas ya expuestas al flujo o a la terminología interna del producto.

### Fallback aceptable

Si el target real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad del flujo y no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Mirá este estado y contame qué entendés que va a pasar si revocás el consentimiento.
2. Avanzá hasta la confirmación y explicame qué quedaría suspendido.
3. Si vieras que este consentimiento ya estaba revocado, contame qué entenderías.
4. Si algo fallara, contame qué esperarías que siga igual.

### Preguntas de sondeo

- ¿La cascada te resultó clara antes de confirmar?
- ¿Hubo algo que te sonara a amenaza o castigo?
- ¿Qué te hizo entender qué pasaba con el registro y con los accesos?
- ¿La confirmación final te alcanzó para cerrar la decisión?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` entrada | el estado vigente se entiende antes de revisar impacto |  |  | `VOICE-CON-002` o `UXS-CON-002` |  |
| `S02` cascada | la suspensión de registro y accesos se entiende antes de confirmar |  |  | `VOICE-CON-002` o `UXS-CON-002` |  |
| `S02` tono | la pantalla se siente seria pero no punitiva |  |  | `VOICE-CON-002` |  |
| `S03` confirmación | el nuevo estado se entiende sin explicación extra |  |  | `VOICE-CON-002` o `UXS-CON-002` |  |
| `S03` error | el fallo permite reintento y conserva claridad |  |  | `VOICE-CON-002` o `UXS-CON-002` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide completar la revocación o deja una lectura errónea sobre suspensión de registro o acceso;
- mayor: no bloquea a todas las personas, pero agrega fricción relevante o vuelve ambigua la cascada principal;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-vin-003-con-002/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-vin-003-con-002/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-CON-002.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-CON-002.md`
- jerarquía, estados, secuencia o interacción -> `UXS-CON-002.md`
- si el hallazgo cambia un patrón reusable de revocación o consentimiento -> revisar además `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md` o `16_patrones_ui.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-CON-002.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `CON-002`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-CON-002.md` una vez exista evidencia observada.
