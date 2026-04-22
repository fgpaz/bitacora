# Task T3: Dashboard unificado con modal y banner Telegram

## Shared Context
**Goal:** Integrar la captura de mood entry en `/dashboard` vía `<dialog>` modal, y agregar un banner condicional de nudge a Telegram. El paciente nunca pierde el contexto del historial.
**Stack:** Next.js 16 App Router, React 19 `useEffect`/`useState`, CSS Modules, native `<dialog>`.
**Architecture:** Dashboard hace fetch paralelo timeline+summary. Refresh post-save vía nonce. Telegram banner lee `getTelegramSession()` y persiste dismiss en `localStorage` por 30 días.

## Locked Decisions
- Modal: `<dialog>` nativo (no librería). Escape y backdrop click cancelan.
- `MoodEntryForm` recibe prop `embedded: boolean` + `onSaved?: () => void`. Cuando `embedded=true` no se envuelve en `PatientPageShell` ni muestra links post-success.
- Banner: se muestra sólo si `session.linked === false` y no se despidió hace <30 días.
- Clave localStorage: `bitacora.telegram.banner.dismissedAt` (timestamp ms).
- `/registro/mood-entry` sigue operativa como deep-link de respaldo (no romper, p.ej. desde reminder de Telegram).
- `Dashboard.tsx` usa patrón `refreshNonce` (counter) para recargar tras save, evitando `react-hooks/set-state-in-effect`.
- Todo texto visible en español con tildes. Sin emojis.

## Task Metadata
```yaml
id: T3
depends_on: [T0]
agent_type: ps-next-vercel
files:
  - modify: frontend/components/patient/dashboard/Dashboard.tsx
  - modify: frontend/components/patient/mood/MoodEntryForm.tsx
  - create: frontend/components/patient/dashboard/MoodEntryDialog.tsx
  - create: frontend/components/patient/dashboard/MoodEntryDialog.module.css
  - create: frontend/components/patient/dashboard/TelegramReminderBanner.tsx
  - create: frontend/components/patient/dashboard/TelegramReminderBanner.module.css
  - read: frontend/components/patient/dashboard/DashboardSummary.tsx
  - read: frontend/components/patient/mood/MoodScale.tsx
  - read: frontend/lib/api/client.ts
complexity: high
done_when: "typecheck + lint exit 0; Dashboard.tsx referencia MoodEntryDialog y TelegramReminderBanner; botones '+ Nuevo registro' y 'Registrar humor' abren modal en lugar de navegar"
```

## Reference
- `frontend/components/patient/mood/MoodEntryForm.tsx` (main) — hoy envuelve en `PatientPageShell`. Refactor mínimo: soportar modo embedded.
- `frontend/lib/api/client.ts:206-215` — `getTelegramSession()` ya normaliza `linked` y `isLinked`.
- `frontend/lib/api/client.ts:513` — `getPatientTimeline(from, to)`.
- `frontend/lib/api/client.ts:544` — `getPatientSummary(from, to)`.
- `frontend/components/patient/dashboard/Dashboard.module.css` — clases existentes (`.stack`, `.emptyState`, `.primaryButton`, `.secondaryLink`, `.trendChart`, `.actions`). No redefinir.

## Prompt
Sos `ps-next-vercel`. Esta tarea tiene 3 piezas que deben quedar integradas:

1. **MoodEntryForm** debe aceptar `embedded` + `onSaved` sin romper la ruta `/registro/mood-entry`.
2. **MoodEntryDialog** envuelve `MoodEntryForm` en un `<dialog>` nativo accesible con título "Nuevo registro".
3. **TelegramReminderBanner** se renderiza en `Dashboard.tsx` sólo si el usuario no tiene Telegram vinculado y no despidió el banner reciente.
4. **Dashboard.tsx** reemplaza los `<Link href="/registro/mood-entry">` por botones que abren el modal. Mantiene el botón `Check-in diario` a `/registro/daily-checkin`.

Reglas duras:
- NO modificar `DashboardSummary.tsx`, `MoodScale.tsx`, ni el API client.
- NO agregar librerías (no recharts, no tremor, no headlessui). Usar `<dialog>` nativo.
- El CSS debe usar tokens existentes (`--brand-primary`, `--surface`, `--space-*`, `--radius-*`, etc.).
- NO usar `useCallback` envolviendo la función que hace fetch + setState en el mismo archivo (rompe `react-hooks/set-state-in-effect`). Usar patrón nonce.

