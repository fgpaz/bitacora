# Wave 1 — P0 Blockers (Redesign Login Flow 2026-04-23)

**Objetivo:** Resolver los 4 hallazgos P0 del audit que son bloqueantes pre-merge. Al finalizar W1, el recurrente que abra el dashboard DEBE ver el CTA de registro en el viewport inicial; el logout DEBE requerir dos taps deliberados; cualquier fallo catastrófico DEBE mostrar marca + CTA en lugar de pantalla cruda; y el modal DEBE cerrarse automáticamente tras guardar con un puente claro al dashboard actualizado.

**Scope**: 4 findings P0 (R-P0-1, R-P0-2, R-P0-3, R-P0-4). Surfaces tocados: dashboard, shell, global-error, modal+form.

**Restricción absoluta**: zero toque a `lib/auth/*`, `app/api/*`, `app/auth/*`, `proxy.ts`, `src/**`.

---

## T1A — R-P0-1: Rail de acción al top del Dashboard ready

**Skill líder:** `impeccable-distill` — rediseñar narrativa del ready para subir la acción dominante arriba del viewport.

**Archivos:**
- `frontend/components/patient/dashboard/Dashboard.tsx` — reordenar árbol DOM del `.stack` en `viewState === 'ready'`.
- `frontend/components/patient/dashboard/Dashboard.module.css` — CSS del nuevo rail y re-jerarquía.

**Evidencia base (Explorer 1):**
- Árbol DOM actual del ready (orden): `TelegramReminderBanner` → `DashboardSummary` → `section trendPanel` → `section recent-entries` → `section.actions` (CTA "+ Nuevo registro" es el último bloque).

**Cambio concreto:**
Agregar un nuevo bloque **"rail de acción personal"** arriba de `DashboardSummary`, con:
- Heading pequeño opcional tipo `"Hoy"` o etiqueta contextual serena (canon 13: sin elogio, sin coach).
- CTA dominante grande `"+ Nuevo registro"` (respeta texto actual; el congelado literal es "Registrar humor" en empty — en ready, `"+ Nuevo registro"` sigue siendo válido porque es el texto vigente del trigger con botón `ref={openDialogRef}`).
- Secundario silencioso `"Check-in diario"` como link (canon 12 §"acción dominante + secundaria silenciosa").

El `section.actions` actual al final del stack se **elimina** (queda absorbido por el rail superior). DashboardSummary + trendPanel + recentEntries quedan debajo como referencia.

**Pseudocódigo (pre-diff):**
```tsx
<div className={styles.stack}>
  {/* NUEVO: rail de acción arriba */}
  <section className={styles.actionRail} aria-label="Acciones de registro">
    <button
      ref={openDialogRef}
      type="button"
      onClick={openDialog}
      className={styles.primaryButton}
    >
      + Nuevo registro
    </button>
    <Link href="/registro/daily-checkin" className={styles.secondaryLink}>
      Check-in diario
    </Link>
  </section>

  <TelegramReminderBanner ... />
  <DashboardSummary ... />
  <section className={styles.trendPanel}>...</section>
  <section aria-labelledby="recent-entries-heading">...</section>
  {/* ELIMINADO: <section className={styles.actions}> al final */}
</div>
```

**Canon referenciado:**
- Manifesto 10 §4.1 "registrar sin fricción"; §5.1 frame de refugio.
- Lineamiento 12 §"una acción dominante"; §"acción dominante + secundaria silenciosa".
- Patrón 16 #3 "Gesto rápido de valor".

**Checkpoint T1A:**
- [ ] Read Dashboard.tsx post-cambio: rail visible arriba del `.stack`, viewport inicial en 360px muestra el CTA sin scroll.
- [ ] `section.actions` al final removido.
- [ ] Ref `openDialogRef` sigue apuntando al botón del rail (para focus return post-modal).
- [ ] E2E `dashboard-modal.spec.ts` pasa con el mismo selector `getByRole('button', { name: /Nuevo registro/ })` (text-based, tolera el cambio de posición).

