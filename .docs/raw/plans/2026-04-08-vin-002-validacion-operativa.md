# VIN-002 — Preparación de Validación Operativa

## Propósito

Este documento prepara la ejecución de validación UX del slice `VIN-002`.

No reemplaza `UX-VALIDATION-VIN-002.md` ni declara hallazgos ya observados. Su función es dejar listo el operativo de sesiones reales para que la evidencia posterior pueda escribirse sin inventar nada.

## Slice y referencia canónica

- slice: `VIN-002`
- caso: auto-vinculación paciente a profesional por código
- ola operativa actual: `Cohorte C`, sesión compartida con `VIN-004`
- prototipo canónico: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-002.md`
- referencia navegable mínima: `.docs/wiki/23_uxui/PROTOTYPE/PROTOTYPE-VIN-002.html`
- documentos dueños para sync posterior:
  - `.docs/wiki/23_uxui/VOICE/VOICE-VIN-002.md`
  - `.docs/wiki/23_uxui/UXS/UXS-VIN-002.md`

## Hipótesis bajo test

1. La persona entiende rápido qué hace el código sin sentir que entra a un flujo técnico.
2. El vínculo se percibe como gesto corto y preciso.
3. El mensaje de control alcanza para entender que el vínculo no activa acceso automático a los datos.
4. Los estados de código inválido o expirado se leen con dignidad y sin culpa.
5. La confirmación posterior al vínculo deja claro el nuevo estado sin sonar administrativa.

## Método operativo

- tipo de sesión: moderada, individual
- modalidad preferida: remota con foco `mobile-first`
- duración objetivo: 15 a 20 minutos dentro de la cohorte compartida
- tamaño mínimo: `5 personas`
- regla de cohorte: compartir sesión con `VIN-004`, pero etiquetar hallazgos por slice
- evidencia mínima:
  - notas estructuradas por sesión
  - marcas temporales en ingreso del código, lectura del límite de acceso y confirmación
  - capturas de pantalla si ayudan a fijar el hallazgo
  - grabación solo con permiso explícito

## Participantes válidos

### Perfil buscado

- personas adultas que podrían vincularse con un profesional desde Bitácora;
- sin uso previo de este flujo;
- con comodidad básica en interacciones móviles breves;
- preferentemente con contexto real o plausible de acompañamiento terapéutico donde tenga sentido decidir sobre acceso a sus datos.

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

1. Abrí este flujo y contame qué entendés que va a pasar.
2. Ingresá el código y seguí hasta terminar.
3. Al ver la confirmación, explicame qué creés que cambió y qué no cambió todavía.
4. Si el código no sirviera, contame qué entenderías que pasó y qué harías después.

### Preguntas de sondeo

- ¿La explicación antes del código te alcanzó o te faltó contexto?
- ¿En algún momento sentiste que estabas haciendo algo técnico?
- ¿Qué te hizo entender que el acceso a tus datos seguía bajo tu control?
- ¿Qué parte te dio más confianza y cuál menos?

## Matriz de captura

| Momento | Lo esperado | Lo observado | Severidad | Documento dueño | ¿Revalidar? |
| --- | --- | --- | --- | --- | --- |
| `S01` entrada | se entiende el propósito sin tecnicismos |  |  | `VOICE-VIN-002` o `UXS-VIN-002` |  |
| `S02` código | el campo principal domina sin ruido accesorio |  |  | `UXS-VIN-002` |  |
| `S02` límite de acceso | el vínculo no se confunde con acceso automático |  |  | `VOICE-VIN-002` o `UXS-VIN-002` |  |
| `S02` error | código inválido o expirado se entiende sin culpa |  |  | `VOICE-VIN-002` o `UXS-VIN-002` |  |
| `S03` confirmación | el vínculo activo se entiende con claridad y control |  |  | `VOICE-VIN-002` o `UXS-VIN-002` |  |

## Severidad y criterio de salida

### Severidad

- crítico: impide completar el vínculo o deja una lectura errónea sobre acceso a datos;
- mayor: no bloquea a todas las personas, pero agrega fricción relevante o deja ambigua la consecuencia del vínculo;
- menor: ruido corregible sin cambiar el corazón del slice.

### Cierre esperado

La ronda solo puede darse por cerrada con:

- `0` hallazgos críticos;
- como máximo `1` mayor no sistémico.

## Paquete de evidencia previsto

- notas de sesión: `artifacts/ux-validation/2026-04-08-hybrid-vin-002-vin-004/notes/`
- grabaciones o capturas, si existen: `artifacts/ux-validation/2026-04-08-hybrid-vin-002-vin-004/evidence/`
- síntesis consolidada para canon: futuro `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-002.md`

## Sync esperado después de las sesiones

- wording, framing o intensidad verbal -> `VOICE-VIN-002.md`
- jerarquía, estados, secuencia o interacción -> `UXS-VIN-002.md`
- si el hallazgo cambia un patrón reusable de vínculo o activación controlada -> revisar además `12_lineamientos_interfaz_visual.md`, `13_voz_tono.md` o `16_patrones_ui.md`

## Regla de cierre de este documento

Este documento queda completo cuando:

1. el moderador puede correr sesiones sin inventar tareas ni criterio de severidad;
2. la muestra mínima y el perfil válido están explícitos;
3. existe un camino claro desde evidencia bruta hasta `UX-VALIDATION-VIN-002.md`;
4. no se afirma que la validación ya ocurrió.

---

**Estado:** preparación operativa lista para correr sesiones reales de `VIN-002`.
**Próximo artefacto legítimo:** `.docs/wiki/23_uxui/UX-VALIDATION/UX-VALIDATION-VIN-002.md` una vez exista evidencia observada.