## Execution Procedure
1. Leer `frontend/components/patient/mood/MoodEntryForm.tsx` completo. Confirmar que hoy devuelve `<PatientPageShell>…</PatientPageShell>` en todas las ramas.
2. Reescribir `MoodEntryForm.tsx` con el Skeleton A: recibir `embedded?: boolean` y `onSaved?: () => void`. Cuando `embedded=true`, no envolver en `PatientPageShell` y omitir links post-success (sólo mostrar "Registro guardado.").
3. Crear `frontend/components/patient/dashboard/MoodEntryDialog.tsx` con el Skeleton B.
4. Crear `frontend/components/patient/dashboard/MoodEntryDialog.module.css` con el Skeleton C.
5. Crear `frontend/components/patient/dashboard/TelegramReminderBanner.tsx` con el Skeleton D.
6. Crear `frontend/components/patient/dashboard/TelegramReminderBanner.module.css` con el Skeleton E.
7. Reescribir `frontend/components/patient/dashboard/Dashboard.tsx` con el Skeleton F. Detalles:
   - Agregar state `dialogOpen` y `refreshNonce`.
   - Mover la función `loadData` dentro del `useEffect` con `cancelled` flag; el effect depende de `[router, refreshNonce]`.
   - Renderizar `<TelegramReminderBanner />` arriba del `<DashboardSummary />` en ready y empty states.
   - Reemplazar `<Link href="/registro/mood-entry">` por `<button onClick={openDialog}>`.
   - Al final, `<MoodEntryDialog open={dialogOpen} onClose={closeDialog} onSaved={handleEntrySaved} />`.
8. `cd frontend && npm run typecheck && npm run lint`. Exit 0 en ambos.
9. Commit.

## Skeleton

Skeleton A — `frontend/components/patient/mood/MoodEntryForm.tsx` (cambios clave):
```tsx
interface Props {
  embedded?: boolean;
  onSaved?: () => void;
}

export function MoodEntryForm({ embedded = false, onSaved }: Props = {}) {
  // ... mismos states ...
  // En setPhase('success'): if (onSaved) onSaved();
  // Construir bloques (consentBlock/sessionBlock/successBlock/formBlock).
  // successBlock: si embedded=false, incluye los Link a /registro/daily-checkin y /dashboard.
  // Al final:
  //   const content = phase === 'consent' ? consentBlock : phase === 'session' ? sessionBlock : phase === 'success' ? successBlock : formBlock;
  //   if (embedded) return content;
  //   return <PatientPageShell>{content}</PatientPageShell>;
}
```

Skeleton B — `frontend/components/patient/dashboard/MoodEntryDialog.tsx`:
```tsx
'use client';

import { useEffect, useRef } from 'react';
import { MoodEntryForm } from '@/components/patient/mood/MoodEntryForm';
import styles from './MoodEntryDialog.module.css';

interface Props {
  open: boolean;
  onClose: () => void;
  onSaved: () => void;
}

export function MoodEntryDialog({ open, onClose, onSaved }: Props) {
  const dialogRef = useRef<HTMLDialogElement | null>(null);

  useEffect(() => {
    const dialog = dialogRef.current;
    if (!dialog) return;
    if (open && !dialog.open) dialog.showModal();
    else if (!open && dialog.open) dialog.close();
  }, [open]);

  return (
    <dialog
      ref={dialogRef}
      className={styles.dialog}
      onCancel={(e) => { e.preventDefault(); onClose(); }}
      onClick={(e) => { if (e.target === dialogRef.current) onClose(); }}
      aria-labelledby="mood-entry-dialog-title"
    >
      <div className={styles.panel}>
        <div className={styles.header}>
          <h2 id="mood-entry-dialog-title" className={styles.title}>Nuevo registro</h2>
          <button type="button" className={styles.closeBtn} onClick={onClose} aria-label="Cerrar">×</button>
        </div>
        <div className={styles.body}>
          <MoodEntryForm embedded onSaved={onSaved} />
        </div>
      </div>
    </dialog>
  );
}
```

Skeleton C — `MoodEntryDialog.module.css`:
```css
.dialog {
  border: none;
  background: transparent;
  padding: 0;
  max-width: min(560px, 100vw);
  width: 100%;
}
.dialog::backdrop { background: rgba(16,24,40,0.55); backdrop-filter: blur(2px); }
.panel { background: var(--surface); border-radius: 16px; box-shadow: 0 24px 48px rgba(16,24,40,0.18); overflow: hidden; display: flex; flex-direction: column; max-height: min(90vh, 720px); }
.header { display: flex; align-items: center; justify-content: space-between; padding: var(--space-md) var(--space-lg); border-bottom: 1px solid var(--border-subtle); }
.title { margin: 0; font-family: var(--font-display); font-size: 1.125rem; font-weight: 500; color: var(--foreground); }
.closeBtn { min-width: 40px; min-height: 40px; font-size: 1.5rem; line-height: 1; color: var(--foreground-muted); background: transparent; border: none; border-radius: var(--radius-sm); cursor: pointer; }
.closeBtn:hover { background: var(--surface-muted); color: var(--foreground); }
.closeBtn:focus-visible { outline: none; box-shadow: var(--focus-ring); }
.body { overflow-y: auto; padding: var(--space-lg); }
```

