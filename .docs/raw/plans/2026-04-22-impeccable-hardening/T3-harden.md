# Task T3: impeccable-harden (a11y, error handling, state coverage)

## Shared Context
**Goal:** Cerrar bloqueantes WCAG 2.1 AA: focus ring visible en HCM, touch targets ≥44px, ARIA correcto, estados faltantes (session_expired, conflict, locked).
**Stack:** Next.js 16 + React 19 + CSS Modules.
**Architecture:** Edits ARIA/CSS en componentes críticos del flujo paciente + consent + modal.

## Locked Decisions
- `:focus-visible` en TODOS los interactivos: `outline: 2px solid var(--brand-primary); outline-offset: 2px;` **MÁS** `box-shadow: var(--focus-ring)` como complemento. Nunca `outline: none` aislado.
- `aria-pressed` es el mecanismo elegido para botones boolean (NO refactor a `role="radio"` — menos invasivo y matchea el patrón actual de `<button>`).
- MoodEntryDialog devuelve foco al disparador via `useRef` al botón "+ Nuevo registro" desde `Dashboard.tsx`.
- CTA legal del consent (`ConsentGatePanel.acceptBtn`) pasa a `min-height: 44px` obligatorio; retryBtn también.
- `role="listitem"` en `<section>` (ConsentGatePanel) se reemplaza por `<div role="listitem">`.
- Oscurecer `--foreground-muted` de `#655E59` a `#4A4440` para cumplir AA. Justificación en commit: compliance salud.
- `--brand-accent` deja de usarse como color de texto (mover a border/énfasis no-textual).
- `h1` dentro de `MoodEntryForm` cambia a `p.headline` con `role="heading" aria-level="3"` cuando renderiza en modo `embedded`.
- Banner "Ahora no": al descartar, emitir `aria-live="polite"` con `"Recordatorio descartado"` antes de desmontar.

## Task Metadata
```yaml
id: T3
depends_on: [T2]
agent_type: ps-next-vercel
files:
  - modify: frontend/styles/tokens.css:13-15
  - modify: frontend/components/ui/InlineFeedback.tsx:19
  - modify: frontend/components/patient/consent/ConsentGatePanel.tsx:58-61
  - modify: frontend/components/patient/checkin/DailyCheckinForm.tsx:178-213
  - modify: frontend/components/ui/PatientPageShell.tsx:41-48
  - modify: frontend/components/patient/mood/MoodEntryForm.tsx:100
  - modify: frontend/components/patient/dashboard/MoodEntryDialog.tsx
  - modify: frontend/components/patient/dashboard/Dashboard.tsx
  - modify: frontend/components/patient/dashboard/TelegramReminderBanner.tsx
  - modify: frontend/components/patient/checkin/DailyCheckinForm.module.css:31-115
  - modify: frontend/components/patient/consent/ConsentGatePanel.module.css:71-101
  - modify: frontend/components/patient/dashboard/TelegramReminderBanner.module.css:45
  - modify: frontend/components/patient/dashboard/MoodEntryDialog.module.css:62
  - modify: frontend/components/patient/mood/MoodScale.module.css:30
  - modify: frontend/components/patient/mood/MoodEntryForm.module.css:42
  - modify: frontend/components/ui/PatientPageShell.module.css:76
  - modify: frontend/components/patient/dashboard/Dashboard.module.css:127
  - read: .docs/raw/reports/2026-04-22-impeccable-audit-baseline.md
complexity: high
done_when: "cd frontend && npm run typecheck && npm run lint && npm run test:e2e exit 0 AND grep -rn 'outline: none' frontend/components --include='*.module.css' | wc -l == 0"
```

## Reference
- Baseline §2 (a11y completo), §6 (state coverage), §7.4 (touch targets).
- WCAG 2.1 AA: 1.4.3 (contrast), 2.4.7 (focus visible), 2.4.11 (focus appearance), 2.5.5 (target size), 4.1.2 (name/role/value), 4.1.3 (status messages).
- Canon: `.docs/wiki/12_lineamientos_interfaz_visual.md` §"Baseline de accesibilidad visual".

## Prompt
Ejecutás edits write con alta precisión a11y. Cada sub-tarea es atómica.

