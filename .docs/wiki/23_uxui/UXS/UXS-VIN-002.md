# UXS-VIN-002 — Ingreso de código de vinculación

## Propósito

Este documento fija el contrato UX del paso crítico del slice `VIN-002`.

No vuelve a contar el journey completo ni reemplaza la voz. Su función es volver operable el momento más sensible o decisivo del caso para prototipo, validación y contrato técnico.

## Relación con el canon

Este documento depende de:

- `../UXR/UXR-VIN-002.md`
- `../UXI/UXI-VIN-002.md`
- `../UJ/UJ-VIN-002.md`
- `../VOICE/VOICE-VIN-002.md`
- `../../03_FL/FL-VIN-02.md`
- `../../06_pruebas/TP-VIN.md`

Y prepara directamente:

- futuro `../PROTOTYPE/PROTOTYPE-VIN-002.md`
- futuro `../UX-VALIDATION/UX-VALIDATION-VIN-002.md`

## Slice y paso dueño

- slice: `VIN-002`
- paso crítico: `S02` — Ingreso y validación del código
- entrada: la persona ya tiene un código válido y necesita resolver el vínculo sin pasos extra
- salida correcta: el vínculo queda activo y la siguiente decisión de acceso queda clara

## Sensación del paso

- sensación objetivo: un paso corto y preciso
- anti-sensación: resolver un código sin entender consecuencias

## Tarea del usuario

1. ingresar el código
2. confirmar que es válido
3. entender el estado del vínculo resultante

## Contrato de interacción

### Estructura mínima

- encabezado breve
- campo principal para código
- resumen corto del efecto del vínculo
- CTA única

### Acción primaria

- `Vincular`

### Acción secundaria

- `Salir por ahora`

## Modelo de estados

| Estado | Qué ve la persona | Comportamiento esperado |
| --- | --- | --- |
| default | campo vacío y resumen visible | permite ingresar código |
| ready_to_submit | CTA habilitada | permite validar y crear vínculo |
| submitting | feedback breve | evita doble submit |
| success | vínculo activo con acceso aún bajo control del paciente | cierra con claridad |
| invalid_or_expired | mensaje corto de error | permite corregir o abandonar |

## Contrato de copy

- titular aprobado: `Ingresá el código`
- texto de apoyo aprobado: `El vínculo no activa acceso automático a tus datos.`
- acción primaria aprobada: `Vincular`
- error recuperable aprobado: `Ese código no está disponible. Pedí uno nuevo a tu profesional.`

## Aceptación

1. el campo principal domina sin ruido accesorio
2. la confirmación explica vínculo activo y acceso aún controlado
3. los errores distinguen código inválido de expirado cuando haga falta

## Defaults transferibles

- una sola acción principal por paso;
- helpers solo cuando destraban una duda real;
- explicitud alta solo cuando el caso cambia acceso, datos o consentimiento;
- después del momento sensible, volver a compresión.

## Criterio de validación rápida

Este `UXS` está bien calibrado si el paso se entiende rápido, sostiene la sensación deseada y no agrega fricción gratuita. Está mal calibrado si el paso pide más lectura o interpretación que el valor que devuelve.

---

**Estado:** `UXS` activo para `VIN-002`.
**Siguiente capa gobernada:** futuro `../PROTOTYPE/PROTOTYPE-VIN-002.md` y `../UX-VALIDATION/UX-VALIDATION-VIN-002.md`.
