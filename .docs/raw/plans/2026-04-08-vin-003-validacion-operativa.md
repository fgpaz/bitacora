# VIN-003 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `VIN-003`.

No reemplaza `UX-VALIDATION-VIN-003.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `VIN-003`
- caso: revocación de vínculo por paciente
- ola operativa actual: `Cohorte D`, sesión compartida con `CON-002`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-003.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-003.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-VIN-003.md`
  - `.docs/wiki/23_uxui/UXS/UXS-VIN-003.md`

## Hipótesis bajo test

1. La persona entiende el impacto del corte antes de confirmar.
2. La revocación se percibe como decisión firme y no como pantalla que mete miedo.
3. La acción secundaria conserva el vínculo sin competir visualmente con la primaria.
4. La confirmación final deja claro que el acceso quedó cortado.
5. El error recuperable mantiene calma y no vuelve técnico el problema.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco `mobile-first`
- duración objetivo: 15 a 20 minutos dentro de la cohorte compartida
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `CON-002`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en lectura del impacto, confirmación y resultado
  - capturas de pantalla si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas que podrían necesitar cortar el vínculo con un profesional;
- sin uso previo de este flujo;
- con comodidad básica en interacciones móviles breves;
- preferentemente con contexto real o plausible de acompañamiento terapéutico donde el control del vínculo tenga sentido.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades que solo puedan “aprobar” la interfaz;
- personas ya expuestas al flujo o al vocabulario interno del producto.

### Fallback aceptable

Si el target real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad del flujo y no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Mirá este vínculo y contame qué entendés que pasaría si lo revocás.
2. Avanzá hasta la confirmación y explicame qué cambió y qué no cambió todavía.
3. Si vieras que este vínculo ya estaba revocado, contame qué entenderías.
4. Si algo fallara, contame qué esperarías que siga igual.

### Preguntas de sondeo

- ¿El impacto te resultó claro antes de tocar la acción principal?
- ¿Hubo algo que te sonara excesivo o dramático?
- ¿La diferencia entre revocar y conservar quedó clara?
- ¿La confirmación final te dio certeza suficiente?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` entrada | el vínculo correcto se entiende antes de decidir |  |  | `VOICE-VIN-003` o `UXS-VIN-003` |  |
| `S02` impacto | la consecuencia se entiende antes de confirmar |  |  | `VOICE-VIN-003` o `UXS-VIN-003` |  |
| `S02` jerarquía | la acción secundaria no compite con la primaria |  |  | `UXS-VIN-003` |  |
| `S03` confirmación | el corte de acceso queda claro sin dramatizar |  |  | `VOICE-VIN-003` o `UXS-VIN-003` |  |
| `S03` error | el fallo permite reintento y conserva contexto |  |  | `VOICE-VIN-003` o `UXS-VIN-003` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide completar la revocación o deja una lectura errónea sobre acceso a datos;
- mayor: no bloquea a todas las personas, pero agrega fricción relevante o vuelve ambigua la consecuencia del corte;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-vin-003-con-002/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-vin-003-con-002/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-003.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-VIN-003.md`
- jerarquía, estados, secuencia o interacción -> `UXS-VIN-003.md`
- si el hallazgo cambia un patrón reusable de revocación o control -> revisar además `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md` o `16_patrones_ui.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-VIN-003.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `VIN-003`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-003.md` una vez exista evidencia observada.