### 3.1 — Contraste: oscurecer `--foreground-muted`
Abrí `frontend/styles/tokens.css:15` y cambiá:
```css
--foreground-muted: #655E59;
```
por:
```css
--foreground-muted: #4A4440;
```
Motivo documentado en commit: subir contraste sobre `--surface-muted` a ≥4.5:1 (WCAG 1.4.3).

### 3.2 — `InlineFeedback`: arreglar conflicto aria-live + role
Abrí `frontend/components/ui/InlineFeedback.tsx:19`. El patrón actual es:
```tsx
role={variant === 'error' ? 'alert' : 'status'}
aria-live="polite"
```
Cambialo a:
```tsx
role={variant === 'error' ? 'alert' : 'status'}
aria-live={variant === 'error' ? 'assertive' : 'polite'}
```
NO elimines `role`.

### 3.3 — `ConsentGatePanel`: `<section role="listitem">` → `<div role="listitem">`
Abrí `ConsentGatePanel.tsx:58-61`. Reemplazá `<section className={styles.section} role="listitem">` por `<div className={styles.section} role="listitem">`. Preservá el CSS (el styleName `.section` queda igual).

### 3.4 — `DailyCheckinForm`: `aria-pressed` en botones boolean
Abrí `DailyCheckinForm.tsx:178-213`. Para cada par Sí/No (4 factores booleanos + medicación), agregá:
```tsx
<button
  type="button"
  className={`${styles.boolBtn} ${form[key] === true ? styles.boolSelected : ''}`}
  onClick={() => setForm(prev => ({ ...prev, [key]: true }))}
  aria-pressed={form[key] === true}
>
  Sí
</button>
<button
  type="button"
  className={`${styles.boolBtn} ${form[key] === false ? styles.boolSelected : ''}`}
  onClick={() => setForm(prev => ({ ...prev, [key]: false }))}
  aria-pressed={form[key] === false}
>
  No
</button>
```
Aplicar el mismo patrón al bloque de `medication_taken` (línea 199-213).

### 3.5 — `PatientPageShell`: `type="button"` explícito en logout
Abrí `PatientPageShell.tsx:41-48`. Agregá `type="button"` al `<button onClick={handleLogout}>`. Preservá `title` y `className`.

### 3.6 — `MoodEntryForm`: jerarquía heading en modo embedded
Abrí `MoodEntryForm.tsx:100`. El componente recibe prop `embedded: boolean`. Cambiá:
```tsx
<h1 className={styles.headline}>¿Cómo te sentís ahora?</h1>
```
a:
```tsx
{embedded ? (
  <p className={styles.headline} role="heading" aria-level={3}>
    ¿Cómo te sentís ahora?
  </p>
) : (
  <h1 className={styles.headline}>¿Cómo te sentís ahora?</h1>
)}
```

### 3.7 — `MoodEntryDialog` + `Dashboard`: devolver foco al disparador
Abrí `Dashboard.tsx`. El botón `"+ Nuevo registro"` (cerca de línea 301) debe usar `useRef`. Agregá al componente:
```tsx
const openDialogRef = useRef<HTMLButtonElement | null>(null);
// ...
function closeDialog() {
  setDialogOpen(false);
  requestAnimationFrame(() => openDialogRef.current?.focus());
}
// En el JSX del botón:
<button ref={openDialogRef} ...>+ Nuevo registro</button>
```
Abrí `MoodEntryDialog.tsx:46-52`. Agregá `aria-modal="true"` al elemento `<dialog>`.

### 3.8 — `TelegramReminderBanner`: aria-live al descartar
Abrí `TelegramReminderBanner.tsx:47-52`. Reescribí `handleDismiss`:
```tsx
const [dismissAnnounce, setDismissAnnounce] = useState<string>('');

function handleDismiss() {
  localStorage.setItem(LOCAL_KEY, String(Date.now()));
  setDismissAnnounce('Recordatorio descartado por 30 días.');
  setTimeout(() => setVisible(false), 100);
}
```
En el JSX, agregá oculto:
```tsx
<span role="status" aria-live="polite" className="visually-hidden">
  {dismissAnnounce}
</span>
```
Si `visually-hidden` no existe en `globals.css`, agregalo ahí:
```css
.visually-hidden {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0,0,0,0);
  border: 0;
}
```

