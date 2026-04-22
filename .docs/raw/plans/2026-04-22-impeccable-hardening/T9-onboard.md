# Task T9: impeccable-onboard (empty state Dashboard refinado)

## Shared Context
**Goal:** Refinar empty state del Dashboard (canon 10 sobrio + canon 13 orientación sin dramatismo) sin tocar el copy congelado `"Empezá con tu primer registro"` + `"Registrar humor"`.
**Stack:** React + CSS Modules.
**Architecture:** Ajustes al componente Dashboard empty state + estilos.

## Locked Decisions
- Copy `"Empezá con tu primer registro"` (h2) + `"Registrar humor"` (CTA) son **congelados**.
- El sub-copy descriptivo `"Acá vas a ver tu historial cuando cargues tu primer registro."` puede ajustarse si las recomendaciones de T1 (critique) lo sugieren, pero debe mantener el tono sereno y concreto.
- El SVG decorativo del empty state mantiene `aria-hidden="true"`.
- NO agregar ilustración nueva (canon 11 §"Ilustración y recursos gráficos" restringe).

## Task Metadata
```yaml
id: T9
depends_on: [T8]
agent_type: ps-next-vercel
files:
  - modify: frontend/components/patient/dashboard/Dashboard.tsx:183-220
  - modify: frontend/components/patient/dashboard/Dashboard.module.css
  - read: .docs/raw/reports/2026-04-22-impeccable-critique.md
complexity: low
done_when: "cd frontend && npm run typecheck && npm run lint && npm run test:e2e exit 0"
```

## Reference
Baseline §7.1 (densidad Dashboard), critique report §"Dashboard densidad tipo tablero" (veredict de T1).
Canon: `.docs/wiki/13_voz_tono.md` §"Reglas para estados vacíos", `16_patrones_ui.md` §"Confirmación factual breve".

## Prompt
Leer primero `.docs/raw/reports/2026-04-22-impeccable-critique.md`. Si el veredict para Dashboard densidad es `rediseñar` o `refinar`, aplicar las recomendaciones que ahí queden documentadas **siempre que no toquen copy congelado**.

Si el critique no sugiere cambios estructurales, sólo pulir el empty state:

### 9.1 — Empty state: consistencia de ritmo y foco
Abrí `Dashboard.tsx:183-220`. El empty state tiene:
- SVG decorativo
- h2 `"Empezá con tu primer registro"` (congelado)
- p descriptivo `"Acá vas a ver tu historial cuando cargues tu primer registro."`
- CTA primaria `"Registrar humor"` (congelado)

Acciones:
1. Validar que el `<section>` o wrapper tiene `role="region"` con `aria-label="Historial vacío"` o similar, para screen readers.
2. Si el p descriptivo queda redundante tras el h2 + CTA, considerarlo **candidato a pulir**; sugerir nuevo texto `"Cuando cargues tu primer registro, lo vas a ver acá."` (menos repetición de "registro" vs "historial" vs "primer registro"). **Solo aplicar si reducís ruido; si el actual suena mejor, no tocar.**
3. En CSS del empty state asegurar: ritmo vertical generoso (≥`var(--space-xl)` entre bloques), alineación centrada, ancho máximo moderado.

### 9.2 — DashboardSummary oculto cuando no hay registros
Critique puede sugerir ocultar `DashboardSummary` en state vacío. Implementación opcional:
```tsx
{entries.length > 0 && <DashboardSummary ... />}
```
**Solo aplicar si el critique lo recomienda explícitamente.** Si se aplica, validar que los specs Playwright no asertan la presencia de DashboardSummary en empty state.

### 9.3 — Ocultar trendChart vacío
Si `entries.length === 0`, no renderizar el bloque `"Variabilidad diaria"` — ya debería estar implícito en el flujo actual, verificar.

## Execution Procedure
1. Leer critique report.
2. Aplicar 9.1, 9.2, 9.3 **condicionalmente** según recomendaciones.
3. Si se cambió sub-copy, verificar que ningún spec Playwright lo assertea directamente. Si lo hace, alertar al humano antes de modificar el spec.
4. Verify final.

## Skeleton
```tsx
// Empty state wrapper:
<section role="region" aria-label="Historial vacío" className={styles.emptyState}>
  {/* SVG decorativo aria-hidden="true" */}
  <h2>Empezá con tu primer registro</h2>
  <p>Cuando cargues tu primer registro, lo vas a ver acá.</p>
  <Link href="/registro/mood-entry">Registrar humor</Link>
</section>
```

## Verify
```bash
cd frontend && npm run typecheck && npm run lint && npm run test:e2e
```

## Commit
`style(impeccable-onboard): empty state Dashboard con ritmo y foco refinado`
