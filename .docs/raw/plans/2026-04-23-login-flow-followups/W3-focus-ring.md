# W3 — Focus ring normalize

**Tipo:** code · **Subagente:** `ps-next-vercel` · **Duración estimada:** 20 min
**Salida:** múltiples archivos CSS modificados con `:focus-visible` canónico.

---

## Contexto

El closure 2026-04-23 §6.5 flagged 2-4 módulos CSS con `outline` sin `var(--focus-ring)` o con `outline: none` suelto. Grep actual (2026-04-23) confirma los siguientes gaps:

**`outline: none` sueltos (sin `:focus-visible` reemplazo canónico):**
- `frontend/app/AppState.module.css:78`
- `frontend/components/patient/vinculos/VinculosForm.module.css:34`
- `frontend/components/professional/ExportGate.module.css:174`
- `frontend/components/professional/InviteForm.module.css:29`
- `frontend/components/patient/telegram/TelegramPairingCard.module.css:301`
- `frontend/components/professional/Timeline.module.css:223`

**Focus rings sin `box-shadow: var(--focus-ring)` complementario:**
- `frontend/components/ui/ProfessionalShell.module.css:51` (`.navLink:focus-visible` — `outline: 2px solid var(--surface)`).
- `frontend/components/ui/ProfessionalShell.module.css:76` (`.logoutButton:focus-visible` — idem).
- `frontend/components/ui/SaveRail.module.css:33` (`.cta:focus-visible` — `outline: 2px solid var(--brand-primary)`).

**Patrón canónico del closure 2026-04-22 para elementos sobre fondo neutro:**
```css
:focus-visible {
  outline: 2px solid var(--brand-primary);
  outline-offset: 2px;
  box-shadow: var(--focus-ring);
}
```

Para elementos sobre fondo `var(--brand-primary)` (como ProfessionalShell header), se adapta a `outline: 2px solid var(--surface)` + `box-shadow: var(--focus-ring)`.

---

## T3A — Inspección contextual de `outline: none` sueltos

Para cada archivo con `outline: none` suelto, investigar si hay un `:focus-visible` cercano en el mismo componente que maneja el foco, o si el `outline: none` está anulando el ring accidentalmente.

**Decisión por archivo:**

1. **`AppState.module.css:78`**: leer contexto (5 líneas antes/después). Si está en un selector decorativo sin target de tab (p.ej., `img`, `svg`), mantener (no es bug). Si está en un elemento interactivo, agregar `:focus-visible` canónico.

2. **`VinculosForm.module.css:34`**: leer contexto. Probablemente en un input. Agregar `:focus-visible` con `outline: 2px solid var(--brand-primary); outline-offset: 2px; box-shadow: var(--focus-ring);`.

3. **`ExportGate.module.css:174`**: leer contexto. Similar.

4. **`InviteForm.module.css:29`**: leer contexto. Este archivo ya tiene `outline: 2px solid var(--brand-primary)` en líneas 66 y 160, así que el `:29` es probablemente un override específico. Verificar y ajustar.

5. **`TelegramPairingCard.module.css:301`**: leer contexto.

6. **`Timeline.module.css:223`**: leer contexto.

**Regla:** si el elemento es interactivo (`button`, `input`, `a`, `[role=button]`, `[tabindex=0]`), debe tener `:focus-visible` canónico. Si es decorativo (reset de browser default en elementos no-tab), mantener.

---

## T3B — Agregar `box-shadow: var(--focus-ring)` a ProfessionalShell y SaveRail

**`ProfessionalShell.module.css`:**

```diff
 .navLink:focus-visible {
   outline: 2px solid var(--surface);
   outline-offset: 2px;
+  box-shadow: var(--focus-ring);
 }

 .logoutButton:focus-visible {
   outline: 2px solid var(--surface);
   outline-offset: 2px;
+  box-shadow: var(--focus-ring);
 }
```

**`SaveRail.module.css`:**

```diff
 .cta:focus-visible {
   outline: 2px solid var(--brand-primary);
   outline-offset: 2px;
+  box-shadow: var(--focus-ring);
 }
```

---

## T3C — Verificar `--focus-ring` existe

Grep en `frontend/app/tokens.css` (o equivalente) para confirmar que `--focus-ring` está definido como variable CSS. Si no existe, bloquear wave y pedir decisión humana (no definir nueva variable sin autorización).

```bash
grep -n "\-\-focus-ring" frontend/app/tokens.css
```

---

## T3D — Verificación

```bash
npm --prefix frontend run typecheck  # exit 0 (no toca TS)
npm --prefix frontend run lint        # exit 0
```

**Grep zonas congeladas:**
```bash
git diff --name-only main..HEAD | grep -E "^frontend/(lib/auth/|app/api/|app/auth/|proxy\.ts|src/)"
# esperado: 0
```

**Grep post-fix de `outline: none` sueltos en elementos interactivos:**
```bash
# Ver que los gaps identificados ya no existen o tienen :focus-visible hermano
grep -n "outline: none" frontend/app/AppState.module.css frontend/components/patient/vinculos/VinculosForm.module.css frontend/components/professional/ExportGate.module.css frontend/components/professional/InviteForm.module.css frontend/components/patient/telegram/TelegramPairingCard.module.css frontend/components/professional/Timeline.module.css
```

---

## T3E — Commit W3

```bash
git add frontend/components/ui/ProfessionalShell.module.css
git add frontend/components/ui/SaveRail.module.css
git add frontend/app/AppState.module.css
git add frontend/components/patient/vinculos/VinculosForm.module.css
git add frontend/components/professional/ExportGate.module.css
git add frontend/components/professional/InviteForm.module.css
git add frontend/components/patient/telegram/TelegramPairingCard.module.css
git add frontend/components/professional/Timeline.module.css

git commit -m "$(cat <<'EOF'
style(w3-followups): focus ring normalize + outline none fix

Cierra follow-up #5 del closure login-flow-redesign 2026-04-23.

- ProfessionalShell.module.css y SaveRail.module.css: agregado box-shadow: var(--focus-ring) complementando el outline canonico (patron espejo del closure 2026-04-22).
- AppState, VinculosForm, ExportGate, InviteForm, TelegramPairingCard, Timeline: outline: none sueltos revisados; donde el elemento es interactivo, agregado :focus-visible canonico con outline + outline-offset + box-shadow.
- Zonas congeladas: 0 cruces. No tocan TS, solo CSS.

- Gabriel Paz -
EOF
)"
```
