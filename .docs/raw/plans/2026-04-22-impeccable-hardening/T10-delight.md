# Task T10: impeccable-delight (micro-interacciones acotadas)

## Shared Context
**Goal:** Sumar 1-2 micro-interacciones sutiles que refuercen la sensación de seguridad sin juicio, respetando canon 10 "postura sobria" y `prefers-reduced-motion`.
**Stack:** CSS Modules.
**Architecture:** Edits CSS puntuales.

## Locked Decisions
- Canon 10 §Anti-señales y canon 11 §"Clima de movimiento" **RESTRINGEN fuerte** este skill: "transiciones suaves", "sin rebotes", "sin celebraciones", "sin pulsos constantes". Cualquier delight que cruce estas líneas es inválido.
- Sólo 2 delight permitidos en esta wave:
  1. Fade-in del `MoodEntryDialog` al abrir (≤200ms, ya existe probablemente; validar y si no, agregar).
  2. Transición suave de color en `:hover` sobre CTAs primarios (≤150ms de `background-color` y `border-color`; sin transform scale).
- TODO cambio debe envolverse en `@media (prefers-reduced-motion: reduce) { transition: none; }`.

## Task Metadata
```yaml
id: T10
depends_on: [T9]
agent_type: ps-next-vercel
files:
  - modify: frontend/components/patient/dashboard/MoodEntryDialog.module.css
  - modify: frontend/components/patient/onboarding/OnboardingEntryHero.module.css
  - modify: frontend/components/patient/dashboard/Dashboard.module.css
complexity: low
done_when: "cd frontend && npm run typecheck && npm run lint && npm run test:e2e exit 0"
```

## Reference
Canon 10 §Anti-señales y canon 11 §"Clima de movimiento".

## Prompt
### 10.1 — Fade in en MoodEntryDialog
Si `MoodEntryDialog.module.css` no tiene fade-in al abrir, agregar:
```css
.dialog[open] {
  animation: fadeIn 200ms ease-out;
}
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}
@media (prefers-reduced-motion: reduce) {
  .dialog[open] { animation: none; }
}
```

### 10.2 — Hover suave en CTA primaria del hero
En `OnboardingEntryHero.module.css` para el link CTA `"Ingresar"`:
```css
.primaryCta {
  transition: background-color 150ms ease-out, border-color 150ms ease-out;
}
.primaryCta:hover {
  background-color: color-mix(in srgb, var(--brand-primary) 92%, black 8%);
}
@media (prefers-reduced-motion: reduce) {
  .primaryCta { transition: none; }
}
```
Usar `color-mix` solo si el browser target lo soporta (Next.js 16 + React 19 típicamente sí). Si no, caer a un token existente.

### 10.3 — Hover suave en CTA `+ Nuevo registro` del Dashboard
Mismo patrón en `Dashboard.module.css`.

## Execution Procedure
1. Revisar si los archivos ya tienen fade/hover; si ya existen, NO duplicar.
2. Agregar sólo si falta.
3. Verify.

## Skeleton
Ya provisto arriba.

## Verify
```bash
cd frontend && npm run typecheck && npm run lint && npm run test:e2e
```

## Commit
`style(impeccable-delight): fade-in dialog y transición CTA sobria con reduced-motion`
