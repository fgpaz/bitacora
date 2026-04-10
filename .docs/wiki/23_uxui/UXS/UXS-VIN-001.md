# UXS-VIN-001 — Emisión de invitación

## Propósito

Este documento fija el contrato UX del paso crítico del slice `VIN-001`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIN-001.md`
- `../UXI/UXI-VIN-001.md`
- `../UJ/UJ-VIN-001.md`
- `../VOICE/VOICE-VIN-001.md`
- `../../03_FL/FL-VIN-01.md`
- `../../06_pruebas/TP-VIN.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-VIN-001.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-VIN-001.md`

## Slice y paso dueño

- slice: `VIN-001`
- paso crítico: `S02` — Revisión y emisión de invitación
- entrada: el profesional llega con intención de invitar a una persona por email
- salida correcta: la invitación queda emitida o el estado existente queda claro

## Sensación del paso

- sensación objetivo: una acción breve y responsable
- anti-sensación: enviar algo sensible sin entender su alcance

## Tarea del usuario

1. cargar el email
2. entender qué crea esta acción
3. emitir la invitación

## Contrato de interacción

### Estructura mínima

- encabezado breve
- campo principal de email
- resumen corto del alcance y estado resultante
- CTA única

### Acción primaria

- `Enviar invitación`

### Acción secundaria

- `Cancelar`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| default | campo y resumen visibles | permite preparar el envío |
| ready_to_submit | CTA habilitada | permite enviar |
| submitting | feedback corto | evita doble envío |
| success | estado pending/invited visible | deja claro qué sigue |
| error_or_conflict | error o vínculo existente explicados en contexto | evita duplicar acciones innecesarias |

## Contrato de copy

- titular aprobado: `Invitar a una persona`
- texto de apoyo aprobado: `La invitación crea un vínculo pendiente. El acceso a datos sigue bajo control del paciente.`
- acción primaria aprobada: `Enviar invitación`
- error recuperable aprobado: `No pudimos emitir esta invitación. Revisá el email o probá de nuevo.`

## Aceptación

1. queda explícito que la invitación no habilita acceso automático
2. el estado resultante se entiende sin explicación oral
3. la pantalla no suena a alta administrativa de paciente

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

---

**Estado:** `UXS` activo para `VIN-001`.
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-VIN-001.md` y `../UX-VALIDATION/UX-VALIDATION-VIN-001.md`.
