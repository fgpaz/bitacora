# HANDOFF-SPEC-ONB-001 — Especificación operativa de handoff

## Propósito

Este documento fija el alcance implementable de `ONB-001` para frontend.

## Referencias fuente

- `../UI-RFC/UI-RFC-ONB-001.md`
- `../UXS/UXS-ONB-001.md`
- `../VOICE/VOICE-ONB-001.md`
- `../PROTOTYPE/PROTOTYPE-ONB-001.md`
- `../../07_tech/TECH-FRONTEND-SYSTEM-DESIGN.md`
- `../../09_contratos_tecnicos.md`
- `../../06_pruebas/TP-ONB.md`
- `../../06_pruebas/TP-CON.md`

## Alcance implementable

### In

- home pública `ONB-first`;
- hero estándar y hero adaptado por invitación;
- retorno auth/bootstrap;
- consentimiento con aceptación;
- confirmación + puente al primer registro.

### Out

- UI del primer `MoodEntry`;
- daily check-in;
- páginas profesionales;
- ajustes de copy legal fuera del consentimiento vigente configurado.

## Estados que deben existir

1. portada estándar;
2. portada invitada explícita;
3. portada invitada fallback;
4. aclaración de contexto;
5. interstitial auth/bootstrap;
6. consentimiento base;
7. consentimiento con recordatorio ligero;
8. conflicto de versión;
9. error recuperable de consentimiento;
10. > **Deprecado 2026-04-22**: bridge final — estado eliminado. El post-consent va directo a `/dashboard`. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

## Restricciones cerradas

- no crear una home separada para invitación;
- no usar CTA secundaria fuerte en hero;
- no prometer colaboración clínica más fuerte que `acompañamiento profesional`;
- no abrir el primer registro dentro de este mismo slice;
- no recuperar copy celebratoria ni bilingüe del material bloqueado.

## Contratos de transición

- `page.tsx` abre el slice público;
- auth válida deriva a bootstrap;
- `needsConsent=true` obliga `S03`;
- `status=consent_granted` o `active` puede saltar directo al bridge o a la siguiente ruta;
- > **Deprecado 2026-04-22**: `POST /api/v1/consent` exitoso derivaba al bridge. Ahora va directo a `/dashboard`. Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

## Blockers explícitos ya resueltos

- drift visual anterior de Stitch ya no gobierna `ONB-001`;
- el slice queda abierto por authority pack manual `2026-04-10`;
- la validación UX real sigue pendiente, pero no bloquea documentación ni implementación inicial.

## Dependencias para implementación

- `T04`: shell, sesión Zitadel, cliente API, routing base;
- `T05`: componentes del flujo paciente y navegación al primer registro.

## Done when de handoff

El handoff spec está bien consumido si frontend puede implementar el flujo sin tener que decidir:

- qué historia cuenta la portada;
- cómo se expresa la invitación;
- qué pasa entre auth y consentimiento;
- cómo se cierra el consentimiento;
- cuál es la siguiente acción del usuario.

## Cambios recientes

- 2026-04-22: estado 10 (bridge final) deprecado. El post-consent va directo a `/dashboard`. Ver decision doc `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

---

**Estado:** consumido por `T04/T05`. Bridge final deprecado 2026-04-22.
**Siguiente artefacto:** `HANDOFF-ASSETS-ONB-001.md`.