**Commit propuesto:**
```
feat(w1-distill): R-P0-1 rail de accion con CTA + Nuevo registro al top del dashboard ready

Responde la queja literal del product owner: el recurrente nocturno en mobile
ahora ve el CTA de registro en el viewport inicial del dashboard, no al final
del scroll. El rail superior contiene la accion dominante (+ Nuevo registro)
y la secundaria silenciosa (Check-in diario). DashboardSummary, trendChart y
lista de recientes quedan debajo como referencia, no como bloqueo.

Responde audit 2026-04-23 finding E3-F1 P0. Canon 10 §4.1 "registrar sin
friccion" + Lineamiento 12 §"una accion dominante" + Patron 16 #3 "Gesto
rapido de valor".

Zero toque a zonas congeladas. Copy congelado preservado ("Registrar humor"
se mantiene como aria-label del trigger; "+ Nuevo registro" sigue siendo el
texto visible del CTA).

- Gabriel Paz -
```

---

## T1B — R-P0-2: ShellMenu con overflow `⋯` y logout protegido

**Skill líder:** `impeccable-harden` — proteger acción destructiva con fricción semántica.

**Archivos:**
- `frontend/components/ui/PatientPageShell.tsx` — reemplazar botón logout directo por `<ShellMenu>`.
- `frontend/components/ui/PatientPageShell.module.css` — ajustar layout del header.
- `frontend/components/ui/ShellMenu.tsx` — **componente nuevo**.
- `frontend/components/ui/ShellMenu.module.css` — **archivo nuevo**.

**Evidencia base (Explorer 1):**
- `PatientPageShell.tsx:41-49`: logout es el primer hijo de `<main>`, sin overflow menu, sin confirmación.
- Shell tiene solo 2 hijos: `logoutButton` + `content div`. No hay header intermedio.

**Cambio concreto:**
- Crear componente `ShellMenu` con:
  - Trigger: `<button aria-haspopup="menu" aria-expanded={open} aria-label="Mi cuenta">⋯</button>` con target ≥44×44.
  - Dropdown: `<ul role="menu" hidden={!open}>` con al menos `<li role="menuitem"><button onClick={signOut}>Cerrar sesión</button></li>`.
  - Cierre: Esc, click-outside, blur del último item.
  - Focus trap: foco ingresa al primer item al abrir; Shift+Tab sale al trigger; Tab desde último cierra y vuelve al trigger.
  - `prefers-reduced-motion` local para evitar animación de apertura si aplica.
- `PatientPageShell` reemplaza el botón logout directo por `<header class=shellHeader><ShellMenu>...</ShellMenu></header>`.
- Ítems del menu: inicialmente solo "Cerrar sesión". En T3B (Wave 3) se agregan "Recordatorios" y "Vínculos".

**Pseudocódigo ShellMenu.tsx:**
```tsx
'use client';
import { useEffect, useRef, useState } from 'react';
import styles from './ShellMenu.module.css';

interface Props {
  children: React.ReactNode; // items del menu
}

export function ShellMenu({ children }: Props) {
  const [open, setOpen] = useState(false);
  const triggerRef = useRef<HTMLButtonElement | null>(null);
  const menuRef = useRef<HTMLUListElement | null>(null);

  // Esc + click outside + focus trap...

  return (
    <div className={styles.menuContainer}>
      <button
        ref={triggerRef}
        type="button"
        className={styles.menuTrigger}
        aria-haspopup="menu"
        aria-expanded={open}
        aria-label="Mi cuenta"
        onClick={() => setOpen((v) => !v)}
      >
        <span aria-hidden="true">⋯</span>
      </button>
      <ul
        ref={menuRef}
        role="menu"
        hidden={!open}
        className={styles.menuList}
      >
        {children}
      </ul>
    </div>
  );
}

export function ShellMenuItem({ onClick, children }: { onClick: () => void; children: React.ReactNode }) {
  return (
    <li role="menuitem">
      <button type="button" onClick={onClick} className={styles.menuItem}>
        {children}
      </button>
    </li>
  );
}
```

**Pseudocódigo PatientPageShell.tsx:**
```tsx
<main className={styles.shell}>
  <header className={styles.shellHeader}>
    <ShellMenu>
      <ShellMenuItem onClick={handleLogout}>Cerrar sesión</ShellMenuItem>
      {/* futuro W3: <ShellMenuItem onClick={() => router.push('/configuracion/telegram')}>Recordatorios</ShellMenuItem> */}
    </ShellMenu>
  </header>
  <div className={styles.content}>{children}</div>
</main>
```

