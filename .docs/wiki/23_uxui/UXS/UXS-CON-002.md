# UXS-CON-002 — Revocación del consentimiento

## Propósito

Este documento fija el contrato UX del paso crítico del slice `CON-002`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-CON-002.md`
- `../UXI/UXI-CON-002.md`
- `../UJ/UJ-CON-002.md`
- `../VOICE/VOICE-CON-002.md`
- `../../03_FL/FL-CON-02.md`
- `../../06_pruebas/TP-CON.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-CON-002.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-CON-002.md`

## Slice y paso dueño

- slice: `CON-002`
- paso crítico: `S02` — Revisión del impacto antes de revocar
- entrada: la persona llega con intención de cortar el consentimiento vigente
- salida correcta: el consentimiento queda revocado y la suspensión de registro/accesos se entiende

## Sensación del paso

- sensación objetivo: una pausa seria pero serena
- anti-sensación: una pared de advertencias

## Tarea del usuario

1. entender el impacto
2. confirmar la decisión
3. salir con claridad sobre el nuevo estado

## Contrato de interacción

### Estructura mínima

- encabezado breve
- resumen de impacto en dos o tres ideas
- acción principal de revocación y secundaria de conservación

### Acción primaria

- `Revocar consentimiento`

### Acción secundaria

- `Conservar consentimiento`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| default | impacto visible con CTA disponible | permite revisar y decidir |
| submitting | feedback breve de proceso | evita doble submit |
| success | estado revocado visible | confirma suspensión de registro y accesos |
| error_retryable | error claro de revocación | permite reintentar sin ambigüedad |

## Contrato de copy

- titular aprobado: `Revocar consentimiento`
- texto de apoyo aprobado: `Si seguís, se suspende el registro y los profesionales pierden acceso a tus datos.`
- acción primaria aprobada: `Revocar consentimiento`
- error recuperable aprobado: `No pudimos revocar este consentimiento. Probá de nuevo.`

## Aceptación

1. la cascada a vínculos queda clara antes de confirmar
2. la pantalla no suena punitiva ni legalista
3. el estado final se entiende sin explicación adicional

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

---

**Estado:** `UXS` activo para `CON-002`.
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-CON-002.md` y `../UX-VALIDATION/UX-VALIDATION-CON-002.md`.
