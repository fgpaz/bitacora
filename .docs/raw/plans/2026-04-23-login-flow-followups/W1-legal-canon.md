# W1 — Legal-review + canon 23_uxui deltas 2026-04-23

**Tipo:** docs-only · **Subagente:** `ps-docs` · **Duración estimada:** 30-40 min
**Salida:** 1 archivo nuevo (decisión legal) + 6 archivos canon modificados.

---

## T1A — Legal-review de R-P1-3 (docs-only)

Crear `.docs/raw/decisiones/2026-04-23-legal-review-r-p1-3.md`.

**Shape obligatorio:**
- Metadato (fecha, rama, alcance, clasificación).
- Resumen ejecutivo con verdict (`resuelto-sin-cambios-bajo-recomendación` esperado).
- Wording bajo review literal (CTA `"Ahora no"`, mensaje post-decline, revocationNote).
- Análisis contra Ley 26.529 Art. 2, 5, 10 (autonomía, consent informado, revocabilidad).
- Análisis contra Ley 25.326 (protección de datos personales sensibles).
- Red flags detectados (o su ausencia).
- Recomendación de validación formal externa (abogado real antes de producción de largo plazo).
- Decisión operativa: cerrar R-P1-3 como `resuelto-sin-cambios-pending-formal-legal-opinion`.

**Ley 26.529 Art. 2 inc. e) autonomía:**
- CTA `"Ahora no"`: ofrece salida respetuosa sin estigma. ✓
- Post-decline message `"Podés aceptar cuando quieras. Tu sesión sigue activa."`: reafirma reversibilidad. ✓
- Sin coacción implícita (no hay timeout, no hay warnings amenazantes). ✓

**Ley 26.529 Art. 10 (revocabilidad):**
- revocationNote `"Podés revocarlo cuando quieras desde Mi cuenta."`: reafirma revocabilidad. ✓
- Consistencia con UI: ShellMenu trigger = `"Mi cuenta"` (ShellMenu.tsx:20). ✓
- Gap observado: no hay página dedicada de revocación de consent. Mitigación: backend ya soporta revoke; UI dedicada queda como P2 follow-up CON-002 (ya trackeado en INDEX.md 23_uxui).

**Ley 25.326:** sin cambios en storage, access control, audit. ✓

**Verdict esperado:** sin red flags hard. Wording coherente con los 3 artículos. Recomendación: validación formal con abogado del equipo antes de `deploy long-term`.

---

## T1B — Canon 23_uxui deltas 2026-04-23

Agregar sección `## Deltas 2026-04-23 — login flow redesign` al final de cada archivo, espejando el patrón ya presente `## Deltas 2026-04-22 — impeccable-hardening`.

**6 archivos a modificar:**

1. **`UXS-ONB-001.md`** — Deltas del slice de portada + consent:
   - Hero variant `"returning"`: cookie viva se detecta en Server Component (app/page.tsx), hero muestra `"Volviste."` + `"Seguí donde dejaste."` + CTA `"Seguir registrando"` → `/dashboard`. Rompe el anti-patrón de tratar al recurrente como primera vez.
   - Consent CTA secundario `"Ahora no"` (ConsentGatePanel.tsx:101-108) + handleDecline redirige a `/?declined=1` sin borrar cookie. Ley 26.529 Art. 2 autonomía.
   - Consent revocationNote `"Podés revocarlo cuando quieras desde Mi cuenta."` (ConsentGatePanel.tsx:97-99). Ley 26.529 Art. 10.
   - Post-decline message en landing: `"Podés aceptar cuando quieras. Tu sesión sigue activa."` via OnboardingEntryHero.heroMessage con `role=status aria-live=polite`.
   - Legal-review docs-only ejecutado (ver `.docs/raw/decisiones/2026-04-23-legal-review-r-p1-3.md`).
   - inviteHint reubicado DESPUÉS del `.sections` block (ConsentGatePanel.tsx:69-73) → anti-señal "vigilancia antes que control" resuelta.

2. **`VOICE-ONB-001.md`** — Deltas verbales:
   - Hero variant returning: `"Volviste."` (h1), `"Seguí donde dejaste."` (sub), `"Seguir registrando"` (CTA).
   - Consent CTA secundario: `"Ahora no"` (label canónico 2026-04-23).
   - Revocabilidad visible: `"Podés revocarlo cuando quieras desde Mi cuenta."`.
   - Post-decline landing message: `"Podés aceptar cuando quieras. Tu sesión sigue activa."` — sereno, sin dramatizar.

3. **`UJ-ONB-001.md`** — Delta del journey:
   - `S01` gana variante `S01-RETURNING` para paciente con cookie viva: pitch de captación reemplazado por puerta de retorno.
   - `S03` gana salida respetuosa (CTA `"Ahora no"`) sin romper el hard gate funcional de RF-CON-003.
   - Post-decline feedback loop landing → consent permanece reversible en la misma sesión.

