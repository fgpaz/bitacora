# Task T4: impeccable-normalize (tokens y DS consistency)

## Shared Context
**Goal:** Eliminar tokens fantasma (`--border`, `--semantic-warning`, `--shadow-strong`), hex hardcodeados y fallbacks crudos en CSS Modules.
**Stack:** CSS Modules + `frontend/styles/tokens.css`.
**Architecture:** Edit coordinado entre tokens.css (definiciones) + módulos CSS (consumo).

## Locked Decisions
- **Radios DS:** conservar los valores actuales del `tokens.css` (4/8/12) y sincronizar el canon `11_identidad_visual.md` para que refleje esto. NO refactorizar a 8/14/20 — es visualmente invasivo.
- Nuevos tokens a agregar en `tokens.css`:
  - `--border: var(--border-subtle);` (alias para compatibilidad + limpieza progresiva).
  - `--semantic-warning: var(--status-warning);` y variantes `-bg`, `-border` ya existen.
  - `--shadow-strong: 0 8px 24px rgba(46, 42, 40, 0.14);` (sobrio editorial, no SaaS).
  - `--foreground-on-brand: #FFFFFF;` (para texto sobre `--brand-primary`).
  - `--overlay-backdrop: rgba(46, 42, 40, 0.55);` (coherente con `--foreground`).
- Magic numbers de spacing en `vinculos/*` → mapear a `--space-*`.
- Paddings en componentes profesionales (chips/badges de 1-4px) → aceptable como exception tipográfica documentada; no tocar.
- Fallback `var(--radius-lg, 16px)` en MoodEntryDialog.module.css:17 → eliminar fallback (deja solo `var(--radius-lg)`).

## Task Metadata
```yaml
id: T4
depends_on: [T3]
agent_type: ps-next-vercel
files:
  - modify: frontend/styles/tokens.css
  - modify: frontend/components/professional/ExportGate.module.css:125,163
  - modify: frontend/components/professional/Timeline.module.css:177,191,198,217,232
  - modify: frontend/components/professional/InviteForm.module.css:84
  - modify: frontend/components/patient/dashboard/MoodEntryDialog.module.css:11,17,18
  - modify: frontend/components/patient/vinculos/VinculosList.module.css:13,27,154
  - modify: frontend/components/patient/vinculos/VinculosManager.module.css:52
  - modify: .docs/wiki/11_identidad_visual.md
  - read: .docs/raw/reports/2026-04-22-impeccable-audit-baseline.md
complexity: medium
done_when: "cd frontend && npm run typecheck && npm run lint && npm run test:e2e exit 0 AND grep -rn 'var(--border[^-])' frontend/components --include='*.module.css' | wc -l == 0 AND grep -rn '#d97706\\|#fff' frontend/components --include='*.module.css' | wc -l == 0"
```

## Reference
- Baseline §4 completo.
- Canon: `.docs/wiki/11_identidad_visual.md` §"Paleta base" y §"Tokens base".

## Prompt
Ejecutás edit coordinado: primero expandís `tokens.css`, luego reemplazás referencias rotas en los módulos.

### 4.1 — Expandir `tokens.css` con nuevos tokens
Abrí `frontend/styles/tokens.css` y agregá estos tokens en sus secciones correspondientes:

Dentro de `:root { ... }`:
- En la sección Borders, sumá:
  ```css
  --border: var(--border-subtle);  /* alias legacy — usar --border-subtle en código nuevo */
  ```
- En la sección Semantic state, sumá:
  ```css
  --semantic-warning: var(--status-warning);
  ```
- En la sección Shadows, sumá:
  ```css
  --shadow-strong: 0 8px 24px rgba(46, 42, 40, 0.14);
  ```
- En la sección Foreground, sumá:
  ```css
  --foreground-on-brand: #FFFFFF;
  ```
- Crear nueva sección `/* Overlays */` con:
  ```css
  --overlay-backdrop: rgba(46, 42, 40, 0.55);
  ```

### 4.2 — `ExportGate.module.css` y `Timeline.module.css`: reemplazar `var(--border)` → `var(--border-subtle)`
Abrí los dos archivos. Reemplazá **todas** las ocurrencias de `var(--border)` por `var(--border-subtle)` (son 5 en total). El alias `--border` en tokens.css queda por compatibilidad pero el código nuevo usa el nombre semántico correcto.

### 4.3 — `Timeline.module.css` líneas 191 y 232: `color: #fff` → `color: var(--foreground-on-brand)`
```css
/* antes: */
.presetBtnActive { color: #fff; }
.applyBtn { color: #fff; }
/* después: */
.presetBtnActive { color: var(--foreground-on-brand); }
.applyBtn { color: var(--foreground-on-brand); }
```

