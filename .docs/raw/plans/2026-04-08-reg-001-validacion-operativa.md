# REG-001 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `REG-001`.

No reemplaza `UX-VALIDATION-REG-001.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `REG-001`
- caso: registro rápido de humor vía web
- ola operativa actual: `Cohorte A`, sesión compartida con `ONB-001`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-REG-001.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-REG-001.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-REG-001.md`
  - `.docs/wiki/23_uxui/UXS/UXS-REG-001.md`

## Hipótesis bajo test

1. La persona entiende la escala sin explicación adicional.
2. El registro se percibe como gesto instantáneo y no como mini formulario emocional.
3. El valor elegido se siente suficiente sin pedir interpretación extra.
4. La confirmación posterior al guardado es breve pero deja certeza.
5. Los estados sensibles de error, sesión expirada y consentimiento faltante se leen con dignidad y sin dramatismo.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco mobile-first
- duración objetivo: 20 a 30 minutos
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `ONB-001`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en elección, guardado y estados sensibles
  - capturas de pantalla si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas que podrían usar Bitácora como registro cotidiano breve;
- sin uso previo de este flujo;
- con comodidad básica en interacciones móviles de un solo gesto;
- preferentemente con contexto real o plausible de seguimiento emocional o acompañamiento terapéutico.

### Exclusiones

- equipo interno;
- diseño, producto o desarrollo;
- amistades que solo puedan “aprobar” la interfaz;
- personas que ya conozcan la escala o el flujo por trabajo previo sobre el producto.

### Fallback aceptable

Si el target real no estuviera disponible en la primera ronda, se acepta un proxy solo como exploratorio y debe quedar marcado como insuficiente para cierre formal.

## Script de moderación

### Apertura

- explicar que se está validando claridad y ritmo del producto, no evaluando a la persona;
- pedir que piense en voz alta;
- aclarar que puede frenar en cualquier momento;
- si se graba, pedir consentimiento explícito antes de empezar.

### Tareas principales

1. Abrí el registro y contame qué entendés que te pide.
2. Elegí el valor que usarías ahora y seguí hasta terminar.
3. Al final, explicame qué creés que quedó guardado.
4. Si aparece un problema para guardar o para seguir, contame qué entendés y qué harías.

### Preguntas de sondeo

- ¿La escala te resultó clara o tuviste que interpretarla demasiado?
- ¿En algún momento esto se sintió como formulario?
- ¿La confirmación te alcanzó para saber que el dato quedó guardado?
- ¿Qué parte te dio más confianza y cuál menos?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` entrada | se entiende rápido qué se va a hacer |  |  | `VOICE-REG-001` o `UXS-REG-001` |  |
| `S02` default | la escala se lee sin explicación |  |  | `UXS-REG-001` |  |
| `S02` guardado | el gesto principal se siente instantáneo |  |  | `UXS-REG-001` |  |
| `S02` estados sensibles | error, sesión y consentimiento se entienden sin dramatismo |  |  | `VOICE-REG-001` o `UXS-REG-001` |  |
| `S03` confirmación | cierre factual con certeza suficiente |  |  | `VOICE-REG-001` o `UXS-REG-001` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide completar el registro o genera una lectura errónea del valor guardado;
- mayor: no bloquea a todas las personas, pero agrega fricción relevante o vuelve ambigua la comprensión de guardado o de un estado sensible;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-onb-reg-001/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-onb-reg-001/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-REG-001.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-REG-001.md`
- jerarquía, estados, secuencia o interacción -> `UXS-REG-001.md`
- si el hallazgo cambia un patrón reusable de registros rápidos -> revisar además `12_lineamientos_interfaz_visual.md` o `13_voz_tono.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-REG-001.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `REG-001`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-REG-001.md` una vez exista evidencia observada.
