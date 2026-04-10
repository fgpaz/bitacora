# Decisión de Gobernanza — Authority pack manual para `ONB-001`

**Fecha:** 2026-04-10

## Contexto

`ONB-001` quedó redefinido como slice `ONB-first`:

- portada pública centrada en guía personal del paciente;
- variante invitada como `hero adaptado`;
- auth/bootstrap return breve;
- consentimiento con `resguardo claro`;
- confirmación con puente al primer registro.

La ola anterior de Stitch dejó hallazgos reales, pero el proyecto necesita destrabar `T04/T05` sin esperar una nueva corrida para este slice puntual.

## Decisión

Se aprueba un authority pack manual para `ONB-001`.

Ese pack queda compuesto por:

- `UXR-ONB-001.md`
- `UXI-ONB-001.md`
- `UJ-ONB-001.md`
- `VOICE-ONB-001.md`
- `UXS-ONB-001.md`
- `PROTOTYPE-ONB-001.md`
- `UI-RFC-ONB-001.md`
- `HANDOFF-*` del slice

## Alcance

Esta aprobación habilita:

- abrir `UI-RFC-ONB-001.md`;
- abrir la cadena `HANDOFF-*` de `ONB-001`;
- permitir que `T04/T05` implementen la home `ONB-first`, el retorno auth/bootstrap, el consentimiento y el bridge.

Esta aprobación no habilita:

- marcar `ONB-001` como validado;
- reabrir o cerrar `UX-VALIDATION`;
- levantar el bloqueo de `REG-001` o `REG-002`;
- tomar el HTML heredado como autoridad visual.

## Riesgo aceptado

Se acepta explícitamente que la validación sobre código funcional podrá devolver cambios a:

- copy;
- jerarquía;
- densidad visual;
- comportamiento del consentimiento.

Si eso ocurre, deberá reabrirse el pack del slice.

## Regla de precedencia

Para `ONB-001`, la autoridad documental operativa queda así:

1. `UXS-ONB-001.md`
2. `UI-RFC-ONB-001.md`
3. `HANDOFF-*`
4. HTML histórico solo como referencia archivada

## Estado

`Aprobado`

## Siguiente uso esperado

- `T04`: bootstrap del runtime web y shell pública;
- `T05`: implementación del flujo paciente hasta el primer puente de registro.