**Canon referenciado:**
- Lineamiento 12 §"pocas decisiones visibles a la vez"; §"acciones destructivas separadas".
- Manifesto 10 §5.2 capa conductual ("¿Estoy aceptando algo sin darme cuenta?").

**Checkpoint T1B:**
- [ ] ShellMenu nuevo tiene `aria-haspopup/aria-expanded` correctos.
- [ ] Esc cierra el menu y retorna focus al trigger.
- [ ] Click-outside cierra el menu.
- [ ] 2 taps deliberados (`⋯` → `Cerrar sesión`) son necesarios para hacer logout.
- [ ] E2E nuevo (si se agrega en W4): abrir menu, confirmar ítem presente, simular Esc y verificar cierre.

**Commit propuesto:**
```
feat(w1-harden): R-P0-2 ShellMenu con overflow para proteger logout accidental

Sustituye el boton 1-click de cierre de sesion (posicionado como primer tap
del shell) por un menu de overflow con icono horizontal de tres puntos y aria-haspopup=menu.
Requiere dos taps deliberados para cerrar sesion, habilitando ademas la
incorporacion futura de "Recordatorios" y "Vinculos" (R-P1-6 Wave 3) sin
redisen estructural.

Responde audit 2026-04-23 finding E3-F6 P0. Lineamiento 12 §"pocas decisiones
visibles" + §"acciones destructivas separadas". Manifesto 10 §5.2 capa
conductual.

A11y: focus trap al abrir (primer item); Esc cierra + retorna focus al trigger;
click-outside cierra. aria-haspopup/aria-expanded actualizados.

Zero toque a lib/auth/* (handleLogout sigue invocando signOut() del mismo
import existente).

- Gabriel Paz -
```

---

## T1C — R-P0-3: Crear `frontend/app/global-error.tsx`

**Skill líder:** `impeccable-harden` — refugio de marca para errores catastróficos.

**Archivos:**
- `frontend/app/global-error.tsx` — **archivo nuevo**.

**Evidencia base (Explorer 3):**
- Glob `frontend/app/global-error.tsx` → 0 resultados. Confirmado ausente.

**Cambio concreto:**
Crear el archivo siguiendo el contrato de Next.js 16 App Router: `'use client'` + wrapper `<html><body>`.

**Pseudocódigo completo:**
```tsx
'use client';

import { useEffect } from 'react';

export default function GlobalError({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  useEffect(() => {
    // log opcional; no exponer error.message al usuario
  }, [error]);

  return (
    <html lang="es">
      <body
        style={{
          minHeight: '100vh',
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          padding: '24px',
          fontFamily:
            "'Source Sans 3', system-ui, sans-serif",
          backgroundColor: '#F6F1EA',
          color: '#2E2A28',
        }}
      >
        <main style={{ maxWidth: 480, textAlign: 'center' }}>
          <p
            style={{
              fontFamily: "'Newsreader', Georgia, serif",
              fontSize: '1.75rem',
              fontWeight: 600,
              marginBottom: '8px',
            }}
          >
            Bitácora
          </p>
          <h1
            style={{
              fontFamily: "'Newsreader', Georgia, serif",
              fontSize: '1.5rem',
              marginTop: '24px',
              marginBottom: '16px',
            }}
          >
            No pudimos cargar el sitio.
          </h1>
          <p style={{ color: '#4A4440', marginBottom: '24px' }}>
            Probá recargar la página. Si el problema continúa, escribinos desde tu correo.
          </p>
          <button
            type="button"
            onClick={() => reset()}
            style={{
              minHeight: 44,
              minWidth: 120,
              padding: '12px 24px',
              borderRadius: 8,
              border: 'none',
              backgroundColor: '#5E766E',
              color: '#FFFFFF',
              fontWeight: 600,
              cursor: 'pointer',
            }}
          >
            Recargar
          </button>
        </main>
      </body>
    </html>
  );
}
```

**Nota**: este archivo usa estilos inline porque los CSS Modules NO están disponibles en `global-error.tsx` (el layout root falló, por lo que no podemos depender del bundle principal). Los valores hardcoded son espejo exacto de `tokens.css` (surface `#F6F1EA`, foreground `#2E2A28`, brand-primary `#5E766E`, radius-md 8, foreground-muted `#4A4440`).

