# VIS-002 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `VIS-002`.

No reemplaza `UX-VALIDATION-VIS-002.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `VIS-002`
- caso: dashboard multi-paciente del profesional
- ola operativa actual: `Cohorte F`, sesión compartida con `VIN-001`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIS-002.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIS-002.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-VIS-002.md`
  - `.docs/wiki/23_uxui/UXS/UXS-VIS-002.md`

## Hipótesis bajo test

1. El dashboard se percibe como tablero sobrio y acotado.
2. La persona entiende que solo aparecen pacientes con acceso activo.
3. La jerarquía de lectura se sostiene aun con varias tarjetas.
4. Las alertas básicas acompañan sin dramatizar.
5. La paginación no rompe la comprensión del conjunto.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco `desktop-first`
- duración objetivo: `18 a 22 minutos` dentro de la cohorte compartida
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `VIN-001`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en lectura inicial, comprensión del acceso y cambio de página
  - capturas si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- profesionales adultos que podrían revisar un conjunto de pacientes vinculados;
- sin uso previo de este flujo;
- comodidad básica con interfaces web de escritorio y lectura de listados;
- idealmente con contexto real o plausible de seguimiento clínico.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades que solo puedan “aprobar” la interfaz;
- personas ya expuestas al flujo o al vocabulario interno del producto.

### Fallback aceptable

Si el target profesional real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando la claridad del tablero y no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Abrí este dashboard y contame qué entendés que estás viendo.
2. Decime por qué creés que estas personas aparecen acá.
3. Elegí a quién abrirías primero y por qué.
4. Cambiá de página y contame si seguís entendiendo la jerarquía.
5. Si no hubiera pacientes visibles, explicame qué entenderías.

### Preguntas de sondeo

- ¿El dashboard se sintió útil o recargado?
- ¿Qué te hizo entender el límite de acceso?
- ¿Las alertas te parecieron clínicas o exageradas?
- ¿La paginación interrumpió la lectura?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` lectura inicial | la lista se entiende sin verse como pared de tarjetas |  |  | `VOICE-VIS-002` o `UXS-VIS-002` |  |
| `S01` acceso | el límite de visibilidad queda explícito |  |  | `VOICE-VIS-002` |  |
| `S01` jerarquía | la acción hacia detalle es clara |  |  | `UXS-VIS-002` |  |
| `S01` paginación | cambiar de página mantiene el hilo |  |  | `UXS-VIS-002` |  |
| `S02` vacío/error | vacío y error se distinguen con claridad |  |  | `VOICE-VIS-002` o `UXS-VIS-002` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide priorizar pacientes o confunde acceso visible con acceso total;
- mayor: no bloquea a todas las personas, pero vuelve recargada o ambigua la lectura;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-vin-001-vis-002/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-vin-001-vis-002/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIS-002.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-VIS-002.md`
- jerarquía, estados, secuencia o interacción -> `UXS-VIS-002.md`
- si el hallazgo cambia un patrón reusable de lectura profesional -> revisar además `16_patrones_ui.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-VIS-002.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `VIS-002`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIS-002.md` una vez exista evidencia observada.