Skeleton D — `TelegramReminderBanner.tsx`:
```tsx
'use client';

import Link from 'next/link';
import { useEffect, useState } from 'react';
import { getTelegramSession } from '@/lib/api/client';
import styles from './TelegramReminderBanner.module.css';

const DISMISS_KEY = 'bitacora.telegram.banner.dismissedAt';
const DISMISS_WINDOW_MS = 30 * 24 * 60 * 60 * 1000;

function wasRecentlyDismissed(): boolean {
  if (typeof window === 'undefined') return false;
  const raw = window.localStorage.getItem(DISMISS_KEY);
  if (!raw) return false;
  const dismissedAt = Number.parseInt(raw, 10);
  if (Number.isNaN(dismissedAt)) return false;
  return Date.now() - dismissedAt < DISMISS_WINDOW_MS;
}

export function TelegramReminderBanner() {
  const [visible, setVisible] = useState(false);

  useEffect(() => {
    let cancelled = false;
    async function check() {
      if (wasRecentlyDismissed()) return;
      try {
        const s = await getTelegramSession();
        if (!cancelled && s.linked === false) setVisible(true);
      } catch { /* nudge silencioso */ }
    }
    check();
    return () => { cancelled = true; };
  }, []);

  function handleDismiss() {
    if (typeof window !== 'undefined') {
      window.localStorage.setItem(DISMISS_KEY, Date.now().toString());
    }
    setVisible(false);
  }

  if (!visible) return null;

  return (
    <aside className={styles.banner} role="complementary" aria-label="Conectar Telegram">
      <div className={styles.content}>
        <p className={styles.title}>Recibí recordatorios por Telegram</p>
        <p className={styles.description}>Tarda un minuto y te ayuda a no olvidar tu registro.</p>
      </div>
      <div className={styles.actions}>
        <Link href="/configuracion/telegram" className={styles.primary}>Conectar</Link>
        <button type="button" onClick={handleDismiss} className={styles.secondary}>Ahora no</button>
      </div>
    </aside>
  );
}
```

Skeleton E — `TelegramReminderBanner.module.css`: seguir los tokens del design system. Ver CSS completo en el decision doc `2026-04-22-dashboard-first-post-login.md`.

Skeleton F — cambios clave en `Dashboard.tsx`:
```tsx
import { useEffect, useState } from 'react';
import { MoodEntryDialog } from './MoodEntryDialog';
import { TelegramReminderBanner } from './TelegramReminderBanner';

// ...
const [dialogOpen, setDialogOpen] = useState(false);
const [refreshNonce, setRefreshNonce] = useState(0);

useEffect(() => {
  let cancelled = false;
  async function loadData() {
    try {
      /* ... fetch timeline+summary ... */
      if (cancelled) return;
      /* ... setTotalEntries, setAvgMoodScore, setEntries, setLastEntryAt, setViewState ... */
    } catch (err) {
      if (cancelled) return;
      /* ... error handling + router.push('/consent') si CONSENT_REQUIRED ... */
    }
  }
  loadData();
  return () => { cancelled = true; };
}, [router, refreshNonce]);

function handleEntrySaved() { setRefreshNonce((n) => n + 1); }
```

Botones (reemplazos):
- Empty state: `<button onClick={openDialog}>Registrar humor</button>` (clase `.primaryButton`).
- Ready actions: `<button onClick={openDialog}>+ Nuevo registro</button>` + `<Link href="/registro/daily-checkin">Check-in diario</Link>`.

## Verify
```bash
cd frontend
grep -l "MoodEntryDialog\|TelegramReminderBanner" components/patient/dashboard/Dashboard.tsx   # expected: 1 match
grep -l "/registro/mood-entry" components/patient/dashboard/Dashboard.tsx                     # expected: 0 matches (ya no navegamos desde el dashboard al formulario)
npm run typecheck   # exit 0
npm run lint        # exit 0
```

## Commit
```
feat(dashboard): modal de nuevo registro + banner Telegram condicional
```