**Canon referenciado:**
- Canon 13 §Errores — título claro ("No pudimos cargar el sitio") + sub accionable + CTA.
- Manifesto 10 §refugio sereno — wordmark presente en momento más sensible.
- Patrón 16 #7 "Error recuperable localizado".

**Checkpoint T1C:**
- [ ] `frontend/app/global-error.tsx` existe.
- [ ] `'use client'` directive presente.
- [ ] `<html>/<body>` wrapper presente.
- [ ] Wordmark "Bitácora" visible.
- [ ] CTA `Recargar` invoca `reset()`.
- [ ] typecheck + lint pasan.

**Commit propuesto:**
```
feat(w1-harden): R-P0-3 global-error.tsx con refugio de marca ante error root

Crea app/global-error.tsx (archivo nuevo) que envuelve html+body para el
contrato Next.js App Router de error en layout root. Renderiza wordmark
"Bitacora" + titulo canon 13 ("No pudimos cargar el sitio.") + sub concreto
("Proba recargar la pagina. Si el problema continua, escribinos desde tu
correo.") + CTA "Recargar" que invoca reset().

Responde audit 2026-04-23 finding E5-F4 P0. Manifesto 10 §refugio sereno +
Canon 13 §Errores + Patron 16 #7.

Estilos inline espejo de tokens.css (surface F6F1EA, brand-primary 5E766E,
foreground 2E2A28, radius 8, foreground-muted 4A4440) porque CSS Modules no
estan disponibles cuando el layout root falla.

- Gabriel Paz -
```

---

## T1D — R-P0-4: Cierre modal post-success + puente embedded

**Skill líder:** `impeccable-clarify` + `impeccable-harden` — microcopy + resiliencia del cierre.

**Archivos:**
- `frontend/components/patient/dashboard/MoodEntryDialog.tsx` — en `handleSaved`, llamar `onClose()` tras delay corto (~800ms).
- `frontend/components/patient/mood/MoodEntryForm.tsx` — agregar puente condensado en modo embedded.
- `frontend/components/patient/mood/MoodEntryForm.module.css` — estilo del puente embedded.
- `frontend/components/patient/dashboard/Dashboard.tsx` — toast `role=status aria-live=polite` tras cierre del modal.

**Evidencia base (Explorer 2):**
- `MoodEntryDialog.tsx:41-43` — `handleSaved` solo llama `onSaved()`, no `onClose()`.
- `MoodEntryForm.tsx:87-94` — `{!embedded && ...}` excluye el puente en modo modal.
- `Dashboard.tsx:109-120` — funciones `openDialog/closeDialog/handleEntrySaved` accesibles.
- Explorer 2 confirma que no existe `useToast` ni infraestructura de toast global — el toast se implementa ad-hoc con `role=status aria-live=polite` dentro del Dashboard.

**Cambio concreto:**
1. **MoodEntryDialog.handleSaved**: tras invocar `onSaved()`, esperar ~800ms y luego `onClose()`. Eso deja tiempo para que AT lea "Registro guardado." (`role=status aria-live=polite` ya presente en MoodEntryForm.tsx:85).
2. **MoodEntryForm (modo embedded)**: agregar puente condensado sobrio dentro del `successBlock`:
   ```tsx
   {embedded && (
     <p className={styles.embeddedBridge}>
       Volviendo al dashboard…
     </p>
   )}
   ```
   (Texto editorial, no celebratorio. Canon 13 §confirmaciones serenas.)
3. **Dashboard**: tras cierre del modal, mostrar toast con mensaje canon 13 `"Registro sumado a tu historial."`. El toast vive como estado local del Dashboard:
   ```tsx
   const [toastMsg, setToastMsg] = useState<string | null>(null);

   function handleEntrySaved() {
     setRefreshNonce((n) => n + 1);
     setToastMsg('Registro sumado a tu historial.');
     setTimeout(() => setToastMsg(null), 4000);
   }
   ```
   Render del toast (abajo del stack, `position: fixed bottom + role=status aria-live=polite`):
   ```tsx
   {toastMsg && (
     <div className={styles.toast} role="status" aria-live="polite">
       {toastMsg}
     </div>
   )}
   ```