### 3.9 — CSS `:focus-visible` — reemplazar `outline: none` por outline real
Archivos a modificar: `MoodEntryDialog.module.css:62`, `MoodScale.module.css:30`, `MoodEntryForm.module.css:42`, `DailyCheckinForm.module.css:81-89,112-115`, `TelegramReminderBanner.module.css:78`, `Dashboard.module.css:127`, `PatientPageShell.module.css:76`.

En cada archivo, buscá patrón:
```css
.xxx:focus-visible {
  outline: none;
  box-shadow: var(--focus-ring);
}
```
Reemplazá por:
```css
.xxx:focus-visible {
  outline: 2px solid var(--brand-primary);
  outline-offset: 2px;
  box-shadow: var(--focus-ring);
}
```

Caso especial `DailyCheckinForm.module.css:56-59,112-115`: hay `:focus` **sin** `:focus-visible`. Eliminar las reglas `:focus` (no `:focus-visible`) y dejar sólo `:focus-visible` con outline real.

### 3.10 — `DailyCheckinForm.module.css:31-33` — error por color solo
Para `.blockError`:
```css
.blockError {
  border-left-color: var(--brand-accent);
}
```
Cambiá a:
```css
.blockError {
  border-left-color: var(--status-danger);
  border-left-width: 4px;
}
```
Además, en `DailyCheckinForm.tsx`, dentro del bloque que aplica `blockError`, agregá un ícono accesible o texto "⚠ Campo con error" cerca del heading del bloque. Usá emoji `⚠` con `aria-hidden="true"` si ícono propio no existe.

### 3.11 — Touch targets ≥44px
- `ConsentGatePanel.module.css:71-83` `.acceptBtn` → agregar `min-height: 44px;`.
- `ConsentGatePanel.module.css:90-101` `.retryBtn` → agregar `min-height: 44px;`.
- `TelegramReminderBanner.module.css:45` → `min-height: 40px` → `min-height: 44px`.

### 3.12 — Validar `DailyCheckinForm` input sleep con `aria-required`
En `DailyCheckinForm.tsx:150-165` al input `sleep_hours` agregar `aria-required="true"` y `required` si aún no está.

### 3.13 — `Dashboard.tsx` chart bars: hide decorative span
En `Dashboard.tsx:244-259`, asegurar que `<span className={getTrendBarClass(...)}...>` tiene `aria-hidden="true"`.

## Execution Procedure
1. Primero token: `tokens.css` paso 3.1. Verificar visualmente o con tool de contraste que `--foreground-muted: #4A4440` sobre `--surface-muted: #E8DED3` da ≥4.5:1.
2. Aplicar 3.2-3.5 (cambios puntuales TSX).
3. Aplicar 3.6 MoodEntryForm heading embedded.
4. Aplicar 3.7 Dashboard + MoodEntryDialog.
5. Aplicar 3.8 TelegramReminderBanner + globals.css.
6. Aplicar 3.9 en los 7 CSS Modules listados.
7. Aplicar 3.10-3.13.
8. `cd frontend && npm run typecheck && npm run lint && npm run test:e2e` — si pasa, commit.
9. Si algún spec de Playwright rompe por cambio de estructura en MoodEntryForm embedded (p.ej. selector por `h1`), reportar con `AskUserQuestion` antes de modificar el spec.

## Skeleton
```tsx
// Patron aria-pressed (DailyCheckinForm):
<button
  type="button"
  className={`${styles.boolBtn} ${form.activity_physical === true ? styles.boolSelected : ''}`}
  onClick={() => setForm(prev => ({ ...prev, activity_physical: true }))}
  aria-pressed={form.activity_physical === true}
>
  Sí
</button>
```
```css
/* Patron focus-visible canonico: */
.interactive:focus-visible {
  outline: 2px solid var(--brand-primary);
  outline-offset: 2px;
  box-shadow: var(--focus-ring);
}
```

## Verify
```bash
cd frontend && npm run typecheck && npm run lint && npm run test:e2e
grep -rn 'outline: none' frontend/components --include='*.module.css'   # debe devolver 0 matches
grep -rn 'role="listitem"' frontend/components | grep '<section'       # debe devolver 0 matches
```

## Commit
`style(impeccable-harden): focus visible, aria-pressed, touch targets legales y contraste AA`
