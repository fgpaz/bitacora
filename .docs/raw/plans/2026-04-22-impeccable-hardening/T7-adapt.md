# Task T7: impeccable-adapt (responsive + breakpoints + typo fluida)

## Shared Context
**Goal:** Unificar breakpoints canónicos en `tokens.css`, extender `clamp()` a headings clave, subir touch targets restantes a ≥44px.
**Stack:** CSS Modules + tokens.css.
**Architecture:** Edits CSS coordinados.

## Locked Decisions
- Breakpoints canónicos: `--bp-sm: 480px`, `--bp-md: 768px`, `--bp-lg: 1024px`. Documentar en `tokens.css`.
- NO reemplazar los media queries existentes (el refactor global rompe demasiado). En su lugar: **agregar** los tokens y usarlos en CSS nuevo + documentar la convención para futuras waves.
- `clamp()` extendido sólo a headings de nivel page (h1/h2). NO tocar texto secundario ni badges.
- Touch targets ≥44px restantes que T3 no cubrió: botones `PatientList` (paginación).

## Task Metadata
```yaml
id: T7
depends_on: [T6]
agent_type: ps-next-vercel
files:
  - modify: frontend/styles/tokens.css
  - modify: frontend/components/patient/dashboard/Dashboard.module.css
  - modify: frontend/components/patient/mood/MoodEntryForm.module.css
  - modify: frontend/components/patient/checkin/DailyCheckinForm.module.css
  - modify: frontend/components/patient/consent/ConsentGatePanel.module.css
  - modify: frontend/components/professional/PatientList.module.css
  - read: .docs/raw/reports/2026-04-22-impeccable-audit-baseline.md
complexity: medium
done_when: "cd frontend && npm run typecheck && npm run lint && npm run test:e2e exit 0"
```

## Reference
Baseline §4.4 (typo fluida), §5.1 (breakpoints fragmentados), §2.5 T4 (touch targets).

## Prompt
### 7.1 — Agregar tokens de breakpoints
En `tokens.css` agregá sección nueva:
```css
/* Breakpoints (para uso en código nuevo; los módulos existentes mantienen sus media queries hasta la próxima normalización) */
--bp-sm: 480px;
--bp-md: 768px;
--bp-lg: 1024px;
```
Los custom properties no son usables directamente en media queries sin PostCSS custom-media, así que este token se agrega como **documentación viva** + referencia para futuro PostCSS plugin.

Agregá comentario al final del archivo:
```css
/*
Breakpoint canon (para waves futuras):
- ≤480px : mobile
- 481-767px : tablet small
- 768-1023px : tablet / desktop mínimo
- ≥1024px : desktop full
No unificar media queries existentes en esta wave (riesgo visual alto); aplicar la convención en código nuevo.
*/
```

### 7.2 — Tipografía fluida en headings page-level
En cada uno de estos archivos, localizá los headings `.headline` / `.pageTitle` / `.sectionTitle` que hoy tienen tamaño rígido y convertilos a `clamp()`:

- `Dashboard.module.css` — título `"Mi historial"` / `"Variabilidad diaria"` / `"Registros recientes"`. Si tienen `font-size: 1.25rem` → `font-size: clamp(1.125rem, 2.5vw, 1.5rem);`.
- `MoodEntryForm.module.css` — `.headline` actualmente ~`1.75rem`. Cambiar a `clamp(1.5rem, 4vw, 2rem);`.
- `DailyCheckinForm.module.css` — `.pageTitle` similar. Aplicar mismo patrón.
- `ConsentGatePanel.module.css` — h2 del consent.

Preservar line-height existente; sólo tocar font-size.

### 7.3 — Touch targets en PatientList paginación
En `PatientList.module.css`, los botones `Anterior` / `Siguiente` (estructura aproximada ~`.pagerButton`) deben tener `min-height: 44px; min-width: 44px;`. Si hay padding chico (ej. `padding: 6px 12px`), expandirlo proporcionalmente.

## Execution Procedure
1. Tokens breakpoints en `tokens.css` (paso 7.1).
2. `clamp()` en 4 módulos (paso 7.2). Antes de editar, abrir cada archivo y verificar el `font-size` actual.
3. Touch target en PatientList (paso 7.3).
4. Verify con `npm run typecheck && npm run lint && npm run test:e2e`.
5. Hacer screenshot manual (opcional) de Dashboard mobile ≤360px antes/después para confirmar que headings no se rompen.

## Skeleton
```css
/* tokens.css sección nueva: */
:root {
  /* ... */
  --bp-sm: 480px;
  --bp-md: 768px;
  --bp-lg: 1024px;
}
/* Heading fluido: */
.headline {
  font-size: clamp(1.5rem, 4vw, 2rem);
  line-height: 1.25;
}
```

## Verify
```bash
cd frontend && npm run typecheck && npm run lint && npm run test:e2e
```

## Commit
`style(impeccable-adapt): breakpoints canónicos documentados y tipografía fluida en headings page`