### 4.4 — `InviteForm.module.css:84`: fallback hex → token limpio
```css
/* antes: */
border-left-color: var(--semantic-warning, #d97706);
/* después: */
border-left-color: var(--status-warning-border);
```
(El token `--status-warning-border` ya existe en tokens.css.)

### 4.5 — `MoodEntryDialog.module.css:11,17,18`: eliminar fallbacks crudos
```css
/* Línea 11 antes: */
.dialog::backdrop { background-color: rgba(16, 24, 40, 0.55); }
/* Línea 11 después: */
.dialog::backdrop { background-color: var(--overlay-backdrop); }

/* Línea 17 antes: */
border-radius: var(--radius-lg, 16px);
/* Línea 17 después: */
border-radius: var(--radius-lg);

/* Línea 18 antes: */
box-shadow: var(--shadow-strong, 0 24px 48px rgba(16, 24, 40, 0.18));
/* Línea 18 después: */
box-shadow: var(--shadow-strong);
```

### 4.6 — `VinculosList.module.css:13,27,154` — magic numbers rem → `--space-*`
```css
/* Mapeo: 2rem → --space-2xl (64px es un poco más); 1.25rem → entre --space-md (16px) y --space-lg (24px);
   1rem → --space-md. Preferir el más cercano SIN cambiar layout visualmente. */

/* Línea 13: padding: 2rem; → padding: var(--space-xl);  (40px; el 2rem=32px estaba entre --space-lg 24 y --space-xl 40 — preferir el mayor por respiración) */
/* Línea 27: padding: 1.25rem;  → padding: var(--space-md);  (16px=1rem — 1.25rem=20px sin match exacto; elegir --space-md es la caída más cercana aceptable) */
/* Línea 154: padding: 1rem; → padding: var(--space-md); (match 16px) */
```
**Importante:** el cambio 2rem→40px y 1.25rem→16px puede alterar visualmente. Hacer screenshot local antes/después; si rompe, revertir ese caso específico y documentar como exception en el commit.

### 4.7 — `VinculosManager.module.css:52` — `padding: 1rem` → `padding: var(--space-md)`
Edit directo.

### 4.8 — Sincronizar canon `11_identidad_visual.md` con radios implementados
Abrí `.docs/wiki/11_identidad_visual.md`. Buscá la sección `## Tokens base` (alrededor de línea 212). La tabla dice:
```
| `radius-sm` | 8px, suavidad contenida |
| `radius-md` | 14px, contenedores principales |
| `radius-lg` | 20px, superficies especiales de refugio |
```
Reemplazá por:
```
| `radius-sm` | 4px, suavidad contenida mínima |
| `radius-md` | 8px, contenedores principales |
| `radius-lg` | 12px, superficies especiales de refugio |
```
Y agregá un bloque `> Actualizado 2026-04-22 — radios sincronizados con tokens.css implementado (ver .docs/raw/plans/2026-04-22-impeccable-hardening/T4-normalize.md). La identidad editorial sobria se preserva; la diferencia de 4-8px no altera la percepción cálida.`

Esta edición del canon debe invocar antes `Skill("ps-asistente-wiki")` para validar paso y dependencias. Si la skill indica que corresponde `crear-identidad-visual`, usarla.

## Execution Procedure
1. Expandí `tokens.css` con los 5 tokens nuevos (paso 4.1).
2. `cd frontend && npm run typecheck` — verificar que la sintaxis CSS no rompe el build.
3. Aplicá 4.2 - 4.7 en los módulos CSS.
4. Aplicá 4.8 en el canon wiki (pasando por `ps-asistente-wiki` si aplica).
5. Final check:
   ```bash
   cd frontend && npm run typecheck && npm run lint && npm run test:e2e
   grep -rn 'var(--border)[^-]' frontend/components --include='*.module.css'  # esperar 0
   grep -rn '#d97706\|color: #fff' frontend/components --include='*.module.css'  # esperar 0
   ```
6. Si algún componente se ve roto (backdrop ausente, sombra del modal desaparecida, bordes invisibles), revisar que los tokens nuevos están bien parseados.

## Skeleton
```css
/* tokens.css — secciones nuevas: */
:root {
  /* ... */
  --border: var(--border-subtle);
  --semantic-warning: var(--status-warning);
  --shadow-strong: 0 8px 24px rgba(46, 42, 40, 0.14);
  --foreground-on-brand: #FFFFFF;

  /* Overlays */
  --overlay-backdrop: rgba(46, 42, 40, 0.55);
}
```

## Verify
```bash
cd frontend && npm run typecheck && npm run lint && npm run test:e2e
grep -rn 'rgba(16, 24, 40' frontend/components --include='*.module.css'   # esperar 0
```

## Commit
`style(impeccable-normalize): consolidar tokens del DS y sincronizar canon 11 con radios reales`
