# VIS-001 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `VIS-001`.

No reemplaza `UX-VALIDATION-VIS-001.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `VIS-001`
- caso: timeline longitudinal del paciente
- ola operativa actual: `Cohorte E`, sesión compartida con `EXP-001`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIS-001.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIS-001.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-VIS-001.md`
  - `.docs/wiki/23_uxui/UXS/UXS-VIS-001.md`

## Hipótesis bajo test

1. El timeline se percibe como lectura acompañada y no como dashboard analítico.
2. La persona entiende qué período está viendo antes de tocar filtros.
3. El chart principal aporta valor visible rápido.
4. El ajuste de período no rompe el hilo de lectura.
5. El estado vacío se entiende como ausencia de datos y no como error.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco `mobile-first`
- duración objetivo: 18 a 22 minutos dentro de la cohorte compartida
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `EXP-001`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en lectura inicial, cambio de período y comprensión del vacío/error
  - capturas de pantalla si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas que podrían revisar su historial de registros de bienestar o humor;
- sin uso previo de este flujo;
- con comodidad básica en lectura móvil y navegación simple;
- preferentemente con contexto real o plausible de seguimiento personal donde un historial longitudinal tenga sentido.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades que solo puedan “aprobar” la interfaz;
- personas ya expuestas al flujo o al vocabulario interno del producto.

### Fallback aceptable

Si el target real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad de lectura y no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Abrí este timeline y contame qué entendés que estás viendo.
2. Antes de tocar nada, decime cuál creés que es el período actual.
3. Cambiá el período y contame si seguís entendiendo el gráfico.
4. Si no hubiera registros en el período, explicame qué entenderías que pasó.
5. Si quisieras sacar una copia de tus datos, mostrame qué camino tomarías.

### Preguntas de sondeo

- ¿El gráfico se sintió útil o técnico?
- ¿Qué te hizo entender el período actual?
- ¿Los filtros aparecieron en el momento justo o demasiado pronto?
- ¿El vacío te sonó a pausa o a fallo del sistema?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` lectura inicial | el chart se entiende antes que controles accesorios |  |  | `VOICE-VIS-001` o `UXS-VIS-001` |  |
| `S02` período | el ajuste no rompe el hilo de lectura |  |  | `UXS-VIS-001` |  |
| `S02` tono | el timeline se siente legible y propio |  |  | `VOICE-VIS-001` |  |
| `S03` vacío | la ausencia de datos se distingue del error |  |  | `VOICE-VIS-001` o `UXS-VIS-001` |  |
| salida secundaria | la exportación aparece sin robar protagonismo |  |  | `UXS-VIS-001` o `UXS-EXP-001` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide entender el historial o confunde error con ausencia de datos;
- mayor: no bloquea a todas las personas, pero vuelve el timeline demasiado técnico o difícil de leer;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-vis-001-exp-001/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-vis-001-exp-001/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIS-001.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-VIS-001.md`
- jerarquía, estados, secuencia o interacción -> `UXS-VIS-001.md`
- si el hallazgo cambia un patrón reusable de lectura longitudinal -> revisar además `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md` o `16_patrones_ui.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-VIS-001.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `VIS-001`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIS-001.md` una vez exista evidencia observada.
