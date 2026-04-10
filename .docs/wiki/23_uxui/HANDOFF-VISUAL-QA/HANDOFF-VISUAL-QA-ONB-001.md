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
- `Empezar ahora` domina el fold;
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

- captura desktop de `S01`, `S03` y `S04`;
- captura mobile de `S01`, `S03` y `S04`;
- evidencia de foco visible en CTA principal y CTA de consentimiento;
- confirmación de que la variante invitada y el fallback conservan jerarquía.

---

**Estado:** checklist visual activa para cierre de implementación.
**Consumidor principal:** frontend + QA visual.