**Canon referenciado:**
- Patrón 16 #12 "Puente de siguiente acción"; #6 "Confirmación factual breve".
- Canon 13 §confirmaciones ("Registro guardado." + "Registro sumado a tu historial." ambos son factuales, sin elogio).

**Checkpoint T1D:**
- [ ] Modal cierra ~800ms post-success; `openDialogRef.current?.focus()` corre en `closeDialog()` preservando focus return.
- [ ] Modo embedded muestra "Volviendo al dashboard…" breve.
- [ ] Dashboard muestra toast "Registro sumado a tu historial." tras cierre.
- [ ] Toast tiene `role=status aria-live=polite`.
- [ ] 12 positivos del closure 2026-04-22 preservados (focus trap, aria-modal, focus return, role=radiogroup, etc.).

**Commit propuesto:**
```
feat(w1-clarify+harden): R-P0-4 cierre auto modal post-success + puente + toast

MoodEntryDialog.handleSaved ahora espera ~800ms tras onSaved() para dar tiempo
a AT de leer "Registro guardado." (role=status aria-live=polite ya presente)
y luego invoca onClose(). El focus return via requestAnimationFrame preserva
el comportamiento del closure 2026-04-22.

MoodEntryForm en modo embedded muestra un puente condensado sereno
("Volviendo al dashboard...") canon 13.

Dashboard handleEntrySaved ahora tambien setea un toast con mensaje canon
13 ("Registro sumado a tu historial.") que desaparece en 4s. Toast tiene
role=status aria-live=polite, sin infraestructura de toast global (no deps
nuevas).

Responde audit 2026-04-23 findings E4-F1 + E4-F2 P0 combinado. Patron 16 #12
"Puente de siguiente accion" + #6 "Confirmacion factual breve". Canon 13
§confirmaciones serenas.

Preserva los 12 positivos del closure 2026-04-22 (focus trap, aria-modal,
focus return, role=radiogroup en MoodScale, aria-busy en submit, etc.).

- Gabriel Paz -
```

---

## T1E — Cierre W1: review + typecheck + lint + e2e + grep zonas congeladas

**Skill líder:** `ps-code-reviewer` + `ps-worker`.

**Pasos:**
1. Dispatch `ps-code-reviewer` con el diff completo de W1 (4 commits); prioridad Performance > Diseño > Seguridad.
2. Correr `cd frontend && npm run typecheck && npm run lint && npm run test:e2e` — exit 0 obligatorio.
3. Grep final: `grep -rE "^(frontend/lib/auth/|frontend/app/api/|frontend/app/auth/|frontend/proxy\.ts|frontend/src/)" <archivos_tocados_w1>` → debe devolver 0 matches.
4. Reportar verdict + cerrar checkpoint humano.

**Done when:**
- ps-code-reviewer devuelve veredicto APROBADO o issues menores sin bloqueantes.
- typecheck + lint + 8/8 e2e verde.
- Grep de zonas congeladas = 0.
- Diff final de W1 revisado por el humano en el checkpoint.

---

## Checkpoint humano W1

Post-W1, pausar y reportar al humano:
- Cadena de commits W1 (hashes + subject).
- Diff summary: archivos tocados + archivos nuevos creados.
- Verdict de `ps-code-reviewer`.
- Resultado tests: typecheck + lint + e2e.
- Grep zonas congeladas: 0 cruces confirmado.
- Screenshot ASCII del dashboard ready (viewport inicial) con el rail arriba (si cabe en contexto).

Esperar OK humano explícito antes de empezar W2.

---

## Zonas congeladas — verificación obligatoria

Ningún archivo de W1 puede tocar:
- `frontend/lib/auth/*`
- `frontend/app/api/*`
- `frontend/app/auth/*`
- `frontend/proxy.ts`
- `frontend/src/**`

Archivos W1 permitidos:
- `frontend/app/global-error.tsx` ✓ (archivo nuevo, fuera de zonas prohibidas)
- `frontend/components/patient/dashboard/Dashboard.tsx` + `.module.css` ✓
- `frontend/components/patient/dashboard/MoodEntryDialog.tsx` ✓
- `frontend/components/patient/mood/MoodEntryForm.tsx` + `.module.css` ✓
- `frontend/components/ui/PatientPageShell.tsx` + `.module.css` ✓
- `frontend/components/ui/ShellMenu.tsx` + `.module.css` (nuevos) ✓
