# HANDOFF-VISUAL-QA-ONB-001 — Control visual final

## Propósito

Este documento define qué debe revisarse visualmente antes de cerrar la implementación de `ONB-001`.

## Por qué el gate está activo

El gate visual está activo porque `ONB-001` combina:

- onboarding público;
- contexto invitado sensible;
- consentimiento;
- continuidad emocional entre estados.

Una deriva visual aquí puede alterar percepción de control, claridad y privacidad.

## Checkpoints obligatorios

### 1. Portada estándar

- se lee primero como guía personal, no como home corporativa;
- `Ingresar` domina el fold (label canónico desde 2026-04-22; antes `Empezar ahora`);
- privacidad/resguardo acompaña sin robar protagonismo;
- no hay CTA secundaria compitiendo.

### 2. Portada invitada

- sigue siendo la misma familia visual que la portada estándar;
- el vínculo + propósito se entienden rápido;
- no parece otro producto ni un onboarding paralelo;
- el fallback genérico conserva la misma estructura.

### 3. Interstitial

- dura lo justo y no se siente pantalla técnica;
- el lenguaje es humano y breve;
- no aparecen spinners o skeletons agresivos.

### 4. Consentimiento

- el contenedor se percibe sensible y claro;
- headings, listas y CTA son legibles en mobile;
- el recordatorio invitado es ligero, no protagonista;
- conflictos y errores quedan localizados y comprensibles.

### 5. Bridge final

> **Deprecado 2026-04-22**: checkpoint 5 corresponde a S04-BRIDGE eliminado. El post-consent redirige directo a `/dashboard` sin pantalla intermedia. El equivalente operativo es el empty state del dashboard con CTA "Registrar humor" que abre el `MoodEntryDialog` (ver `RF-VIS-015`). Ver `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.

- la confirmación es factual;
- el CTA `Hacer mi primer registro` es inequívoco;
- el contexto invitado ya no domina la lectura.

## Drift inaceptable

- copy bilingüe o demasiado poética;
- consentimiento sobredramatizado o solemne;
- animación ornamental;
- hero invitado separado del resto de la home;
- cierre final celebratorio o ambiguo;
- señales de privacidad repetidas como panel permanente.

## Tolerancias aceptables

- pequeñas variaciones de espaciado que no cambien jerarquía;
- ajuste fino de densidad entre desktop y mobile;
- split interno de componentes sin cambiar la lectura del slice.

## Evidencia mínima de cierre

- captura desktop de `S01`, `S03` y `S05-DASHBOARD-EMPTY` (antes `S04`, deprecado 2026-04-22 — ver decision doc);
- captura mobile de `S01`, `S03` y `S05-DASHBOARD-EMPTY` (antes `S04`);
- evidencia de foco visible en CTA principal y CTA de consentimiento;
- confirmación de que la variante invitada y el fallback conservan jerarquía.

---

**Estado:** checklist visual activa para cierre de implementacion. Implementacion着陆 pero validacion UX no completa.
**Consumidor principal:** frontend + QA visual.

## Notas de implementacion

- `OnboardingFlow` component lands en `/onboarding` con estados: `auth -> consent -> bridge`.
- `AuthBootstrapInterstitial` con variants `default` e `invite_context`.
- `ConsentGatePanel` renderiza en `/consent` (route separada).
- `PatientPageShell` usado como shell unico para todos los estados.
- Validacion UX y visual QA pendientes de ejecutarse.

> **Deprecado 2026-04-22**: `NextActionBridgeCard` como puente al primer registro fue eliminado. El post-consent va directo a `/dashboard`. El checkpoint 5 "Bridge final" ya no aplica al flujo de implementacion. Ver decision doc `.docs/raw/decisiones/2026-04-22-dashboard-first-post-login.md`.