4. **`UXS-REG-001.md`** — Deltas del slice de registro rápido:
   - MoodEntryDialog cierra automáticamente 800ms post-success (closeDialog auto tras setToastMsg).
   - Dashboard emite toast `role=status aria-live=polite` con `"Registro sumado a tu historial."` tras cierre del modal → Patrón 16 #12 "Puente de siguiente acción".
   - Puente embedded en MoodEntryForm: `"Volviendo al dashboard…"` (contraparte de `{!embedded && ...}` previo).

5. **`VOICE-REG-001.md`** — Deltas verbales del modal:
   - Toast post-success: `"Registro sumado a tu historial."` — factual, canon 13, sin celebración.
   - Puente embedded: `"Volviendo al dashboard…"`.

6. **`UXS-VIS-001.md`** — Deltas del dashboard:
   - `ready` state: rail de acción al TOP con CTA dominante `"+ Nuevo registro"` + secundario silencioso `"Check-in diario"`. Rompe el anti-patrón "CTA enterrado" (E3-F1 del audit 2026-04-23).
   - DashboardSummary `variant="compact"` en ready: 3 stat cards reemplazados por texto corrido sobrio. Canon 10 §12.2 "evitar tableros de vigilancia".
   - Heading h1 del dashboard: `"Hola. Acá está lo que registraste."` (saludo contextual) + subtítulo de privacidad `"Solo vos ves lo que registrás. Tus datos son privados."`. Canon 10 §refugio antes que archivo.
   - ShellMenu nuevo (`PatientPageShell` → `ShellMenu`) con overflow `⋯` (trigger `"Mi cuenta"`): items `"Recordatorios"`, `"Vínculos"`, separator, `"Cerrar sesión"` (destructive). aria-haspopup/aria-expanded + Escape + click-outside. Protege logout de taps accidentales.
   - trendChart `@media (max-width: 399px)`: 5 columnas con gap `var(--space-sm)` + `nth-child(n+6) { display: none }`. Reflow WCAG 1.4.10.

**Shape de cada sección delta:**
```markdown
## Deltas 2026-04-23 — login flow redesign

> 2026-04-23 — sync login flow redesign: deltas aplicados sobre implementación en rama `feature/login-flow-redesign-2026-04-23` (W1-W4), merged to main en commit `5d91158`. Fuente de verdad: `.docs/raw/reports/2026-04-23-login-flow-redesign-closure.md`.

### <Componente> — <tipo de cambio>

- <bullet> (W<n>).
- <bullet> (W<n>).
```

---

## T1C — Commit W1

```bash
git add .docs/raw/decisiones/2026-04-23-legal-review-r-p1-3.md
git add .docs/wiki/23_uxui/UXS/UXS-ONB-001.md
git add .docs/wiki/23_uxui/UXS/UXS-REG-001.md
git add .docs/wiki/23_uxui/UXS/UXS-VIS-001.md
git add .docs/wiki/23_uxui/VOICE/VOICE-ONB-001.md
git add .docs/wiki/23_uxui/VOICE/VOICE-REG-001.md
git add .docs/wiki/23_uxui/UJ/UJ-ONB-001.md
git add .docs/raw/plans/2026-04-23-login-flow-followups.md
git add .docs/raw/plans/2026-04-23-login-flow-followups/
git add .docs/raw/prompts/2026-04-23-login-flow-followups.md

git commit -m "$(cat <<'EOF'
docs(w1-followups): legal-review R-P1-3 + canon 23_uxui deltas 2026-04-23

Cierra follow-ups #1 y #2 del closure login-flow-redesign 2026-04-23.

- Legal-review docs-only de R-P1-3 (CTA "Ahora no" + mensaje post-decline + revocationNote) contra Ley 26.529 Art. 2/5/10 + Ley 25.326. Verdict: resuelto-sin-cambios-pending-formal-legal-opinion (sin red flags hard).
- Deltas 2026-04-23 agregados a UXS-ONB-001, UXS-REG-001, UXS-VIS-001, VOICE-ONB-001, VOICE-REG-001, UJ-ONB-001: variant returning hero, CTA Ahora no, revocationNote, rail dashboard, ShellMenu, DashboardSummary variant compact, cierre auto modal, toast.
- Plan wave-dispatchable + subdocs persistido.
- Prompt fuente committed como input canonico.

No tocar codigo en esta wave.

- Gabriel Paz -
EOF
)"
```

## T1D — Verificación

- `git diff --name-only main..HEAD | grep -E "^(frontend/)"` → 0 cruces (docs-only).
- `git diff --name-only main..HEAD | grep -v "^.docs/"` → 0 líneas (docs-only).
- Cada archivo canon tiene la sección `## Deltas 2026-04-23 — login flow redesign` bien formateada.
- Archivo decisión legal existe y es coherente.
